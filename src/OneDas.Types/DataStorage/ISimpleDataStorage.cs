using System;

namespace OneDas.DataStorage
{
    public interface ISimpleDataStorage : IDataStorage
    {
        #region "Properties"

        new Span<double> DataBuffer { get; }

        #endregion
    }
}