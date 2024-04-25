using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;
using JetBrains.Annotations;
using Poq.Api.Utility.Http.Extension;

namespace Poq.Api.Utility.Http
{
    public partial class PoqHeaderCollection
    {
        public const string AppIdHeaderName = "Poq-App-Id";
        public const string AuthorizationHeaderName = "Authorization";
        public const string IsPreviewHeaderName = "preview";
        public const string PlatformHeaderName = "platform";
        public const string PoqUserIdHeaderName = "Poq-User-Id";
        public const string PreviewDateHeaderName = "PreviewDateValue";
        public const string VersionHeaderName = "Version-Code";
        public const string CurrencyHeaderName = "Currency-Code";

        public int AppId { get; set; }
        public PoqHttpHeader AuthorizationHttpHeader { get; set; }
        public bool IsPreview { get; set; }
        public string Platform { get; set; }
        public string PoqUserId { get; set; }
        public DateTime? PreviewDate { get; set; }
        public string Version { get; set; }

        /// <summary>
        ///     Values for this header should conform to the ISO 4217 standard
        ///     for currency codes.
        /// </summary>
        public string CurrencyCode { get; set; }

        public static PoqHeaderCollection FromHttpHeaders([NotNull] HttpHeaders headers)
        {
            return new PoqHeaderCollection
            {
                AppId = int.TryParse(headers.FirstOrDefault(AppIdHeaderName), out var appId) ? appId : default(int),
                AuthorizationHttpHeader = headers.FirstOrDefault(AuthorizationHeaderName) != null ? new PoqHttpHeader(AuthorizationHeaderName, headers.FirstOrDefault(AuthorizationHeaderName)) : null,
                IsPreview = bool.TryParse(headers.FirstOrDefault(IsPreviewHeaderName), out var isPreview) && isPreview,
                Platform = headers.FirstOrDefault(PlatformHeaderName),
                PoqUserId = headers.FirstOrDefault(PoqUserIdHeaderName),
                PreviewDate = DateTime.TryParse(headers.FirstOrDefault(PreviewDateHeaderName), out var previewDate) ? previewDate : default(DateTime?),
                Version = headers.FirstOrDefault(VersionHeaderName),
                CurrencyCode = headers.FirstOrDefault(CurrencyHeaderName),
            };
        }

        public List<PoqHttpHeader> AsPoqHttpHeaderList()
        {
            var headers = new List<PoqHttpHeader>();

            if (AppId > 0) headers.Add(new PoqHttpHeader(AppIdHeaderName, AppId.ToString()));
            if (AuthorizationHttpHeader != null) headers.Add(AuthorizationHttpHeader);
            headers.Add(new PoqHttpHeader(IsPreviewHeaderName, IsPreview.ToString(CultureInfo.InvariantCulture).ToLower()));
            if (!string.IsNullOrEmpty(Platform)) headers.Add(new PoqHttpHeader(PlatformHeaderName, Platform));
            if (!string.IsNullOrEmpty(PoqUserId)) headers.Add(new PoqHttpHeader(PoqUserIdHeaderName, PoqUserId));
            if (PreviewDate.HasValue) headers.Add(new PoqHttpHeader(PreviewDateHeaderName, PreviewDate.Value.ToString("o")));
            if (!string.IsNullOrEmpty(Version)) headers.Add(new PoqHttpHeader(VersionHeaderName, Version));
            if (!string.IsNullOrEmpty(CurrencyCode)) headers.Add(new PoqHttpHeader(CurrencyHeaderName, CurrencyCode));

            return headers;
        }
    }
}