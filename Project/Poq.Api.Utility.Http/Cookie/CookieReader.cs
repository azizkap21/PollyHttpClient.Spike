using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Poq.Api.Utility.Http.Cookie
{
    public static class CookieReader
    {
        [NotNull]
        public static List<PoqHttpCookie> ReadCookiesFromResponseHeaders([CanBeNull] IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            return headers?
                .FirstOrDefault(c => c.Key == "Set-Cookie")
                .Value?
                .Select(PoqHttpCookie.FromRawValue)
                .ToList() ?? new List<PoqHttpCookie>();
        }
    }
}
