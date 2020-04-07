using HDF.PInvoke;
using OneDas.DataManagement.Hdf;
using OneDas.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.DataManagement.Database
{
    public class HdfDatasetInfo : CampaignElement
    {
        #region "Constructors"

        public HdfDatasetInfo(string id, CampaignElement parent) : base(id, parent)
        {
            //
        }

        #endregion

        #region Properties

        public OneDasDataType DataType { get; set; }

        #endregion

        #region "Methods"

        public SampleRateContainer GetSampleRate()
        {
            // is_chunk_completed_set
            if (this.Id.StartsWith("is_chunk_completed_set"))
                return new SampleRateContainer(1440, ensureNonZeroIntegerHz: false);
            else
                return new SampleRateContainer(this.Id);
        }

        public DatasetInfo ToDataset(VariableInfo parent)
        {
            var dataset = new DatasetInfo(this.Id, parent)
            {
                DataType = this.DataType
            };

            return dataset;
        }

        public void Update(long datasetId, FileContext fileContext, UpdateSourceFileMapDelegate updateSourceFileMap)
        {
            long dataspaceId = -1;
            long typeId = -1;

            ulong[] actualDimenionSet = new ulong[1];
            ulong[] maximumDimensionSet = new ulong[1];

            try
            {
                dataspaceId = H5D.get_space(datasetId);
                H5S.get_simple_extent_dims(dataspaceId, actualDimenionSet, maximumDimensionSet);

                typeId = H5D.get_type(datasetId);
                this.DataType = OneDasUtilities.GetOneDasDataTypeFromType(TypeConversionHelper.GetTypeFromHdfTypeId(typeId));
            }
            finally
            {
                if (H5I.is_valid(typeId) > 0) { H5T.close(typeId); }
                if (H5I.is_valid(dataspaceId) > 0) { H5S.close(dataspaceId); }
            }

            var sourceFileInfo = new SourceFileInfo(fileContext.FilePath, actualDimenionSet.First(), fileContext.DateTime);
            updateSourceFileMap?.Invoke(datasetId, this, sourceFileInfo);
        }

        public override string GetPath()
        {
            return $"{this.Parent.GetPath()}/{this.Id}";
        }

        public override IEnumerable<CampaignElement> GetChilds()
        {
            return new List<DatasetInfo> { };
        }

        #endregion
    }
}