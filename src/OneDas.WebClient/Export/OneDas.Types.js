var DataDirectionEnum;
(function (DataDirectionEnum) {
    DataDirectionEnum[DataDirectionEnum["Input"] = 1] = "Input";
    DataDirectionEnum[DataDirectionEnum["Output"] = 2] = "Output";
})(DataDirectionEnum || (DataDirectionEnum = {}));
var EndiannessEnum;
(function (EndiannessEnum) {
    EndiannessEnum[EndiannessEnum["LittleEndian"] = 1] = "LittleEndian";
    EndiannessEnum[EndiannessEnum["BigEndian"] = 2] = "BigEndian";
})(EndiannessEnum || (EndiannessEnum = {}));
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
    OneDasStateEnum[OneDasStateEnum["Idle"] = 3] = "Idle";
    OneDasStateEnum[OneDasStateEnum["ApplyConfiguration"] = 4] = "ApplyConfiguration";
    OneDasStateEnum[OneDasStateEnum["Ready"] = 5] = "Ready";
    OneDasStateEnum[OneDasStateEnum["Run"] = 6] = "Run";
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
var OneDasModuleSelectorModeEnum;
(function (OneDasModuleSelectorModeEnum) {
    OneDasModuleSelectorModeEnum[OneDasModuleSelectorModeEnum["Duplex"] = 1] = "Duplex";
    OneDasModuleSelectorModeEnum[OneDasModuleSelectorModeEnum["InputOnly"] = 2] = "InputOnly";
    OneDasModuleSelectorModeEnum[OneDasModuleSelectorModeEnum["OutputOnly"] = 3] = "OutputOnly";
})(OneDasModuleSelectorModeEnum || (OneDasModuleSelectorModeEnum = {}));
class BufferRequestModel {
    constructor(sampleRate, groupFilter) {
        this.SampleRate = sampleRate;
        this.GroupFilter = groupFilter;
    }
}
class ChannelHubModel {
    constructor(name, group, dataType) {
        this.Name = name;
        this.Group = group;
        this.DataType = dataType;
        this.Guid = Guid.NewGuid();
        this.CreationDateTime = new Date().toISOString();
        this.Unit = "";
        this.TransferFunctionSet = [];
        this.AssociatedDataInputId = "";
        this.AssociatedDataOutputIdSet = [];
    }
}
class OneDasModuleModel {
    constructor(dataType, dataDirection, endianness, size) {
        this.DataType = dataType;
        this.DataDirection = dataDirection;
        this.Endianness = endianness;
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
        ConnectionManager.WebClientHub = new signalR.HubConnection('/webclienthub');
    }
}
ConnectionManager.InvokeWebClientHub = (methodName, ...args) => __awaiter(this, void 0, void 0, function* () {
    return yield Promise.resolve(ConnectionManager.WebClientHub.invoke(methodName, ...args));
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
ErrorMessage["Project_DetachedExclamationMarkNotAllowed"] = "A detached exclamation mark is not allowed.";
ErrorMessage["Project_GroupFilterEmpty"] = "The group filter must not be empty.";
ErrorMessage["Project_IsAlreadyInGroup"] = "The channel is already a member of this group.";
ErrorMessage["Project_InvalidCharacters"] = "Use A-Z, a-z, 0-9 or _.";
ErrorMessage["Project_InvalidLeadingCharacter"] = "Use A-Z or a-z as first character.";
ErrorMessage["Project_NameEmpty"] = "The name must not be empty.";
class ObservableGroup {
    constructor(key, members = new Array()) {
        this.Key = key;
        this.Members = ko.observableArray(members);
    }
}
function ObservableGroupBy(list, nameGetter, groupNameGetter, filter) {
    let result;
    let regExp;
    result = [];
    regExp = new RegExp(filter, "i");
    list.forEach(element => {
        if (regExp.test(nameGetter(element))) {
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
function delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
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
        pluginViewModelRaw = yield ConnectionManager.InvokeWebClientHub("GetPluginStringResource", pluginModel.Description.Id, pluginIdentification.ViewModelResourceName);
        pluginViewModel = new Function(pluginViewModelRaw + "; return ViewModelConstructor")()(pluginModel, pluginIdentification);
        return pluginViewModel;
    }
    else {
        throw new Error("No corresponding plugin description for plugin ID '" + pluginModel.Description.Id + "' found.");
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
        this.DataTypeSet = ko.observableArray(EnumerationHelper.GetEnumValues('OneDasDataTypeEnum'));
        this.DataType = ko.observable(channelHubModel.DataType);
        this.Guid = channelHubModel.Guid;
        this.CreationDateTime = channelHubModel.CreationDateTime;
        this.Unit = ko.observable(channelHubModel.Unit);
        this.TransferFunctionSet = ko.observableArray(channelHubModel.TransferFunctionSet.map(tf => new TransferFunctionViewModel(tf)));
        this.SelectedTransferFunction = ko.observable(this.CreateDefaultTransferFunction());
        this.IsSelected = ko.observable(false);
        this.AssociatedDataInput = ko.observable();
        this.AssociatedDataOutputSet = ko.observableArray();
        this.AssociatedDataInputId = channelHubModel.AssociatedDataInputId;
        this.AssociatedDataOutputIdSet = channelHubModel.AssociatedDataOutputIdSet;
        this.EvaluatedTransferFunctionSet = [];
    }
    IsAssociationAllowed(dataPort) {
        return (dataPort.DataType & 0xff) == (this.DataType() & 0xff);
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
            DataType: this.DataType(),
            Guid: this.Guid,
            CreationDateTime: this.CreationDateTime,
            Unit: this.Unit(),
            TransferFunctionSet: this.TransferFunctionSet().map(tf => tf.ToModel()),
            AssociatedDataInputId: this.AssociatedDataInputId,
            AssociatedDataOutputIdSet: this.AssociatedDataOutputIdSet
        };
    }
}
class OneDasModuleViewModel {
    constructor(oneDasModuleModel) {
        this.OnPropertyChanged = () => {
            this.Validate();
            this._onPropertyChanged.dispatch(this, null);
        };
        this.GetByteCount = (booleanBitSize) => {
            if (booleanBitSize && this.DataType() === OneDasDataTypeEnum.BOOLEAN) {
                booleanBitSize = parseInt(booleanBitSize);
                return Math.ceil(booleanBitSize * this.Size() / 8);
            }
            else {
                return (this.DataType() & 0x0FF) / 8 * this.Size();
            }
        };
        this._model = oneDasModuleModel;
        this.DataTypeSet = ko.observableArray(EnumerationHelper.GetEnumValues('OneDasDataTypeEnum').filter(dataType => dataType !== OneDasDataTypeEnum.BOOLEAN));
        this.DataType = ko.observable(oneDasModuleModel.DataType);
        this.DataDirection = ko.observable(oneDasModuleModel.DataDirection);
        this.Endianness = ko.observable(oneDasModuleModel.Endianness);
        this.Size = ko.observable(oneDasModuleModel.Size);
        this.MaxSize = ko.observable(Infinity);
        this.ErrorMessage = ko.observable("");
        this.HasError = ko.computed(() => this.ErrorMessage().length > 0);
        this._onPropertyChanged = new EventDispatcher();
        this.DataType.subscribe(newValue => this.OnPropertyChanged());
        this.DataDirection.subscribe(newValue => this.OnPropertyChanged());
        this.Size.subscribe(newValue => this.OnPropertyChanged());
    }
    get PropertyChanged() {
        return this._onPropertyChanged;
    }
    Validate() {
        this.ErrorMessage("");
        if (this.Size() < 1 || (isFinite(this.MaxSize()) && this.Size() > this.MaxSize())) {
            this.ErrorMessage("Size must be within range 1.." + this.MaxSize() + ".");
        }
    }
    ToString() {
        return this.Size() + "x " + EnumerationHelper.GetEnumLocalization('OneDasDataTypeEnum', this.DataType());
    }
    ExtendModel(model) {
        //
    }
    ToModel() {
        let model = {
            $type: this._model.$type,
            DataType: this.DataType(),
            Size: this.Size(),
            DataDirection: this.DataDirection(),
            Endianness: this.Endianness()
        };
        this.ExtendModel(model);
        return model;
    }
}
class OneDasModuleSelectorViewModel {
    constructor(oneDasModuleSelectorMode, moduleSet = []) {
        // methods
        this.SetMaxBytes = (value) => {
            this.MaxBytes(value);
            this.InternalUpdate();
        };
        this.GetInputModuleSet = () => {
            return this.ModuleSet().filter(module => module.DataDirection() === DataDirectionEnum.Input);
        };
        this.GetOutputModuleSet = () => {
            return this.ModuleSet().filter(module => module.DataDirection() === DataDirectionEnum.Output);
        };
        this.OnModulePropertyChanged = () => {
            this.InternalUpdate();
        };
        // commands
        this.AddModule = () => {
            let newModule;
            if (!this.HasError()) {
                this.ModuleSet.push(this.NewModule());
                this.InternalCreateNewModule();
                this.InternalUpdate();
                this._onModuleSetChanged.dispatch(this, this.ModuleSet());
            }
        };
        this.DeleteModule = () => {
            this.ModuleSet.pop();
            this.InternalUpdate();
            this._onModuleSetChanged.dispatch(this, this.ModuleSet());
        };
        this.OneDasModuleSelectorMode = ko.observable(oneDasModuleSelectorMode);
        this.SettingsTemplateName = ko.observable("Project_OneDasModuleTemplate");
        this.NewModule = ko.observable();
        this.MaxBytes = ko.observable(Infinity);
        this.RemainingBytes = ko.observable(NaN);
        this.RemainingCount = ko.observable(NaN);
        this.ModuleSet = ko.observableArray(moduleSet);
        this.ErrorMessage = ko.observable("");
        this.HasError = ko.computed(() => this.ErrorMessage().length > 0);
        this._onModuleSetChanged = new EventDispatcher();
        this.InternalCreateNewModule();
        this.InternalUpdate();
    }
    get OnModuleSetChanged() {
        return this._onModuleSetChanged;
    }
    InternalUpdate() {
        this.Update();
        this.Validate();
    }
    Update() {
        let moduleSet;
        let remainingBytes;
        switch (this.NewModule().DataDirection()) {
            case DataDirectionEnum.Input:
                moduleSet = this.GetInputModuleSet();
                break;
            case DataDirectionEnum.Output:
                moduleSet = this.GetOutputModuleSet();
                break;
        }
        remainingBytes = this.MaxBytes() - moduleSet.map(oneDasModule => oneDasModule.GetByteCount()).reduce((previousValue, currentValue) => previousValue + currentValue, 0);
        this.RemainingBytes(remainingBytes);
        this.RemainingCount(Math.floor(this.RemainingBytes() / ((this.NewModule().DataType() & 0x0FF) / 8)));
    }
    Validate() {
        this.ErrorMessage("");
        if (this.NewModule().HasError()) {
            this.ErrorMessage("Resolve all remaining module errors before continuing.");
        }
        if (this.OneDasModuleSelectorMode() === OneDasModuleSelectorModeEnum.InputOnly && this.NewModule().DataDirection() == DataDirectionEnum.Output) {
            this.ErrorMessage("Only input modules are allowed.");
        }
        if (this.OneDasModuleSelectorMode() === OneDasModuleSelectorModeEnum.OutputOnly && this.NewModule().DataDirection() == DataDirectionEnum.Input) {
            this.ErrorMessage("Only output modules are allowed.");
        }
        if (isFinite(this.RemainingBytes()) && (this.RemainingBytes() - this.NewModule().GetByteCount() < 0)) {
            this.ErrorMessage("Byte count of new module is too high.");
        }
        if (this.RemainingCount() <= 0) {
            this.ErrorMessage("The maximum number of modules is reached.");
        }
    }
    CreateNewModule() {
        if (this.NewModule()) {
            return new OneDasModuleViewModel(new OneDasModuleModel(this.NewModule().DataType(), this.NewModule().DataDirection(), this.NewModule().Endianness(), 1));
        }
        else {
            return new OneDasModuleViewModel(new OneDasModuleModel(OneDasDataTypeEnum.UINT16, DataDirectionEnum.Input, EndiannessEnum.LittleEndian, 1));
        }
    }
    InternalCreateNewModule() {
        if (this.NewModule()) {
            this.NewModule().PropertyChanged.unsubscribe(this.OnModulePropertyChanged);
        }
        this.NewModule(this.CreateNewModule());
        this.NewModule().PropertyChanged.subscribe(this.OnModulePropertyChanged);
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
class BufferRequestSelectorViewModel {
    constructor(bufferRequestSet = []) {
        this.OnBufferRequestPropertyChanged = () => {
            this.InternalUpdate();
        };
        // commands
        this.AddBufferRequest = () => {
            let newBufferRequest;
            if (!this.HasError()) {
                this.BufferRequestSet.push(this.NewBufferRequest());
                this.InternalCreateNewBufferRequest();
                this.InternalUpdate();
                this._onBufferRequestSetChanged.dispatch(this, this.BufferRequestSet());
            }
        };
        this.DeleteBufferRequest = () => {
            this.BufferRequestSet.pop();
            this.InternalUpdate();
            this._onBufferRequestSetChanged.dispatch(this, this.BufferRequestSet());
        };
        this.NewBufferRequest = ko.observable();
        this.BufferRequestSet = ko.observableArray(bufferRequestSet);
        this.ErrorMessage = ko.observable("");
        this.HasError = ko.computed(() => this.ErrorMessage().length > 0);
        this._onBufferRequestSetChanged = new EventDispatcher();
        this.InternalCreateNewBufferRequest();
        this.InternalUpdate();
    }
    get OnBufferRequestSetChanged() {
        return this._onBufferRequestSetChanged;
    }
    // methods
    InternalUpdate() {
        this.Update();
        this.Validate();
    }
    Update() {
        //
    }
    Validate() {
        this.ErrorMessage("");
        if (this.NewBufferRequest().HasError()) {
            this.ErrorMessage("Resolve all remaining errors before continuing.");
        }
    }
    CreateNewBufferRequest() {
        if (this.NewBufferRequest()) {
            return new BufferRequestViewModel(new BufferRequestModel(this.NewBufferRequest().SampleRate(), this.NewBufferRequest().GroupFilter()));
        }
        else {
            return new BufferRequestViewModel(new BufferRequestModel(SampleRateEnum.SampleRate_1, "*"));
        }
    }
    InternalCreateNewBufferRequest() {
        if (this.NewBufferRequest()) {
            this.NewBufferRequest().PropertyChanged.unsubscribe(this.OnBufferRequestPropertyChanged);
        }
        this.NewBufferRequest(this.CreateNewBufferRequest());
        this.NewBufferRequest().PropertyChanged.subscribe(this.OnBufferRequestPropertyChanged);
    }
}
class BufferRequestViewModel {
    constructor(model) {
        this.OnPropertyChanged = () => {
            this.Validate();
            this._onPropertyChanged.dispatch(this, null);
        };
        this.SampleRateSet = ko.observableArray(EnumerationHelper.GetEnumValues("SampleRateEnum"));
        this.SampleRate = ko.observable(model.SampleRate);
        this.GroupFilter = ko.observable(model.GroupFilter);
        this.ErrorMessage = ko.observable("");
        this.HasError = ko.computed(() => this.ErrorMessage().length > 0);
        this._onPropertyChanged = new EventDispatcher();
        this.SampleRate.subscribe(newValue => this.OnPropertyChanged());
        this.GroupFilter.subscribe(newValue => this.OnPropertyChanged());
    }
    get PropertyChanged() {
        return this._onPropertyChanged;
    }
    Validate() {
        let result;
        this.ErrorMessage("");
        this.GroupFilter().split(";").forEach(groupFilter => {
            result = this.CheckGroupFilter(groupFilter);
            if (result.HasError) {
                this.ErrorMessage(result.ErrorDescription);
                return;
            }
        });
    }
    ToString() {
        return "sample rate: " + EnumerationHelper.GetEnumLocalization("SampleRateEnum", this.SampleRate()) + " / group filter: '" + this.GroupFilter() + "'";
    }
    ToModel() {
        let model = {
            SampleRate: this.SampleRate(),
            GroupFilter: this.GroupFilter()
        };
        return model;
    }
    CheckGroupFilter(value) {
        var regExp;
        if (value.length === 0) {
            return { HasError: true, ErrorDescription: ErrorMessage["Project_GroupFilterEmpty"] };
        }
        regExp = new RegExp("[^A-Za-z0-9_!*]");
        if (regExp.test(value)) {
            return { HasError: true, ErrorDescription: ErrorMessage["Project_InvalidCharacters"] };
        }
        regExp = new RegExp("^[0-9_]");
        if (regExp.test(value)) {
            return { HasError: true, ErrorDescription: ErrorMessage["Project_InvalidLeadingCharacter"] };
        }
        regExp = new RegExp("^!");
        if (regExp.test(value) && value.length === 1) {
            return { HasError: true, ErrorDescription: ErrorMessage["Project_DetachedExclamationMarkNotAllowed"] };
        }
        return {
            HasError: false,
            ErrorDescription: ""
        };
    }
}
class DataPortViewModel {
    // constructors
    constructor(dataPortModel, associatedDataGateway) {
        this.Name = ko.observable(dataPortModel.Name);
        this.DataType = dataPortModel.DataType;
        this.DataDirection = dataPortModel.DataDirection;
        this.Endianness = dataPortModel.Endianness;
        this.IsSelected = ko.observable(false);
        this.AssociatedChannelHubSet = ko.observableArray();
        this.AssociatedDataGateway = associatedDataGateway;
        this.LiveDescription = ko.computed(() => {
            let result;
            result = "<div class='text-left'>" + this.Name() + "</div><div class='text-left'>" + EnumerationHelper.GetEnumLocalization('OneDasDataTypeEnum', this.DataType) + "</div>";
            if (this.AssociatedChannelHubSet().length > 0) {
                this.AssociatedChannelHubSet().forEach(channelHub => {
                    result += "</br ><div class='text-left'>" + channelHub.Name() + "</div><div class='text-left'>" + EnumerationHelper.GetEnumLocalization('OneDasDataTypeEnum', channelHub.DataType()) + "</div>";
                });
            }
            return result;
        });
    }
    // methods
    GetId() {
        return this.Name();
    }
    ToFullQualifiedIdentifier() {
        return this.AssociatedDataGateway.Description.Id + " (" + this.AssociatedDataGateway.Description.InstanceId + ") / " + this.GetId();
    }
    ExtendModel(model) {
        //
    }
    ToModel() {
        let model = {
            Name: this.Name(),
            DataType: this.DataType,
            DataDirection: this.DataDirection
        };
        this.ExtendModel(model);
        return model;
    }
    ResetAssociations(maintainWeakReference) {
        if (this.AssociatedChannelHubSet().length > 0) {
            this.AssociatedChannelHubSet().forEach(channelHub => channelHub.ResetAssociation(maintainWeakReference, this));
        }
    }
}
class PluginViewModelBase {
    constructor(pluginSettingsModel, pluginIdentification) {
        this.SendActionRequest = (instanceId, methodName, data) => __awaiter(this, void 0, void 0, function* () {
            return yield ConnectionManager.InvokeWebClientHub("RequestAction", new ActionRequest(this.Description.Id, instanceId, methodName, data));
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
        this._model = pluginSettingsModel;
        this.Description = new PluginDescriptionViewModel(pluginSettingsModel.Description);
        this.PluginIdentification = pluginIdentification;
        this.IsInSettingsMode = ko.observable(false);
    }
    ExtendModel(model) {
        //
    }
    ToModel() {
        let model = {
            $type: this._model.$type,
            Description: this.Description.ToModel()
        };
        this.ExtendModel(model);
        return model;
    }
}
/// <reference path="PluginViewModelBase.ts"/>
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
class ExtendedDataGatewayViewModelBase extends DataGatewayViewModelBase {
    constructor(model, identification, oneDasModuleSelector) {
        super(model, identification);
        this.ModuleToDataPortMap = ko.observableArray();
        this.OneDasModuleSelector = ko.observable(oneDasModuleSelector);
        if (this.OneDasModuleSelector()) {
            this.OneDasModuleSelector().OnModuleSetChanged.subscribe((sender, args) => {
                this.UpdateDataPortSet();
            });
        }
    }
    InitializeAsync() {
        return __awaiter(this, void 0, void 0, function* () {
            this.UpdateDataPortSet();
        });
    }
    UpdateDataPortSet() {
        let index;
        let moduleToDataPortMap;
        moduleToDataPortMap = [];
        // inputs
        index = 0;
        moduleToDataPortMap = moduleToDataPortMap.concat(this.OneDasModuleSelector().ModuleSet().filter(oneDasModule => oneDasModule.DataDirection() === DataDirectionEnum.Input).map(oneDasModule => {
            let group;
            group = new ObservableGroup(oneDasModule.ToString(), this.CreateDataPortSet(oneDasModule, index));
            index += oneDasModule.Size();
            return group;
        }));
        // outputs
        index = 0;
        moduleToDataPortMap = moduleToDataPortMap.concat(this.OneDasModuleSelector().ModuleSet().filter(oneDasModule => oneDasModule.DataDirection() === DataDirectionEnum.Output).map(oneDasModule => {
            let group;
            group = new ObservableGroup(oneDasModule.ToString(), this.CreateDataPortSet(oneDasModule, index));
            index += oneDasModule.Size();
            return group;
        }));
        this.ModuleToDataPortMap(moduleToDataPortMap);
        this.DataPortSet(MapMany(this.ModuleToDataPortMap(), group => group.Members()));
    }
    CreateDataPortSet(oneDasModule, index) {
        let prefix;
        switch (oneDasModule.DataDirection()) {
            case DataDirectionEnum.Input:
                prefix = "Input";
                break;
            case DataDirectionEnum.Output:
                prefix = "Output";
                break;
        }
        return Array.from(new Array(oneDasModule.Size()), (x, i) => {
            return {
                Name: prefix + " " + (index + i),
                DataType: oneDasModule.DataType(),
                DataDirection: oneDasModule.DataDirection()
            };
        }).map(dataPortModel => new DataPortViewModel(dataPortModel, this));
    }
}
/// <reference path="PluginViewModelBase.ts"/>
class DataWriterViewModelBase extends PluginViewModelBase {
    constructor(model, identification) {
        super(model, identification);
        this.FileGranularity = ko.observable(model.FileGranularity);
        this.BufferRequestSet = ko.observableArray(model.BufferRequestSet.map(bufferRequest => new BufferRequestViewModel(bufferRequest)));
        this.BufferRequestSelector = ko.observable(new BufferRequestSelectorViewModel(this.BufferRequestSet()));
    }
    ExtendModel(model) {
        super.ExtendModel(model);
        model.FileGranularity = this.FileGranularity();
        model.BufferRequestSet = this.BufferRequestSet().map(bufferRequest => bufferRequest.ToModel());
    }
}
class PluginDescriptionViewModel {
    constructor(pluginDescriptionModel) {
        this.ProductVersion = pluginDescriptionModel.ProductVersion;
        this.Id = pluginDescriptionModel.Id;
        this.InstanceId = pluginDescriptionModel.InstanceId;
        this.InstanceName = ko.observable(pluginDescriptionModel.InstanceName);
        this.IsEnabled = ko.observable(pluginDescriptionModel.IsEnabled);
    }
    ToModel() {
        var model = {
            ProductVersion: this.ProductVersion,
            Id: this.Id,
            InstanceId: this.InstanceId,
            InstanceName: this.InstanceName(),
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
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiT25lRGFzLlR5cGVzLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vT25lRGFzLlR5cGVzL0RhdGFEaXJlY3Rpb25FbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL0VuZGlhbm5lc3NFbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL0ZpbGVHcmFudWxhcml0eUVudW0udHMiLCIuLi9PbmVEYXMuVHlwZXMvT25lRGFzRGF0YVR5cGVFbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL09uZURhc1N0YXRlRW51bS50cyIsIi4uL09uZURhcy5UeXBlcy9TYW1wbGVSYXRlRW51bS50cyIsIi4uL0NvcmUvQWN0aW9uUmVxdWVzdC50cyIsIi4uL0NvcmUvQWN0aW9uUmVzcG9uc2UudHMiLCIuLi9Db3JlL0V2ZW50RGlzcGF0Y2hlci50cyIsIi4uL0NvcmUvSUV2ZW50LnRzIiwiLi4vQ29yZS9PbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtLnRzIiwiLi4vTW9kZWxzL0J1ZmZlclJlcXVlc3RNb2RlbC50cyIsIi4uL01vZGVscy9DaGFubmVsSHViTW9kZWwudHMiLCIuLi9Nb2RlbHMvT25lRGFzTW9kdWxlTW9kZWwudHMiLCIuLi9Nb2RlbHMvVHJhbnNmZXJGdW5jdGlvbk1vZGVsLnRzIiwiLi4vU3RhdGljL0Nvbm5lY3Rpb25NYW5hZ2VyLnRzIiwiLi4vU3RhdGljL0VudW1lcmF0aW9uSGVscGVyLnRzIiwiLi4vU3RhdGljL0Vycm9yTWVzc2FnZS50cyIsIi4uL1N0YXRpYy9IZWxwZXIudHMiLCIuLi9TdGF0aWMvUGx1Z2luRmFjdG9yeS50cyIsIi4uL1N0YXRpYy9QbHVnaW5IaXZlLnRzIiwiLi4vVmlld01vZGVscy9Db3JlL0NoYW5uZWxIdWJWaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL0NvcmUvT25lRGFzTW9kdWxlVmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9Db3JlL09uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9Db3JlL1RyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL1BsdWdpbi9CdWZmZXJSZXF1ZXN0U2VsZWN0b3JWaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL1BsdWdpbi9CdWZmZXJSZXF1ZXN0Vmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vRGF0YVBvcnRWaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL1BsdWdpbi9QbHVnaW5WaWV3TW9kZWxCYXNlLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vRXh0ZW5kZWREYXRhR2F0ZXdheVZpZXdNb2RlbEJhc2UudHMiLCIuLi9WaWV3TW9kZWxzL1BsdWdpbi9EYXRhV3JpdGVyVmlld01vZGVsQmFzZS50cyIsIi4uL1ZpZXdNb2RlbHMvUGx1Z2luL1BsdWdpbkRlc2NyaXB0aW9uVmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vUGx1Z2luSWRlbnRpZmljYXRpb25WaWV3TW9kZWwudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsSUFBSyxpQkFJSjtBQUpELFdBQUssaUJBQWlCO0lBRWxCLDJEQUFTLENBQUE7SUFDVCw2REFBVSxDQUFBO0FBQ2QsQ0FBQyxFQUpJLGlCQUFpQixLQUFqQixpQkFBaUIsUUFJckI7QUNKRCxJQUFLLGNBSUo7QUFKRCxXQUFLLGNBQWM7SUFFZixtRUFBZ0IsQ0FBQTtJQUNoQiw2REFBYSxDQUFBO0FBQ2pCLENBQUMsRUFKSSxjQUFjLEtBQWQsY0FBYyxRQUlsQjtBQ0pELElBQUssbUJBTUo7QUFORCxXQUFLLG1CQUFtQjtJQUVwQixzRUFBYSxDQUFBO0lBQ2IseUVBQWUsQ0FBQTtJQUNmLGdFQUFXLENBQUE7SUFDWCwrREFBVyxDQUFBO0FBQ2YsQ0FBQyxFQU5JLG1CQUFtQixLQUFuQixtQkFBbUIsUUFNdkI7QUNORCxJQUFLLGtCQVdKO0FBWEQsV0FB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QVcsQ0FBQTtRQUN4RCxJQUFJLENBQUMsZ0JBQWdCLEdBQUcseUJBQXlCLENBQUMsZ0JBQWdCLENBQUE7UUFDbEUsSUFBSSxDQUFDLHFCQUFxQixHQUFHLHlCQUF5QixDQUFDLHFCQUFxQixDQUFBO0lBQ2hGLENBQUM7Q0FDSiIsInNvdXJjZXNDb250ZW50IjpbImVudW0gRGF0YURpcmVjdGlvbkVudW1cclxue1xyXG4gICAgSW5wdXQgPSAxLFxyXG4gICAgT3V0cHV0ID0gMlxyXG59IiwiZW51bSBFbmRpYW5uZXNzRW51bVxyXG57XHJcbiAgICBMaXR0bGVFbmRpYW4gPSAxLFxyXG4gICAgQmlnRW5kaWFuID0gMlxyXG59IiwiZW51bSBGaWxlR3JhbnVsYXJpdHlFbnVtXHJcbntcclxuICAgIE1pbnV0ZV8xID0gNjAsXHJcbiAgICBNaW51dGVfMTAgPSA2MDAsXHJcbiAgICBIb3VyID0gMzYwMCxcclxuICAgIERheSA9IDg2NDAwXHJcbn0iLCJlbnVtIE9uZURhc0RhdGFUeXBlRW51bVxyXG57XHJcbiAgICBCT09MRUFOID0gMHgwMDgsXHJcbiAgICBVSU5UOCA9IDB4MTA4LFxyXG4gICAgSU5UOCA9IDB4MjA4LFxyXG4gICAgVUlOVDE2ID0gMHgxMTAsXHJcbiAgICBJTlQxNiA9IDB4MjEwLFxyXG4gICAgVUlOVDMyID0gMHgxMjAsXHJcbiAgICBJTlQzMiA9IDB4MjIwLFxyXG4gICAgRkxPQVQzMiA9IDB4MzIwLFxyXG4gICAgRkxPQVQ2NCA9IDB4MzQwXHJcbn0iLCJlbnVtIE9uZURhc1N0YXRlRW51bVxyXG57XHJcbiAgICBFcnJvciA9IDEsXHJcbiAgICBJbml0aWFsaXphdGlvbiA9IDIsXHJcbiAgICBJZGxlID0gMyxcclxuICAgIEFwcGx5Q29uZmlndXJhdGlvbiA9IDQsXHJcbiAgICBSZWFkeSA9IDUsXHJcbiAgICBSdW4gPSA2XHJcbn0iLCJlbnVtIFNhbXBsZVJhdGVFbnVtXHJcbntcclxuICAgIFNhbXBsZVJhdGVfMTAwID0gMSxcclxuICAgIFNhbXBsZVJhdGVfMjUgPSA0LFxyXG4gICAgU2FtcGxlUmF0ZV81ID0gMjAsXHJcbiAgICBTYW1wbGVSYXRlXzEgPSAxMDBcclxufSIsImNsYXNzIEFjdGlvblJlcXVlc3Rcclxue1xyXG4gICAgcHVibGljIHJlYWRvbmx5IFBsdWdpbklkOiBzdHJpbmdcclxuICAgIHB1YmxpYyByZWFkb25seSBJbnN0YW5jZUlkOiBudW1iZXJcclxuICAgIHB1YmxpYyByZWFkb25seSBNZXRob2ROYW1lOiBzdHJpbmdcclxuICAgIHB1YmxpYyByZWFkb25seSBEYXRhOiBhbnlcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihwbHVnaW5JZDogc3RyaW5nLCBpbnN0YW5jZUlkOiBudW1iZXIsIG1ldGhvZE5hbWU6IHN0cmluZywgZGF0YTogYW55KVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuUGx1Z2luSWQgPSBwbHVnaW5JZDtcclxuICAgICAgICB0aGlzLkluc3RhbmNlSWQgPSBpbnN0YW5jZUlkO1xyXG4gICAgICAgIHRoaXMuTWV0aG9kTmFtZSA9IG1ldGhvZE5hbWU7XHJcbiAgICAgICAgdGhpcy5EYXRhID0gZGF0YTtcclxuICAgIH1cclxufSIsImNsYXNzIEFjdGlvblJlc3BvbnNlXHJcbntcclxuICAgIHB1YmxpYyBEYXRhOiBhbnlcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihkYXRhOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5EYXRhID0gZGF0YTtcclxuICAgIH1cclxufSIsImNsYXNzIEV2ZW50RGlzcGF0Y2hlcjxUU2VuZGVyLCBUQXJncz4gaW1wbGVtZW50cyBJRXZlbnQ8VFNlbmRlciwgVEFyZ3M+XHJcbntcclxuICAgIHByaXZhdGUgX3N1YnNjcmlwdGlvbnM6IEFycmF5PChzZW5kZXI6IFRTZW5kZXIsIGFyZ3M6IFRBcmdzKSA9PiB2b2lkPiA9IG5ldyBBcnJheTwoc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncykgPT4gdm9pZD4oKTtcclxuXHJcbiAgICBzdWJzY3JpYmUoZm46IChzZW5kZXI6IFRTZW5kZXIsIGFyZ3M6IFRBcmdzKSA9PiB2b2lkKTogdm9pZFxyXG4gICAge1xyXG4gICAgICAgIGlmIChmbilcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuX3N1YnNjcmlwdGlvbnMucHVzaChmbik7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHVuc3Vic2NyaWJlKGZuOiAoc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncykgPT4gdm9pZCk6IHZvaWRcclxuICAgIHtcclxuICAgICAgICBsZXQgaSA9IHRoaXMuX3N1YnNjcmlwdGlvbnMuaW5kZXhPZihmbik7XHJcblxyXG4gICAgICAgIGlmIChpID4gLTEpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLl9zdWJzY3JpcHRpb25zLnNwbGljZShpLCAxKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgZGlzcGF0Y2goc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncyk6IHZvaWRcclxuICAgIHtcclxuICAgICAgICBmb3IgKGxldCBoYW5kbGVyIG9mIHRoaXMuX3N1YnNjcmlwdGlvbnMpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBoYW5kbGVyKHNlbmRlciwgYXJncyk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG59IiwiaW50ZXJmYWNlIElFdmVudDxUU2VuZGVyLCBUQXJncz5cclxue1xyXG4gICAgc3Vic2NyaWJlKGZuOiAoc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncykgPT4gdm9pZCk6IHZvaWQ7XHJcbiAgICB1bnN1YnNjcmliZShmbjogKHNlbmRlcjogVFNlbmRlciwgYXJnczogVEFyZ3MpID0+IHZvaWQpOiB2b2lkO1xyXG59IiwiZW51bSBPbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtXHJcbntcclxuICAgIER1cGxleCA9IDEsXHJcbiAgICBJbnB1dE9ubHkgPSAyLFxyXG4gICAgT3V0cHV0T25seSA9IDMsXHJcbn0iLCJjbGFzcyBCdWZmZXJSZXF1ZXN0TW9kZWxcclxue1xyXG4gICAgcHVibGljIFNhbXBsZVJhdGU6IFNhbXBsZVJhdGVFbnVtXHJcbiAgICBwdWJsaWMgR3JvdXBGaWx0ZXI6IHN0cmluZ1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKHNhbXBsZVJhdGU6IFNhbXBsZVJhdGVFbnVtLCBncm91cEZpbHRlcjogc3RyaW5nKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuU2FtcGxlUmF0ZSA9IHNhbXBsZVJhdGU7XHJcbiAgICAgICAgdGhpcy5Hcm91cEZpbHRlciA9IGdyb3VwRmlsdGVyO1xyXG4gICAgfVxyXG59IiwiY2xhc3MgQ2hhbm5lbEh1Yk1vZGVsXHJcbntcclxuICAgIHB1YmxpYyBOYW1lOiBzdHJpbmdcclxuICAgIHB1YmxpYyBHcm91cDogc3RyaW5nXHJcbiAgICBwdWJsaWMgRGF0YVR5cGU6IE9uZURhc0RhdGFUeXBlRW51bVxyXG4gICAgcHVibGljIEd1aWQ6IHN0cmluZ1xyXG4gICAgcHVibGljIENyZWF0aW9uRGF0ZVRpbWU6IHN0cmluZ1xyXG4gICAgcHVibGljIFVuaXQ6IHN0cmluZ1xyXG4gICAgcHVibGljIFRyYW5zZmVyRnVuY3Rpb25TZXQ6IGFueVtdXHJcbiAgICBwdWJsaWMgQXNzb2NpYXRlZERhdGFJbnB1dElkOiBzdHJpbmdcclxuICAgIHB1YmxpYyBBc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0OiBzdHJpbmdbXVxyXG5cclxuICAgIGNvbnN0cnVjdG9yKG5hbWU6IHN0cmluZywgZ3JvdXA6IHN0cmluZywgZGF0YVR5cGU6IE9uZURhc0RhdGFUeXBlRW51bSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLk5hbWUgPSBuYW1lO1xyXG4gICAgICAgIHRoaXMuR3JvdXAgPSBncm91cDtcclxuICAgICAgICB0aGlzLkRhdGFUeXBlID0gZGF0YVR5cGU7XHJcbiAgICAgICAgdGhpcy5HdWlkID0gR3VpZC5OZXdHdWlkKClcclxuICAgICAgICB0aGlzLkNyZWF0aW9uRGF0ZVRpbWUgPSBuZXcgRGF0ZSgpLnRvSVNPU3RyaW5nKClcclxuICAgICAgICB0aGlzLlVuaXQgPSBcIlwiXHJcbiAgICAgICAgdGhpcy5UcmFuc2ZlckZ1bmN0aW9uU2V0ID0gW11cclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXRJZCA9IFwiXCJcclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQgPSBbXVxyXG4gICAgfVxyXG59IiwiY2xhc3MgT25lRGFzTW9kdWxlTW9kZWxcclxue1xyXG4gICAgcHVibGljIERhdGFUeXBlOiBPbmVEYXNEYXRhVHlwZUVudW1cclxuICAgIHB1YmxpYyBEYXRhRGlyZWN0aW9uOiBEYXRhRGlyZWN0aW9uRW51bVxyXG4gICAgcHVibGljIEVuZGlhbm5lc3M6IEVuZGlhbm5lc3NFbnVtXHJcbiAgICBwdWJsaWMgU2l6ZTogbnVtYmVyXHJcblxyXG4gICAgY29uc3RydWN0b3IoZGF0YVR5cGU6IE9uZURhc0RhdGFUeXBlRW51bSwgZGF0YURpcmVjdGlvbjogRGF0YURpcmVjdGlvbkVudW0sIGVuZGlhbm5lc3M6IEVuZGlhbm5lc3NFbnVtLCBzaXplOiBudW1iZXIpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZSA9IGRhdGFUeXBlXHJcbiAgICAgICAgdGhpcy5EYXRhRGlyZWN0aW9uID0gZGF0YURpcmVjdGlvblxyXG4gICAgICAgIHRoaXMuRW5kaWFubmVzcyA9IGVuZGlhbm5lc3NcclxuICAgICAgICB0aGlzLlNpemUgPSBzaXplXHJcbiAgICB9XHJcbn1cclxuIiwiY2xhc3MgVHJhbnNmZXJGdW5jdGlvbk1vZGVsXHJcbntcclxuICAgIHB1YmxpYyBEYXRlVGltZTogc3RyaW5nXHJcbiAgICBwdWJsaWMgVHlwZTogc3RyaW5nXHJcbiAgICBwdWJsaWMgT3B0aW9uOiBzdHJpbmdcclxuICAgIHB1YmxpYyBBcmd1bWVudDogc3RyaW5nXHJcblxyXG4gICAgY29uc3RydWN0b3IoZGF0ZVRpbWU6IHN0cmluZywgdHlwZTogc3RyaW5nLCBvcHRpb246IHN0cmluZywgYXJndW1lbnQ6IHN0cmluZylcclxuICAgIHtcclxuICAgICAgICB0aGlzLkRhdGVUaW1lID0gZGF0ZVRpbWVcclxuICAgICAgICB0aGlzLlR5cGUgPSB0eXBlXHJcbiAgICAgICAgdGhpcy5PcHRpb24gPSBvcHRpb25cclxuICAgICAgICB0aGlzLkFyZ3VtZW50ID0gYXJndW1lbnRcclxuICAgIH1cclxufSIsImRlY2xhcmUgdmFyIHNpZ25hbFI6IGFueVxyXG5cclxuY2xhc3MgQ29ubmVjdGlvbk1hbmFnZXJcclxue1xyXG4gICAgcHVibGljIHN0YXRpYyBXZWJDbGllbnRIdWI6IGFueSAvLyBpbXByb3ZlOiB1c2UgdHlwaW5nc1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgSW5pdGlhbGl6ZShlbmFibGVMb2dnaW5nOiBib29sZWFuKVxyXG4gICAge1xyXG4gICAgICAgIENvbm5lY3Rpb25NYW5hZ2VyLldlYkNsaWVudEh1YiA9IG5ldyBzaWduYWxSLkh1YkNvbm5lY3Rpb24oJy93ZWJjbGllbnRodWInKTtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIEludm9rZVdlYkNsaWVudEh1YiA9IGFzeW5jKG1ldGhvZE5hbWU6IHN0cmluZywgLi4uYXJnczogYW55W10pID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIGF3YWl0IFByb21pc2UucmVzb2x2ZShDb25uZWN0aW9uTWFuYWdlci5XZWJDbGllbnRIdWIuaW52b2tlKG1ldGhvZE5hbWUsIC4uLmFyZ3MpKVxyXG4gICAgfVxyXG59XHJcbiIsImNsYXNzIEVudW1lcmF0aW9uSGVscGVyXHJcbntcclxuICAgIHB1YmxpYyBzdGF0aWMgRGVzY3JpcHRpb246IHsgW2luZGV4OiBzdHJpbmddOiBzdHJpbmcgfSA9IHt9XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBHZXRFbnVtTG9jYWxpemF0aW9uID0gKHR5cGVOYW1lOiBzdHJpbmcsIHZhbHVlKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHZhciBrZXk6IHN0cmluZyA9IGV2YWwodHlwZU5hbWUgKyBcIltcIiArIHZhbHVlICsgXCJdXCIpXHJcbiAgICAgICAgcmV0dXJuIGV2YWwoXCJFbnVtZXJhdGlvbkhlbHBlci5EZXNjcmlwdGlvblsnXCIgKyB0eXBlTmFtZSArIFwiX1wiICsga2V5ICsgXCInXVwiKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgR2V0RW51bVZhbHVlcyA9ICh0eXBlTmFtZTogc3RyaW5nKSA9PlxyXG4gICAge1xyXG4gICAgICAgIGxldCB2YWx1ZXM6IGFueVtdXHJcblxyXG4gICAgICAgIHZhbHVlcyA9IGV2YWwoXCJPYmplY3Qua2V5cyhcIiArIHR5cGVOYW1lICsgXCIpLm1hcChrZXkgPT4gXCIgKyB0eXBlTmFtZSArIFwiW2tleV0pXCIpXHJcbiAgICAgICAgcmV0dXJuIDxudW1iZXJbXT52YWx1ZXMuZmlsdGVyKHZhbHVlID0+IHR5cGVvZiAodmFsdWUpID09PSBcIm51bWJlclwiKVxyXG4gICAgfVxyXG59IiwibGV0IEVycm9yTWVzc2FnZTogeyBbaW5kZXg6IHN0cmluZ106IHN0cmluZyB9ID0ge31cclxuRXJyb3JNZXNzYWdlW1wiTXVsdGlNYXBwaW5nRWRpdG9yVmlld01vZGVsX0ludmFsaWRTZXR0aW5nc1wiXSA9IFwiT25lIG9yIG1vcmUgc2V0dGluZ3MgYXJlIGludmFsaWQuXCJcclxuRXJyb3JNZXNzYWdlW1wiTXVsdGlNYXBwaW5nRWRpdG9yVmlld01vZGVsX1dyb25nRGF0YVR5cGVcIl0gPSBcIk9uZSBvciBtb3JlIHZhcmlhYmxlLWNoYW5uZWwgZGF0YSB0eXBlIGNvbWJpbmF0aW9ucyBhcmUgaW52YWxpZC5cIlxyXG5FcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0NoYW5uZWxBbHJlYWR5RXhpc3RzXCJdID0gXCJBIGNoYW5uZWwgd2l0aCB0aGF0IG5hbWUgYWxyZWFkeSBleGlzdHMuXCJcclxuRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9EZXRhY2hlZEV4Y2xhbWF0aW9uTWFya05vdEFsbG93ZWRcIl0gPSBcIkEgZGV0YWNoZWQgZXhjbGFtYXRpb24gbWFyayBpcyBub3QgYWxsb3dlZC5cIlxyXG5FcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0dyb3VwRmlsdGVyRW1wdHlcIl0gPSBcIlRoZSBncm91cCBmaWx0ZXIgbXVzdCBub3QgYmUgZW1wdHkuXCJcclxuRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9Jc0FscmVhZHlJbkdyb3VwXCJdID0gXCJUaGUgY2hhbm5lbCBpcyBhbHJlYWR5IGEgbWVtYmVyIG9mIHRoaXMgZ3JvdXAuXCJcclxuRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9JbnZhbGlkQ2hhcmFjdGVyc1wiXSA9IFwiVXNlIEEtWiwgYS16LCAwLTkgb3IgXy5cIlxyXG5FcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0ludmFsaWRMZWFkaW5nQ2hhcmFjdGVyXCJdID0gXCJVc2UgQS1aIG9yIGEteiBhcyBmaXJzdCBjaGFyYWN0ZXIuXCJcclxuRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9OYW1lRW1wdHlcIl0gPSBcIlRoZSBuYW1lIG11c3Qgbm90IGJlIGVtcHR5LlwiXHJcbiIsImNsYXNzIE9ic2VydmFibGVHcm91cDxUPlxyXG57XHJcbiAgICBLZXk6IHN0cmluZztcclxuICAgIE1lbWJlcnM6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PFQ+XHJcblxyXG4gICAgY29uc3RydWN0b3Ioa2V5OiBzdHJpbmcsIG1lbWJlcnM6IFRbXSA9IG5ldyBBcnJheTxUPigpKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuS2V5ID0ga2V5XHJcbiAgICAgICAgdGhpcy5NZW1iZXJzID0ga28ub2JzZXJ2YWJsZUFycmF5KG1lbWJlcnMpXHJcbiAgICB9XHJcbn1cclxuXHJcbmZ1bmN0aW9uIE9ic2VydmFibGVHcm91cEJ5PFQ+KGxpc3Q6IFRbXSwgbmFtZUdldHRlcjogKHg6IFQpID0+IHN0cmluZywgZ3JvdXBOYW1lR2V0dGVyOiAoeDogVCkgPT4gc3RyaW5nLCBmaWx0ZXI6IHN0cmluZyk6IE9ic2VydmFibGVHcm91cDxUPltdXHJcbntcclxuICAgIGxldCByZXN1bHQ6IE9ic2VydmFibGVHcm91cDxUPltdXHJcbiAgICBsZXQgcmVnRXhwOiBSZWdFeHBcclxuXHJcbiAgICByZXN1bHQgPSBbXVxyXG4gICAgcmVnRXhwID0gbmV3IFJlZ0V4cChmaWx0ZXIsIFwiaVwiKVxyXG5cclxuICAgIGxpc3QuZm9yRWFjaChlbGVtZW50ID0+XHJcbiAgICB7XHJcbiAgICAgICAgaWYgKHJlZ0V4cC50ZXN0KG5hbWVHZXR0ZXIoZWxlbWVudCkpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgZ3JvdXBOYW1lR2V0dGVyKGVsZW1lbnQpLnNwbGl0KFwiXFxuXCIpLmZvckVhY2goZ3JvdXBOYW1lID0+XHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIEFkZFRvR3JvdXBlZEFycmF5KGVsZW1lbnQsIGdyb3VwTmFtZSwgcmVzdWx0KVxyXG4gICAgICAgICAgICB9KVxyXG4gICAgICAgIH1cclxuICAgIH0pXHJcblxyXG4gICAgcmV0dXJuIHJlc3VsdFxyXG59XHJcblxyXG5mdW5jdGlvbiBBZGRUb0dyb3VwZWRBcnJheTxUPihpdGVtOiBULCBncm91cE5hbWU6IHN0cmluZywgb2JzZXJ2YWJsZUdyb3VwU2V0OiBPYnNlcnZhYmxlR3JvdXA8VD5bXSlcclxue1xyXG4gICAgbGV0IGdyb3VwOiBPYnNlcnZhYmxlR3JvdXA8VD5cclxuXHJcbiAgICBncm91cCA9IG9ic2VydmFibGVHcm91cFNldC5maW5kKHkgPT4geS5LZXkgPT09IGdyb3VwTmFtZSlcclxuXHJcbiAgICBpZiAoIWdyb3VwKVxyXG4gICAge1xyXG4gICAgICAgIGdyb3VwID0gbmV3IE9ic2VydmFibGVHcm91cDxUPihncm91cE5hbWUpXHJcbiAgICAgICAgb2JzZXJ2YWJsZUdyb3VwU2V0LnB1c2goZ3JvdXApXHJcbiAgICB9XHJcblxyXG4gICAgZ3JvdXAuTWVtYmVycy5wdXNoKGl0ZW0pXHJcbn1cclxuXHJcbi8vZnVuY3Rpb24gQWRkVG9Hcm91cGVkT2JzZXJ2YWJsZUFycmF5PFQ+KGl0ZW06IFQsIGdyb3VwTmFtZTogc3RyaW5nLCBvYnNlcnZhYmxlR3JvdXBTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PE9ic2VydmFibGVHcm91cDxUPj4pXHJcbi8ve1xyXG4vLyAgICBsZXQgZ3JvdXA6IE9ic2VydmFibGVHcm91cDxUPlxyXG5cclxuLy8gICAgZ3JvdXAgPSBvYnNlcnZhYmxlR3JvdXBTZXQoKS5maW5kKHkgPT4geS5LZXkgPT09IGdyb3VwTmFtZSlcclxuXHJcbi8vICAgIGlmICghZ3JvdXApXHJcbi8vICAgIHtcclxuLy8gICAgICAgIGdyb3VwID0gbmV3IE9ic2VydmFibGVHcm91cDxUPihncm91cE5hbWUpXHJcbi8vICAgICAgICBvYnNlcnZhYmxlR3JvdXBTZXQucHVzaChncm91cClcclxuLy8gICAgfVxyXG5cclxuLy8gICAgZ3JvdXAuTWVtYmVycy5wdXNoKGl0ZW0pXHJcbi8vfVxyXG5cclxuLy9mdW5jdGlvbiBSZW1vdmVGcm9tR3JvdXBlZE9ic2VydmFibGVBcnJheTxUPihpdGVtOiBULCBvYnNlcnZhYmxlR3JvdXBTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PE9ic2VydmFibGVHcm91cDxUPj4pXHJcbi8ve1xyXG4vLyAgICB2YXIgZ3JvdXA6IE9ic2VydmFibGVHcm91cDxUPlxyXG5cclxuLy8gICAgb2JzZXJ2YWJsZUdyb3VwU2V0KCkuc29tZSh4ID0+XHJcbi8vICAgIHtcclxuLy8gICAgICAgIGlmICh4Lk1lbWJlcnMoKS5pbmRleE9mKGl0ZW0pID4gLTEpXHJcbi8vICAgICAgICB7XHJcbi8vICAgICAgICAgICAgZ3JvdXAgPSB4XHJcblxyXG4vLyAgICAgICAgICAgIHJldHVybiB0cnVlXHJcbi8vICAgICAgICB9XHJcblxyXG4vLyAgICAgICAgcmV0dXJuIGZhbHNlXHJcbi8vICAgIH0pXHJcblxyXG4vLyAgICBpZiAoZ3JvdXApXHJcbi8vICAgIHtcclxuLy8gICAgICAgIGdyb3VwLk1lbWJlcnMucmVtb3ZlKGl0ZW0pXHJcblxyXG4vLyAgICAgICAgaWYgKGdyb3VwLk1lbWJlcnMoKS5sZW5ndGggPT09IDApXHJcbi8vICAgICAgICB7XHJcbi8vICAgICAgICAgICAgb2JzZXJ2YWJsZUdyb3VwU2V0LnJlbW92ZShncm91cClcclxuLy8gICAgICAgIH1cclxuXHJcbi8vICAgICAgICByZXR1cm4gdHJ1ZVxyXG4vLyAgICB9XHJcblxyXG4vLyAgICByZXR1cm4gZmFsc2VcclxuLy99XHJcblxyXG4vL2Z1bmN0aW9uIFVwZGF0ZUdyb3VwZWRPYnNlcnZhYmxlQXJyYXk8VD4oaXRlbTogVCwgZ3JvdXBOYW1lOiBzdHJpbmcsIG9ic2VydmFibGVHcm91cFNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8T2JzZXJ2YWJsZUdyb3VwPFQ+PilcclxuLy97XHJcbi8vICAgIFJlbW92ZUZyb21Hcm91cGVkT2JzZXJ2YWJsZUFycmF5KGl0ZW0sIG9ic2VydmFibGVHcm91cFNldClcclxuLy8gICAgQWRkVG9Hcm91cGVkT2JzZXJ2YWJsZUFycmF5KGl0ZW0sIGdyb3VwTmFtZSwgb2JzZXJ2YWJsZUdyb3VwU2V0KVxyXG4vL31cclxuXHJcbmZ1bmN0aW9uIE1hcE1hbnk8VEFycmF5RWxlbWVudCwgVFNlbGVjdD4oYXJyYXk6IFRBcnJheUVsZW1lbnRbXSwgbWFwRnVuYzogKGl0ZW06IFRBcnJheUVsZW1lbnQpID0+IFRTZWxlY3RbXSk6IFRTZWxlY3RbXVxyXG57XHJcbiAgICByZXR1cm4gYXJyYXkucmVkdWNlKChwcmV2aW91cywgY3VycmVudCwgaSkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gcHJldmlvdXMuY29uY2F0KG1hcEZ1bmMoY3VycmVudCkpO1xyXG4gICAgfSwgPFRTZWxlY3RbXT5bXSk7XHJcbn1cclxuXHJcbmNsYXNzIEd1aWRcclxue1xyXG4gICAgc3RhdGljIE5ld0d1aWQoKVxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiAneHh4eHh4eHgteHh4eC00eHh4LXl4eHgteHh4eHh4eHh4eHh4Jy5yZXBsYWNlKC9beHldL2csIGZ1bmN0aW9uIChjKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdmFyIHIgPSBNYXRoLnJhbmRvbSgpICogMTYgfCAwXHJcbiAgICAgICAgICAgIHZhciB2ID0gYyA9PT0gJ3gnID8gciA6IChyICYgMHgzIHwgMHg4KVxyXG5cclxuICAgICAgICAgICAgcmV0dXJuIHYudG9TdHJpbmcoMTYpXHJcbiAgICAgICAgfSlcclxuICAgIH1cclxufVxyXG5cclxuZnVuY3Rpb24gZGVsYXkobXM6IG51bWJlcilcclxue1xyXG4gICAgcmV0dXJuIG5ldyBQcm9taXNlKHJlc29sdmUgPT4gc2V0VGltZW91dChyZXNvbHZlLCBtcykpO1xyXG59XHJcblxyXG5sZXQgQ2hlY2tOYW1pbmdDb252ZW50aW9uID0gKHZhbHVlOiBzdHJpbmcpID0+XHJcbntcclxuICAgIHZhciByZWdFeHA6IGFueVxyXG5cclxuICAgIGlmICh2YWx1ZS5sZW5ndGggPT09IDApXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHsgSGFzRXJyb3I6IHRydWUsIEVycm9yRGVzY3JpcHRpb246IEVycm9yTWVzc2FnZVtcIlByb2plY3RfTmFtZUVtcHR5XCJdIH1cclxuICAgIH1cclxuXHJcbiAgICByZWdFeHAgPSBuZXcgUmVnRXhwKFwiW15BLVphLXowLTlfXVwiKVxyXG5cclxuICAgIGlmIChyZWdFeHAudGVzdCh2YWx1ZSkpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHsgSGFzRXJyb3I6IHRydWUsIEVycm9yRGVzY3JpcHRpb246IEVycm9yTWVzc2FnZVtcIlByb2plY3RfSW52YWxpZENoYXJhY3RlcnNcIl0gfVxyXG4gICAgfVxyXG5cclxuICAgIHJlZ0V4cCA9IG5ldyBSZWdFeHAoXCJeWzAtOV9dXCIpXHJcblxyXG4gICAgaWYgKHJlZ0V4cC50ZXN0KHZhbHVlKSlcclxuICAgIHtcclxuICAgICAgICByZXR1cm4geyBIYXNFcnJvcjogdHJ1ZSwgRXJyb3JEZXNjcmlwdGlvbjogRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9JbnZhbGlkTGVhZGluZ0NoYXJhY3RlclwiXSB9XHJcbiAgICB9XHJcblxyXG4gICAgcmV0dXJuIHtcclxuICAgICAgICBIYXNFcnJvcjogZmFsc2UsXHJcbiAgICAgICAgRXJyb3JEZXNjcmlwdGlvbjogXCJcIlxyXG4gICAgfVxyXG59IiwiY2xhc3MgUGx1Z2luRmFjdG9yeVxyXG57XHJcbiAgICBwdWJsaWMgc3RhdGljIENyZWF0ZVBsdWdpblZpZXdNb2RlbEFzeW5jID0gYXN5bmMgKHBsdWdpblR5cGU6IHN0cmluZywgcGx1Z2luTW9kZWw6IGFueSkgPT5cclxuICAgIHtcclxuICAgICAgICBsZXQgcGx1Z2luSWRlbnRpZmljYXRpb246IFBsdWdpbklkZW50aWZpY2F0aW9uVmlld01vZGVsXHJcbiAgICAgICAgbGV0IHBsdWdpblZpZXdNb2RlbDogUGx1Z2luVmlld01vZGVsQmFzZVxyXG4gICAgICAgIGxldCBwbHVnaW5WaWV3TW9kZWxSYXc6IHN0cmluZ1xyXG5cclxuICAgICAgICBwbHVnaW5JZGVudGlmaWNhdGlvbiA9IFBsdWdpbkhpdmUuRmluZFBsdWdpbklkZW50aWZpY2F0aW9uKHBsdWdpblR5cGUsIHBsdWdpbk1vZGVsLkRlc2NyaXB0aW9uLklkKVxyXG5cclxuICAgICAgICBpZiAocGx1Z2luSWRlbnRpZmljYXRpb24pXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBwbHVnaW5WaWV3TW9kZWxSYXcgPSBhd2FpdCBDb25uZWN0aW9uTWFuYWdlci5JbnZva2VXZWJDbGllbnRIdWIoXCJHZXRQbHVnaW5TdHJpbmdSZXNvdXJjZVwiLCBwbHVnaW5Nb2RlbC5EZXNjcmlwdGlvbi5JZCwgcGx1Z2luSWRlbnRpZmljYXRpb24uVmlld01vZGVsUmVzb3VyY2VOYW1lKVxyXG4gICAgICAgICAgICBwbHVnaW5WaWV3TW9kZWwgPSA8UGx1Z2luVmlld01vZGVsQmFzZT5uZXcgRnVuY3Rpb24ocGx1Z2luVmlld01vZGVsUmF3ICsgXCI7IHJldHVybiBWaWV3TW9kZWxDb25zdHJ1Y3RvclwiKSgpKHBsdWdpbk1vZGVsLCBwbHVnaW5JZGVudGlmaWNhdGlvbilcclxuXHJcbiAgICAgICAgICAgIHJldHVybiBwbHVnaW5WaWV3TW9kZWxcclxuICAgICAgICB9XHJcbiAgICAgICAgZWxzZVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhyb3cgbmV3IEVycm9yKFwiTm8gY29ycmVzcG9uZGluZyBwbHVnaW4gZGVzY3JpcHRpb24gZm9yIHBsdWdpbiBJRCAnXCIgKyBwbHVnaW5Nb2RlbC5EZXNjcmlwdGlvbi5JZCArIFwiJyBmb3VuZC5cIilcclxuICAgICAgICB9XHJcbiAgICB9XHJcbn0iLCJjbGFzcyBQbHVnaW5IaXZlXHJcbntcclxuICAgIC8vIGZpZWxkc1xyXG4gICAgcHVibGljIHN0YXRpYyBQbHVnaW5JZGVudGlmaWNhdGlvblNldDogTWFwPHN0cmluZywgUGx1Z2luSWRlbnRpZmljYXRpb25WaWV3TW9kZWxbXT5cclxuXHJcbiAgICAvLyBjb25zdHJ1Y3RvcnNcclxuICAgIHN0YXRpYyBJbml0aWFsaXplID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICBQbHVnaW5IaXZlLlBsdWdpbklkZW50aWZpY2F0aW9uU2V0ID0gbmV3IE1hcDxzdHJpbmcsIFBsdWdpbklkZW50aWZpY2F0aW9uVmlld01vZGVsW10+KClcclxuICAgIH0gICBcclxuXHJcbiAgICBzdGF0aWMgRmluZFBsdWdpbklkZW50aWZpY2F0aW9uID0gKHBsdWdpblR5cGVOYW1lOiBzdHJpbmcsIHBsdWdpbklkOiBzdHJpbmcpID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIFBsdWdpbkhpdmUuUGx1Z2luSWRlbnRpZmljYXRpb25TZXQuZ2V0KHBsdWdpblR5cGVOYW1lKS5maW5kKHBsdWdpbklkZW50aWZpY2F0aW9uID0+IHBsdWdpbklkZW50aWZpY2F0aW9uLklkID09PSBwbHVnaW5JZCk7XHJcbiAgICB9XHJcbn0iLCJjbGFzcyBDaGFubmVsSHViVmlld01vZGVsXHJcbntcclxuICAgIHB1YmxpYyBOYW1lOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIEdyb3VwOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IERhdGFUeXBlOiBLbm9ja291dE9ic2VydmFibGU8T25lRGFzRGF0YVR5cGVFbnVtPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IEd1aWQ6IHN0cmluZ1xyXG4gICAgcHVibGljIHJlYWRvbmx5IENyZWF0aW9uRGF0ZVRpbWU6IHN0cmluZ1xyXG4gICAgcHVibGljIHJlYWRvbmx5IFVuaXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgVHJhbnNmZXJGdW5jdGlvblNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8VHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbD5cclxuICAgIHB1YmxpYyBTZWxlY3RlZFRyYW5zZmVyRnVuY3Rpb246IEtub2Nrb3V0T2JzZXJ2YWJsZTxUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsPlxyXG4gICAgcHVibGljIEV2YWx1YXRlZFRyYW5zZmVyRnVuY3Rpb25TZXQ6ICgodmFsdWU6IG51bWJlcikgPT4gbnVtYmVyKVtdXHJcbiAgICBwdWJsaWMgSXNTZWxlY3RlZDogS25vY2tvdXRPYnNlcnZhYmxlPGJvb2xlYW4+XHJcblxyXG4gICAgcHVibGljIHJlYWRvbmx5IERhdGFUeXBlU2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxPbmVEYXNEYXRhVHlwZUVudW0+XHJcblxyXG4gICAgcHVibGljIHJlYWRvbmx5IEFzc29jaWF0ZWREYXRhSW5wdXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZTxEYXRhUG9ydFZpZXdNb2RlbD5cclxuICAgIHB1YmxpYyByZWFkb25seSBBc3NvY2lhdGVkRGF0YU91dHB1dFNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8RGF0YVBvcnRWaWV3TW9kZWw+XHJcblxyXG4gICAgcHJpdmF0ZSBBc3NvY2lhdGVkRGF0YUlucHV0SWQ6IHN0cmluZ1xyXG4gICAgcHJpdmF0ZSBBc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0OiBzdHJpbmdbXVxyXG5cclxuICAgIGNvbnN0cnVjdG9yKGNoYW5uZWxIdWJNb2RlbDogQ2hhbm5lbEh1Yk1vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuTmFtZSA9IGtvLm9ic2VydmFibGU8c3RyaW5nPihjaGFubmVsSHViTW9kZWwuTmFtZSlcclxuICAgICAgICB0aGlzLkdyb3VwID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KGNoYW5uZWxIdWJNb2RlbC5Hcm91cClcclxuICAgICAgICB0aGlzLkRhdGFUeXBlU2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PE9uZURhc0RhdGFUeXBlRW51bT4oRW51bWVyYXRpb25IZWxwZXIuR2V0RW51bVZhbHVlcygnT25lRGFzRGF0YVR5cGVFbnVtJykpXHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZSA9IGtvLm9ic2VydmFibGU8T25lRGFzRGF0YVR5cGVFbnVtPihjaGFubmVsSHViTW9kZWwuRGF0YVR5cGUpXHJcbiAgICAgICAgdGhpcy5HdWlkID0gY2hhbm5lbEh1Yk1vZGVsLkd1aWRcclxuICAgICAgICB0aGlzLkNyZWF0aW9uRGF0ZVRpbWUgPSBjaGFubmVsSHViTW9kZWwuQ3JlYXRpb25EYXRlVGltZVxyXG4gICAgICAgIHRoaXMuVW5pdCA9IGtvLm9ic2VydmFibGU8c3RyaW5nPihjaGFubmVsSHViTW9kZWwuVW5pdClcclxuICAgICAgICB0aGlzLlRyYW5zZmVyRnVuY3Rpb25TZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8VHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbD4oY2hhbm5lbEh1Yk1vZGVsLlRyYW5zZmVyRnVuY3Rpb25TZXQubWFwKHRmID0+IG5ldyBUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsKHRmKSkpXHJcbiAgICAgICAgdGhpcy5TZWxlY3RlZFRyYW5zZmVyRnVuY3Rpb24gPSBrby5vYnNlcnZhYmxlPFRyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWw+KHRoaXMuQ3JlYXRlRGVmYXVsdFRyYW5zZmVyRnVuY3Rpb24oKSlcclxuICAgICAgICB0aGlzLklzU2VsZWN0ZWQgPSBrby5vYnNlcnZhYmxlPGJvb2xlYW4+KGZhbHNlKVxyXG5cclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXQgPSBrby5vYnNlcnZhYmxlPERhdGFQb3J0Vmlld01vZGVsPigpXHJcbiAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dFNldCA9IGtvLm9ic2VydmFibGVBcnJheTxEYXRhUG9ydFZpZXdNb2RlbD4oKVxyXG5cclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXRJZCA9IGNoYW5uZWxIdWJNb2RlbC5Bc3NvY2lhdGVkRGF0YUlucHV0SWRcclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQgPSBjaGFubmVsSHViTW9kZWwuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldFxyXG4gICAgICAgIHRoaXMuRXZhbHVhdGVkVHJhbnNmZXJGdW5jdGlvblNldCA9IFtdXHJcbiAgICB9XHJcblxyXG4gICAgLy8gbWV0aG9kc1xyXG4gICAgcHVibGljIEdldFRyYW5zZm9ybWVkVmFsdWUgPSAodmFsdWU6IGFueSk6IG51bWJlciA9PiBcclxuICAgIHtcclxuICAgICAgICBpZiAodmFsdWUgPT09IFwiTmFOXCIpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB2YWx1ZSA9IE5hTlxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5FdmFsdWF0ZWRUcmFuc2ZlckZ1bmN0aW9uU2V0LmZvckVhY2godGYgPT4gdmFsdWUgPSB0Zih2YWx1ZSkpXHJcblxyXG4gICAgICAgIHJldHVybiB2YWx1ZVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgQ3JlYXRlRGVmYXVsdFRyYW5zZmVyRnVuY3Rpb24gPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiBuZXcgVHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbChuZXcgVHJhbnNmZXJGdW5jdGlvbk1vZGVsKFwiMDAwMS0wMS0wMVQwMDowMDowMFpcIiwgXCJwb2x5bm9taWFsXCIsIFwicGVybWFuZW50XCIsIFwiMTswXCIpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBJc0Fzc29jaWF0aW9uQWxsb3dlZChkYXRhUG9ydDogRGF0YVBvcnRWaWV3TW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIChkYXRhUG9ydC5EYXRhVHlwZSAmIDB4ZmYpID09ICh0aGlzLkRhdGFUeXBlKCkgJiAweGZmKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBVcGRhdGVBc3NvY2lhdGlvbiA9IChkYXRhUG9ydDogRGF0YVBvcnRWaWV3TW9kZWwpID0+XHJcbiAgICB7XHJcbiAgICAgICAgc3dpdGNoIChkYXRhUG9ydC5EYXRhRGlyZWN0aW9uKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dDpcclxuXHJcbiAgICAgICAgICAgICAgICB0aGlzLlJlc2V0QXNzb2NpYXRpb24oZmFsc2UsIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dCgpKVxyXG4gICAgICAgICAgICAgICAgYnJlYWtcclxuXHJcbiAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uT3V0cHV0OlxyXG5cclxuICAgICAgICAgICAgICAgIHRoaXMuUmVzZXRBc3NvY2lhdGlvbihmYWxzZSwgZGF0YVBvcnQpXHJcbiAgICAgICAgICAgICAgICBicmVha1xyXG5cclxuICAgICAgICAgICAgZGVmYXVsdDpcclxuICAgICAgICAgICAgICAgIHRocm93IG5ldyBFcnJvcihcIk5vdCBpbXBsZW1lbnRlZC5cIik7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0aGlzLlNldEFzc29jaWF0aW9uKGRhdGFQb3J0KVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBTZXRBc3NvY2lhdGlvbihkYXRhUG9ydDogRGF0YVBvcnRWaWV3TW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgZGF0YVBvcnQuQXNzb2NpYXRlZENoYW5uZWxIdWJTZXQucHVzaCh0aGlzKVxyXG5cclxuICAgICAgICBzd2l0Y2ggKGRhdGFQb3J0LkRhdGFEaXJlY3Rpb24pXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLklucHV0OlxyXG5cclxuICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dChkYXRhUG9ydClcclxuICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dElkID0gZGF0YVBvcnQuVG9GdWxsUXVhbGlmaWVkSWRlbnRpZmllcigpXHJcblxyXG4gICAgICAgICAgICAgICAgYnJlYWtcclxuXHJcbiAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uT3V0cHV0OlxyXG5cclxuICAgICAgICAgICAgICAgIGxldCBkYXRhT3V0cHV0SWQgPSBkYXRhUG9ydC5Ub0Z1bGxRdWFsaWZpZWRJZGVudGlmaWVyKClcclxuXHJcbiAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0U2V0LnB1c2goZGF0YVBvcnQpXHJcblxyXG4gICAgICAgICAgICAgICAgaWYgKHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldC5pbmRleE9mKGRhdGFPdXRwdXRJZCkgPCAwKVxyXG4gICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldC5wdXNoKGRhdGFQb3J0LlRvRnVsbFF1YWxpZmllZElkZW50aWZpZXIoKSlcclxuICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICBicmVha1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgUmVzZXRBc3NvY2lhdGlvbihtYWludGFpbldlYWtSZWZlcmVuY2U6IGJvb2xlYW4sIC4uLmRhdGFQb3J0U2V0OiBEYXRhUG9ydFZpZXdNb2RlbFtdKVxyXG4gICAge1xyXG4gICAgICAgIGRhdGFQb3J0U2V0LmZvckVhY2goZGF0YVBvcnQgPT5cclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGlmICghZGF0YVBvcnQpXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIHJldHVyblxyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICBkYXRhUG9ydC5Bc3NvY2lhdGVkQ2hhbm5lbEh1YlNldC5yZW1vdmUodGhpcylcclxuXHJcbiAgICAgICAgICAgIHN3aXRjaCAoZGF0YVBvcnQuRGF0YURpcmVjdGlvbilcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dDpcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0KG51bGwpXHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmICghbWFpbnRhaW5XZWFrUmVmZXJlbmNlKVxyXG4gICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0SWQgPSBudWxsXHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBicmVha1xyXG5cclxuICAgICAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uT3V0cHV0OlxyXG5cclxuICAgICAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0U2V0LnJlbW92ZShkYXRhUG9ydClcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCFtYWludGFpbldlYWtSZWZlcmVuY2UpXHJcbiAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBsZXQgaW5kZXg6IG51bWJlciA9IHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldC5pbmRleE9mKGRhdGFQb3J0LlRvRnVsbFF1YWxpZmllZElkZW50aWZpZXIoKSlcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmIChpbmRleCA+IC0xKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQuc3BsaWNlKGluZGV4LCAxKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9ICAgXHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBicmVha1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgUmVzZXRBbGxBc3NvY2lhdGlvbnMobWFpbnRhaW5XZWFrUmVmZXJlbmNlOiBib29sZWFuKVxyXG4gICAge1xyXG4gICAgICAgIGlmICh0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXQoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuUmVzZXRBc3NvY2lhdGlvbihtYWludGFpbldlYWtSZWZlcmVuY2UsIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dCgpKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5SZXNldEFzc29jaWF0aW9uKG1haW50YWluV2Vha1JlZmVyZW5jZSwgLi4udGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dFNldCgpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBHZXRBc3NvY2lhdGVkRGF0YUlucHV0SWQgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXRJZDtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgR2V0QXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldDtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHtcclxuICAgICAgICAgICAgTmFtZTogPHN0cmluZz50aGlzLk5hbWUoKSxcclxuICAgICAgICAgICAgR3JvdXA6IDxzdHJpbmc+dGhpcy5Hcm91cCgpLFxyXG4gICAgICAgICAgICBEYXRhVHlwZTogPE9uZURhc0RhdGFUeXBlRW51bT50aGlzLkRhdGFUeXBlKCksXHJcbiAgICAgICAgICAgIEd1aWQ6IDxzdHJpbmc+dGhpcy5HdWlkLFxyXG4gICAgICAgICAgICBDcmVhdGlvbkRhdGVUaW1lOiA8c3RyaW5nPnRoaXMuQ3JlYXRpb25EYXRlVGltZSxcclxuICAgICAgICAgICAgVW5pdDogPHN0cmluZz50aGlzLlVuaXQoKSxcclxuICAgICAgICAgICAgVHJhbnNmZXJGdW5jdGlvblNldDogPFRyYW5zZmVyRnVuY3Rpb25Nb2RlbFtdPnRoaXMuVHJhbnNmZXJGdW5jdGlvblNldCgpLm1hcCh0ZiA9PiB0Zi5Ub01vZGVsKCkpLFxyXG4gICAgICAgICAgICBBc3NvY2lhdGVkRGF0YUlucHV0SWQ6IDxzdHJpbmc+dGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0SWQsXHJcbiAgICAgICAgICAgIEFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQ6IDxzdHJpbmdbXT50aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXRcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEFkZFRyYW5zZmVyRnVuY3Rpb24gPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuVHJhbnNmZXJGdW5jdGlvblNldC5wdXNoKHRoaXMuU2VsZWN0ZWRUcmFuc2ZlckZ1bmN0aW9uKCkpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIERlbGV0ZVRyYW5zZmVyRnVuY3Rpb24gPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuVHJhbnNmZXJGdW5jdGlvblNldC5yZW1vdmUodGhpcy5TZWxlY3RlZFRyYW5zZmVyRnVuY3Rpb24oKSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgTmV3VHJhbnNmZXJGdW5jdGlvbiA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5TZWxlY3RlZFRyYW5zZmVyRnVuY3Rpb24odGhpcy5DcmVhdGVEZWZhdWx0VHJhbnNmZXJGdW5jdGlvbigpKVxyXG4gICAgfVxyXG5cclxuICAgIC8vIGNvbW1hbmRzXHJcbiAgICBwdWJsaWMgU2VsZWN0VHJhbnNmZXJGdW5jdGlvbiA9ICh0cmFuc2ZlckZ1bmN0aW9uOiBUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuU2VsZWN0ZWRUcmFuc2ZlckZ1bmN0aW9uKHRyYW5zZmVyRnVuY3Rpb24pXHJcbiAgICB9XHJcbn0iLCJjbGFzcyBPbmVEYXNNb2R1bGVWaWV3TW9kZWxcclxue1xyXG4gICAgcHVibGljIERhdGFUeXBlOiBLbm9ja291dE9ic2VydmFibGU8T25lRGFzRGF0YVR5cGVFbnVtPlxyXG4gICAgcHVibGljIERhdGFEaXJlY3Rpb246IEtub2Nrb3V0T2JzZXJ2YWJsZTxEYXRhRGlyZWN0aW9uRW51bT5cclxuICAgIHB1YmxpYyBFbmRpYW5uZXNzOiBLbm9ja291dE9ic2VydmFibGU8RW5kaWFubmVzc0VudW0+XHJcbiAgICBwdWJsaWMgU2l6ZTogS25vY2tvdXRPYnNlcnZhYmxlPG51bWJlcj5cclxuICAgIHB1YmxpYyBNYXhTaXplOiBLbm9ja291dE9ic2VydmFibGU8bnVtYmVyPlxyXG4gICAgcHVibGljIEVycm9yTWVzc2FnZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBIYXNFcnJvcjogS25vY2tvdXRDb21wdXRlZDxib29sZWFuPlxyXG5cclxuICAgIHB1YmxpYyBEYXRhVHlwZVNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8T25lRGFzRGF0YVR5cGVFbnVtPlxyXG5cclxuICAgIHByaXZhdGUgX29uUHJvcGVydHlDaGFuZ2VkOiBFdmVudERpc3BhdGNoZXI8T25lRGFzTW9kdWxlVmlld01vZGVsLCBhbnk+XHJcbiAgICBwcm90ZWN0ZWQgX21vZGVsOiBhbnlcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihvbmVEYXNNb2R1bGVNb2RlbDogT25lRGFzTW9kdWxlTW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5fbW9kZWwgPSBvbmVEYXNNb2R1bGVNb2RlbFxyXG5cclxuICAgICAgICB0aGlzLkRhdGFUeXBlU2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PE9uZURhc0RhdGFUeXBlRW51bT4oRW51bWVyYXRpb25IZWxwZXIuR2V0RW51bVZhbHVlcygnT25lRGFzRGF0YVR5cGVFbnVtJykuZmlsdGVyKGRhdGFUeXBlID0+IGRhdGFUeXBlICE9PSBPbmVEYXNEYXRhVHlwZUVudW0uQk9PTEVBTikpXHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZSA9IGtvLm9ic2VydmFibGU8T25lRGFzRGF0YVR5cGVFbnVtPihvbmVEYXNNb2R1bGVNb2RlbC5EYXRhVHlwZSlcclxuICAgICAgICB0aGlzLkRhdGFEaXJlY3Rpb24gPSBrby5vYnNlcnZhYmxlPERhdGFEaXJlY3Rpb25FbnVtPihvbmVEYXNNb2R1bGVNb2RlbC5EYXRhRGlyZWN0aW9uKVxyXG4gICAgICAgIHRoaXMuRW5kaWFubmVzcyA9IGtvLm9ic2VydmFibGU8RW5kaWFubmVzc0VudW0+KG9uZURhc01vZHVsZU1vZGVsLkVuZGlhbm5lc3MpXHJcbiAgICAgICAgdGhpcy5TaXplID0ga28ub2JzZXJ2YWJsZTxudW1iZXI+KG9uZURhc01vZHVsZU1vZGVsLlNpemUpXHJcbiAgICAgICAgdGhpcy5NYXhTaXplID0ga28ub2JzZXJ2YWJsZTxudW1iZXI+KEluZmluaXR5KVxyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KFwiXCIpXHJcbiAgICAgICAgdGhpcy5IYXNFcnJvciA9IGtvLmNvbXB1dGVkPGJvb2xlYW4+KCgpID0+IHRoaXMuRXJyb3JNZXNzYWdlKCkubGVuZ3RoID4gMClcclxuXHJcbiAgICAgICAgdGhpcy5fb25Qcm9wZXJ0eUNoYW5nZWQgPSBuZXcgRXZlbnREaXNwYXRjaGVyPE9uZURhc01vZHVsZVZpZXdNb2RlbCwgYW55PigpO1xyXG5cclxuICAgICAgICB0aGlzLkRhdGFUeXBlLnN1YnNjcmliZShuZXdWYWx1ZSA9PiB0aGlzLk9uUHJvcGVydHlDaGFuZ2VkKCkpXHJcbiAgICAgICAgdGhpcy5EYXRhRGlyZWN0aW9uLnN1YnNjcmliZShuZXdWYWx1ZSA9PiB0aGlzLk9uUHJvcGVydHlDaGFuZ2VkKCkpXHJcbiAgICAgICAgdGhpcy5TaXplLnN1YnNjcmliZShuZXdWYWx1ZSA9PiB0aGlzLk9uUHJvcGVydHlDaGFuZ2VkKCkpXHJcbiAgICB9XHJcblxyXG4gICAgZ2V0IFByb3BlcnR5Q2hhbmdlZCgpOiBJRXZlbnQ8T25lRGFzTW9kdWxlVmlld01vZGVsLCBhbnk+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuX29uUHJvcGVydHlDaGFuZ2VkO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBPblByb3BlcnR5Q2hhbmdlZCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5WYWxpZGF0ZSgpXHJcbiAgICAgICAgdGhpcy5fb25Qcm9wZXJ0eUNoYW5nZWQuZGlzcGF0Y2godGhpcywgbnVsbClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgR2V0Qnl0ZUNvdW50ID0gKGJvb2xlYW5CaXRTaXplPzogbnVtYmVyKSA9PlxyXG4gICAge1xyXG4gICAgICAgIGlmIChib29sZWFuQml0U2l6ZSAmJiB0aGlzLkRhdGFUeXBlKCkgPT09IE9uZURhc0RhdGFUeXBlRW51bS5CT09MRUFOKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgYm9vbGVhbkJpdFNpemUgPSBwYXJzZUludCg8YW55PmJvb2xlYW5CaXRTaXplKVxyXG5cclxuICAgICAgICAgICAgcmV0dXJuIE1hdGguY2VpbChib29sZWFuQml0U2l6ZSAqIHRoaXMuU2l6ZSgpIC8gOCk7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIGVsc2VcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHJldHVybiAodGhpcy5EYXRhVHlwZSgpICYgMHgwRkYpIC8gOCAqIHRoaXMuU2l6ZSgpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBWYWxpZGF0ZSgpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJcIilcclxuXHJcbiAgICAgICAgaWYgKHRoaXMuU2l6ZSgpIDwgMSB8fCAoaXNGaW5pdGUodGhpcy5NYXhTaXplKCkpICYmIHRoaXMuU2l6ZSgpID4gdGhpcy5NYXhTaXplKCkpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJTaXplIG11c3QgYmUgd2l0aGluIHJhbmdlIDEuLlwiICsgdGhpcy5NYXhTaXplKCkgKyBcIi5cIilcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvU3RyaW5nKClcclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5TaXplKCkgKyBcInggXCIgKyBFbnVtZXJhdGlvbkhlbHBlci5HZXRFbnVtTG9jYWxpemF0aW9uKCdPbmVEYXNEYXRhVHlwZUVudW0nLCB0aGlzLkRhdGFUeXBlKCkpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEV4dGVuZE1vZGVsKG1vZGVsOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgLy9cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IG1vZGVsOiBhbnkgPSB7XHJcbiAgICAgICAgICAgICR0eXBlOiA8c3RyaW5nPnRoaXMuX21vZGVsLiR0eXBlLFxyXG4gICAgICAgICAgICBEYXRhVHlwZTogPE9uZURhc0RhdGFUeXBlRW51bT50aGlzLkRhdGFUeXBlKCksXHJcbiAgICAgICAgICAgIFNpemU6IDxudW1iZXI+dGhpcy5TaXplKCksXHJcbiAgICAgICAgICAgIERhdGFEaXJlY3Rpb246IDxEYXRhRGlyZWN0aW9uRW51bT50aGlzLkRhdGFEaXJlY3Rpb24oKSxcclxuICAgICAgICAgICAgRW5kaWFubmVzczogPEVuZGlhbm5lc3NFbnVtPnRoaXMuRW5kaWFubmVzcygpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0aGlzLkV4dGVuZE1vZGVsKG1vZGVsKVxyXG5cclxuICAgICAgICByZXR1cm4gbW9kZWxcclxuICAgIH1cclxufVxyXG4iLCJjbGFzcyBPbmVEYXNNb2R1bGVTZWxlY3RvclZpZXdNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgU2V0dGluZ3NUZW1wbGF0ZU5hbWU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgTmV3TW9kdWxlOiBLbm9ja291dE9ic2VydmFibGU8T25lRGFzTW9kdWxlVmlld01vZGVsPiAgXHJcbiAgICBwdWJsaWMgTWF4Qnl0ZXM6IEtub2Nrb3V0T2JzZXJ2YWJsZTxudW1iZXI+XHJcbiAgICBwdWJsaWMgUmVtYWluaW5nQnl0ZXM6IEtub2Nrb3V0T2JzZXJ2YWJsZTxudW1iZXI+XHJcbiAgICBwdWJsaWMgUmVtYWluaW5nQ291bnQ6IEtub2Nrb3V0T2JzZXJ2YWJsZTxudW1iZXI+ICAgIFxyXG4gICAgcHVibGljIE1vZHVsZVNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8T25lRGFzTW9kdWxlVmlld01vZGVsPlxyXG4gICAgcHVibGljIEVycm9yTWVzc2FnZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBIYXNFcnJvcjogS25vY2tvdXRDb21wdXRlZDxib29sZWFuPlxyXG5cclxuICAgIHB1YmxpYyBPbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxPbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtPlxyXG5cclxuICAgIHByaXZhdGUgX29uTW9kdWxlU2V0Q2hhbmdlZDogRXZlbnREaXNwYXRjaGVyPE9uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsLCBPbmVEYXNNb2R1bGVWaWV3TW9kZWxbXT5cclxuXHJcbiAgICBjb25zdHJ1Y3RvcihvbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGU6IE9uZURhc01vZHVsZVNlbGVjdG9yTW9kZUVudW0sIG1vZHVsZVNldDogT25lRGFzTW9kdWxlVmlld01vZGVsW10gPSBbXSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yTW9kZSA9IGtvLm9ic2VydmFibGU8T25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlRW51bT4ob25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlKVxyXG5cclxuICAgICAgICB0aGlzLlNldHRpbmdzVGVtcGxhdGVOYW1lID0ga28ub2JzZXJ2YWJsZShcIlByb2plY3RfT25lRGFzTW9kdWxlVGVtcGxhdGVcIilcclxuICAgICAgICB0aGlzLk5ld01vZHVsZSA9IGtvLm9ic2VydmFibGU8T25lRGFzTW9kdWxlVmlld01vZGVsPigpO1xyXG4gICAgICAgIHRoaXMuTWF4Qnl0ZXMgPSBrby5vYnNlcnZhYmxlPG51bWJlcj4oSW5maW5pdHkpO1xyXG4gICAgICAgIHRoaXMuUmVtYWluaW5nQnl0ZXMgPSBrby5vYnNlcnZhYmxlPG51bWJlcj4oTmFOKTtcclxuICAgICAgICB0aGlzLlJlbWFpbmluZ0NvdW50ID0ga28ub2JzZXJ2YWJsZTxudW1iZXI+KE5hTik7XHJcbiAgICAgICAgdGhpcy5Nb2R1bGVTZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8T25lRGFzTW9kdWxlVmlld01vZGVsPihtb2R1bGVTZXQpO1xyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KFwiXCIpXHJcbiAgICAgICAgdGhpcy5IYXNFcnJvciA9IGtvLmNvbXB1dGVkPGJvb2xlYW4+KCgpID0+IHRoaXMuRXJyb3JNZXNzYWdlKCkubGVuZ3RoID4gMClcclxuXHJcbiAgICAgICAgdGhpcy5fb25Nb2R1bGVTZXRDaGFuZ2VkID0gbmV3IEV2ZW50RGlzcGF0Y2hlcjxPbmVEYXNNb2R1bGVTZWxlY3RvclZpZXdNb2RlbCwgT25lRGFzTW9kdWxlVmlld01vZGVsW10+KCk7XHJcblxyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxDcmVhdGVOZXdNb2R1bGUoKVxyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgfVxyXG5cclxuICAgIGdldCBPbk1vZHVsZVNldENoYW5nZWQoKTogSUV2ZW50PE9uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsLCBPbmVEYXNNb2R1bGVWaWV3TW9kZWxbXT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5fb25Nb2R1bGVTZXRDaGFuZ2VkO1xyXG4gICAgfVxyXG4gICAgXHJcbiAgICAvLyBtZXRob2RzXHJcbiAgICBwdWJsaWMgU2V0TWF4Qnl0ZXMgPSAodmFsdWU6IG51bWJlcikgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLk1heEJ5dGVzKHZhbHVlKVxyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBHZXRJbnB1dE1vZHVsZVNldCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuTW9kdWxlU2V0KCkuZmlsdGVyKG1vZHVsZSA9PiBtb2R1bGUuRGF0YURpcmVjdGlvbigpID09PSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgR2V0T3V0cHV0TW9kdWxlU2V0ID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5Nb2R1bGVTZXQoKS5maWx0ZXIobW9kdWxlID0+IG1vZHVsZS5EYXRhRGlyZWN0aW9uKCkgPT09IERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dClcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIEludGVybmFsVXBkYXRlKClcclxuICAgIHtcclxuICAgICAgICB0aGlzLlVwZGF0ZSgpXHJcbiAgICAgICAgdGhpcy5WYWxpZGF0ZSgpXHJcbiAgICB9XHJcblxyXG4gICAgcHJvdGVjdGVkIFVwZGF0ZSgpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IG1vZHVsZVNldDogT25lRGFzTW9kdWxlVmlld01vZGVsW11cclxuICAgICAgICBsZXQgcmVtYWluaW5nQnl0ZXM6IG51bWJlclxyXG5cclxuICAgICAgICBzd2l0Y2ggKHRoaXMuTmV3TW9kdWxlKCkuRGF0YURpcmVjdGlvbigpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dDpcclxuICAgICAgICAgICAgICAgIG1vZHVsZVNldCA9IHRoaXMuR2V0SW5wdXRNb2R1bGVTZXQoKVxyXG4gICAgICAgICAgICAgICAgYnJlYWs7XHJcblxyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dDpcclxuICAgICAgICAgICAgICAgIG1vZHVsZVNldCA9IHRoaXMuR2V0T3V0cHV0TW9kdWxlU2V0KClcclxuICAgICAgICAgICAgICAgIGJyZWFrO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmVtYWluaW5nQnl0ZXMgPSB0aGlzLk1heEJ5dGVzKCkgLSBtb2R1bGVTZXQubWFwKG9uZURhc01vZHVsZSA9PiBvbmVEYXNNb2R1bGUuR2V0Qnl0ZUNvdW50KCkpLnJlZHVjZSgocHJldmlvdXNWYWx1ZSwgY3VycmVudFZhbHVlKSA9PiBwcmV2aW91c1ZhbHVlICsgY3VycmVudFZhbHVlLCAwKVxyXG5cclxuICAgICAgICB0aGlzLlJlbWFpbmluZ0J5dGVzKHJlbWFpbmluZ0J5dGVzKVxyXG4gICAgICAgIHRoaXMuUmVtYWluaW5nQ291bnQoTWF0aC5mbG9vcih0aGlzLlJlbWFpbmluZ0J5dGVzKCkgLyAoKHRoaXMuTmV3TW9kdWxlKCkuRGF0YVR5cGUoKSAmIDB4MEZGKSAvIDgpKSlcclxuICAgIH1cclxuXHJcbiAgICBwcm90ZWN0ZWQgVmFsaWRhdGUoKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiXCIpXHJcblxyXG4gICAgICAgIGlmICh0aGlzLk5ld01vZHVsZSgpLkhhc0Vycm9yKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIlJlc29sdmUgYWxsIHJlbWFpbmluZyBtb2R1bGUgZXJyb3JzIGJlZm9yZSBjb250aW51aW5nLlwiKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaWYgKHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlKCkgPT09IE9uZURhc01vZHVsZVNlbGVjdG9yTW9kZUVudW0uSW5wdXRPbmx5ICYmIHRoaXMuTmV3TW9kdWxlKCkuRGF0YURpcmVjdGlvbigpID09IERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dClcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiT25seSBpbnB1dCBtb2R1bGVzIGFyZSBhbGxvd2VkLlwiKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaWYgKHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlKCkgPT09IE9uZURhc01vZHVsZVNlbGVjdG9yTW9kZUVudW0uT3V0cHV0T25seSAmJiB0aGlzLk5ld01vZHVsZSgpLkRhdGFEaXJlY3Rpb24oKSA9PSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dClcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiT25seSBvdXRwdXQgbW9kdWxlcyBhcmUgYWxsb3dlZC5cIilcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmIChpc0Zpbml0ZSh0aGlzLlJlbWFpbmluZ0J5dGVzKCkpICYmICh0aGlzLlJlbWFpbmluZ0J5dGVzKCkgLSB0aGlzLk5ld01vZHVsZSgpLkdldEJ5dGVDb3VudCgpIDwgMCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIkJ5dGUgY291bnQgb2YgbmV3IG1vZHVsZSBpcyB0b28gaGlnaC5cIilcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmICh0aGlzLlJlbWFpbmluZ0NvdW50KCkgPD0gMClcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiVGhlIG1heGltdW0gbnVtYmVyIG9mIG1vZHVsZXMgaXMgcmVhY2hlZC5cIilcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHJvdGVjdGVkIENyZWF0ZU5ld01vZHVsZSgpXHJcbiAgICB7XHJcbiAgICAgICAgaWYgKHRoaXMuTmV3TW9kdWxlKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICByZXR1cm4gbmV3IE9uZURhc01vZHVsZVZpZXdNb2RlbChuZXcgT25lRGFzTW9kdWxlTW9kZWwodGhpcy5OZXdNb2R1bGUoKS5EYXRhVHlwZSgpLCB0aGlzLk5ld01vZHVsZSgpLkRhdGFEaXJlY3Rpb24oKSwgdGhpcy5OZXdNb2R1bGUoKS5FbmRpYW5uZXNzKCksIDEpKVxyXG4gICAgICAgIH1cclxuICAgICAgICBlbHNlXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICByZXR1cm4gbmV3IE9uZURhc01vZHVsZVZpZXdNb2RlbChuZXcgT25lRGFzTW9kdWxlTW9kZWwoT25lRGFzRGF0YVR5cGVFbnVtLlVJTlQxNiwgRGF0YURpcmVjdGlvbkVudW0uSW5wdXQsIEVuZGlhbm5lc3NFbnVtLkxpdHRsZUVuZGlhbiwgMSkpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgSW50ZXJuYWxDcmVhdGVOZXdNb2R1bGUoKVxyXG4gICAge1xyXG4gICAgICAgIGlmICh0aGlzLk5ld01vZHVsZSgpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5OZXdNb2R1bGUoKS5Qcm9wZXJ0eUNoYW5nZWQudW5zdWJzY3JpYmUodGhpcy5Pbk1vZHVsZVByb3BlcnR5Q2hhbmdlZClcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuTmV3TW9kdWxlKHRoaXMuQ3JlYXRlTmV3TW9kdWxlKCkpXHJcbiAgICAgICAgdGhpcy5OZXdNb2R1bGUoKS5Qcm9wZXJ0eUNoYW5nZWQuc3Vic2NyaWJlKHRoaXMuT25Nb2R1bGVQcm9wZXJ0eUNoYW5nZWQpXHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBPbk1vZHVsZVByb3BlcnR5Q2hhbmdlZCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5JbnRlcm5hbFVwZGF0ZSgpXHJcbiAgICB9XHJcblxyXG4gICAgLy8gY29tbWFuZHNcclxuICAgIHB1YmxpYyBBZGRNb2R1bGUgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIGxldCBuZXdNb2R1bGU6IE9uZURhc01vZHVsZVZpZXdNb2RlbFxyXG5cclxuICAgICAgICBpZiAoIXRoaXMuSGFzRXJyb3IoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuTW9kdWxlU2V0LnB1c2godGhpcy5OZXdNb2R1bGUoKSlcclxuICAgICAgICAgICAgdGhpcy5JbnRlcm5hbENyZWF0ZU5ld01vZHVsZSgpXHJcbiAgICAgICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgICAgICAgICB0aGlzLl9vbk1vZHVsZVNldENoYW5nZWQuZGlzcGF0Y2godGhpcywgdGhpcy5Nb2R1bGVTZXQoKSlcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIERlbGV0ZU1vZHVsZSA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5Nb2R1bGVTZXQucG9wKClcclxuICAgICAgICB0aGlzLkludGVybmFsVXBkYXRlKClcclxuICAgICAgICB0aGlzLl9vbk1vZHVsZVNldENoYW5nZWQuZGlzcGF0Y2godGhpcywgdGhpcy5Nb2R1bGVTZXQoKSlcclxuICAgIH1cclxufSIsImNsYXNzIFRyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWxcclxue1xyXG4gICAgcHVibGljIERhdGVUaW1lOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIFR5cGU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgT3B0aW9uOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIEFyZ3VtZW50OiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKHRyYW5zZmVyRnVuY3Rpb25Nb2RlbDogVHJhbnNmZXJGdW5jdGlvbk1vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuRGF0ZVRpbWUgPSBrby5vYnNlcnZhYmxlKHRyYW5zZmVyRnVuY3Rpb25Nb2RlbC5EYXRlVGltZSlcclxuICAgICAgICB0aGlzLlR5cGUgPSBrby5vYnNlcnZhYmxlKHRyYW5zZmVyRnVuY3Rpb25Nb2RlbC5UeXBlKVxyXG4gICAgICAgIHRoaXMuT3B0aW9uID0ga28ub2JzZXJ2YWJsZSh0cmFuc2ZlckZ1bmN0aW9uTW9kZWwuT3B0aW9uKVxyXG4gICAgICAgIHRoaXMuQXJndW1lbnQgPSBrby5vYnNlcnZhYmxlKHRyYW5zZmVyRnVuY3Rpb25Nb2RlbC5Bcmd1bWVudClcclxuICAgIH1cclxuXHJcbiAgICAvLyBtZXRob2RzXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIG5ldyBUcmFuc2ZlckZ1bmN0aW9uTW9kZWwodGhpcy5EYXRlVGltZSgpLCB0aGlzLlR5cGUoKSwgdGhpcy5PcHRpb24oKSwgdGhpcy5Bcmd1bWVudCgpKVxyXG4gICAgfVxyXG59IiwiY2xhc3MgQnVmZmVyUmVxdWVzdFNlbGVjdG9yVmlld01vZGVsXHJcbntcclxuICAgIHB1YmxpYyBOZXdCdWZmZXJSZXF1ZXN0OiBLbm9ja291dE9ic2VydmFibGU8QnVmZmVyUmVxdWVzdFZpZXdNb2RlbD4gIFxyXG4gICAgcHVibGljIEJ1ZmZlclJlcXVlc3RTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWw+XHJcbiAgICBwdWJsaWMgRXJyb3JNZXNzYWdlOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIEhhc0Vycm9yOiBLbm9ja291dENvbXB1dGVkPGJvb2xlYW4+XHJcblxyXG4gICAgcHJpdmF0ZSBfb25CdWZmZXJSZXF1ZXN0U2V0Q2hhbmdlZDogRXZlbnREaXNwYXRjaGVyPEJ1ZmZlclJlcXVlc3RTZWxlY3RvclZpZXdNb2RlbCwgQnVmZmVyUmVxdWVzdFZpZXdNb2RlbFtdPlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKGJ1ZmZlclJlcXVlc3RTZXQ6IEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWxbXSA9IFtdKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuTmV3QnVmZmVyUmVxdWVzdCA9IGtvLm9ic2VydmFibGU8QnVmZmVyUmVxdWVzdFZpZXdNb2RlbD4oKTtcclxuICAgICAgICB0aGlzLkJ1ZmZlclJlcXVlc3RTZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8QnVmZmVyUmVxdWVzdFZpZXdNb2RlbD4oYnVmZmVyUmVxdWVzdFNldCk7XHJcbiAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UgPSBrby5vYnNlcnZhYmxlPHN0cmluZz4oXCJcIilcclxuICAgICAgICB0aGlzLkhhc0Vycm9yID0ga28uY29tcHV0ZWQ8Ym9vbGVhbj4oKCkgPT4gdGhpcy5FcnJvck1lc3NhZ2UoKS5sZW5ndGggPiAwKVxyXG5cclxuICAgICAgICB0aGlzLl9vbkJ1ZmZlclJlcXVlc3RTZXRDaGFuZ2VkID0gbmV3IEV2ZW50RGlzcGF0Y2hlcjxCdWZmZXJSZXF1ZXN0U2VsZWN0b3JWaWV3TW9kZWwsIEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWxbXT4oKTtcclxuXHJcbiAgICAgICAgdGhpcy5JbnRlcm5hbENyZWF0ZU5ld0J1ZmZlclJlcXVlc3QoKVxyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgfVxyXG5cclxuICAgIGdldCBPbkJ1ZmZlclJlcXVlc3RTZXRDaGFuZ2VkKCk6IElFdmVudDxCdWZmZXJSZXF1ZXN0U2VsZWN0b3JWaWV3TW9kZWwsIEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWxbXT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5fb25CdWZmZXJSZXF1ZXN0U2V0Q2hhbmdlZDtcclxuICAgIH1cclxuICAgIFxyXG4gICAgLy8gbWV0aG9kc1xyXG4gICAgcHJpdmF0ZSBJbnRlcm5hbFVwZGF0ZSgpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5VcGRhdGUoKVxyXG4gICAgICAgIHRoaXMuVmFsaWRhdGUoKVxyXG4gICAgfVxyXG5cclxuICAgIHByb3RlY3RlZCBVcGRhdGUoKVxyXG4gICAge1xyXG4gICAgICAgIC8vXHJcbiAgICB9XHJcblxyXG4gICAgcHJvdGVjdGVkIFZhbGlkYXRlKClcclxuICAgIHtcclxuICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIlwiKVxyXG5cclxuICAgICAgICBpZiAodGhpcy5OZXdCdWZmZXJSZXF1ZXN0KCkuSGFzRXJyb3IoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiUmVzb2x2ZSBhbGwgcmVtYWluaW5nIGVycm9ycyBiZWZvcmUgY29udGludWluZy5cIilcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHJvdGVjdGVkIENyZWF0ZU5ld0J1ZmZlclJlcXVlc3QoKVxyXG4gICAge1xyXG4gICAgICAgIGlmICh0aGlzLk5ld0J1ZmZlclJlcXVlc3QoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHJldHVybiBuZXcgQnVmZmVyUmVxdWVzdFZpZXdNb2RlbChuZXcgQnVmZmVyUmVxdWVzdE1vZGVsKHRoaXMuTmV3QnVmZmVyUmVxdWVzdCgpLlNhbXBsZVJhdGUoKSwgdGhpcy5OZXdCdWZmZXJSZXF1ZXN0KCkuR3JvdXBGaWx0ZXIoKSkpXHJcbiAgICAgICAgfVxyXG4gICAgICAgIGVsc2VcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHJldHVybiBuZXcgQnVmZmVyUmVxdWVzdFZpZXdNb2RlbChuZXcgQnVmZmVyUmVxdWVzdE1vZGVsKFNhbXBsZVJhdGVFbnVtLlNhbXBsZVJhdGVfMSwgXCIqXCIpKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIEludGVybmFsQ3JlYXRlTmV3QnVmZmVyUmVxdWVzdCgpXHJcbiAgICB7XHJcbiAgICAgICAgaWYgKHRoaXMuTmV3QnVmZmVyUmVxdWVzdCgpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5OZXdCdWZmZXJSZXF1ZXN0KCkuUHJvcGVydHlDaGFuZ2VkLnVuc3Vic2NyaWJlKHRoaXMuT25CdWZmZXJSZXF1ZXN0UHJvcGVydHlDaGFuZ2VkKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5OZXdCdWZmZXJSZXF1ZXN0KHRoaXMuQ3JlYXRlTmV3QnVmZmVyUmVxdWVzdCgpKVxyXG4gICAgICAgIHRoaXMuTmV3QnVmZmVyUmVxdWVzdCgpLlByb3BlcnR5Q2hhbmdlZC5zdWJzY3JpYmUodGhpcy5PbkJ1ZmZlclJlcXVlc3RQcm9wZXJ0eUNoYW5nZWQpXHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBPbkJ1ZmZlclJlcXVlc3RQcm9wZXJ0eUNoYW5nZWQgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgfVxyXG5cclxuICAgIC8vIGNvbW1hbmRzXHJcbiAgICBwdWJsaWMgQWRkQnVmZmVyUmVxdWVzdCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgbGV0IG5ld0J1ZmZlclJlcXVlc3Q6IEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWxcclxuXHJcbiAgICAgICAgaWYgKCF0aGlzLkhhc0Vycm9yKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkJ1ZmZlclJlcXVlc3RTZXQucHVzaCh0aGlzLk5ld0J1ZmZlclJlcXVlc3QoKSlcclxuICAgICAgICAgICAgdGhpcy5JbnRlcm5hbENyZWF0ZU5ld0J1ZmZlclJlcXVlc3QoKVxyXG4gICAgICAgICAgICB0aGlzLkludGVybmFsVXBkYXRlKClcclxuICAgICAgICAgICAgdGhpcy5fb25CdWZmZXJSZXF1ZXN0U2V0Q2hhbmdlZC5kaXNwYXRjaCh0aGlzLCB0aGlzLkJ1ZmZlclJlcXVlc3RTZXQoKSlcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIERlbGV0ZUJ1ZmZlclJlcXVlc3QgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuQnVmZmVyUmVxdWVzdFNldC5wb3AoKVxyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgICAgIHRoaXMuX29uQnVmZmVyUmVxdWVzdFNldENoYW5nZWQuZGlzcGF0Y2godGhpcywgdGhpcy5CdWZmZXJSZXF1ZXN0U2V0KCkpXHJcbiAgICB9XHJcbn0iLCJjbGFzcyBCdWZmZXJSZXF1ZXN0Vmlld01vZGVsXHJcbntcclxuICAgIHB1YmxpYyBTYW1wbGVSYXRlOiBLbm9ja291dE9ic2VydmFibGU8U2FtcGxlUmF0ZUVudW0+XHJcbiAgICBwdWJsaWMgR3JvdXBGaWx0ZXI6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgRXJyb3JNZXNzYWdlOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIEhhc0Vycm9yOiBLbm9ja291dENvbXB1dGVkPGJvb2xlYW4+XHJcblxyXG4gICAgcHVibGljIFNhbXBsZVJhdGVTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PFNhbXBsZVJhdGVFbnVtPlxyXG5cclxuICAgIHByaXZhdGUgX29uUHJvcGVydHlDaGFuZ2VkOiBFdmVudERpc3BhdGNoZXI8QnVmZmVyUmVxdWVzdFZpZXdNb2RlbCwgYW55PlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKG1vZGVsOiBCdWZmZXJSZXF1ZXN0TW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5TYW1wbGVSYXRlU2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PFNhbXBsZVJhdGVFbnVtPihFbnVtZXJhdGlvbkhlbHBlci5HZXRFbnVtVmFsdWVzKFwiU2FtcGxlUmF0ZUVudW1cIikpXHJcbiAgICAgICAgdGhpcy5TYW1wbGVSYXRlID0ga28ub2JzZXJ2YWJsZTxTYW1wbGVSYXRlRW51bT4obW9kZWwuU2FtcGxlUmF0ZSk7XHJcbiAgICAgICAgdGhpcy5Hcm91cEZpbHRlciA9IGtvLm9ic2VydmFibGU8c3RyaW5nPihtb2RlbC5Hcm91cEZpbHRlcik7XHJcblxyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KFwiXCIpXHJcbiAgICAgICAgdGhpcy5IYXNFcnJvciA9IGtvLmNvbXB1dGVkPGJvb2xlYW4+KCgpID0+IHRoaXMuRXJyb3JNZXNzYWdlKCkubGVuZ3RoID4gMClcclxuXHJcbiAgICAgICAgdGhpcy5fb25Qcm9wZXJ0eUNoYW5nZWQgPSBuZXcgRXZlbnREaXNwYXRjaGVyPEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWwsIGFueT4oKTtcclxuXHJcbiAgICAgICAgdGhpcy5TYW1wbGVSYXRlLnN1YnNjcmliZShuZXdWYWx1ZSA9PiB0aGlzLk9uUHJvcGVydHlDaGFuZ2VkKCkpXHJcbiAgICAgICAgdGhpcy5Hcm91cEZpbHRlci5zdWJzY3JpYmUobmV3VmFsdWUgPT4gdGhpcy5PblByb3BlcnR5Q2hhbmdlZCgpKVxyXG4gICAgfVxyXG5cclxuICAgIGdldCBQcm9wZXJ0eUNoYW5nZWQoKTogSUV2ZW50PEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWwsIGFueT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5fb25Qcm9wZXJ0eUNoYW5nZWQ7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIE9uUHJvcGVydHlDaGFuZ2VkID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLlZhbGlkYXRlKClcclxuICAgICAgICB0aGlzLl9vblByb3BlcnR5Q2hhbmdlZC5kaXNwYXRjaCh0aGlzLCBudWxsKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBWYWxpZGF0ZSgpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IHJlc3VsdDogYW55XHJcblxyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiXCIpXHJcblxyXG4gICAgICAgIHRoaXMuR3JvdXBGaWx0ZXIoKS5zcGxpdChcIjtcIikuZm9yRWFjaChncm91cEZpbHRlciA9PlxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgcmVzdWx0ID0gdGhpcy5DaGVja0dyb3VwRmlsdGVyKGdyb3VwRmlsdGVyKVxyXG5cclxuICAgICAgICAgICAgaWYgKHJlc3VsdC5IYXNFcnJvcilcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UocmVzdWx0LkVycm9yRGVzY3JpcHRpb24pXHJcblxyXG4gICAgICAgICAgICAgICAgcmV0dXJuXHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9KVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb1N0cmluZygpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIFwic2FtcGxlIHJhdGU6IFwiICsgRW51bWVyYXRpb25IZWxwZXIuR2V0RW51bUxvY2FsaXphdGlvbihcIlNhbXBsZVJhdGVFbnVtXCIsIHRoaXMuU2FtcGxlUmF0ZSgpKSArIFwiIC8gZ3JvdXAgZmlsdGVyOiAnXCIgKyB0aGlzLkdyb3VwRmlsdGVyKCkgKyBcIidcIlxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb01vZGVsKClcclxuICAgIHtcclxuICAgICAgICBsZXQgbW9kZWw6IGFueSA9IHtcclxuICAgICAgICAgICAgU2FtcGxlUmF0ZTogPFNhbXBsZVJhdGVFbnVtPnRoaXMuU2FtcGxlUmF0ZSgpLFxyXG4gICAgICAgICAgICBHcm91cEZpbHRlcjogPHN0cmluZz50aGlzLkdyb3VwRmlsdGVyKClcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJldHVybiBtb2RlbFxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgQ2hlY2tHcm91cEZpbHRlcih2YWx1ZTogc3RyaW5nKVxyXG4gICAge1xyXG4gICAgICAgIHZhciByZWdFeHA6IGFueVxyXG5cclxuICAgICAgICBpZiAodmFsdWUubGVuZ3RoID09PSAwKSB7XHJcbiAgICAgICAgICAgIHJldHVybiB7IEhhc0Vycm9yOiB0cnVlLCBFcnJvckRlc2NyaXB0aW9uOiBFcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0dyb3VwRmlsdGVyRW1wdHlcIl0gfVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmVnRXhwID0gbmV3IFJlZ0V4cChcIlteQS1aYS16MC05XyEqXVwiKVxyXG5cclxuICAgICAgICBpZiAocmVnRXhwLnRlc3QodmFsdWUpKSB7XHJcbiAgICAgICAgICAgIHJldHVybiB7IEhhc0Vycm9yOiB0cnVlLCBFcnJvckRlc2NyaXB0aW9uOiBFcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0ludmFsaWRDaGFyYWN0ZXJzXCJdIH1cclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJlZ0V4cCA9IG5ldyBSZWdFeHAoXCJeWzAtOV9dXCIpXHJcblxyXG4gICAgICAgIGlmIChyZWdFeHAudGVzdCh2YWx1ZSkpIHtcclxuICAgICAgICAgICAgcmV0dXJuIHsgSGFzRXJyb3I6IHRydWUsIEVycm9yRGVzY3JpcHRpb246IEVycm9yTWVzc2FnZVtcIlByb2plY3RfSW52YWxpZExlYWRpbmdDaGFyYWN0ZXJcIl0gfVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmVnRXhwID0gbmV3IFJlZ0V4cChcIl4hXCIpXHJcblxyXG4gICAgICAgIGlmIChyZWdFeHAudGVzdCh2YWx1ZSkgJiYgdmFsdWUubGVuZ3RoID09PSAxKSB7XHJcbiAgICAgICAgICAgIHJldHVybiB7IEhhc0Vycm9yOiB0cnVlLCBFcnJvckRlc2NyaXB0aW9uOiBFcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0RldGFjaGVkRXhjbGFtYXRpb25NYXJrTm90QWxsb3dlZFwiXSB9XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4ge1xyXG4gICAgICAgICAgICBIYXNFcnJvcjogZmFsc2UsXHJcbiAgICAgICAgICAgIEVycm9yRGVzY3JpcHRpb246IFwiXCJcclxuICAgICAgICB9XHJcbiAgICB9XHJcbn0iLCJjbGFzcyBEYXRhUG9ydFZpZXdNb2RlbFxyXG57XHJcbiAgICAvLyBmaWVsZHNcclxuICAgIHB1YmxpYyBOYW1lOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IERhdGFUeXBlOiBPbmVEYXNEYXRhVHlwZUVudW1cclxuICAgIHB1YmxpYyByZWFkb25seSBEYXRhRGlyZWN0aW9uOiBEYXRhRGlyZWN0aW9uRW51bVxyXG4gICAgcHVibGljIHJlYWRvbmx5IEVuZGlhbm5lc3M6IEVuZGlhbm5lc3NFbnVtXHJcblxyXG4gICAgcHVibGljIElzU2VsZWN0ZWQ6IEtub2Nrb3V0T2JzZXJ2YWJsZTxib29sZWFuPlxyXG4gICAgcHVibGljIEFzc29jaWF0ZWRDaGFubmVsSHViU2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxDaGFubmVsSHViVmlld01vZGVsPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IEFzc29jaWF0ZWREYXRhR2F0ZXdheTogRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgTGl2ZURlc2NyaXB0aW9uOiBLbm9ja291dENvbXB1dGVkPHN0cmluZz5cclxuXHJcbiAgICAvLyBjb25zdHJ1Y3RvcnNcclxuICAgIGNvbnN0cnVjdG9yKGRhdGFQb3J0TW9kZWw6IGFueSwgYXNzb2NpYXRlZERhdGFHYXRld2F5OiBEYXRhR2F0ZXdheVZpZXdNb2RlbEJhc2UpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5OYW1lID0ga28ub2JzZXJ2YWJsZShkYXRhUG9ydE1vZGVsLk5hbWUpXHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZSA9IGRhdGFQb3J0TW9kZWwuRGF0YVR5cGVcclxuICAgICAgICB0aGlzLkRhdGFEaXJlY3Rpb24gPSBkYXRhUG9ydE1vZGVsLkRhdGFEaXJlY3Rpb25cclxuICAgICAgICB0aGlzLkVuZGlhbm5lc3MgPSBkYXRhUG9ydE1vZGVsLkVuZGlhbm5lc3NcclxuXHJcbiAgICAgICAgdGhpcy5Jc1NlbGVjdGVkID0ga28ub2JzZXJ2YWJsZTxib29sZWFuPihmYWxzZSlcclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWRDaGFubmVsSHViU2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PENoYW5uZWxIdWJWaWV3TW9kZWw+KClcclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhR2F0ZXdheSA9IGFzc29jaWF0ZWREYXRhR2F0ZXdheVxyXG5cclxuICAgICAgICB0aGlzLkxpdmVEZXNjcmlwdGlvbiA9IGtvLmNvbXB1dGVkKCgpID0+XHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBsZXQgcmVzdWx0OiBzdHJpbmdcclxuXHJcbiAgICAgICAgICAgIHJlc3VsdCA9IFwiPGRpdiBjbGFzcz0ndGV4dC1sZWZ0Jz5cIiArIHRoaXMuTmFtZSgpICsgXCI8L2Rpdj48ZGl2IGNsYXNzPSd0ZXh0LWxlZnQnPlwiICsgRW51bWVyYXRpb25IZWxwZXIuR2V0RW51bUxvY2FsaXphdGlvbignT25lRGFzRGF0YVR5cGVFbnVtJywgdGhpcy5EYXRhVHlwZSkgKyBcIjwvZGl2PlwiXHJcblxyXG4gICAgICAgICAgICBpZiAodGhpcy5Bc3NvY2lhdGVkQ2hhbm5lbEh1YlNldCgpLmxlbmd0aCA+IDApXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZENoYW5uZWxIdWJTZXQoKS5mb3JFYWNoKGNoYW5uZWxIdWIgPT5cclxuICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICByZXN1bHQgKz0gXCI8L2JyID48ZGl2IGNsYXNzPSd0ZXh0LWxlZnQnPlwiICsgY2hhbm5lbEh1Yi5OYW1lKCkgKyBcIjwvZGl2PjxkaXYgY2xhc3M9J3RleHQtbGVmdCc+XCIgKyBFbnVtZXJhdGlvbkhlbHBlci5HZXRFbnVtTG9jYWxpemF0aW9uKCdPbmVEYXNEYXRhVHlwZUVudW0nLCBjaGFubmVsSHViLkRhdGFUeXBlKCkpICsgXCI8L2Rpdj5cIlxyXG4gICAgICAgICAgICAgICAgfSlcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgcmV0dXJuIHJlc3VsdFxyXG4gICAgICAgIH0pXHJcbiAgICB9XHJcblxyXG4gICAgLy8gbWV0aG9kc1xyXG4gICAgcHVibGljIEdldElkKCk6IHN0cmluZ1xyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLk5hbWUoKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb0Z1bGxRdWFsaWZpZWRJZGVudGlmaWVyKCk6IHN0cmluZ1xyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLkFzc29jaWF0ZWREYXRhR2F0ZXdheS5EZXNjcmlwdGlvbi5JZCArIFwiIChcIiArIHRoaXMuQXNzb2NpYXRlZERhdGFHYXRld2F5LkRlc2NyaXB0aW9uLkluc3RhbmNlSWQgKyBcIikgLyBcIiArIHRoaXMuR2V0SWQoKTtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgRXh0ZW5kTW9kZWwobW9kZWw6IGFueSlcclxuICAgIHtcclxuICAgICAgICAvL1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb01vZGVsKClcclxuICAgIHtcclxuICAgICAgICBsZXQgbW9kZWw6IGFueSA9IHtcclxuICAgICAgICAgICAgTmFtZTogPHN0cmluZz50aGlzLk5hbWUoKSxcclxuICAgICAgICAgICAgRGF0YVR5cGU6IDxPbmVEYXNEYXRhVHlwZUVudW0+dGhpcy5EYXRhVHlwZSxcclxuICAgICAgICAgICAgRGF0YURpcmVjdGlvbjogPERhdGFEaXJlY3Rpb25FbnVtPnRoaXMuRGF0YURpcmVjdGlvblxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5FeHRlbmRNb2RlbChtb2RlbClcclxuXHJcbiAgICAgICAgcmV0dXJuIG1vZGVsXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFJlc2V0QXNzb2NpYXRpb25zKG1haW50YWluV2Vha1JlZmVyZW5jZTogYm9vbGVhbilcclxuICAgIHtcclxuICAgICAgICBpZiAodGhpcy5Bc3NvY2lhdGVkQ2hhbm5lbEh1YlNldCgpLmxlbmd0aCA+IDApXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWRDaGFubmVsSHViU2V0KCkuZm9yRWFjaChjaGFubmVsSHViID0+IGNoYW5uZWxIdWIuUmVzZXRBc3NvY2lhdGlvbihtYWludGFpbldlYWtSZWZlcmVuY2UsIHRoaXMpKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxufSIsImFic3RyYWN0IGNsYXNzIFBsdWdpblZpZXdNb2RlbEJhc2Vcclxue1xyXG4gICAgcHVibGljIERlc2NyaXB0aW9uOiBQbHVnaW5EZXNjcmlwdGlvblZpZXdNb2RlbFxyXG4gICAgcHVibGljIFBsdWdpbklkZW50aWZpY2F0aW9uOiBQbHVnaW5JZGVudGlmaWNhdGlvblZpZXdNb2RlbFxyXG4gICAgcHVibGljIElzSW5TZXR0aW5nc01vZGU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxib29sZWFuPlxyXG5cclxuICAgIHByaXZhdGUgX21vZGVsOiBhbnlcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihwbHVnaW5TZXR0aW5nc01vZGVsOiBhbnksIHBsdWdpbklkZW50aWZpY2F0aW9uOiBQbHVnaW5JZGVudGlmaWNhdGlvblZpZXdNb2RlbClcclxuICAgIHtcclxuICAgICAgICB0aGlzLl9tb2RlbCA9IHBsdWdpblNldHRpbmdzTW9kZWxcclxuICAgICAgICB0aGlzLkRlc2NyaXB0aW9uID0gbmV3IFBsdWdpbkRlc2NyaXB0aW9uVmlld01vZGVsKHBsdWdpblNldHRpbmdzTW9kZWwuRGVzY3JpcHRpb24pXHJcbiAgICAgICAgdGhpcy5QbHVnaW5JZGVudGlmaWNhdGlvbiA9IHBsdWdpbklkZW50aWZpY2F0aW9uXHJcbiAgICAgICAgdGhpcy5Jc0luU2V0dGluZ3NNb2RlID0ga28ub2JzZXJ2YWJsZTxib29sZWFuPihmYWxzZSlcclxuICAgIH1cclxuXHJcbiAgICAvLyBtZXRob2RzXHJcbiAgICBwdWJsaWMgYWJzdHJhY3QgYXN5bmMgSW5pdGlhbGl6ZUFzeW5jKCk6IFByb21pc2U8YW55PlxyXG5cclxuICAgIHB1YmxpYyBTZW5kQWN0aW9uUmVxdWVzdCA9IGFzeW5jIChpbnN0YW5jZUlkOiBudW1iZXIsIG1ldGhvZE5hbWU6IHN0cmluZywgZGF0YTogYW55KSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiA8QWN0aW9uUmVzcG9uc2U+IGF3YWl0IENvbm5lY3Rpb25NYW5hZ2VyLkludm9rZVdlYkNsaWVudEh1YihcIlJlcXVlc3RBY3Rpb25cIiwgbmV3IEFjdGlvblJlcXVlc3QodGhpcy5EZXNjcmlwdGlvbi5JZCwgaW5zdGFuY2VJZCwgbWV0aG9kTmFtZSwgZGF0YSkpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEV4dGVuZE1vZGVsKG1vZGVsOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgLy9cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IG1vZGVsOiBhbnkgPSB7XHJcbiAgICAgICAgICAgICR0eXBlOiA8c3RyaW5nPnRoaXMuX21vZGVsLiR0eXBlLFxyXG4gICAgICAgICAgICBEZXNjcmlwdGlvbjogPFBsdWdpbkRlc2NyaXB0aW9uVmlld01vZGVsPnRoaXMuRGVzY3JpcHRpb24uVG9Nb2RlbCgpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0aGlzLkV4dGVuZE1vZGVsKG1vZGVsKVxyXG5cclxuICAgICAgICByZXR1cm4gbW9kZWxcclxuICAgIH1cclxuXHJcbiAgICAvLyBjb21tYW5kc1xyXG4gICAgcHVibGljIEVuYWJsZVNldHRpbmdzTW9kZSA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5Jc0luU2V0dGluZ3NNb2RlKHRydWUpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIERpc2FibGVTZXR0aW5nc01vZGUgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuSXNJblNldHRpbmdzTW9kZShmYWxzZSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9nZ2xlU2V0dGluZ3NNb2RlID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLklzSW5TZXR0aW5nc01vZGUoIXRoaXMuSXNJblNldHRpbmdzTW9kZSgpKVxyXG4gICAgfVxyXG59IiwiLy8vIDxyZWZlcmVuY2UgcGF0aD1cIlBsdWdpblZpZXdNb2RlbEJhc2UudHNcIi8+XHJcblxyXG5hYnN0cmFjdCBjbGFzcyBEYXRhR2F0ZXdheVZpZXdNb2RlbEJhc2UgZXh0ZW5kcyBQbHVnaW5WaWV3TW9kZWxCYXNlXHJcbntcclxuICAgIHB1YmxpYyByZWFkb25seSBNYXhpbXVtRGF0YXNldEFnZTogS25vY2tvdXRPYnNlcnZhYmxlPG51bWJlcj5cclxuICAgIHB1YmxpYyByZWFkb25seSBEYXRhUG9ydFNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8RGF0YVBvcnRWaWV3TW9kZWw+XHJcblxyXG4gICAgY29uc3RydWN0b3IobW9kZWwsIGlkZW50aWZpY2F0aW9uOiBQbHVnaW5JZGVudGlmaWNhdGlvblZpZXdNb2RlbClcclxuICAgIHtcclxuICAgICAgICBzdXBlcihtb2RlbCwgaWRlbnRpZmljYXRpb24pXHJcblxyXG4gICAgICAgIHRoaXMuTWF4aW11bURhdGFzZXRBZ2UgPSBrby5vYnNlcnZhYmxlPG51bWJlcj4obW9kZWwuTWF4aW11bURhdGFzZXRBZ2UpXHJcbiAgICAgICAgdGhpcy5EYXRhUG9ydFNldCA9IGtvLm9ic2VydmFibGVBcnJheTxEYXRhUG9ydFZpZXdNb2RlbD4oKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBFeHRlbmRNb2RlbChtb2RlbDogYW55KVxyXG4gICAge1xyXG4gICAgICAgIHN1cGVyLkV4dGVuZE1vZGVsKG1vZGVsKVxyXG5cclxuICAgICAgICBtb2RlbC5NYXhpbXVtRGF0YXNldEFnZSA9IDxudW1iZXI+TnVtYmVyLnBhcnNlSW50KDxhbnk+dGhpcy5NYXhpbXVtRGF0YXNldEFnZSgpKVxyXG4gICAgfVxyXG59IiwiYWJzdHJhY3QgY2xhc3MgRXh0ZW5kZWREYXRhR2F0ZXdheVZpZXdNb2RlbEJhc2UgZXh0ZW5kcyBEYXRhR2F0ZXdheVZpZXdNb2RlbEJhc2Vcclxue1xyXG4gICAgcHVibGljIE1vZHVsZVRvRGF0YVBvcnRNYXA6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PE9ic2VydmFibGVHcm91cDxEYXRhUG9ydFZpZXdNb2RlbD4+XHJcbiAgICBwdWJsaWMgT25lRGFzTW9kdWxlU2VsZWN0b3I6IEtub2Nrb3V0T2JzZXJ2YWJsZTxPbmVEYXNNb2R1bGVTZWxlY3RvclZpZXdNb2RlbD5cclxuXHJcbiAgICBjb25zdHJ1Y3Rvcihtb2RlbCwgaWRlbnRpZmljYXRpb246IFBsdWdpbklkZW50aWZpY2F0aW9uVmlld01vZGVsLCBvbmVEYXNNb2R1bGVTZWxlY3RvcjogT25lRGFzTW9kdWxlU2VsZWN0b3JWaWV3TW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgc3VwZXIobW9kZWwsIGlkZW50aWZpY2F0aW9uKVxyXG5cclxuICAgICAgICB0aGlzLk1vZHVsZVRvRGF0YVBvcnRNYXAgPSBrby5vYnNlcnZhYmxlQXJyYXkoKVxyXG4gICAgICAgIHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3IgPSBrby5vYnNlcnZhYmxlPE9uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsPihvbmVEYXNNb2R1bGVTZWxlY3RvcilcclxuXHJcbiAgICAgICAgaWYgKHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3IoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3IoKS5Pbk1vZHVsZVNldENoYW5nZWQuc3Vic2NyaWJlKChzZW5kZXIsIGFyZ3MpID0+XHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIHRoaXMuVXBkYXRlRGF0YVBvcnRTZXQoKVxyXG4gICAgICAgICAgICB9KVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgYXN5bmMgSW5pdGlhbGl6ZUFzeW5jKClcclxuICAgIHtcclxuICAgICAgICB0aGlzLlVwZGF0ZURhdGFQb3J0U2V0KClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVXBkYXRlRGF0YVBvcnRTZXQoKVxyXG4gICAge1xyXG4gICAgICAgIGxldCBpbmRleDogbnVtYmVyXHJcbiAgICAgICAgbGV0IG1vZHVsZVRvRGF0YVBvcnRNYXA6IE9ic2VydmFibGVHcm91cDxEYXRhUG9ydFZpZXdNb2RlbD5bXVxyXG5cclxuICAgICAgICBtb2R1bGVUb0RhdGFQb3J0TWFwID0gW11cclxuXHJcbiAgICAgICAgLy8gaW5wdXRzXHJcbiAgICAgICAgaW5kZXggPSAwXHJcblxyXG4gICAgICAgIG1vZHVsZVRvRGF0YVBvcnRNYXAgPSBtb2R1bGVUb0RhdGFQb3J0TWFwLmNvbmNhdCh0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yKCkuTW9kdWxlU2V0KCkuZmlsdGVyKG9uZURhc01vZHVsZSA9PiBvbmVEYXNNb2R1bGUuRGF0YURpcmVjdGlvbigpID09PSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dCkubWFwKG9uZURhc01vZHVsZSA9PlxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgbGV0IGdyb3VwOiBPYnNlcnZhYmxlR3JvdXA8RGF0YVBvcnRWaWV3TW9kZWw+XHJcblxyXG4gICAgICAgICAgICBncm91cCA9IG5ldyBPYnNlcnZhYmxlR3JvdXA8RGF0YVBvcnRWaWV3TW9kZWw+KG9uZURhc01vZHVsZS5Ub1N0cmluZygpLCB0aGlzLkNyZWF0ZURhdGFQb3J0U2V0KG9uZURhc01vZHVsZSwgaW5kZXgpKVxyXG4gICAgICAgICAgICBpbmRleCArPSBvbmVEYXNNb2R1bGUuU2l6ZSgpO1xyXG5cclxuICAgICAgICAgICAgcmV0dXJuIGdyb3VwXHJcbiAgICAgICAgfSkpXHJcblxyXG4gICAgICAgIC8vIG91dHB1dHNcclxuICAgICAgICBpbmRleCA9IDBcclxuXHJcbiAgICAgICAgbW9kdWxlVG9EYXRhUG9ydE1hcCA9IG1vZHVsZVRvRGF0YVBvcnRNYXAuY29uY2F0KHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3IoKS5Nb2R1bGVTZXQoKS5maWx0ZXIob25lRGFzTW9kdWxlID0+IG9uZURhc01vZHVsZS5EYXRhRGlyZWN0aW9uKCkgPT09IERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dCkubWFwKG9uZURhc01vZHVsZSA9PlxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgbGV0IGdyb3VwOiBPYnNlcnZhYmxlR3JvdXA8RGF0YVBvcnRWaWV3TW9kZWw+XHJcblxyXG4gICAgICAgICAgICBncm91cCA9IG5ldyBPYnNlcnZhYmxlR3JvdXA8RGF0YVBvcnRWaWV3TW9kZWw+KG9uZURhc01vZHVsZS5Ub1N0cmluZygpLCB0aGlzLkNyZWF0ZURhdGFQb3J0U2V0KG9uZURhc01vZHVsZSwgaW5kZXgpKVxyXG4gICAgICAgICAgICBpbmRleCArPSBvbmVEYXNNb2R1bGUuU2l6ZSgpO1xyXG5cclxuICAgICAgICAgICAgcmV0dXJuIGdyb3VwXHJcbiAgICAgICAgfSkpXHJcblxyXG4gICAgICAgIHRoaXMuTW9kdWxlVG9EYXRhUG9ydE1hcChtb2R1bGVUb0RhdGFQb3J0TWFwKVxyXG4gICAgICAgIHRoaXMuRGF0YVBvcnRTZXQoTWFwTWFueSh0aGlzLk1vZHVsZVRvRGF0YVBvcnRNYXAoKSwgZ3JvdXAgPT4gZ3JvdXAuTWVtYmVycygpKSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgQ3JlYXRlRGF0YVBvcnRTZXQob25lRGFzTW9kdWxlOiBPbmVEYXNNb2R1bGVWaWV3TW9kZWwsIGluZGV4OiBudW1iZXIpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IHByZWZpeDogc3RyaW5nXHJcblxyXG4gICAgICAgIHN3aXRjaCAob25lRGFzTW9kdWxlLkRhdGFEaXJlY3Rpb24oKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uSW5wdXQ6XHJcbiAgICAgICAgICAgICAgICBwcmVmaXggPSBcIklucHV0XCJcclxuICAgICAgICAgICAgICAgIGJyZWFrXHJcblxyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dDpcclxuICAgICAgICAgICAgICAgIHByZWZpeCA9IFwiT3V0cHV0XCJcclxuICAgICAgICAgICAgICAgIGJyZWFrXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gQXJyYXkuZnJvbShuZXcgQXJyYXkob25lRGFzTW9kdWxlLlNpemUoKSksICh4LCBpKSA9PiBcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHJldHVybiB7XHJcbiAgICAgICAgICAgICAgICBOYW1lOiA8c3RyaW5nPnByZWZpeCArIFwiIFwiICsgKGluZGV4ICsgaSksXHJcbiAgICAgICAgICAgICAgICBEYXRhVHlwZTogPE9uZURhc0RhdGFUeXBlRW51bT5vbmVEYXNNb2R1bGUuRGF0YVR5cGUoKSxcclxuICAgICAgICAgICAgICAgIERhdGFEaXJlY3Rpb246IDxEYXRhRGlyZWN0aW9uRW51bT5vbmVEYXNNb2R1bGUuRGF0YURpcmVjdGlvbigpXHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9KS5tYXAoZGF0YVBvcnRNb2RlbCA9PiBuZXcgRGF0YVBvcnRWaWV3TW9kZWwoZGF0YVBvcnRNb2RlbCwgdGhpcykpXHJcbiAgICB9XHJcbn0iLCIvLy8gPHJlZmVyZW5jZSBwYXRoPVwiUGx1Z2luVmlld01vZGVsQmFzZS50c1wiLz5cclxuXHJcbmFic3RyYWN0IGNsYXNzIERhdGFXcml0ZXJWaWV3TW9kZWxCYXNlIGV4dGVuZHMgUGx1Z2luVmlld01vZGVsQmFzZVxyXG57XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRmlsZUdyYW51bGFyaXR5OiBLbm9ja291dE9ic2VydmFibGU8RmlsZUdyYW51bGFyaXR5RW51bT5cclxuICAgIHB1YmxpYyByZWFkb25seSBCdWZmZXJSZXF1ZXN0U2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxCdWZmZXJSZXF1ZXN0Vmlld01vZGVsPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IEJ1ZmZlclJlcXVlc3RTZWxlY3RvcjogS25vY2tvdXRPYnNlcnZhYmxlPEJ1ZmZlclJlcXVlc3RTZWxlY3RvclZpZXdNb2RlbD5cclxuXHJcbiAgICBjb25zdHJ1Y3Rvcihtb2RlbCwgaWRlbnRpZmljYXRpb246IFBsdWdpbklkZW50aWZpY2F0aW9uVmlld01vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHN1cGVyKG1vZGVsLCBpZGVudGlmaWNhdGlvbilcclxuXHJcbiAgICAgICAgdGhpcy5GaWxlR3JhbnVsYXJpdHkgPSBrby5vYnNlcnZhYmxlPEZpbGVHcmFudWxhcml0eUVudW0+KG1vZGVsLkZpbGVHcmFudWxhcml0eSlcclxuICAgICAgICB0aGlzLkJ1ZmZlclJlcXVlc3RTZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8QnVmZmVyUmVxdWVzdFZpZXdNb2RlbD4obW9kZWwuQnVmZmVyUmVxdWVzdFNldC5tYXAoYnVmZmVyUmVxdWVzdCA9PiBuZXcgQnVmZmVyUmVxdWVzdFZpZXdNb2RlbChidWZmZXJSZXF1ZXN0KSkpXHJcblxyXG4gICAgICAgIHRoaXMuQnVmZmVyUmVxdWVzdFNlbGVjdG9yID0ga28ub2JzZXJ2YWJsZTxCdWZmZXJSZXF1ZXN0U2VsZWN0b3JWaWV3TW9kZWw+KG5ldyBCdWZmZXJSZXF1ZXN0U2VsZWN0b3JWaWV3TW9kZWwodGhpcy5CdWZmZXJSZXF1ZXN0U2V0KCkpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBFeHRlbmRNb2RlbChtb2RlbDogYW55KVxyXG4gICAge1xyXG4gICAgICAgIHN1cGVyLkV4dGVuZE1vZGVsKG1vZGVsKVxyXG5cclxuICAgICAgICBtb2RlbC5GaWxlR3JhbnVsYXJpdHkgPSA8RmlsZUdyYW51bGFyaXR5RW51bT50aGlzLkZpbGVHcmFudWxhcml0eSgpXHJcbiAgICAgICAgbW9kZWwuQnVmZmVyUmVxdWVzdFNldCA9IDxCdWZmZXJSZXF1ZXN0TW9kZWxbXT50aGlzLkJ1ZmZlclJlcXVlc3RTZXQoKS5tYXAoYnVmZmVyUmVxdWVzdCA9PiBidWZmZXJSZXF1ZXN0LlRvTW9kZWwoKSlcclxuICAgIH1cclxufSIsImNsYXNzIFBsdWdpbkRlc2NyaXB0aW9uVmlld01vZGVsXHJcbntcclxuICAgIHB1YmxpYyBQcm9kdWN0VmVyc2lvbjogbnVtYmVyXHJcbiAgICBwdWJsaWMgSWQ6IHN0cmluZ1xyXG4gICAgcHVibGljIEluc3RhbmNlSWQ6IG51bWJlclxyXG4gICAgcHVibGljIEluc3RhbmNlTmFtZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBJc0VuYWJsZWQ6IEtub2Nrb3V0T2JzZXJ2YWJsZTxib29sZWFuPlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKHBsdWdpbkRlc2NyaXB0aW9uTW9kZWw6IGFueSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLlByb2R1Y3RWZXJzaW9uID0gcGx1Z2luRGVzY3JpcHRpb25Nb2RlbC5Qcm9kdWN0VmVyc2lvblxyXG4gICAgICAgIHRoaXMuSWQgPSBwbHVnaW5EZXNjcmlwdGlvbk1vZGVsLklkXHJcbiAgICAgICAgdGhpcy5JbnN0YW5jZUlkID0gcGx1Z2luRGVzY3JpcHRpb25Nb2RlbC5JbnN0YW5jZUlkXHJcbiAgICAgICAgdGhpcy5JbnN0YW5jZU5hbWUgPSBrby5vYnNlcnZhYmxlPHN0cmluZz4ocGx1Z2luRGVzY3JpcHRpb25Nb2RlbC5JbnN0YW5jZU5hbWUpXHJcbiAgICAgICAgdGhpcy5Jc0VuYWJsZWQgPSBrby5vYnNlcnZhYmxlPGJvb2xlYW4+KHBsdWdpbkRlc2NyaXB0aW9uTW9kZWwuSXNFbmFibGVkKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb01vZGVsKClcclxuICAgIHtcclxuICAgICAgICB2YXIgbW9kZWw6IGFueSA9IHtcclxuICAgICAgICAgICAgUHJvZHVjdFZlcnNpb246IDxudW1iZXI+dGhpcy5Qcm9kdWN0VmVyc2lvbixcclxuICAgICAgICAgICAgSWQ6IDxzdHJpbmc+dGhpcy5JZCxcclxuICAgICAgICAgICAgSW5zdGFuY2VJZDogPG51bWJlcj50aGlzLkluc3RhbmNlSWQsXHJcbiAgICAgICAgICAgIEluc3RhbmNlTmFtZTogPHN0cmluZz50aGlzLkluc3RhbmNlTmFtZSgpLFxyXG4gICAgICAgICAgICBJc0VuYWJsZWQ6IDxib29sZWFuPnRoaXMuSXNFbmFibGVkKClcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJldHVybiBtb2RlbFxyXG4gICAgfVxyXG59IiwiY2xhc3MgUGx1Z2luSWRlbnRpZmljYXRpb25WaWV3TW9kZWxcclxue1xyXG4gICAgcHVibGljIFByb2R1Y3RWZXJzaW9uOiBzdHJpbmdcclxuICAgIHB1YmxpYyBJZDogc3RyaW5nXHJcbiAgICBwdWJsaWMgTmFtZTogc3RyaW5nXHJcbiAgICBwdWJsaWMgRGVzY3JpcHRpb246IHN0cmluZ1xyXG4gICAgcHVibGljIFZpZXdSZXNvdXJjZU5hbWU6IHN0cmluZ1xyXG4gICAgcHVibGljIFZpZXdNb2RlbFJlc291cmNlTmFtZTogc3RyaW5nXHJcblxyXG4gICAgY29uc3RydWN0b3IocGx1Z2luSWRlbnRpZmljYXRpb25Nb2RlbDogYW55KVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuUHJvZHVjdFZlcnNpb24gPSBwbHVnaW5JZGVudGlmaWNhdGlvbk1vZGVsLlByb2R1Y3RWZXJzaW9uXHJcbiAgICAgICAgdGhpcy5JZCA9IHBsdWdpbklkZW50aWZpY2F0aW9uTW9kZWwuSWRcclxuICAgICAgICB0aGlzLk5hbWUgPSBwbHVnaW5JZGVudGlmaWNhdGlvbk1vZGVsLk5hbWVcclxuICAgICAgICB0aGlzLkRlc2NyaXB0aW9uID0gcGx1Z2luSWRlbnRpZmljYXRpb25Nb2RlbC5EZXNjcmlwdGlvblxyXG4gICAgICAgIHRoaXMuVmlld1Jlc291cmNlTmFtZSA9IHBsdWdpbklkZW50aWZpY2F0aW9uTW9kZWwuVmlld1Jlc291cmNlTmFtZVxyXG4gICAgICAgIHRoaXMuVmlld01vZGVsUmVzb3VyY2VOYW1lID0gcGx1Z2luSWRlbnRpZmljYXRpb25Nb2RlbC5WaWV3TW9kZWxSZXNvdXJjZU5hbWVcclxuICAgIH1cclxufSJdfQ==