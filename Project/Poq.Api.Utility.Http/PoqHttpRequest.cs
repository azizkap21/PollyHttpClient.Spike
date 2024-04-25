using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using JetBrains.Annotations;
using Poq.Api.Utility.Http.Extension;

namespace Poq.Api.Utility.Http
{
    public class PoqHttpRequest
    {
        private List<PoqHttpCookie> _cookies;

        public List<PoqHttpCookie> Cookies
        {
            [NotNull]
            get { _cookies = _cookies ?? new List<PoqHttpCookie>(); return _cookies; }
            set { _cookies = value; }
        }

        private List<PoqHttpHeader> _headers;

        public List<PoqHttpHeader> Headers
        {
            [NotNull]
            get { _headers = _headers ?? new List<PoqHttpHeader>(); return _headers; }
            set { _headers = value; }
        }

        public string ContentAsString { get; set; }
        public HttpMethod Method { get; set; }
        public string Url { get; set; }

        public PoqHttpRequest(HttpMethod method, string url)
            : this(method, url, null, null)
        {
        }

        public PoqHttpRequest(HttpMethod method, string url, IEnumerable<PoqHttpCookie> cookies)
            : this(method, url, cookies, null)
        {
        }

        public PoqHttpRequest(HttpMethod method, string url, IEnumerable<PoqHttpHeader> headers)
            : this(method, url, null, headers)
        {
        }

        public PoqHttpRequest(HttpMethod method, string url, IEnumerable<PoqHttpCookie> cookies, IEnumerable<PoqHttpHeader> headers)
            : this(method, url, cookies, headers, null)
        {
        }

        public PoqHttpRequest(HttpMethod method, string url, IEnumerable<PoqHttpCookie> cookies, IEnumerable<PoqHttpHeader> headers, string contentAsString)
        {
            Method = method;
            Url = url;
            Cookies = cookies as List<PoqHttpCookie> ?? cookies?.ToList();
            Headers = headers as List<PoqHttpHeader> ?? headers?.ToList();
            ContentAsString = contentAsString;
        }

        public PoqHttpRequest Clone()
        {
            return new PoqHttpRequest(Method, Url)
            {
                _cookies = _cookies,
                _headers = _headers,
                ContentAsString = ContentAsString,
            };
        }

        public override string ToString()
        {
            return this.ToCurlCommand();
        }
    }
}
