using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.Extensibility.DataGatewaySample
{
    public class DataGatewaySampleGateway : ExtendedDataGatewayExtensionLogicBase
    {
        private DataGatewaySampleSettings _settings;
        private List<DataGatewaySampleNumberGeneratorBase> _numberGeneratorSet;

        public DataGatewaySampleGateway(DataGatewaySampleSettings settings) : base(settings)
        {
            Random random;

            random = new Random();
            _settings = settings;

            _numberGeneratorSet = this.DataPortSet.Where(dataPort => dataPort.DataDirection == DataDirection.Input).ToList().Select(dataPort =>
            {
                Type type;

                type = typeof(DataGatewaySampleNumberGenerator<>).MakeGenericType(OneDasUtilities.GetTypeFromOneDasDataType(dataPort.DataType));

                return (DataGatewaySampleNumberGeneratorBase)Activator.CreateInstance(type, dataPort, random);
            }).ToList();
        }

        protected override void OnUpdateIo(DateTime referenceDateTime)
        {
            _numberGeneratorSet.ForEach(genericGenerator => genericGenerator.Update());

            this.LastSuccessfulUpdate.Restart();
        }
    }
}
