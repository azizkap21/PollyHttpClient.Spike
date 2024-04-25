using JetBrains.Annotations;
using Poq.Api.Utility.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PollyHttpClientCore
{
    public interface ICustomHttpClient
    {
        [ItemNotNull]
        Task<PoqHttpResponse<TResponse>> GetAsync<TResponse>(Uri uri, [NotNull]IEnumerable<PoqHttpCookie> cookies, [NotNull]IEnumerable<PoqHttpHeader> headers, IResponseParser<TResponse> resultParser)
            where TResponse : class;

        [ItemNotNull]
        Task<PoqHttpResponse<TResponse>> GetJsonAsync<TResponse>(Uri uri, [NotNull]IEnumerable<PoqHttpCookie> cookies, [NotNull]IEnumerable<PoqHttpHeader> headers)
           where TResponse : class;
        [ItemNotNull]
        Task<PoqHttpResponse<T>> PostAsync<T>(Uri uri, [NotNull]IEnumerable<PoqHttpCookie> cookies, [NotNull]IEnumerable<PoqHttpHeader> headers, [CanBeNull]List<KeyValuePair<string, string>> postData, IResponseParser<T> resultParser)
                    where T : class;

        [ItemNotNull]
        Task<PoqHttpResponse> SendJsonAsync<TRequest>(HttpMethod method, Uri uri, [NotNull]IEnumerable<PoqHttpCookie> cookies, [NotNull]IEnumerable<PoqHttpHeader> headers, TRequest body = null)
            where TRequest : class;

        [ItemNotNull]
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
}
