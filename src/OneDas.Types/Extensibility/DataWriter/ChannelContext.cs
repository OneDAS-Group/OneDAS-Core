using OneDas.Buffers;
using System;

namespace OneDas.Extensibility
{
    public class ChannelContext
    {
        #region "Constructors"

        public ChannelContext(ChannelDescription channelDescription, IBuffer buffer)
        {
            this.ChannelDescription = channelDescription;
            this.Buffer = buffer;

            if (channelDescription.BufferType != buffer.Type)
                throw new ArgumentException(ErrorMessage.ChannelContext_BufferTypeInvalid);
        }

        #endregion

        #region "Properties"

        public ChannelDescription ChannelDescription { get; private set; }

        public IBuffer Buffer { get; private set; }

        #endregion
    }
}