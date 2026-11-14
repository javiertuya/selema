using System;
using System.IO;
using System.Net.Http;

namespace Giis.Selema.Portable
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
            RestCall("GET", url, targetFile);
        }

        /// <summary>
        /// Performs a REST operation; if GET and targetFile is not null, writes the binary response body to this file.
        /// The file is only created if the response is successful, to avoid overwriting a previous file with empty content
        /// </summary>
        public static string RestCall(string method, string url, string targetFile)
        {
            try
            {
                var request = new HttpRequestMessage(new HttpMethod(method), url);
                var response = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    if (method.ToUpper() == "GET" && targetFile != null)
                    {
                        var responseStream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
                        using (var outStream = new FileStream(targetFile, FileMode.Create, FileAccess.Write))
                        {
                            responseStream.CopyTo(outStream);
                        }
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