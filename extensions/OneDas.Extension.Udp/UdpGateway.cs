using OneDas.Extensibility;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.Extension.Udp
{
    public class UdpGateway : ExtendedDataGatewayExtensionLogicBase
    {
        #region "Fields"

        private UdpSettings _settings;
        private UdpClient _udpClientInput;
        private UdpClient _udpClientOutput;
        private IPEndPoint _localEndpoint;
        private IPEndPoint _remoteEndpoint;

        private Task _udpTask;
        private CancellationTokenSource _cts;

        private byte[] _udpBuffer;
        private uint _cycleCounter;
        private bool _transmitData;

        #endregion

        #region "Constructors"

        public UdpGateway(UdpSettings settings) : base(settings)
        {
            _settings = settings;

            _cycleCounter = 0;
        }

        #endregion

        #region "Methods"

        protected override void OnConfigure()
        {
            UdpReceiveResult udpReceiveResult;
            int inputBufferSize;

            base.OnConfigure();

            inputBufferSize = this.GetInputBuffer().Length;
            _transmitData = _settings.GetOutputModuleSet().Any();

            // udp
            _localEndpoint = new IPEndPoint(IPAddress.Any, (int)_settings.LocalDataPort);
            _udpClientInput = this.CreateUdpClient(_localEndpoint);

            _remoteEndpoint = new IPEndPoint(IPAddress.Parse(_settings.RemoteIpAddress), (int)_settings.RemoteDataPort);
            _udpClientOutput = new UdpClient();

            _cts = new CancellationTokenSource();

            if (_settings.GetInputModuleSet().Any())
            {
                _udpTask = Task.Run(async () =>
                {
                    while (!_cts.IsCancellationRequested)
                    {
                        try
                        {
                            udpReceiveResult = await _udpClientInput.ReceiveAsync();

                            // check if remote IP address is correct
                            if (!udpReceiveResult.RemoteEndPoint.Address.Equals(_remoteEndpoint.Address))
                                continue;

                            // check if udpData size is correct
                            if (udpReceiveResult.Buffer.Length != inputBufferSize)
                                continue;

                            _udpBuffer = udpReceiveResult.Buffer;
                            this.LastSuccessfulUpdate.Restart();
                        }
                        catch (ObjectDisposedException)
                        {
                            //
                        }
                    }
                }, _cts.Token);
            }
        }

        protected override void OnUpdateIo(DateTime referenceDateTime)
        {
            Span<byte> outputBuffer;

            // input
            if (_udpBuffer != null) // new udpData received
            {
                _udpBuffer.CopyTo(this.GetInputBuffer());
                _udpBuffer = null;
            }

            // output
            if (_cycleCounter % _settings.FrameRateDivider == 0 && _transmitData)
            {
                outputBuffer = this.GetOutputBuffer();
                _udpClientOutput.Send(outputBuffer.ToArray(), outputBuffer.Length, _remoteEndpoint);
            }

            _cycleCounter++;

            if (_cycleCounter >= OneDasConstants.NativeSampleRate)
                _cycleCounter = 0;
        }

        public UdpClient CreateUdpClient(IPEndPoint ipEndPoint)
        {
            UdpClient udpClient;

            udpClient = new UdpClient();
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.Client.Bind(ipEndPoint);

            return udpClient;
        }

        protected override void FreeManagedResources()
        {
            base.FreeManagedResources();

            _udpClientInput?.Close();
            _udpClientOutput?.Close();
            _cts?.Cancel();

            try
            {
                _udpTask?.Wait();
            }
            catch (Exception ex) when (ex.InnerException.GetType() == typeof(TaskCanceledException))
            {
                //
            }
        }

        #endregion
    }
}
