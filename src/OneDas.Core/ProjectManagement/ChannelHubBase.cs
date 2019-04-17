using OneDas.Infrastructure;
using OneDas.Extensibility;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using OneDas.ProjectManagement;

namespace OneDas.Core.ProjectManagement
{
    public abstract class ChannelHubBase
    {
        #region "Constructors"

        public ChannelHubBase(ChannelHubSettings channelHubSettings)
        {
            this.Settings = channelHubSettings;

            this.AssociatedDataOutputSet = new List<DataPort>();
        }

        #endregion

        #region "Properties"

        public ChannelHubSettings Settings { get; }

        public DataPort AssociatedDataInput { get; protected set; }

        public List<DataPort> AssociatedDataOutputSet { get; }

        #endregion

        #region "Methods"

        public void SetAssociation(DataPort dataPort)
        {
            Contract.Requires(dataPort != null);

            switch (dataPort.DataDirection)
            {
                case DataDirection.Input:

                    this.AssociatedDataInput = dataPort;

                    break;

                case DataDirection.Output:

                    this.AssociatedDataOutputSet.Add(dataPort);

                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public abstract object GetValue();

        #endregion
    }
}