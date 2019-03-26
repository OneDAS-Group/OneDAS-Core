using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneDas.Core.ProjectManagement;
using OneDas.DataStorage;
using OneDas.Extensibility;
using OneDas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.Core.Engine
{
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

        private TimeSpan _chunkPeriod;

        private IServiceProvider _serviceProvider;
        private ILogger _systemLogger;
        private ILogger _oneDasEngineLogger;

        private OneDasOptions _options;
        private DriveInfo _driveInfo;

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

        private List<(DataPort Source, DataPort Target)> _linkedDataPortSet;
        private Dictionary<SampleRate, List<DataStorageContext>> _sampleRateToDataStorageContextMap;
        private Dictionary<DataWriterExtensionLogicBase, List<VariableDescription>> _dataWriterToVariableDescriptionMap;
        private Dictionary<DataWriterExtensionLogicBase, List<List<IDataStorage>>> _dataWriterToStorageSetMap;
        private Dictionary<DataGatewayExtensionLogicBase, bool> _hasValidDataSet;

        // overwritten
        private Exception _exception;
        private IReferenceClock _referenceClock;

        #endregion

        #region "Constructors"

        public OneDasEngine(IServiceProvider serviceProvider, IOptions<OneDasOptions> options, ILoggerFactory loggerFactory)
        {
            _serviceProvider = serviceProvider;
            _options = options.Value;
            _driveInfo = new DriveInfo(_options.DataDirectoryPath);

            // state
            _oneDasState = OneDasState.Initialization;

            // logging
            _systemLogger = loggerFactory.CreateLogger("System");
            _oneDasEngineLogger = loggerFactory.CreateLogger("Engine");

            // 
            _baseFrequency_To_DateTime = Convert.ToInt64(10000000L / OneDasConstants.NativeSampleRate);
            _timer_UpdateIo = new RtTimer();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _executionState = SafeNativeMethods.SetThreadExecutionState(ExecutionState.CONTINUOUS | ExecutionState.SYSTEM_REQUIRED);
            }
            
            _ratedCycleTime_Ms = 1.0 / OneDasConstants.NativeSampleRate * 1000.0;

            _storageThread = new Thread(() => this.StoreData())
            {
                Priority = ThreadPriority.Lowest,
                IsBackground = false,
                Name = "Storage"
            };

            _storageAutoResetEvent = new AutoResetEvent(false);
            _storageCancellationTokenSource = new CancellationTokenSource();

            _chunkPeriod = new TimeSpan(0, 0, (int)OneDasConstants.ChunkPeriod);
            _syncLock = new object();

            // process priority

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

                try
                {
                    Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime; // try to get even higher
                }
                catch
                {
                    //
                }
            }
            else
            {
                try
                {
                    Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
                }
                catch
                {
                    //
                }
            }

            GcNotification.GcDone += GcNotification_GcOccured;

            _storageThread.Start();

            _oneDasState = OneDasState.Idle;
        }

        #endregion

        #region "Properties"

        public string LastError { get; private set; }

        public OneDasState OneDasState
        {
            get
            {
                return _oneDasState;
            }
            private set
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

                    case OneDasState.Idle:

                        if (this.OneDasState == OneDasState.Initialization || this.OneDasState == OneDasState.ApplyConfiguration || this.OneDasState == OneDasState.Ready)
                        {
                            isTransitionValid = true;
                        }

                        break;

                    case OneDasState.ApplyConfiguration:

                        if (this.OneDasState == OneDasState.Idle || this.OneDasState == OneDasState.Ready)
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
                    throw new Exception($"invalid transition from state { this.OneDasState } to state { value }");
                }

                if (value != OneDasState.Error)
                {
                    this.LastError = string.Empty;
                }

                _systemLogger.LogInformation($"transition from state { _oneDasState.ToString() } to state { value.ToString() }");

                this.OnOneDasStateChanged(_oneDasState, value);
                _oneDasState = value;
            }
        }

        public OneDasProject Project { get; private set; }

        #endregion

        #region "Methods"

        public void Start()
        {
            for (int i = 0; i < STORAGE_COUNT; i++)
            {
                this.ClearDataStorages(i);
            }

            this.OneDasState = OneDasState.Run;

            _oneDasEngineLogger.LogInformation("data recording enabled");
        }

        public void Pause()
        {
            this.OneDasState = OneDasState.Ready;

            _oneDasEngineLogger.LogInformation("data recording paused");
        }

        public void Stop()
        {
            if (this.OneDasState == OneDasState.Run)
            {
                this.Pause();
            }

            this.OneDasState = OneDasState.Idle;

            this.Project?.Dispose();
            this.Project = null;

            _oneDasEngineLogger.LogInformation("cyclic I/O update stopped");
        }

        public void AcknowledgeError()
        {
            this.OneDasState = OneDasState.Initialization;
            this.OneDasState = OneDasState.Idle;
        }

        public OneDasPerformanceInformation CreatePerformanceInformation()
        {
            return new OneDasPerformanceInformation(
                DateTime.UtcNow,
                this.OneDasState,
                Process.GetCurrentProcess().PriorityClass,
                _lateBy,
                _cycleTime,
                _timerDrift,
                _cpuTime,
                (int)((DateTime.UtcNow - _lastActivationDateTime).TotalSeconds),
                _driveInfo.AvailableFreeSpace,
                _driveInfo.TotalSize);
        }

        public IEnumerable<object> CreateDataSnapshot(IList<ChannelHubBase> channelHubSet = null)
        {
            if (this.OneDasState >= OneDasState.Ready)
            {
                lock (_syncLock)
                {
                    if (channelHubSet != null)
                    {
                        return channelHubSet.Select(channelHub => channelHub.GetValue() ?? Double.NaN).ToList();
                    }
                    else
                    {
                        return this.Project.ActiveChannelHubSet.Select(channelHub => channelHub.GetValue() ?? Double.NaN).ToList();
                    }
                }
            }
            else
            {
                throw new Exception(ErrorMessage.OneDasEngine_OneDasStateNotGreaterEqualReady);
            }
        }

        private void GcNotification_GcOccured(int generation)
        {
            _oneDasEngineLogger.LogDebug($"garbage collection ({ generation }. generation)");
        }

        private void OnOneDasStateChanged(OneDasState oldState, OneDasState newState)
        {
            // notify all listeners of new state
            Task.Run(() => this.OneDasStateChanged?.Invoke(this, new OneDasStateChangedEventArgs(oldState, newState)));
        }

        #endregion

        #region "Exception handling"

        public void HandleException(Exception exception)
        {
            this.Project?.Dispose();

            exception = EngineUtilities.UnwrapException(exception);

            this.LastError = exception.Message;
            this.OneDasState = OneDasState.Error;

            _systemLogger.LogError(exception, exception.Message);
        }

        #endregion

        #region "Configuration"

        public void ActivateProject(OneDasProjectSettings projectSettings)
        {
            this.ActivateProject(projectSettings, 0);
        }

        public void ActivateProject(OneDasProjectSettings projectSettings, int retryCount)
        {
            Contract.Requires(projectSettings != null);

            this.OneDasState = OneDasState.ApplyConfiguration;

            for (int i = 0; i <= retryCount; i++)
            {
                if (i == 0)
                {
                    _systemLogger.LogInformation($"starting engine");
                }
                else
                {
                    _systemLogger.LogWarning($"starting engine (attempt {i + 1})");
                }

                try
                {
                    this.Project?.Dispose();
                    this.Project = ActivatorUtilities.CreateInstance<OneDasProject>(_serviceProvider, projectSettings);

                    this.InternalActivateProject();

                    break;
                }
                catch (Exception)
                {
                    if (i >= retryCount - 1)
                    {
                        this.OneDasState = OneDasState.Idle;
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
                this.Step_1_PrepareLinkedDataPortSet();
                this.Step_2_PrepareBuffers();
                this.Step_3_PrepareDataGateway();
                this.Step_4_PrepareDataWriter();
                this.Step_5_PrepareIoTimer();

                // diagnostics
                _lastActivationDateTime = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                this.Project?.Dispose();

                throw EngineUtilities.UnwrapException(ex);
            }
        }

        private void Step_0_Reset()
        {
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

            _hasValidDataSet = new Dictionary<DataGatewayExtensionLogicBase, bool>();
        }

        private void Step_1_PrepareLinkedDataPortSet()
        {
            _linkedDataPortSet = this.Project.ActiveChannelHubSet.SelectMany(channelHub =>
            {
                return channelHub.AssociatedDataOutputSet.Select(dataPort => (channelHub.AssociatedDataInput, dataPort));
            }).ToList();
        }

        private void Step_2_PrepareBuffers()
        {
            Dictionary<SampleRate, Dictionary<ChannelHubBase, HashSet<DataWriterExtensionLogicBase>>> sr_to_ch_map;

            sr_to_ch_map = new Dictionary<SampleRate, Dictionary<ChannelHubBase, HashSet<DataWriterExtensionLogicBase>>>();

            /* ---------------------------------------------- */
            //                  sr_to_ch_map
            //           SR1        SR2             SR3
            //           /           |               \
            //        ch1->DW1,     ch1->DW1         ch1->DW2
            //        ch2->DW2,     ch4->DW1, DW5    ch2->DW3
            //        ch3->DW3,     
            /* ---------------------------------------------- */

            // for each data writer
            this.Project.GetEnabledDataWriters().ForEach(dataWriter =>
            {
                if (!dataWriter.Settings.BufferRequestSet.Any())
                {
                    _oneDasEngineLogger.LogWarning(ErrorMessage.OneDasEngine_DataWriterHasNoBufferRequests, dataWriter.DisplayName);
                }

                // for each buffer request
                dataWriter.Settings.BufferRequestSet.ForEach(bufferRequest =>
                {
                    Dictionary<ChannelHubBase, HashSet<DataWriterExtensionLogicBase>> ch_to_dw_map;

                    // ensure that there is a sample rate entry
                    if (sr_to_ch_map.ContainsKey(bufferRequest.SampleRate))
                    {
                        ch_to_dw_map = sr_to_ch_map[bufferRequest.SampleRate];
                    }
                    else
                    {
                        ch_to_dw_map = new Dictionary<ChannelHubBase, HashSet<DataWriterExtensionLogicBase>>();
                        sr_to_ch_map[bufferRequest.SampleRate] = ch_to_dw_map;
                    }

                    // add channel hubs
                    this.FilterChannelHubs(this.Project.ActiveChannelHubSet, bufferRequest.GroupFilter).ForEach(channelHub =>
                    {
                        // ensure that there is a channel hub entry
                        if (!ch_to_dw_map.ContainsKey(channelHub))
                        {
                            ch_to_dw_map[channelHub] = new HashSet<DataWriterExtensionLogicBase>();
                        }

                        ch_to_dw_map[channelHub].Add(dataWriter);
                    });
                });
            });

            // prepare _dataWriterToVariableDescriptionMap
            _dataWriterToVariableDescriptionMap = new Dictionary<DataWriterExtensionLogicBase, List<VariableDescription>>();
            _dataWriterToStorageSetMap = new Dictionary<DataWriterExtensionLogicBase, List<List<IDataStorage>>>();

            this.Project.DataWriterSet.ForEach(dataWriter =>
            {
                _dataWriterToVariableDescriptionMap[dataWriter] = new List<VariableDescription>();
                _dataWriterToStorageSetMap[dataWriter] = new List<List<IDataStorage>>();

                Enumerable.Range(0, STORAGE_COUNT).ToList().ForEach(index => _dataWriterToStorageSetMap[dataWriter].Add(new List<IDataStorage>()));
            });

            /* ----------------------------------------- */
            //     _sampleRateToDataStorageContextMap
            //           SR1     SR2      SR3
            //           /        |        \
            //        ctx1      ctx4       ctx7
            //        ctx2      ctx5       ctx8
            //        ctx3      ctx6       ctx9
            /* ----------------------------------------- */

            _sampleRateToDataStorageContextMap = sr_to_ch_map.ToDictionary(entry => entry.Key, entry => entry.Value.Select(subEntry =>
            {
                SampleRate sampleRate;
                ChannelHubBase channelHub;
                ChannelHubSettings channelHubSettings;

                List<IExtendedDataStorage> dataStorageSet;
                List<DataWriterExtensionLogicBase> dataWriterSet;

                sampleRate = entry.Key;
                channelHub = subEntry.Key;
                channelHubSettings = subEntry.Key.Settings;
                dataWriterSet = subEntry.Value.ToList();

                // create data storages
                dataStorageSet = this.CreateDataStorages(sampleRate, channelHub.Settings.DataType, STORAGE_COUNT);

                // helper dictionaries
                dataWriterSet.ForEach(dataWriter =>
                {
                    // _dataWriterToStorageSetMap
                    for (int i = 0; i < STORAGE_COUNT; i++)
                    {
                        _dataWriterToStorageSetMap[dataWriter][i].Add(dataStorageSet[i]);
                    }

                    // _dataWriterToVariableDescriptionMap
                    _dataWriterToVariableDescriptionMap[dataWriter].Add(new VariableDescription(
                        channelHubSettings.Guid,
                        channelHubSettings.Name,
                        $"{ 100 / (int)sampleRate } Hz",
                        channelHubSettings.Group,
                        channelHubSettings.DataType,
                        OneDasUtilities.GetSamplesPerDayFromSampleRate(sampleRate),
                        channelHubSettings.Unit,
                        channelHubSettings.TransferFunctionSet,
                        typeof(IExtendedDataStorage)
                    ));
                });

                return new DataStorageContext(dataStorageSet, channelHub.AssociatedDataInput.AssociatedDataGateway, channelHub.AssociatedDataInput);
            }).ToList());
        }

        private void Step_3_PrepareDataGateway()
        {
            _referenceClock = this.Project.DataGatewaySet.FirstOrDefault(x => x is IReferenceClock) as IReferenceClock;

            if (_referenceClock == null)
            {
                _oneDasEngineLogger.LogWarning("no reference clock found (fallback to default clock)");
            }
            else
            {
                _oneDasEngineLogger.LogInformation($"reference clock is { ((ExtensionLogicBase)_referenceClock).Settings.Description.Id } ({ ((ExtensionLogicBase)_referenceClock).Settings.Description.InstanceId })");
            }

            this.Project.DataGatewaySet.AsParallel().ForAll(dataGateway =>
            {
                DataGatewayContext dataGatewayContext;

                dataGatewayContext = new DataGatewayContext();
                dataGateway.Configure(dataGatewayContext);
            });
        }

        private void Step_4_PrepareDataWriter()
        {
            DateTime currentDateTime;
            IList<CustomMetadataEntry> customMetadataEntrySet;

            customMetadataEntrySet = new List<CustomMetadataEntry>();
            //customMetadataEntrySet.Add(new CustomMetadataEntry("system_name", "OneDAS", CustomMetadataEntryLevel.File));

            currentDateTime = DateTime.UtcNow;

            this.Project.DataWriterSet.ForEach(dataWriter =>
            {
                string baseDirectoryPath;
                DataWriterContext dataWriterContext;

                // Improve - make general extension validation
                if (new Regex($"[{Regex.Escape(new string(Path.GetInvalidPathChars()))}]").IsMatch(dataWriter.Settings.Description.Id))
                {
                    throw new Exception(ErrorMessage.OneDasEngine_DirectoryNameInvalid);
                }

                baseDirectoryPath = Path.Combine(_options.DataDirectoryPath, $"{ this.Project.Settings.Description.PrimaryGroupName }_{ this.Project.Settings.Description.SecondaryGroupName }_{ this.Project.Settings.Description.CampaignName }_V{ this.Project.Settings.Description.Version }_{ this.Project.Settings.Description.Guid.ToString().Substring(0, 8) }", $"{ dataWriter.Settings.Description.Id }_DW{ dataWriter.Settings.Description.InstanceId }");

                Directory.CreateDirectory(baseDirectoryPath);

                dataWriterContext = new DataWriterContext("OneDAS", baseDirectoryPath, this.Project.Settings.Description, customMetadataEntrySet);
                dataWriter.Configure(dataWriterContext, _dataWriterToVariableDescriptionMap[dataWriter]);
            });
        }

        private void Step_5_PrepareIoTimer()
        {
            TimeSpan interval;
            TimeSpan timeShift;

            _timer_UpdateIo.Stop();

            interval = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(1.0 / OneDasConstants.NativeSampleRate * 1000.0));
            timeShift = new TimeSpan(0, 0, 0, 0, TIMER_SHIFT);

            _timer_UpdateIo.Start(interval, timeShift, this.UpdateIo);

            if (Process.GetCurrentProcess().PriorityClass < ProcessPriorityClass.RealTime)
            {
                _oneDasEngineLogger.LogWarning($"process priority is lower than RealTime: { Process.GetCurrentProcess().PriorityClass }");
            }
            else
            {
                _oneDasEngineLogger.LogInformation($"process priority is: RealTime");
            }
        }

        private List<IExtendedDataStorage> CreateDataStorages(SampleRate sampleRate, OneDasDataType dataType, int count)
        {
            int length;
            Type type;

            length = Convert.ToInt32(OneDasConstants.NativeSampleRate / Convert.ToInt32(sampleRate) * OneDasConstants.ChunkPeriod);
            type = typeof(ExtendedDataStorage<>).MakeGenericType(new Type[] { OneDasUtilities.GetTypeFromOneDasDataType(dataType) });

            return Enumerable.Range(0, count).Select(x => (IExtendedDataStorage)Activator.CreateInstance(type, length)).ToList();
        }

        private List<ChannelHubBase> FilterChannelHubs(List<ChannelHubBase> channelHubSet, string groupFilter)
        {
            HashSet<ChannelHubBase> filteredChannelHubSet;

            filteredChannelHubSet = new HashSet<ChannelHubBase>();

            groupFilter.Split(';').ToList().ForEach(groupFilterPart =>
            {
                string filter;

                filter = groupFilterPart.Replace("*", ".*");

                if (groupFilterPart.StartsWith("!"))
                {
                    filter = filter.Replace("!", string.Empty);
                    channelHubSet.Where(channelHub => !Regex.IsMatch(channelHub.Settings.Group, filter, RegexOptions.IgnoreCase)).ToList().ForEach(channelHub => filteredChannelHubSet.Add(channelHub));
                }
                else
                {
                    channelHubSet.Where(channelHub => Regex.IsMatch(channelHub.Settings.Group, filter, RegexOptions.IgnoreCase)).ToList().ForEach(channelHub => filteredChannelHubSet.Add(channelHub));
                }
            });

            return filteredChannelHubSet.ToList();
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
                        if (_chunkIndex % OneDasConstants.NativeSampleRate == 0)
                        {
                            _systemLogger.LogDebug($"max cycle time: {_maxCycleTime:0.00} ms / max late by: {_maxLateBy:0.00} ms");

                            if (_timerLateCounter == OneDasConstants.NativeSampleRate)
                            {
                                _oneDasEngineLogger.LogWarning($"timer late by > { _ratedCycleTime_Ms } ms");
                                _timerLateCounter = 0;
                            }

                            if (_cycleTimeTooLongCounter == OneDasConstants.NativeSampleRate)
                            {
                                _oneDasEngineLogger.LogWarning($"cycle time > { _ratedCycleTime_Ms } ms");
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
                            ((DataGatewayExtensionLogicBase)_referenceClock).UpdateIo(currentWindowsDateTime);

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

                        this.Project.DataGatewaySet.ForEach(dataGateway => // update IO of all remaining data-gateways
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

                        // store data: data port (input) --> data storage
                        foreach (var entry in _sampleRateToDataStorageContextMap)
                        {
                            int realChunkIndex;

                            if (_chunkIndex % (int)entry.Key != 0 || entry.Value.Count() == 0)
                            {
                                continue;
                            }

                            realChunkIndex = _chunkIndex / (int)entry.Key;

                            foreach (DataGatewayExtensionLogicBase dataGateway in this.Project.DataGatewaySet)
                            {
                                int referencePeriod;

                                // MaximumDatasetAge is ether determined by the corresponding extension setting or by the sample period of the associated ChannelHub
                                //
                                // MaximumDatasetAge = 0                       -> normal behavior (no oversampling)
                                // MaximumDatasetAge > extension IO cycle time -> compensate unstable cycle periods (packet drop tolerance)
                                // MaximumDatasetAge >= sample period          -> allow oversampling (repeat values)
                                referencePeriod = Math.Max(dataGateway.Settings.MaximumDatasetAge, (int)entry.Key * 10);

                                _hasValidDataSet[dataGateway] = dataGateway.LastSuccessfulUpdate?.Elapsed.TotalMilliseconds <= referencePeriod;
                            }

                            foreach (DataStorageContext context in entry.Value)
                            {
                                if (_hasValidDataSet[context.DataGateway])
                                {
                                    this.CopyToDataStorage(context.DataStorageSet[_currentStorageIndex], realChunkIndex, context.DataPort, 1);
                                }
                                // Improve: implement more than just 0 / 1 as data quality indicator
                                //else
                                //{
                                //    this.WriteToDataStorage(context.ChannelHub.AssociatedDataStorageSet[_currentStorageIndex], realChunkIndex, context.DataPort, 0);
                                //}
                            }
                        }

                        // forward data: data port (input) --> data port (output)
                        foreach (var entry in _linkedDataPortSet)
                        {
                            this.CopyToDataPort(entry.Source, entry.Target);
                        }

                        _lastChunkDateTime = _chunkDateTime;

                        return _utcDateTime;
                    }
                    catch (Exception ex)
                    {
                        this.HandleException(ex);
                    }
                }
            }

            return DateTime.MinValue;
        }

        public unsafe void CopyToDataStorage(IExtendedDataStorage dataStorage, int index, DataPort dataPort, byte status)
        {
            int elementSize;
            byte* sourcePtr;
            byte* targetPtr;

            elementSize = dataStorage.ElementSize;
            sourcePtr = (byte*)dataPort.DataPtr.ToPointer();
            targetPtr = (byte*)dataStorage.DataBufferPtr.ToPointer() + index * elementSize;

            dataStorage.GetStatusBuffer()[index] = status;

            if (dataPort.DataType == OneDasDataType.BOOLEAN && dataPort.BitOffset > -1) // special handling for boolean
            {
                // from bit to byte
                bool value;

                value = (*sourcePtr & (1 << dataPort.BitOffset)) > 0;
                targetPtr[0] = *(byte*)&value;
            }
            else
            {
                switch (dataPort.Endianness)
                {
                    case Endianness.LittleEndian:

                        for (int i = 0; i < elementSize; i++)
                        {
                            targetPtr[i] = sourcePtr[i];
                        }

                        break;

                    case Endianness.BigEndian:

                        for (int i = 0; i < elementSize; i++)
                        {
                            targetPtr[i] = sourcePtr[elementSize - i - 1];
                        }

                        break;

                    default:

                        throw new ArgumentException();
                }
            }
        }

        public unsafe void CopyToDataPort(DataPort source, DataPort target)
        {
            int elementSize;
            byte* sourcePtr;
            byte* targetPtr;

            elementSize = OneDasUtilities.SizeOf(source.DataType);
            sourcePtr = (byte*)source.DataPtr.ToPointer();
            targetPtr = (byte*)target.DataPtr.ToPointer();

            if (source.DataType == OneDasDataType.BOOLEAN && (source.BitOffset > -1 || target.BitOffset > -1)) // special handling for boolean
            {
                // from bit to byte
                if (source.BitOffset > -1 && !(target.BitOffset > -1))
                {
                    bool value;

                    value = (*sourcePtr & (1 << source.BitOffset)) > 0;
                    targetPtr[0] = *(byte*)&value;
                }
                // from byte to bit
                else if (!(source.BitOffset > -1) && target.BitOffset > -1)
                {
                    bool value;

                    value = *(bool*)sourcePtr;

                    if (value)
                    {
                        *targetPtr |= (byte)(1 << target.BitOffset);
                    }
                    else
                    {
                        *targetPtr &= (byte)(~(1 << target.BitOffset));
                    }
                }
                // from bit to bit
                else if (source.BitOffset > -1 && target.BitOffset > -1)
                {
                    bool value;

                    value = (*sourcePtr & (1 << source.BitOffset)) > 0;

                    if (value)
                    {
                        *targetPtr |= (byte)(1 << target.BitOffset);
                    }
                    else
                    {
                        *targetPtr &= (byte)(~(1 << target.BitOffset));
                    }
                }
            }
            else
            {
                if (source.Endianness == target.Endianness)
                {
                    for (int i = 0; i < elementSize; i++)
                    {
                        targetPtr[i] = sourcePtr[i];
                    }
                }
                else
                {
                    for (int i = 0; i < elementSize; i++)
                    {
                        targetPtr[i] = sourcePtr[elementSize - i - 1];
                    }
                }
            }
        }

        #endregion

        #region "Data storage"

        public void ClearDataStorages(int index)
        {
            foreach (var entry in _sampleRateToDataStorageContextMap)
            {
                foreach (DataStorageContext context in entry.Value)
                {
                    context.DataStorageSet[index].Clear();
                }
            }
        }

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

                    this.Project.GetEnabledDataWriters().Where(dataWriter => dataWriter.Settings.Description.IsEnabled).ToList().ForEach(dataWriter =>
                    {
                        dataWriter.Write(_cachedChunkDateTime, TimeSpan.FromMinutes(1), _dataWriterToStorageSetMap[dataWriter][_cachedDataStorageIndex]);
                    });

                    this.ClearDataStorages(_cachedDataStorageIndex);
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
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        SafeNativeMethods.SetThreadExecutionState(_executionState);
                    }

                    _storageCancellationTokenSource.Cancel();
                    _storageAutoResetEvent.Set();
                    _storageThread?.Join();

                    this.Project?.Dispose();
                }
            }

            isDisposed = true;
        }

        #endregion
    }
}