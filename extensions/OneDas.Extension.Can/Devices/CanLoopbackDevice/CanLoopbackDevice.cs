using System;
using System.Collections.Concurrent;

namespace OneDas.Extension.Can
{
    public class CanLoopbackDevice : ICanDevice
    {
        #region Fields

        ConcurrentQueue<(uint Identifier, byte[] Data)> _messageQueue;

        #endregion

        #region Constructors

        public CanLoopbackDevice()
        {
            _messageQueue = new ConcurrentQueue<(uint identifier, byte[] data)>();
        }

        #endregion

        #region Properties

        public int AvailableMessagesCount => _messageQueue.Count;

        #endregion

        #region Methods

        public void Send(uint identifier, CanFrameFormat frameFormat, Span<byte> data)
        {
            if (data.Length > 8)
                throw new FormatException(ErrorMessage.MessageSizeExceeded);

            _messageQueue.Enqueue((identifier, data.ToArray()));
        }

        public bool Receive(out uint identifier, out byte[] data)
        {
            identifier = default;
            data = default;

            if (_messageQueue.TryDequeue(out var message))
            {
                identifier = message.Identifier;
                data = message.Data;

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Dispose()
        {
            //
        }

        #endregion
    }
}
