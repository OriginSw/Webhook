using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Webhook.Helpers;

namespace Webhook
{
    public class Hook : Component, IHook
    {
        private Client _client;
        private static Action<Exception> OnError;

        public Hook(Action<Exception> onError = null)
        {
            _client = new Client();
            OnError = onError;
        }

        public async Task Notify(string key, object queryString = null, object body = null)
        {
            try
            {
                var data = ConfigSection.Webhook.Data[key];

                if (!(ConfigSection.Webhook.Hooks.Enable && data.Enable))
                    return;

                var url = (string.IsNullOrEmpty(data.Url) ? ConfigSection.Webhook.Hooks.DefaultUrl : data.Url) + data.Endpoint;

                if (data.Method == "GET")
                {
                    url = url + (data.Endpoint.Contains("?") ? "&" : "?") + ClientHelpers.GetQueryString(queryString);
                    await _client.httpGetRequest(url);
                }
                else if (data.Method == "POST")
                {
                    await _client.httpPostRequest(url, ClientHelpers.GetJsonBody(body));
                }
                else
                {
                    throw new NotImplementedException(string.Format("Http method {0} is not implemented yet", data.Method));
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
            }
        }
    }
}