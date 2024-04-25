using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
//using System.Xml;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Poq.Api.Utility.Http;
using Poq.Api.Utility.Http.Cookie;
using Poq.Api.Utility.Http.ResponseParsers;

namespace PollyHttpClientCore
{
    public class CustomHttpClient : ICustomHttpClient
    {
        HttpClient _httpClient;

        private static Dictionary<string, object> _jsonResponseParsers;
        private static Dictionary<string, object> JsonResponseParsers
        {
            get { return _jsonResponseParsers = _jsonResponseParsers ?? new Dictionary<string, object>(); }
        }

        public CustomHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PoqHttpResponse<TResponse>> GetAsync<TResponse>(Uri uri, [NotNull] IEnumerable<PoqHttpCookie> cookies, [NotNull] IEnumerable<PoqHttpHeader> headers, IResponseParser<TResponse> resultParser) where TResponse : class
        {
            return await SendAsync(HttpMethod.Get, uri, cookies, headers, null, null, resultParser);
        }
               

        public async Task<PoqHttpResponse<TResponse>> GetJsonAsync<TResponse>(Uri uri, IEnumerable<PoqHttpCookie> cookies, IEnumerable<PoqHttpHeader> headers) where TResponse : class
        {
            var jsonResponseParser = GetJsonResponseParser<TResponse>();
            return await SendAsync(HttpMethod.Get, uri, cookies, headers, null, null, jsonResponseParser);
        }

        public async Task<PoqHttpResponse<T>> PostAsync<T>(Uri uri, IEnumerable<PoqHttpCookie> cookies, IEnumerable<PoqHttpHeader> headers, List<KeyValuePair<string, string>> postData, IResponseParser<T> resultParser = null) where T : class
        {
            return await PostAsync(uri, new PoqHttpCookie[0], headers, postData, resultParser);
        }

        public async Task<PoqHttpResponse> SendJsonAsync<TRequest>(HttpMethod method, Uri uri, [NotNull] IEnumerable<PoqHttpCookie> cookies, [NotNull] IEnumerable<PoqHttpHeader> headers, TRequest body = null) where TRequest : class
        {
            var jsonContent = GetJsonStringContent(body);
            return await SendAsync(method, uri, cookies, headers, MediaType.Json, jsonContent);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {            
            return await _httpClient.SendAsync(request);
        }

        public async Task<PoqHttpResponse<TResponse>> SendAsync<TResponse>(HttpMethod method, Uri uri, [CanBeNull] IEnumerable<PoqHttpCookie> cookies,
           [CanBeNull] IEnumerable<PoqHttpHeader> headers, string mediaType, string content, IResponseParser<TResponse> resultParser) where TResponse : class
        {
            var cookiesAsArray = cookies as PoqHttpCookie[] ?? cookies?.ToArray();
            var headersAsArray = headers as PoqHttpHeader[] ?? headers?.ToArray();

            var response = await SendAsync(method, uri, cookiesAsArray, headersAsArray, mediaType, content);

            TResponse result = null;
            Exception serializeException = null;

            if (response.Exception == null && resultParser != null)
            {
                TryDeserialize(response.ResultAsString, resultParser, out result, out serializeException);

                if (serializeException != null)
                {
                    LogException(response.Request, serializeException);
                }
            }

            var poqHttpResponse = new PoqHttpResponse<TResponse>
            {
                Cookies = response.Cookies,
                Duration = response.Duration,
                Exception = response.Exception ?? serializeException,
                Headers = response.Headers,
                Request = new PoqHttpRequest(method, uri.ToString(), cookiesAsArray, headersAsArray, content),
                Result = result,
                ResultAsString = response.ResultAsString,
                StatusCode = response.StatusCode,
            };

            return poqHttpResponse;
        }

        public async Task<PoqHttpResponse> SendAsync(
            HttpMethod method, Uri uri, IEnumerable<PoqHttpCookie> cookies, IEnumerable<PoqHttpHeader> headers,
            string mediaType, string content)
        {
            var request = new HttpRequestMessage(method, uri);

            if (content != null)
            {
                if (method == HttpMethod.Get)
                {
                    var separator = uri.Query == string.Empty ? "?" : "&";
                    var uriWithParams = new Uri($"{request.RequestUri}{separator}{content}");
                    request.RequestUri = uriWithParams;
                }
                else
                {
                    request.Content = new StringContent(content, Encoding.UTF8, mediaType);
                }
            }

            foreach (var header in headers ?? new PoqHttpHeader[0])
            {
                request.Headers.Add(header.Name, header.Value);
            }

            var cookiesAsArray = cookies?.ToArray();
            var cookieHeader = cookies == null ? string.Empty : string.Join(";", cookiesAsArray.Select(c => c.Name + "=" + c.Value));
            if (!string.IsNullOrEmpty(cookieHeader)) request.Headers.Add("Cookie", cookieHeader);

            var poqHttpRequest = new PoqHttpRequest(method, uri.ToString(), cookiesAsArray, headers, content);

            var (response, exception, elapsed) = await TrySendRequestAsync(request, ex => LogException(poqHttpRequest, ex));

            var resultAsString = response?.Content != null ? await response.Content.ReadAsStringAsync() : null;
            var responseCookies = CookieReader.ReadCookiesFromResponseHeaders(response?.Headers);
            var responseHeaders = response?.Headers.Select(h => new PoqHttpHeader(h.Key, string.Join(",", h.Value))).ToList();

            var poqHttpResponse = new PoqHttpResponse
            {
                Cookies = responseCookies,
                Duration = elapsed,
                Exception = exception,
                Headers = responseHeaders,
                Request = poqHttpRequest,
                ResultAsString = resultAsString,
                StatusCode = response?.StatusCode ?? HttpStatusCode.InternalServerError,
            };

            LogRequest(poqHttpRequest, poqHttpResponse);

            return poqHttpResponse;
        }

        private async Task<Tuple<HttpResponseMessage, Exception, TimeSpan>> TrySendRequestAsync(
            HttpRequestMessage request, Action<Exception> logExceptionAction)
        {
            Exception exception;
            HttpResponseMessage response;

            var stopwatch = Stopwatch.StartNew();

            try
            {
                response = await _httpClient.SendAsync(request);
                exception = null;
            }
            catch (TaskCanceledException ex)
            {
                // https://stackoverflow.com/questions/29179848/httpclient-a-task-was-cancelled
                exception = !ex.CancellationToken.IsCancellationRequested
                    ? new TimeoutException($"Timeout (elapsed {stopwatch.Elapsed.TotalMilliseconds} ms) while making a request to {request.RequestUri}.", ex)
                    : (Exception)ex;

                response = null;
                logExceptionAction(exception);
            }
            catch (Exception ex)
            {
                exception = ex;
                response = null;
                logExceptionAction(exception);
            }

            stopwatch.Stop();

            return new Tuple<HttpResponseMessage, Exception, TimeSpan>(response, exception, stopwatch.Elapsed);
        }

        private static void TryDeserialize<T>(string resultAsString, IResponseParser<T> resultParser, out T result, out Exception exception) where T : class
        {
            if (typeof(T) == typeof(string))
            {
                result = resultAsString as T;
                exception = null;
                return;
            }

            if (string.IsNullOrEmpty(resultAsString))
            {
                result = null;
                exception = null;
                return;
            }

            try
            {
                result = resultParser.Parse(resultAsString);
            }
            catch (Exception ex)
            {
                result = null;

                exception = new SerializationException($"{typeof(IResponseParser<T>)} was used for deserializing http response but an exception was thrown. " +
                    $"The data being deserialised was:\n\n{resultAsString}", ex);

                return;
            }

            if (result == null)
            {
                exception = new SerializationException($"{typeof(IResponseParser<T>)} was used for deserializing http response but the deserialized object is null. " +
                    $"The data being deserialised was:\r\n{resultAsString}");
                return;
            }

            exception = null;
        }

        private static string GetJsonStringContent<T>(T data, bool ignoreNullValues = false) where T : class
        {
            return
                data == null ? null :
                !ignoreNullValues ? JsonConvert.SerializeObject(data) :
                JsonConvert.SerializeObject(data, Formatting.None,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
        }

        private static async Task<string> GetUrlEncodedContent(List<KeyValuePair<string, string>> data)
        {
            if (data == null || data.Count == 0)
            {
                return null;
            }

            var urlEncodedContent = new FormUrlEncodedContent(data);
            return await urlEncodedContent.ReadAsStringAsync();
        }

        private static IResponseParser<T> GetJsonResponseParser<T>()
            where T : class
        {
            var typeName = typeof(T).FullName ?? string.Empty;

            if (!JsonResponseParsers.TryGetValue(typeName, out var responseParserAsObject))
            {
                var newResponseParser = new JsonResponseParser<T>();
                JsonResponseParsers[typeName] = newResponseParser;
                return newResponseParser;
            }

            if (responseParserAsObject is JsonResponseParser<T> responseParser)
            {
                return responseParser;
            }

            responseParser = new JsonResponseParser<T>();
            JsonResponseParsers[typeName] = responseParser;
            return responseParser;
        }

        private void LogRequest(PoqHttpRequest request, PoqHttpResponse response)
        {
            
        }

        private void LogException(PoqHttpRequest request, Exception ex)
        {
            //var requestToLog = request.Clone();

            //if (_options.SkipLoggingRequestBodies)
            //{
            //    requestToLog.ContentAsString = null;
            //}

            //_httpTracker?.LogWebRequestException(requestToLog, ex);
        }

       
    }
}

