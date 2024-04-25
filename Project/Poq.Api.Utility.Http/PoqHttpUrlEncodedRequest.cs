using System.Collections.Generic;
using System.Net.Http;

namespace Poq.Api.Utility.Http
{
    public class PoqHttpUrlEncodedRequest : PoqHttpRequest
    {
        private List<KeyValuePair<string, string>> _parameters;

        public List<KeyValuePair<string, string>> Parameters
        {
            get { _parameters = _parameters ?? new List<KeyValuePair<string, string>>(); return _parameters; }
            set { _parameters = value; }
        }

        public PoqHttpUrlEncodedRequest(HttpMethod method, string url, List<KeyValuePair<string, string>> parameters)
            : this(method, url, null, null, parameters)
        {
        }

        public PoqHttpUrlEncodedRequest(HttpMethod method, string url, List<PoqHttpCookie> cookies, List<KeyValuePair<string, string>> parameters)
            : this(method, url, cookies, null, parameters)
        {
        }

        public PoqHttpUrlEncodedRequest(HttpMethod method, string url, List<PoqHttpCookie> cookies, List<PoqHttpHeader> headers, List<KeyValuePair<string, string>> parameters)
            : base(method, url, cookies, headers)
        {
            Parameters = parameters;
        }
    }
}
