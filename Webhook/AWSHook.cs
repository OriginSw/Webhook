using Amazon.SimpleNotificationService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public async Task Notify(string key, Dictionary<string, object> queryString = null, Dictionary<string, object> body = null, Dictionary<string, object> metadata = null)
        {
            await _inner?.Notify(key, queryString, body);
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

                    if (metadata != null)
                        await _awsClient.Value.PublishAsync(arn, JsonConvert.SerializeObject(message), JsonConvert.SerializeObject(metadata));
                    else
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