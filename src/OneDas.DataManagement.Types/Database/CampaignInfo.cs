using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OneDas.DataManagement.Database
{
    [DebuggerDisplay("{Id,nq}")]
    public class CampaignInfo : CampaignElement
    {
        #region "Constructors"

        public CampaignInfo(string id) : base(id, null)
        {
            this.Variables = new List<VariableInfo>();
        }

        private CampaignInfo()
        {
            //
        }

        #endregion

        #region "Properties"

        public DateTime CampaignStart { get; set; }

        public DateTime CampaignEnd { get; set; }

        public List<VariableInfo> Variables { get; set; }

        #endregion

        #region "Methods"

        public void Merge(CampaignInfo campaign, VariableMergeMode mergeMode)
        {
            if (this.Id != campaign.Id)
                throw new Exception("The campaign to be merged has a different ID.");

            // merge variables
            List<VariableInfo> newVariables = new List<VariableInfo>();

            foreach (var variable in campaign.Variables)
            {
                var referenceVariable = this.Variables.FirstOrDefault(current => current.Id == variable.Id);

                if (referenceVariable != null)
                    referenceVariable.Merge(variable, mergeMode);
                else
                    newVariables.Add(variable);

                variable.Parent = this;
            }

            this.Variables.AddRange(newVariables);

            // merge other data
            if (this.CampaignStart == DateTime.MinValue)
                this.CampaignStart = campaign.CampaignStart;
            else
                this.CampaignStart = new DateTime(Math.Min(this.CampaignStart.Ticks, campaign.CampaignStart.Ticks));

            if (this.CampaignEnd == DateTime.MinValue)
                this.CampaignEnd = campaign.CampaignEnd;
            else
                this.CampaignEnd = new DateTime(Math.Max(this.CampaignEnd.Ticks, campaign.CampaignEnd.Ticks));
        }

        public override string GetPath()
        {
            return this.Id;
        }

        public override IEnumerable<CampaignElement> GetChilds()
        {
            return this.Variables;
        }

        public override void Initialize()
        {
            base.Initialize();

            foreach (var variable in this.Variables)
            {
                variable.Parent = this;
                variable.Initialize();
            }
        }

        #endregion
    }
}
