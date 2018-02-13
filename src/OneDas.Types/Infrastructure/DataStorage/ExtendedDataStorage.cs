namespace OneDas.Infrastructure
{
    public class ExtendedDataStorage<T> : ExtendedDataStorageBase where T : struct
    {
        #region "Constuctors"

        public ExtendedDataStorage(int elementCount) : base(typeof(T), elementCount)
        {
            //
        }

        #endregion

        #region "Methods"

        public override object Get(int index)
        {
            return this.GetDataBuffer<T>()[index];
        }

        public override double[] ApplyDatasetStatus()
        {
            return ExtendedDataStorageBase.ApplyDatasetStatus(this.GetDataBuffer<T>().ToArray(), this.GetStatusBuffer().ToArray());
        }

        public override SimpleDataStorage ToSimpleDataStorage()
        {
            return new SimpleDataStorage(this.ApplyDatasetStatus());
        }

        #endregion
    }
}