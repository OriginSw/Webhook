using Amazon.SimpleNotificationService;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Threading.Tasks;
using Webhook.Helpers;

namespace Webhook
{
    public class AWSHook : IHook
    {
        private IHook _inner;
        private Action<Exception> _onError;
        private Lazy<AmazonSimpleNotificationServiceClient> _awsClient;

        public AWSHook(IHook inner = null, Action<Exception> onError = null)
        {
            _inner = inner;
            _onError = onError;
            _awsClient = new Lazy<AmazonSimpleNotificationServiceClient>(() => new AmazonSimpleNotificationServiceClient());
        }

        public async Task Notify(string key, object queryString = null, object body = null)
        {
            if (_inner != null)
                await _inner.Notify(key, queryString, body);
            try
            {
                if (ConfigSection.Webhook.AwsHooks.Enable && ConfigSection.Webhook.AwsData[key].Enable)
                {
                    var data = ConfigSection.Webhook.AwsData[key];

                    if (!(ConfigSection.Webhook.AwsHooks.Enable && data.Enable))
                        return;

                    object message;
                    if (queryString != null)
                        message = new { deleteIds = queryString };
                    else
                        message = body;

                    var arnPrefix = ConfigSection.Webhook.AwsHooks.ArnPrefix;

                    if (string.IsNullOrWhiteSpace(data.Arn) && (string.IsNullOrWhiteSpace(arnPrefix) || string.IsNullOrWhiteSpace(data.ArnSufix)))
                        throw new ConfigurationErrorsException("Missing configuration element 'arnPrefix' or 'arnSufix'");

                    var arn = !string.IsNullOrWhiteSpace(data.Arn) ? data.Arn : string.Concat(arnPrefix, data.ArnSufix);

                    await _awsClient.Value.PublishAsync(arn, JsonConvert.SerializeObject(message));
                }
            }
            catch (Exception ex)
            {
                _onError?.Invoke(ex);
            }
        }
    }
}