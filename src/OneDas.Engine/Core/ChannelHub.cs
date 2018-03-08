using System;
using System.Runtime.InteropServices;

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
            if (this.AssociatedDataInput != null)
            {
                return new Span<T>(this.AssociatedDataInput.DataPtr.ToPointer(), 1)[0];
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}