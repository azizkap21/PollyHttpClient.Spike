using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using JetBrains.Annotations;

namespace Poq.Api.Utility.Http.Extension
{
    public static class HttpHeadersExtensions
    {
        [CanBeNull]
        public static string FirstOrDefault(this HttpHeaders theHeaders, string headerName)
        {
            if (string.IsNullOrWhiteSpace(headerName)) throw new ArgumentNullException(nameof(headerName));

            var returnValue = theHeaders.TryGetValues(headerName, out IEnumerable<string> headerValues) 
                ? headerValues.FirstOrDefault() : null;

            return returnValue;
        }

        public static void AddPoqHeaders(this HttpHeaders theHeaders, [NotNull] PoqHeaderCollection poqHeaders)
        {
            theHeaders.TryAddWithoutValidation(PoqHeaderCollection.AppIdHeaderName, poqHeaders.AppId.ToString());
            theHeaders.TryAddWithoutValidation(poqHeaders.AuthorizationHttpHeader.Name, poqHeaders.AuthorizationHttpHeader.Value);
            theHeaders.TryAddWithoutValidation(PoqHeaderCollection.IsPreviewHeaderName, poqHeaders.IsPreview.ToString());
            theHeaders.TryAddWithoutValidation(PoqHeaderCollection.PlatformHeaderName, poqHeaders.Platform);
            theHeaders.TryAddWithoutValidation(PoqHeaderCollection.PoqUserIdHeaderName, poqHeaders.PoqUserId);
            theHeaders.TryAddWithoutValidation(PoqHeaderCollection.VersionHeaderName, poqHeaders.Version);
            theHeaders.TryAddWithoutValidation(PoqHeaderCollection.PreviewDateHeaderName, poqHeaders.PreviewDate.ToString());
            theHeaders.TryAddWithoutValidation(PoqHeaderCollection.CurrencyHeaderName, poqHeaders.CurrencyCode);
        }
    }
}