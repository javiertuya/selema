using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Giis.Selema.Portable.Selenium
{
    public static class RestClient
    {
        private static readonly HttpClient client = new HttpClient();

        public static string RestCall(string method, string url)
        {
            return RestCall(method, url, null);
        }

        public static void RestDownload(string url, string targetFile)
        {
            try
            {
                using (var outStream = new FileStream(targetFile, FileMode.Create, FileAccess.Write))
                {
                    RestCall("GET", url, outStream);
                }
            }
            catch (Exception ex)
            {
                throw new VideoControllerException("Can't get video file: " + ex.Message, ex);
            }
        }

        public static string RestCall(string method, string url, Stream outputStream)
        {
            try
            {
                var request = new HttpRequestMessage(new HttpMethod(method), url);
                var response = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    if (method.ToUpper() == "GET" && outputStream != null)
                    {
                        var responseStream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
                        responseStream.CopyTo(outputStream);
                        return "downloaded";
                    }
                    else
                    {
                        return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    }
                }
                else
                {
                    string error = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    throw new VideoControllerException($"HTTP error {(int)response.StatusCode}: {error}");
                }
            }
            catch (Exception ex)
            {
                throw new VideoControllerException("Unexpected http client exception, method: "
                    + method + ", url: " + url + ", message: " + ex.Message, ex);
            }
        }
    }

}