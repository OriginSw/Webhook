using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webhook.Helpers
{
    public static class ClientHelpers
    {
        internal static string GetQueryString(object obj = null)
        {
            if (obj == null)
                return string.Empty;

            var properties = obj.GetType().GetProperties()
                .Where(p => p.GetValue(obj, null) != null);
            var qs = new List<string>();
            foreach (var prop in properties)
            {
                if (prop.PropertyType.IsArray)
                    foreach (var p in prop.GetValue(obj, null) as string[])
                        qs.Add(prop.Name + "=" + HttpUtility.UrlEncode(p.ToString()));
                else
                    qs.Add(prop.Name + "=" + HttpUtility.UrlEncode(prop.GetValue(obj, null).ToString()));
            }

            return string.Join("&", qs.ToArray());
        }

        internal static string GetJsonBody(object obj = null)
        {
            if (obj == null)
                return string.Empty;

            return JsonConvert.SerializeObject(obj);
        }
    }
}