using OneDas.DataManagement.Database;
using OneDas.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.DataManagement.Explorer.ViewModels
{
    public class VariableInfoViewModel
    {
        #region Fields

        private VariableInfo _variable;
        private VariableMetaInfo _variableMeta;

        #endregion

        #region Constructors

        public VariableInfoViewModel(VariableInfo variable, VariableMetaInfo variableMeta)
        {
            _variable = variable;
            _variableMeta = variableMeta;

            this.Datasets = variable.Datasets
                .Where(dataset => !dataset.Id.EndsWith("_status"))
                .Select(dataset => new DatasetInfoViewModel(dataset, this)).ToList();
        }

        #endregion

        #region Properties

        public string Id => _variable.Id;

        public string Name => _variable.Name;

        public string Group => _variable.Group;

        public string Unit => !string.IsNullOrWhiteSpace(_variableMeta.Unit) 
            ? _variableMeta.Unit 
            : _variable.Unit;

        public string Description
        {
            get { return _variableMeta.Description; }
            set { _variableMeta.Description = value; }
        }

        public string SpecialInfo
        {
            get { return _variableMeta.SpecialInfo; }
            set { _variableMeta.SpecialInfo = value; }
        }

        public ProjectInfo Parent => (ProjectInfo)_variable.Parent;

        public List<TransferFunction> TransferFunctions => _variableMeta.TransferFunctions.Any() 
            ? _variableMeta.TransferFunctions
            : _variable.TransferFunctions;

        public List<DatasetInfoViewModel> Datasets { get; private set; }

        #endregion
    }
}
