using System.Collections.Generic;
using System.Threading.Tasks;

namespace Webhook
{
    public interface IHook
    {
        Task Notify(string key, Dictionary<string, object> queryString = null, Dictionary<string, object> body = null, Dictionary<string, object> metadata = null);
    }
}