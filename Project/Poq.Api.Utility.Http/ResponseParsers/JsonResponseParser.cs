using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Poq.Api.Utility.Http.ResponseParsers
{
    public class JsonResponseParser<TResponse> : IResponseParser<TResponse> where TResponse : class
    {
        [CanBeNull]
        public TResponse Parse([CanBeNull]string result)
        {
            return string.IsNullOrEmpty(result) ?
                null :
                JsonConvert.DeserializeObject<TResponse>(result);
        }
    }
}
