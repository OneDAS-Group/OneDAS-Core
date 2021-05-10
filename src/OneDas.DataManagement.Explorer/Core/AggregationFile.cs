using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace OneDas.DataManagement.Explorer.Core
{
    public class AggregationFile : IDisposable
    {
        #region Fields

        // derived from https://support.hdfgroup.org/HDF5/doc/H5.format.html#Superblock
        private static byte[] _signature = new byte[] { 0x89, 0x4E, 0x45, 0x58, 0x0D, 0x0A, 0x1A, 0x0A };

        #endregion

        #region Methods

        public static Span<T> Read<T>(string filePath, int expectedLength) where T : unmanaged
        {
            // open file
            var fileStream = File.OpenRead(filePath);

            // validate signature
            Span<byte> signature = stackalloc byte[8];
            fileStream.Read(signature);
            AggregationFile.ValidateSignature(signature, _signature);

            // return data
            using var decompressedStream = new MemoryStream(capacity: expectedLength);
            using var decompressionStream = new DeflateStream(fileStream, CompressionMode.Decompress);

            decompressionStream.CopyTo(decompressedStream);

            var span = decompressedStream
                .GetBuffer()
                .AsSpan(0, (int)decompressedStream.Length);

            return MemoryMarshal
                .Cast<byte, T>(span);
        }

        public static void Create<T>(string filePath, ReadOnlySpan<T> buffer) where T : unmanaged
        {
            // target stream
            var targetStream = File.Open(filePath, FileMode.CreateNew, FileAccess.Write);

            // add format signature
            targetStream.Write(_signature);

            // add version
            targetStream.WriteByte(1);

            // write data
            using var compressionStream = new DeflateStream(targetStream, CompressionMode.Compress);
            var byteBuffer = MemoryMarshal.AsBytes<T>(buffer);
            compressionStream.Write(byteBuffer);
        }

        private static void ValidateSignature(Span<byte> actual, Span<byte> expected)
        {
            if (actual.Length == expected.Length)
            {
                if (actual[0] == expected[0] && actual[1] == expected[1] && actual[2] == expected[2] && actual[3] == expected[3]
                 && actual[4] == expected[4] && actual[5] == expected[5] && actual[6] == expected[6] && actual[7] == expected[7])
                {
                    return;
                }
            }

            throw new Exception("This is not a valid Nexus file.");
        }

        #endregion

        #region IDisposable

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
