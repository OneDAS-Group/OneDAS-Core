using OneDas.Extensibility;
using System;

namespace OneDas.Extension.CanRaw
{
    public class CanRawGateway : ExtendedDataGatewayExtensionLogicBase
    {
        private CanRawSettings _settings;

        public CanRawGateway(CanRawSettings settings) : base(settings)
        {
            _settings = settings;
            
            if (_settings.CanDriver == CanDriver.Ixxat)
            {
                
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        protected override void OnUpdateIo(DateTime referenceDateTime)
        {
            this.LastSuccessfulUpdate.Restart();
        }
    }
}
