using System;

namespace OneDas.Infrastructure
{
    public interface ISimpleDataStorage : IDataStorage
    {
        #region "Properties"

        new Span<double> DataBuffer { get; }

        #endregion
    }
}