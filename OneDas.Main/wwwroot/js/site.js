var DataDirectionEnum;
(function (DataDirectionEnum) {
    DataDirectionEnum[DataDirectionEnum["Input"] = 1] = "Input";
    DataDirectionEnum[DataDirectionEnum["Output"] = 2] = "Output";
})(DataDirectionEnum || (DataDirectionEnum = {}));
var FileGranularityEnum;
(function (FileGranularityEnum) {
    FileGranularityEnum[FileGranularityEnum["Minute_1"] = 60] = "Minute_1";
    FileGranularityEnum[FileGranularityEnum["Minute_10"] = 600] = "Minute_10";
    FileGranularityEnum[FileGranularityEnum["Hour"] = 3600] = "Hour";
    FileGranularityEnum[FileGranularityEnum["Day"] = 86400] = "Day";
})(FileGranularityEnum || (FileGranularityEnum = {}));
var OneDasDataTypeEnum;
(function (OneDasDataTypeEnum) {
    OneDasDataTypeEnum[OneDasDataTypeEnum["BOOLEAN"] = 8] = "BOOLEAN";
    OneDasDataTypeEnum[OneDasDataTypeEnum["UINT8"] = 264] = "UINT8";
    OneDasDataTypeEnum[OneDasDataTypeEnum["INT8"] = 520] = "INT8";
    OneDasDataTypeEnum[OneDasDataTypeEnum["UINT16"] = 272] = "UINT16";
    OneDasDataTypeEnum[OneDasDataTypeEnum["INT16"] = 528] = "INT16";
    OneDasDataTypeEnum[OneDasDataTypeEnum["UINT32"] = 288] = "UINT32";
    OneDasDataTypeEnum[OneDasDataTypeEnum["INT32"] = 544] = "INT32";
    OneDasDataTypeEnum[OneDasDataTypeEnum["FLOAT32"] = 800] = "FLOAT32";
    OneDasDataTypeEnum[OneDasDataTypeEnum["FLOAT64"] = 832] = "FLOAT64";
})(OneDasDataTypeEnum || (OneDasDataTypeEnum = {}));
var OneDasStateEnum;
(function (OneDasStateEnum) {
    OneDasStateEnum[OneDasStateEnum["Error"] = 1] = "Error";
    OneDasStateEnum[OneDasStateEnum["Initialization"] = 2] = "Initialization";
    OneDasStateEnum[OneDasStateEnum["Unconfigured"] = 3] = "Unconfigured";
    OneDasStateEnum[OneDasStateEnum["ApplyConfiguration"] = 5] = "ApplyConfiguration";
    OneDasStateEnum[OneDasStateEnum["Ready"] = 6] = "Ready";
    OneDasStateEnum[OneDasStateEnum["Run"] = 7] = "Run";
})(OneDasStateEnum || (OneDasStateEnum = {}));
var SampleRateEnum;
(function (SampleRateEnum) {
    SampleRateEnum[SampleRateEnum["SampleRate_100"] = 1] = "SampleRate_100";
    SampleRateEnum[SampleRateEnum["SampleRate_25"] = 4] = "SampleRate_25";
    SampleRateEnum[SampleRateEnum["SampleRate_5"] = 20] = "SampleRate_5";
    SampleRateEnum[SampleRateEnum["SampleRate_1"] = 100] = "SampleRate_1";
})(SampleRateEnum || (SampleRateEnum = {}));
class ActionRequest {
    constructor(pluginId, instanceId, methodName, data) {
        this.PluginId = pluginId;
        this.InstanceId = instanceId;
        this.MethodName = methodName;
        this.Data = data;
    }
}
class ActionResponse {
    constructor(data) {
        this.Data = data;
    }
}
class ChartContext {
    constructor(channelHub, chart, valueSet) {
        this.ChannelHub = channelHub;
        this.Chart = chart;
        this.ValueSet = valueSet;
    }
}
class EventDispatcher {
    constructor() {
        this._subscriptions = new Array();
    }
    subscribe(fn) {
        if (fn) {
            this._subscriptions.push(fn);
        }
    }
    unsubscribe(fn) {
        let i = this._subscriptions.indexOf(fn);
        if (i > -1) {
            this._subscriptions.splice(i, 1);
        }
    }
    dispatch(sender, args) {
        for (let handler of this._subscriptions) {
            handler(sender, args);
        }
    }
}
var LiveViewPeriodEnum;
(function (LiveViewPeriodEnum) {
    LiveViewPeriodEnum[LiveViewPeriodEnum["Period_60"] = 60] = "Period_60";
    LiveViewPeriodEnum[LiveViewPeriodEnum["Period_600"] = 600] = "Period_600";
    LiveViewPeriodEnum[LiveViewPeriodEnum["Period_3600"] = 3600] = "Period_3600";
})(LiveViewPeriodEnum || (LiveViewPeriodEnum = {}));
class WorkspaceBase {
    constructor(address, title, viewName, activeProject) {
        this.Address = address;
        this.Title = title;
        this.ViewName = viewName;
        this.ActiveProject = activeProject;
    }
}
class ChannelHubModel {
    constructor(name, group, oneDasDataType, sampleRate) {
        this.Name = name;
        this.Group = group;
        this.OneDasDataType = oneDasDataType;
        this.SampleRate = sampleRate;
        this.Guid = Guid.NewGuid();
        this.CreationDateTime = new Date().toISOString();
        this.Unit = "";
        this.TransferFunctionSet = [];
        this.SerializerDataInputId = "";
        this.SerializerDataOutputIdSet = [];
    }
}
class OneDasModuleModel {
    constructor(dataType, dataDirection, size) {
        this.DataType = dataType;
        this.DataDirection = dataDirection;
        this.Size = size;
    }
}
class TransferFunctionModel {
    constructor(dateTime, type, option, argument) {
        this.DateTime = dateTime;
        this.Type = type;
        this.Option = option;
        this.Argument = argument;
    }
}
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class ConnectionManager {
    static Initialize(enableLogging) {
        ConnectionManager.Broadcaster = new signalR.HubConnection('/broadcaster');
    }
}
ConnectionManager.InvokeBroadcaster = (methodName, ...args) => __awaiter(this, void 0, void 0, function* () {
    return yield Promise.resolve(ConnectionManager.Broadcaster.invoke(methodName, ...args));
});
class EnumerationHelper {
}
EnumerationHelper.Description = {};
EnumerationHelper.GetEnumLocalization = (typeName, value) => {
    var key = eval(typeName + "[" + value + "]");
    return eval("EnumerationHelper.Description['" + typeName + "_" + key + "']");
};
EnumerationHelper.GetEnumValues = (typeName) => {
    let values;
    values = eval("Object.keys(" + typeName + ").map(key => " + typeName + "[key])");
    return values.filter(value => typeof (value) === "number");
};
let ErrorMessage = {};
ErrorMessage["MultiMappingEditorViewModel_InvalidSettings"] = "One or more settings are invalid.";
ErrorMessage["MultiMappingEditorViewModel_WrongDataType"] = "One or more variable-channel data type combinations are invalid.";
ErrorMessage["Project_ChannelAlreadyExists"] = "A channel with that name already exists.";
ErrorMessage["Project_IsAlreadyInGroup"] = "The channel is already a member of this group.";
ErrorMessage["Project_InvalidCharacters"] = "Use A-Z, a-z, 0-9 or _.";
ErrorMessage["Project_InvalidLeadingCharacter"] = "Use A-Z or a-z as first character.";
ErrorMessage["Project_NameEmpty"] = "The name must not be empty.";
class ObservableGroup {
    constructor(key) {
        this.Key = key;
        this.Members = ko.observableArray([]);
    }
}
function ObservableGroupBy(list, nameGetter, groupNameGetter, filter) {
    let result;
    result = [];
    list.forEach(element => {
        if (nameGetter(element).indexOf(filter) > -1) {
            groupNameGetter(element).split("\n").forEach(groupName => {
                AddToGroupedArray(element, groupName, result);
            });
        }
    });
    return result;
}
function AddToGroupedArray(item, groupName, observableGroupSet) {
    let group;
    group = observableGroupSet.find(y => y.Key === groupName);
    if (!group) {
        group = new ObservableGroup(groupName);
        observableGroupSet.push(group);
    }
    group.Members.push(item);
}
//function AddToGroupedObservableArray<T>(item: T, groupName: string, observableGroupSet: KnockoutObservableArray<ObservableGroup<T>>)
//{
//    let group: ObservableGroup<T>
//    group = observableGroupSet().find(y => y.Key === groupName)
//    if (!group)
//    {
//        group = new ObservableGroup<T>(groupName)
//        observableGroupSet.push(group)
//    }
//    group.Members.push(item)
//}
//function RemoveFromGroupedObservableArray<T>(item: T, observableGroupSet: KnockoutObservableArray<ObservableGroup<T>>)
//{
//    var group: ObservableGroup<T>
//    observableGroupSet().some(x =>
//    {
//        if (x.Members().indexOf(item) > -1)
//        {
//            group = x
//            return true
//        }
//        return false
//    })
//    if (group)
//    {
//        group.Members.remove(item)
//        if (group.Members().length === 0)
//        {
//            observableGroupSet.remove(group)
//        }
//        return true
//    }
//    return false
//}
//function UpdateGroupedObservableArray<T>(item: T, groupName: string, observableGroupSet: KnockoutObservableArray<ObservableGroup<T>>)
//{
//    RemoveFromGroupedObservableArray(item, observableGroupSet)
//    AddToGroupedObservableArray(item, groupName, observableGroupSet)
//}
function MapMany(array, mapFunc) {
    return array.reduce((previous, current, i) => {
        return previous.concat(mapFunc(current));
    }, []);
}
class Guid {
    static NewGuid() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0;
            var v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
}
let CheckNamingConvention = (value) => {
    var regExp;
    if (value.length === 0) {
        return { HasError: true, ErrorDescription: ErrorMessage["Project_NameEmpty"] };
    }
    regExp = new RegExp("[^A-Za-z0-9_]");
    if (regExp.test(value)) {
        return { HasError: true, ErrorDescription: ErrorMessage["Project_InvalidCharacters"] };
    }
    regExp = new RegExp("^[0-9_]");
    if (regExp.test(value)) {
        return { HasError: true, ErrorDescription: ErrorMessage["Project_InvalidLeadingCharacter"] };
    }
    return {
        HasError: false,
        ErrorDescription: ""
    };
};
class PluginFactory {
}
PluginFactory.CreatePluginViewModelAsync = (pluginType, pluginModel) => __awaiter(this, void 0, void 0, function* () {
    let pluginIdentification;
    let pluginViewModel;
    let pluginViewModelRaw;
    pluginIdentification = PluginHive.FindPluginIdentification(pluginType, pluginModel.Description.Id);
    if (pluginIdentification) {
        pluginViewModelRaw = yield ConnectionManager.InvokeBroadcaster("GetPluginStringResource", pluginModel.Description.Id, pluginIdentification.ViewModelResourceName);
        pluginViewModel = new Function(pluginViewModelRaw + "; return ViewModelConstructor")()(pluginModel, pluginIdentification);
        return pluginViewModel;
    }
    else {
        throw new Error("No corresponding plugin description found.");
    }
});
class PluginHive {
}
// constructors
PluginHive.Initialize = () => {
    PluginHive.PluginIdentificationSet = new Map();
};
PluginHive.FindPluginIdentification = (pluginTypeName, pluginId) => {
    return PluginHive.PluginIdentificationSet.get(pluginTypeName).find(pluginIdentification => pluginIdentification.Id === pluginId);
};
class ChannelHubViewModel {
    constructor(channelHubModel) {
        // methods
        this.GetTransformedValue = (value) => {
            if (value === "NaN") {
                value = NaN;
            }
            this.EvaluatedTransferFunctionSet.forEach(tf => value = tf(value));
            return value;
        };
        this.CreateDefaultTransferFunction = () => {
            return new TransferFunctionViewModel(new TransferFunctionModel("0001-01-01T00:00:00Z", "polynomial", "permanent", "1;0"));
        };
        this.UpdateAssociation = (dataPort) => {
            switch (dataPort.DataDirection) {
                case DataDirectionEnum.Input:
                    this.ResetAssociation(false, this.AssociatedDataInput());
                    break;
                case DataDirectionEnum.Output:
                    this.ResetAssociation(false, dataPort);
                    break;
                default:
                    throw new Error("Not implemented.");
            }
            this.SetAssociation(dataPort);
        };
        this.GetAssociatedDataInputId = () => {
            return this.AssociatedDataInputId;
        };
        this.GetAssociatedDataOutputIdSet = () => {
            return this.AssociatedDataOutputIdSet;
        };
        this.AddTransferFunction = () => {
            this.TransferFunctionSet.push(this.SelectedTransferFunction());
        };
        this.DeleteTransferFunction = () => {
            this.TransferFunctionSet.remove(this.SelectedTransferFunction());
        };
        this.NewTransferFunction = () => {
            this.SelectedTransferFunction(this.CreateDefaultTransferFunction());
        };
        // commands
        this.SelectTransferFunction = (transferFunction) => {
            this.SelectedTransferFunction(transferFunction);
        };
        this.Name = ko.observable(channelHubModel.Name);
        this.Group = ko.observable(channelHubModel.Group);
        this.OneDasDataType = ko.observable(channelHubModel.OneDasDataType);
        this.SampleRate = ko.observable(channelHubModel.SampleRate);
        this.Guid = channelHubModel.Guid;
        this.CreationDateTime = channelHubModel.CreationDateTime;
        this.Unit = ko.observable(channelHubModel.Unit);
        this.TransferFunctionSet = ko.observableArray(channelHubModel.TransferFunctionSet.map(tf => new TransferFunctionViewModel(tf)));
        this.SelectedTransferFunction = ko.observable(this.CreateDefaultTransferFunction());
        this.IsSelected = ko.observable(false);
        this.AssociatedDataInput = ko.observable();
        this.AssociatedDataOutputSet = ko.observableArray();
        this.AssociatedDataInputId = channelHubModel.SerializerDataInputId;
        this.AssociatedDataOutputIdSet = channelHubModel.SerializerDataOutputIdSet;
        this.EvaluatedTransferFunctionSet = [];
    }
    IsAssociationAllowed(dataPort) {
        return (dataPort.OneDasDataType & 0xff) == (this.OneDasDataType() & 0xff);
    }
    SetAssociation(dataPort) {
        dataPort.AssociatedChannelHubSet.push(this);
        switch (dataPort.DataDirection) {
            case DataDirectionEnum.Input:
                this.AssociatedDataInput(dataPort);
                this.AssociatedDataInputId = dataPort.ToFullQualifiedIdentifier();
                break;
            case DataDirectionEnum.Output:
                let dataOutputId = dataPort.ToFullQualifiedIdentifier();
                this.AssociatedDataOutputSet.push(dataPort);
                if (this.AssociatedDataOutputIdSet.indexOf(dataOutputId) < 0) {
                    this.AssociatedDataOutputIdSet.push(dataPort.ToFullQualifiedIdentifier());
                }
                break;
        }
    }
    ResetAssociation(maintainWeakReference, ...dataPortSet) {
        dataPortSet.forEach(dataPort => {
            if (!dataPort) {
                return;
            }
            dataPort.AssociatedChannelHubSet.remove(this);
            switch (dataPort.DataDirection) {
                case DataDirectionEnum.Input:
                    this.AssociatedDataInput(null);
                    if (!maintainWeakReference) {
                        this.AssociatedDataInputId = null;
                    }
                    break;
                case DataDirectionEnum.Output:
                    this.AssociatedDataOutputSet.remove(dataPort);
                    if (!maintainWeakReference) {
                        let index = this.AssociatedDataOutputIdSet.indexOf(dataPort.ToFullQualifiedIdentifier());
                        if (index > -1) {
                            this.AssociatedDataOutputIdSet.splice(index, 1);
                        }
                    }
                    break;
            }
        });
    }
    ResetAllAssociations(maintainWeakReference) {
        if (this.AssociatedDataInput()) {
            this.ResetAssociation(maintainWeakReference, this.AssociatedDataInput());
        }
        this.ResetAssociation(maintainWeakReference, ...this.AssociatedDataOutputSet());
    }
    ToModel() {
        return {
            Name: this.Name(),
            Group: this.Group(),
            OneDasDataType: this.OneDasDataType(),
            SampleRate: this.SampleRate(),
            Guid: this.Guid,
            CreationDateTime: this.CreationDateTime,
            Unit: this.Unit(),
            TransferFunctionSet: this.TransferFunctionSet().map(tf => tf.ToModel()),
            SerializerDataInputId: this.AssociatedDataInputId,
            SerializerDataOutputIdSet: this.AssociatedDataOutputIdSet
        };
    }
}
class SlimOneDasSettingsViewModel {
    constructor(slimOneDasSettings) {
        this.OneDasName = slimOneDasSettings.OneDasName;
        this.AspBaseUrl = slimOneDasSettings.AspBaseUrl;
        this.BaseDirectoryPath = slimOneDasSettings.BaseDirectoryPath;
    }
}
class OneDasPerformanceInformationViewModel {
    constructor(performanceInformationModel) {
        let timeSpan;
        let secPerMinute = 60;
        let secPerHour = secPerMinute * 60;
        this.LateBy = performanceInformationModel.LateBy;
        this.CycleTime = performanceInformationModel.CycleTime;
        this.TimerDrift = performanceInformationModel.TimerDrift;
        this.CpuTime = performanceInformationModel.CycleTime;
        this.TimerDriftMicroseconds = Math.abs(this.TimerDrift) / 1000;
        // calculate system uptime
        timeSpan = performanceInformationModel.UpTime;
        this.UpTimeHours = Math.floor(timeSpan / secPerHour);
        timeSpan -= this.UpTimeHours * secPerHour;
        this.UpTimeMinutes = Math.floor(timeSpan / secPerMinute);
        timeSpan -= this.UpTimeMinutes * secPerMinute;
        this.UpTimeSeconds = timeSpan;
    }
}
class MessageLogEntryViewModel {
    constructor(prefix, message) {
        this.Prefix = prefix;
        this.Message = message;
    }
}
class MultiMappingEditorViewModel {
    constructor(dataPortSet, channelHubSet) {
        // methods
        this.Validate = () => {
            let startDataPortIndex;
            let startChannelHubIndex;
            let endDataPortIndex;
            let endChannelHubIndex;
            let filteredChannelHubSet;
            startDataPortIndex = this.DataPortStartIndex();
            startChannelHubIndex = this.ChannelHubStartIndex();
            endDataPortIndex = this.DataPortStartIndex() + this.Length() - 1;
            endChannelHubIndex = this.ChannelHubStartIndex() + this.Length() - 1;
            filteredChannelHubSet = this.FilteredChannelHubSet();
            this._dataPortSet.forEach(dataPort => dataPort.IsSelected(false));
            this._channelHubSet.forEach(channelHub => channelHub.IsSelected(false));
            // check parameters
            if ((startDataPortIndex < 0) || (endDataPortIndex >= this._dataPortSet.length) ||
                (startChannelHubIndex < 0) || (endChannelHubIndex >= filteredChannelHubSet.length)) {
                this.ErrorMessage(ErrorMessage["MultiMappingEditorViewModel_InvalidSettings"]);
                this.IsValid(false);
                return;
            }
            // check data type
            for (var i = 0; i < this.Length(); i++) {
                if (!filteredChannelHubSet[i + startChannelHubIndex].IsAssociationAllowed(this._dataPortSet[i + startDataPortIndex])) {
                    this.ErrorMessage(ErrorMessage["MultiMappingEditorViewModel_WrongDataType"]);
                    this.IsValid(false);
                    return;
                }
            }
            // no errors found (valid)
            this._dataPortSet.forEach((dataPort, index) => {
                if (startDataPortIndex <= index && index <= endDataPortIndex) {
                    dataPort.IsSelected(true);
                }
            });
            filteredChannelHubSet.forEach((channelHub, index) => {
                if (startChannelHubIndex <= index && index <= endChannelHubIndex) {
                    channelHub.IsSelected(true);
                }
            });
            this.ErrorMessage("");
            this.IsValid(true);
        };
        this._dataPortSet = dataPortSet;
        this._channelHubSet = channelHubSet;
        this.SelectedGroupName = ko.observable("");
        this.ChannelHubGroupNameSet = ko.observableArray(Array.from(new Set(MapMany(channelHubSet.map(channelHub => channelHub.Group), group => group().split("\n")))));
        this.FilteredChannelHubSet = ko.observableArray();
        this.DataPortStartIndex = ko.observable(0);
        this.ChannelHubStartIndex = ko.observable(0);
        this.Length = ko.observable(0);
        this.KeepVariableMappings = ko.observable(false);
        this.IsValid = ko.observable(false);
        this.ErrorMessage = ko.observable("");
        this.SelectedGroupName.subscribe(newValue => {
            this.FilteredChannelHubSet(channelHubSet.filter(channelHub => channelHub.Group().indexOf(this.SelectedGroupName()) > -1));
            this.Validate();
        });
        this.DataPortStartIndex.subscribe(newValue => {
            this.Validate();
        });
        this.ChannelHubStartIndex.subscribe(newValue => {
            this.Validate();
        });
        this.Length.subscribe(newValue => {
            this.Validate();
        });
    }
    // commands
    Apply() {
        let filteredChannelHubSet;
        let keepVariableMappings;
        let startDataPortIndex;
        let startChannelHubIndex;
        filteredChannelHubSet = this.FilteredChannelHubSet();
        keepVariableMappings = this.KeepVariableMappings();
        startDataPortIndex = this.DataPortStartIndex();
        startChannelHubIndex = this.ChannelHubStartIndex();
        for (var i = 0; i < this.Length(); i++) {
            if (!keepVariableMappings) {
                this._dataPortSet[i + startDataPortIndex].ResetAssociations(false);
            }
            filteredChannelHubSet[i + startChannelHubIndex].UpdateAssociation(this._dataPortSet[i + startDataPortIndex]);
        }
    }
}
class OneDasModuleViewModel {
    constructor(model) {
        this.GetByteCount = (booleanBitSize) => {
            if (booleanBitSize && this.DataType === OneDasDataTypeEnum.BOOLEAN) {
                booleanBitSize = parseInt(booleanBitSize);
                return Math.ceil(booleanBitSize * this.Size / 8);
            }
            else {
                return (this.DataType & 0x0FF) / 8 * this.Size;
            }
        };
        if (!Number.isInteger(model.Size)) {
            throw new Error("The value of size must be integer.");
        }
        if (model.Size <= 0) {
            throw new Error("The minimum value for size is 1.");
        }
        this.DataType = model.DataType;
        this.DataDirection = model.DataDirection;
        this.Size = model.Size;
    }
    ToModel() {
        return {
            DataType: this.DataType,
            Size: this.Size,
            DataDirection: this.DataDirection
        };
    }
}
class OneDasModuleSelectorViewModel {
    constructor(allowInputs, allowOutputs, allowBoolean) {
        // commands
        this.AddInputModule = () => {
            if (this.AllowInputs()) {
                this.InputCount(this.InputCount());
                this.CheckDataType(this.SelectedInputDataType());
                if (Number.isNaN(this.InputCount()) || this.InputCount() <= this.InputRemainingCount()) {
                    this.InputModuleSet.push(new OneDasModuleViewModel(new OneDasModuleModel(this.SelectedInputDataType(), DataDirectionEnum.Input, this.InputCount())));
                }
                this._onInputModuleSetChanged.dispatch(this, this.InputModuleSet());
            }
            else {
                throw new Error("Input modules are disabled.");
            }
        };
        this.DeleteInputModule = (value) => {
            this.InputModuleSet.pop();
            this.Update();
            this._onInputModuleSetChanged.dispatch(this, this.InputModuleSet());
        };
        this.AddOutputModule = () => {
            if (this.AllowOutputs()) {
                this.OutputCount(this.OutputCount());
                this.CheckDataType(this.SelectedOutputDataType());
                if (Number.isNaN(this.OutputCount()) || this.OutputCount() <= this.OutputRemainingCount()) {
                    this.OutputModuleSet.push(new OneDasModuleViewModel(new OneDasModuleModel(this.SelectedOutputDataType(), DataDirectionEnum.Output, this.OutputCount())));
                }
                this._onOutputModuleSetChanged.dispatch(this, this.OutputModuleSet());
            }
            else {
                throw new Error("Outputs modules are disabled.");
            }
        };
        this.DeleteOutputModule = (value) => {
            this.OutputModuleSet.pop();
            this.Update();
            this._onOutputModuleSetChanged.dispatch(this, this.OutputModuleSet());
        };
        this.AllowInputs = ko.observable(allowInputs);
        this.AllowOutputs = ko.observable(allowOutputs);
        this.AllowBoolean = ko.observable(allowBoolean);
        this.InputCount = ko.observable(1);
        this.OutputCount = ko.observable(1);
        this.InputRemainingBytes = ko.observable(NaN);
        this.OutputRemainingBytes = ko.observable(NaN);
        this.InputRemainingCount = ko.observable(NaN);
        this.OutputRemainingCount = ko.observable(NaN);
        this.SelectedInputDataType = ko.observable(OneDasDataTypeEnum.UINT16);
        this.SelectedOutputDataType = ko.observable(OneDasDataTypeEnum.UINT16);
        this.InputModuleSet = ko.observableArray();
        this.OutputModuleSet = ko.observableArray();
        this.SelectedInputDataType.subscribe(newValue => { this.Update(); });
        this.SelectedOutputDataType.subscribe(newValue => { this.Update(); });
        this.InputModuleSet.subscribe(newValue => { this.Update(); });
        this.OutputModuleSet.subscribe(newValue => { this.Update(); });
        this._onInputModuleSetChanged = new EventDispatcher();
        this._onOutputModuleSetChanged = new EventDispatcher();
        this.Update();
    }
    get OnInputModuleSetChanged() {
        return this._onInputModuleSetChanged;
    }
    get OnOutputModuleSetChanged() {
        return this._onOutputModuleSetChanged;
    }
    CheckDataType(oneDasDataType) {
        if (!this.AllowBoolean() && oneDasDataType === OneDasDataTypeEnum.BOOLEAN) {
            throw new Error("Wrong data direction of module.");
        }
    }
}
class ProjectViewModel {
    constructor(projectModel) {
        this.ToModel = () => {
            let model;
            model = {
                Description: this.Description.ToModel(),
                ChannelHubSet: this.ChannelHubSet().map(x => x.ToModel()),
                DataGatewaySettingsSet: this.DataGatewaySet().map(x => x.ToModel()),
                DataWriterSettingsSet: this.DataWriterSet().map(x => x.ToModel())
            };
            model.ChannelHubSet.sort((channelHub1, channelHub2) => this.CompareStrings(channelHub1.Name, channelHub2.Name));
            return model;
        };
        this.GetDataGateway = (index) => {
            return this.DataGatewaySet()[index];
        };
        this.GetDataWriter = (index) => {
            return this.DataWriterSet()[index];
        };
        this.SubscribeToChanges = () => {
            // ChannelHub <-> DataPort mapping
            this.DataGatewaySet.subscribe(changes => {
                changes.forEach(change => {
                    switch (change.status) {
                        // data-gateway added (subscribe to data-port array changes)
                        case "added":
                            change.value.DataPortSet.subscribe(changes => {
                                if (changes.length === 1) {
                                    switch (changes[0].status) {
                                        // data port added (do nothing)
                                        case "added":
                                            break;
                                        // data port removed (remove data-port <-> channel hub association)
                                        case "deleted":
                                            changes[0].value.ResetAssociations(false);
                                            break;
                                    }
                                }
                                else {
                                    this.UpdateMapping();
                                }
                            }, null, "arrayChange");
                            break;
                        // data-gateway removed (remove all data-port <-> channel hub associations)
                        case "deleted":
                            change.value.DataPortSet().forEach(dataPort => {
                                dataPort.ResetAssociations(true);
                            });
                            break;
                    }
                });
            }, null, "arrayChange");
        };
        this.RegisterDataGatewayViewModelBase = (DataGatewayViewModelBase) => {
            this.DataGatewaySet.push(DataGatewayViewModelBase);
        };
        this.RegisterDataWriterViewModelBase = (DataWriterViewModelBase) => {
            this.DataWriterSet.push(DataWriterViewModelBase);
        };
        this.OnNewChannelModalClosing = () => {
            this.NewChannelIsNotificationVisible(false);
        };
        this.Draggable = (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) => {
            element.addEventListener("dragstart", (event) => {
                this.SelectedDataPort(viewModel);
                event.dataTransfer.setData("Dummy", "");
            });
        };
        this.Dropable = (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) => {
            element.addEventListener("dragover", (event) => {
                if (this.IsDropAllowed(viewModel)) {
                    event.preventDefault();
                }
            });
            element.addEventListener("drop", (event) => {
                event.preventDefault();
                this.SelectedChannelHub(viewModel);
                this.SelectedChannelHub().UpdateAssociation(this.SelectedDataPort());
            });
        };
        this.IsDropAllowed = (channelHub) => {
            return channelHub.IsAssociationAllowed(this.SelectedDataPort());
        };
        this.ResetSearch = () => {
            this.SearchString("");
        };
        this.ShowContextMenu = (item, e) => {
            let offset;
            this.SelectedChannelHub_ContextMenu(item);
            this.SelectedChannelHub(item);
            if (e.ctrlKey) {
                return;
            }
            offset = $("body").offset();
            $("#Project_ChannelContextMenu").show().css({
                position: "absolute",
                left: e.pageX - offset.left,
                top: e.pageY - offset.top
            });
        };
        this.BeginEditChannel = () => {
            this.EditChannelDummyChannel().Name(this.SelectedChannelHub().Name());
            this.EditChannelDummyChannel().Group(this.SelectedChannelHub().Group());
            this.EditChannelDummyChannel().OneDasDataType(this.SelectedChannelHub().OneDasDataType());
            this.EditChannelDummyChannel().SampleRate(this.SelectedChannelHub().SampleRate());
        };
        this.EndEditChannel = () => {
            this.SelectedChannelHub().Name(this.EditChannelDummyChannel().Name());
            this.SelectedChannelHub().Group(this.EditChannelDummyChannel().Group());
            this.UpdateGroupedChannelHubSet(true);
        };
        this.AddNewChannel = () => {
            this.NewChannelName().split("\n").forEach(channelName => {
                this.ChannelHubSet.push(new ChannelHubViewModel(new ChannelHubModel(channelName, this.NewChannelGroup(), this.NewChannelSelectedOneDasDataType(), this.NewChannelSelectedSampleRate())));
            });
            this.NewChannelCheckError(false);
            this.NewChannelName("");
            this.NewChannelIsNotificationVisible(true);
        };
        this.DeleteChannel = () => {
            this.SelectedChannelHub().ResetAllAssociations(false);
            this.ChannelHubSet.remove(this.SelectedChannelHub());
        };
        this.AddNewDataGateway = (item) => __awaiter(this, void 0, void 0, function* () {
            let pluginModel;
            let pluginViewModel;
            let lastInstanceId;
            try {
                lastInstanceId = Math.max(...this.DataGatewaySet().map(dataGateway => dataGateway.Description.InstanceId));
                pluginModel = yield ConnectionManager.InvokeBroadcaster('CreateDataGatewaySettings', item.Name);
                pluginModel.Description.InstanceId = this.DataGatewaySet().length > 0 ? lastInstanceId + 1 : 1;
                pluginViewModel = (yield PluginFactory.CreatePluginViewModelAsync("DataGateway", pluginModel));
                yield pluginViewModel.InitializeAsync();
                this.DataGatewaySet.push(pluginViewModel);
            }
            catch (e) {
                alert(e.message);
            }
        });
        this.AddNewDataWriter = (item) => __awaiter(this, void 0, void 0, function* () {
            let pluginModel;
            let pluginViewModel;
            let lastInstanceId;
            try {
                lastInstanceId = Math.max(...this.DataWriterSet().map(dataWriter => dataWriter.Description.InstanceId));
                pluginModel = yield ConnectionManager.InvokeBroadcaster('CreateDataWriterSettings', item.Name);
                pluginModel.Description.InstanceId = this.DataWriterSet().length > 0 ? lastInstanceId + 1 : 1;
                pluginViewModel = (yield PluginFactory.CreatePluginViewModelAsync("DataWriter", pluginModel));
                yield pluginViewModel.InitializeAsync();
                this.DataWriterSet.push(pluginViewModel);
            }
            catch (e) {
                alert(e.message);
            }
        });
        this.SelectPlugin = (item) => {
            this.SelectedPlugin(item);
        };
        this.DeleteDataGateway = () => {
            this.DataGatewaySet.remove(this.SelectedPlugin());
        };
        this.DeleteDataWriter = () => {
            this.DataWriterSet.remove(this.SelectedPlugin());
        };
        this.ClearSelectedViewModels = (e) => {
            this.CurrentTitle(document.title);
            this.SelectedDataGateway(null);
            this.SelectedDataWriter(null);
        };
        this.SelectDataGateway = (e) => {
            if (e.page.ctx.__proto__ instanceof DataGatewayViewModelBase) {
                this.SelectedDataGateway(e.page.ctx);
                this.SelectedDataWriter(null);
            }
            this.CurrentTitle(document.title);
        };
        this.SelectDataWriter = (e) => {
            if (e.page.ctx.__proto__ instanceof DataWriterViewModelBase) {
                this.SelectedDataGateway(null);
                this.SelectedDataWriter(e.page.ctx);
            }
            this.CurrentTitle(document.title);
        };
        // mapping
        this.HubResetDataInputAssociation = () => {
            let channelHub = this.SelectedChannelHub();
            channelHub.ResetAssociation(false, channelHub.AssociatedDataInput());
        };
        this.HubResetDataOutputAssociations = () => {
            let channelHub = this.SelectedChannelHub();
            channelHub.ResetAssociation(false, ...channelHub.AssociatedDataOutputSet());
        };
        this.CreateDataPortIdMap = () => {
            let dataPortIdMap;
            dataPortIdMap = new Map();
            this.DataGatewaySet().forEach(dataGateway => {
                dataGateway.DataPortSet().forEach(dataPort => {
                    dataPortIdMap.set(dataPort, dataPort.ToFullQualifiedIdentifier());
                });
            });
            return dataPortIdMap;
        };
        this.UpdateMapping = () => {
            console.log("OneDAS: update mapping");
            let inputSet;
            let outputSet;
            let dataPortIdMap;
            inputSet = [];
            outputSet = [];
            dataPortIdMap = this.CreateDataPortIdMap();
            //
            this.DataGatewaySet().forEach((x) => {
                x.DataPortSet().forEach(y => {
                    switch (y.DataDirection) {
                        case DataDirectionEnum.Input:
                            inputSet.push(y);
                            break;
                        case DataDirectionEnum.Output:
                            outputSet.push(y);
                            break;
                    }
                });
            });
            this.ChannelHubSet().forEach(channelHub => {
                let dataPort;
                let inputId;
                this.SelectedChannelHub(channelHub);
                channelHub.ResetAllAssociations(true);
                // input
                inputId = channelHub.GetAssociatedDataInputId();
                if (inputId) {
                    dataPort = inputSet.find(input => dataPortIdMap.get(input) == inputId);
                    if (dataPort && channelHub.IsAssociationAllowed(dataPort)) {
                        channelHub.SetAssociation(dataPort);
                    }
                }
                // output
                channelHub.GetAssociatedDataOutputIdSet().forEach(outputId => {
                    dataPort = outputSet.find(output => dataPortIdMap.get(output) == outputId);
                    if (dataPort && channelHub.IsAssociationAllowed(dataPort)) {
                        channelHub.SetAssociation(dataPort);
                    }
                });
            });
        };
        let promiseSet;
        this.Description = new ProjectDescriptionViewModel(projectModel.Description);
        this.ChannelHubSet = ko.observableArray(projectModel.ChannelHubSet.map(x => new ChannelHubViewModel(x)));
        this.GroupedChannelHubSet = ko.observableArray(ObservableGroupBy(this.ChannelHubSet(), x => x.Name(), x => x.Group(), ""));
        this.DataGatewaySet = ko.observableArray();
        this.DataWriterSet = ko.observableArray();
        this.DataSnapshot = ko.observableArray();
        this.NewChannelName = ko.observable("");
        this.NewChannelGroup = ko.observable("Default");
        this.NewChannelSelectedOneDasDataType = ko.observable(OneDasDataTypeEnum.INT16);
        this.NewChannelSelectedSampleRate = ko.observable(SampleRateEnum.SampleRate_1);
        this.NewChannelCheckError = ko.observable(true);
        this.NewChannelNameHasError = ko.observable(false);
        this.NewChannelNameErrorDescription = ko.observable("");
        this.NewChannelGroupHasError = ko.observable(false);
        this.NewChannelGroupErrorDescription = ko.observable("");
        this.NewChannelIsNotificationVisible = ko.observable(false);
        this.EditChannelNameHasError = ko.observable(false);
        this.EditChannelNameErrorDescription = ko.observable("");
        this.EditChannelGroupHasError = ko.observable(false);
        this.EditChannelGroupErrorDescription = ko.observable("");
        this.EditChannelDummyChannel = ko.observable(new ChannelHubViewModel(new ChannelHubModel("", "", 0, 0)));
        this.MultiMappingEditor = ko.observable();
        this.SearchString = ko.observable("");
        this.SearchString.extend({ rateLimit: { timeout: 300, method: "notifyWhenChangesStop" } });
        this.SelectedDataPort = ko.observable();
        this.SelectedChannelHub = ko.observable();
        this.SelectedChannelHub_ContextMenu = ko.observable();
        this.SelectedDataGateway = ko.observable();
        this.SelectedDataWriter = ko.observable();
        this.SelectedPlugin = ko.observable();
        this.CurrentTitle = ko.observable();
        this.IsFullEditingEnabled = ko.observable(this.Description.CampaignVersion() === 0);
        // Multi-Mapping
        this.SelectedDataGateway.subscribe(newValue => {
            if (newValue) {
                if (this.MultiMappingEditor()) {
                    this.InitializeMultiMappingMode();
                }
            }
            else {
                this.MultiMappingEditor(null);
            }
        });
        // validation
        this.NewChannelName.subscribe(value => {
            let result;
            let valueSet;
            if (!this.NewChannelCheckError()) {
                result = { HasError: false, ErrorDescription: "" };
            }
            else {
                valueSet = value.split("\n");
                valueSet.some(currentValue => {
                    if ($.inArray(currentValue, this.ChannelHubSet().map((element) => element.Name())) > -1 || valueSet.length !== new Set(valueSet).size) {
                        result = { HasError: true, ErrorDescription: ErrorMessage["Project_ChannelAlreadyExists"] };
                    }
                    else {
                        result = CheckNamingConvention(currentValue);
                    }
                    if (result.HasError) {
                        return true;
                    }
                });
            }
            this.NewChannelNameHasError(result.HasError);
            this.NewChannelNameErrorDescription(result.ErrorDescription);
            this.NewChannelCheckError(true);
            this.NewChannelIsNotificationVisible(false);
        });
        this.NewChannelGroup.subscribe(value => {
            let result;
            let valueSet;
            if (!this.NewChannelCheckError()) {
                result = { HasError: false, ErrorDescription: "" };
            }
            else {
                valueSet = value.split("\n");
                valueSet.forEach(currentValue => {
                    if (valueSet.length !== new Set(valueSet).size) {
                        result = { HasError: true, ErrorDescription: ErrorMessage["Project_IsAlreadyInGroup"] };
                    }
                    else {
                        result = CheckNamingConvention(currentValue);
                    }
                    if (result.HasError) {
                        return;
                    }
                });
            }
            this.NewChannelGroupHasError(result.HasError);
            this.NewChannelGroupErrorDescription(result.ErrorDescription);
        });
        this.EditChannelDummyChannel().Name.subscribe(value => {
            let result;
            if (this.EditChannelDummyChannel().Name() !== this.SelectedChannelHub().Name() && // channel name is not the same as before
                $.inArray(value, this.ChannelHubSet().map((element) => element.Name())) > -1) {
                result = { HasError: true, ErrorDescription: ErrorMessage["Project_ChannelAlreadyExists"] };
            }
            else {
                result = CheckNamingConvention(value);
            }
            this.EditChannelNameHasError(result.HasError);
            this.EditChannelNameErrorDescription(result.ErrorDescription);
        });
        this.EditChannelDummyChannel().Group.subscribe(value => {
            let result;
            let valueSet;
            if (!this.NewChannelCheckError()) {
                result = { HasError: false, ErrorDescription: "" };
            }
            else {
                valueSet = value.split("\n");
                valueSet.forEach(currentValue => {
                    if (valueSet.length !== new Set(valueSet).size) {
                        result = { HasError: true, ErrorDescription: ErrorMessage["Project_IsAlreadyInGroup"] };
                    }
                    else {
                        result = CheckNamingConvention(currentValue);
                    }
                    if (result.HasError) {
                        return;
                    }
                });
            }
            this.EditChannelGroupHasError(result.HasError);
            this.EditChannelGroupErrorDescription(result.ErrorDescription);
        });
        // channel grouping
        this.ChannelHubSet.subscribe(changes => {
            this.UpdateGroupedChannelHubSet(true);
        }, null, "arrayChange");
        // search
        this.SearchString.subscribe(value => {
            this.UpdateGroupedChannelHubSet(false);
        });
    }
    // methods
    InitializeMultiMappingMode() {
        this.MultiMappingEditor(new MultiMappingEditorViewModel(this.SelectedDataGateway().DataPortSet(), this.ChannelHubSet()));
    }
    InitializeAsync(dataGatewaySettingsModelSet, dataWriterSettingsModelSet) {
        return __awaiter(this, void 0, void 0, function* () {
            try {
                let dataGatewaySet;
                let dataWriterSet;
                // register callbacks
                this.SubscribeToChanges();
                // data gateway
                dataGatewaySet = (yield Promise.all(dataGatewaySettingsModelSet.map((pluginModel) => __awaiter(this, void 0, void 0, function* () {
                    return yield PluginFactory.CreatePluginViewModelAsync("DataGateway", pluginModel);
                }))));
                dataGatewaySet.forEach(dataGateway => {
                    if (dataGateway.Description.InstanceId === 0) {
                        dataGateway.Description.InstanceId = Math.max(...dataGatewaySet.map(x => x.Description.InstanceId)) + 1;
                    }
                });
                yield Promise.all(dataGatewaySet.map(dataGateway => dataGateway.InitializeAsync()));
                this.DataGatewaySet(dataGatewaySet);
                // data writer
                dataWriterSet = (yield Promise.all(dataWriterSettingsModelSet.map((pluginModel) => __awaiter(this, void 0, void 0, function* () {
                    return yield PluginFactory.CreatePluginViewModelAsync("DataWriter", pluginModel);
                }))));
                dataWriterSet.forEach(dataWriter => {
                    if (dataWriter.Description.InstanceId === 0) {
                        dataWriter.Description.InstanceId = Math.max(...dataWriterSet.map(x => x.Description.InstanceId)) + 1;
                    }
                });
                yield Promise.all(dataWriterSet.map(dataWriter => dataWriter.InitializeAsync()));
                this.DataWriterSet(dataWriterSet);
                // finish
                this.UpdateMapping();
            }
            catch (e) {
                alert(e.message);
            }
        });
    }
    UpdateGroupedChannelHubSet(resetSearchString) {
        let groupedChannelHubSet;
        if (this.IsUpdating) {
            return;
        }
        this.IsUpdating = true;
        if (resetSearchString) {
            this.SearchString("");
        }
        groupedChannelHubSet = ObservableGroupBy(this.ChannelHubSet(), x => x.Name(), x => x.Group(), this.SearchString());
        groupedChannelHubSet.forEach(channelHubSet => channelHubSet.Members.sort((channelHub1, channelHub2) => this.CompareStrings(channelHub1.Name(), channelHub2.Name())));
        groupedChannelHubSet.sort((channelHubSet1, channelHubSet2) => this.CompareStrings(channelHubSet1.Key, channelHubSet2.Key));
        this.GroupedChannelHubSet(groupedChannelHubSet);
        this.IsUpdating = false;
    }
    CompareStrings(string1, string2) {
        let comparison = 0;
        if (string1 > string2) {
            comparison = 1;
        }
        else if (string1 < string2) {
            comparison = -1;
        }
        return comparison;
    }
    // commands
    ToggleMultiMappingMode() {
        if (this.MultiMappingEditor()) {
            this.MultiMappingEditor(null);
        }
        else {
            this.InitializeMultiMappingMode();
        }
    }
}
class ProjectDescriptionViewModel {
    constructor(projectDescriptionModel) {
        this.ToModel = () => {
            return {
                FormatVersion: this.FormatVersion,
                CampaignVersion: this.CampaignVersion(),
                Guid: this.Guid,
                CampaignPrimaryGroup: this.CampaignPrimaryGroup,
                CampaignSecondaryGroup: this.CampaignSecondaryGroup,
                CampaignName: this.CampaignName,
            };
        };
        this.FormatVersion = projectDescriptionModel.FormatVersion;
        this.Guid = projectDescriptionModel.Guid;
        this.CampaignVersion = ko.observable(projectDescriptionModel.CampaignVersion);
        this.CampaignPrimaryGroup = projectDescriptionModel.CampaignPrimaryGroup;
        this.CampaignSecondaryGroup = projectDescriptionModel.CampaignSecondaryGroup;
        this.CampaignName = projectDescriptionModel.CampaignName;
    }
}
class TransferFunctionViewModel {
    constructor(transferFunctionModel) {
        this.DateTime = ko.observable(transferFunctionModel.DateTime);
        this.Type = ko.observable(transferFunctionModel.Type);
        this.Option = ko.observable(transferFunctionModel.Option);
        this.Argument = ko.observable(transferFunctionModel.Argument);
    }
    // methods
    ToModel() {
        return new TransferFunctionModel(this.DateTime(), this.Type(), this.Option(), this.Argument());
    }
}
class ControlViewModel extends WorkspaceBase {
    constructor(activeProject) {
        super('control', 'Control', 'control.html', activeProject);
        // commands
        this.GetProjectDescriptions = () => __awaiter(this, void 0, void 0, function* () {
            let projectDescriptionSet;
            try {
                projectDescriptionSet = yield ConnectionManager.InvokeBroadcaster('GetProjectDescriptions');
                this.ProjectDescriptionSet(projectDescriptionSet.map(projectDescription => new ProjectDescriptionViewModel(projectDescription)));
            }
            catch (e) {
                alert(e.message);
            }
        });
        this.ActivateProject = (projectDescriptionViewModel) => __awaiter(this, void 0, void 0, function* () {
            try {
                yield ConnectionManager.InvokeBroadcaster("ActivateProject", projectDescriptionViewModel.ToModel());
            }
            catch (e) {
                alert(e.message);
            }
        });
        this.StartOneDas = () => __awaiter(this, void 0, void 0, function* () {
            try {
                yield ConnectionManager.InvokeBroadcaster("StartOneDas");
            }
            catch (e) {
                alert(e.message);
            }
        });
        this.StopOneDas = () => __awaiter(this, void 0, void 0, function* () {
            try {
                yield ConnectionManager.InvokeBroadcaster("StopOneDas");
            }
            catch (e) {
                alert(e.message);
            }
        });
        this.ProjectDescriptionSet = ko.observableArray();
    }
}
class DiscoveryViewModel extends WorkspaceBase {
    constructor(activeProject) {
        super('discovery', 'Discovery', 'discovery.html', activeProject);
    }
}
class EditorViewModel extends WorkspaceBase {
    constructor(activeProject) {
        super('editor', 'Editor', 'editor.html', activeProject);
        // commands
        this.GetProjectDescriptions = () => __awaiter(this, void 0, void 0, function* () {
            let projectDescriptionSet;
            try {
                projectDescriptionSet = yield ConnectionManager.InvokeBroadcaster('GetProjectDescriptions');
                this.ProjectDescriptionSet(projectDescriptionSet.map(projectDescription => new ProjectDescriptionViewModel(projectDescription)));
            }
            catch (e) {
                alert(e.message);
            }
        });
        this.CreateNewProject = () => __awaiter(this, void 0, void 0, function* () {
            let projectModel;
            try {
                projectModel = yield ConnectionManager.InvokeBroadcaster("CreateProject", this.CampaignPrimaryGroup(), this.CampaignSecondaryGroup(), this.CampaignName());
                this.Project(new ProjectViewModel(projectModel));
                yield this.Project().InitializeAsync(projectModel.DataGatewaySettingsSet, projectModel.DataWriterSettingsSet);
                this.NewProjectCheckError(false);
                this.CampaignPrimaryGroup("");
                this.NewProjectCheckError(false);
                this.CampaignSecondaryGroup("");
                this.NewProjectCheckError(false);
                this.CampaignName("");
            }
            catch (e) {
                alert(e.message);
            }
        });
        this.SaveProject = () => __awaiter(this, void 0, void 0, function* () {
            try {
                this.Project().Description.CampaignVersion(this.Project().Description.CampaignVersion() + 1);
                yield ConnectionManager.InvokeBroadcaster("SaveProject", this.Project().ToModel());
                console.log("OneDAS: project saved");
            }
            catch (e) {
                this.Project().Description.CampaignVersion(this.Project().Description.CampaignVersion() - 1);
                alert(e.message);
            }
        });
        this.OpenProject = (projectDescriptionViewModel) => __awaiter(this, void 0, void 0, function* () {
            try {
                let projectModel;
                projectModel = yield ConnectionManager.InvokeBroadcaster("OpenProject", projectDescriptionViewModel.ToModel());
                this.Project(new ProjectViewModel(projectModel));
                yield this.Project().InitializeAsync(projectModel.DataGatewaySettingsSet, projectModel.DataWriterSettingsSet);
                console.log("OneDAS: project opened");
            }
            catch (e) {
                alert(e.message);
            }
        });
        this.CloseProject = () => {
            this.Project(null);
        };
        this.ProjectDescriptionSet = ko.observableArray();
        this.Project = ko.observable();
        this.CampaignPrimaryGroup = ko.observable("");
        this.CampaignSecondaryGroup = ko.observable("");
        this.CampaignName = ko.observable("");
        this.CampaignPrimaryGroupHasError = ko.observable(false);
        this.CampaignSecondaryGroupHasError = ko.observable(false);
        this.CampaignNameHasError = ko.observable(false);
        this.CampaignPrimaryGroupErrorDescription = ko.observable("");
        this.CampaignSecondaryGroupErrorDescription = ko.observable("");
        this.CampaignNameErrorDescription = ko.observable("");
        this.NewProjectCheckError = ko.observable(false);
        // validation
        this.CampaignPrimaryGroup.subscribe((value) => {
            var result;
            if (!this.NewProjectCheckError()) {
                result = { HasError: false, ErrorDescription: "" };
            }
            else {
                result = CheckNamingConvention(value);
            }
            this.CampaignPrimaryGroupHasError(result.HasError);
            this.CampaignPrimaryGroupErrorDescription(result.ErrorDescription);
            this.NewProjectCheckError(true);
        });
        this.CampaignSecondaryGroup.subscribe((value) => {
            var result;
            if (!this.NewProjectCheckError()) {
                result = { HasError: false, ErrorDescription: "" };
            }
            else {
                result = CheckNamingConvention(value);
            }
            this.CampaignSecondaryGroupHasError(result.HasError);
            this.CampaignSecondaryGroupErrorDescription(result.ErrorDescription);
            this.NewProjectCheckError(true);
        });
        this.CampaignName.subscribe((value) => {
            var result;
            if (!this.NewProjectCheckError()) {
                result = { HasError: false, ErrorDescription: "" };
            }
            else {
                result = CheckNamingConvention(value);
            }
            this.CampaignNameHasError(result.HasError);
            this.CampaignNameErrorDescription(result.ErrorDescription);
            this.NewProjectCheckError(true);
        });
    }
}
class ExtensionViewModel extends WorkspaceBase {
    constructor(activeProject) {
        super('extension', 'Extensions', 'extension.html', activeProject);
    }
}
class LiveViewViewModel extends WorkspaceBase {
    constructor(activeProject) {
        super('liveview', 'Live View', 'liveview.html', activeProject);
        this.CreateChart = (context, channelHub, valueSet) => {
            ///////////////////////////
            let labels = [];
            labels.length = valueSet.length;
            labels.fill(NaN);
            return new Chart(context, {
                type: "line",
                data: {
                    labels: labels,
                    datasets: [{
                            data: valueSet,
                            backgroundColor: "rgba(54, 162, 235, 0.2)",
                            borderColor: "rgba(54, 162, 235)",
                            borderWidth: 1,
                            lineTension: 0.25,
                            pointRadius: 0
                        }]
                },
                options: {
                    animation: {
                        duration: this._speed * 1.5,
                        easing: "linear"
                    },
                    legend: {
                        display: false
                    },
                    scales: {
                        xAxes: [{
                                //type: "time",
                                //time: {
                                //    displayFormats: {
                                //        "millisecond": "HH:mm:ss",
                                //        "second": "HH:mm:ss",
                                //        "minute": "HH:mm:ss",
                                //        "hour": "HH:mm:ss"
                                //    }
                                //},
                                ///////////////////////////
                                //ticks: {
                                //    autoSkip: true,
                                //    minRotation: 45,
                                //    maxRotation: 45,
                                //},
                                display: false
                            }],
                        yAxes: [{
                                type: "linear",
                                position: "left",
                                scaleLabel: {
                                    display: true,
                                    labelString: channelHub.Unit()
                                },
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                    },
                    title: {
                        display: true,
                        text: channelHub.Name()
                    },
                    tooltips: {
                        enabled: false
                    }
                }
            });
        };
        // commands
        this.ToggleChannelHubIsSelected = (channelHub) => {
            channelHub.IsSelected(!channelHub.IsSelected());
            this.ReinitializeCharts(true, null);
        };
        this.InitializeCharts = () => {
            this.ReinitializeCharts(false, true);
        };
        this._speed = 200;
        this._iteration = 0;
        this.SelectedChannelHubSet = ko.observableArray();
        this.SelectedChartContextSet = ko.observable();
        this.SelectedLiveViewPeriod = ko.observable(LiveViewPeriodEnum.Period_60);
        this.ActiveProject.subscribe(newValue => {
            this.SelectedChannelHubSet.removeAll();
            this.SelectedChartContextSet(new Map());
        });
        this.SelectedLiveViewPeriod.subscribe(newValue => {
            this.ReinitializeCharts(false, false);
        });
        ConnectionManager.Broadcaster.on("SendLiveViewData", (subscriptionId, dateTime, dataSnapshot) => {
            let index;
            index = 0;
            this._iteration = (this._iteration + 1) % 4;
            if (this._subscriptionId === subscriptionId) {
                this.SelectedChartContextSet().forEach(chartContext => {
                    if (chartContext.ChannelHub.OneDasDataType() === OneDasDataTypeEnum.BOOLEAN) {
                        //chartContext.ValueSet.push({ x: dateTime, y: dataSnapshot[index] ? 1 : 0 })
                        ///////////////////////////
                        chartContext.ValueSet.push((dataSnapshot[index] ? 1 : 0));
                    }
                    else {
                        //chartContext.ValueSet.push({ x: dateTime, y: chartContext.ChannelHub.GetTransformedValue(dataSnapshot[index]) })
                        ///////////////////////////
                        chartContext.ValueSet.push(chartContext.ChannelHub.GetTransformedValue(dataSnapshot[index]));
                    }
                    index++;
                    chartContext.ValueSet.shift();
                    if (this.SelectedLiveViewPeriod() === LiveViewPeriodEnum.Period_60 || this._iteration == 0) {
                        chartContext.Chart.update();
                    }
                });
            }
        });
    }
    // methods
    ReinitializeCharts(reuseChartSet, reuseDatasetSet) {
        return __awaiter(this, void 0, void 0, function* () {
            let referenceSet;
            let activeChannelHubSet;
            //let valueSet: Chart.ChartPoint[]
            let valueSet; ///////////////////////////
            let context;
            let chart;
            this._subscriptionId = 0; // important!
            referenceSet = this.SelectedChartContextSet();
            activeChannelHubSet = this.ActiveProject().ChannelHubSet().filter(channelHub => channelHub.AssociatedDataInput());
            this.SelectedChannelHubSet(this.ActiveProject().ChannelHubSet().filter(channelHub => channelHub.IsSelected()));
            this.SelectedChartContextSet(new Map());
            this.SelectedChannelHubSet().forEach(channelHub => {
                if (!reuseChartSet && reuseDatasetSet && referenceSet.has(channelHub.Guid)) {
                    valueSet = referenceSet.get(channelHub.Guid).ValueSet;
                }
                else {
                    valueSet = [];
                    valueSet.length = 1000 / this._speed * this.SelectedLiveViewPeriod();
                    valueSet.fill(NaN);
                }
                if (reuseChartSet && referenceSet.has(channelHub.Guid)) {
                    this.SelectedChartContextSet().set(channelHub.Guid, referenceSet.get(channelHub.Guid));
                }
                else {
                    context = document.getElementById("chart_" + channelHub.Guid);
                    chart = this.CreateChart(context, channelHub, valueSet); ///////////////////////////
                    this.SelectedChartContextSet().set(channelHub.Guid, new ChartContext(channelHub, chart, valueSet)); ///////////////////////////
                }
            });
            this.SelectedChartContextSet().forEach(chartContext => {
                chartContext.Chart.update(0);
            });
            try {
                this._subscriptionId = yield ConnectionManager.InvokeBroadcaster("UpdateLiveViewSubscription", this.SelectedChannelHubSet().map(channelHub => channelHub.Guid));
            }
            catch (e) {
                alert(e.message);
            }
        });
    }
}
class StartViewModel extends WorkspaceBase {
    constructor(activeProject) {
        super('start', 'Start', 'start.html', activeProject);
    }
}
class PluginViewModelBase {
    constructor(pluginSettingsModel, pluginIdentification) {
        this.SendActionRequest = (instanceId, methodName, data) => __awaiter(this, void 0, void 0, function* () {
            return yield ConnectionManager.InvokeBroadcaster("RequestAction", new ActionRequest(this.Description.Id, instanceId, methodName, data));
        });
        // commands
        this.EnableSettingsMode = () => {
            this.IsInSettingsMode(true);
        };
        this.DisableSettingsMode = () => {
            this.IsInSettingsMode(false);
        };
        this.ToggleSettingsMode = () => {
            this.IsInSettingsMode(!this.IsInSettingsMode());
        };
        this.Model = pluginSettingsModel;
        this.Description = new PluginDescriptionViewModel(pluginSettingsModel.Description);
        this.PluginIdentification = pluginIdentification;
        this.IsInSettingsMode = ko.observable(false);
    }
    ExtendModel(model) {
        //
    }
    ToModel() {
        let model = {
            $type: this.Model.$type,
            Description: this.Description.ToModel()
        };
        this.ExtendModel(model);
        return model;
    }
}
class DataGatewayViewModelBase extends PluginViewModelBase {
    constructor(model, identification) {
        super(model, identification);
        this.MaximumDatasetAge = ko.observable(model.MaximumDatasetAge);
        this.DataPortSet = ko.observableArray();
    }
    ExtendModel(model) {
        super.ExtendModel(model);
        model.MaximumDatasetAge = Number.parseInt(this.MaximumDatasetAge());
    }
}
class DataPortViewModelBase {
    // constructors
    constructor(name, oneDasDataType, dataDirection, associatedDataGateway) {
        this.Name = ko.observable(name);
        this.OneDasDataType = oneDasDataType;
        this.DataDirection = dataDirection;
        this.IsSelected = ko.observable(false);
        this.AssociatedChannelHubSet = ko.observableArray();
        this.AssociatedDataGateway = associatedDataGateway;
        this.LiveDescription = ko.computed(() => {
            let result;
            result = "<div class='text-left'>" + this.Name() + "</div><div class='text-left'>" + EnumerationHelper.GetEnumLocalization('OneDasDataTypeEnum', this.OneDasDataType) + "</div>";
            if (this.AssociatedChannelHubSet().length > 0) {
                this.AssociatedChannelHubSet().forEach(channelHub => {
                    result += "</br ><div class='text-left'>" + channelHub.Name() + "</div><div class='text-left'>" + EnumerationHelper.GetEnumLocalization('OneDasDataTypeEnum', channelHub.OneDasDataType()) + "</div>";
                });
            }
            return result;
        });
    }
    ToFullQualifiedIdentifier() {
        return this.AssociatedDataGateway.Description.Id + " (" + this.AssociatedDataGateway.Description.InstanceId + ") / " + this.GetId();
    }
    ResetAssociations(maintainWeakReference) {
        if (this.AssociatedChannelHubSet().length > 0) {
            this.AssociatedChannelHubSet().forEach(channelHub => channelHub.ResetAssociation(maintainWeakReference, this));
        }
    }
}
class DataWriterViewModelBase extends PluginViewModelBase {
    constructor(model, identification) {
        super(model, identification);
        this.FileGranularity = ko.observable(model.FileGranularity);
    }
    ExtendModel(model) {
        super.ExtendModel(model);
        model.FileGranularity = this.FileGranularity();
    }
}
class PluginDescriptionViewModel {
    constructor(pluginDescriptionModel) {
        this.ProductVersion = pluginDescriptionModel.ProductVersion;
        this.Id = pluginDescriptionModel.Id;
        this.InstanceId = pluginDescriptionModel.InstanceId;
        this.IsEnabled = ko.observable(pluginDescriptionModel.IsEnabled);
    }
    ToModel() {
        var model = {
            ProductVersion: this.ProductVersion,
            Id: this.Id,
            InstanceId: this.InstanceId,
            IsEnabled: this.IsEnabled()
        };
        return model;
    }
}
class PluginIdentificationViewModel {
    constructor(pluginIdentificationModel) {
        this.ProductVersion = pluginIdentificationModel.ProductVersion;
        this.Id = pluginIdentificationModel.Id;
        this.Name = pluginIdentificationModel.Name;
        this.Description = pluginIdentificationModel.Description;
        this.ViewResourceName = pluginIdentificationModel.ViewResourceName;
        this.ViewModelResourceName = pluginIdentificationModel.ViewModelResourceName;
    }
}
class AppViewModel {
    constructor(appModel) {
        // methods
        this.InitializeProject = (projectModel) => __awaiter(this, void 0, void 0, function* () {
            let project;
            project = new ProjectViewModel(projectModel);
            yield project.InitializeAsync(projectModel.DataGatewaySettingsSet, projectModel.DataWriterSettingsSet);
            this.ActiveProject(project);
            console.log("OneDAS: project activated");
        });
        // commands
        this.AcknowledgeError = () => __awaiter(this, void 0, void 0, function* () {
            try {
                yield ConnectionManager.InvokeBroadcaster('AcknowledgeError');
            }
            catch (e) {
                alert(e.message);
            }
        });
        this.UpdateNewSlimOneDasSettings = () => {
            this.NewSlimOneDasSettingsOneDasName(this.SlimOneDasSettings().OneDasName);
            this.NewSlimOneDasSettingsAspBaseUrl(this.SlimOneDasSettings().AspBaseUrl);
            this.NewSlimOneDasSettingsBaseDirectoryPath(this.SlimOneDasSettings().BaseDirectoryPath);
        };
        this.SaveSlimOneDasSettings = () => __awaiter(this, void 0, void 0, function* () {
            // improve! find better solution in combination with validation
            this.SlimOneDasSettings().OneDasName = this.NewSlimOneDasSettingsOneDasName();
            this.SlimOneDasSettings().AspBaseUrl = this.NewSlimOneDasSettingsAspBaseUrl();
            this.SlimOneDasSettings().BaseDirectoryPath = this.NewSlimOneDasSettingsBaseDirectoryPath();
            try {
                yield ConnectionManager.InvokeBroadcaster('SaveSlimOneDasSettings', this.SlimOneDasSettings());
            }
            catch (e) {
                alert(e.message);
            }
        });
        this.OneDasState = ko.observable(0); // default
        this.WorkspaceSet = ko.observableArray();
        this.ClientMessageLog = ko.observableArray();
        this.PerformanceInformation = ko.observable();
        this.NewSlimOneDasSettingsOneDasName = ko.observable();
        this.NewSlimOneDasSettingsAspBaseUrl = ko.observable();
        this.NewSlimOneDasSettingsBaseDirectoryPath = ko.observable();
        this.ActiveProject = ko.observable();
        // enumeration description
        EnumerationHelper.Description["FileGranularityEnum_Minute_1"] = "1 file per minute";
        EnumerationHelper.Description["FileGranularityEnum_Minute_10"] = "1 file per 10 minutes";
        EnumerationHelper.Description["FileGranularityEnum_Hour"] = "1 file per hour";
        EnumerationHelper.Description["FileGranularityEnum_Day"] = "1 file per day";
        EnumerationHelper.Description["LiveViewPeriodEnum_Period_60"] = "1 min";
        EnumerationHelper.Description["LiveViewPeriodEnum_Period_600"] = "10 min";
        EnumerationHelper.Description["LiveViewPeriodEnum_Period_3600"] = "1 hour";
        EnumerationHelper.Description["OneDasDataTypeEnum_BOOLEAN"] = "BOOLEAN";
        EnumerationHelper.Description["OneDasDataTypeEnum_UINT8"] = "UINT8";
        EnumerationHelper.Description["OneDasDataTypeEnum_INT8"] = "INT8";
        EnumerationHelper.Description["OneDasDataTypeEnum_UINT16"] = "UINT16";
        EnumerationHelper.Description["OneDasDataTypeEnum_INT16"] = "INT16";
        EnumerationHelper.Description["OneDasDataTypeEnum_UINT32"] = "UINT32";
        EnumerationHelper.Description["OneDasDataTypeEnum_INT32"] = "INT32";
        EnumerationHelper.Description["OneDasDataTypeEnum_FLOAT32"] = "FLOAT32";
        EnumerationHelper.Description["OneDasDataTypeEnum_FLOAT64"] = "FLOAT64";
        EnumerationHelper.Description["SampleRateEnum_SampleRate_100"] = "100 Hz";
        EnumerationHelper.Description["SampleRateEnum_SampleRate_25"] = "25 Hz";
        EnumerationHelper.Description["SampleRateEnum_SampleRate_5"] = "5 Hz";
        EnumerationHelper.Description["SampleRateEnum_SampleRate_1"] = "1 Hz";
        // app model      
        this.ClientSet = ko.observableArray(appModel.ClientSet);
        this.LastError = ko.observable(appModel.LastError);
        this.OneDasState(appModel.OneDasState);
        this.SlimOneDasSettings = ko.observable(new SlimOneDasSettingsViewModel(appModel.SlimOneDasSettings));
        this.WorkspaceSet.push(new StartViewModel(this.ActiveProject));
        this.WorkspaceSet.push(new ControlViewModel(this.ActiveProject));
        this.WorkspaceSet.push(new LiveViewViewModel(this.ActiveProject));
        this.WorkspaceSet.push(new EditorViewModel(this.ActiveProject));
        this.WorkspaceSet.push(new DiscoveryViewModel(this.ActiveProject));
        this.WorkspaceSet.push(new ExtensionViewModel(this.ActiveProject));
        this.ReducedWorkspaceSet = ko.observableArray(this.WorkspaceSet().slice(1, this.WorkspaceSet().length));
        // register components
        PluginHive.PluginIdentificationSet.set("DataGateway", appModel.DataGatewayPluginIdentificationSet.map(x => new PluginIdentificationViewModel(x)));
        PluginHive.PluginIdentificationSet.set("DataWriter", appModel.DataWriterPluginIdentificationSet.map(x => new PluginIdentificationViewModel(x)));
        PluginHive.PluginIdentificationSet.get("DataGateway").forEach(pluginIdentification => {
            ko.components.register(pluginIdentification.Id, {
                template: {
                    PluginType: "DataGateway", PluginIdentification: pluginIdentification
                },
                viewModel: {
                    createViewModel: (params, componentInfo) => {
                        return params.GetDataGatewayCallback(params.Index);
                    }
                }
            });
        });
        PluginHive.PluginIdentificationSet.get("DataWriter").forEach(pluginIdentification => {
            ko.components.register(pluginIdentification.Id, {
                template: {
                    PluginType: "DataWriter", PluginIdentification: pluginIdentification
                },
                viewModel: {
                    createViewModel: (params, componentInfo) => {
                        return params.GetDataWriterCallback(params.Index);
                    }
                }
            });
        });
        // project
        this.ActiveProject.subscribe(newValue => {
            if (newValue) {
                newValue.ChannelHubSet().forEach(channelHub => {
                    channelHub.EvaluatedTransferFunctionSet = channelHub.TransferFunctionSet().map(tf => {
                        switch (tf.Type()) {
                            case "polynomial":
                                let argumentSet;
                                let coefficient0;
                                let coefficient1;
                                argumentSet = tf.Argument().split(";");
                                coefficient0 = math.eval(argumentSet[1]);
                                coefficient1 = math.eval(argumentSet[0]);
                                return (x) => { return x * coefficient1 + coefficient0; };
                            case "function":
                                let evalFunction;
                                evalFunction = math.compile(tf.Argument());
                                return (x) => { return evalFunction.eval({ x: x }); };
                        }
                    });
                });
            }
        });
        if (appModel.ActiveProject) {
            this.InitializeProject(appModel.ActiveProject);
        }
        // server callbacks
        ConnectionManager.Broadcaster.on("SendSlimOneDasSettings", oneDasSettingsModel => {
            this.SlimOneDasSettings(new SlimOneDasSettingsViewModel(oneDasSettingsModel));
        });
        ConnectionManager.Broadcaster.on("SendOneDasState", (oneDasState) => __awaiter(this, void 0, void 0, function* () {
            this.OneDasState(oneDasState);
            if (oneDasState == OneDasStateEnum.Error) {
                console.log("OneDAS: called");
                this.ActiveProject(null);
                try {
                    let lastError;
                    lastError = yield ConnectionManager.InvokeBroadcaster("GetLastError");
                    this.LastError(lastError);
                }
                catch (e) {
                    alert(e.message);
                }
            }
        }));
        ConnectionManager.Broadcaster.on("SendActiveProject", projectModel => {
            this.InitializeProject(projectModel);
        });
        ConnectionManager.Broadcaster.on("SendPerformanceInformation", performanceInformationModel => {
            this.PerformanceInformation(new OneDasPerformanceInformationViewModel(performanceInformationModel));
        });
        ConnectionManager.Broadcaster.on("SendDataSnapshot", (dateTime, dataSnapshot) => {
            if (this.ActiveProject()) {
                this.ActiveProject().DataSnapshot(dataSnapshot);
            }
        });
        ConnectionManager.Broadcaster.on("SendClientMessage", clientMessage => {
            this.ClientMessageLog.push(new MessageLogEntryViewModel(new Date().toLocaleTimeString('de-DE', {
                hour12: false,
                hour: "numeric",
                minute: "numeric",
                second: "numeric"
            }), clientMessage));
            if (this.ClientMessageLog().length > 10) {
                this.ClientMessageLog.shift();
            }
        });
    }
}
let appViewModel = ko.observable();
let componentLoader;
// javascript
//window.onbeforeunload = function () {
//    return "Are you sure to close or reload the page? All unsaved changes will be lost.";
//}
// pager workaround
ko.bindingHandlers.page = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var page = null;
        if (ko.utils.unwrapObservable(valueAccessor()) instanceof pager.Page) {
            page = ko.utils.unwrapObservable(valueAccessor());
            page.element = element;
            if (page.allBindingsAccessor == null) {
                page.allBindingsAccessor = allBindingsAccessor;
            }
            if (page.viewModel == null) {
                page.viewModel = viewModel;
            }
            if (page.bindingContext == null) {
                page.bindingContext = bindingContext;
            }
        }
        else {
            page = new pager.Page(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);
        }
        var result = page.init();
        if (page.isStartPage() && page.parentPage.isVisible() && (!page.parentPage.route || page.parentPage.route.length === 0 || page.parentPage.route[0] === '')) {
            setTimeout(function () {
                page.parentPage.showPage('');
            }, 0);
        }
        return result;
    }
};
// jQuery
$(document).ready(() => {
    $("body").tooltip({ selector: '[data-toggle=tooltip]', container: "body" });
    $("body").popover({ selector: "[data-toggle=popover]", container: "body", trigger: "focus" });
    $("body").click(() => {
        $("#Project_ChannelContextMenu").hide();
    });
});
console.log("OneDAS: bootstrap configured");
// signalr
ConnectionManager.Initialize(false);
PluginHive.Initialize();
ConnectionManager.Broadcaster.start().then(() => __awaiter(this, void 0, void 0, function* () {
    let appModel;
    console.log("OneDAS: signalr connected");
    try {
        appModel = yield ConnectionManager.InvokeBroadcaster("GetAppModel");
        appViewModel(new AppViewModel(appModel));
        console.log("OneDAS: app model received");
        // pager
        pager.extendWithPage(appViewModel());
        pager.Href5.hash = "";
        pager.useHTML5history = true;
        pager.Href5.history = History;
        console.log("OneDAS: pager configured");
        // knockout
        componentLoader =
            {
                loadTemplate: (name, templateConfig, callback) => {
                    let pluginIdentification;
                    if (templateConfig.PluginType && templateConfig.PluginIdentification) {
                        pluginIdentification = templateConfig.PluginIdentification;
                        ConnectionManager.InvokeBroadcaster("GetPluginStringResource", pluginIdentification.Id, pluginIdentification.ViewResourceName).then(pluginView => {
                            let element;
                            if (templateConfig.PluginType === "DataGateway" || templateConfig.PluginType === "DataWriter") {
                                element = document.createElement("div");
                                element.innerHTML = document.querySelector("#Project_PluginTemplate_" + templateConfig.PluginType).innerHTML;
                                element.querySelector("#Project_PluginTemplate_Content").innerHTML = pluginView;
                                callback([element]);
                            }
                            else {
                                ko.components.defaultLoader.loadTemplate(name, pluginView, callback);
                            }
                        }).catch(() => {
                            callback(null);
                        });
                    }
                    else {
                        callback(null);
                    }
                }
            };
        ko.components.loaders.unshift(componentLoader);
        ko.selectExtensions.readValue = (element) => // for "value" binding in combination with "type=number"
         {
            var anyElement;
            var hasDomDataExpandoProperty = '__ko__hasDomDataOptionValue__';
            anyElement = element;
            switch (ko.utils.tagNameLower(element)) {
                case 'option':
                    if (element[hasDomDataExpandoProperty] === true)
                        return ko.utils.domData.get(element, ko.bindingHandlers.options.optionValueDomDataKey);
                    return anyElement.value;
                case 'select':
                    return anyElement.selectedIndex >= 0 ? ko.selectExtensions.readValue(anyElement.options[anyElement.selectedIndex]) : undefined;
                case 'input':
                    if (anyElement.getAttribute("type") === "number") {
                        return anyElement.valueAsNumber;
                    }
                    else {
                        return anyElement.value;
                    }
                default:
                    return anyElement.value;
            }
        };
        ko.bindingHandlers.fadeVisible = {
            init: function (element, valueAccessor) {
                let value = valueAccessor();
                $(element).hide();
            },
            update: function (element, valueAccessor) {
                var value = valueAccessor();
                ko.unwrap(value) ? $(element).fadeIn(750) : $(element).fadeOut(750);
            }
        };
        ko.bindingHandlers.callFunction = {
            init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
                let functionSet = ko.unwrap(valueAccessor());
                if (Array.isArray(functionSet)) {
                    functionSet.forEach(x => {
                        x(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);
                    });
                }
                else {
                    functionSet(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);
                }
            }
        };
        ko.bindingHandlers.toggleArrow = {
            init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
                $(element).on("click", function () {
                    $(".fa", this)
                        .toggleClass("fa-caret-right")
                        .toggleClass("fa-caret-down");
                });
            }
        };
        ko.bindingHandlers.dynamicTooltip = {
            update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
                $(element).attr('title', ko.unwrap(valueAccessor())).tooltip('_fixTitle');
            }
        };
        ko.bindingHandlers.tooltip = {
            init: function (element, valueAccessor) {
                var local = ko.utils.unwrapObservable(valueAccessor());
                var options = {};
                ko.utils.extend(options, ko.bindingHandlers.tooltip.options);
                ko.utils.extend(options, local);
                $(element).tooltip(options);
                ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                    $(element).tooltip("hide");
                });
            },
            update: function (element, valueAccessor) {
                var local = ko.utils.unwrapObservable(valueAccessor());
                var options = {};
                ko.utils.extend(options, ko.bindingHandlers.tooltip.options);
                ko.utils.extend(options, local);
                $(element).attr('data-original-title', options.title);
                $(element).tooltip(options);
            },
            options: {
                placement: "top",
                trigger: "hover"
            }
        };
        ko.bindingHandlers.numericText = {
            update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
                var value = ko.utils.unwrapObservable(valueAccessor());
                var precision = ko.utils.unwrapObservable(allBindingsAccessor().precision);
                var formattedValue = value.toFixed(precision);
                if (allBindingsAccessor().unit) {
                    ko.bindingHandlers.text.update(element, function () { return formattedValue + " " + allBindingsAccessor().unit; }, allBindingsAccessor, viewModel, bindingContext);
                }
                else {
                    ko.bindingHandlers.text.update(element, function () { return formattedValue; }, allBindingsAccessor, viewModel, bindingContext);
                }
            }
        };
        ko.applyBindings(appViewModel);
        console.log("OneDAS: ko configured");
        // pager
        pager.startHistoryJs();
        console.log("OneDAS: pager started");
    }
    catch (e) {
        alert(e.message);
    }
})).catch(() => {
    console.log("OneDAS: signalr connection failed");
});
