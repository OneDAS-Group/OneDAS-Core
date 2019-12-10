using System.Collections.Generic;
using System.Linq;

namespace OneDas.Extensibility
{
    public class VariableContextGroup
    {
        public VariableContextGroup(ulong samplesPerDay, IList<VariableContext> variableContextSet)
        {
            this.SamplesPerDay = samplesPerDay;
            this.VariableContextSet = variableContextSet.ToList();
        }

        public ulong SamplesPerDay { get; }
        public List<VariableContext> VariableContextSet { get; }
    }
}
