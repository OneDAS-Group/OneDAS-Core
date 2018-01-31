using System;

namespace OneDas.Infrastructure
{
    /// <summary>
    /// Represents an infrastructure to buffer data.
    /// </summary>
    public abstract class DataStorageBase
    {
        #region "Constructors"

        /// <summary>
        /// Creates a new <see cref="DataStorageBase"/> instance.
        /// </summary>
        /// <param name="length">The buffer length.</param>
        public DataStorageBase(Array dataset)
        {
            this.Dataset = dataset;
            this.Length = dataset.LongLength;
            this.ElementSize = InfrastructureHelper.GetBitLength(InfrastructureHelper.GetOneDasDataTypeFromType(dataset.GetType().GetElementType()), true) / 8;
        }

        #endregion

        #region "Properties"

        public int ElementSize { get; private set; }
        public long Length { get; private set; }
        public Array Dataset { get; private set; }

        #endregion

        #region "Methods"

        public abstract IntPtr GetDataPointer(int index);
        public abstract SimpleDataStorage ToSimpleDataStorage();

        #endregion
    }
}