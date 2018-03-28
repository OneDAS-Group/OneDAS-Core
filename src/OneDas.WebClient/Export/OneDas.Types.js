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
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiT25lRGFzLlR5cGVzLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vT25lRGFzLlR5cGVzL0RhdGFEaXJlY3Rpb25FbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL0VuZGlhbm5lc3NFbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL0ZpbGVHcmFudWxhcml0eUVudW0udHMiLCIuLi9PbmVEYXMuVHlwZXMvT25lRGFzRGF0YVR5cGVFbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL09uZURhc1N0YXRlRW51bS50cyIsIi4uL09uZURhcy5UeXBlcy9TYW1wbGVSYXRlRW51bS50cyIsIi4uL0NvcmUvQWN0aW9uUmVxdWVzdC50cyIsIi4uL0NvcmUvQWN0aW9uUmVzcG9uc2UudHMiLCIuLi9Db3JlL0V2ZW50RGlzcGF0Y2hlci50cyIsIi4uL0NvcmUvSUV2ZW50LnRzIiwiLi4vQ29yZS9PbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtLnRzIiwiLi4vTW9kZWxzL0J1ZmZlclJlcXVlc3RNb2RlbC50cyIsIi4uL01vZGVscy9DaGFubmVsSHViTW9kZWwudHMiLCIuLi9Nb2RlbHMvT25lRGFzTW9kdWxlTW9kZWwudHMiLCIuLi9Nb2RlbHMvVHJhbnNmZXJGdW5jdGlvbk1vZGVsLnRzIiwiLi4vU3RhdGljL0Nvbm5lY3Rpb25NYW5hZ2VyLnRzIiwiLi4vU3RhdGljL0VudW1lcmF0aW9uSGVscGVyLnRzIiwiLi4vU3RhdGljL0Vycm9yTWVzc2FnZS50cyIsIi4uL1N0YXRpYy9IZWxwZXIudHMiLCIuLi9TdGF0aWMvUGx1Z2luRmFjdG9yeS50cyIsIi4uL1N0YXRpYy9QbHVnaW5IaXZlLnRzIiwiLi4vVmlld01vZGVscy9Db3JlL0NoYW5uZWxIdWJWaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL0NvcmUvT25lRGFzTW9kdWxlVmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9Db3JlL09uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9Db3JlL1RyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL1BsdWdpbi9CdWZmZXJSZXF1ZXN0U2VsZWN0b3JWaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL1BsdWdpbi9CdWZmZXJSZXF1ZXN0Vmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vRGF0YVBvcnRWaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL1BsdWdpbi9QbHVnaW5WaWV3TW9kZWxCYXNlLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vRXh0ZW5kZWREYXRhR2F0ZXdheVZpZXdNb2RlbEJhc2UudHMiLCIuLi9WaWV3TW9kZWxzL1BsdWdpbi9EYXRhV3JpdGVyVmlld01vZGVsQmFzZS50cyIsIi4uL1ZpZXdNb2RlbHMvUGx1Z2luL1BsdWdpbkRlc2NyaXB0aW9uVmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vUGx1Z2luSWRlbnRpZmljYXRpb25WaWV3TW9kZWwudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsSUFBSyxpQkFJSjtBQUpELFdBQUssaUJBQWlCO0lBRWxCLDJEQUFTLENBQUE7SUFDVCw2REFBVSxDQUFBO0FBQ2QsQ0FBQyxFQUpJLGlCQUFpQixLQUFqQixpQkFBaUIsUUFJckI7QUNKRCxJQUFLLGNBSUo7QUFKRCxXQUFLLGNBQWM7SUFFZixtRUFBZ0IsQ0FBQTtJQUNoQiw2REFBYSxDQUFBO0FBQ2pCLENBQUMsRUFKSSxjQUFjLEtBQWQsY0FBYyxRQUlsQjtBQ0pELElBQUssbUJBTUo7QUFORCxXQUFLLG1CQUFtQjtJQUVwQixzRUFBYSxDQUFBO0lBQ2IseUVBQWUsQ0FBQTtJQUNmLGdFQUFXLENBQUE7SUFDWCwrREFBVyxDQUFBO0FBQ2YsQ0FBQyxFQU5JLG1CQUFtQixLQUFuQixtQkFBbUIsUUFNdkI7QUNORCxJQUFLLGtCQVdKO0FBWEQsV0FB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QUFFLENBQUMsVUFBVSxDQUFTLHNCQUFzQixDQUFDLFlBQVksQ0FBQyxDQUFBO1FBQzlFLElBQUksQ0FBQyxTQUFTLEdBQUcsRUFBRSxDQUFDLFVBQVUsQ0FBVSxzQkFBc0IsQ0FBQyxTQUFTLENBQUMsQ0FBQTtJQUM3RSxDQUFDO0lBRU0sT0FBTztRQUVWLElBQUksS0FBSyxHQUFRO1lBQ2IsY0FBYyxFQUFVLElBQUksQ0FBQyxjQUFjO1lBQzNDLEVBQUUsRUFBVSxJQUFJLENBQUMsRUFBRTtZQUNuQixVQUFVLEVBQVUsSUFBSSxDQUFDLFVBQVU7WUFDbkMsWUFBWSxFQUFVLElBQUksQ0FBQyxZQUFZLEVBQUU7WUFDekMsU0FBUyxFQUFXLElBQUksQ0FBQyxTQUFTLEVBQUU7U0FDdkMsQ0FBQTtRQUVELE1BQU0sQ0FBQyxLQUFLLENBQUE7SUFDaEIsQ0FBQztDQUNKO0FDN0JEO0lBU0ksWUFBWSx5QkFBOEI7UUFFdEMsSUFBSSxDQUFDLGNBQWMsR0FBRyx5QkFBeUIsQ0FBQyxjQUFjLENBQUE7UUFDOUQsSUFBSSxDQUFDLEVBQUUsR0FBRyx5QkFBeUIsQ0FBQyxFQUFFLENBQUE7UUFDdEMsSUFBSSxDQUFDLElBQUksR0FBRyx5QkFBeUIsQ0FBQyxJQUFJLENBQUE7UUFDMUMsSUFBSSxDQUFDLFdBQVcsR0FBRyx5QkFBeUIsQ0FBQyxXQUFXLENBQUE7UUFDeEQsSUFBSSxDQUFDLGdCQUFnQixHQUFHLHlCQUF5QixDQUFDLGdCQUFnQixDQUFBO1FBQ2xFLElBQUksQ0FBQyxxQkFBcUIsR0FBRyx5QkFBeUIsQ0FBQyxxQkFBcUIsQ0FBQTtJQUNoRixDQUFDO0NBQ0oiLCJzb3VyY2VzQ29udGVudCI6WyJlbnVtIERhdGFEaXJlY3Rpb25FbnVtXHJcbntcclxuICAgIElucHV0ID0gMSxcclxuICAgIE91dHB1dCA9IDJcclxufSIsImVudW0gRW5kaWFubmVzc0VudW1cclxue1xyXG4gICAgTGl0dGxlRW5kaWFuID0gMSxcclxuICAgIEJpZ0VuZGlhbiA9IDJcclxufSIsImVudW0gRmlsZUdyYW51bGFyaXR5RW51bVxyXG57XHJcbiAgICBNaW51dGVfMSA9IDYwLFxyXG4gICAgTWludXRlXzEwID0gNjAwLFxyXG4gICAgSG91ciA9IDM2MDAsXHJcbiAgICBEYXkgPSA4NjQwMFxyXG59IiwiZW51bSBPbmVEYXNEYXRhVHlwZUVudW1cclxue1xyXG4gICAgQk9PTEVBTiA9IDB4MDA4LFxyXG4gICAgVUlOVDggPSAweDEwOCxcclxuICAgIElOVDggPSAweDIwOCxcclxuICAgIFVJTlQxNiA9IDB4MTEwLFxyXG4gICAgSU5UMTYgPSAweDIxMCxcclxuICAgIFVJTlQzMiA9IDB4MTIwLFxyXG4gICAgSU5UMzIgPSAweDIyMCxcclxuICAgIEZMT0FUMzIgPSAweDMyMCxcclxuICAgIEZMT0FUNjQgPSAweDM0MFxyXG59IiwiZW51bSBPbmVEYXNTdGF0ZUVudW1cclxue1xyXG4gICAgRXJyb3IgPSAxLFxyXG4gICAgSW5pdGlhbGl6YXRpb24gPSAyLFxyXG4gICAgSWRsZSA9IDMsXHJcbiAgICBBcHBseUNvbmZpZ3VyYXRpb24gPSA0LFxyXG4gICAgUmVhZHkgPSA1LFxyXG4gICAgUnVuID0gNlxyXG59IiwiZW51bSBTYW1wbGVSYXRlRW51bVxyXG57XHJcbiAgICBTYW1wbGVSYXRlXzEwMCA9IDEsXHJcbiAgICBTYW1wbGVSYXRlXzI1ID0gNCxcclxuICAgIFNhbXBsZVJhdGVfNSA9IDIwLFxyXG4gICAgU2FtcGxlUmF0ZV8xID0gMTAwXHJcbn0iLCJjbGFzcyBBY3Rpb25SZXF1ZXN0XHJcbntcclxuICAgIHB1YmxpYyByZWFkb25seSBQbHVnaW5JZDogc3RyaW5nXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgSW5zdGFuY2VJZDogbnVtYmVyXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgTWV0aG9kTmFtZTogc3RyaW5nXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRGF0YTogYW55XHJcblxyXG4gICAgY29uc3RydWN0b3IocGx1Z2luSWQ6IHN0cmluZywgaW5zdGFuY2VJZDogbnVtYmVyLCBtZXRob2ROYW1lOiBzdHJpbmcsIGRhdGE6IGFueSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLlBsdWdpbklkID0gcGx1Z2luSWQ7XHJcbiAgICAgICAgdGhpcy5JbnN0YW5jZUlkID0gaW5zdGFuY2VJZDtcclxuICAgICAgICB0aGlzLk1ldGhvZE5hbWUgPSBtZXRob2ROYW1lO1xyXG4gICAgICAgIHRoaXMuRGF0YSA9IGRhdGE7XHJcbiAgICB9XHJcbn0iLCJjbGFzcyBBY3Rpb25SZXNwb25zZVxyXG57XHJcbiAgICBwdWJsaWMgRGF0YTogYW55XHJcblxyXG4gICAgY29uc3RydWN0b3IoZGF0YTogYW55KVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuRGF0YSA9IGRhdGE7XHJcbiAgICB9XHJcbn0iLCJjbGFzcyBFdmVudERpc3BhdGNoZXI8VFNlbmRlciwgVEFyZ3M+IGltcGxlbWVudHMgSUV2ZW50PFRTZW5kZXIsIFRBcmdzPlxyXG57XHJcbiAgICBwcml2YXRlIF9zdWJzY3JpcHRpb25zOiBBcnJheTwoc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncykgPT4gdm9pZD4gPSBuZXcgQXJyYXk8KHNlbmRlcjogVFNlbmRlciwgYXJnczogVEFyZ3MpID0+IHZvaWQ+KCk7XHJcblxyXG4gICAgc3Vic2NyaWJlKGZuOiAoc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncykgPT4gdm9pZCk6IHZvaWRcclxuICAgIHtcclxuICAgICAgICBpZiAoZm4pXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLl9zdWJzY3JpcHRpb25zLnB1c2goZm4pO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICB1bnN1YnNjcmliZShmbjogKHNlbmRlcjogVFNlbmRlciwgYXJnczogVEFyZ3MpID0+IHZvaWQpOiB2b2lkXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IGkgPSB0aGlzLl9zdWJzY3JpcHRpb25zLmluZGV4T2YoZm4pO1xyXG5cclxuICAgICAgICBpZiAoaSA+IC0xKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5fc3Vic2NyaXB0aW9ucy5zcGxpY2UoaSwgMSk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIGRpc3BhdGNoKHNlbmRlcjogVFNlbmRlciwgYXJnczogVEFyZ3MpOiB2b2lkXHJcbiAgICB7XHJcbiAgICAgICAgZm9yIChsZXQgaGFuZGxlciBvZiB0aGlzLl9zdWJzY3JpcHRpb25zKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgaGFuZGxlcihzZW5kZXIsIGFyZ3MpO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxufSIsImludGVyZmFjZSBJRXZlbnQ8VFNlbmRlciwgVEFyZ3M+XHJcbntcclxuICAgIHN1YnNjcmliZShmbjogKHNlbmRlcjogVFNlbmRlciwgYXJnczogVEFyZ3MpID0+IHZvaWQpOiB2b2lkO1xyXG4gICAgdW5zdWJzY3JpYmUoZm46IChzZW5kZXI6IFRTZW5kZXIsIGFyZ3M6IFRBcmdzKSA9PiB2b2lkKTogdm9pZDtcclxufSIsImVudW0gT25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlRW51bVxyXG57XHJcbiAgICBEdXBsZXggPSAxLFxyXG4gICAgSW5wdXRPbmx5ID0gMixcclxuICAgIE91dHB1dE9ubHkgPSAzLFxyXG59IiwiY2xhc3MgQnVmZmVyUmVxdWVzdE1vZGVsXHJcbntcclxuICAgIHB1YmxpYyBTYW1wbGVSYXRlOiBTYW1wbGVSYXRlRW51bVxyXG4gICAgcHVibGljIEdyb3VwRmlsdGVyOiBzdHJpbmdcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihzYW1wbGVSYXRlOiBTYW1wbGVSYXRlRW51bSwgZ3JvdXBGaWx0ZXI6IHN0cmluZylcclxuICAgIHtcclxuICAgICAgICB0aGlzLlNhbXBsZVJhdGUgPSBzYW1wbGVSYXRlO1xyXG4gICAgICAgIHRoaXMuR3JvdXBGaWx0ZXIgPSBncm91cEZpbHRlcjtcclxuICAgIH1cclxufSIsImNsYXNzIENoYW5uZWxIdWJNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgTmFtZTogc3RyaW5nXHJcbiAgICBwdWJsaWMgR3JvdXA6IHN0cmluZ1xyXG4gICAgcHVibGljIERhdGFUeXBlOiBPbmVEYXNEYXRhVHlwZUVudW1cclxuICAgIHB1YmxpYyBHdWlkOiBzdHJpbmdcclxuICAgIHB1YmxpYyBDcmVhdGlvbkRhdGVUaW1lOiBzdHJpbmdcclxuICAgIHB1YmxpYyBVbml0OiBzdHJpbmdcclxuICAgIHB1YmxpYyBUcmFuc2ZlckZ1bmN0aW9uU2V0OiBhbnlbXVxyXG4gICAgcHVibGljIEFzc29jaWF0ZWREYXRhSW5wdXRJZDogc3RyaW5nXHJcbiAgICBwdWJsaWMgQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldDogc3RyaW5nW11cclxuXHJcbiAgICBjb25zdHJ1Y3RvcihuYW1lOiBzdHJpbmcsIGdyb3VwOiBzdHJpbmcsIGRhdGFUeXBlOiBPbmVEYXNEYXRhVHlwZUVudW0pXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5OYW1lID0gbmFtZTtcclxuICAgICAgICB0aGlzLkdyb3VwID0gZ3JvdXA7XHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZSA9IGRhdGFUeXBlO1xyXG4gICAgICAgIHRoaXMuR3VpZCA9IEd1aWQuTmV3R3VpZCgpXHJcbiAgICAgICAgdGhpcy5DcmVhdGlvbkRhdGVUaW1lID0gbmV3IERhdGUoKS50b0lTT1N0cmluZygpXHJcbiAgICAgICAgdGhpcy5Vbml0ID0gXCJcIlxyXG4gICAgICAgIHRoaXMuVHJhbnNmZXJGdW5jdGlvblNldCA9IFtdXHJcbiAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0SWQgPSBcIlwiXHJcbiAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0ID0gW11cclxuICAgIH1cclxufSIsImNsYXNzIE9uZURhc01vZHVsZU1vZGVsXHJcbntcclxuICAgIHB1YmxpYyBEYXRhVHlwZTogT25lRGFzRGF0YVR5cGVFbnVtXHJcbiAgICBwdWJsaWMgRGF0YURpcmVjdGlvbjogRGF0YURpcmVjdGlvbkVudW1cclxuICAgIHB1YmxpYyBFbmRpYW5uZXNzOiBFbmRpYW5uZXNzRW51bVxyXG4gICAgcHVibGljIFNpemU6IG51bWJlclxyXG5cclxuICAgIGNvbnN0cnVjdG9yKGRhdGFUeXBlOiBPbmVEYXNEYXRhVHlwZUVudW0sIGRhdGFEaXJlY3Rpb246IERhdGFEaXJlY3Rpb25FbnVtLCBlbmRpYW5uZXNzOiBFbmRpYW5uZXNzRW51bSwgc2l6ZTogbnVtYmVyKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuRGF0YVR5cGUgPSBkYXRhVHlwZVxyXG4gICAgICAgIHRoaXMuRGF0YURpcmVjdGlvbiA9IGRhdGFEaXJlY3Rpb25cclxuICAgICAgICB0aGlzLkVuZGlhbm5lc3MgPSBlbmRpYW5uZXNzXHJcbiAgICAgICAgdGhpcy5TaXplID0gc2l6ZVxyXG4gICAgfVxyXG59XHJcbiIsImNsYXNzIFRyYW5zZmVyRnVuY3Rpb25Nb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgRGF0ZVRpbWU6IHN0cmluZ1xyXG4gICAgcHVibGljIFR5cGU6IHN0cmluZ1xyXG4gICAgcHVibGljIE9wdGlvbjogc3RyaW5nXHJcbiAgICBwdWJsaWMgQXJndW1lbnQ6IHN0cmluZ1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKGRhdGVUaW1lOiBzdHJpbmcsIHR5cGU6IHN0cmluZywgb3B0aW9uOiBzdHJpbmcsIGFyZ3VtZW50OiBzdHJpbmcpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5EYXRlVGltZSA9IGRhdGVUaW1lXHJcbiAgICAgICAgdGhpcy5UeXBlID0gdHlwZVxyXG4gICAgICAgIHRoaXMuT3B0aW9uID0gb3B0aW9uXHJcbiAgICAgICAgdGhpcy5Bcmd1bWVudCA9IGFyZ3VtZW50XHJcbiAgICB9XHJcbn0iLCJkZWNsYXJlIHZhciBzaWduYWxSOiBhbnlcclxuXHJcbmNsYXNzIENvbm5lY3Rpb25NYW5hZ2VyXHJcbntcclxuICAgIHB1YmxpYyBzdGF0aWMgV2ViQ2xpZW50SHViOiBhbnkgLy8gaW1wcm92ZTogdXNlIHR5cGluZ3NcclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIEluaXRpYWxpemUoZW5hYmxlTG9nZ2luZzogYm9vbGVhbilcclxuICAgIHtcclxuICAgICAgICBDb25uZWN0aW9uTWFuYWdlci5XZWJDbGllbnRIdWIgPSBuZXcgc2lnbmFsUi5IdWJDb25uZWN0aW9uKCcvd2ViY2xpZW50aHViJyk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBJbnZva2VXZWJDbGllbnRIdWIgPSBhc3luYyhtZXRob2ROYW1lOiBzdHJpbmcsIC4uLmFyZ3M6IGFueVtdKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiBhd2FpdCBQcm9taXNlLnJlc29sdmUoQ29ubmVjdGlvbk1hbmFnZXIuV2ViQ2xpZW50SHViLmludm9rZShtZXRob2ROYW1lLCAuLi5hcmdzKSlcclxuICAgIH1cclxufVxyXG4iLCJjbGFzcyBFbnVtZXJhdGlvbkhlbHBlclxyXG57XHJcbiAgICBwdWJsaWMgc3RhdGljIERlc2NyaXB0aW9uOiB7IFtpbmRleDogc3RyaW5nXTogc3RyaW5nIH0gPSB7fVxyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgR2V0RW51bUxvY2FsaXphdGlvbiA9ICh0eXBlTmFtZTogc3RyaW5nLCB2YWx1ZSkgPT5cclxuICAgIHtcclxuICAgICAgICB2YXIga2V5OiBzdHJpbmcgPSBldmFsKHR5cGVOYW1lICsgXCJbXCIgKyB2YWx1ZSArIFwiXVwiKVxyXG4gICAgICAgIHJldHVybiBldmFsKFwiRW51bWVyYXRpb25IZWxwZXIuRGVzY3JpcHRpb25bJ1wiICsgdHlwZU5hbWUgKyBcIl9cIiArIGtleSArIFwiJ11cIilcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIEdldEVudW1WYWx1ZXMgPSAodHlwZU5hbWU6IHN0cmluZykgPT5cclxuICAgIHtcclxuICAgICAgICBsZXQgdmFsdWVzOiBhbnlbXVxyXG5cclxuICAgICAgICB2YWx1ZXMgPSBldmFsKFwiT2JqZWN0LmtleXMoXCIgKyB0eXBlTmFtZSArIFwiKS5tYXAoa2V5ID0+IFwiICsgdHlwZU5hbWUgKyBcIltrZXldKVwiKVxyXG4gICAgICAgIHJldHVybiA8bnVtYmVyW10+dmFsdWVzLmZpbHRlcih2YWx1ZSA9PiB0eXBlb2YgKHZhbHVlKSA9PT0gXCJudW1iZXJcIilcclxuICAgIH1cclxufSIsImxldCBFcnJvck1lc3NhZ2U6IHsgW2luZGV4OiBzdHJpbmddOiBzdHJpbmcgfSA9IHt9XHJcbkVycm9yTWVzc2FnZVtcIk11bHRpTWFwcGluZ0VkaXRvclZpZXdNb2RlbF9JbnZhbGlkU2V0dGluZ3NcIl0gPSBcIk9uZSBvciBtb3JlIHNldHRpbmdzIGFyZSBpbnZhbGlkLlwiXHJcbkVycm9yTWVzc2FnZVtcIk11bHRpTWFwcGluZ0VkaXRvclZpZXdNb2RlbF9Xcm9uZ0RhdGFUeXBlXCJdID0gXCJPbmUgb3IgbW9yZSB2YXJpYWJsZS1jaGFubmVsIGRhdGEgdHlwZSBjb21iaW5hdGlvbnMgYXJlIGludmFsaWQuXCJcclxuRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9DaGFubmVsQWxyZWFkeUV4aXN0c1wiXSA9IFwiQSBjaGFubmVsIHdpdGggdGhhdCBuYW1lIGFscmVhZHkgZXhpc3RzLlwiXHJcbkVycm9yTWVzc2FnZVtcIlByb2plY3RfRGV0YWNoZWRFeGNsYW1hdGlvbk1hcmtOb3RBbGxvd2VkXCJdID0gXCJBIGRldGFjaGVkIGV4Y2xhbWF0aW9uIG1hcmsgaXMgbm90IGFsbG93ZWQuXCJcclxuRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9Hcm91cEZpbHRlckVtcHR5XCJdID0gXCJUaGUgZ3JvdXAgZmlsdGVyIG11c3Qgbm90IGJlIGVtcHR5LlwiXHJcbkVycm9yTWVzc2FnZVtcIlByb2plY3RfSXNBbHJlYWR5SW5Hcm91cFwiXSA9IFwiVGhlIGNoYW5uZWwgaXMgYWxyZWFkeSBhIG1lbWJlciBvZiB0aGlzIGdyb3VwLlwiXHJcbkVycm9yTWVzc2FnZVtcIlByb2plY3RfSW52YWxpZENoYXJhY3RlcnNcIl0gPSBcIlVzZSBBLVosIGEteiwgMC05IG9yIF8uXCJcclxuRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9JbnZhbGlkTGVhZGluZ0NoYXJhY3RlclwiXSA9IFwiVXNlIEEtWiBvciBhLXogYXMgZmlyc3QgY2hhcmFjdGVyLlwiXHJcbkVycm9yTWVzc2FnZVtcIlByb2plY3RfTmFtZUVtcHR5XCJdID0gXCJUaGUgbmFtZSBtdXN0IG5vdCBiZSBlbXB0eS5cIlxyXG4iLCJjbGFzcyBPYnNlcnZhYmxlR3JvdXA8VD5cclxue1xyXG4gICAgS2V5OiBzdHJpbmc7XHJcbiAgICBNZW1iZXJzOiBLbm9ja291dE9ic2VydmFibGVBcnJheTxUPlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKGtleTogc3RyaW5nLCBtZW1iZXJzOiBUW10gPSBuZXcgQXJyYXk8VD4oKSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLktleSA9IGtleVxyXG4gICAgICAgIHRoaXMuTWVtYmVycyA9IGtvLm9ic2VydmFibGVBcnJheShtZW1iZXJzKVxyXG4gICAgfVxyXG59XHJcblxyXG5mdW5jdGlvbiBPYnNlcnZhYmxlR3JvdXBCeTxUPihsaXN0OiBUW10sIG5hbWVHZXR0ZXI6ICh4OiBUKSA9PiBzdHJpbmcsIGdyb3VwTmFtZUdldHRlcjogKHg6IFQpID0+IHN0cmluZywgZmlsdGVyOiBzdHJpbmcpOiBPYnNlcnZhYmxlR3JvdXA8VD5bXVxyXG57XHJcbiAgICBsZXQgcmVzdWx0OiBPYnNlcnZhYmxlR3JvdXA8VD5bXVxyXG4gICAgbGV0IHJlZ0V4cDogUmVnRXhwXHJcblxyXG4gICAgcmVzdWx0ID0gW11cclxuICAgIHJlZ0V4cCA9IG5ldyBSZWdFeHAoZmlsdGVyLCBcImlcIilcclxuXHJcbiAgICBsaXN0LmZvckVhY2goZWxlbWVudCA9PlxyXG4gICAge1xyXG4gICAgICAgIGlmIChyZWdFeHAudGVzdChuYW1lR2V0dGVyKGVsZW1lbnQpKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGdyb3VwTmFtZUdldHRlcihlbGVtZW50KS5zcGxpdChcIlxcblwiKS5mb3JFYWNoKGdyb3VwTmFtZSA9PlxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICBBZGRUb0dyb3VwZWRBcnJheShlbGVtZW50LCBncm91cE5hbWUsIHJlc3VsdClcclxuICAgICAgICAgICAgfSlcclxuICAgICAgICB9XHJcbiAgICB9KVxyXG5cclxuICAgIHJldHVybiByZXN1bHRcclxufVxyXG5cclxuZnVuY3Rpb24gQWRkVG9Hcm91cGVkQXJyYXk8VD4oaXRlbTogVCwgZ3JvdXBOYW1lOiBzdHJpbmcsIG9ic2VydmFibGVHcm91cFNldDogT2JzZXJ2YWJsZUdyb3VwPFQ+W10pXHJcbntcclxuICAgIGxldCBncm91cDogT2JzZXJ2YWJsZUdyb3VwPFQ+XHJcblxyXG4gICAgZ3JvdXAgPSBvYnNlcnZhYmxlR3JvdXBTZXQuZmluZCh5ID0+IHkuS2V5ID09PSBncm91cE5hbWUpXHJcblxyXG4gICAgaWYgKCFncm91cClcclxuICAgIHtcclxuICAgICAgICBncm91cCA9IG5ldyBPYnNlcnZhYmxlR3JvdXA8VD4oZ3JvdXBOYW1lKVxyXG4gICAgICAgIG9ic2VydmFibGVHcm91cFNldC5wdXNoKGdyb3VwKVxyXG4gICAgfVxyXG5cclxuICAgIGdyb3VwLk1lbWJlcnMucHVzaChpdGVtKVxyXG59XHJcblxyXG4vL2Z1bmN0aW9uIEFkZFRvR3JvdXBlZE9ic2VydmFibGVBcnJheTxUPihpdGVtOiBULCBncm91cE5hbWU6IHN0cmluZywgb2JzZXJ2YWJsZUdyb3VwU2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxPYnNlcnZhYmxlR3JvdXA8VD4+KVxyXG4vL3tcclxuLy8gICAgbGV0IGdyb3VwOiBPYnNlcnZhYmxlR3JvdXA8VD5cclxuXHJcbi8vICAgIGdyb3VwID0gb2JzZXJ2YWJsZUdyb3VwU2V0KCkuZmluZCh5ID0+IHkuS2V5ID09PSBncm91cE5hbWUpXHJcblxyXG4vLyAgICBpZiAoIWdyb3VwKVxyXG4vLyAgICB7XHJcbi8vICAgICAgICBncm91cCA9IG5ldyBPYnNlcnZhYmxlR3JvdXA8VD4oZ3JvdXBOYW1lKVxyXG4vLyAgICAgICAgb2JzZXJ2YWJsZUdyb3VwU2V0LnB1c2goZ3JvdXApXHJcbi8vICAgIH1cclxuXHJcbi8vICAgIGdyb3VwLk1lbWJlcnMucHVzaChpdGVtKVxyXG4vL31cclxuXHJcbi8vZnVuY3Rpb24gUmVtb3ZlRnJvbUdyb3VwZWRPYnNlcnZhYmxlQXJyYXk8VD4oaXRlbTogVCwgb2JzZXJ2YWJsZUdyb3VwU2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxPYnNlcnZhYmxlR3JvdXA8VD4+KVxyXG4vL3tcclxuLy8gICAgdmFyIGdyb3VwOiBPYnNlcnZhYmxlR3JvdXA8VD5cclxuXHJcbi8vICAgIG9ic2VydmFibGVHcm91cFNldCgpLnNvbWUoeCA9PlxyXG4vLyAgICB7XHJcbi8vICAgICAgICBpZiAoeC5NZW1iZXJzKCkuaW5kZXhPZihpdGVtKSA+IC0xKVxyXG4vLyAgICAgICAge1xyXG4vLyAgICAgICAgICAgIGdyb3VwID0geFxyXG5cclxuLy8gICAgICAgICAgICByZXR1cm4gdHJ1ZVxyXG4vLyAgICAgICAgfVxyXG5cclxuLy8gICAgICAgIHJldHVybiBmYWxzZVxyXG4vLyAgICB9KVxyXG5cclxuLy8gICAgaWYgKGdyb3VwKVxyXG4vLyAgICB7XHJcbi8vICAgICAgICBncm91cC5NZW1iZXJzLnJlbW92ZShpdGVtKVxyXG5cclxuLy8gICAgICAgIGlmIChncm91cC5NZW1iZXJzKCkubGVuZ3RoID09PSAwKVxyXG4vLyAgICAgICAge1xyXG4vLyAgICAgICAgICAgIG9ic2VydmFibGVHcm91cFNldC5yZW1vdmUoZ3JvdXApXHJcbi8vICAgICAgICB9XHJcblxyXG4vLyAgICAgICAgcmV0dXJuIHRydWVcclxuLy8gICAgfVxyXG5cclxuLy8gICAgcmV0dXJuIGZhbHNlXHJcbi8vfVxyXG5cclxuLy9mdW5jdGlvbiBVcGRhdGVHcm91cGVkT2JzZXJ2YWJsZUFycmF5PFQ+KGl0ZW06IFQsIGdyb3VwTmFtZTogc3RyaW5nLCBvYnNlcnZhYmxlR3JvdXBTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PE9ic2VydmFibGVHcm91cDxUPj4pXHJcbi8ve1xyXG4vLyAgICBSZW1vdmVGcm9tR3JvdXBlZE9ic2VydmFibGVBcnJheShpdGVtLCBvYnNlcnZhYmxlR3JvdXBTZXQpXHJcbi8vICAgIEFkZFRvR3JvdXBlZE9ic2VydmFibGVBcnJheShpdGVtLCBncm91cE5hbWUsIG9ic2VydmFibGVHcm91cFNldClcclxuLy99XHJcblxyXG5mdW5jdGlvbiBNYXBNYW55PFRBcnJheUVsZW1lbnQsIFRTZWxlY3Q+KGFycmF5OiBUQXJyYXlFbGVtZW50W10sIG1hcEZ1bmM6IChpdGVtOiBUQXJyYXlFbGVtZW50KSA9PiBUU2VsZWN0W10pOiBUU2VsZWN0W11cclxue1xyXG4gICAgcmV0dXJuIGFycmF5LnJlZHVjZSgocHJldmlvdXMsIGN1cnJlbnQsIGkpID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHByZXZpb3VzLmNvbmNhdChtYXBGdW5jKGN1cnJlbnQpKTtcclxuICAgIH0sIDxUU2VsZWN0W10+W10pO1xyXG59XHJcblxyXG5jbGFzcyBHdWlkXHJcbntcclxuICAgIHN0YXRpYyBOZXdHdWlkKClcclxuICAgIHtcclxuICAgICAgICByZXR1cm4gJ3h4eHh4eHh4LXh4eHgtNHh4eC15eHh4LXh4eHh4eHh4eHh4eCcucmVwbGFjZSgvW3h5XS9nLCBmdW5jdGlvbiAoYylcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHZhciByID0gTWF0aC5yYW5kb20oKSAqIDE2IHwgMFxyXG4gICAgICAgICAgICB2YXIgdiA9IGMgPT09ICd4JyA/IHIgOiAociAmIDB4MyB8IDB4OClcclxuXHJcbiAgICAgICAgICAgIHJldHVybiB2LnRvU3RyaW5nKDE2KVxyXG4gICAgICAgIH0pXHJcbiAgICB9XHJcbn1cclxuXHJcbmZ1bmN0aW9uIGRlbGF5KG1zOiBudW1iZXIpXHJcbntcclxuICAgIHJldHVybiBuZXcgUHJvbWlzZShyZXNvbHZlID0+IHNldFRpbWVvdXQocmVzb2x2ZSwgbXMpKTtcclxufVxyXG5cclxubGV0IENoZWNrTmFtaW5nQ29udmVudGlvbiA9ICh2YWx1ZTogc3RyaW5nKSA9PlxyXG57XHJcbiAgICB2YXIgcmVnRXhwOiBhbnlcclxuXHJcbiAgICBpZiAodmFsdWUubGVuZ3RoID09PSAwKVxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB7IEhhc0Vycm9yOiB0cnVlLCBFcnJvckRlc2NyaXB0aW9uOiBFcnJvck1lc3NhZ2VbXCJQcm9qZWN0X05hbWVFbXB0eVwiXSB9XHJcbiAgICB9XHJcblxyXG4gICAgcmVnRXhwID0gbmV3IFJlZ0V4cChcIlteQS1aYS16MC05X11cIilcclxuXHJcbiAgICBpZiAocmVnRXhwLnRlc3QodmFsdWUpKVxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB7IEhhc0Vycm9yOiB0cnVlLCBFcnJvckRlc2NyaXB0aW9uOiBFcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0ludmFsaWRDaGFyYWN0ZXJzXCJdIH1cclxuICAgIH1cclxuXHJcbiAgICByZWdFeHAgPSBuZXcgUmVnRXhwKFwiXlswLTlfXVwiKVxyXG5cclxuICAgIGlmIChyZWdFeHAudGVzdCh2YWx1ZSkpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHsgSGFzRXJyb3I6IHRydWUsIEVycm9yRGVzY3JpcHRpb246IEVycm9yTWVzc2FnZVtcIlByb2plY3RfSW52YWxpZExlYWRpbmdDaGFyYWN0ZXJcIl0gfVxyXG4gICAgfVxyXG5cclxuICAgIHJldHVybiB7XHJcbiAgICAgICAgSGFzRXJyb3I6IGZhbHNlLFxyXG4gICAgICAgIEVycm9yRGVzY3JpcHRpb246IFwiXCJcclxuICAgIH1cclxufSIsImNsYXNzIFBsdWdpbkZhY3Rvcnlcclxue1xyXG4gICAgcHVibGljIHN0YXRpYyBDcmVhdGVQbHVnaW5WaWV3TW9kZWxBc3luYyA9IGFzeW5jIChwbHVnaW5UeXBlOiBzdHJpbmcsIHBsdWdpbk1vZGVsOiBhbnkpID0+XHJcbiAgICB7XHJcbiAgICAgICAgbGV0IHBsdWdpbklkZW50aWZpY2F0aW9uOiBQbHVnaW5JZGVudGlmaWNhdGlvblZpZXdNb2RlbFxyXG4gICAgICAgIGxldCBwbHVnaW5WaWV3TW9kZWw6IFBsdWdpblZpZXdNb2RlbEJhc2VcclxuICAgICAgICBsZXQgcGx1Z2luVmlld01vZGVsUmF3OiBzdHJpbmdcclxuXHJcbiAgICAgICAgcGx1Z2luSWRlbnRpZmljYXRpb24gPSBQbHVnaW5IaXZlLkZpbmRQbHVnaW5JZGVudGlmaWNhdGlvbihwbHVnaW5UeXBlLCBwbHVnaW5Nb2RlbC5EZXNjcmlwdGlvbi5JZClcclxuXHJcbiAgICAgICAgaWYgKHBsdWdpbklkZW50aWZpY2F0aW9uKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgcGx1Z2luVmlld01vZGVsUmF3ID0gYXdhaXQgQ29ubmVjdGlvbk1hbmFnZXIuSW52b2tlV2ViQ2xpZW50SHViKFwiR2V0UGx1Z2luU3RyaW5nUmVzb3VyY2VcIiwgcGx1Z2luTW9kZWwuRGVzY3JpcHRpb24uSWQsIHBsdWdpbklkZW50aWZpY2F0aW9uLlZpZXdNb2RlbFJlc291cmNlTmFtZSlcclxuICAgICAgICAgICAgcGx1Z2luVmlld01vZGVsID0gPFBsdWdpblZpZXdNb2RlbEJhc2U+bmV3IEZ1bmN0aW9uKHBsdWdpblZpZXdNb2RlbFJhdyArIFwiOyByZXR1cm4gVmlld01vZGVsQ29uc3RydWN0b3JcIikoKShwbHVnaW5Nb2RlbCwgcGx1Z2luSWRlbnRpZmljYXRpb24pXHJcblxyXG4gICAgICAgICAgICByZXR1cm4gcGx1Z2luVmlld01vZGVsXHJcbiAgICAgICAgfVxyXG4gICAgICAgIGVsc2VcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRocm93IG5ldyBFcnJvcihcIk5vIGNvcnJlc3BvbmRpbmcgcGx1Z2luIGRlc2NyaXB0aW9uIGZvciBwbHVnaW4gSUQgJ1wiICsgcGx1Z2luTW9kZWwuRGVzY3JpcHRpb24uSWQgKyBcIicgZm91bmQuXCIpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG59IiwiY2xhc3MgUGx1Z2luSGl2ZVxyXG57XHJcbiAgICAvLyBmaWVsZHNcclxuICAgIHB1YmxpYyBzdGF0aWMgUGx1Z2luSWRlbnRpZmljYXRpb25TZXQ6IE1hcDxzdHJpbmcsIFBsdWdpbklkZW50aWZpY2F0aW9uVmlld01vZGVsW10+XHJcblxyXG4gICAgLy8gY29uc3RydWN0b3JzXHJcbiAgICBzdGF0aWMgSW5pdGlhbGl6ZSA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgUGx1Z2luSGl2ZS5QbHVnaW5JZGVudGlmaWNhdGlvblNldCA9IG5ldyBNYXA8c3RyaW5nLCBQbHVnaW5JZGVudGlmaWNhdGlvblZpZXdNb2RlbFtdPigpXHJcbiAgICB9ICAgXHJcblxyXG4gICAgc3RhdGljIEZpbmRQbHVnaW5JZGVudGlmaWNhdGlvbiA9IChwbHVnaW5UeXBlTmFtZTogc3RyaW5nLCBwbHVnaW5JZDogc3RyaW5nKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiBQbHVnaW5IaXZlLlBsdWdpbklkZW50aWZpY2F0aW9uU2V0LmdldChwbHVnaW5UeXBlTmFtZSkuZmluZChwbHVnaW5JZGVudGlmaWNhdGlvbiA9PiBwbHVnaW5JZGVudGlmaWNhdGlvbi5JZCA9PT0gcGx1Z2luSWQpO1xyXG4gICAgfVxyXG59IiwiY2xhc3MgQ2hhbm5lbEh1YlZpZXdNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgTmFtZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBHcm91cDogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyByZWFkb25seSBEYXRhVHlwZTogS25vY2tvdXRPYnNlcnZhYmxlPE9uZURhc0RhdGFUeXBlRW51bT5cclxuICAgIHB1YmxpYyByZWFkb25seSBHdWlkOiBzdHJpbmdcclxuICAgIHB1YmxpYyByZWFkb25seSBDcmVhdGlvbkRhdGVUaW1lOiBzdHJpbmdcclxuICAgIHB1YmxpYyByZWFkb25seSBVbml0OiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IFRyYW5zZmVyRnVuY3Rpb25TZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PFRyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWw+XHJcbiAgICBwdWJsaWMgU2VsZWN0ZWRUcmFuc2ZlckZ1bmN0aW9uOiBLbm9ja291dE9ic2VydmFibGU8VHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbD5cclxuICAgIHB1YmxpYyBFdmFsdWF0ZWRUcmFuc2ZlckZ1bmN0aW9uU2V0OiAoKHZhbHVlOiBudW1iZXIpID0+IG51bWJlcilbXVxyXG4gICAgcHVibGljIElzU2VsZWN0ZWQ6IEtub2Nrb3V0T2JzZXJ2YWJsZTxib29sZWFuPlxyXG5cclxuICAgIHB1YmxpYyByZWFkb25seSBEYXRhVHlwZVNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8T25lRGFzRGF0YVR5cGVFbnVtPlxyXG5cclxuICAgIHB1YmxpYyByZWFkb25seSBBc3NvY2lhdGVkRGF0YUlucHV0OiBLbm9ja291dE9ic2VydmFibGU8RGF0YVBvcnRWaWV3TW9kZWw+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgQXNzb2NpYXRlZERhdGFPdXRwdXRTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PERhdGFQb3J0Vmlld01vZGVsPlxyXG5cclxuICAgIHByaXZhdGUgQXNzb2NpYXRlZERhdGFJbnB1dElkOiBzdHJpbmdcclxuICAgIHByaXZhdGUgQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldDogc3RyaW5nW11cclxuXHJcbiAgICBjb25zdHJ1Y3RvcihjaGFubmVsSHViTW9kZWw6IENoYW5uZWxIdWJNb2RlbClcclxuICAgIHtcclxuICAgICAgICB0aGlzLk5hbWUgPSBrby5vYnNlcnZhYmxlPHN0cmluZz4oY2hhbm5lbEh1Yk1vZGVsLk5hbWUpXHJcbiAgICAgICAgdGhpcy5Hcm91cCA9IGtvLm9ic2VydmFibGU8c3RyaW5nPihjaGFubmVsSHViTW9kZWwuR3JvdXApXHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZVNldCA9IGtvLm9ic2VydmFibGVBcnJheTxPbmVEYXNEYXRhVHlwZUVudW0+KEVudW1lcmF0aW9uSGVscGVyLkdldEVudW1WYWx1ZXMoJ09uZURhc0RhdGFUeXBlRW51bScpKVxyXG4gICAgICAgIHRoaXMuRGF0YVR5cGUgPSBrby5vYnNlcnZhYmxlPE9uZURhc0RhdGFUeXBlRW51bT4oY2hhbm5lbEh1Yk1vZGVsLkRhdGFUeXBlKVxyXG4gICAgICAgIHRoaXMuR3VpZCA9IGNoYW5uZWxIdWJNb2RlbC5HdWlkXHJcbiAgICAgICAgdGhpcy5DcmVhdGlvbkRhdGVUaW1lID0gY2hhbm5lbEh1Yk1vZGVsLkNyZWF0aW9uRGF0ZVRpbWVcclxuICAgICAgICB0aGlzLlVuaXQgPSBrby5vYnNlcnZhYmxlPHN0cmluZz4oY2hhbm5lbEh1Yk1vZGVsLlVuaXQpXHJcbiAgICAgICAgdGhpcy5UcmFuc2ZlckZ1bmN0aW9uU2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PFRyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWw+KGNoYW5uZWxIdWJNb2RlbC5UcmFuc2ZlckZ1bmN0aW9uU2V0Lm1hcCh0ZiA9PiBuZXcgVHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbCh0ZikpKVxyXG4gICAgICAgIHRoaXMuU2VsZWN0ZWRUcmFuc2ZlckZ1bmN0aW9uID0ga28ub2JzZXJ2YWJsZTxUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsPih0aGlzLkNyZWF0ZURlZmF1bHRUcmFuc2ZlckZ1bmN0aW9uKCkpXHJcbiAgICAgICAgdGhpcy5Jc1NlbGVjdGVkID0ga28ub2JzZXJ2YWJsZTxib29sZWFuPihmYWxzZSlcclxuXHJcbiAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0ID0ga28ub2JzZXJ2YWJsZTxEYXRhUG9ydFZpZXdNb2RlbD4oKVxyXG4gICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRTZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8RGF0YVBvcnRWaWV3TW9kZWw+KClcclxuXHJcbiAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0SWQgPSBjaGFubmVsSHViTW9kZWwuQXNzb2NpYXRlZERhdGFJbnB1dElkXHJcbiAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0ID0gY2hhbm5lbEh1Yk1vZGVsLkFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXRcclxuICAgICAgICB0aGlzLkV2YWx1YXRlZFRyYW5zZmVyRnVuY3Rpb25TZXQgPSBbXVxyXG4gICAgfVxyXG5cclxuICAgIC8vIG1ldGhvZHNcclxuICAgIHB1YmxpYyBHZXRUcmFuc2Zvcm1lZFZhbHVlID0gKHZhbHVlOiBhbnkpOiBudW1iZXIgPT4gXHJcbiAgICB7XHJcbiAgICAgICAgaWYgKHZhbHVlID09PSBcIk5hTlwiKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdmFsdWUgPSBOYU5cclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuRXZhbHVhdGVkVHJhbnNmZXJGdW5jdGlvblNldC5mb3JFYWNoKHRmID0+IHZhbHVlID0gdGYodmFsdWUpKVxyXG5cclxuICAgICAgICByZXR1cm4gdmFsdWVcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIENyZWF0ZURlZmF1bHRUcmFuc2ZlckZ1bmN0aW9uID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gbmV3IFRyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWwobmV3IFRyYW5zZmVyRnVuY3Rpb25Nb2RlbChcIjAwMDEtMDEtMDFUMDA6MDA6MDBaXCIsIFwicG9seW5vbWlhbFwiLCBcInBlcm1hbmVudFwiLCBcIjE7MFwiKSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgSXNBc3NvY2lhdGlvbkFsbG93ZWQoZGF0YVBvcnQ6IERhdGFQb3J0Vmlld01vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiAoZGF0YVBvcnQuRGF0YVR5cGUgJiAweGZmKSA9PSAodGhpcy5EYXRhVHlwZSgpICYgMHhmZilcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVXBkYXRlQXNzb2NpYXRpb24gPSAoZGF0YVBvcnQ6IERhdGFQb3J0Vmlld01vZGVsKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHN3aXRjaCAoZGF0YVBvcnQuRGF0YURpcmVjdGlvbilcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uSW5wdXQ6XHJcblxyXG4gICAgICAgICAgICAgICAgdGhpcy5SZXNldEFzc29jaWF0aW9uKGZhbHNlLCB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXQoKSlcclxuICAgICAgICAgICAgICAgIGJyZWFrXHJcblxyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dDpcclxuXHJcbiAgICAgICAgICAgICAgICB0aGlzLlJlc2V0QXNzb2NpYXRpb24oZmFsc2UsIGRhdGFQb3J0KVxyXG4gICAgICAgICAgICAgICAgYnJlYWtcclxuXHJcbiAgICAgICAgICAgIGRlZmF1bHQ6XHJcbiAgICAgICAgICAgICAgICB0aHJvdyBuZXcgRXJyb3IoXCJOb3QgaW1wbGVtZW50ZWQuXCIpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5TZXRBc3NvY2lhdGlvbihkYXRhUG9ydClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgU2V0QXNzb2NpYXRpb24oZGF0YVBvcnQ6IERhdGFQb3J0Vmlld01vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIGRhdGFQb3J0LkFzc29jaWF0ZWRDaGFubmVsSHViU2V0LnB1c2godGhpcylcclxuXHJcbiAgICAgICAgc3dpdGNoIChkYXRhUG9ydC5EYXRhRGlyZWN0aW9uKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dDpcclxuXHJcbiAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXQoZGF0YVBvcnQpXHJcbiAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXRJZCA9IGRhdGFQb3J0LlRvRnVsbFF1YWxpZmllZElkZW50aWZpZXIoKVxyXG5cclxuICAgICAgICAgICAgICAgIGJyZWFrXHJcblxyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dDpcclxuXHJcbiAgICAgICAgICAgICAgICBsZXQgZGF0YU91dHB1dElkID0gZGF0YVBvcnQuVG9GdWxsUXVhbGlmaWVkSWRlbnRpZmllcigpXHJcblxyXG4gICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dFNldC5wdXNoKGRhdGFQb3J0KVxyXG5cclxuICAgICAgICAgICAgICAgIGlmICh0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQuaW5kZXhPZihkYXRhT3V0cHV0SWQpIDwgMClcclxuICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQucHVzaChkYXRhUG9ydC5Ub0Z1bGxRdWFsaWZpZWRJZGVudGlmaWVyKCkpXHJcbiAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgYnJlYWtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFJlc2V0QXNzb2NpYXRpb24obWFpbnRhaW5XZWFrUmVmZXJlbmNlOiBib29sZWFuLCAuLi5kYXRhUG9ydFNldDogRGF0YVBvcnRWaWV3TW9kZWxbXSlcclxuICAgIHtcclxuICAgICAgICBkYXRhUG9ydFNldC5mb3JFYWNoKGRhdGFQb3J0ID0+XHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBpZiAoIWRhdGFQb3J0KVxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICByZXR1cm5cclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgZGF0YVBvcnQuQXNzb2NpYXRlZENoYW5uZWxIdWJTZXQucmVtb3ZlKHRoaXMpXHJcblxyXG4gICAgICAgICAgICBzd2l0Y2ggKGRhdGFQb3J0LkRhdGFEaXJlY3Rpb24pXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uSW5wdXQ6XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dChudWxsKVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBpZiAoIW1haW50YWluV2Vha1JlZmVyZW5jZSlcclxuICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dElkID0gbnVsbFxyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgYnJlYWtcclxuXHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dDpcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dFNldC5yZW1vdmUoZGF0YVBvcnQpXHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmICghbWFpbnRhaW5XZWFrUmVmZXJlbmNlKVxyXG4gICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgbGV0IGluZGV4OiBudW1iZXIgPSB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQuaW5kZXhPZihkYXRhUG9ydC5Ub0Z1bGxRdWFsaWZpZWRJZGVudGlmaWVyKCkpXHJcblxyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAoaW5kZXggPiAtMSlcclxuICAgICAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0LnNwbGljZShpbmRleCwgMSlcclxuICAgICAgICAgICAgICAgICAgICAgICAgfSAgIFxyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgYnJlYWtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH0pXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFJlc2V0QWxsQXNzb2NpYXRpb25zKG1haW50YWluV2Vha1JlZmVyZW5jZTogYm9vbGVhbilcclxuICAgIHtcclxuICAgICAgICBpZiAodGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0KCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLlJlc2V0QXNzb2NpYXRpb24obWFpbnRhaW5XZWFrUmVmZXJlbmNlLCB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXQoKSlcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuUmVzZXRBc3NvY2lhdGlvbihtYWludGFpbldlYWtSZWZlcmVuY2UsIC4uLnRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRTZXQoKSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgR2V0QXNzb2NpYXRlZERhdGFJbnB1dElkID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0SWQ7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEdldEFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQ7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvTW9kZWwoKVxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB7XHJcbiAgICAgICAgICAgIE5hbWU6IDxzdHJpbmc+dGhpcy5OYW1lKCksXHJcbiAgICAgICAgICAgIEdyb3VwOiA8c3RyaW5nPnRoaXMuR3JvdXAoKSxcclxuICAgICAgICAgICAgRGF0YVR5cGU6IDxPbmVEYXNEYXRhVHlwZUVudW0+dGhpcy5EYXRhVHlwZSgpLFxyXG4gICAgICAgICAgICBHdWlkOiA8c3RyaW5nPnRoaXMuR3VpZCxcclxuICAgICAgICAgICAgQ3JlYXRpb25EYXRlVGltZTogPHN0cmluZz50aGlzLkNyZWF0aW9uRGF0ZVRpbWUsXHJcbiAgICAgICAgICAgIFVuaXQ6IDxzdHJpbmc+dGhpcy5Vbml0KCksXHJcbiAgICAgICAgICAgIFRyYW5zZmVyRnVuY3Rpb25TZXQ6IDxUcmFuc2ZlckZ1bmN0aW9uTW9kZWxbXT50aGlzLlRyYW5zZmVyRnVuY3Rpb25TZXQoKS5tYXAodGYgPT4gdGYuVG9Nb2RlbCgpKSxcclxuICAgICAgICAgICAgQXNzb2NpYXRlZERhdGFJbnB1dElkOiA8c3RyaW5nPnRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dElkLFxyXG4gICAgICAgICAgICBBc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0OiA8c3RyaW5nW10+dGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBBZGRUcmFuc2ZlckZ1bmN0aW9uID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLlRyYW5zZmVyRnVuY3Rpb25TZXQucHVzaCh0aGlzLlNlbGVjdGVkVHJhbnNmZXJGdW5jdGlvbigpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBEZWxldGVUcmFuc2ZlckZ1bmN0aW9uID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLlRyYW5zZmVyRnVuY3Rpb25TZXQucmVtb3ZlKHRoaXMuU2VsZWN0ZWRUcmFuc2ZlckZ1bmN0aW9uKCkpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIE5ld1RyYW5zZmVyRnVuY3Rpb24gPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuU2VsZWN0ZWRUcmFuc2ZlckZ1bmN0aW9uKHRoaXMuQ3JlYXRlRGVmYXVsdFRyYW5zZmVyRnVuY3Rpb24oKSlcclxuICAgIH1cclxuXHJcbiAgICAvLyBjb21tYW5kc1xyXG4gICAgcHVibGljIFNlbGVjdFRyYW5zZmVyRnVuY3Rpb24gPSAodHJhbnNmZXJGdW5jdGlvbjogVHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLlNlbGVjdGVkVHJhbnNmZXJGdW5jdGlvbih0cmFuc2ZlckZ1bmN0aW9uKVxyXG4gICAgfVxyXG59IiwiY2xhc3MgT25lRGFzTW9kdWxlVmlld01vZGVsXHJcbntcclxuICAgIHB1YmxpYyBEYXRhVHlwZTogS25vY2tvdXRPYnNlcnZhYmxlPE9uZURhc0RhdGFUeXBlRW51bT5cclxuICAgIHB1YmxpYyBEYXRhRGlyZWN0aW9uOiBLbm9ja291dE9ic2VydmFibGU8RGF0YURpcmVjdGlvbkVudW0+XHJcbiAgICBwdWJsaWMgRW5kaWFubmVzczogS25vY2tvdXRPYnNlcnZhYmxlPEVuZGlhbm5lc3NFbnVtPlxyXG4gICAgcHVibGljIFNpemU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxudW1iZXI+XHJcbiAgICBwdWJsaWMgTWF4U2l6ZTogS25vY2tvdXRPYnNlcnZhYmxlPG51bWJlcj5cclxuICAgIHB1YmxpYyBFcnJvck1lc3NhZ2U6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgSGFzRXJyb3I6IEtub2Nrb3V0Q29tcHV0ZWQ8Ym9vbGVhbj5cclxuXHJcbiAgICBwdWJsaWMgRGF0YVR5cGVTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PE9uZURhc0RhdGFUeXBlRW51bT5cclxuXHJcbiAgICBwcml2YXRlIF9vblByb3BlcnR5Q2hhbmdlZDogRXZlbnREaXNwYXRjaGVyPE9uZURhc01vZHVsZVZpZXdNb2RlbCwgYW55PlxyXG4gICAgcHJvdGVjdGVkIF9tb2RlbDogYW55XHJcblxyXG4gICAgY29uc3RydWN0b3Iob25lRGFzTW9kdWxlTW9kZWw6IE9uZURhc01vZHVsZU1vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuX21vZGVsID0gb25lRGFzTW9kdWxlTW9kZWxcclxuXHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZVNldCA9IGtvLm9ic2VydmFibGVBcnJheTxPbmVEYXNEYXRhVHlwZUVudW0+KEVudW1lcmF0aW9uSGVscGVyLkdldEVudW1WYWx1ZXMoJ09uZURhc0RhdGFUeXBlRW51bScpLmZpbHRlcihkYXRhVHlwZSA9PiBkYXRhVHlwZSAhPT0gT25lRGFzRGF0YVR5cGVFbnVtLkJPT0xFQU4pKVxyXG4gICAgICAgIHRoaXMuRGF0YVR5cGUgPSBrby5vYnNlcnZhYmxlPE9uZURhc0RhdGFUeXBlRW51bT4ob25lRGFzTW9kdWxlTW9kZWwuRGF0YVR5cGUpXHJcbiAgICAgICAgdGhpcy5EYXRhRGlyZWN0aW9uID0ga28ub2JzZXJ2YWJsZTxEYXRhRGlyZWN0aW9uRW51bT4ob25lRGFzTW9kdWxlTW9kZWwuRGF0YURpcmVjdGlvbilcclxuICAgICAgICB0aGlzLkVuZGlhbm5lc3MgPSBrby5vYnNlcnZhYmxlPEVuZGlhbm5lc3NFbnVtPihvbmVEYXNNb2R1bGVNb2RlbC5FbmRpYW5uZXNzKVxyXG4gICAgICAgIHRoaXMuU2l6ZSA9IGtvLm9ic2VydmFibGU8bnVtYmVyPihvbmVEYXNNb2R1bGVNb2RlbC5TaXplKVxyXG4gICAgICAgIHRoaXMuTWF4U2l6ZSA9IGtvLm9ic2VydmFibGU8bnVtYmVyPihJbmZpbml0eSlcclxuICAgICAgICB0aGlzLkVycm9yTWVzc2FnZSA9IGtvLm9ic2VydmFibGU8c3RyaW5nPihcIlwiKVxyXG4gICAgICAgIHRoaXMuSGFzRXJyb3IgPSBrby5jb21wdXRlZDxib29sZWFuPigoKSA9PiB0aGlzLkVycm9yTWVzc2FnZSgpLmxlbmd0aCA+IDApXHJcblxyXG4gICAgICAgIHRoaXMuX29uUHJvcGVydHlDaGFuZ2VkID0gbmV3IEV2ZW50RGlzcGF0Y2hlcjxPbmVEYXNNb2R1bGVWaWV3TW9kZWwsIGFueT4oKTtcclxuXHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZS5zdWJzY3JpYmUobmV3VmFsdWUgPT4gdGhpcy5PblByb3BlcnR5Q2hhbmdlZCgpKVxyXG4gICAgICAgIHRoaXMuRGF0YURpcmVjdGlvbi5zdWJzY3JpYmUobmV3VmFsdWUgPT4gdGhpcy5PblByb3BlcnR5Q2hhbmdlZCgpKVxyXG4gICAgICAgIHRoaXMuU2l6ZS5zdWJzY3JpYmUobmV3VmFsdWUgPT4gdGhpcy5PblByb3BlcnR5Q2hhbmdlZCgpKVxyXG4gICAgfVxyXG5cclxuICAgIGdldCBQcm9wZXJ0eUNoYW5nZWQoKTogSUV2ZW50PE9uZURhc01vZHVsZVZpZXdNb2RlbCwgYW55PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLl9vblByb3BlcnR5Q2hhbmdlZDtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgT25Qcm9wZXJ0eUNoYW5nZWQgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuVmFsaWRhdGUoKVxyXG4gICAgICAgIHRoaXMuX29uUHJvcGVydHlDaGFuZ2VkLmRpc3BhdGNoKHRoaXMsIG51bGwpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEdldEJ5dGVDb3VudCA9IChib29sZWFuQml0U2l6ZT86IG51bWJlcikgPT5cclxuICAgIHtcclxuICAgICAgICBpZiAoYm9vbGVhbkJpdFNpemUgJiYgdGhpcy5EYXRhVHlwZSgpID09PSBPbmVEYXNEYXRhVHlwZUVudW0uQk9PTEVBTilcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGJvb2xlYW5CaXRTaXplID0gcGFyc2VJbnQoPGFueT5ib29sZWFuQml0U2l6ZSlcclxuXHJcbiAgICAgICAgICAgIHJldHVybiBNYXRoLmNlaWwoYm9vbGVhbkJpdFNpemUgKiB0aGlzLlNpemUoKSAvIDgpO1xyXG4gICAgICAgIH1cclxuICAgICAgICBlbHNlXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICByZXR1cm4gKHRoaXMuRGF0YVR5cGUoKSAmIDB4MEZGKSAvIDggKiB0aGlzLlNpemUoKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVmFsaWRhdGUoKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiXCIpXHJcblxyXG4gICAgICAgIGlmICh0aGlzLlNpemUoKSA8IDEgfHwgKGlzRmluaXRlKHRoaXMuTWF4U2l6ZSgpKSAmJiB0aGlzLlNpemUoKSA+IHRoaXMuTWF4U2l6ZSgpKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiU2l6ZSBtdXN0IGJlIHdpdGhpbiByYW5nZSAxLi5cIiArIHRoaXMuTWF4U2l6ZSgpICsgXCIuXCIpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb1N0cmluZygpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuU2l6ZSgpICsgXCJ4IFwiICsgRW51bWVyYXRpb25IZWxwZXIuR2V0RW51bUxvY2FsaXphdGlvbignT25lRGFzRGF0YVR5cGVFbnVtJywgdGhpcy5EYXRhVHlwZSgpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBFeHRlbmRNb2RlbChtb2RlbDogYW55KVxyXG4gICAge1xyXG4gICAgICAgIC8vXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvTW9kZWwoKVxyXG4gICAge1xyXG4gICAgICAgIGxldCBtb2RlbDogYW55ID0ge1xyXG4gICAgICAgICAgICAkdHlwZTogPHN0cmluZz50aGlzLl9tb2RlbC4kdHlwZSxcclxuICAgICAgICAgICAgRGF0YVR5cGU6IDxPbmVEYXNEYXRhVHlwZUVudW0+dGhpcy5EYXRhVHlwZSgpLFxyXG4gICAgICAgICAgICBTaXplOiA8bnVtYmVyPnRoaXMuU2l6ZSgpLFxyXG4gICAgICAgICAgICBEYXRhRGlyZWN0aW9uOiA8RGF0YURpcmVjdGlvbkVudW0+dGhpcy5EYXRhRGlyZWN0aW9uKCksXHJcbiAgICAgICAgICAgIEVuZGlhbm5lc3M6IDxFbmRpYW5uZXNzRW51bT50aGlzLkVuZGlhbm5lc3MoKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5FeHRlbmRNb2RlbChtb2RlbClcclxuXHJcbiAgICAgICAgcmV0dXJuIG1vZGVsXHJcbiAgICB9XHJcbn1cclxuIiwiY2xhc3MgT25lRGFzTW9kdWxlU2VsZWN0b3JWaWV3TW9kZWxcclxue1xyXG4gICAgcHVibGljIFNldHRpbmdzVGVtcGxhdGVOYW1lOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIE5ld01vZHVsZTogS25vY2tvdXRPYnNlcnZhYmxlPE9uZURhc01vZHVsZVZpZXdNb2RlbD4gIFxyXG4gICAgcHVibGljIE1heEJ5dGVzOiBLbm9ja291dE9ic2VydmFibGU8bnVtYmVyPlxyXG4gICAgcHVibGljIFJlbWFpbmluZ0J5dGVzOiBLbm9ja291dE9ic2VydmFibGU8bnVtYmVyPlxyXG4gICAgcHVibGljIFJlbWFpbmluZ0NvdW50OiBLbm9ja291dE9ic2VydmFibGU8bnVtYmVyPiAgICBcclxuICAgIHB1YmxpYyBNb2R1bGVTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PE9uZURhc01vZHVsZVZpZXdNb2RlbD5cclxuICAgIHB1YmxpYyBFcnJvck1lc3NhZ2U6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgSGFzRXJyb3I6IEtub2Nrb3V0Q29tcHV0ZWQ8Ym9vbGVhbj5cclxuXHJcbiAgICBwdWJsaWMgT25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlOiBLbm9ja291dE9ic2VydmFibGU8T25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlRW51bT5cclxuXHJcbiAgICBwcml2YXRlIF9vbk1vZHVsZVNldENoYW5nZWQ6IEV2ZW50RGlzcGF0Y2hlcjxPbmVEYXNNb2R1bGVTZWxlY3RvclZpZXdNb2RlbCwgT25lRGFzTW9kdWxlVmlld01vZGVsW10+XHJcblxyXG4gICAgY29uc3RydWN0b3Iob25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlOiBPbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtLCBtb2R1bGVTZXQ6IE9uZURhc01vZHVsZVZpZXdNb2RlbFtdID0gW10pXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5PbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGUgPSBrby5vYnNlcnZhYmxlPE9uZURhc01vZHVsZVNlbGVjdG9yTW9kZUVudW0+KG9uZURhc01vZHVsZVNlbGVjdG9yTW9kZSlcclxuXHJcbiAgICAgICAgdGhpcy5TZXR0aW5nc1RlbXBsYXRlTmFtZSA9IGtvLm9ic2VydmFibGUoXCJQcm9qZWN0X09uZURhc01vZHVsZVRlbXBsYXRlXCIpXHJcbiAgICAgICAgdGhpcy5OZXdNb2R1bGUgPSBrby5vYnNlcnZhYmxlPE9uZURhc01vZHVsZVZpZXdNb2RlbD4oKTtcclxuICAgICAgICB0aGlzLk1heEJ5dGVzID0ga28ub2JzZXJ2YWJsZTxudW1iZXI+KEluZmluaXR5KTtcclxuICAgICAgICB0aGlzLlJlbWFpbmluZ0J5dGVzID0ga28ub2JzZXJ2YWJsZTxudW1iZXI+KE5hTik7XHJcbiAgICAgICAgdGhpcy5SZW1haW5pbmdDb3VudCA9IGtvLm9ic2VydmFibGU8bnVtYmVyPihOYU4pO1xyXG4gICAgICAgIHRoaXMuTW9kdWxlU2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PE9uZURhc01vZHVsZVZpZXdNb2RlbD4obW9kdWxlU2V0KTtcclxuICAgICAgICB0aGlzLkVycm9yTWVzc2FnZSA9IGtvLm9ic2VydmFibGU8c3RyaW5nPihcIlwiKVxyXG4gICAgICAgIHRoaXMuSGFzRXJyb3IgPSBrby5jb21wdXRlZDxib29sZWFuPigoKSA9PiB0aGlzLkVycm9yTWVzc2FnZSgpLmxlbmd0aCA+IDApXHJcblxyXG4gICAgICAgIHRoaXMuX29uTW9kdWxlU2V0Q2hhbmdlZCA9IG5ldyBFdmVudERpc3BhdGNoZXI8T25lRGFzTW9kdWxlU2VsZWN0b3JWaWV3TW9kZWwsIE9uZURhc01vZHVsZVZpZXdNb2RlbFtdPigpO1xyXG5cclxuICAgICAgICB0aGlzLkludGVybmFsQ3JlYXRlTmV3TW9kdWxlKClcclxuICAgICAgICB0aGlzLkludGVybmFsVXBkYXRlKClcclxuICAgIH1cclxuXHJcbiAgICBnZXQgT25Nb2R1bGVTZXRDaGFuZ2VkKCk6IElFdmVudDxPbmVEYXNNb2R1bGVTZWxlY3RvclZpZXdNb2RlbCwgT25lRGFzTW9kdWxlVmlld01vZGVsW10+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuX29uTW9kdWxlU2V0Q2hhbmdlZDtcclxuICAgIH1cclxuICAgIFxyXG4gICAgLy8gbWV0aG9kc1xyXG4gICAgcHVibGljIFNldE1heEJ5dGVzID0gKHZhbHVlOiBudW1iZXIpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5NYXhCeXRlcyh2YWx1ZSlcclxuICAgICAgICB0aGlzLkludGVybmFsVXBkYXRlKClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgR2V0SW5wdXRNb2R1bGVTZXQgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLk1vZHVsZVNldCgpLmZpbHRlcihtb2R1bGUgPT4gbW9kdWxlLkRhdGFEaXJlY3Rpb24oKSA9PT0gRGF0YURpcmVjdGlvbkVudW0uSW5wdXQpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEdldE91dHB1dE1vZHVsZVNldCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuTW9kdWxlU2V0KCkuZmlsdGVyKG1vZHVsZSA9PiBtb2R1bGUuRGF0YURpcmVjdGlvbigpID09PSBEYXRhRGlyZWN0aW9uRW51bS5PdXRwdXQpXHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBJbnRlcm5hbFVwZGF0ZSgpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5VcGRhdGUoKVxyXG4gICAgICAgIHRoaXMuVmFsaWRhdGUoKVxyXG4gICAgfVxyXG5cclxuICAgIHByb3RlY3RlZCBVcGRhdGUoKVxyXG4gICAge1xyXG4gICAgICAgIGxldCBtb2R1bGVTZXQ6IE9uZURhc01vZHVsZVZpZXdNb2RlbFtdXHJcbiAgICAgICAgbGV0IHJlbWFpbmluZ0J5dGVzOiBudW1iZXJcclxuXHJcbiAgICAgICAgc3dpdGNoICh0aGlzLk5ld01vZHVsZSgpLkRhdGFEaXJlY3Rpb24oKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uSW5wdXQ6XHJcbiAgICAgICAgICAgICAgICBtb2R1bGVTZXQgPSB0aGlzLkdldElucHV0TW9kdWxlU2V0KClcclxuICAgICAgICAgICAgICAgIGJyZWFrO1xyXG5cclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5PdXRwdXQ6XHJcbiAgICAgICAgICAgICAgICBtb2R1bGVTZXQgPSB0aGlzLkdldE91dHB1dE1vZHVsZVNldCgpXHJcbiAgICAgICAgICAgICAgICBicmVhaztcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJlbWFpbmluZ0J5dGVzID0gdGhpcy5NYXhCeXRlcygpIC0gbW9kdWxlU2V0Lm1hcChvbmVEYXNNb2R1bGUgPT4gb25lRGFzTW9kdWxlLkdldEJ5dGVDb3VudCgpKS5yZWR1Y2UoKHByZXZpb3VzVmFsdWUsIGN1cnJlbnRWYWx1ZSkgPT4gcHJldmlvdXNWYWx1ZSArIGN1cnJlbnRWYWx1ZSwgMClcclxuXHJcbiAgICAgICAgdGhpcy5SZW1haW5pbmdCeXRlcyhyZW1haW5pbmdCeXRlcylcclxuICAgICAgICB0aGlzLlJlbWFpbmluZ0NvdW50KE1hdGguZmxvb3IodGhpcy5SZW1haW5pbmdCeXRlcygpIC8gKCh0aGlzLk5ld01vZHVsZSgpLkRhdGFUeXBlKCkgJiAweDBGRikgLyA4KSkpXHJcbiAgICB9XHJcblxyXG4gICAgcHJvdGVjdGVkIFZhbGlkYXRlKClcclxuICAgIHtcclxuICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIlwiKVxyXG5cclxuICAgICAgICBpZiAodGhpcy5OZXdNb2R1bGUoKS5IYXNFcnJvcigpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJSZXNvbHZlIGFsbCByZW1haW5pbmcgbW9kdWxlIGVycm9ycyBiZWZvcmUgY29udGludWluZy5cIilcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmICh0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yTW9kZSgpID09PSBPbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtLklucHV0T25seSAmJiB0aGlzLk5ld01vZHVsZSgpLkRhdGFEaXJlY3Rpb24oKSA9PSBEYXRhRGlyZWN0aW9uRW51bS5PdXRwdXQpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIk9ubHkgaW5wdXQgbW9kdWxlcyBhcmUgYWxsb3dlZC5cIilcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmICh0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yTW9kZSgpID09PSBPbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtLk91dHB1dE9ubHkgJiYgdGhpcy5OZXdNb2R1bGUoKS5EYXRhRGlyZWN0aW9uKCkgPT0gRGF0YURpcmVjdGlvbkVudW0uSW5wdXQpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIk9ubHkgb3V0cHV0IG1vZHVsZXMgYXJlIGFsbG93ZWQuXCIpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBpZiAoaXNGaW5pdGUodGhpcy5SZW1haW5pbmdCeXRlcygpKSAmJiAodGhpcy5SZW1haW5pbmdCeXRlcygpIC0gdGhpcy5OZXdNb2R1bGUoKS5HZXRCeXRlQ291bnQoKSA8IDApKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJCeXRlIGNvdW50IG9mIG5ldyBtb2R1bGUgaXMgdG9vIGhpZ2guXCIpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBpZiAodGhpcy5SZW1haW5pbmdDb3VudCgpIDw9IDApXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIlRoZSBtYXhpbXVtIG51bWJlciBvZiBtb2R1bGVzIGlzIHJlYWNoZWQuXCIpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHByb3RlY3RlZCBDcmVhdGVOZXdNb2R1bGUoKVxyXG4gICAge1xyXG4gICAgICAgIGlmICh0aGlzLk5ld01vZHVsZSgpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgcmV0dXJuIG5ldyBPbmVEYXNNb2R1bGVWaWV3TW9kZWwobmV3IE9uZURhc01vZHVsZU1vZGVsKHRoaXMuTmV3TW9kdWxlKCkuRGF0YVR5cGUoKSwgdGhpcy5OZXdNb2R1bGUoKS5EYXRhRGlyZWN0aW9uKCksIHRoaXMuTmV3TW9kdWxlKCkuRW5kaWFubmVzcygpLCAxKSlcclxuICAgICAgICB9XHJcbiAgICAgICAgZWxzZVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgcmV0dXJuIG5ldyBPbmVEYXNNb2R1bGVWaWV3TW9kZWwobmV3IE9uZURhc01vZHVsZU1vZGVsKE9uZURhc0RhdGFUeXBlRW51bS5VSU5UMTYsIERhdGFEaXJlY3Rpb25FbnVtLklucHV0LCBFbmRpYW5uZXNzRW51bS5MaXR0bGVFbmRpYW4sIDEpKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIEludGVybmFsQ3JlYXRlTmV3TW9kdWxlKClcclxuICAgIHtcclxuICAgICAgICBpZiAodGhpcy5OZXdNb2R1bGUoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuTmV3TW9kdWxlKCkuUHJvcGVydHlDaGFuZ2VkLnVuc3Vic2NyaWJlKHRoaXMuT25Nb2R1bGVQcm9wZXJ0eUNoYW5nZWQpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0aGlzLk5ld01vZHVsZSh0aGlzLkNyZWF0ZU5ld01vZHVsZSgpKVxyXG4gICAgICAgIHRoaXMuTmV3TW9kdWxlKCkuUHJvcGVydHlDaGFuZ2VkLnN1YnNjcmliZSh0aGlzLk9uTW9kdWxlUHJvcGVydHlDaGFuZ2VkKVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgT25Nb2R1bGVQcm9wZXJ0eUNoYW5nZWQgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgfVxyXG5cclxuICAgIC8vIGNvbW1hbmRzXHJcbiAgICBwdWJsaWMgQWRkTW9kdWxlID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICBsZXQgbmV3TW9kdWxlOiBPbmVEYXNNb2R1bGVWaWV3TW9kZWxcclxuXHJcbiAgICAgICAgaWYgKCF0aGlzLkhhc0Vycm9yKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLk1vZHVsZVNldC5wdXNoKHRoaXMuTmV3TW9kdWxlKCkpXHJcbiAgICAgICAgICAgIHRoaXMuSW50ZXJuYWxDcmVhdGVOZXdNb2R1bGUoKVxyXG4gICAgICAgICAgICB0aGlzLkludGVybmFsVXBkYXRlKClcclxuICAgICAgICAgICAgdGhpcy5fb25Nb2R1bGVTZXRDaGFuZ2VkLmRpc3BhdGNoKHRoaXMsIHRoaXMuTW9kdWxlU2V0KCkpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBEZWxldGVNb2R1bGUgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuTW9kdWxlU2V0LnBvcCgpXHJcbiAgICAgICAgdGhpcy5JbnRlcm5hbFVwZGF0ZSgpXHJcbiAgICAgICAgdGhpcy5fb25Nb2R1bGVTZXRDaGFuZ2VkLmRpc3BhdGNoKHRoaXMsIHRoaXMuTW9kdWxlU2V0KCkpXHJcbiAgICB9XHJcbn0iLCJjbGFzcyBUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsXHJcbntcclxuICAgIHB1YmxpYyBEYXRlVGltZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBUeXBlOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIE9wdGlvbjogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBBcmd1bWVudDogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuXHJcbiAgICBjb25zdHJ1Y3Rvcih0cmFuc2ZlckZ1bmN0aW9uTW9kZWw6IFRyYW5zZmVyRnVuY3Rpb25Nb2RlbClcclxuICAgIHtcclxuICAgICAgICB0aGlzLkRhdGVUaW1lID0ga28ub2JzZXJ2YWJsZSh0cmFuc2ZlckZ1bmN0aW9uTW9kZWwuRGF0ZVRpbWUpXHJcbiAgICAgICAgdGhpcy5UeXBlID0ga28ub2JzZXJ2YWJsZSh0cmFuc2ZlckZ1bmN0aW9uTW9kZWwuVHlwZSlcclxuICAgICAgICB0aGlzLk9wdGlvbiA9IGtvLm9ic2VydmFibGUodHJhbnNmZXJGdW5jdGlvbk1vZGVsLk9wdGlvbilcclxuICAgICAgICB0aGlzLkFyZ3VtZW50ID0ga28ub2JzZXJ2YWJsZSh0cmFuc2ZlckZ1bmN0aW9uTW9kZWwuQXJndW1lbnQpXHJcbiAgICB9XHJcblxyXG4gICAgLy8gbWV0aG9kc1xyXG4gICAgcHVibGljIFRvTW9kZWwoKVxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiBuZXcgVHJhbnNmZXJGdW5jdGlvbk1vZGVsKHRoaXMuRGF0ZVRpbWUoKSwgdGhpcy5UeXBlKCksIHRoaXMuT3B0aW9uKCksIHRoaXMuQXJndW1lbnQoKSlcclxuICAgIH1cclxufSIsImNsYXNzIEJ1ZmZlclJlcXVlc3RTZWxlY3RvclZpZXdNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgTmV3QnVmZmVyUmVxdWVzdDogS25vY2tvdXRPYnNlcnZhYmxlPEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWw+ICBcclxuICAgIHB1YmxpYyBCdWZmZXJSZXF1ZXN0U2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxCdWZmZXJSZXF1ZXN0Vmlld01vZGVsPlxyXG4gICAgcHVibGljIEVycm9yTWVzc2FnZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBIYXNFcnJvcjogS25vY2tvdXRDb21wdXRlZDxib29sZWFuPlxyXG5cclxuICAgIHByaXZhdGUgX29uQnVmZmVyUmVxdWVzdFNldENoYW5nZWQ6IEV2ZW50RGlzcGF0Y2hlcjxCdWZmZXJSZXF1ZXN0U2VsZWN0b3JWaWV3TW9kZWwsIEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWxbXT5cclxuXHJcbiAgICBjb25zdHJ1Y3RvcihidWZmZXJSZXF1ZXN0U2V0OiBCdWZmZXJSZXF1ZXN0Vmlld01vZGVsW10gPSBbXSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLk5ld0J1ZmZlclJlcXVlc3QgPSBrby5vYnNlcnZhYmxlPEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWw+KCk7XHJcbiAgICAgICAgdGhpcy5CdWZmZXJSZXF1ZXN0U2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWw+KGJ1ZmZlclJlcXVlc3RTZXQpO1xyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KFwiXCIpXHJcbiAgICAgICAgdGhpcy5IYXNFcnJvciA9IGtvLmNvbXB1dGVkPGJvb2xlYW4+KCgpID0+IHRoaXMuRXJyb3JNZXNzYWdlKCkubGVuZ3RoID4gMClcclxuXHJcbiAgICAgICAgdGhpcy5fb25CdWZmZXJSZXF1ZXN0U2V0Q2hhbmdlZCA9IG5ldyBFdmVudERpc3BhdGNoZXI8QnVmZmVyUmVxdWVzdFNlbGVjdG9yVmlld01vZGVsLCBCdWZmZXJSZXF1ZXN0Vmlld01vZGVsW10+KCk7XHJcblxyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxDcmVhdGVOZXdCdWZmZXJSZXF1ZXN0KClcclxuICAgICAgICB0aGlzLkludGVybmFsVXBkYXRlKClcclxuICAgIH1cclxuXHJcbiAgICBnZXQgT25CdWZmZXJSZXF1ZXN0U2V0Q2hhbmdlZCgpOiBJRXZlbnQ8QnVmZmVyUmVxdWVzdFNlbGVjdG9yVmlld01vZGVsLCBCdWZmZXJSZXF1ZXN0Vmlld01vZGVsW10+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuX29uQnVmZmVyUmVxdWVzdFNldENoYW5nZWQ7XHJcbiAgICB9XHJcbiAgICBcclxuICAgIC8vIG1ldGhvZHNcclxuICAgIHByaXZhdGUgSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuVXBkYXRlKClcclxuICAgICAgICB0aGlzLlZhbGlkYXRlKClcclxuICAgIH1cclxuXHJcbiAgICBwcm90ZWN0ZWQgVXBkYXRlKClcclxuICAgIHtcclxuICAgICAgICAvL1xyXG4gICAgfVxyXG5cclxuICAgIHByb3RlY3RlZCBWYWxpZGF0ZSgpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJcIilcclxuXHJcbiAgICAgICAgaWYgKHRoaXMuTmV3QnVmZmVyUmVxdWVzdCgpLkhhc0Vycm9yKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIlJlc29sdmUgYWxsIHJlbWFpbmluZyBlcnJvcnMgYmVmb3JlIGNvbnRpbnVpbmcuXCIpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHByb3RlY3RlZCBDcmVhdGVOZXdCdWZmZXJSZXF1ZXN0KClcclxuICAgIHtcclxuICAgICAgICBpZiAodGhpcy5OZXdCdWZmZXJSZXF1ZXN0KCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICByZXR1cm4gbmV3IEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWwobmV3IEJ1ZmZlclJlcXVlc3RNb2RlbCh0aGlzLk5ld0J1ZmZlclJlcXVlc3QoKS5TYW1wbGVSYXRlKCksIHRoaXMuTmV3QnVmZmVyUmVxdWVzdCgpLkdyb3VwRmlsdGVyKCkpKVxyXG4gICAgICAgIH1cclxuICAgICAgICBlbHNlXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICByZXR1cm4gbmV3IEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWwobmV3IEJ1ZmZlclJlcXVlc3RNb2RlbChTYW1wbGVSYXRlRW51bS5TYW1wbGVSYXRlXzEsIFwiKlwiKSlcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBJbnRlcm5hbENyZWF0ZU5ld0J1ZmZlclJlcXVlc3QoKVxyXG4gICAge1xyXG4gICAgICAgIGlmICh0aGlzLk5ld0J1ZmZlclJlcXVlc3QoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuTmV3QnVmZmVyUmVxdWVzdCgpLlByb3BlcnR5Q2hhbmdlZC51bnN1YnNjcmliZSh0aGlzLk9uQnVmZmVyUmVxdWVzdFByb3BlcnR5Q2hhbmdlZClcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuTmV3QnVmZmVyUmVxdWVzdCh0aGlzLkNyZWF0ZU5ld0J1ZmZlclJlcXVlc3QoKSlcclxuICAgICAgICB0aGlzLk5ld0J1ZmZlclJlcXVlc3QoKS5Qcm9wZXJ0eUNoYW5nZWQuc3Vic2NyaWJlKHRoaXMuT25CdWZmZXJSZXF1ZXN0UHJvcGVydHlDaGFuZ2VkKVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgT25CdWZmZXJSZXF1ZXN0UHJvcGVydHlDaGFuZ2VkID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLkludGVybmFsVXBkYXRlKClcclxuICAgIH1cclxuXHJcbiAgICAvLyBjb21tYW5kc1xyXG4gICAgcHVibGljIEFkZEJ1ZmZlclJlcXVlc3QgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIGxldCBuZXdCdWZmZXJSZXF1ZXN0OiBCdWZmZXJSZXF1ZXN0Vmlld01vZGVsXHJcblxyXG4gICAgICAgIGlmICghdGhpcy5IYXNFcnJvcigpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5CdWZmZXJSZXF1ZXN0U2V0LnB1c2godGhpcy5OZXdCdWZmZXJSZXF1ZXN0KCkpXHJcbiAgICAgICAgICAgIHRoaXMuSW50ZXJuYWxDcmVhdGVOZXdCdWZmZXJSZXF1ZXN0KClcclxuICAgICAgICAgICAgdGhpcy5JbnRlcm5hbFVwZGF0ZSgpXHJcbiAgICAgICAgICAgIHRoaXMuX29uQnVmZmVyUmVxdWVzdFNldENoYW5nZWQuZGlzcGF0Y2godGhpcywgdGhpcy5CdWZmZXJSZXF1ZXN0U2V0KCkpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBEZWxldGVCdWZmZXJSZXF1ZXN0ID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLkJ1ZmZlclJlcXVlc3RTZXQucG9wKClcclxuICAgICAgICB0aGlzLkludGVybmFsVXBkYXRlKClcclxuICAgICAgICB0aGlzLl9vbkJ1ZmZlclJlcXVlc3RTZXRDaGFuZ2VkLmRpc3BhdGNoKHRoaXMsIHRoaXMuQnVmZmVyUmVxdWVzdFNldCgpKVxyXG4gICAgfVxyXG59IiwiY2xhc3MgQnVmZmVyUmVxdWVzdFZpZXdNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgU2FtcGxlUmF0ZTogS25vY2tvdXRPYnNlcnZhYmxlPFNhbXBsZVJhdGVFbnVtPlxyXG4gICAgcHVibGljIEdyb3VwRmlsdGVyOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIEVycm9yTWVzc2FnZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBIYXNFcnJvcjogS25vY2tvdXRDb21wdXRlZDxib29sZWFuPlxyXG5cclxuICAgIHB1YmxpYyBTYW1wbGVSYXRlU2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxTYW1wbGVSYXRlRW51bT5cclxuXHJcbiAgICBwcml2YXRlIF9vblByb3BlcnR5Q2hhbmdlZDogRXZlbnREaXNwYXRjaGVyPEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWwsIGFueT5cclxuXHJcbiAgICBjb25zdHJ1Y3Rvcihtb2RlbDogQnVmZmVyUmVxdWVzdE1vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuU2FtcGxlUmF0ZVNldCA9IGtvLm9ic2VydmFibGVBcnJheTxTYW1wbGVSYXRlRW51bT4oRW51bWVyYXRpb25IZWxwZXIuR2V0RW51bVZhbHVlcyhcIlNhbXBsZVJhdGVFbnVtXCIpKVxyXG4gICAgICAgIHRoaXMuU2FtcGxlUmF0ZSA9IGtvLm9ic2VydmFibGU8U2FtcGxlUmF0ZUVudW0+KG1vZGVsLlNhbXBsZVJhdGUpO1xyXG4gICAgICAgIHRoaXMuR3JvdXBGaWx0ZXIgPSBrby5vYnNlcnZhYmxlPHN0cmluZz4obW9kZWwuR3JvdXBGaWx0ZXIpO1xyXG5cclxuICAgICAgICB0aGlzLkVycm9yTWVzc2FnZSA9IGtvLm9ic2VydmFibGU8c3RyaW5nPihcIlwiKVxyXG4gICAgICAgIHRoaXMuSGFzRXJyb3IgPSBrby5jb21wdXRlZDxib29sZWFuPigoKSA9PiB0aGlzLkVycm9yTWVzc2FnZSgpLmxlbmd0aCA+IDApXHJcblxyXG4gICAgICAgIHRoaXMuX29uUHJvcGVydHlDaGFuZ2VkID0gbmV3IEV2ZW50RGlzcGF0Y2hlcjxCdWZmZXJSZXF1ZXN0Vmlld01vZGVsLCBhbnk+KCk7XHJcblxyXG4gICAgICAgIHRoaXMuU2FtcGxlUmF0ZS5zdWJzY3JpYmUobmV3VmFsdWUgPT4gdGhpcy5PblByb3BlcnR5Q2hhbmdlZCgpKVxyXG4gICAgICAgIHRoaXMuR3JvdXBGaWx0ZXIuc3Vic2NyaWJlKG5ld1ZhbHVlID0+IHRoaXMuT25Qcm9wZXJ0eUNoYW5nZWQoKSlcclxuICAgIH1cclxuXHJcbiAgICBnZXQgUHJvcGVydHlDaGFuZ2VkKCk6IElFdmVudDxCdWZmZXJSZXF1ZXN0Vmlld01vZGVsLCBhbnk+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuX29uUHJvcGVydHlDaGFuZ2VkO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBPblByb3BlcnR5Q2hhbmdlZCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5WYWxpZGF0ZSgpXHJcbiAgICAgICAgdGhpcy5fb25Qcm9wZXJ0eUNoYW5nZWQuZGlzcGF0Y2godGhpcywgbnVsbClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVmFsaWRhdGUoKVxyXG4gICAge1xyXG4gICAgICAgIGxldCByZXN1bHQ6IGFueVxyXG5cclxuICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIlwiKVxyXG5cclxuICAgICAgICB0aGlzLkdyb3VwRmlsdGVyKCkuc3BsaXQoXCI7XCIpLmZvckVhY2goZ3JvdXBGaWx0ZXIgPT5cclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHJlc3VsdCA9IHRoaXMuQ2hlY2tHcm91cEZpbHRlcihncm91cEZpbHRlcilcclxuXHJcbiAgICAgICAgICAgIGlmIChyZXN1bHQuSGFzRXJyb3IpXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKHJlc3VsdC5FcnJvckRlc2NyaXB0aW9uKVxyXG5cclxuICAgICAgICAgICAgICAgIHJldHVyblxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9TdHJpbmcoKVxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiBcInNhbXBsZSByYXRlOiBcIiArIEVudW1lcmF0aW9uSGVscGVyLkdldEVudW1Mb2NhbGl6YXRpb24oXCJTYW1wbGVSYXRlRW51bVwiLCB0aGlzLlNhbXBsZVJhdGUoKSkgKyBcIiAvIGdyb3VwIGZpbHRlcjogJ1wiICsgdGhpcy5Hcm91cEZpbHRlcigpICsgXCInXCJcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IG1vZGVsOiBhbnkgPSB7XHJcbiAgICAgICAgICAgIFNhbXBsZVJhdGU6IDxTYW1wbGVSYXRlRW51bT50aGlzLlNhbXBsZVJhdGUoKSxcclxuICAgICAgICAgICAgR3JvdXBGaWx0ZXI6IDxzdHJpbmc+dGhpcy5Hcm91cEZpbHRlcigpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gbW9kZWxcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIENoZWNrR3JvdXBGaWx0ZXIodmFsdWU6IHN0cmluZylcclxuICAgIHtcclxuICAgICAgICB2YXIgcmVnRXhwOiBhbnlcclxuXHJcbiAgICAgICAgaWYgKHZhbHVlLmxlbmd0aCA9PT0gMCkge1xyXG4gICAgICAgICAgICByZXR1cm4geyBIYXNFcnJvcjogdHJ1ZSwgRXJyb3JEZXNjcmlwdGlvbjogRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9Hcm91cEZpbHRlckVtcHR5XCJdIH1cclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJlZ0V4cCA9IG5ldyBSZWdFeHAoXCJbXkEtWmEtejAtOV8hKl1cIilcclxuXHJcbiAgICAgICAgaWYgKHJlZ0V4cC50ZXN0KHZhbHVlKSkge1xyXG4gICAgICAgICAgICByZXR1cm4geyBIYXNFcnJvcjogdHJ1ZSwgRXJyb3JEZXNjcmlwdGlvbjogRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9JbnZhbGlkQ2hhcmFjdGVyc1wiXSB9XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZWdFeHAgPSBuZXcgUmVnRXhwKFwiXlswLTlfXVwiKVxyXG5cclxuICAgICAgICBpZiAocmVnRXhwLnRlc3QodmFsdWUpKSB7XHJcbiAgICAgICAgICAgIHJldHVybiB7IEhhc0Vycm9yOiB0cnVlLCBFcnJvckRlc2NyaXB0aW9uOiBFcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0ludmFsaWRMZWFkaW5nQ2hhcmFjdGVyXCJdIH1cclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJlZ0V4cCA9IG5ldyBSZWdFeHAoXCJeIVwiKVxyXG5cclxuICAgICAgICBpZiAocmVnRXhwLnRlc3QodmFsdWUpICYmIHZhbHVlLmxlbmd0aCA9PT0gMSkge1xyXG4gICAgICAgICAgICByZXR1cm4geyBIYXNFcnJvcjogdHJ1ZSwgRXJyb3JEZXNjcmlwdGlvbjogRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9EZXRhY2hlZEV4Y2xhbWF0aW9uTWFya05vdEFsbG93ZWRcIl0gfVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmV0dXJuIHtcclxuICAgICAgICAgICAgSGFzRXJyb3I6IGZhbHNlLFxyXG4gICAgICAgICAgICBFcnJvckRlc2NyaXB0aW9uOiBcIlwiXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG59IiwiY2xhc3MgRGF0YVBvcnRWaWV3TW9kZWxcclxue1xyXG4gICAgLy8gZmllbGRzXHJcbiAgICBwdWJsaWMgTmFtZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyByZWFkb25seSBEYXRhVHlwZTogT25lRGFzRGF0YVR5cGVFbnVtXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRGF0YURpcmVjdGlvbjogRGF0YURpcmVjdGlvbkVudW1cclxuICAgIHB1YmxpYyByZWFkb25seSBFbmRpYW5uZXNzOiBFbmRpYW5uZXNzRW51bVxyXG5cclxuICAgIHB1YmxpYyBJc1NlbGVjdGVkOiBLbm9ja291dE9ic2VydmFibGU8Ym9vbGVhbj5cclxuICAgIHB1YmxpYyBBc3NvY2lhdGVkQ2hhbm5lbEh1YlNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8Q2hhbm5lbEh1YlZpZXdNb2RlbD5cclxuICAgIHB1YmxpYyByZWFkb25seSBBc3NvY2lhdGVkRGF0YUdhdGV3YXk6IERhdGFHYXRld2F5Vmlld01vZGVsQmFzZVxyXG4gICAgcHVibGljIHJlYWRvbmx5IExpdmVEZXNjcmlwdGlvbjogS25vY2tvdXRDb21wdXRlZDxzdHJpbmc+XHJcblxyXG4gICAgLy8gY29uc3RydWN0b3JzXHJcbiAgICBjb25zdHJ1Y3RvcihkYXRhUG9ydE1vZGVsOiBhbnksIGFzc29jaWF0ZWREYXRhR2F0ZXdheTogRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuTmFtZSA9IGtvLm9ic2VydmFibGUoZGF0YVBvcnRNb2RlbC5OYW1lKVxyXG4gICAgICAgIHRoaXMuRGF0YVR5cGUgPSBkYXRhUG9ydE1vZGVsLkRhdGFUeXBlXHJcbiAgICAgICAgdGhpcy5EYXRhRGlyZWN0aW9uID0gZGF0YVBvcnRNb2RlbC5EYXRhRGlyZWN0aW9uXHJcbiAgICAgICAgdGhpcy5FbmRpYW5uZXNzID0gZGF0YVBvcnRNb2RlbC5FbmRpYW5uZXNzXHJcblxyXG4gICAgICAgIHRoaXMuSXNTZWxlY3RlZCA9IGtvLm9ic2VydmFibGU8Ym9vbGVhbj4oZmFsc2UpXHJcbiAgICAgICAgdGhpcy5Bc3NvY2lhdGVkQ2hhbm5lbEh1YlNldCA9IGtvLm9ic2VydmFibGVBcnJheTxDaGFubmVsSHViVmlld01vZGVsPigpXHJcbiAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YUdhdGV3YXkgPSBhc3NvY2lhdGVkRGF0YUdhdGV3YXlcclxuXHJcbiAgICAgICAgdGhpcy5MaXZlRGVzY3JpcHRpb24gPSBrby5jb21wdXRlZCgoKSA9PlxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgbGV0IHJlc3VsdDogc3RyaW5nXHJcblxyXG4gICAgICAgICAgICByZXN1bHQgPSBcIjxkaXYgY2xhc3M9J3RleHQtbGVmdCc+XCIgKyB0aGlzLk5hbWUoKSArIFwiPC9kaXY+PGRpdiBjbGFzcz0ndGV4dC1sZWZ0Jz5cIiArIEVudW1lcmF0aW9uSGVscGVyLkdldEVudW1Mb2NhbGl6YXRpb24oJ09uZURhc0RhdGFUeXBlRW51bScsIHRoaXMuRGF0YVR5cGUpICsgXCI8L2Rpdj5cIlxyXG5cclxuICAgICAgICAgICAgaWYgKHRoaXMuQXNzb2NpYXRlZENoYW5uZWxIdWJTZXQoKS5sZW5ndGggPiAwKVxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWRDaGFubmVsSHViU2V0KCkuZm9yRWFjaChjaGFubmVsSHViID0+XHJcbiAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgcmVzdWx0ICs9IFwiPC9iciA+PGRpdiBjbGFzcz0ndGV4dC1sZWZ0Jz5cIiArIGNoYW5uZWxIdWIuTmFtZSgpICsgXCI8L2Rpdj48ZGl2IGNsYXNzPSd0ZXh0LWxlZnQnPlwiICsgRW51bWVyYXRpb25IZWxwZXIuR2V0RW51bUxvY2FsaXphdGlvbignT25lRGFzRGF0YVR5cGVFbnVtJywgY2hhbm5lbEh1Yi5EYXRhVHlwZSgpKSArIFwiPC9kaXY+XCJcclxuICAgICAgICAgICAgICAgIH0pXHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIHJldHVybiByZXN1bHRcclxuICAgICAgICB9KVxyXG4gICAgfVxyXG5cclxuICAgIC8vIG1ldGhvZHNcclxuICAgIHB1YmxpYyBHZXRJZCgpOiBzdHJpbmdcclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5OYW1lKClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9GdWxsUXVhbGlmaWVkSWRlbnRpZmllcigpOiBzdHJpbmdcclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5Bc3NvY2lhdGVkRGF0YUdhdGV3YXkuRGVzY3JpcHRpb24uSWQgKyBcIiAoXCIgKyB0aGlzLkFzc29jaWF0ZWREYXRhR2F0ZXdheS5EZXNjcmlwdGlvbi5JbnN0YW5jZUlkICsgXCIpIC8gXCIgKyB0aGlzLkdldElkKCk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEV4dGVuZE1vZGVsKG1vZGVsOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgLy9cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IG1vZGVsOiBhbnkgPSB7XHJcbiAgICAgICAgICAgIE5hbWU6IDxzdHJpbmc+dGhpcy5OYW1lKCksXHJcbiAgICAgICAgICAgIERhdGFUeXBlOiA8T25lRGFzRGF0YVR5cGVFbnVtPnRoaXMuRGF0YVR5cGUsXHJcbiAgICAgICAgICAgIERhdGFEaXJlY3Rpb246IDxEYXRhRGlyZWN0aW9uRW51bT50aGlzLkRhdGFEaXJlY3Rpb25cclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuRXh0ZW5kTW9kZWwobW9kZWwpXHJcblxyXG4gICAgICAgIHJldHVybiBtb2RlbFxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBSZXNldEFzc29jaWF0aW9ucyhtYWludGFpbldlYWtSZWZlcmVuY2U6IGJvb2xlYW4pXHJcbiAgICB7XHJcbiAgICAgICAgaWYgKHRoaXMuQXNzb2NpYXRlZENoYW5uZWxIdWJTZXQoKS5sZW5ndGggPiAwKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkQ2hhbm5lbEh1YlNldCgpLmZvckVhY2goY2hhbm5lbEh1YiA9PiBjaGFubmVsSHViLlJlc2V0QXNzb2NpYXRpb24obWFpbnRhaW5XZWFrUmVmZXJlbmNlLCB0aGlzKSlcclxuICAgICAgICB9XHJcbiAgICB9XHJcbn0iLCJhYnN0cmFjdCBjbGFzcyBQbHVnaW5WaWV3TW9kZWxCYXNlXHJcbntcclxuICAgIHB1YmxpYyBEZXNjcmlwdGlvbjogUGx1Z2luRGVzY3JpcHRpb25WaWV3TW9kZWxcclxuICAgIHB1YmxpYyBQbHVnaW5JZGVudGlmaWNhdGlvbjogUGx1Z2luSWRlbnRpZmljYXRpb25WaWV3TW9kZWxcclxuICAgIHB1YmxpYyBJc0luU2V0dGluZ3NNb2RlOiBLbm9ja291dE9ic2VydmFibGU8Ym9vbGVhbj5cclxuXHJcbiAgICBwcml2YXRlIF9tb2RlbDogYW55XHJcblxyXG4gICAgY29uc3RydWN0b3IocGx1Z2luU2V0dGluZ3NNb2RlbDogYW55LCBwbHVnaW5JZGVudGlmaWNhdGlvbjogUGx1Z2luSWRlbnRpZmljYXRpb25WaWV3TW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5fbW9kZWwgPSBwbHVnaW5TZXR0aW5nc01vZGVsXHJcbiAgICAgICAgdGhpcy5EZXNjcmlwdGlvbiA9IG5ldyBQbHVnaW5EZXNjcmlwdGlvblZpZXdNb2RlbChwbHVnaW5TZXR0aW5nc01vZGVsLkRlc2NyaXB0aW9uKVxyXG4gICAgICAgIHRoaXMuUGx1Z2luSWRlbnRpZmljYXRpb24gPSBwbHVnaW5JZGVudGlmaWNhdGlvblxyXG4gICAgICAgIHRoaXMuSXNJblNldHRpbmdzTW9kZSA9IGtvLm9ic2VydmFibGU8Ym9vbGVhbj4oZmFsc2UpXHJcbiAgICB9XHJcblxyXG4gICAgLy8gbWV0aG9kc1xyXG4gICAgcHVibGljIGFic3RyYWN0IGFzeW5jIEluaXRpYWxpemVBc3luYygpOiBQcm9taXNlPGFueT5cclxuXHJcbiAgICBwdWJsaWMgU2VuZEFjdGlvblJlcXVlc3QgPSBhc3luYyAoaW5zdGFuY2VJZDogbnVtYmVyLCBtZXRob2ROYW1lOiBzdHJpbmcsIGRhdGE6IGFueSkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gPEFjdGlvblJlc3BvbnNlPiBhd2FpdCBDb25uZWN0aW9uTWFuYWdlci5JbnZva2VXZWJDbGllbnRIdWIoXCJSZXF1ZXN0QWN0aW9uXCIsIG5ldyBBY3Rpb25SZXF1ZXN0KHRoaXMuRGVzY3JpcHRpb24uSWQsIGluc3RhbmNlSWQsIG1ldGhvZE5hbWUsIGRhdGEpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBFeHRlbmRNb2RlbChtb2RlbDogYW55KVxyXG4gICAge1xyXG4gICAgICAgIC8vXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvTW9kZWwoKVxyXG4gICAge1xyXG4gICAgICAgIGxldCBtb2RlbDogYW55ID0ge1xyXG4gICAgICAgICAgICAkdHlwZTogPHN0cmluZz50aGlzLl9tb2RlbC4kdHlwZSxcclxuICAgICAgICAgICAgRGVzY3JpcHRpb246IDxQbHVnaW5EZXNjcmlwdGlvblZpZXdNb2RlbD50aGlzLkRlc2NyaXB0aW9uLlRvTW9kZWwoKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5FeHRlbmRNb2RlbChtb2RlbClcclxuXHJcbiAgICAgICAgcmV0dXJuIG1vZGVsXHJcbiAgICB9XHJcblxyXG4gICAgLy8gY29tbWFuZHNcclxuICAgIHB1YmxpYyBFbmFibGVTZXR0aW5nc01vZGUgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuSXNJblNldHRpbmdzTW9kZSh0cnVlKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBEaXNhYmxlU2V0dGluZ3NNb2RlID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLklzSW5TZXR0aW5nc01vZGUoZmFsc2UpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvZ2dsZVNldHRpbmdzTW9kZSA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5Jc0luU2V0dGluZ3NNb2RlKCF0aGlzLklzSW5TZXR0aW5nc01vZGUoKSlcclxuICAgIH1cclxufSIsIi8vLyA8cmVmZXJlbmNlIHBhdGg9XCJQbHVnaW5WaWV3TW9kZWxCYXNlLnRzXCIvPlxyXG5cclxuYWJzdHJhY3QgY2xhc3MgRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlIGV4dGVuZHMgUGx1Z2luVmlld01vZGVsQmFzZVxyXG57XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgTWF4aW11bURhdGFzZXRBZ2U6IEtub2Nrb3V0T2JzZXJ2YWJsZTxudW1iZXI+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRGF0YVBvcnRTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PERhdGFQb3J0Vmlld01vZGVsPlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKG1vZGVsLCBpZGVudGlmaWNhdGlvbjogUGx1Z2luSWRlbnRpZmljYXRpb25WaWV3TW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgc3VwZXIobW9kZWwsIGlkZW50aWZpY2F0aW9uKVxyXG5cclxuICAgICAgICB0aGlzLk1heGltdW1EYXRhc2V0QWdlID0ga28ub2JzZXJ2YWJsZTxudW1iZXI+KG1vZGVsLk1heGltdW1EYXRhc2V0QWdlKVxyXG4gICAgICAgIHRoaXMuRGF0YVBvcnRTZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8RGF0YVBvcnRWaWV3TW9kZWw+KClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgRXh0ZW5kTW9kZWwobW9kZWw6IGFueSlcclxuICAgIHtcclxuICAgICAgICBzdXBlci5FeHRlbmRNb2RlbChtb2RlbClcclxuXHJcbiAgICAgICAgbW9kZWwuTWF4aW11bURhdGFzZXRBZ2UgPSA8bnVtYmVyPk51bWJlci5wYXJzZUludCg8YW55PnRoaXMuTWF4aW11bURhdGFzZXRBZ2UoKSlcclxuICAgIH1cclxufSIsImFic3RyYWN0IGNsYXNzIEV4dGVuZGVkRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlIGV4dGVuZHMgRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlXHJcbntcclxuICAgIHB1YmxpYyBNb2R1bGVUb0RhdGFQb3J0TWFwOiBLbm9ja291dE9ic2VydmFibGVBcnJheTxPYnNlcnZhYmxlR3JvdXA8RGF0YVBvcnRWaWV3TW9kZWw+PlxyXG4gICAgcHVibGljIE9uZURhc01vZHVsZVNlbGVjdG9yOiBLbm9ja291dE9ic2VydmFibGU8T25lRGFzTW9kdWxlU2VsZWN0b3JWaWV3TW9kZWw+XHJcblxyXG4gICAgY29uc3RydWN0b3IobW9kZWwsIGlkZW50aWZpY2F0aW9uOiBQbHVnaW5JZGVudGlmaWNhdGlvblZpZXdNb2RlbCwgb25lRGFzTW9kdWxlU2VsZWN0b3I6IE9uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHN1cGVyKG1vZGVsLCBpZGVudGlmaWNhdGlvbilcclxuXHJcbiAgICAgICAgdGhpcy5Nb2R1bGVUb0RhdGFQb3J0TWFwID0ga28ub2JzZXJ2YWJsZUFycmF5KClcclxuICAgICAgICB0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yID0ga28ub2JzZXJ2YWJsZTxPbmVEYXNNb2R1bGVTZWxlY3RvclZpZXdNb2RlbD4ob25lRGFzTW9kdWxlU2VsZWN0b3IpXHJcblxyXG4gICAgICAgIGlmICh0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yKCkuT25Nb2R1bGVTZXRDaGFuZ2VkLnN1YnNjcmliZSgoc2VuZGVyLCBhcmdzKSA9PlxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICB0aGlzLlVwZGF0ZURhdGFQb3J0U2V0KClcclxuICAgICAgICAgICAgfSlcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIGFzeW5jIEluaXRpYWxpemVBc3luYygpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5VcGRhdGVEYXRhUG9ydFNldCgpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFVwZGF0ZURhdGFQb3J0U2V0KClcclxuICAgIHtcclxuICAgICAgICBsZXQgaW5kZXg6IG51bWJlclxyXG4gICAgICAgIGxldCBtb2R1bGVUb0RhdGFQb3J0TWFwOiBPYnNlcnZhYmxlR3JvdXA8RGF0YVBvcnRWaWV3TW9kZWw+W11cclxuXHJcbiAgICAgICAgbW9kdWxlVG9EYXRhUG9ydE1hcCA9IFtdXHJcblxyXG4gICAgICAgIC8vIGlucHV0c1xyXG4gICAgICAgIGluZGV4ID0gMFxyXG5cclxuICAgICAgICBtb2R1bGVUb0RhdGFQb3J0TWFwID0gbW9kdWxlVG9EYXRhUG9ydE1hcC5jb25jYXQodGhpcy5PbmVEYXNNb2R1bGVTZWxlY3RvcigpLk1vZHVsZVNldCgpLmZpbHRlcihvbmVEYXNNb2R1bGUgPT4gb25lRGFzTW9kdWxlLkRhdGFEaXJlY3Rpb24oKSA9PT0gRGF0YURpcmVjdGlvbkVudW0uSW5wdXQpLm1hcChvbmVEYXNNb2R1bGUgPT5cclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGxldCBncm91cDogT2JzZXJ2YWJsZUdyb3VwPERhdGFQb3J0Vmlld01vZGVsPlxyXG5cclxuICAgICAgICAgICAgZ3JvdXAgPSBuZXcgT2JzZXJ2YWJsZUdyb3VwPERhdGFQb3J0Vmlld01vZGVsPihvbmVEYXNNb2R1bGUuVG9TdHJpbmcoKSwgdGhpcy5DcmVhdGVEYXRhUG9ydFNldChvbmVEYXNNb2R1bGUsIGluZGV4KSlcclxuICAgICAgICAgICAgaW5kZXggKz0gb25lRGFzTW9kdWxlLlNpemUoKTtcclxuXHJcbiAgICAgICAgICAgIHJldHVybiBncm91cFxyXG4gICAgICAgIH0pKVxyXG5cclxuICAgICAgICAvLyBvdXRwdXRzXHJcbiAgICAgICAgaW5kZXggPSAwXHJcblxyXG4gICAgICAgIG1vZHVsZVRvRGF0YVBvcnRNYXAgPSBtb2R1bGVUb0RhdGFQb3J0TWFwLmNvbmNhdCh0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yKCkuTW9kdWxlU2V0KCkuZmlsdGVyKG9uZURhc01vZHVsZSA9PiBvbmVEYXNNb2R1bGUuRGF0YURpcmVjdGlvbigpID09PSBEYXRhRGlyZWN0aW9uRW51bS5PdXRwdXQpLm1hcChvbmVEYXNNb2R1bGUgPT5cclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGxldCBncm91cDogT2JzZXJ2YWJsZUdyb3VwPERhdGFQb3J0Vmlld01vZGVsPlxyXG5cclxuICAgICAgICAgICAgZ3JvdXAgPSBuZXcgT2JzZXJ2YWJsZUdyb3VwPERhdGFQb3J0Vmlld01vZGVsPihvbmVEYXNNb2R1bGUuVG9TdHJpbmcoKSwgdGhpcy5DcmVhdGVEYXRhUG9ydFNldChvbmVEYXNNb2R1bGUsIGluZGV4KSlcclxuICAgICAgICAgICAgaW5kZXggKz0gb25lRGFzTW9kdWxlLlNpemUoKTtcclxuXHJcbiAgICAgICAgICAgIHJldHVybiBncm91cFxyXG4gICAgICAgIH0pKVxyXG5cclxuICAgICAgICB0aGlzLk1vZHVsZVRvRGF0YVBvcnRNYXAobW9kdWxlVG9EYXRhUG9ydE1hcClcclxuICAgICAgICB0aGlzLkRhdGFQb3J0U2V0KE1hcE1hbnkodGhpcy5Nb2R1bGVUb0RhdGFQb3J0TWFwKCksIGdyb3VwID0+IGdyb3VwLk1lbWJlcnMoKSkpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIENyZWF0ZURhdGFQb3J0U2V0KG9uZURhc01vZHVsZTogT25lRGFzTW9kdWxlVmlld01vZGVsLCBpbmRleDogbnVtYmVyKVxyXG4gICAge1xyXG4gICAgICAgIGxldCBwcmVmaXg6IHN0cmluZ1xyXG5cclxuICAgICAgICBzd2l0Y2ggKG9uZURhc01vZHVsZS5EYXRhRGlyZWN0aW9uKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLklucHV0OlxyXG4gICAgICAgICAgICAgICAgcHJlZml4ID0gXCJJbnB1dFwiXHJcbiAgICAgICAgICAgICAgICBicmVha1xyXG5cclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5PdXRwdXQ6XHJcbiAgICAgICAgICAgICAgICBwcmVmaXggPSBcIk91dHB1dFwiXHJcbiAgICAgICAgICAgICAgICBicmVha1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmV0dXJuIEFycmF5LmZyb20obmV3IEFycmF5KG9uZURhc01vZHVsZS5TaXplKCkpLCAoeCwgaSkgPT4gXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICByZXR1cm4ge1xyXG4gICAgICAgICAgICAgICAgTmFtZTogPHN0cmluZz5wcmVmaXggKyBcIiBcIiArIChpbmRleCArIGkpLFxyXG4gICAgICAgICAgICAgICAgRGF0YVR5cGU6IDxPbmVEYXNEYXRhVHlwZUVudW0+b25lRGFzTW9kdWxlLkRhdGFUeXBlKCksXHJcbiAgICAgICAgICAgICAgICBEYXRhRGlyZWN0aW9uOiA8RGF0YURpcmVjdGlvbkVudW0+b25lRGFzTW9kdWxlLkRhdGFEaXJlY3Rpb24oKVxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfSkubWFwKGRhdGFQb3J0TW9kZWwgPT4gbmV3IERhdGFQb3J0Vmlld01vZGVsKGRhdGFQb3J0TW9kZWwsIHRoaXMpKVxyXG4gICAgfVxyXG59IiwiLy8vIDxyZWZlcmVuY2UgcGF0aD1cIlBsdWdpblZpZXdNb2RlbEJhc2UudHNcIi8+XHJcblxyXG5hYnN0cmFjdCBjbGFzcyBEYXRhV3JpdGVyVmlld01vZGVsQmFzZSBleHRlbmRzIFBsdWdpblZpZXdNb2RlbEJhc2Vcclxue1xyXG4gICAgcHVibGljIHJlYWRvbmx5IEZpbGVHcmFudWxhcml0eTogS25vY2tvdXRPYnNlcnZhYmxlPEZpbGVHcmFudWxhcml0eUVudW0+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgQnVmZmVyUmVxdWVzdFNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8QnVmZmVyUmVxdWVzdFZpZXdNb2RlbD5cclxuICAgIHB1YmxpYyByZWFkb25seSBCdWZmZXJSZXF1ZXN0U2VsZWN0b3I6IEtub2Nrb3V0T2JzZXJ2YWJsZTxCdWZmZXJSZXF1ZXN0U2VsZWN0b3JWaWV3TW9kZWw+XHJcblxyXG4gICAgY29uc3RydWN0b3IobW9kZWwsIGlkZW50aWZpY2F0aW9uOiBQbHVnaW5JZGVudGlmaWNhdGlvblZpZXdNb2RlbClcclxuICAgIHtcclxuICAgICAgICBzdXBlcihtb2RlbCwgaWRlbnRpZmljYXRpb24pXHJcblxyXG4gICAgICAgIHRoaXMuRmlsZUdyYW51bGFyaXR5ID0ga28ub2JzZXJ2YWJsZTxGaWxlR3JhbnVsYXJpdHlFbnVtPihtb2RlbC5GaWxlR3JhbnVsYXJpdHkpXHJcbiAgICAgICAgdGhpcy5CdWZmZXJSZXF1ZXN0U2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWw+KG1vZGVsLkJ1ZmZlclJlcXVlc3RTZXQubWFwKGJ1ZmZlclJlcXVlc3QgPT4gbmV3IEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWwoYnVmZmVyUmVxdWVzdCkpKVxyXG5cclxuICAgICAgICB0aGlzLkJ1ZmZlclJlcXVlc3RTZWxlY3RvciA9IGtvLm9ic2VydmFibGU8QnVmZmVyUmVxdWVzdFNlbGVjdG9yVmlld01vZGVsPihuZXcgQnVmZmVyUmVxdWVzdFNlbGVjdG9yVmlld01vZGVsKHRoaXMuQnVmZmVyUmVxdWVzdFNldCgpKSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgRXh0ZW5kTW9kZWwobW9kZWw6IGFueSlcclxuICAgIHtcclxuICAgICAgICBzdXBlci5FeHRlbmRNb2RlbChtb2RlbClcclxuXHJcbiAgICAgICAgbW9kZWwuRmlsZUdyYW51bGFyaXR5ID0gPEZpbGVHcmFudWxhcml0eUVudW0+dGhpcy5GaWxlR3JhbnVsYXJpdHkoKVxyXG4gICAgICAgIG1vZGVsLkJ1ZmZlclJlcXVlc3RTZXQgPSA8QnVmZmVyUmVxdWVzdE1vZGVsW10+dGhpcy5CdWZmZXJSZXF1ZXN0U2V0KCkubWFwKGJ1ZmZlclJlcXVlc3QgPT4gYnVmZmVyUmVxdWVzdC5Ub01vZGVsKCkpXHJcbiAgICB9XHJcbn0iLCJjbGFzcyBQbHVnaW5EZXNjcmlwdGlvblZpZXdNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgUHJvZHVjdFZlcnNpb246IG51bWJlclxyXG4gICAgcHVibGljIElkOiBzdHJpbmdcclxuICAgIHB1YmxpYyBJbnN0YW5jZUlkOiBudW1iZXJcclxuICAgIHB1YmxpYyBJbnN0YW5jZU5hbWU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgSXNFbmFibGVkOiBLbm9ja291dE9ic2VydmFibGU8Ym9vbGVhbj5cclxuXHJcbiAgICBjb25zdHJ1Y3RvcihwbHVnaW5EZXNjcmlwdGlvbk1vZGVsOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5Qcm9kdWN0VmVyc2lvbiA9IHBsdWdpbkRlc2NyaXB0aW9uTW9kZWwuUHJvZHVjdFZlcnNpb25cclxuICAgICAgICB0aGlzLklkID0gcGx1Z2luRGVzY3JpcHRpb25Nb2RlbC5JZFxyXG4gICAgICAgIHRoaXMuSW5zdGFuY2VJZCA9IHBsdWdpbkRlc2NyaXB0aW9uTW9kZWwuSW5zdGFuY2VJZFxyXG4gICAgICAgIHRoaXMuSW5zdGFuY2VOYW1lID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KHBsdWdpbkRlc2NyaXB0aW9uTW9kZWwuSW5zdGFuY2VOYW1lKVxyXG4gICAgICAgIHRoaXMuSXNFbmFibGVkID0ga28ub2JzZXJ2YWJsZTxib29sZWFuPihwbHVnaW5EZXNjcmlwdGlvbk1vZGVsLklzRW5hYmxlZClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpXHJcbiAgICB7XHJcbiAgICAgICAgdmFyIG1vZGVsOiBhbnkgPSB7XHJcbiAgICAgICAgICAgIFByb2R1Y3RWZXJzaW9uOiA8bnVtYmVyPnRoaXMuUHJvZHVjdFZlcnNpb24sXHJcbiAgICAgICAgICAgIElkOiA8c3RyaW5nPnRoaXMuSWQsXHJcbiAgICAgICAgICAgIEluc3RhbmNlSWQ6IDxudW1iZXI+dGhpcy5JbnN0YW5jZUlkLFxyXG4gICAgICAgICAgICBJbnN0YW5jZU5hbWU6IDxzdHJpbmc+dGhpcy5JbnN0YW5jZU5hbWUoKSxcclxuICAgICAgICAgICAgSXNFbmFibGVkOiA8Ym9vbGVhbj50aGlzLklzRW5hYmxlZCgpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gbW9kZWxcclxuICAgIH1cclxufSIsImNsYXNzIFBsdWdpbklkZW50aWZpY2F0aW9uVmlld01vZGVsXHJcbntcclxuICAgIHB1YmxpYyBQcm9kdWN0VmVyc2lvbjogc3RyaW5nXHJcbiAgICBwdWJsaWMgSWQ6IHN0cmluZ1xyXG4gICAgcHVibGljIE5hbWU6IHN0cmluZ1xyXG4gICAgcHVibGljIERlc2NyaXB0aW9uOiBzdHJpbmdcclxuICAgIHB1YmxpYyBWaWV3UmVzb3VyY2VOYW1lOiBzdHJpbmdcclxuICAgIHB1YmxpYyBWaWV3TW9kZWxSZXNvdXJjZU5hbWU6IHN0cmluZ1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKHBsdWdpbklkZW50aWZpY2F0aW9uTW9kZWw6IGFueSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLlByb2R1Y3RWZXJzaW9uID0gcGx1Z2luSWRlbnRpZmljYXRpb25Nb2RlbC5Qcm9kdWN0VmVyc2lvblxyXG4gICAgICAgIHRoaXMuSWQgPSBwbHVnaW5JZGVudGlmaWNhdGlvbk1vZGVsLklkXHJcbiAgICAgICAgdGhpcy5OYW1lID0gcGx1Z2luSWRlbnRpZmljYXRpb25Nb2RlbC5OYW1lXHJcbiAgICAgICAgdGhpcy5EZXNjcmlwdGlvbiA9IHBsdWdpbklkZW50aWZpY2F0aW9uTW9kZWwuRGVzY3JpcHRpb25cclxuICAgICAgICB0aGlzLlZpZXdSZXNvdXJjZU5hbWUgPSBwbHVnaW5JZGVudGlmaWNhdGlvbk1vZGVsLlZpZXdSZXNvdXJjZU5hbWVcclxuICAgICAgICB0aGlzLlZpZXdNb2RlbFJlc291cmNlTmFtZSA9IHBsdWdpbklkZW50aWZpY2F0aW9uTW9kZWwuVmlld01vZGVsUmVzb3VyY2VOYW1lXHJcbiAgICB9XHJcbn0iXX0=