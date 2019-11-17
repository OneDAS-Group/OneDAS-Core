using System;

namespace OneDas.DataStorage
{
    public interface IExtendedDataStorage : IDataStorage
    {
        #region "Properties"

        IntPtr StatusBufferPtr { get; }

        #endregion

        #region "Methods"

        Span<byte> GetStatusBuffer();

        double[] ApplyDatasetStatus();

        #endregion
    }
}