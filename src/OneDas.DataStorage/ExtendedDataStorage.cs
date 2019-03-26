using OneDas.DataStorage;
using System;

namespace OneDas.Extensibility
{
    public class ExtendedDataStorage<T> : ExtendedDataStorageBase where T : struct
    {
        #region "Constuctors"

        public ExtendedDataStorage(int elementCount) : base(typeof(T), elementCount)
        {
            //
        }

        public ExtendedDataStorage(T[] dataset, byte[] statusSet) : base(typeof(T), statusSet)
        {
            dataset.CopyTo(this.GetDataBuffer<T>());
        }

        #endregion

        #region "Methods"

        public override object GetValue(int index)
        {
            return this.GetDataBuffer<T>()[index];
        }

        public override double[] ApplyDatasetStatus()
        {
            return ExtendedDataStorageBase.ApplyDatasetStatus(this.GetDataBuffer<T>().ToArray(), this.GetStatusBuffer().ToArray());
        }

        public override ISimpleDataStorage ToSimpleDataStorage()
        {
            return new SimpleDataStorage(this.ApplyDatasetStatus());
        }

        #endregion
    }
}