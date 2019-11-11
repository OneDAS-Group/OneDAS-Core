using EtherCAT.NET;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneDas.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.Extension.Ethercat
{
    public class EthercatGateway : DataGatewayExtensionLogicBase, IReferenceClock
    {
        #region "Fields"

        private ILogger _logger;
        private IExtensionFactory _extensionFactory;
        private EthercatSupporter _ethercatSupporter;
        private EthercatSettings _settings;
        private EcMaster _ecMaster;
        private OneDasOptions _options;

        #endregion

        #region "Constructors"

        public EthercatGateway(EthercatSettings settings, IExtensionFactory extensionFactory, ILoggerFactory loggerFactory, IOptions<OneDasOptions> options) : base(settings)
        {
            EcSettings ecSettings;

            _settings = settings;
            _extensionFactory = extensionFactory;
            _logger = loggerFactory.CreateLogger(this.DisplayName);
            _options = options.Value;

            _ethercatSupporter = (EthercatSupporter)extensionFactory.BuildSupporter(typeof(EthercatSettings));
            ecSettings = new EcSettings(OneDasConstants.NativeSampleRate, _ethercatSupporter.EsiSourceDirectoryPath, settings.NicHardwareAddress);
            _ecMaster = new EcMaster(ecSettings, extensionFactory, _logger);
        }

        #endregion

        #region "Methods"

        public override IEnumerable<DataPort> GetDataPortSet()
        {
            if (_settings.RootSlaveInfo != null)
            {
                return _settings.RootSlaveInfo.Descendants().SelectMany(x => x.GetVariableSet()).Cast<DataPort>().ToList();
            }
            else
            {
                return new List<DataPort>();
            }
        }

        public DateTime GetUtcDateTime()
        {
            return _ecMaster.UtcDateTime;
        }

        public long GetTimerDrift()
        {
            return -_ecMaster.DcRingBufferAverage;
        }

        protected override void OnConfigure()
        {
            _ecMaster.Configure();
        }

        protected override void OnUpdateIo(DateTime referenceDateTime)
        {
            _ecMaster.UpdateIO(referenceDateTime);
            this.LastSuccessfulUpdate.Restart();
        }

        protected override void FreeUnmanagedResources()
        {
            _ecMaster?.Dispose();
        }

        #endregion
    }
}
