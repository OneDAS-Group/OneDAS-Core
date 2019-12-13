using Ixxat.Vci4.Bal.Can;
using System;

namespace OneDas.Extension.Can
{
    static class IxxatUtils
    {
        #region Properties

        public static Guid AcceptedDeviceClass { get; } = new Guid("2219f6a0-732a-4750-b66f-0ae731fb40e1");

        #endregion

        #region Methods

        public static CanBitrate CiaBitRateToCanBitrate(CiaBitRate bitRate)
        {
            return bitRate switch
            {
                CiaBitRate.Cia10KBit => CanBitrate.Cia10KBit,
                CiaBitRate.Cia20KBit => CanBitrate.Cia20KBit,
                CiaBitRate.Cia50KBit => CanBitrate.Cia50KBit,
                CiaBitRate.Cia125KBit => CanBitrate.Cia125KBit,
                CiaBitRate.Cia500KBit => CanBitrate.Cia500KBit,
                CiaBitRate.Cia800KBit => CanBitrate.Cia800KBit,
                CiaBitRate.Cia1000KBit => CanBitrate.Cia1000KBit,
                _ => throw new NotSupportedException()
            };
        }

        public static string TrimHardwareId(string hardwareId)
        {
            return hardwareId.Replace("\0", string.Empty);
        }

        #endregion
    }
}
