using System.Threading.Tasks;

namespace Webhook
{
    public interface IHook
    {
        Task Notify(string key, object queryString = null, object body = null);
    }
}