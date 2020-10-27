using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OneDas.DataManagement.Database
{
    [DebuggerDisplay("{Id,nq}")]
    public class ProjectInfo : ProjectElement
    {
        #region "Constructors"

        public ProjectInfo(string id) : base(id, null)
        {
            this.Variables = new List<VariableInfo>();
        }

        private ProjectInfo()
        {
            //
        }

        #endregion

        #region "Properties"

        public DateTime ProjectStart { get; set; }

        public DateTime ProjectEnd { get; set; }

        public List<VariableInfo> Variables { get; set; }

        #endregion

        #region "Methods"

        public void Merge(ProjectInfo project, VariableMergeMode mergeMode)
        {
            if (this.Id != project.Id)
                throw new Exception("The project to be merged has a different ID.");

            // merge variables
            List<VariableInfo> newVariables = new List<VariableInfo>();

            foreach (var variable in project.Variables)
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
            if (this.ProjectStart == DateTime.MinValue)
                this.ProjectStart = project.ProjectStart;
            else
                this.ProjectStart = new DateTime(Math.Min(this.ProjectStart.Ticks, project.ProjectStart.Ticks));

            if (this.ProjectEnd == DateTime.MinValue)
                this.ProjectEnd = project.ProjectEnd;
            else
                this.ProjectEnd = new DateTime(Math.Max(this.ProjectEnd.Ticks, project.ProjectEnd.Ticks));
        }

        public override string GetPath()
        {
            return this.Id;
        }

        public override IEnumerable<ProjectElement> GetChilds()
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
