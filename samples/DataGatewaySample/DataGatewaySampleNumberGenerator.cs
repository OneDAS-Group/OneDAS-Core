using System;
using System.Linq;

namespace OneDas.Extensibility.DataGateway.DataGatewaySample
{
    public unsafe class DataGatewaySampleNumberGenerator<T> : DataGatewaySampleNumberGeneratorBase where T : struct
    {
        public DataGatewaySampleNumberGenerator(DataPort dataPort, Random random): base(dataPort, random)
        {
            //
        }

        public override void Update()
        {
            Span<T> value;

            value = new Span<T>(this.DataPort.DataPtr.ToPointer(), 1);
            this.Buffer.Enqueue(this.Random.NextDouble() * 127);

            if (this.Buffer.Count > 10)
            {
                this.Buffer.Dequeue();
            }

            value[0] = (T)Convert.ChangeType(this.Buffer.Average(), typeof(T));
        }
    }
}
