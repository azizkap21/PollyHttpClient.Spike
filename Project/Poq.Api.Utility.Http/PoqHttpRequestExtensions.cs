using System.Net.Http;
using System.Text;

namespace Poq.Api.Utility.Http.Extension
{
    public static class PoqHttpRequestExtensions
    {
        public static string ToCurlCommand(this PoqHttpRequest thisRequest)
        {
            var sb = new StringBuilder("curl -X ");

            sb.Append(thisRequest.Method.Method.ToUpperInvariant() + " \\\r\n");
            sb.Append(thisRequest.Url + " \\\r\n");

            foreach (var header in thisRequest.Headers)
            {
                sb.Append($"-H '{header.Name.ToLowerInvariant()}: {header.Value}' \\\r\n");
            }

            if ((thisRequest.Method == HttpMethod.Post || thisRequest.Method == HttpMethod.Put) &&
                thisRequest.ContentAsString != null)
            {
                sb.Append($"-d '{thisRequest.ContentAsString.Replace("'", "'\\''")}'");
            }

            return sb.ToString();
        }
    }
}
