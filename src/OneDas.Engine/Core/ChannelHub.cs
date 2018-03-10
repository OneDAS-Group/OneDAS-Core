using OneDas.Infrastructure;
using OneDas.Plugin;
using System;

namespace OneDas.Engine.Core
{
    public class ChannelHub<T> : ChannelHubBase
    {
        #region "Constructors"

        public ChannelHub(ChannelHubSettings channelHubSettings) : base(channelHubSettings)
        {
            //
        }

        #endregion

        #region "Methods"
            
        public unsafe override object GetValue()
        {
            int elementSize;
            byte* sourcePtr;
            DataPort dataPort;

            dataPort = this.AssociatedDataInput;

            if (this.AssociatedDataInput != null)
            {
                sourcePtr = (byte*)dataPort.DataPtr.ToPointer();
                elementSize = OneDasUtilities.SizeOf(typeof(T));

                byte* targetPtr = stackalloc byte[elementSize];

                if (dataPort.DataType == OneDasDataType.BOOLEAN && dataPort.BitOffset > -1) // special handling for boolean
                {
                    // from bit to byte
                    bool value;

                    value = (*sourcePtr & (1 << dataPort.BitOffset)) > 0;
                    targetPtr[0] = *(byte*)&value;
                }
                else
                {
                    switch (dataPort.Endianness)
                    {
                        case Endianness.LittleEndian:

                            for (int i = 0; i < elementSize; i++)
                            {
                                targetPtr[i] = sourcePtr[i];
                            }

                            break;

                        case Endianness.BigEndian:

                            for (int i = 0; i < elementSize; i++)
                            {
                                targetPtr[i] = sourcePtr[elementSize - i - 1];
                            }

                            break;

                        default:

                            throw new ArgumentException();
                    }
                }

                return new Span<T>(targetPtr, 1)[0];
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}