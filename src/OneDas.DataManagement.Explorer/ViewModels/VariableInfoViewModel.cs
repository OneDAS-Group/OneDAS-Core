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

        public string Name => _variable.VariableNames.Last();

        public string Group => _variable.VariableGroups.Last();

        public string Unit => _variableMeta == null ? _variable.Units.Last() : _variableMeta.Unit;

        public string Description => _variableMeta == null ? "<no description available>" : _variableMeta.Description;

        public CampaignInfo Parent => (CampaignInfo)_variable.Parent;

        public List<TransferFunction> TransferFunctions => _variableMeta == null ? new List<TransferFunction>() : _variableMeta.TransferFunctions;

        public List<DatasetInfoViewModel> Datasets { get; private set; }

        #endregion
    }
}
