namespace OneDas.Infrastructure
{
    /// <summary>
    /// Represents a generic infrastructure to buffer data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExtendedDataStorage<T> : ExtendedDataStorageBase where T : struct
    {
        #region "Constuctors"

        public ExtendedDataStorage(int length) : base(new T[length], typeof(T))
        {
            //
        }

        public ExtendedDataStorage(T[] dataset, byte[] statusSet) : base(dataset, statusSet, typeof(T))
        {
            //
        }

        #endregion

        #region "Methods"

        public override double[] ApplyDatasetStatus()
        {
            return ExtendedDataStorageBase.ApplyDatasetStatus((T[])this.Dataset, this.StatusSet);
        }

        public override SimpleDataStorage ToSimpleDataStorage()
        {
            return new SimpleDataStorage(this.ApplyDatasetStatus());
        }

        #endregion
    }
}