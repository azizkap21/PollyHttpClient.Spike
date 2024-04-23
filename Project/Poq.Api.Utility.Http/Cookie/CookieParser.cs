using System;

namespace Poq.Api.Utility.Http.Cookie
{
    internal static class CookieParser
    {
        internal static PoqHttpCookie Parse(string rawValue)
        {
            var cookieParts = rawValue.Split(';');
            var cookiePartCount = cookieParts.Length;

            var cookie = new PoqHttpCookie { RawValue = rawValue };

            for (var i = 0; i < cookiePartCount; i++)
            {
                if (i == 0)
                {
                    var cookieNameAndValue = cookieParts[i];

                    if (cookieNameAndValue != string.Empty)
                    {
                        var firstEqual = cookieNameAndValue.IndexOf("=", StringComparison.Ordinal);
                        var firstName = cookieNameAndValue.Substring(0, firstEqual);
                        var allValue = cookieNameAndValue.Substring(firstEqual + 1, cookieNameAndValue.Length - (firstEqual + 1));
                        cookie.Name = firstName;
                        cookie.Value = allValue;
                    }

                    continue;
                }

                if (cookieParts[i].IndexOf("path", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var pathNameAndValue = cookieParts[i];

                    if (pathNameAndValue != string.Empty)
                    {
                        var nameValuePairTemp = pathNameAndValue.Split('=');
                        cookie.Path = nameValuePairTemp[1] != string.Empty ? nameValuePairTemp[1] : "/";
                    }

                    continue;
                }

                if (cookieParts[i].IndexOf("domain", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var domainNameAndValue = cookieParts[i];

                    if (domainNameAndValue != string.Empty)
                    {
                        var nameValuePairTemp = domainNameAndValue.Split('=');
                        cookie.Domain = nameValuePairTemp[1] != string.Empty ? nameValuePairTemp[1] : string.Empty;
                    }

                    continue;
                }

                if (cookieParts[i].IndexOf("expires", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var expiresStr = cookieParts[i];

                    string expiresDateStr;

                    if (!string.IsNullOrEmpty(expiresStr))
                    {
                        var splittedDate = expiresStr.Split('=');
                        expiresDateStr = splittedDate.Length > 1 ? expiresStr.Split('=')[1] : null;
                    }
                    else
                    {
                        expiresDateStr = null;
                    }

                    DateTime expires;

                    DateTime.TryParse(expiresDateStr, out expires);

                    expires = expires.ToUniversalTime();

                    cookie.ExpireDate = expires;
                }

                if (cookieParts[i].IndexOf("comment", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var commentNameAndValue = cookieParts[i];

                    if (commentNameAndValue != string.Empty)
                    {
                        var nameValuePairTemp = commentNameAndValue.Split('=');
                        cookie.Comment = nameValuePairTemp[1] != string.Empty ? nameValuePairTemp[1] : string.Empty;
                    }

                    continue;
                }

                if (cookieParts[i].IndexOf("commenturi", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var commentUrlNameAndValue = cookieParts[i];

                    if (commentUrlNameAndValue != string.Empty)
                    {
                        var nameValuePairTemp = commentUrlNameAndValue.Split('=');
                        cookie.CommentUrl = nameValuePairTemp[1] != string.Empty
                            ? nameValuePairTemp[1]
                            : string.Empty;
                    }

                    continue;
                }

                if (cookieParts[i].IndexOf("port", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var portNameAndValue = cookieParts[i];

                    if (portNameAndValue != string.Empty)
                    {
                        var nameValuePairTemp = portNameAndValue.Split('=');
                        cookie.Port = nameValuePairTemp[1] != string.Empty ? nameValuePairTemp[1] : string.Empty;
                    }

                    continue;
                }

                cookie.IsSecure |= cookieParts[i].Trim().Equals("secure", StringComparison.OrdinalIgnoreCase);

                cookie.IsHttpOnly |= cookieParts[i].Trim().Equals("httponly", StringComparison.OrdinalIgnoreCase);
            }

            if (cookie.Path == string.Empty)
            {
                cookie.Path = "/";
            }

            return cookie;
        }
    }
}
