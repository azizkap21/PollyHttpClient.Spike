using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Poq.Api.Utility.Service;

namespace Poq.Api.Utility.Http
{
    public class PoqHttpResponse
    {
        private List<PoqHttpCookie> _cookies;

        public List<PoqHttpCookie> Cookies
        {
            [NotNull]
            get { _cookies = _cookies ?? new List<PoqHttpCookie>(); return _cookies; }
            set { _cookies = value; }
        }

        public TimeSpan Duration { get; set; }

        private List<PoqHttpHeader> _headers;

        public List<PoqHttpHeader> Headers
        {
            [NotNull]
            get { _headers = _headers ?? new List<PoqHttpHeader>(); return _headers; }
            set { _headers = value; }
        }

        public Exception Exception { get; set; }

        [CanBeNull]
        public PoqHttpRequest Request { get; set; }

        [CanBeNull]
        public string ResultAsString { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public PoqHttpResponse Clone()
        {
            return new PoqHttpResponse
            {
                _cookies = _cookies,
                _headers = _headers,
                Duration = Duration,
                Exception = Exception,
                Request = Request,
                ResultAsString = ResultAsString,
                StatusCode = StatusCode,
            };
        }

        [NotNull]
        public Result ToResult()
        {
            var success =
                (int)StatusCode >= 200 && (int)StatusCode <= 299 &&
                Exception == null;

            if (success) return Result.Successful;

            var poqResponse = GetPoqResponseForError();
            return Result.Failure(GetReason(), poqResponse.Message, poqResponse.ErrorMessage);
        }

        protected Response GetPoqResponseForError()
        {
            var errorMessage = $"Http call to {Request?.Url} resulted in {StatusCode}.";
            string message;

            if (TryGetPoqResponse(out var poqResponse))
            {
                errorMessage += "\r\n\r\n" + poqResponse.ErrorMessage;
                message = poqResponse.Message;
            }
            else
            {
                message = "Something went wrong.";
            }

            if (Exception != null)
            {
                errorMessage += "\r\n\r\n" + Exception;
            }

            return new Response { ErrorMessage = errorMessage, Message = message };
        }

        protected bool TryGetPoqResponse(out Response poqResponse)
        {
            try
            {
                poqResponse = JsonConvert.DeserializeObject<Response>(ResultAsString);
                return poqResponse != null;
            }
            catch (Exception)
            {
                poqResponse = null;
                return false;
            }
        }

        protected string CreateErrorMessage()
        {
            var message = $"Http call to {Request?.Url} resulted in {StatusCode}.";

            if (Exception == null)
            {
                return message;
            }

            return message + "\r\n\r\n" + Exception;
        }

        protected Reason GetReason()
        {
            if ((int)StatusCode >= 200 && (int)StatusCode <= 299)
            {
                switch (Exception)
                {
                    case null:
                        return Reason.Success;

                    case SerializationException _:
                        return Reason.SerializationFailed;
                }

                return Reason.Unknown;
            }

            switch (StatusCode)
            {
                case HttpStatusCode.NotFound: return Reason.NotFound;
                case HttpStatusCode.Forbidden: return Reason.Forbidden;
                case HttpStatusCode.BadRequest: return Reason.BadRequest;
                case HttpStatusCode.Unauthorized: return Reason.Unauthorized;
                default: return Reason.Unknown;
            }
        }
    }
}
