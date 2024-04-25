using System;

namespace Poq.Api.Utility.Http.ResponseParsers
{
    public class ResponseParser<T> : IResponseParser<T> where T : class
    {
        private readonly Func<string, T> _parseFunc;

        public ResponseParser(Func<string, T> parseFunc)
        {
            _parseFunc = parseFunc ?? throw new ArgumentNullException(nameof(parseFunc));
        }

        public T Parse(string result)
        {
            return _parseFunc(result);
        }
    }
}
