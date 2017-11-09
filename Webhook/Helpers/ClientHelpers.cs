using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webhook.Helpers
{
    public static class ClientHelpers
    {
        internal static string GetQueryString(Dictionary<string, object> obj = null)
        {
            if (obj == null)
                return string.Empty;

            var properties = obj.Where(x => x.Value != null);
            var qs = new List<string>();
            foreach (var prop in properties)
            {
                var type = prop.Value.GetType();
                if (type.IsArray)
                    foreach (var p in prop.Value as string[])
                        qs.Add(prop.Key + "=" + HttpUtility.UrlEncode(p.ToString()));
                else
                    qs.Add(prop.Key + "=" + HttpUtility.UrlEncode(prop.Value.ToString()));
            }

            return string.Join("&", qs.ToArray());
        }

        internal static string GetJsonBody(Dictionary<string, object> obj = null)
        {
            if (obj == null)
                return string.Empty;

            return JsonConvert.SerializeObject(obj);
        }
    }
}