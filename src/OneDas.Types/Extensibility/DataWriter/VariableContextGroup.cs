using OneDas.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.Extensibility
{
    public class VariableContextGroup
    {
        public VariableContextGroup(SampleRateContainer sampleRate, IList<VariableContext> variableContextSet)
        {
            this.SampleRate = sampleRate;
            this.VariableContextSet = variableContextSet.ToList();
        }

        public SampleRateContainer SampleRate { get; }
        public List<VariableContext> VariableContextSet { get; }
    }
}
