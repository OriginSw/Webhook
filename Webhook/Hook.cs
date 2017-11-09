using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webhook.Helpers;

namespace Webhook
{
    public class Hook : IHook
    {
        private Client _client;
        private Action<Exception> OnError;

        public Hook(Action<Exception> onError = null)
        {
            _client = new Client(onError);
            OnError = onError;
        }

        public async Task Notify(string key, Dictionary<string, object> queryString = null, Dictionary<string, object> body = null, Dictionary<string, object> metadata = null)
        {
            try
            {
                if (ConfigSection.Webhook.Hooks.Enable && ConfigSection.Webhook.Data[key].Enable)
                {
                    var data = ConfigSection.Webhook.Data[key];

                    IEnumerable<string> urls = string.IsNullOrEmpty(data.Url) ? ConfigSection.Webhook.Hooks.DefaultUrl.Split(',').ToList() : data.Url.Split(',').ToList();
                    urls = urls.Select(x => string.Concat(x.Trim(), data.Endpoint));

                    var tasks = new List<Task>();

                    if (data.Method == "GET")
                    {
                        foreach (var url in urls)
                        {
                            var _url = string.Concat(url, data.Endpoint.Contains("?") ? "&" : "?", ClientHelpers.GetQueryString(queryString));
                            tasks.Add(Task.Factory.StartNew(() => _client.httpGetRequest(_url)));
                        }

                        Task.WaitAll(tasks.ToArray());
                    }
                    else if (data.Method == "POST")
                    {
                        if (metadata != null)
                            body.Add("metadata", metadata);
                        foreach (var url in urls)
                        {
                            var _url = string.Concat(url, data.Endpoint.Contains("?") ? "&" : "?", ClientHelpers.GetQueryString(queryString));
                            tasks.Add(Task.Factory.StartNew(() => _client.httpPostRequest(_url, ClientHelpers.GetJsonBody(body))));
                        }

                        Task.WaitAll(tasks.ToArray());
                    }
                    else if (data.Method == "DELETE")
                    {
                        if (metadata != null)
                            body.Add("metadata", metadata);
                        foreach (var url in urls)
                        {
                            var _url = string.Concat(url, data.Endpoint.Contains("?") ? "&" : "?", ClientHelpers.GetQueryString(queryString));
                            tasks.Add(Task.Factory.StartNew(() => _client.httpDeleteRequest(_url, ClientHelpers.GetJsonBody(body))));
                        }

                        Task.WaitAll(tasks.ToArray());
                    }
                    else
                    {
                        throw new NotImplementedException(string.Format("Http method {0} is not implemented yet", data.Method));
                    }
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
            }
        }
    }
}