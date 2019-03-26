using OneDas.DataStorage;
using System;

namespace OneDas.Extensibility
{
    public class SimpleDataStorage : DataStorageBase, ISimpleDataStorage
    {
        #region "Constuctors"

        public SimpleDataStorage(int elementCount) : base(typeof(double), elementCount)
        {
            //
        }

        public SimpleDataStorage(double[] dataset) : base(typeof(double), dataset.Length)
        {
            dataset.CopyTo(this.DataBuffer);
        }

        #endregion

        #region "Properties"

        public new Span<double> DataBuffer
        {
            get
            {
                return this.GetDataBuffer<double>();
            }
        }

        #endregion

        #region "Methods"

        public override object GetValue(int index)
        {
            return this.DataBuffer[index];
        }

        public override ISimpleDataStorage ToSimpleDataStorage()
        {
            return this;
        }

        #endregion
    }
}