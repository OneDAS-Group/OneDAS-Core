using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneDas.Common;
using OneDas.Engine.Core;
using OneDas.Infrastructure;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Timers;

namespace OneDas.WebServer.Shell
{
    public class OneDasConsole
    {
        #region "Fields"

        private System.Timers.Timer _timer_UpdateConsole;
        private object _syncLock_UpdateConsole;

        private SafeNativeMethods.HandlerRoutine _handlerRoutine;
        private HubConnection _consoleHubClient;
        private WebServerOptions _webServerOptions;

        private bool _isConnected = false;

        #endregion

        #region "Contructors"

        public OneDasConsole(IOptions<WebServerOptions> options)
        {
            _webServerOptions = options.Value;

            WindowHelper.ModifyConsoleMenu(SystemCommand.SC_CLOSE, 0x0);
            Thread.CurrentThread.Name = "Main thread";

            Console.SetWindowSize(80, 25);
            Console.CursorVisible = false;

            _syncLock_UpdateConsole = new object();
            _handlerRoutine = new SafeNativeMethods.HandlerRoutine(ConsoleCtrlCallback);

            SafeNativeMethods.SetConsoleCtrlHandler(_handlerRoutine, true);

            Console.Write("initialization (standard) ... ");
            BasicBootloader.SystemLogger.LogInformation("started in user interactive mode (console)");        }

        #endregion

        #region "Methods"

        public void Run(bool isHosting)
        {
            _consoleHubClient = this.GetNewConnection();

            // timer
            _timer_UpdateConsole = new System.Timers.Timer(new TimeSpan(0, 0, 1).TotalMilliseconds)
            {
                AutoReset = true,
                Enabled = true
            };

            _timer_UpdateConsole.Elapsed += _timer_UpdateConsole_Elapsed;

            // to serve or not to serve?
            Console.Title = "OneDAS Server";

            if (!isHosting)
            {
                Console.Title += " (remote control)";
            }

            // wait for user input (loop)
            this.ResetConsole();

            while (true)
            {
                ConsoleKey consoleKey;

                consoleKey = Console.ReadKey(true).Key;

                try
                {
                    lock (_syncLock_UpdateConsole)
                    {
                        switch (consoleKey)
                        {
                            case ConsoleKey.D:

                                // debug
                                _consoleHubClient.InvokeAsync("ToggleDebugOutput").Wait();

                                break;

                            case ConsoleKey.E:

                                // exit
                                Console.WriteLine("Do you want to shutdown the application? (Y)es (N)o.");

                                if (Console.ReadKey(true).Key == ConsoleKey.Y)
                                {
                                    BasicBootloader.Shutdown(0);
                                }

                                this.ResetConsole();

                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    _consoleHubClient = this.GetNewConnection();
                }
            }
        }

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
                this.WriteColoredLine($"(d)ebug output                                                          (e)xit", ConsoleColor.Cyan);

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

        private async void Update()
        {
            OneDasPerformanceInformation performanceInformation;

            try
            {
                performanceInformation = await _consoleHubClient.InvokeAsync<OneDasPerformanceInformation>("GetPerformanceInformation");

                lock (_syncLock_UpdateConsole)
                {
                    int offset = 39;

                    if (!_isConnected)
                    {
                        _isConnected = true;
                        this.ResetConsole();
                    }

                    // text
                    Console.SetCursorPosition(7, 2);
                    // empty

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
                    ServiceControllerStatus oneDasServiceStatus = BasicBootloader.GetOneDasServiceStatus();
                    string text = oneDasServiceStatus == 0 ? "NotFound" : oneDasServiceStatus.ToString();
                    this.WriteColored($"{text,15}", oneDasServiceStatus == ServiceControllerStatus.Running ? ConsoleColor.White : ConsoleColor.Red);

                    // numbers
                    Console.SetCursorPosition(33 + offset, 2);
                    // empty

                    Console.SetCursorPosition(33 + offset, 3);
                    // empty

                    Console.SetCursorPosition(32 + offset, 4);
                    // empty

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
            }
            catch (Exception)
            {
                _isConnected = false;

                this.ResetConsole();

                _consoleHubClient = this.GetNewConnection();
            }
        }

        private void ConsoleCtrlCallback(CtrlType ctrlType)
        {
            switch (ctrlType)
            {
                case CtrlType.CTRL_CLOSE_EVENT:
                case CtrlType.CTRL_BREAK_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                    BasicBootloader.Shutdown(0);
                    break;
                default:
                    return;
            }
        }

        private HubConnection BuildHubConnection()
        {
            return new HubConnectionBuilder()
                 .WithUrl($"{ _webServerOptions.AspBaseUrl }/{ _webServerOptions.ConsoleHubName }")
                 .Build();
        }

        private HubConnection GetNewConnection()
        {
            HubConnection hubConnection;

            while (true)
            {
                hubConnection = this.BuildHubConnection();

                try
                {
                    hubConnection.StartAsync().Wait();

                    break;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }

            return hubConnection;
        }

        #endregion

        #region "Callbacks"

        private void _timer_UpdateConsole_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Update();
        }

        #endregion
    }
}