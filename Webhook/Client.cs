using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Webhook
{
    public class Client
    {
        public async Task<bool> httpPostRequest(string url, string body = null)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                if (body != null)
                {
                    streamWriter.Write(body);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                using (var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync())
                {
                    return httpResponse.StatusCode == HttpStatusCode.OK;
                }
            }
        }

        public async Task<bool> httpGetRequest(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "GET";

            using (var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync())
            {
                return httpResponse.StatusCode == HttpStatusCode.OK;
            }
        }
    }
}