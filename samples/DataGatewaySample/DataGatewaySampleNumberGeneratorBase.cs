using System;
using System.Collections.Generic;

namespace OneDas.Extensibility.DataGateway.DataGatewaySample
{
    public abstract class DataGatewaySampleNumberGeneratorBase
    {
        public DataGatewaySampleNumberGeneratorBase(DataPort dataPort, Random random)
        {
            this.DataPort = dataPort;

            this.Random = random;
            this.Buffer = new Queue<double>();
        }

        protected DataPort DataPort { get; private set; }
        protected Random Random { get; private set; }
        protected Queue<double> Buffer { get; private set; }

        public abstract void Update();
    }
}