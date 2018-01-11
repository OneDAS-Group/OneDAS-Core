using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneDas.Common;
using OneDas.Infrastructure;
using OneDas.Plugin;
using OneDas.Types.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.Engine.Core
{
    /// <summary>
    /// Represents the manager of the OneDAS itself. Acts as gateway for incoming requests.
    /// </summary>
    public partial class OneDasEngine : IDisposable
    {
        #region "Events"

        public event EventHandler<OneDasStateChangedEventArgs> OneDasStateChanged;

        #endregion

        #region "Fields"

        // one time initialization
        private const int TIMER_SHIFT = 2;
        private const int STORAGE_COUNT = 2;

        private long _baseFrequency_To_DateTime;
        private double _ratedCycleTime_Ms;
        private object _syncLock;

        private RtTimer _timer_UpdateIo;
        private OneDasState _oneDasState;
        private ExecutionState _executionState;

        private Thread _storageThread;
        private AutoResetEvent _storageAutoResetEvent;
        private CancellationTokenSource _storageCancellationTokenSource;
        private System.Timers.Timer _timer_UpdateDebugInformation;

        private TimeSpan _chunkPeriod;

        private ILogger _systemLogger;
        private ILogger _oneDasEngineLogger;

        PluginManager _pluginManager;
        OneDasOptions _oneDasOptions;

        // reset required	
        private long _timerDrift;

        private int _chunkIndex;
        private int _currentStorageIndex;
        private int _cachedDataStorageIndex;
        private int _timerLateCounter;
        private int _cycleTimeTooLongCounter;

        private float _cpuTime;

        private double _cycleTime;
        private double _maxCycleTime;
        private double _lateBy;
        private double _maxLateBy;

        private DateTime _chunkDateTime;
        private DateTime _cachedChunkDateTime;
        private DateTime _lastChunkDateTime;
        private DateTime _lastUtcDateTime;
        private DateTime _utcDateTime;
        private DateTime _utcDateTime_Rounded;
        private DateTime _lastActivationDateTime;

        private Dictionary<SampleRate, IEnumerable<DataStorageContext>> _sampleRateInputDictionary;
        private Dictionary<SampleRate, IEnumerable<DataStorageContext>> _sampleRateToDatStorageContextMap;
        private Dictionary<DataGatewayPluginLogicBase, bool> _hasValidDataSet;

        // overwritten
        private Project _project;
        private Exception _exception;
        private List<DataGatewayPluginLogicBase> _dataGatewaySet;
        private List<DataWriterPluginLogicBase> _dataWriterSet;
        private IList<List<DataStorageBase>> _dataStorageSet;
        private IReferenceClock _referenceClock;

        #endregion

        #region "Constructors"

        public OneDasEngine(PluginManager pluginManager, IOptions<OneDasOptions> options, ILoggerFactory loggerFactory)
        {
            Version minimumVersion;
            List<Type> pluginTypeSet;

            _pluginManager = pluginManager;
            _oneDasOptions = options.Value;

            // state
            _oneDasState = OneDasState.Initialization;

            Directory.CreateDirectory(_oneDasOptions.BaseDirectoryPath);
            Directory.CreateDirectory(Path.Combine(_oneDasOptions.BaseDirectoryPath, "backup"));
            Directory.CreateDirectory(Path.Combine(_oneDasOptions.BaseDirectoryPath, "data"));
            Directory.CreateDirectory(Path.Combine(_oneDasOptions.BaseDirectoryPath, "debug"));
            Directory.CreateDirectory(Path.Combine(_oneDasOptions.BaseDirectoryPath, "packages"));
            Directory.CreateDirectory(Path.Combine(_oneDasOptions.BaseDirectoryPath, "plugin", "DataGateway"));
            Directory.CreateDirectory(Path.Combine(_oneDasOptions.BaseDirectoryPath, "plugin", "DataWriter"));
            Directory.CreateDirectory(Path.Combine(_oneDasOptions.BaseDirectoryPath, "project"));

            // load plugins
            minimumVersion = new Version(new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion).Major, 0, 0, 0);

            pluginTypeSet = _pluginManager.LoadAssemblies(Path.Combine(_oneDasOptions.BaseDirectoryPath, "plugin", "DataGateway"), "OneDAS", minimumVersion);

            _pluginManager.AddRange<DataGatewayPluginLogicBase>(pluginTypeSet.Where(pluginType => typeof(DataGatewayPluginLogicBase).IsAssignableFrom(pluginType)).ToList());
            _pluginManager.AddRange<DataGatewayPluginSettingsBase>(pluginTypeSet.Where(pluginType => typeof(DataGatewayPluginSettingsBase).IsAssignableFrom(pluginType)).ToList());

            pluginTypeSet = _pluginManager.LoadAssemblies(Path.Combine(_oneDasOptions.BaseDirectoryPath, "plugin", "DataWriter"), "OneDAS", minimumVersion);

            _pluginManager.AddRange<DataWriterPluginLogicBase>(pluginTypeSet.Where(pluginType => typeof(DataWriterPluginLogicBase).IsAssignableFrom(pluginType)).ToList());
            _pluginManager.AddRange<DataWriterPluginSettingsBase>(pluginTypeSet.Where(pluginType => typeof(DataWriterPluginSettingsBase).IsAssignableFrom(pluginType)).ToList());

            // logging
            _systemLogger = loggerFactory.CreateLogger("System");
            _oneDasEngineLogger = loggerFactory.CreateLogger("Engine");

            // 
            this.IsDebugOutputEnabled = false;

            _baseFrequency_To_DateTime = Convert.ToInt64(10000000L / _oneDasOptions.NativeSampleRate);
            _timer_UpdateIo = new RtTimer();
            _executionState = SafeNativeMethods.SetThreadExecutionState(ExecutionState.CONTINUOUS | ExecutionState.SYSTEM_REQUIRED);
            _ratedCycleTime_Ms = 1.0 / _oneDasOptions.NativeSampleRate * 1000.0;

            _storageThread = new Thread(() => this.StoreData())
            {
                Priority = ThreadPriority.Lowest,
                IsBackground = false,
                Name = "Storage"
            };

            _storageAutoResetEvent = new AutoResetEvent(false);
            _storageCancellationTokenSource = new CancellationTokenSource();

            _timer_UpdateDebugInformation = new System.Timers.Timer(new TimeSpan(0, 0, 1).TotalMilliseconds)
            {
                AutoReset = true,
                Enabled = true
            };

            _timer_UpdateDebugInformation.Elapsed += OnTimerUpdateDebugInformationElapsed;

            //_performanceCounter_Cpu = New PerformanceCounter("Processor", "% Processor Time", "_Total")
            _chunkPeriod = new TimeSpan(0, 0, (int)_oneDasOptions.ChunkPeriod);
            _syncLock = new object();

            // process priority
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime; // try to get even higher

            GcNotification.GcDone += GcNotification_GcOccured;

            _storageThread.Start();

            _oneDasState = OneDasState.Unconfigured;
        }

        #endregion

        #region "Properties"

        public bool IsDebugOutputEnabled { get; set; }

        /// <summary>
        /// Gets or sets the last error that occured in the OneDAS server.
        /// </summary>
        /// <returns>Returns last error that occured in the OneDAS server.</returns>
        public string LastError { get; private set; }

        /// <summary>
        /// Gets or sets the state of the OneDAS.
        /// </summary>
        /// <returns>Returns the state of the OneDAS.</returns>
        public OneDasState OneDasState
        {
            get
            {
                return _oneDasState;
            }
            set
            {
                if (this.OneDasState == value)
                {
                    return;
                }

                bool isTransitionValid = false;

                if (this.OneDasState == OneDasState.Error && value != OneDasState.Error && value != OneDasState.Initialization)
                {
                    throw new Exception(ErrorMessage.OneDasEngine_SystemFaultedAcknowledgeFirst);
                }

                switch (value)
                {
                    case OneDasState.Initialization:

                        if (this.OneDasState == OneDasState.Error)
                        {
                            isTransitionValid = true;
                        }

                        break;

                    case OneDasState.Unconfigured:

                        if (this.OneDasState == OneDasState.Initialization || this.OneDasState == OneDasState.ApplyConfiguration)
                        {
                            isTransitionValid = true;
                        }

                        break;

                    case OneDasState.ApplyConfiguration:

                        if (this.OneDasState == OneDasState.Unconfigured || this.OneDasState == OneDasState.Ready)
                        {
                            isTransitionValid = true;
                        }

                        break;

                    case OneDasState.Ready:

                        if (this.OneDasState == OneDasState.ApplyConfiguration || this.OneDasState == OneDasState.Run)
                        {
                            isTransitionValid = true;
                        }

                        break;

                    case OneDasState.Run:

                        if (this.OneDasState == OneDasState.Ready)
                        {
                            isTransitionValid = true;
                        }

                        break;

                    case OneDasState.Error:

                        isTransitionValid = true;

                        break;
                }

                if (!isTransitionValid)
                {
                    throw new Exception($"The requested transition from state { this.OneDasState } to state { value } is invalid.");
                }

                if (value != OneDasState.Error)
                {
                    this.LastError = string.Empty;
                }

                _systemLogger.LogInformation($"Performed transition from state { _oneDasState.ToString() } to state { value.ToString() }.");

                this.OnOneDasStateChanged(_oneDasState, value);
                _oneDasState = value;
            }
        }

        public Project Project
        {
            get
            {
                return _project;
            }
        }

        #endregion

        #region "Methods"

        public OneDasPerformanceInformation CreatePerformanceInformation()
        {
            return new OneDasPerformanceInformation(this.OneDasState, Process.GetCurrentProcess().PriorityClass,
                this.IsDebugOutputEnabled,
                _lateBy,
                _cycleTime,
                _timerDrift,
                _cpuTime,
                (int)((DateTime.UtcNow - _lastActivationDateTime).TotalSeconds));
        }

        public IEnumerable<object> CreateDataSnapshot(IList<ChannelHub> channelHubSet = null)
        {
            if (this.OneDasState >= OneDasState.Ready)
            {
                lock (_syncLock)
                {
                    if (channelHubSet != null)
                    {
                        return channelHubSet.Select(channelHub => this.ReadSnapshotData(channelHub, _currentStorageIndex, _chunkIndex)).ToList();
                    }
                    else
                    {
                        return this.Project.ActiveChannelHubSet.Select(channelHub => this.ReadSnapshotData(channelHub, _currentStorageIndex, _chunkIndex)).ToList();
                    }
                }
            }
            else
            {
                throw new Exception(ErrorMessage.OneDasEngine_OneDasStateNotGreaterEqualReady);
            }
        }

        private object ReadSnapshotData(ChannelHub channelHub, int currentStorageIndex, long currentChunkIndex)
        {
            int realChunkIndex;

            realChunkIndex = Convert.ToInt32(currentChunkIndex / (int)channelHub.SampleRate);

            if (channelHub.AssociatedDataStorageSet[currentStorageIndex].ReadStatus(realChunkIndex) == 1)
            {
                return channelHub.AssociatedDataStorageSet[currentStorageIndex].Read(realChunkIndex);
            }
            else
            {
                return Double.NaN;
            }
        }

        private void GcNotification_GcOccured(int generation)
        {
            _oneDasEngineLogger.LogInformation($"garbage collection ({ generation }. generation)");
            Trace.WriteLine($"Garbage collection ({ generation }. generation).");
        }

        private void OnOneDasStateChanged(OneDasState oldState, OneDasState newState)
        {
            // clear data that was collected during "ready" state
            if (oldState == OneDasState.Ready && newState == OneDasState.Run)
            {
                foreach (var sampleRateCategory in _sampleRateInputDictionary)
                {
                    foreach (DataStorageContext context in sampleRateCategory.Value)
                    {
                        context.ChannelHub.AssociatedDataStorageSet.ForEach(dataStorage => dataStorage.Clear());
                    }
                }
            }

            // notify all listeners of new state
            Task.Run(() => this.OneDasStateChanged?.Invoke(this, new OneDasStateChangedEventArgs(oldState, newState)));
        }

        private void DisposePlugins()
        {
            _dataGatewaySet?.ForEach(dataGateway => dataGateway.Dispose());
            _dataWriterSet?.ForEach(dataWriter => dataWriter.Dispose());
        }

        #endregion

        #region "Debug"

        private void UpdateDebugInformation()
        {
            _cpuTime = float.NaN;
            // _performanceCounter_Cpu.NextValue()

            if (this.IsDebugOutputEnabled && this.OneDasState == OneDasState.Run)
            {
                this.WriteDebugOutput();
            }
        }

        private void WriteDebugOutput()
        {
            string debugOutputFilePath = Path.Combine(_oneDasOptions.BaseDirectoryPath, "debug", "Debug.csv");

            if (!File.Exists(debugOutputFilePath))
            {
                File.WriteAllText(debugOutputFilePath, $"DateTime;Late by;Cycle time;Processor Time;Timer drift{Environment.NewLine}-;ms;ms;%;µs{Environment.NewLine}");
            }
            else
            {
                File.AppendAllText(debugOutputFilePath, $"{DateTime.UtcNow};{_lateBy,4:0.0};{_cycleTime,4:0.0};{_cpuTime,3:0};{Convert.ToInt64(_timerDrift / 1000),7}{Environment.NewLine}");
            }
        }

        #endregion

        #region "Callbacks"

        private void OnTimerUpdateDebugInformationElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.UpdateDebugInformation();
        }

        #endregion

        #region "Exception handling"

        public void HandleException(Exception exception)
        {
            exception = ExceptionHelper.UnwrapException(exception);

            this.LastError = exception.Message;
            this.OneDasState = OneDasState.Error;

            _systemLogger.LogError(exception, exception.Message);
        }

        #endregion

        #region "Configuration"

        public void ActivateProject(Project project, int retryCount)
        {
            Contract.Requires(project != null);

            _project = project;

            this.OneDasState = OneDasState.ApplyConfiguration;

            for (int i = 0; i <= retryCount - 1; i++)
            {
                if (i == 0)
                {
                    _systemLogger.LogInformation($"starting engine (attempt {0 + 1})");
                }
                else
                {
                    _systemLogger.LogWarning($"starting engine (attempt {i + 1})");
                }

                try
                {
                    this.InternalActivateProject();
                    break;
                }
                catch (Exception)
                {
                    if (i >= retryCount - 1)
                    {
                        this.OneDasState = OneDasState.Unconfigured;
                        throw;
                    }
                }
            }

            _oneDasEngineLogger.LogInformation("project activated");
            this.OneDasState = OneDasState.Ready;
        }

        private void InternalActivateProject()
        {
            try
            {
                this.Step_0_Reset();
                this.Step_1_PrepareDataGateway();
                this.Step_2_PrepareBuffers();
                this.Step_3_PrepareDataWriter();
                this.Step_4_PrepareIoTimer();

                // diagnostics
                _lastActivationDateTime = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                this.DisposePlugins();

                throw ExceptionHelper.UnwrapException(ex);
            }
        }

        private void Step_0_Reset()
        {
            this.DisposePlugins();

            _timerDrift = 0;

            _chunkIndex = 0;
            _currentStorageIndex = 0;
            _cachedDataStorageIndex = 0;
            _timerLateCounter = 0;
            _cycleTimeTooLongCounter = 0;

            _cpuTime = 0;

            _cycleTime = 0;
            _maxCycleTime = 0;
            _lateBy = 0;
            _maxLateBy = 0;

            _chunkDateTime = DateTime.MinValue;
            _cachedChunkDateTime = DateTime.MinValue;
            _lastChunkDateTime = DateTime.MinValue;
            _lastUtcDateTime = DateTime.MinValue;
            _utcDateTime = DateTime.MinValue;
            _utcDateTime_Rounded = DateTime.MinValue;
            _lastActivationDateTime = DateTime.MinValue;

            _sampleRateInputDictionary = new Dictionary<SampleRate, IEnumerable<DataStorageContext>>();
            _sampleRateToDatStorageContextMap = new Dictionary<SampleRate, IEnumerable<DataStorageContext>>();
            _hasValidDataSet = new Dictionary<DataGatewayPluginLogicBase, bool>();
        }

        private void Step_1_PrepareDataGateway()
        {
            _dataGatewaySet = _project.DataGatewaySettingsSet.Select(settings => _pluginManager.BuildSettingsContainer<DataGatewayPluginLogicBase>(settings)).ToList();
            _referenceClock = _dataGatewaySet.FirstOrDefault(x => x is IReferenceClock) as IReferenceClock;

            if (_referenceClock == null)
            {
                _oneDasEngineLogger.LogInformation("no reference clock found (fallback to default clock)");
            }
            else
            {
                _oneDasEngineLogger.LogInformation($"reference clock is { ((PluginLogicBase)_referenceClock).Settings.Description.Id } ({ ((PluginLogicBase)_referenceClock).Settings.Description.InstanceId })");
            }

            _dataGatewaySet.AsParallel().ForAll(dataGateway => dataGateway.Configure());
        }

        private void Step_2_PrepareBuffers()
        {
            _sampleRateInputDictionary.Clear();
            _sampleRateToDatStorageContextMap.Clear();

            foreach (SampleRate sampleRate in Enum.GetValues(typeof(SampleRate)).Cast<SampleRate>().ToList())
            {
                List<ChannelHub> channelHubSet;
                List<DataStorageContext> dataStorageContextInputSet;
                List<DataStorageContext> dataStorageContextOutputSet;

                dataStorageContextInputSet = new List<DataStorageContext>();
                dataStorageContextOutputSet = new List<DataStorageContext>();

                channelHubSet = _project.ChannelHubSet.Where(channelHub => channelHub.SampleRate == sampleRate && (channelHub.AssociatedDataInput != null || channelHub.AssociatedDataOutputSet.Any())).ToList();

                channelHubSet.ForEach(channelHub =>
                {
                    int length;
                    Type type;
                    DataGatewayPluginLogicBase dataGateway;

                    length = Convert.ToInt32(_oneDasOptions.NativeSampleRate / Convert.ToInt32(sampleRate) * _oneDasOptions.ChunkPeriod);
                    type = typeof(ExtendedDataStorage<>).MakeGenericType(new Type[] { InfrastructureHelper.GetTypeFromOneDasDataType(channelHub.OneDasDataType) });
                    channelHub.AssociatedDataStorageSet = Enumerable.Range(0, STORAGE_COUNT).Select(x => (ExtendedDataStorageBase)Activator.CreateInstance(type, length)).ToList();

                    // input
                    if (channelHub.AssociatedDataInput != null)
                    {
                        dataGateway = _dataGatewaySet.First(y => y.Settings == channelHub.AssociatedDataInput.AssociatedDataGateway);
                        dataStorageContextInputSet.Add(new DataStorageContext(channelHub, dataGateway, channelHub.AssociatedDataInput));
                    }

                    // output
                    dataStorageContextOutputSet.AddRange(channelHub.AssociatedDataOutputSet.Select(dataOutput =>
                    {
                        dataGateway = _dataGatewaySet.First(y => y.Settings == dataOutput.AssociatedDataGateway);

                        return new DataStorageContext(channelHub, dataGateway, dataOutput);
                    }));
                });

                _sampleRateInputDictionary.Add(sampleRate, dataStorageContextInputSet);
                _sampleRateToDatStorageContextMap.Add(sampleRate, dataStorageContextOutputSet);
            }
        }

        private void Step_3_PrepareDataWriter()
        {
            DateTime currentDateTime;
            IList<CustomMetadataEntry> customMetadataEntrySet;

            _dataWriterSet = _project.DataWriterSettingsSet.Select(settings => _pluginManager.BuildSettingsContainer<DataWriterPluginLogicBase>(settings)).ToList();

            customMetadataEntrySet = new List<CustomMetadataEntry>();
            //customMetadataEntrySet.Add(new CustomMetadataEntry("system_name", "OneDAS", CustomMetadataEntryLevel.File));

            currentDateTime = DateTime.UtcNow;

            _dataWriterSet.ToList().ForEach(dataWriter =>
            {
                string baseDirectoryPath;

                // Improve - make general plugin validation
                if (new Regex($"[{Regex.Escape(new string(Path.GetInvalidPathChars()))}]").IsMatch(dataWriter.Settings.Description.Id))
                {
                    throw new Exception(ErrorMessage.OneDasEngine_DirectoryNameInvalid);
                }

                baseDirectoryPath = Path.Combine(_oneDasOptions.BaseDirectoryPath, "data", $"{ this.Project.Description.CampaignPrimaryGroup }_{ this.Project.Description.CampaignSecondaryGroup }_{ this.Project.Description.CampaignName }_V{ this.Project.Description.CampaignVersion }_{ this.Project.Description.Guid.ToString().Substring(0, 8) }", $"{ dataWriter.Settings.Description.Id }_DW{ dataWriter.Settings.Description.InstanceId }");

                Directory.CreateDirectory(baseDirectoryPath);

                dataWriter.Initialize(
                    currentDateTime,
                    new DataWriterContext("OneDAS", baseDirectoryPath, _project.Description, customMetadataEntrySet),
                    this.Project.ActiveChannelHubSet.Select(channelHub => new VariableDescription(channelHub)).ToList()
                );
            });

            _dataStorageSet = new List<List<DataStorageBase>>();
            _dataStorageSet.Add(this.Project.ActiveChannelHubSet.Select(channelHub => (DataStorageBase)(channelHub.AssociatedDataStorageSet[0])).ToList());
            _dataStorageSet.Add(this.Project.ActiveChannelHubSet.Select(channelHub => (DataStorageBase)(channelHub.AssociatedDataStorageSet[1])).ToList());
        }

        private void Step_4_PrepareIoTimer()
        {
            TimeSpan interval;
            TimeSpan timeShift;

            _timer_UpdateIo.Stop();

            interval = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(1.0 / _oneDasOptions.NativeSampleRate * 1000.0));
            timeShift = new TimeSpan(0, 0, 0, 0, TIMER_SHIFT);

            _timer_UpdateIo.Start(interval, timeShift, this.UpdateIo, UnmanagedThreadPriority.THREAD_PRIORITY_TIME_CRITICAL);
        }

        #endregion

        #region "Data aquisition"

        //                       _______Block________
        // StartDateTime = t_0   |                  | BlockIndex_0                        'TickOffset = TickCount_n - Δt
        //                       |                  |
        //                       |                  |
        //                       |                  |
        // CurrentDateTime = t_n |  Δt = t_n - t_1  | BlockIndex_n
        //                       |                  |     
        //                       |                  | 
        //                       |                  | 
        //                       |                  |
        //                       |                  |
        //                       |__________________| 
        private DateTime UpdateIo()
        {
            DateTime currentWindowsDateTime;

            currentWindowsDateTime = DateTime.UtcNow;

            if (_exception != null)
            {
                this.DisposePlugins();
                this.HandleException(_exception);
                _exception = null;
            }

            lock (_syncLock)
            {
                if (this.OneDasState >= OneDasState.Ready)
                {
                    try
                    {
                        // statistics
                        if (_chunkIndex % _oneDasOptions.NativeSampleRate == 0)
                        {
                            Trace.WriteLine($"max cycle time: {_maxCycleTime:0.00} ms / max late by: {_maxLateBy:0.00} ms");

                            if (_timerLateCounter == _oneDasOptions.NativeSampleRate)
                            {
                                _oneDasEngineLogger.LogInformation($"timer late by > { _ratedCycleTime_Ms } ms");
                                _timerLateCounter = 0;
                            }

                            if (_cycleTimeTooLongCounter == _oneDasOptions.NativeSampleRate)
                            {
                                _oneDasEngineLogger.LogInformation($"cycle time > { _ratedCycleTime_Ms } ms");
                                _cycleTimeTooLongCounter = 0;
                            }

                            _timerLateCounter = 0;
                            _cycleTimeTooLongCounter = 0;

                            _cycleTime = _maxCycleTime;
                            _lateBy = _maxLateBy;

                            _maxCycleTime = 0;
                            _maxLateBy = 0;
                        }

                        // late by
                        if (_timer_UpdateIo.LateBy > _ratedCycleTime_Ms)
                        {
                            _timerLateCounter += 1;
                        }

                        if (_timer_UpdateIo.LateBy > _maxLateBy)
                        {
                            _maxLateBy = _timer_UpdateIo.LateBy;
                        }

                        // cycle time
                        if (_timer_UpdateIo.LastActionTime > _ratedCycleTime_Ms)
                        {
                            _cycleTimeTooLongCounter += 1;
                        }

                        if (_timer_UpdateIo.LastActionTime > _maxCycleTime)
                        {
                            _maxCycleTime = _timer_UpdateIo.LastActionTime;
                        }

                        // get reference clock time
                        _utcDateTime = DateTime.MinValue;

                        if (_referenceClock != null) // reference clock is available
                        {
                            ((DataGatewayPluginLogicBase)_referenceClock).UpdateIo(currentWindowsDateTime);

                            _utcDateTime = _referenceClock.GetUtcDateTime();
                            _timerDrift = _referenceClock.GetTimerDrift();

                            if (_utcDateTime == DateTime.MinValue) // updating IO of reference clock data gateway was not successful
                            {
                                return DateTime.MinValue;
                            }
                        }
                        else // reference clock is not available
                        {
                            _utcDateTime = currentWindowsDateTime;
                        }

                        if (_utcDateTime < _lastUtcDateTime)
                        {
                            throw new Exception(ErrorMessage.OneDasEngine_ReferenceClockNotMonotonouslyRising);
                        }

                        _dataGatewaySet.ForEach(dataGateway => // update IO of all remaining data-gateways
                        {
                            if (dataGateway != _referenceClock)
                            {
                                dataGateway.UpdateIo(_utcDateTime); // use utcDateTime as reference
                            }
                        });

                        // The timer tries to fire at discrete times. It is allowed to be early or delayed by < (CycleTime - Offset) and the resulting DC time will be floored to nearest 10 ms. 
                        _utcDateTime_Rounded = _utcDateTime.RoundDown(new TimeSpan(0, 0, 0, 0, 10));
                        _chunkDateTime = _utcDateTime_Rounded.RoundDown(_chunkPeriod);
                        _chunkIndex = (int)Convert.ToInt64((_utcDateTime_Rounded.Ticks % _chunkPeriod.Ticks) / _baseFrequency_To_DateTime);

                        // write collected data to file and select new storage
                        if (_chunkDateTime != _lastChunkDateTime && _lastChunkDateTime != DateTime.MinValue)
                        {
                            // ensure that the storage thread gets unmodified copies
                            _cachedDataStorageIndex = _currentStorageIndex;
                            _cachedChunkDateTime = _lastChunkDateTime;

                            if (this.OneDasState == OneDasState.Run)
                            {
                                _storageAutoResetEvent.Set();
                            }

                            _currentStorageIndex = (_currentStorageIndex + 1) % STORAGE_COUNT;
                        }

                        // read: data port (input) --> channel hub storage
                        foreach (var sampleRateCategory in _sampleRateInputDictionary)
                        {
                            int realChunkIndex;

                            if (_chunkIndex % (int)sampleRateCategory.Key != 0 || sampleRateCategory.Value.Count() == 0)
                            {
                                continue;
                            }

                            realChunkIndex = _chunkIndex / (int)sampleRateCategory.Key;

                            foreach (DataGatewayPluginLogicBase dataGateway in _dataGatewaySet)
                            {
                                int referencePeriod;

                                // MaximumDatasetAge is ether determined by the corresponding plugin setting or by the sample period of the associated ChannelHub
                                //
                                // MaximumDatasetAge = 0                    -> normal behavior (no oversampling)
                                // MaximumDatasetAge > plugin IO cycle time -> compensate unstable cycle periods (packet drop tolerance)
                                // MaximumDatasetAge >= sample period       -> allow oversampling (repeat values)
                                referencePeriod = Math.Max(dataGateway.Settings.MaximumDatasetAge, (int)sampleRateCategory.Key * 10);

                                _hasValidDataSet[dataGateway] = dataGateway.LastSuccessfulUpdate?.Elapsed.TotalMilliseconds <= referencePeriod;
                            }

                            foreach (DataStorageContext context in sampleRateCategory.Value)
                            {
                                if (_hasValidDataSet[context.DataGateway])
                                {
                                    this.WriteToDataStorage(context.ChannelHub.AssociatedDataStorageSet[_currentStorageIndex], realChunkIndex, context.DataPort, 1);
                                }
                                // Improve: implement more than just 0 / 1 as data quality indicator
                                //else
                                //{
                                //    this.WriteToDataStorage(context.ChannelHub.AssociatedDataStorageSet[_currentStorageIndex], realChunkIndex, context.DataPort, 0);
                                //}
                            }
                        }

                        // write: channel hub storage --> data port (output)                   // IMPROVE: data will be send in next cycle. maybe LRW is not the best option
                        foreach (var sampleRateCategory in _sampleRateToDatStorageContextMap)
                        {
                            int realChunkIndex;

                            if (_chunkIndex % (int)sampleRateCategory.Key != 0 || sampleRateCategory.Value.Count() == 0)
                            {
                                continue;
                            }

                            realChunkIndex = _chunkIndex / (int)sampleRateCategory.Key;

                            foreach (DataStorageContext context in sampleRateCategory.Value)
                            {
                                this.WriteToDataPort(context.DataPort, context.ChannelHub.AssociatedDataStorageSet[_currentStorageIndex], realChunkIndex);
                            }
                        }

                        _lastChunkDateTime = _chunkDateTime;

                        return _utcDateTime;
                    }
                    catch (Exception ex)
                    {
                        this.DisposePlugins();
                        this.HandleException(ex);
                    }
                }
            }

            return DateTime.MinValue;
        }

        public unsafe void WriteToDataStorage(ExtendedDataStorageBase dataStorage, int index, DataPort dataPort, byte status)
        {
            byte* sourcePtr;
            byte* targetPtr;

            sourcePtr = (byte*)dataPort.DataPtr.ToPointer();
            targetPtr = (byte*)dataStorage.GetDataPointer(index).ToPointer();

            dataStorage.WriteStatus(index, status);

            if (dataPort.OneDasDataType == OneDasDataType.BOOLEAN && dataPort.BitOffset > -1) // special handling for boolean
            {
                bool value;

                value = (*sourcePtr & (1 << dataPort.BitOffset)) > 0;
                targetPtr[0] = *(byte*)&value;
            }
            else
            {
                switch (dataPort.Endianness)
                {
                    case Endianness.LittleEndian:

                        for (int i = 0; i < dataStorage.ElementSize; i++)
                        {
                            targetPtr[i] = sourcePtr[i];
                        }

                        break;

                    case Endianness.BigEndian:

                        for (int i = 0; i < dataStorage.ElementSize; i++)
                        {
                            targetPtr[i] = sourcePtr[dataStorage.ElementSize - i - 1];
                        }

                        break;

                    default:

                        throw new ArgumentException();
                }
            }
        }

        public unsafe void WriteToDataPort(DataPort dataPort, DataStorageBase dataStorage, int index)
        {
            byte* sourcePtr;
            byte* targetPtr;

            sourcePtr = (byte*)dataStorage.GetDataPointer(index).ToPointer();
            targetPtr = (byte*)dataPort.DataPtr.ToPointer();

            if (dataPort.OneDasDataType == OneDasDataType.BOOLEAN && dataPort.BitOffset > -1) // special handling for boolean
            {
                if (*(bool*)sourcePtr)
                {
                    *targetPtr = (byte)(*targetPtr | (1 << dataPort.BitOffset));
                }
                else
                {
                    *targetPtr = (byte)(*targetPtr & ~(1 << dataPort.BitOffset));
                }
            }
            else
            {
                switch (dataPort.Endianness)
                {
                    case Endianness.LittleEndian:

                        for (int i = 0; i < dataStorage.ElementSize; i++)
                        {
                            targetPtr[i] = sourcePtr[i];
                        }

                        break;

                    case Endianness.BigEndian:

                        for (int i = 0; i < dataStorage.ElementSize; i++)
                        {
                            targetPtr[i] = sourcePtr[dataStorage.ElementSize - i - 1];
                        }

                        break;

                    default:

                        throw new ArgumentException();
                }
            }
        }

        public void ClearDataStorage(int index)
        {
            foreach (var sampleRateCategory in _sampleRateInputDictionary)
            {
                foreach (DataStorageContext context in sampleRateCategory.Value)
                {
                    context.ChannelHub.AssociatedDataStorageSet[index].Clear();
                }
            }
        }

        #endregion

        #region "Data storage"

        private void StoreData()
        {
            while (true)
            {
                try
                {
                    _storageAutoResetEvent.WaitOne();

                    if (_storageCancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }

                    _dataWriterSet.ToList().AsParallel().ForAll(dataWriter =>
                    {
                        dataWriter.Write(_cachedChunkDateTime, TimeSpan.FromMinutes(1), _dataStorageSet[_cachedDataStorageIndex]);
                    });

                    this.ClearDataStorage(_cachedDataStorageIndex);
                }
                catch (Exception ex)
                {
                    _exception = ex;
                }
            }
        }

        #endregion

        #region "IDisposable Support"

        private bool isDisposed;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    // IO
                    _timer_UpdateIo?.Stop();

                    // general
                    SafeNativeMethods.SetThreadExecutionState(_executionState);

                    _storageCancellationTokenSource.Cancel();
                    _storageAutoResetEvent.Set();
                    _storageThread?.Join();

                    this.DisposePlugins();
                }
            }

            isDisposed = true;
        }

        #endregion
    }
}