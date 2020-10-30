using OneDas.Buffers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace OneDas.DataManagement.Extensibility
{
    public class DataReaderDoubleStream : Stream
    {
        #region Fields

        private double[] _buffer;
        private int _offset;
        private int _position;
        private int _remaining;

        private long _length;
        private IEnumerator<DataReaderProgressRecord> _enumerator;

        #endregion

        #region Constructors

        public DataReaderDoubleStream(long length, IEnumerable<DataReaderProgressRecord> progressRecords)
        {
            _length = length;
            _enumerator = progressRecords.GetEnumerator();

            this._buffer = new double[0];
        }

        #endregion

        #region Properties

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => _length;

        public override long Position
        {
            get
            {
                return _position;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Methods

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var remaining = count;

            while (remaining > 0)
            {
                var byteBuffer = MemoryMarshal.AsBytes(_buffer.AsSpan());
                _remaining = byteBuffer.Length - _offset;

                // we have some remaining bytes in the buffer
                if (_remaining > 0)
                {
                    var actualCount = Math.Min(_remaining, remaining);
                    var source = byteBuffer.Slice(_offset, actualCount);
                    var target = buffer.AsSpan().Slice(offset, actualCount);

                    source.CopyTo(target);

                    // update counters
                    remaining -= actualCount;
                    _offset += actualCount;
                    _position += actualCount;
                    _remaining -= actualCount;
                }

                // load next buffer
                if (_remaining <= 0)
                {
                    _buffer = this.GetNext();
                    _offset = 0;
                }

                if (_buffer == null)
                    break;
            }

            return count - remaining;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        private double[] GetNext()
        {
            var success = _enumerator.MoveNext();

            if (success)
            {
                double[] doubleData;

                var entry = _enumerator.Current.DatasetToRecordMap.First();
                var dataset = entry.Key;
                var dataRecord = entry.Value;

                if (dataset.IsNative)
                    doubleData = BufferUtilities.ApplyDatasetStatus2(dataRecord.Dataset, dataRecord.Status);
                else
                    doubleData = (double[])dataRecord.Dataset;

                return doubleData;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
