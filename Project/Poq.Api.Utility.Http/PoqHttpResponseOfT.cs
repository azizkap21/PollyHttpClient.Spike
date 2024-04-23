using JetBrains.Annotations;
using Poq.Api.Utility.Service;

namespace Poq.Api.Utility.Http
{
    public class PoqHttpResponse<TResponse> : PoqHttpResponse
    {
        [CanBeNull]
        public TResponse Result { get; set; }

        public new PoqHttpResponse<TResponse> Clone()
        {
            return new PoqHttpResponse<TResponse>
            {
                Cookies = Cookies,
                Headers = Headers,
                Duration = Duration,
                Exception = Exception,
                Request = Request,
                ResultAsString = ResultAsString,
                Result = Result,
                StatusCode = StatusCode,
            };
        }

        [NotNull]
        public new Result<TResponse> ToResult()
        {
            var success =
                (int)StatusCode >= 200 && (int)StatusCode <= 299 &&
                Exception == null;

            if (success) return Result<TResponse>.Successful(Result);

            var poqResponse = GetPoqResponseForError();
            return Result<TResponse>.Failure(GetReason(), poqResponse.Message, poqResponse.ErrorMessage);
        }
    }
}
