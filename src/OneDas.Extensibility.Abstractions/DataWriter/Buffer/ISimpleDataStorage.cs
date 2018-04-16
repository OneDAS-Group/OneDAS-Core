using System;

namespace OneDas.Extensibility
{
    public interface ISimpleDataStorage : IDataStorage
    {
        #region "Properties"

        new Span<double> DataBuffer { get; }

        #endregion
    }
}