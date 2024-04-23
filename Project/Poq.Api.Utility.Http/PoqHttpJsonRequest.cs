using System.Collections.Generic;
using System.Net.Http;

namespace Poq.Api.Utility.Http
{
    public class PoqHttpJsonRequest<TJsonBody> : PoqHttpRequest
    {
        public TJsonBody Body { get; set; }
        public bool IgnoreNullValues { get; set; }

        public PoqHttpJsonRequest(HttpMethod method, string url, TJsonBody body)
            : this(method, url, null, null, body)
        {
        }

        public PoqHttpJsonRequest(HttpMethod method, string url, List<PoqHttpCookie> cookies, TJsonBody body)
            : this(method, url, cookies, null, body)
        {
        }

        public PoqHttpJsonRequest(HttpMethod method, string url, List<PoqHttpCookie> cookies, List<PoqHttpHeader> headers, TJsonBody body)
            : base(method, url, cookies, headers)
        {
            Body = body;
        }
    }
}
