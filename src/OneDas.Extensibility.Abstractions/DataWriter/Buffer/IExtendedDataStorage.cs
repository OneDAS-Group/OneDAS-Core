using System;

namespace OneDas.Extensibility
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