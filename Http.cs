using System.IO;
using System.Net;

namespace kk33.RbxStreamSniper
{
    public class HttpHelpers
    {
        public static string Get(string url, string cookie = null, string ua = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3835.0 Safari/537.36")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("User-Agent", ua);
            if (cookie != null)
            {
                request.Headers.Add("Cookie", cookie);
            }
            request.Method = "GET";
            request.AllowAutoRedirect = true;
            using (var req = request.GetResponse())
            using (var sr = new StreamReader(req.GetResponseStream()))
            {
                return sr.ReadToEnd();
            }
        }

        public static string GetRedirectLocation(string url, string cookie = null, string ua = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3835.0 Safari/537.36")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("User-Agent", ua);
            if (cookie != null)
            {
                request.Headers.Add("Cookie", cookie);
            }
            request.Method = "GET";
            request.AllowAutoRedirect = true;
            using (var req = request.GetResponse())
                return req.ResponseUri.ToString();
        }
    }
}