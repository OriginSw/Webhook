using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Webhook.Helpers;

namespace Webhook
{
    internal class Client
    {
        private static Action<Exception> OnError;

        public Client(Action<Exception> onError = null)
        {
            OnError = onError;
        }

        public int Timeout { get { return ConfigSection.Webhook.Hooks.Timeout; } }

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
                httpWebRequest.Timeout = Timeout;
                return await GetResponse(httpWebRequest);
            }
        }

        public async Task<bool> httpDeleteRequest(string url, string body = null)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "DELETE";
            httpWebRequest.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                if (body != null)
                {
                    streamWriter.Write(body);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                httpWebRequest.Timeout = Timeout;
                return await GetResponse(httpWebRequest);
            }
        }

        public async Task<bool> httpGetRequest(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "GET";
            httpWebRequest.Timeout = Timeout;

            return await GetResponse(httpWebRequest);
        }

        private async Task<bool> GetResponse(WebRequest request)
        {
            bool ok = false;
            try
            {
                using (var httpResponse = (HttpWebResponse)await request.GetResponseAsync())
                {
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                        ok = true;
                    else
                        OnError(new WebhookHttpRequestException(((int)httpResponse.StatusCode).ToString(), request.RequestUri));
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    var response = (HttpWebResponse)ex.Response;
                    OnError(new WebhookHttpRequestException(((int)response.StatusCode).ToString(), request.RequestUri, ex));
                }
                else
                    OnError(new WebhookHttpRequestException(ex.Status.ToString(), request.RequestUri, ex));
            }
            return ok;
        }
    }
}