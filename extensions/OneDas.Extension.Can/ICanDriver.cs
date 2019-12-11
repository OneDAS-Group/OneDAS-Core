using System;

namespace OneDas.Extension.Can
{
    interface ICanDriver
    {
        #region Properties

        int AvailableMessagesCount { get; }

        #endregion

        #region Methods

        void Send(uint identifier, CanFrameFormat frameFormat, Span<byte> data);

        bool Receive(out uint identifier, out byte[] data);

        void Dispose();

        #endregion
    }
}
