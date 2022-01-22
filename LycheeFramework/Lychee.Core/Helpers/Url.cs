using System.IO;

namespace Lychee.Core.Helpers
{
    public static class Url
    {
        public static string Combine(params string[] urls)
        {
            return Path.Combine(urls).Replace(@"\", "/");
        }

        public static string Join(string url, string param)
        {
            return $"{GetUrl(url)}{param}";
        }

        public static string GetUrl(string url)
        {
            if (!url.Contains("?"))
            {
                return $"{url}?";
            }

            if (url.EndsWith("?"))
            {
                return url;
            }

            if (url.EndsWith("&"))
            {
                return url;
            }

            return $"{url}&";
        }
    }
}