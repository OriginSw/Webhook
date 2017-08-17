using System;

namespace Webhook
{
    public class WebhookHttpRequestException : Exception
    {
        public WebhookHttpRequestException()
            : base() { }

        public WebhookHttpRequestException(string status, Uri uri, Exception inner = null)
            : base(GetMessage(status, uri), inner) { }

        private static string GetMessage(string status, Uri uri)
        {
            var _message = string.Format("Webhook - Status: {0} - Uri: {1}", status, uri.AbsoluteUri);

            return _message;
        }
    }
}