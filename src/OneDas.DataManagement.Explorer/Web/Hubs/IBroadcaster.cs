using OneDas.DataManagement.Explorer.Core;
using System.Threading.Tasks;

namespace OneDas.DataManagement.Explorer.Web
{
    public interface IBroadcaster
    {
        Task SendState(OneDasExplorerState explorerState);
        Task SendProgress(double percent, string message);
        Task SendByteCount(ulong byteCount);
    }
}
