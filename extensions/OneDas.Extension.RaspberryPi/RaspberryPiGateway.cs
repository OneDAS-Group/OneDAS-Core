using Microsoft.Extensions.Logging;
using OneDas.Extensibility;
using System;

namespace OneDas.Extension.RaspberryPi
{
    public class RaspberryPiGateway : ExtendedDataGatewayExtensionLogicBase
    {
        private RaspberryPiSettings _settings;
        private Random _random;

        public RaspberryPiGateway(RaspberryPiSettings settings, ILogger logger) : base(settings, logger)
        {
            _settings = settings;
            _random = new Random();
        }

        protected override void OnUpdateIo(DateTime referenceDateTime)
        {
            this.LastSuccessfulUpdate.Restart();
        }
    }
}
