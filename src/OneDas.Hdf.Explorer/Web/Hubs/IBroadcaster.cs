using OneDas.Hdf.Explorer.Core;

namespace OneDas.Hdf.Explorer.Web
{
    public interface IBroadcaster
    {
        void SendState(HdfExplorerState hdfExplorerState);
        void SendProgress(double percent, string message);
        void SendByteCount(ulong byteCount);
    }
}
