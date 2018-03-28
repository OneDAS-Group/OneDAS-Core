using HDF.PInvoke;
using OneDas.Hdf.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace OneDas.Hdf.Core
{
    [DataContract]
    public class VariableInfo : HdfElementBase
    {
        #region "Fields"

        private List<string> _variableNameSet;
        private List<string> _variableGroupSet;
        private List<string> _unitSet;
        private List<hdf_transfer_function_t> _transferFunctionSet;
        private List<DatasetInfo> _datasetInfoSet;

        #endregion

        #region "Constructors"

        public VariableInfo(string name, HdfElementBase parent, bool isLazyLoading) : base(name, parent, isLazyLoading)
        {
            _variableNameSet = new List<string>();
            _variableGroupSet = new List<string>();
            _unitSet = new List<string>();
            _transferFunctionSet = new List<hdf_transfer_function_t>();

            _datasetInfoSet = new List<DatasetInfo>();

            this.RegisterSyncGroup(nameof(this.VariableNameSet), new SyncContext(this.UpdateVariableNameSet));
            this.RegisterSyncGroup(nameof(VariableInfo), new SyncContext(this.UpdateVariableInfo));
        }

        #endregion

        #region "Properties"

        [DataMember]
        public List<string> VariableNameSet
        {
            get
            {
                this.Update(nameof(this.VariableNameSet));

                return _variableNameSet;
            }
        }

        [DataMember]
        public List<string> VariableGroupSet
        {
            get
            {
                this.Update(nameof(VariableInfo));

                return _variableGroupSet;
            }
        }

        [DataMember]
        public List<string> UnitSet
        {
            get
            {
                this.Update(nameof(VariableInfo));

                return _unitSet;
            }
        }

        [DataMember]
        public List<hdf_transfer_function_t> TransferFunctionSet
        {
            get
            {
                this.Update(nameof(VariableInfo));

                return _transferFunctionSet;
            }
        }

        [DataMember]
        public List<DatasetInfo> DatasetInfoSet
        {
            get
            {
                this.Update(nameof(VariableInfo));

                return _datasetInfoSet;
            }
        }

        #endregion

        #region "Methods"

        public override string GetPath()
        {
            return $"{ this.Parent.GetPath() }/{ this.Name }";
        }

        public override string GetDisplayName()
        {
            return $"{new string(this.VariableNameSet.Last().Take(55).ToArray()),-55}\t({ new string(this.Name.Take(8).ToArray()) }...)";
        }

        public override IEnumerable<HdfElementBase> GetChildSet()
        {
            return this.DatasetInfoSet;
        }

        protected override long GetId(long fileId)
        {
            return H5G.open(fileId, this.GetPath());
        }

        protected override void CloseId(long id)
        {
            if (H5I.is_valid(id) > 0) { H5G.close(id); }
        }

        private void UpdateVariableNameSet(FileContext fileContext, long variableGroupId)
        {
            _variableNameSet = IOHelper.UpdateAttributeList(variableGroupId, "name_set", _variableNameSet.ToArray()).ToList();
        }

        private void UpdateVariableInfo(FileContext fileContext, long variableGroupId)
        {
            ulong idx;

            idx = 0;

            _variableGroupSet = IOHelper.UpdateAttributeList(variableGroupId, "group_set", _variableGroupSet.ToArray()).ToList();

            if (fileContext.FormatVersion != 1)
            {
                _unitSet = IOHelper.UpdateAttributeList(variableGroupId, "unit_set", _unitSet.ToArray()).ToList();
                _transferFunctionSet = IOHelper.UpdateAttributeList(variableGroupId, "transfer_function_set", _transferFunctionSet.ToArray()).ToList();
            }

            H5L.iterate(variableGroupId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero);

            int Callback(long variableGroupId2, IntPtr intPtrName, ref H5L.info_t info, IntPtr userDataPtr)
            {
                long datasetId = -1;
                long typeId_do_not_close = -1;
                string name;

                DatasetInfo currentDatasetInfo;

                try
                {
                    name = Marshal.PtrToStringAnsi(intPtrName);

                    if (H5L.exists(variableGroupId2, name) > 0)
                    {
                        datasetId = H5D.open(variableGroupId2, name);

                        currentDatasetInfo = _datasetInfoSet.FirstOrDefault(datasetInfo => datasetInfo.Name == name);

                        if (currentDatasetInfo == null)
                        {
                            typeId_do_not_close = H5D.get_type(datasetId);
                            currentDatasetInfo = new DatasetInfo(name, typeId_do_not_close, this, this.IsLazyLoading);

                            _datasetInfoSet.Add(currentDatasetInfo);
                        }

                        currentDatasetInfo.Update(fileContext);
                    }
                }
                finally
                {
                    if (H5I.is_valid(datasetId) > 0) { H5D.close(datasetId); }
                }

                return 0;
            }
        }

        #endregion
    }
}
