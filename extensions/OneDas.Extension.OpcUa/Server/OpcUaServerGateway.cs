using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneDas.Extensibility;
using Opc.Ua;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

// %appdata%\Microsoft\SystemCertificates\My\Certificates
// cd CERT:\\
// ls .\\CurrentUser
// ls .\\CurrentUser\My

namespace OneDas.Extension.OpcUa
{
    public class OpcUaServerGateway : ExtendedDataGatewayExtensionLogicBase
    {
        #region "Fields"

        private uint _cycleCounter;

        private OneDasOptions _options;
        private OpcUaServerSettings _settings;
        private OpcUaServer _opcServer;

        #endregion

        #region "Constructors"

        public OpcUaServerGateway(OpcUaServerSettings settings, ILogger logger, IOptions<OneDasOptions> options) 
            : base(settings, logger)
        {
            _settings = settings;
            _options = options.Value;

            this.LastSuccessfulUpdate.Restart();
        }

        #endregion

        #region "Methods"

        protected override void OnConfigure()
        {
            string configPath;
            ApplicationConfiguration config;
            X509Certificate2 certificate;

            configPath = Path.Combine(_options.ConfigurationDirectoryPath, @"OPC-UA/configuration.xml");

            base.OnConfigure();

            config = ApplicationConfiguration.Load(new FileInfo(configPath), ApplicationType.Server, null, true).Result;
            config.CertificateValidator.CertificateValidation += (sender, e) => e.Accept = true;

            if (config.SecurityConfiguration.ApplicationCertificate.Certificate == null)
            {
                certificate = CertificateManager.CreateApplicationInstanceCertificate(config, lifeTimeInMonths: 36).Result;
                this.Logger.LogInformation($"Certificate with thumbprint = {certificate.Thumbprint} created.");
            }

            _opcServer = new OpcUaServer((OpcUaServerSettings)this.Settings, this.Logger);
            _opcServer.Start(config);
        }

        protected override void OnUpdateIo(DateTime referenceDateTime)
        {
            int count;
            Span<float> buffer;
            long referenceDateTimeUnix;

            referenceDateTimeUnix = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

            if (_cycleCounter % _settings.FrameRateDivider == 0)
            {
                count = 0;
                buffer = MemoryMarshal.Cast<byte, float>(this.GetOutputBuffer());

                _opcServer.CustomNodeManager.VariableSet.First().Value = referenceDateTimeUnix;

                foreach (var variable in _opcServer.CustomNodeManager.VariableSet.Skip(1))
                {
                    variable.Value = buffer[count];
                    variable.Timestamp = referenceDateTime;
                    count++;
                }

                // TODO: get reference to parent more secure
                _opcServer.CustomNodeManager.VariableSet[0].Parent.ClearChangeMasks(_opcServer.CustomNodeManager.SystemContext, true);

                this.LastSuccessfulUpdate.Restart();
            }

            _cycleCounter++;

            if (_cycleCounter >= OneDasConstants.NativeSampleRate)
            {
                _cycleCounter = 0;
            }
        }

        protected override void FreeManagedResources()
        {
            base.FreeManagedResources();

            _opcServer?.Dispose();
        }

        #endregion
    }
}
