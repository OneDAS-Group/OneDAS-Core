using OneDas.Buffers;
using OneDas.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.DataManagement.Database
{
    public class SparseCampaignInfo : CampaignInfo
    {
        #region "Constructors"

        public SparseCampaignInfo(string id) : base(id)
        {
            //
        }

        #endregion

        #region "Methods"

        public List<VariableDescription> ToVariableDescriptions()
        {
            return this.Variables.SelectMany(variable =>
            {
                return variable.Datasets.Select(dataset =>
                {
                    var guid = new Guid(variable.Id);
                    var displayName = variable.Name;
                    var datasetName = dataset.Id;
                    var groupName = variable.Group;
                    var dataType = dataset.DataType;
                    var sampleRate = dataset.GetSampleRate();
                    var unit = variable.Unit;
                    var transferFunctions = variable.TransferFunctions;

                    return new VariableDescription(guid,
                                                   displayName,
                                                   datasetName,
                                                   groupName,
                                                   dataType,
                                                   sampleRate,
                                                   unit,
                                                   transferFunctions,
                                                   BufferType.Simple);
                });
            }).ToList();
        }

        #endregion
    }
}
