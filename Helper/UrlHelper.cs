using System.Globalization;

namespace Menu4Tech.Helper
{
    public class UrlHelper
    {
        public static string GetLocalizedUrl(string url, string queryString, string cultureIso)
        {
            string culture = CultureInfo.CurrentCulture.Name;
            if (url.Contains($"/{culture}"))
            {
                return url.Replace($"/{culture}", $"/{cultureIso}");
            }
            if (!url.StartsWith("/"))
            {
                url = "/" + url;
            }
            return $"/{cultureIso}{url}{queryString}";
        }

        public static string GetHostUrl()
        {
            var context = EnvironmentHelper.HttpContextAccessor.HttpContext;

            if (context.Request.Host.Value.StartsWith("m."))
            {
                return $"https://{context.Request.Host.Value.ReplaceFirst("m.", "www.")}";
            }
            
            return context.Request.Host.Value.Contains("89.23.5.24") ? $"http://{context.Request.Host.Value}" : $"https://{context.Request.Host.Value}";
        }
    }
}
