﻿using System;

namespace OneDas.Infrastructure
{
    public class SimpleDataStorage : DataStorageBase
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

        public override object Get(int index)
        {
            return this.DataBuffer[index];
        }

        public override SimpleDataStorage ToSimpleDataStorage()
        {
            return this;
        }

        #endregion
    }
}