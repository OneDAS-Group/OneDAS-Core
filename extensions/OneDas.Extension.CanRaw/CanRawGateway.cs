using OneDas.Extensibility;
using System;

namespace OneDas.Extension.CanRaw
{
    public class CanRawGateway : ExtendedDataGatewayExtensionLogicBase
    {
        private CanRawSettings _settings;
        private Random _random;

        public CanRawGateway(CanRawSettings settings) : base(settings)
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
