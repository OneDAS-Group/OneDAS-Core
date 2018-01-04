using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;
using System.Timers;
using OneDas.WebServer.Core;
using OneDas.Common;
using OneDas.Infrastructure;
using OneDas.Properties;

namespace OneDas.WebServer.Shell
{
    public class OneDasConsole
    {
        #region "Fields"

        private System.Timers.Timer _timer_UpdateConsole;
        private object _syncLock_UpdateConsole;

        private SafeNativeMethods.HandlerRoutine _handlerRoutine;
        private IManagementService _managementServiceClient;

        private bool _isConnected = false;

        #endregion

        #region "Contructors"

        public OneDasConsole()
        {
            WindowHelper.ModifyConsoleMenu(SystemCommand.SC_CLOSE, 0x0);
            Thread.CurrentThread.Name = "Main thread";

            Console.SetWindowSize(80, 25);
            Console.CursorVisible = false;

            _handlerRoutine = new SafeNativeMethods.HandlerRoutine(ConsoleCtrlCallback);

            SafeNativeMethods.SetConsoleCtrlHandler(_handlerRoutine, true);

            _timer_UpdateConsole = new System.Timers.Timer(new TimeSpan(0, 0, 1).TotalMilliseconds)
            {
                AutoReset = true,
                Enabled = true
            };

            _timer_UpdateConsole.Elapsed += _timer_UpdateConsole_Elapsed;
            _syncLock_UpdateConsole = new object();

            Console.Write("initialization (standard) ... ");

        #region "to serve or not to serve?"

            Console.Title = "OneDAS Server";

            if (Bootloader.OneDasController == null)
            {
                Console.Title += " (remote control)";
            }

            _managementServiceClient = new ChannelFactory<IManagementService>(new NetNamedPipeBinding(NetNamedPipeSecurityMode.None), new EndpointAddress(ConfigurationManager<OneDasSettings>.Settings.ManagementServiceBaseAddress + "/pipe")).CreateChannel();

            Bootloader.WriteLogEntry("started in user interactive mode (console)", EventLogEntryType.Information);

        #endregion

        #region "Loop / wait for user input"

            this.ResetConsole();

            while (true)
            {
                ConsoleKey consoleKey = Console.ReadKey(true).Key;

                lock (_syncLock_UpdateConsole)
                {
                    try
                    {
                        switch (consoleKey)
                        {
                            case ConsoleKey.B:

                                // boost process priority
                                Console.WriteLine("Do you want to boost the process priority to realtime? (Y)es (N)o.");

                                if (Console.ReadKey(true).Key == ConsoleKey.Y)
                                {
                                    _managementServiceClient.BoostProcessPriority();
                                }

                                this.ResetConsole();

                                break;

                            case ConsoleKey.D:

                                // debug
                                _managementServiceClient.ToggleDebugOutput();

                                break;

                            case ConsoleKey.S:

                                // service
                                if (Bootloader.GetOneDasServiceStatus() > 0)
                                {
                                    Console.Write("Do you want to ");
                                    this.WriteColored("(u)ninstall", ConsoleColor.Red);
                                    Console.WriteLine(", (r)estart or (s)top the OneDAS server Windows service?");

                                    consoleKey = Console.ReadKey(true).Key;

                                    switch (consoleKey)
                                    {
                                        case ConsoleKey.U:
                                            Bootloader.UninstallOneDasService();
                                            Bootloader.Shutdown(true, 0);
                                            break;
                                        case ConsoleKey.R:
                                            _managementServiceClient.Shutdown(true);
                                            break;
                                        case ConsoleKey.S:
                                            _managementServiceClient.Shutdown(false);
                                            break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Do you want to install the OneDAS server Windows service? (Y)es (N)o");

                                    if (Console.ReadKey(true).Key == ConsoleKey.Y)
                                    {
                                        Bootloader.OneDasController?.Dispose(); // to allow new service to startup properly 'Improve, see also Bootloader.Shutdown
                                        Bootloader.InstallOneDasService();
                                        Bootloader.Shutdown(true, 0);
                                    }
                                }

                                this.ResetConsole();
                                break;

                            case ConsoleKey.R:

                                // restart
                                Console.WriteLine("Do you want to restart the application? (Y)es (N)o.");

                                if (Console.ReadKey(true).Key == ConsoleKey.Y)
                                {
                                    Bootloader.Shutdown(true, 0);
                                }

                                this.ResetConsole();

                                break;

                            case ConsoleKey.E:

                                // exit
                                Console.WriteLine("Do you want to shutdown the application? (Y)es (N)o.");

                                if (Console.ReadKey(true).Key == ConsoleKey.Y)
                                {
                                    Bootloader.Shutdown(false, 0);
                                }

                                this.ResetConsole();

                                break;
                        }
                    }
                    catch
                    {
                        _managementServiceClient = new ChannelFactory<IManagementService>(new NetNamedPipeBinding(NetNamedPipeSecurityMode.None), new EndpointAddress(ConfigurationManager<OneDasSettings>.Settings.ManagementServiceBaseAddress + "/pipe")).CreateChannel();
                    }
                }
            }

        #endregion

        }

        #endregion

        #region "Methods"

        private void WriteColoredLine(string text, ConsoleColor color)
        {
            this.WriteColored(text, color);
            Console.WriteLine();
        }

        private void WriteColored(string text, ConsoleColor color)
        {
            if (color == ConsoleColor.White)
            {
                color = Console.ForegroundColor;
            }

            ConsoleColor currentColor = Console.ForegroundColor;

            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = currentColor;
        }

        private void ResetConsole()
        {
            lock (_syncLock_UpdateConsole)
            {
                int offset = 38;

                // frame
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.DarkCyan;

                Console.Write($"+==============================="); this.WriteColored($" OneDAS Server ", ConsoleColor.Cyan); Console.WriteLine($"===============================+");
                Console.WriteLine($"|                                                                             |");
                Console.WriteLine($"|                                     |                                       |");
                Console.WriteLine($"|                                     |                                       |");
                Console.WriteLine($"|                                     |                                       |");
                Console.WriteLine($"|                                     |                                       |");
                Console.WriteLine($"|                                     |                                       |");
                Console.WriteLine($"|                                     |                                       |");
                Console.WriteLine($"|                                     |                                       |");
                Console.WriteLine($"|                                                                             |");
                Console.WriteLine($"+=============================================================================+");
                this.WriteColoredLine($"(b)oost process priority    (d)ebug output    (s)ervice    (r)estart    (e)xit", ConsoleColor.Cyan);

                // text
                Console.ForegroundColor = ConsoleColor.Cyan;

                Console.SetCursorPosition(2, 2);
                Console.Write("Name:");

                Console.SetCursorPosition(2, 3);
                Console.Write("Status:");

                Console.SetCursorPosition(2, 4);
                Console.Write("Process priority:");

                Console.SetCursorPosition(2, 5);
                Console.Write("Debug information:");

                Console.SetCursorPosition(2, 6);
                Console.Write("Windows service:");

                // numbers
                Console.SetCursorPosition(2 + offset, 2);
                Console.Write("Clients:");

                Console.SetCursorPosition(2 + offset, 3);
                Console.Write("Chunk period:");

                Console.SetCursorPosition(2 + offset, 4);
                Console.Write("Frequency:");

                Console.SetCursorPosition(2 + offset, 5);
                Console.Write("Late by:");

                Console.SetCursorPosition(2 + offset, 6);
                Console.Write("Cycle time:");

                Console.SetCursorPosition(2 + offset, 7);
                Console.Write("Timer drift:");

                Console.SetCursorPosition(2 + offset, 8);
                Console.Write("Processor time:");

                if (!_isConnected)
                {
                    Console.SetCursorPosition(31, 9);
                    this.WriteColoredLine("connection lost", ConsoleColor.Red);
                }

                Console.SetCursorPosition(0, 13);
            }
        }

        private void UpdateConsole()
        {
            lock (_syncLock_UpdateConsole)
            {
                try
                {
                    OneDasPerformanceInformation performanceInformation = _managementServiceClient.CreatePerformanceInformation();

                    int offset = 39;

                    if (!_isConnected)
                    {
                        _isConnected = true;
                        this.ResetConsole();
                    }

                    // text
                    Console.SetCursorPosition(7, 2);
                    Console.Write($"{ConfigurationManager<OneDasSettings>.Settings.OneDasName,30}");

                    Console.SetCursorPosition(19, 3);

                    switch (performanceInformation.OneDasState)
                    {
                        case OneDasState.Run:
                            Console.Write($"{performanceInformation.OneDasState,18}");
                            break;
                        default:
                            this.WriteColored($"{performanceInformation.OneDasState,18}", ConsoleColor.Red);
                            break;
                    }

                    Console.SetCursorPosition(26, 4);
                    this.WriteColored($"{performanceInformation.ProcessPriorityClass,11}", performanceInformation.ProcessPriorityClass == ProcessPriorityClass.RealTime ? ConsoleColor.White : ConsoleColor.Red);

                    Console.SetCursorPosition(32, 5);
                    this.WriteColored($"{performanceInformation.IsDebugOutputEnabled,5}", performanceInformation.IsDebugOutputEnabled ? ConsoleColor.Red : ConsoleColor.White);

                    Console.SetCursorPosition(22, 6);
                    ServiceControllerStatus oneDasServiceStatus = Bootloader.GetOneDasServiceStatus();
                    string text = oneDasServiceStatus == 0 ? "NotFound" : oneDasServiceStatus.ToString();
                    this.WriteColored($"{text,15}", oneDasServiceStatus == ServiceControllerStatus.Running ? ConsoleColor.White : ConsoleColor.Red);

                    // numbers
                    Console.SetCursorPosition(33 + offset, 2);
                    //Console.Write($"{performanceInformation.ClientSetCount,2}");

                    Console.SetCursorPosition(33 + offset, 3);
                    Console.WriteLine($"{Settings.Default.ChunkPeriod,2} s");

                    Console.SetCursorPosition(32 + offset, 4);
                    Console.WriteLine($"{Settings.Default.NativeSampleRate,3} Hz");

                    Console.SetCursorPosition(30 + offset, 5);
                    Console.Write($"{performanceInformation.LateBy,5:0.0} ms");

                    Console.SetCursorPosition(30 + offset, 6);
                    Console.Write($"{performanceInformation.CycleTime,5:0.0} ms");

                    Console.SetCursorPosition(28 + offset, 7);
                    Console.Write($"{(int)(performanceInformation.TimerDrift / 1000),7} µs");

                    Console.SetCursorPosition(28 + offset, 8);
                    Console.Write($"{performanceInformation.CpuTime,7:0} %");

                    Console.SetCursorPosition(0, 13);
                }
                catch
                {
                    _isConnected = false;
                    this.ResetConsole();
                    _managementServiceClient = new ChannelFactory<IManagementService>(new NetNamedPipeBinding(NetNamedPipeSecurityMode.None), new EndpointAddress(ConfigurationManager<OneDasSettings>.Settings.ManagementServiceBaseAddress + "/pipe")).CreateChannel();
                }
            }
        }

        private void ConsoleCtrlCallback(CtrlType ctrlType)
        {
            switch (ctrlType)
            {
                case CtrlType.CTRL_CLOSE_EVENT:
                case CtrlType.CTRL_BREAK_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                    Bootloader.Shutdown(false, 0);
                    break;
                default:
                    return;
            }
        }

        #endregion

        #region "Callbacks"

        private void _timer_UpdateConsole_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.UpdateConsole();
        }

        #endregion
    }
}