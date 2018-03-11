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
        }

        #endregion

        #region "Properties"

        [DataMember]
        public List<string> VariableNameSet
        {
            get
            {
                this.Update();

                return _variableNameSet;
            }
        }

        [DataMember]
        public List<string> VariableGroupSet
        {
            get
            {
                this.Update();

                return _variableGroupSet;
            }
        }

        [DataMember]
        public List<string> UnitSet
        {
            get
            {
                this.Update();

                return _unitSet;
            }
        }

        [DataMember]
        public List<hdf_transfer_function_t> TransferFunctionSet
        {
            get
            {
                this.Update();

                return _transferFunctionSet;
            }
        }

        [DataMember]
        public List<DatasetInfo> DatasetInfoSet
        {
            get
            {
                this.Update();

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

        protected override void OnUpdate(FileContext fileContext)
        {
            ulong idx;
            long variableGroupId;

            idx = 0;
            variableGroupId = H5G.open(fileContext.FileId, this.GetPath());

            try
            {
                _variableNameSet = IOHelper.UpdateAttributeList(variableGroupId, "name_set", _variableNameSet.ToArray()).ToList();
                _variableGroupSet = IOHelper.UpdateAttributeList(variableGroupId, "group_set", _variableGroupSet.ToArray()).ToList();

                if (fileContext.FormatVersion > 1)
                {
                    _unitSet = IOHelper.UpdateAttributeList(variableGroupId, "unit_set", _unitSet.ToArray()).ToList();
                    _transferFunctionSet = IOHelper.UpdateAttributeList(variableGroupId, "transfer_function_set", _transferFunctionSet.ToArray()).ToList();
                }

                // _databaseInfoSet
                H5L.iterate(variableGroupId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback, IntPtr.Zero);
            }
            finally
            {
                if (H5I.is_valid(variableGroupId) > 0) { H5G.close(variableGroupId); }
            }

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
