using System;
using Poq.Api.Utility.Http.Cookie;

namespace Poq.Api.Utility.Http
{
    [Serializable]
    public class PoqHttpCookie
    {
        // Basic
        public string Name { get; set; }
        public string Value { get; set; }
        public DateTime? ExpireDate { get; set; }

        // Rest
        public string Comment { get; set; }
        public string CommentUrl { get; set; }
        public string Domain { get; set; }
        public bool IsHttpOnly { get; set; }
        public bool IsSecure { get; set; }
        public bool IsSessionOnly { get; set; }
        public string Path { get; set; }
        public string Port { get; set; }
        public string Version { get; set; }

        public string RawValue { get; internal set; }

        public static PoqHttpCookie FromRawValue(string rawValue) => CookieParser.Parse(rawValue);
    }
}
