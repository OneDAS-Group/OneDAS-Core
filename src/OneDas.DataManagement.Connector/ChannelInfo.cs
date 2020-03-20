using OneDas.DataManagement.Infrastructure;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.DataManagement.Connector
{
    public class ChannelInfo
    {
        #region Constructors

        public ChannelInfo(VariableInfo variable, string channelName, double[] data)
        {
            this.Name = variable.VariableNames.Last();
            this.Group = variable.VariableGroups.Last();
            this.Unit = variable.Units.Last();
            this.TransferFunctions = variable.TransferFunctions;
            this.DatasetName = channelName.Split('/').Last();
            this.Values = data;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public string Group { get; private set; }

        public string Unit { get; private set; }

        public List<TransferFunction> TransferFunctions { get; private set; }

        public string DatasetName { get; private set; }

        public double[] Values { get; private set; }

        #endregion
    }
}
