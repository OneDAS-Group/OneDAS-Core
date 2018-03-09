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
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiT25lRGFzLlR5cGVzLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vT25lRGFzLlR5cGVzL0RhdGFEaXJlY3Rpb25FbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL0VuZGlhbm5lc3NFbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL0ZpbGVHcmFudWxhcml0eUVudW0udHMiLCIuLi9PbmVEYXMuVHlwZXMvT25lRGFzRGF0YVR5cGVFbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL09uZURhc1N0YXRlRW51bS50cyIsIi4uL09uZURhcy5UeXBlcy9TYW1wbGVSYXRlRW51bS50cyIsIi4uL0NvcmUvQWN0aW9uUmVxdWVzdC50cyIsIi4uL0NvcmUvQWN0aW9uUmVzcG9uc2UudHMiLCIuLi9Db3JlL0V2ZW50RGlzcGF0Y2hlci50cyIsIi4uL0NvcmUvSUV2ZW50LnRzIiwiLi4vQ29yZS9PbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtLnRzIiwiLi4vTW9kZWxzL0J1ZmZlclJlcXVlc3RNb2RlbC50cyIsIi4uL01vZGVscy9DaGFubmVsSHViTW9kZWwudHMiLCIuLi9Nb2RlbHMvT25lRGFzTW9kdWxlTW9kZWwudHMiLCIuLi9Nb2RlbHMvVHJhbnNmZXJGdW5jdGlvbk1vZGVsLnRzIiwiLi4vU3RhdGljL0Nvbm5lY3Rpb25NYW5hZ2VyLnRzIiwiLi4vU3RhdGljL0VudW1lcmF0aW9uSGVscGVyLnRzIiwiLi4vU3RhdGljL0Vycm9yTWVzc2FnZS50cyIsIi4uL1N0YXRpYy9IZWxwZXIudHMiLCIuLi9TdGF0aWMvUGx1Z2luRmFjdG9yeS50cyIsIi4uL1N0YXRpYy9QbHVnaW5IaXZlLnRzIiwiLi4vVmlld01vZGVscy9Db3JlL0NoYW5uZWxIdWJWaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL0NvcmUvT25lRGFzTW9kdWxlVmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9Db3JlL09uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9Db3JlL1RyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL1BsdWdpbi9CdWZmZXJSZXF1ZXN0U2VsZWN0b3JWaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL1BsdWdpbi9CdWZmZXJSZXF1ZXN0Vmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vRGF0YVBvcnRWaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL1BsdWdpbi9QbHVnaW5WaWV3TW9kZWxCYXNlLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vRXh0ZW5kZWREYXRhR2F0ZXdheVZpZXdNb2RlbEJhc2UudHMiLCIuLi9WaWV3TW9kZWxzL1BsdWdpbi9EYXRhV3JpdGVyVmlld01vZGVsQmFzZS50cyIsIi4uL1ZpZXdNb2RlbHMvUGx1Z2luL1BsdWdpbkRlc2NyaXB0aW9uVmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vUGx1Z2luSWRlbnRpZmljYXRpb25WaWV3TW9kZWwudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsSUFBSyxpQkFJSjtBQUpELFdBQUssaUJBQWlCO0lBRWxCLDJEQUFTLENBQUE7SUFDVCw2REFBVSxDQUFBO0FBQ2QsQ0FBQyxFQUpJLGlCQUFpQixLQUFqQixpQkFBaUIsUUFJckI7QUNKRCxJQUFLLGNBSUo7QUFKRCxXQUFLLGNBQWM7SUFFZixtRUFBZ0IsQ0FBQTtJQUNoQiw2REFBYSxDQUFBO0FBQ2pCLENBQUMsRUFKSSxjQUFjLEtBQWQsY0FBYyxRQUlsQjtBQ0pELElBQUssbUJBTUo7QUFORCxXQUFLLG1CQUFtQjtJQUVwQixzRUFBYSxDQUFBO0lBQ2IseUVBQWUsQ0FBQTtJQUNmLGdFQUFXLENBQUE7SUFDWCwrREFBVyxDQUFBO0FBQ2YsQ0FBQyxFQU5JLG1CQUFtQixLQUFuQixtQkFBbUIsUUFNdkI7QUNORCxJQUFLLGtCQVdKO0FBWEQsV0FB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R0FBUTtZQUNiLGNBQWMsRUFBVSxJQUFJLENBQUMsY0FBYztZQUMzQyxFQUFFLEVBQVUsSUFBSSxDQUFDLEVBQUU7WUFDbkIsVUFBVSxFQUFVLElBQUksQ0FBQyxVQUFVO1lBQ25DLFlBQVksRUFBVSxJQUFJLENBQUMsWUFBWSxFQUFFO1lBQ3pDLFNBQVMsRUFBVyxJQUFJLENBQUMsU0FBUyxFQUFFO1NBQ3ZDLENBQUE7UUFFRCxNQUFNLENBQUMsS0FBSyxDQUFBO0lBQ2hCLENBQUM7Q0FDSjtBQzdCRDtJQVNJLFlBQVkseUJBQThCO1FBRXRDLElBQUksQ0FBQyxjQUFjLEdBQUcseUJBQXlCLENBQUMsY0FBYyxDQUFBO1FBQzlELElBQUksQ0FBQyxFQUFFLEdBQUcseUJBQXlCLENBQUMsRUFBRSxDQUFBO1FBQ3RDLElBQUksQ0FBQyxJQUFJLEdBQUcseUJBQXlCLENBQUMsSUFBSSxDQUFBO1FBQzFDLElBQUksQ0FBQyxXQUFXLEdBQUcseUJBQXlCLENBQUMsV0FBVyxDQUFBO1FBQ3hELElBQUksQ0FBQyxnQkFBZ0IsR0FBRyx5QkFBeUIsQ0FBQyxnQkFBZ0IsQ0FBQTtRQUNsRSxJQUFJLENBQUMscUJBQXFCLEdBQUcseUJBQXlCLENBQUMscUJBQXFCLENBQUE7SUFDaEYsQ0FBQztDQUNKIiwic291cmNlc0NvbnRlbnQiOlsiZW51bSBEYXRhRGlyZWN0aW9uRW51bVxyXG57XHJcbiAgICBJbnB1dCA9IDEsXHJcbiAgICBPdXRwdXQgPSAyXHJcbn0iLCJlbnVtIEVuZGlhbm5lc3NFbnVtXHJcbntcclxuICAgIExpdHRsZUVuZGlhbiA9IDEsXHJcbiAgICBCaWdFbmRpYW4gPSAyXHJcbn0iLCJlbnVtIEZpbGVHcmFudWxhcml0eUVudW1cclxue1xyXG4gICAgTWludXRlXzEgPSA2MCxcclxuICAgIE1pbnV0ZV8xMCA9IDYwMCxcclxuICAgIEhvdXIgPSAzNjAwLFxyXG4gICAgRGF5ID0gODY0MDBcclxufSIsImVudW0gT25lRGFzRGF0YVR5cGVFbnVtXHJcbntcclxuICAgIEJPT0xFQU4gPSAweDAwOCxcclxuICAgIFVJTlQ4ID0gMHgxMDgsXHJcbiAgICBJTlQ4ID0gMHgyMDgsXHJcbiAgICBVSU5UMTYgPSAweDExMCxcclxuICAgIElOVDE2ID0gMHgyMTAsXHJcbiAgICBVSU5UMzIgPSAweDEyMCxcclxuICAgIElOVDMyID0gMHgyMjAsXHJcbiAgICBGTE9BVDMyID0gMHgzMjAsXHJcbiAgICBGTE9BVDY0ID0gMHgzNDBcclxufSIsImVudW0gT25lRGFzU3RhdGVFbnVtXHJcbntcclxuICAgIEVycm9yID0gMSxcclxuICAgIEluaXRpYWxpemF0aW9uID0gMixcclxuICAgIFVuY29uZmlndXJlZCA9IDMsXHJcbiAgICBBcHBseUNvbmZpZ3VyYXRpb24gPSA1LFxyXG4gICAgUmVhZHkgPSA2LFxyXG4gICAgUnVuID0gN1xyXG59IiwiZW51bSBTYW1wbGVSYXRlRW51bVxyXG57XHJcbiAgICBTYW1wbGVSYXRlXzEwMCA9IDEsXHJcbiAgICBTYW1wbGVSYXRlXzI1ID0gNCxcclxuICAgIFNhbXBsZVJhdGVfNSA9IDIwLFxyXG4gICAgU2FtcGxlUmF0ZV8xID0gMTAwXHJcbn0iLCJjbGFzcyBBY3Rpb25SZXF1ZXN0XHJcbntcclxuICAgIHB1YmxpYyByZWFkb25seSBQbHVnaW5JZDogc3RyaW5nXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgSW5zdGFuY2VJZDogbnVtYmVyXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgTWV0aG9kTmFtZTogc3RyaW5nXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRGF0YTogYW55XHJcblxyXG4gICAgY29uc3RydWN0b3IocGx1Z2luSWQ6IHN0cmluZywgaW5zdGFuY2VJZDogbnVtYmVyLCBtZXRob2ROYW1lOiBzdHJpbmcsIGRhdGE6IGFueSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLlBsdWdpbklkID0gcGx1Z2luSWQ7XHJcbiAgICAgICAgdGhpcy5JbnN0YW5jZUlkID0gaW5zdGFuY2VJZDtcclxuICAgICAgICB0aGlzLk1ldGhvZE5hbWUgPSBtZXRob2ROYW1lO1xyXG4gICAgICAgIHRoaXMuRGF0YSA9IGRhdGE7XHJcbiAgICB9XHJcbn0iLCJjbGFzcyBBY3Rpb25SZXNwb25zZVxyXG57XHJcbiAgICBwdWJsaWMgRGF0YTogYW55XHJcblxyXG4gICAgY29uc3RydWN0b3IoZGF0YTogYW55KVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuRGF0YSA9IGRhdGE7XHJcbiAgICB9XHJcbn0iLCJjbGFzcyBFdmVudERpc3BhdGNoZXI8VFNlbmRlciwgVEFyZ3M+IGltcGxlbWVudHMgSUV2ZW50PFRTZW5kZXIsIFRBcmdzPlxyXG57XHJcbiAgICBwcml2YXRlIF9zdWJzY3JpcHRpb25zOiBBcnJheTwoc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncykgPT4gdm9pZD4gPSBuZXcgQXJyYXk8KHNlbmRlcjogVFNlbmRlciwgYXJnczogVEFyZ3MpID0+IHZvaWQ+KCk7XHJcblxyXG4gICAgc3Vic2NyaWJlKGZuOiAoc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncykgPT4gdm9pZCk6IHZvaWRcclxuICAgIHtcclxuICAgICAgICBpZiAoZm4pXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLl9zdWJzY3JpcHRpb25zLnB1c2goZm4pO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICB1bnN1YnNjcmliZShmbjogKHNlbmRlcjogVFNlbmRlciwgYXJnczogVEFyZ3MpID0+IHZvaWQpOiB2b2lkXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IGkgPSB0aGlzLl9zdWJzY3JpcHRpb25zLmluZGV4T2YoZm4pO1xyXG5cclxuICAgICAgICBpZiAoaSA+IC0xKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5fc3Vic2NyaXB0aW9ucy5zcGxpY2UoaSwgMSk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIGRpc3BhdGNoKHNlbmRlcjogVFNlbmRlciwgYXJnczogVEFyZ3MpOiB2b2lkXHJcbiAgICB7XHJcbiAgICAgICAgZm9yIChsZXQgaGFuZGxlciBvZiB0aGlzLl9zdWJzY3JpcHRpb25zKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgaGFuZGxlcihzZW5kZXIsIGFyZ3MpO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxufSIsImludGVyZmFjZSBJRXZlbnQ8VFNlbmRlciwgVEFyZ3M+XHJcbntcclxuICAgIHN1YnNjcmliZShmbjogKHNlbmRlcjogVFNlbmRlciwgYXJnczogVEFyZ3MpID0+IHZvaWQpOiB2b2lkO1xyXG4gICAgdW5zdWJzY3JpYmUoZm46IChzZW5kZXI6IFRTZW5kZXIsIGFyZ3M6IFRBcmdzKSA9PiB2b2lkKTogdm9pZDtcclxufSIsImVudW0gT25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlRW51bVxyXG57XHJcbiAgICBEdXBsZXggPSAxLFxyXG4gICAgSW5wdXRPbmx5ID0gMixcclxuICAgIE91dHB1dE9ubHkgPSAzLFxyXG59IiwiY2xhc3MgQnVmZmVyUmVxdWVzdE1vZGVsXHJcbntcclxuICAgIHB1YmxpYyBTYW1wbGVSYXRlOiBTYW1wbGVSYXRlRW51bVxyXG4gICAgcHVibGljIEdyb3VwRmlsdGVyOiBzdHJpbmdcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihzYW1wbGVSYXRlOiBTYW1wbGVSYXRlRW51bSwgZ3JvdXBGaWx0ZXI6IHN0cmluZylcclxuICAgIHtcclxuICAgICAgICB0aGlzLlNhbXBsZVJhdGUgPSBzYW1wbGVSYXRlO1xyXG4gICAgICAgIHRoaXMuR3JvdXBGaWx0ZXIgPSBncm91cEZpbHRlcjtcclxuICAgIH1cclxufSIsImNsYXNzIENoYW5uZWxIdWJNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgTmFtZTogc3RyaW5nXHJcbiAgICBwdWJsaWMgR3JvdXA6IHN0cmluZ1xyXG4gICAgcHVibGljIERhdGFUeXBlOiBPbmVEYXNEYXRhVHlwZUVudW1cclxuICAgIHB1YmxpYyBHdWlkOiBzdHJpbmdcclxuICAgIHB1YmxpYyBDcmVhdGlvbkRhdGVUaW1lOiBzdHJpbmdcclxuICAgIHB1YmxpYyBVbml0OiBzdHJpbmdcclxuICAgIHB1YmxpYyBUcmFuc2ZlckZ1bmN0aW9uU2V0OiBhbnlbXVxyXG4gICAgcHVibGljIEFzc29jaWF0ZWREYXRhSW5wdXRJZDogc3RyaW5nXHJcbiAgICBwdWJsaWMgQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldDogc3RyaW5nW11cclxuXHJcbiAgICBjb25zdHJ1Y3RvcihuYW1lOiBzdHJpbmcsIGdyb3VwOiBzdHJpbmcsIGRhdGFUeXBlOiBPbmVEYXNEYXRhVHlwZUVudW0pXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5OYW1lID0gbmFtZTtcclxuICAgICAgICB0aGlzLkdyb3VwID0gZ3JvdXA7XHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZSA9IGRhdGFUeXBlO1xyXG4gICAgICAgIHRoaXMuR3VpZCA9IEd1aWQuTmV3R3VpZCgpXHJcbiAgICAgICAgdGhpcy5DcmVhdGlvbkRhdGVUaW1lID0gbmV3IERhdGUoKS50b0lTT1N0cmluZygpXHJcbiAgICAgICAgdGhpcy5Vbml0ID0gXCJcIlxyXG4gICAgICAgIHRoaXMuVHJhbnNmZXJGdW5jdGlvblNldCA9IFtdXHJcbiAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0SWQgPSBcIlwiXHJcbiAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0ID0gW11cclxuICAgIH1cclxufSIsImNsYXNzIE9uZURhc01vZHVsZU1vZGVsXHJcbntcclxuICAgIHB1YmxpYyBEYXRhVHlwZTogT25lRGFzRGF0YVR5cGVFbnVtXHJcbiAgICBwdWJsaWMgRGF0YURpcmVjdGlvbjogRGF0YURpcmVjdGlvbkVudW1cclxuICAgIHB1YmxpYyBFbmRpYW5uZXNzOiBFbmRpYW5uZXNzRW51bVxyXG4gICAgcHVibGljIFNpemU6IG51bWJlclxyXG5cclxuICAgIGNvbnN0cnVjdG9yKGRhdGFUeXBlOiBPbmVEYXNEYXRhVHlwZUVudW0sIGRhdGFEaXJlY3Rpb246IERhdGFEaXJlY3Rpb25FbnVtLCBlbmRpYW5uZXNzOiBFbmRpYW5uZXNzRW51bSwgc2l6ZTogbnVtYmVyKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuRGF0YVR5cGUgPSBkYXRhVHlwZVxyXG4gICAgICAgIHRoaXMuRGF0YURpcmVjdGlvbiA9IGRhdGFEaXJlY3Rpb25cclxuICAgICAgICB0aGlzLkVuZGlhbm5lc3MgPSBlbmRpYW5uZXNzXHJcbiAgICAgICAgdGhpcy5TaXplID0gc2l6ZVxyXG4gICAgfVxyXG59XHJcbiIsImNsYXNzIFRyYW5zZmVyRnVuY3Rpb25Nb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgRGF0ZVRpbWU6IHN0cmluZ1xyXG4gICAgcHVibGljIFR5cGU6IHN0cmluZ1xyXG4gICAgcHVibGljIE9wdGlvbjogc3RyaW5nXHJcbiAgICBwdWJsaWMgQXJndW1lbnQ6IHN0cmluZ1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKGRhdGVUaW1lOiBzdHJpbmcsIHR5cGU6IHN0cmluZywgb3B0aW9uOiBzdHJpbmcsIGFyZ3VtZW50OiBzdHJpbmcpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5EYXRlVGltZSA9IGRhdGVUaW1lXHJcbiAgICAgICAgdGhpcy5UeXBlID0gdHlwZVxyXG4gICAgICAgIHRoaXMuT3B0aW9uID0gb3B0aW9uXHJcbiAgICAgICAgdGhpcy5Bcmd1bWVudCA9IGFyZ3VtZW50XHJcbiAgICB9XHJcbn0iLCJkZWNsYXJlIHZhciBzaWduYWxSOiBhbnlcclxuXHJcbmNsYXNzIENvbm5lY3Rpb25NYW5hZ2VyXHJcbntcclxuICAgIHB1YmxpYyBzdGF0aWMgV2ViQ2xpZW50SHViOiBhbnkgLy8gaW1wcm92ZTogdXNlIHR5cGluZ3NcclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIEluaXRpYWxpemUoZW5hYmxlTG9nZ2luZzogYm9vbGVhbilcclxuICAgIHtcclxuICAgICAgICBDb25uZWN0aW9uTWFuYWdlci5XZWJDbGllbnRIdWIgPSBuZXcgc2lnbmFsUi5IdWJDb25uZWN0aW9uKCcvd2ViY2xpZW50aHViJyk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBJbnZva2VXZWJDbGllbnRIdWIgPSBhc3luYyhtZXRob2ROYW1lOiBzdHJpbmcsIC4uLmFyZ3M6IGFueVtdKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiBhd2FpdCBQcm9taXNlLnJlc29sdmUoQ29ubmVjdGlvbk1hbmFnZXIuV2ViQ2xpZW50SHViLmludm9rZShtZXRob2ROYW1lLCAuLi5hcmdzKSlcclxuICAgIH1cclxufVxyXG4iLCJjbGFzcyBFbnVtZXJhdGlvbkhlbHBlclxyXG57XHJcbiAgICBwdWJsaWMgc3RhdGljIERlc2NyaXB0aW9uOiB7IFtpbmRleDogc3RyaW5nXTogc3RyaW5nIH0gPSB7fVxyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgR2V0RW51bUxvY2FsaXphdGlvbiA9ICh0eXBlTmFtZTogc3RyaW5nLCB2YWx1ZSkgPT5cclxuICAgIHtcclxuICAgICAgICB2YXIga2V5OiBzdHJpbmcgPSBldmFsKHR5cGVOYW1lICsgXCJbXCIgKyB2YWx1ZSArIFwiXVwiKVxyXG4gICAgICAgIHJldHVybiBldmFsKFwiRW51bWVyYXRpb25IZWxwZXIuRGVzY3JpcHRpb25bJ1wiICsgdHlwZU5hbWUgKyBcIl9cIiArIGtleSArIFwiJ11cIilcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIEdldEVudW1WYWx1ZXMgPSAodHlwZU5hbWU6IHN0cmluZykgPT5cclxuICAgIHtcclxuICAgICAgICBsZXQgdmFsdWVzOiBhbnlbXVxyXG5cclxuICAgICAgICB2YWx1ZXMgPSBldmFsKFwiT2JqZWN0LmtleXMoXCIgKyB0eXBlTmFtZSArIFwiKS5tYXAoa2V5ID0+IFwiICsgdHlwZU5hbWUgKyBcIltrZXldKVwiKVxyXG4gICAgICAgIHJldHVybiA8bnVtYmVyW10+dmFsdWVzLmZpbHRlcih2YWx1ZSA9PiB0eXBlb2YgKHZhbHVlKSA9PT0gXCJudW1iZXJcIilcclxuICAgIH1cclxufSIsImxldCBFcnJvck1lc3NhZ2U6IHsgW2luZGV4OiBzdHJpbmddOiBzdHJpbmcgfSA9IHt9XHJcbkVycm9yTWVzc2FnZVtcIk11bHRpTWFwcGluZ0VkaXRvclZpZXdNb2RlbF9JbnZhbGlkU2V0dGluZ3NcIl0gPSBcIk9uZSBvciBtb3JlIHNldHRpbmdzIGFyZSBpbnZhbGlkLlwiXHJcbkVycm9yTWVzc2FnZVtcIk11bHRpTWFwcGluZ0VkaXRvclZpZXdNb2RlbF9Xcm9uZ0RhdGFUeXBlXCJdID0gXCJPbmUgb3IgbW9yZSB2YXJpYWJsZS1jaGFubmVsIGRhdGEgdHlwZSBjb21iaW5hdGlvbnMgYXJlIGludmFsaWQuXCJcclxuRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9DaGFubmVsQWxyZWFkeUV4aXN0c1wiXSA9IFwiQSBjaGFubmVsIHdpdGggdGhhdCBuYW1lIGFscmVhZHkgZXhpc3RzLlwiXHJcbkVycm9yTWVzc2FnZVtcIlByb2plY3RfRGV0YWNoZWRFeGNsYW1hdGlvbk1hcmtOb3RBbGxvd2VkXCJdID0gXCJBIGRldGFjaGVkIGV4Y2xhbWF0aW9uIG1hcmsgaXMgbm90IGFsbG93ZWQuXCJcclxuRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9Hcm91cEZpbHRlckVtcHR5XCJdID0gXCJUaGUgZ3JvdXAgZmlsdGVyIG11c3Qgbm90IGJlIGVtcHR5LlwiXHJcbkVycm9yTWVzc2FnZVtcIlByb2plY3RfSXNBbHJlYWR5SW5Hcm91cFwiXSA9IFwiVGhlIGNoYW5uZWwgaXMgYWxyZWFkeSBhIG1lbWJlciBvZiB0aGlzIGdyb3VwLlwiXHJcbkVycm9yTWVzc2FnZVtcIlByb2plY3RfSW52YWxpZENoYXJhY3RlcnNcIl0gPSBcIlVzZSBBLVosIGEteiwgMC05IG9yIF8uXCJcclxuRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9JbnZhbGlkTGVhZGluZ0NoYXJhY3RlclwiXSA9IFwiVXNlIEEtWiBvciBhLXogYXMgZmlyc3QgY2hhcmFjdGVyLlwiXHJcbkVycm9yTWVzc2FnZVtcIlByb2plY3RfTmFtZUVtcHR5XCJdID0gXCJUaGUgbmFtZSBtdXN0IG5vdCBiZSBlbXB0eS5cIlxyXG4iLCJjbGFzcyBPYnNlcnZhYmxlR3JvdXA8VD5cclxue1xyXG4gICAgS2V5OiBzdHJpbmc7XHJcbiAgICBNZW1iZXJzOiBLbm9ja291dE9ic2VydmFibGVBcnJheTxUPlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKGtleTogc3RyaW5nLCBtZW1iZXJzOiBUW10gPSBuZXcgQXJyYXk8VD4oKSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLktleSA9IGtleVxyXG4gICAgICAgIHRoaXMuTWVtYmVycyA9IGtvLm9ic2VydmFibGVBcnJheShtZW1iZXJzKVxyXG4gICAgfVxyXG59XHJcblxyXG5mdW5jdGlvbiBPYnNlcnZhYmxlR3JvdXBCeTxUPihsaXN0OiBUW10sIG5hbWVHZXR0ZXI6ICh4OiBUKSA9PiBzdHJpbmcsIGdyb3VwTmFtZUdldHRlcjogKHg6IFQpID0+IHN0cmluZywgZmlsdGVyOiBzdHJpbmcpOiBPYnNlcnZhYmxlR3JvdXA8VD5bXVxyXG57XHJcbiAgICBsZXQgcmVzdWx0OiBPYnNlcnZhYmxlR3JvdXA8VD5bXVxyXG4gICAgbGV0IHJlZ0V4cDogUmVnRXhwXHJcblxyXG4gICAgcmVzdWx0ID0gW11cclxuICAgIHJlZ0V4cCA9IG5ldyBSZWdFeHAoZmlsdGVyLCBcImlcIilcclxuXHJcbiAgICBsaXN0LmZvckVhY2goZWxlbWVudCA9PlxyXG4gICAge1xyXG4gICAgICAgIGlmIChyZWdFeHAudGVzdChuYW1lR2V0dGVyKGVsZW1lbnQpKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGdyb3VwTmFtZUdldHRlcihlbGVtZW50KS5zcGxpdChcIlxcblwiKS5mb3JFYWNoKGdyb3VwTmFtZSA9PlxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICBBZGRUb0dyb3VwZWRBcnJheShlbGVtZW50LCBncm91cE5hbWUsIHJlc3VsdClcclxuICAgICAgICAgICAgfSlcclxuICAgICAgICB9XHJcbiAgICB9KVxyXG5cclxuICAgIHJldHVybiByZXN1bHRcclxufVxyXG5cclxuZnVuY3Rpb24gQWRkVG9Hcm91cGVkQXJyYXk8VD4oaXRlbTogVCwgZ3JvdXBOYW1lOiBzdHJpbmcsIG9ic2VydmFibGVHcm91cFNldDogT2JzZXJ2YWJsZUdyb3VwPFQ+W10pXHJcbntcclxuICAgIGxldCBncm91cDogT2JzZXJ2YWJsZUdyb3VwPFQ+XHJcblxyXG4gICAgZ3JvdXAgPSBvYnNlcnZhYmxlR3JvdXBTZXQuZmluZCh5ID0+IHkuS2V5ID09PSBncm91cE5hbWUpXHJcblxyXG4gICAgaWYgKCFncm91cClcclxuICAgIHtcclxuICAgICAgICBncm91cCA9IG5ldyBPYnNlcnZhYmxlR3JvdXA8VD4oZ3JvdXBOYW1lKVxyXG4gICAgICAgIG9ic2VydmFibGVHcm91cFNldC5wdXNoKGdyb3VwKVxyXG4gICAgfVxyXG5cclxuICAgIGdyb3VwLk1lbWJlcnMucHVzaChpdGVtKVxyXG59XHJcblxyXG4vL2Z1bmN0aW9uIEFkZFRvR3JvdXBlZE9ic2VydmFibGVBcnJheTxUPihpdGVtOiBULCBncm91cE5hbWU6IHN0cmluZywgb2JzZXJ2YWJsZUdyb3VwU2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxPYnNlcnZhYmxlR3JvdXA8VD4+KVxyXG4vL3tcclxuLy8gICAgbGV0IGdyb3VwOiBPYnNlcnZhYmxlR3JvdXA8VD5cclxuXHJcbi8vICAgIGdyb3VwID0gb2JzZXJ2YWJsZUdyb3VwU2V0KCkuZmluZCh5ID0+IHkuS2V5ID09PSBncm91cE5hbWUpXHJcblxyXG4vLyAgICBpZiAoIWdyb3VwKVxyXG4vLyAgICB7XHJcbi8vICAgICAgICBncm91cCA9IG5ldyBPYnNlcnZhYmxlR3JvdXA8VD4oZ3JvdXBOYW1lKVxyXG4vLyAgICAgICAgb2JzZXJ2YWJsZUdyb3VwU2V0LnB1c2goZ3JvdXApXHJcbi8vICAgIH1cclxuXHJcbi8vICAgIGdyb3VwLk1lbWJlcnMucHVzaChpdGVtKVxyXG4vL31cclxuXHJcbi8vZnVuY3Rpb24gUmVtb3ZlRnJvbUdyb3VwZWRPYnNlcnZhYmxlQXJyYXk8VD4oaXRlbTogVCwgb2JzZXJ2YWJsZUdyb3VwU2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxPYnNlcnZhYmxlR3JvdXA8VD4+KVxyXG4vL3tcclxuLy8gICAgdmFyIGdyb3VwOiBPYnNlcnZhYmxlR3JvdXA8VD5cclxuXHJcbi8vICAgIG9ic2VydmFibGVHcm91cFNldCgpLnNvbWUoeCA9PlxyXG4vLyAgICB7XHJcbi8vICAgICAgICBpZiAoeC5NZW1iZXJzKCkuaW5kZXhPZihpdGVtKSA+IC0xKVxyXG4vLyAgICAgICAge1xyXG4vLyAgICAgICAgICAgIGdyb3VwID0geFxyXG5cclxuLy8gICAgICAgICAgICByZXR1cm4gdHJ1ZVxyXG4vLyAgICAgICAgfVxyXG5cclxuLy8gICAgICAgIHJldHVybiBmYWxzZVxyXG4vLyAgICB9KVxyXG5cclxuLy8gICAgaWYgKGdyb3VwKVxyXG4vLyAgICB7XHJcbi8vICAgICAgICBncm91cC5NZW1iZXJzLnJlbW92ZShpdGVtKVxyXG5cclxuLy8gICAgICAgIGlmIChncm91cC5NZW1iZXJzKCkubGVuZ3RoID09PSAwKVxyXG4vLyAgICAgICAge1xyXG4vLyAgICAgICAgICAgIG9ic2VydmFibGVHcm91cFNldC5yZW1vdmUoZ3JvdXApXHJcbi8vICAgICAgICB9XHJcblxyXG4vLyAgICAgICAgcmV0dXJuIHRydWVcclxuLy8gICAgfVxyXG5cclxuLy8gICAgcmV0dXJuIGZhbHNlXHJcbi8vfVxyXG5cclxuLy9mdW5jdGlvbiBVcGRhdGVHcm91cGVkT2JzZXJ2YWJsZUFycmF5PFQ+KGl0ZW06IFQsIGdyb3VwTmFtZTogc3RyaW5nLCBvYnNlcnZhYmxlR3JvdXBTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PE9ic2VydmFibGVHcm91cDxUPj4pXHJcbi8ve1xyXG4vLyAgICBSZW1vdmVGcm9tR3JvdXBlZE9ic2VydmFibGVBcnJheShpdGVtLCBvYnNlcnZhYmxlR3JvdXBTZXQpXHJcbi8vICAgIEFkZFRvR3JvdXBlZE9ic2VydmFibGVBcnJheShpdGVtLCBncm91cE5hbWUsIG9ic2VydmFibGVHcm91cFNldClcclxuLy99XHJcblxyXG5mdW5jdGlvbiBNYXBNYW55PFRBcnJheUVsZW1lbnQsIFRTZWxlY3Q+KGFycmF5OiBUQXJyYXlFbGVtZW50W10sIG1hcEZ1bmM6IChpdGVtOiBUQXJyYXlFbGVtZW50KSA9PiBUU2VsZWN0W10pOiBUU2VsZWN0W11cclxue1xyXG4gICAgcmV0dXJuIGFycmF5LnJlZHVjZSgocHJldmlvdXMsIGN1cnJlbnQsIGkpID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHByZXZpb3VzLmNvbmNhdChtYXBGdW5jKGN1cnJlbnQpKTtcclxuICAgIH0sIDxUU2VsZWN0W10+W10pO1xyXG59XHJcblxyXG5jbGFzcyBHdWlkXHJcbntcclxuICAgIHN0YXRpYyBOZXdHdWlkKClcclxuICAgIHtcclxuICAgICAgICByZXR1cm4gJ3h4eHh4eHh4LXh4eHgtNHh4eC15eHh4LXh4eHh4eHh4eHh4eCcucmVwbGFjZSgvW3h5XS9nLCBmdW5jdGlvbiAoYylcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHZhciByID0gTWF0aC5yYW5kb20oKSAqIDE2IHwgMFxyXG4gICAgICAgICAgICB2YXIgdiA9IGMgPT09ICd4JyA/IHIgOiAociAmIDB4MyB8IDB4OClcclxuXHJcbiAgICAgICAgICAgIHJldHVybiB2LnRvU3RyaW5nKDE2KVxyXG4gICAgICAgIH0pXHJcbiAgICB9XHJcbn1cclxuXHJcbmxldCBDaGVja05hbWluZ0NvbnZlbnRpb24gPSAodmFsdWU6IHN0cmluZykgPT5cclxue1xyXG4gICAgdmFyIHJlZ0V4cDogYW55XHJcblxyXG4gICAgaWYgKHZhbHVlLmxlbmd0aCA9PT0gMClcclxuICAgIHtcclxuICAgICAgICByZXR1cm4geyBIYXNFcnJvcjogdHJ1ZSwgRXJyb3JEZXNjcmlwdGlvbjogRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9OYW1lRW1wdHlcIl0gfVxyXG4gICAgfVxyXG5cclxuICAgIHJlZ0V4cCA9IG5ldyBSZWdFeHAoXCJbXkEtWmEtejAtOV9dXCIpXHJcblxyXG4gICAgaWYgKHJlZ0V4cC50ZXN0KHZhbHVlKSlcclxuICAgIHtcclxuICAgICAgICByZXR1cm4geyBIYXNFcnJvcjogdHJ1ZSwgRXJyb3JEZXNjcmlwdGlvbjogRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9JbnZhbGlkQ2hhcmFjdGVyc1wiXSB9XHJcbiAgICB9XHJcblxyXG4gICAgcmVnRXhwID0gbmV3IFJlZ0V4cChcIl5bMC05X11cIilcclxuXHJcbiAgICBpZiAocmVnRXhwLnRlc3QodmFsdWUpKVxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB7IEhhc0Vycm9yOiB0cnVlLCBFcnJvckRlc2NyaXB0aW9uOiBFcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0ludmFsaWRMZWFkaW5nQ2hhcmFjdGVyXCJdIH1cclxuICAgIH1cclxuXHJcbiAgICByZXR1cm4ge1xyXG4gICAgICAgIEhhc0Vycm9yOiBmYWxzZSxcclxuICAgICAgICBFcnJvckRlc2NyaXB0aW9uOiBcIlwiXHJcbiAgICB9XHJcbn0iLCJjbGFzcyBQbHVnaW5GYWN0b3J5XHJcbntcclxuICAgIHB1YmxpYyBzdGF0aWMgQ3JlYXRlUGx1Z2luVmlld01vZGVsQXN5bmMgPSBhc3luYyAocGx1Z2luVHlwZTogc3RyaW5nLCBwbHVnaW5Nb2RlbDogYW55KSA9PlxyXG4gICAge1xyXG4gICAgICAgIGxldCBwbHVnaW5JZGVudGlmaWNhdGlvbjogUGx1Z2luSWRlbnRpZmljYXRpb25WaWV3TW9kZWxcclxuICAgICAgICBsZXQgcGx1Z2luVmlld01vZGVsOiBQbHVnaW5WaWV3TW9kZWxCYXNlXHJcbiAgICAgICAgbGV0IHBsdWdpblZpZXdNb2RlbFJhdzogc3RyaW5nXHJcblxyXG4gICAgICAgIHBsdWdpbklkZW50aWZpY2F0aW9uID0gUGx1Z2luSGl2ZS5GaW5kUGx1Z2luSWRlbnRpZmljYXRpb24ocGx1Z2luVHlwZSwgcGx1Z2luTW9kZWwuRGVzY3JpcHRpb24uSWQpXHJcblxyXG4gICAgICAgIGlmIChwbHVnaW5JZGVudGlmaWNhdGlvbilcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHBsdWdpblZpZXdNb2RlbFJhdyA9IGF3YWl0IENvbm5lY3Rpb25NYW5hZ2VyLkludm9rZVdlYkNsaWVudEh1YihcIkdldFBsdWdpblN0cmluZ1Jlc291cmNlXCIsIHBsdWdpbk1vZGVsLkRlc2NyaXB0aW9uLklkLCBwbHVnaW5JZGVudGlmaWNhdGlvbi5WaWV3TW9kZWxSZXNvdXJjZU5hbWUpXHJcbiAgICAgICAgICAgIHBsdWdpblZpZXdNb2RlbCA9IDxQbHVnaW5WaWV3TW9kZWxCYXNlPm5ldyBGdW5jdGlvbihwbHVnaW5WaWV3TW9kZWxSYXcgKyBcIjsgcmV0dXJuIFZpZXdNb2RlbENvbnN0cnVjdG9yXCIpKCkocGx1Z2luTW9kZWwsIHBsdWdpbklkZW50aWZpY2F0aW9uKVxyXG5cclxuICAgICAgICAgICAgcmV0dXJuIHBsdWdpblZpZXdNb2RlbFxyXG4gICAgICAgIH1cclxuICAgICAgICBlbHNlXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aHJvdyBuZXcgRXJyb3IoXCJObyBjb3JyZXNwb25kaW5nIHBsdWdpbiBkZXNjcmlwdGlvbiBmb3VuZC5cIilcclxuICAgICAgICB9XHJcbiAgICB9XHJcbn0iLCJjbGFzcyBQbHVnaW5IaXZlXHJcbntcclxuICAgIC8vIGZpZWxkc1xyXG4gICAgcHVibGljIHN0YXRpYyBQbHVnaW5JZGVudGlmaWNhdGlvblNldDogTWFwPHN0cmluZywgUGx1Z2luSWRlbnRpZmljYXRpb25WaWV3TW9kZWxbXT5cclxuXHJcbiAgICAvLyBjb25zdHJ1Y3RvcnNcclxuICAgIHN0YXRpYyBJbml0aWFsaXplID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICBQbHVnaW5IaXZlLlBsdWdpbklkZW50aWZpY2F0aW9uU2V0ID0gbmV3IE1hcDxzdHJpbmcsIFBsdWdpbklkZW50aWZpY2F0aW9uVmlld01vZGVsW10+KClcclxuICAgIH0gICBcclxuXHJcbiAgICBzdGF0aWMgRmluZFBsdWdpbklkZW50aWZpY2F0aW9uID0gKHBsdWdpblR5cGVOYW1lOiBzdHJpbmcsIHBsdWdpbklkOiBzdHJpbmcpID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIFBsdWdpbkhpdmUuUGx1Z2luSWRlbnRpZmljYXRpb25TZXQuZ2V0KHBsdWdpblR5cGVOYW1lKS5maW5kKHBsdWdpbklkZW50aWZpY2F0aW9uID0+IHBsdWdpbklkZW50aWZpY2F0aW9uLklkID09PSBwbHVnaW5JZCk7XHJcbiAgICB9XHJcbn0iLCJjbGFzcyBDaGFubmVsSHViVmlld01vZGVsXHJcbntcclxuICAgIHB1YmxpYyBOYW1lOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIEdyb3VwOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IERhdGFUeXBlOiBLbm9ja291dE9ic2VydmFibGU8T25lRGFzRGF0YVR5cGVFbnVtPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IEd1aWQ6IHN0cmluZ1xyXG4gICAgcHVibGljIHJlYWRvbmx5IENyZWF0aW9uRGF0ZVRpbWU6IHN0cmluZ1xyXG4gICAgcHVibGljIHJlYWRvbmx5IFVuaXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgVHJhbnNmZXJGdW5jdGlvblNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8VHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbD5cclxuICAgIHB1YmxpYyBTZWxlY3RlZFRyYW5zZmVyRnVuY3Rpb246IEtub2Nrb3V0T2JzZXJ2YWJsZTxUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsPlxyXG4gICAgcHVibGljIEV2YWx1YXRlZFRyYW5zZmVyRnVuY3Rpb25TZXQ6ICgodmFsdWU6IG51bWJlcikgPT4gbnVtYmVyKVtdXHJcbiAgICBwdWJsaWMgSXNTZWxlY3RlZDogS25vY2tvdXRPYnNlcnZhYmxlPGJvb2xlYW4+XHJcblxyXG4gICAgcHVibGljIHJlYWRvbmx5IERhdGFUeXBlU2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxPbmVEYXNEYXRhVHlwZUVudW0+XHJcblxyXG4gICAgcHVibGljIHJlYWRvbmx5IEFzc29jaWF0ZWREYXRhSW5wdXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZTxEYXRhUG9ydFZpZXdNb2RlbD5cclxuICAgIHB1YmxpYyByZWFkb25seSBBc3NvY2lhdGVkRGF0YU91dHB1dFNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8RGF0YVBvcnRWaWV3TW9kZWw+XHJcblxyXG4gICAgcHJpdmF0ZSBBc3NvY2lhdGVkRGF0YUlucHV0SWQ6IHN0cmluZ1xyXG4gICAgcHJpdmF0ZSBBc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0OiBzdHJpbmdbXVxyXG5cclxuICAgIGNvbnN0cnVjdG9yKGNoYW5uZWxIdWJNb2RlbDogQ2hhbm5lbEh1Yk1vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuTmFtZSA9IGtvLm9ic2VydmFibGU8c3RyaW5nPihjaGFubmVsSHViTW9kZWwuTmFtZSlcclxuICAgICAgICB0aGlzLkdyb3VwID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KGNoYW5uZWxIdWJNb2RlbC5Hcm91cClcclxuICAgICAgICB0aGlzLkRhdGFUeXBlU2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PE9uZURhc0RhdGFUeXBlRW51bT4oRW51bWVyYXRpb25IZWxwZXIuR2V0RW51bVZhbHVlcygnT25lRGFzRGF0YVR5cGVFbnVtJykpXHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZSA9IGtvLm9ic2VydmFibGU8T25lRGFzRGF0YVR5cGVFbnVtPihjaGFubmVsSHViTW9kZWwuRGF0YVR5cGUpXHJcbiAgICAgICAgdGhpcy5HdWlkID0gY2hhbm5lbEh1Yk1vZGVsLkd1aWRcclxuICAgICAgICB0aGlzLkNyZWF0aW9uRGF0ZVRpbWUgPSBjaGFubmVsSHViTW9kZWwuQ3JlYXRpb25EYXRlVGltZVxyXG4gICAgICAgIHRoaXMuVW5pdCA9IGtvLm9ic2VydmFibGU8c3RyaW5nPihjaGFubmVsSHViTW9kZWwuVW5pdClcclxuICAgICAgICB0aGlzLlRyYW5zZmVyRnVuY3Rpb25TZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8VHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbD4oY2hhbm5lbEh1Yk1vZGVsLlRyYW5zZmVyRnVuY3Rpb25TZXQubWFwKHRmID0+IG5ldyBUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsKHRmKSkpXHJcbiAgICAgICAgdGhpcy5TZWxlY3RlZFRyYW5zZmVyRnVuY3Rpb24gPSBrby5vYnNlcnZhYmxlPFRyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWw+KHRoaXMuQ3JlYXRlRGVmYXVsdFRyYW5zZmVyRnVuY3Rpb24oKSlcclxuICAgICAgICB0aGlzLklzU2VsZWN0ZWQgPSBrby5vYnNlcnZhYmxlPGJvb2xlYW4+KGZhbHNlKVxyXG5cclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXQgPSBrby5vYnNlcnZhYmxlPERhdGFQb3J0Vmlld01vZGVsPigpXHJcbiAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dFNldCA9IGtvLm9ic2VydmFibGVBcnJheTxEYXRhUG9ydFZpZXdNb2RlbD4oKVxyXG5cclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXRJZCA9IGNoYW5uZWxIdWJNb2RlbC5Bc3NvY2lhdGVkRGF0YUlucHV0SWRcclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQgPSBjaGFubmVsSHViTW9kZWwuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldFxyXG4gICAgICAgIHRoaXMuRXZhbHVhdGVkVHJhbnNmZXJGdW5jdGlvblNldCA9IFtdXHJcbiAgICB9XHJcblxyXG4gICAgLy8gbWV0aG9kc1xyXG4gICAgcHVibGljIEdldFRyYW5zZm9ybWVkVmFsdWUgPSAodmFsdWU6IGFueSk6IHN0cmluZyA9PiBcclxuICAgIHtcclxuICAgICAgICBpZiAodmFsdWUgPT09IFwiTmFOXCIpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB2YWx1ZSA9IE5hTlxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5FdmFsdWF0ZWRUcmFuc2ZlckZ1bmN0aW9uU2V0LmZvckVhY2godGYgPT4gdmFsdWUgPSB0Zih2YWx1ZSkpXHJcblxyXG4gICAgICAgIHJldHVybiB2YWx1ZVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgQ3JlYXRlRGVmYXVsdFRyYW5zZmVyRnVuY3Rpb24gPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiBuZXcgVHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbChuZXcgVHJhbnNmZXJGdW5jdGlvbk1vZGVsKFwiMDAwMS0wMS0wMVQwMDowMDowMFpcIiwgXCJwb2x5bm9taWFsXCIsIFwicGVybWFuZW50XCIsIFwiMTswXCIpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBJc0Fzc29jaWF0aW9uQWxsb3dlZChkYXRhUG9ydDogRGF0YVBvcnRWaWV3TW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIChkYXRhUG9ydC5EYXRhVHlwZSAmIDB4ZmYpID09ICh0aGlzLkRhdGFUeXBlKCkgJiAweGZmKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBVcGRhdGVBc3NvY2lhdGlvbiA9IChkYXRhUG9ydDogRGF0YVBvcnRWaWV3TW9kZWwpID0+XHJcbiAgICB7XHJcbiAgICAgICAgc3dpdGNoIChkYXRhUG9ydC5EYXRhRGlyZWN0aW9uKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dDpcclxuXHJcbiAgICAgICAgICAgICAgICB0aGlzLlJlc2V0QXNzb2NpYXRpb24oZmFsc2UsIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dCgpKVxyXG4gICAgICAgICAgICAgICAgYnJlYWtcclxuXHJcbiAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uT3V0cHV0OlxyXG5cclxuICAgICAgICAgICAgICAgIHRoaXMuUmVzZXRBc3NvY2lhdGlvbihmYWxzZSwgZGF0YVBvcnQpXHJcbiAgICAgICAgICAgICAgICBicmVha1xyXG5cclxuICAgICAgICAgICAgZGVmYXVsdDpcclxuICAgICAgICAgICAgICAgIHRocm93IG5ldyBFcnJvcihcIk5vdCBpbXBsZW1lbnRlZC5cIik7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0aGlzLlNldEFzc29jaWF0aW9uKGRhdGFQb3J0KVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBTZXRBc3NvY2lhdGlvbihkYXRhUG9ydDogRGF0YVBvcnRWaWV3TW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgZGF0YVBvcnQuQXNzb2NpYXRlZENoYW5uZWxIdWJTZXQucHVzaCh0aGlzKVxyXG5cclxuICAgICAgICBzd2l0Y2ggKGRhdGFQb3J0LkRhdGFEaXJlY3Rpb24pXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLklucHV0OlxyXG5cclxuICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dChkYXRhUG9ydClcclxuICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dElkID0gZGF0YVBvcnQuVG9GdWxsUXVhbGlmaWVkSWRlbnRpZmllcigpXHJcblxyXG4gICAgICAgICAgICAgICAgYnJlYWtcclxuXHJcbiAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uT3V0cHV0OlxyXG5cclxuICAgICAgICAgICAgICAgIGxldCBkYXRhT3V0cHV0SWQgPSBkYXRhUG9ydC5Ub0Z1bGxRdWFsaWZpZWRJZGVudGlmaWVyKClcclxuXHJcbiAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0U2V0LnB1c2goZGF0YVBvcnQpXHJcblxyXG4gICAgICAgICAgICAgICAgaWYgKHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldC5pbmRleE9mKGRhdGFPdXRwdXRJZCkgPCAwKVxyXG4gICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldC5wdXNoKGRhdGFQb3J0LlRvRnVsbFF1YWxpZmllZElkZW50aWZpZXIoKSlcclxuICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICBicmVha1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgUmVzZXRBc3NvY2lhdGlvbihtYWludGFpbldlYWtSZWZlcmVuY2U6IGJvb2xlYW4sIC4uLmRhdGFQb3J0U2V0OiBEYXRhUG9ydFZpZXdNb2RlbFtdKVxyXG4gICAge1xyXG4gICAgICAgIGRhdGFQb3J0U2V0LmZvckVhY2goZGF0YVBvcnQgPT5cclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGlmICghZGF0YVBvcnQpXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIHJldHVyblxyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICBkYXRhUG9ydC5Bc3NvY2lhdGVkQ2hhbm5lbEh1YlNldC5yZW1vdmUodGhpcylcclxuXHJcbiAgICAgICAgICAgIHN3aXRjaCAoZGF0YVBvcnQuRGF0YURpcmVjdGlvbilcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dDpcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0KG51bGwpXHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmICghbWFpbnRhaW5XZWFrUmVmZXJlbmNlKVxyXG4gICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0SWQgPSBudWxsXHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBicmVha1xyXG5cclxuICAgICAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uT3V0cHV0OlxyXG5cclxuICAgICAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0U2V0LnJlbW92ZShkYXRhUG9ydClcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCFtYWludGFpbldlYWtSZWZlcmVuY2UpXHJcbiAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBsZXQgaW5kZXg6IG51bWJlciA9IHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldC5pbmRleE9mKGRhdGFQb3J0LlRvRnVsbFF1YWxpZmllZElkZW50aWZpZXIoKSlcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmIChpbmRleCA+IC0xKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQuc3BsaWNlKGluZGV4LCAxKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9ICAgXHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBicmVha1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgUmVzZXRBbGxBc3NvY2lhdGlvbnMobWFpbnRhaW5XZWFrUmVmZXJlbmNlOiBib29sZWFuKVxyXG4gICAge1xyXG4gICAgICAgIGlmICh0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXQoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuUmVzZXRBc3NvY2lhdGlvbihtYWludGFpbldlYWtSZWZlcmVuY2UsIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dCgpKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5SZXNldEFzc29jaWF0aW9uKG1haW50YWluV2Vha1JlZmVyZW5jZSwgLi4udGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dFNldCgpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBHZXRBc3NvY2lhdGVkRGF0YUlucHV0SWQgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXRJZDtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgR2V0QXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldDtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHtcclxuICAgICAgICAgICAgTmFtZTogPHN0cmluZz50aGlzLk5hbWUoKSxcclxuICAgICAgICAgICAgR3JvdXA6IDxzdHJpbmc+dGhpcy5Hcm91cCgpLFxyXG4gICAgICAgICAgICBEYXRhVHlwZTogPE9uZURhc0RhdGFUeXBlRW51bT50aGlzLkRhdGFUeXBlKCksXHJcbiAgICAgICAgICAgIEd1aWQ6IDxzdHJpbmc+dGhpcy5HdWlkLFxyXG4gICAgICAgICAgICBDcmVhdGlvbkRhdGVUaW1lOiA8c3RyaW5nPnRoaXMuQ3JlYXRpb25EYXRlVGltZSxcclxuICAgICAgICAgICAgVW5pdDogPHN0cmluZz50aGlzLlVuaXQoKSxcclxuICAgICAgICAgICAgVHJhbnNmZXJGdW5jdGlvblNldDogPFRyYW5zZmVyRnVuY3Rpb25Nb2RlbFtdPnRoaXMuVHJhbnNmZXJGdW5jdGlvblNldCgpLm1hcCh0ZiA9PiB0Zi5Ub01vZGVsKCkpLFxyXG4gICAgICAgICAgICBBc3NvY2lhdGVkRGF0YUlucHV0SWQ6IDxzdHJpbmc+dGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0SWQsXHJcbiAgICAgICAgICAgIEFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQ6IDxzdHJpbmdbXT50aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXRcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEFkZFRyYW5zZmVyRnVuY3Rpb24gPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuVHJhbnNmZXJGdW5jdGlvblNldC5wdXNoKHRoaXMuU2VsZWN0ZWRUcmFuc2ZlckZ1bmN0aW9uKCkpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIERlbGV0ZVRyYW5zZmVyRnVuY3Rpb24gPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuVHJhbnNmZXJGdW5jdGlvblNldC5yZW1vdmUodGhpcy5TZWxlY3RlZFRyYW5zZmVyRnVuY3Rpb24oKSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgTmV3VHJhbnNmZXJGdW5jdGlvbiA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5TZWxlY3RlZFRyYW5zZmVyRnVuY3Rpb24odGhpcy5DcmVhdGVEZWZhdWx0VHJhbnNmZXJGdW5jdGlvbigpKVxyXG4gICAgfVxyXG5cclxuICAgIC8vIGNvbW1hbmRzXHJcbiAgICBwdWJsaWMgU2VsZWN0VHJhbnNmZXJGdW5jdGlvbiA9ICh0cmFuc2ZlckZ1bmN0aW9uOiBUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuU2VsZWN0ZWRUcmFuc2ZlckZ1bmN0aW9uKHRyYW5zZmVyRnVuY3Rpb24pXHJcbiAgICB9XHJcbn0iLCJjbGFzcyBPbmVEYXNNb2R1bGVWaWV3TW9kZWxcclxue1xyXG4gICAgcHVibGljIERhdGFUeXBlOiBLbm9ja291dE9ic2VydmFibGU8T25lRGFzRGF0YVR5cGVFbnVtPlxyXG4gICAgcHVibGljIERhdGFEaXJlY3Rpb246IEtub2Nrb3V0T2JzZXJ2YWJsZTxEYXRhRGlyZWN0aW9uRW51bT5cclxuICAgIHB1YmxpYyBFbmRpYW5uZXNzOiBLbm9ja291dE9ic2VydmFibGU8RW5kaWFubmVzc0VudW0+XHJcbiAgICBwdWJsaWMgU2l6ZTogS25vY2tvdXRPYnNlcnZhYmxlPG51bWJlcj5cclxuICAgIHB1YmxpYyBNYXhTaXplOiBLbm9ja291dE9ic2VydmFibGU8bnVtYmVyPlxyXG4gICAgcHVibGljIEVycm9yTWVzc2FnZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBIYXNFcnJvcjogS25vY2tvdXRDb21wdXRlZDxib29sZWFuPlxyXG5cclxuICAgIHB1YmxpYyBEYXRhVHlwZVNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8T25lRGFzRGF0YVR5cGVFbnVtPlxyXG5cclxuICAgIHByaXZhdGUgX29uUHJvcGVydHlDaGFuZ2VkOiBFdmVudERpc3BhdGNoZXI8T25lRGFzTW9kdWxlVmlld01vZGVsLCBhbnk+XHJcbiAgICBwcm90ZWN0ZWQgX21vZGVsOiBhbnlcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihvbmVEYXNNb2R1bGVNb2RlbDogT25lRGFzTW9kdWxlTW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5fbW9kZWwgPSBvbmVEYXNNb2R1bGVNb2RlbFxyXG5cclxuICAgICAgICB0aGlzLkRhdGFUeXBlU2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PE9uZURhc0RhdGFUeXBlRW51bT4oRW51bWVyYXRpb25IZWxwZXIuR2V0RW51bVZhbHVlcygnT25lRGFzRGF0YVR5cGVFbnVtJykuZmlsdGVyKGRhdGFUeXBlID0+IGRhdGFUeXBlICE9PSBPbmVEYXNEYXRhVHlwZUVudW0uQk9PTEVBTikpXHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZSA9IGtvLm9ic2VydmFibGU8T25lRGFzRGF0YVR5cGVFbnVtPihvbmVEYXNNb2R1bGVNb2RlbC5EYXRhVHlwZSlcclxuICAgICAgICB0aGlzLkRhdGFEaXJlY3Rpb24gPSBrby5vYnNlcnZhYmxlPERhdGFEaXJlY3Rpb25FbnVtPihvbmVEYXNNb2R1bGVNb2RlbC5EYXRhRGlyZWN0aW9uKVxyXG4gICAgICAgIHRoaXMuRW5kaWFubmVzcyA9IGtvLm9ic2VydmFibGU8RW5kaWFubmVzc0VudW0+KG9uZURhc01vZHVsZU1vZGVsLkVuZGlhbm5lc3MpXHJcbiAgICAgICAgdGhpcy5TaXplID0ga28ub2JzZXJ2YWJsZTxudW1iZXI+KG9uZURhc01vZHVsZU1vZGVsLlNpemUpXHJcbiAgICAgICAgdGhpcy5NYXhTaXplID0ga28ub2JzZXJ2YWJsZTxudW1iZXI+KEluZmluaXR5KVxyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KFwiXCIpXHJcbiAgICAgICAgdGhpcy5IYXNFcnJvciA9IGtvLmNvbXB1dGVkPGJvb2xlYW4+KCgpID0+IHRoaXMuRXJyb3JNZXNzYWdlKCkubGVuZ3RoID4gMClcclxuXHJcbiAgICAgICAgdGhpcy5fb25Qcm9wZXJ0eUNoYW5nZWQgPSBuZXcgRXZlbnREaXNwYXRjaGVyPE9uZURhc01vZHVsZVZpZXdNb2RlbCwgYW55PigpO1xyXG5cclxuICAgICAgICB0aGlzLkRhdGFUeXBlLnN1YnNjcmliZShuZXdWYWx1ZSA9PiB0aGlzLk9uUHJvcGVydHlDaGFuZ2VkKCkpXHJcbiAgICAgICAgdGhpcy5EYXRhRGlyZWN0aW9uLnN1YnNjcmliZShuZXdWYWx1ZSA9PiB0aGlzLk9uUHJvcGVydHlDaGFuZ2VkKCkpXHJcbiAgICAgICAgdGhpcy5TaXplLnN1YnNjcmliZShuZXdWYWx1ZSA9PiB0aGlzLk9uUHJvcGVydHlDaGFuZ2VkKCkpXHJcbiAgICB9XHJcblxyXG4gICAgZ2V0IFByb3BlcnR5Q2hhbmdlZCgpOiBJRXZlbnQ8T25lRGFzTW9kdWxlVmlld01vZGVsLCBhbnk+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuX29uUHJvcGVydHlDaGFuZ2VkO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBPblByb3BlcnR5Q2hhbmdlZCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5WYWxpZGF0ZSgpXHJcbiAgICAgICAgdGhpcy5fb25Qcm9wZXJ0eUNoYW5nZWQuZGlzcGF0Y2godGhpcywgbnVsbClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgR2V0Qnl0ZUNvdW50ID0gKGJvb2xlYW5CaXRTaXplPzogbnVtYmVyKSA9PlxyXG4gICAge1xyXG4gICAgICAgIGlmIChib29sZWFuQml0U2l6ZSAmJiB0aGlzLkRhdGFUeXBlKCkgPT09IE9uZURhc0RhdGFUeXBlRW51bS5CT09MRUFOKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgYm9vbGVhbkJpdFNpemUgPSBwYXJzZUludCg8YW55PmJvb2xlYW5CaXRTaXplKVxyXG5cclxuICAgICAgICAgICAgcmV0dXJuIE1hdGguY2VpbChib29sZWFuQml0U2l6ZSAqIHRoaXMuU2l6ZSgpIC8gOCk7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIGVsc2VcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHJldHVybiAodGhpcy5EYXRhVHlwZSgpICYgMHgwRkYpIC8gOCAqIHRoaXMuU2l6ZSgpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBWYWxpZGF0ZSgpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJcIilcclxuXHJcbiAgICAgICAgaWYgKHRoaXMuU2l6ZSgpIDwgMSB8fCAoaXNGaW5pdGUodGhpcy5NYXhTaXplKCkpICYmIHRoaXMuU2l6ZSgpID4gdGhpcy5NYXhTaXplKCkpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJTaXplIG11c3QgYmUgd2l0aGluIHJhbmdlIDEuLlwiICsgdGhpcy5NYXhTaXplKCkgKyBcIi5cIilcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvU3RyaW5nKClcclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5TaXplKCkgKyBcInggXCIgKyBFbnVtZXJhdGlvbkhlbHBlci5HZXRFbnVtTG9jYWxpemF0aW9uKCdPbmVEYXNEYXRhVHlwZUVudW0nLCB0aGlzLkRhdGFUeXBlKCkpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEV4dGVuZE1vZGVsKG1vZGVsOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgLy9cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IG1vZGVsOiBhbnkgPSB7XHJcbiAgICAgICAgICAgICR0eXBlOiA8c3RyaW5nPnRoaXMuX21vZGVsLiR0eXBlLFxyXG4gICAgICAgICAgICBEYXRhVHlwZTogPE9uZURhc0RhdGFUeXBlRW51bT50aGlzLkRhdGFUeXBlKCksXHJcbiAgICAgICAgICAgIFNpemU6IDxudW1iZXI+dGhpcy5TaXplKCksXHJcbiAgICAgICAgICAgIERhdGFEaXJlY3Rpb246IDxEYXRhRGlyZWN0aW9uRW51bT50aGlzLkRhdGFEaXJlY3Rpb24oKSxcclxuICAgICAgICAgICAgRW5kaWFubmVzczogPEVuZGlhbm5lc3NFbnVtPnRoaXMuRW5kaWFubmVzcygpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0aGlzLkV4dGVuZE1vZGVsKG1vZGVsKVxyXG5cclxuICAgICAgICByZXR1cm4gbW9kZWxcclxuICAgIH1cclxufVxyXG4iLCJjbGFzcyBPbmVEYXNNb2R1bGVTZWxlY3RvclZpZXdNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgU2V0dGluZ3NUZW1wbGF0ZU5hbWU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgTmV3TW9kdWxlOiBLbm9ja291dE9ic2VydmFibGU8T25lRGFzTW9kdWxlVmlld01vZGVsPiAgXHJcbiAgICBwdWJsaWMgTWF4Qnl0ZXM6IEtub2Nrb3V0T2JzZXJ2YWJsZTxudW1iZXI+XHJcbiAgICBwdWJsaWMgUmVtYWluaW5nQnl0ZXM6IEtub2Nrb3V0T2JzZXJ2YWJsZTxudW1iZXI+XHJcbiAgICBwdWJsaWMgUmVtYWluaW5nQ291bnQ6IEtub2Nrb3V0T2JzZXJ2YWJsZTxudW1iZXI+ICAgIFxyXG4gICAgcHVibGljIE1vZHVsZVNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8T25lRGFzTW9kdWxlVmlld01vZGVsPlxyXG4gICAgcHVibGljIEVycm9yTWVzc2FnZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBIYXNFcnJvcjogS25vY2tvdXRDb21wdXRlZDxib29sZWFuPlxyXG5cclxuICAgIHB1YmxpYyBPbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxPbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtPlxyXG5cclxuICAgIHByaXZhdGUgX29uTW9kdWxlU2V0Q2hhbmdlZDogRXZlbnREaXNwYXRjaGVyPE9uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsLCBPbmVEYXNNb2R1bGVWaWV3TW9kZWxbXT5cclxuXHJcbiAgICBjb25zdHJ1Y3RvcihvbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGU6IE9uZURhc01vZHVsZVNlbGVjdG9yTW9kZUVudW0sIG1vZHVsZVNldDogT25lRGFzTW9kdWxlVmlld01vZGVsW10gPSBbXSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yTW9kZSA9IGtvLm9ic2VydmFibGU8T25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlRW51bT4ob25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlKVxyXG5cclxuICAgICAgICB0aGlzLlNldHRpbmdzVGVtcGxhdGVOYW1lID0ga28ub2JzZXJ2YWJsZShcIlByb2plY3RfT25lRGFzTW9kdWxlVGVtcGxhdGVcIilcclxuICAgICAgICB0aGlzLk5ld01vZHVsZSA9IGtvLm9ic2VydmFibGU8T25lRGFzTW9kdWxlVmlld01vZGVsPigpO1xyXG4gICAgICAgIHRoaXMuTWF4Qnl0ZXMgPSBrby5vYnNlcnZhYmxlPG51bWJlcj4oSW5maW5pdHkpO1xyXG4gICAgICAgIHRoaXMuUmVtYWluaW5nQnl0ZXMgPSBrby5vYnNlcnZhYmxlPG51bWJlcj4oTmFOKTtcclxuICAgICAgICB0aGlzLlJlbWFpbmluZ0NvdW50ID0ga28ub2JzZXJ2YWJsZTxudW1iZXI+KE5hTik7XHJcbiAgICAgICAgdGhpcy5Nb2R1bGVTZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8T25lRGFzTW9kdWxlVmlld01vZGVsPihtb2R1bGVTZXQpO1xyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KFwiXCIpXHJcbiAgICAgICAgdGhpcy5IYXNFcnJvciA9IGtvLmNvbXB1dGVkPGJvb2xlYW4+KCgpID0+IHRoaXMuRXJyb3JNZXNzYWdlKCkubGVuZ3RoID4gMClcclxuXHJcbiAgICAgICAgdGhpcy5fb25Nb2R1bGVTZXRDaGFuZ2VkID0gbmV3IEV2ZW50RGlzcGF0Y2hlcjxPbmVEYXNNb2R1bGVTZWxlY3RvclZpZXdNb2RlbCwgT25lRGFzTW9kdWxlVmlld01vZGVsW10+KCk7XHJcblxyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxDcmVhdGVOZXdNb2R1bGUoKVxyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgfVxyXG5cclxuICAgIGdldCBPbk1vZHVsZVNldENoYW5nZWQoKTogSUV2ZW50PE9uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsLCBPbmVEYXNNb2R1bGVWaWV3TW9kZWxbXT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5fb25Nb2R1bGVTZXRDaGFuZ2VkO1xyXG4gICAgfVxyXG4gICAgXHJcbiAgICAvLyBtZXRob2RzXHJcbiAgICBwdWJsaWMgU2V0TWF4Qnl0ZXMgPSAodmFsdWU6IG51bWJlcikgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLk1heEJ5dGVzKHZhbHVlKVxyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBHZXRJbnB1dE1vZHVsZVNldCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuTW9kdWxlU2V0KCkuZmlsdGVyKG1vZHVsZSA9PiBtb2R1bGUuRGF0YURpcmVjdGlvbigpID09PSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgR2V0T3V0cHV0TW9kdWxlU2V0ID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5Nb2R1bGVTZXQoKS5maWx0ZXIobW9kdWxlID0+IG1vZHVsZS5EYXRhRGlyZWN0aW9uKCkgPT09IERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dClcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIEludGVybmFsVXBkYXRlKClcclxuICAgIHtcclxuICAgICAgICB0aGlzLlVwZGF0ZSgpXHJcbiAgICAgICAgdGhpcy5WYWxpZGF0ZSgpXHJcbiAgICB9XHJcblxyXG4gICAgcHJvdGVjdGVkIFVwZGF0ZSgpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IG1vZHVsZVNldDogT25lRGFzTW9kdWxlVmlld01vZGVsW11cclxuICAgICAgICBsZXQgcmVtYWluaW5nQnl0ZXM6IG51bWJlclxyXG5cclxuICAgICAgICBzd2l0Y2ggKHRoaXMuTmV3TW9kdWxlKCkuRGF0YURpcmVjdGlvbigpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dDpcclxuICAgICAgICAgICAgICAgIG1vZHVsZVNldCA9IHRoaXMuR2V0SW5wdXRNb2R1bGVTZXQoKVxyXG4gICAgICAgICAgICAgICAgYnJlYWs7XHJcblxyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dDpcclxuICAgICAgICAgICAgICAgIG1vZHVsZVNldCA9IHRoaXMuR2V0T3V0cHV0TW9kdWxlU2V0KClcclxuICAgICAgICAgICAgICAgIGJyZWFrO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmVtYWluaW5nQnl0ZXMgPSB0aGlzLk1heEJ5dGVzKCkgLSBtb2R1bGVTZXQubWFwKG9uZURhc01vZHVsZSA9PiBvbmVEYXNNb2R1bGUuR2V0Qnl0ZUNvdW50KCkpLnJlZHVjZSgocHJldmlvdXNWYWx1ZSwgY3VycmVudFZhbHVlKSA9PiBwcmV2aW91c1ZhbHVlICsgY3VycmVudFZhbHVlLCAwKVxyXG5cclxuICAgICAgICB0aGlzLlJlbWFpbmluZ0J5dGVzKHJlbWFpbmluZ0J5dGVzKVxyXG4gICAgICAgIHRoaXMuUmVtYWluaW5nQ291bnQoTWF0aC5mbG9vcih0aGlzLlJlbWFpbmluZ0J5dGVzKCkgLyAoKHRoaXMuTmV3TW9kdWxlKCkuRGF0YVR5cGUoKSAmIDB4MEZGKSAvIDgpKSlcclxuICAgIH1cclxuXHJcbiAgICBwcm90ZWN0ZWQgVmFsaWRhdGUoKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiXCIpXHJcblxyXG4gICAgICAgIGlmICh0aGlzLk5ld01vZHVsZSgpLkhhc0Vycm9yKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIlJlc29sdmUgYWxsIHJlbWFpbmluZyBtb2R1bGUgZXJyb3JzIGJlZm9yZSBjb250aW51aW5nLlwiKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaWYgKHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlKCkgPT09IE9uZURhc01vZHVsZVNlbGVjdG9yTW9kZUVudW0uSW5wdXRPbmx5ICYmIHRoaXMuTmV3TW9kdWxlKCkuRGF0YURpcmVjdGlvbigpID09IERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dClcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiT25seSBpbnB1dCBtb2R1bGVzIGFyZSBhbGxvd2VkLlwiKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaWYgKHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlKCkgPT09IE9uZURhc01vZHVsZVNlbGVjdG9yTW9kZUVudW0uT3V0cHV0T25seSAmJiB0aGlzLk5ld01vZHVsZSgpLkRhdGFEaXJlY3Rpb24oKSA9PSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dClcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiT25seSBvdXRwdXQgbW9kdWxlcyBhcmUgYWxsb3dlZC5cIilcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmIChpc0Zpbml0ZSh0aGlzLlJlbWFpbmluZ0J5dGVzKCkpICYmICh0aGlzLlJlbWFpbmluZ0J5dGVzKCkgLSB0aGlzLk5ld01vZHVsZSgpLkdldEJ5dGVDb3VudCgpIDwgMCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIkJ5dGUgY291bnQgb2YgbmV3IG1vZHVsZSBpcyB0b28gaGlnaC5cIilcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmICh0aGlzLlJlbWFpbmluZ0NvdW50KCkgPD0gMClcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiVGhlIG1heGltdW0gbnVtYmVyIG9mIG1vZHVsZXMgaXMgcmVhY2hlZC5cIilcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHJvdGVjdGVkIENyZWF0ZU5ld01vZHVsZSgpXHJcbiAgICB7XHJcbiAgICAgICAgaWYgKHRoaXMuTmV3TW9kdWxlKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICByZXR1cm4gbmV3IE9uZURhc01vZHVsZVZpZXdNb2RlbChuZXcgT25lRGFzTW9kdWxlTW9kZWwodGhpcy5OZXdNb2R1bGUoKS5EYXRhVHlwZSgpLCB0aGlzLk5ld01vZHVsZSgpLkRhdGFEaXJlY3Rpb24oKSwgdGhpcy5OZXdNb2R1bGUoKS5FbmRpYW5uZXNzKCksIDEpKVxyXG4gICAgICAgIH1cclxuICAgICAgICBlbHNlXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICByZXR1cm4gbmV3IE9uZURhc01vZHVsZVZpZXdNb2RlbChuZXcgT25lRGFzTW9kdWxlTW9kZWwoT25lRGFzRGF0YVR5cGVFbnVtLlVJTlQxNiwgRGF0YURpcmVjdGlvbkVudW0uSW5wdXQsIEVuZGlhbm5lc3NFbnVtLkxpdHRsZUVuZGlhbiwgMSkpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgSW50ZXJuYWxDcmVhdGVOZXdNb2R1bGUoKVxyXG4gICAge1xyXG4gICAgICAgIGlmICh0aGlzLk5ld01vZHVsZSgpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5OZXdNb2R1bGUoKS5Qcm9wZXJ0eUNoYW5nZWQudW5zdWJzY3JpYmUodGhpcy5Pbk1vZHVsZVByb3BlcnR5Q2hhbmdlZClcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuTmV3TW9kdWxlKHRoaXMuQ3JlYXRlTmV3TW9kdWxlKCkpXHJcbiAgICAgICAgdGhpcy5OZXdNb2R1bGUoKS5Qcm9wZXJ0eUNoYW5nZWQuc3Vic2NyaWJlKHRoaXMuT25Nb2R1bGVQcm9wZXJ0eUNoYW5nZWQpXHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBPbk1vZHVsZVByb3BlcnR5Q2hhbmdlZCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5JbnRlcm5hbFVwZGF0ZSgpXHJcbiAgICB9XHJcblxyXG4gICAgLy8gY29tbWFuZHNcclxuICAgIHB1YmxpYyBBZGRNb2R1bGUgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIGxldCBuZXdNb2R1bGU6IE9uZURhc01vZHVsZVZpZXdNb2RlbFxyXG5cclxuICAgICAgICBpZiAoIXRoaXMuSGFzRXJyb3IoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuTW9kdWxlU2V0LnB1c2godGhpcy5OZXdNb2R1bGUoKSlcclxuICAgICAgICAgICAgdGhpcy5JbnRlcm5hbENyZWF0ZU5ld01vZHVsZSgpXHJcbiAgICAgICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgICAgICAgICB0aGlzLl9vbk1vZHVsZVNldENoYW5nZWQuZGlzcGF0Y2godGhpcywgdGhpcy5Nb2R1bGVTZXQoKSlcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIERlbGV0ZU1vZHVsZSA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5Nb2R1bGVTZXQucG9wKClcclxuICAgICAgICB0aGlzLkludGVybmFsVXBkYXRlKClcclxuICAgICAgICB0aGlzLl9vbk1vZHVsZVNldENoYW5nZWQuZGlzcGF0Y2godGhpcywgdGhpcy5Nb2R1bGVTZXQoKSlcclxuICAgIH1cclxufSIsImNsYXNzIFRyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWxcclxue1xyXG4gICAgcHVibGljIERhdGVUaW1lOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIFR5cGU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgT3B0aW9uOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIEFyZ3VtZW50OiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKHRyYW5zZmVyRnVuY3Rpb25Nb2RlbDogVHJhbnNmZXJGdW5jdGlvbk1vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuRGF0ZVRpbWUgPSBrby5vYnNlcnZhYmxlKHRyYW5zZmVyRnVuY3Rpb25Nb2RlbC5EYXRlVGltZSlcclxuICAgICAgICB0aGlzLlR5cGUgPSBrby5vYnNlcnZhYmxlKHRyYW5zZmVyRnVuY3Rpb25Nb2RlbC5UeXBlKVxyXG4gICAgICAgIHRoaXMuT3B0aW9uID0ga28ub2JzZXJ2YWJsZSh0cmFuc2ZlckZ1bmN0aW9uTW9kZWwuT3B0aW9uKVxyXG4gICAgICAgIHRoaXMuQXJndW1lbnQgPSBrby5vYnNlcnZhYmxlKHRyYW5zZmVyRnVuY3Rpb25Nb2RlbC5Bcmd1bWVudClcclxuICAgIH1cclxuXHJcbiAgICAvLyBtZXRob2RzXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIG5ldyBUcmFuc2ZlckZ1bmN0aW9uTW9kZWwodGhpcy5EYXRlVGltZSgpLCB0aGlzLlR5cGUoKSwgdGhpcy5PcHRpb24oKSwgdGhpcy5Bcmd1bWVudCgpKVxyXG4gICAgfVxyXG59IiwiY2xhc3MgQnVmZmVyUmVxdWVzdFNlbGVjdG9yVmlld01vZGVsXHJcbntcclxuICAgIHB1YmxpYyBOZXdCdWZmZXJSZXF1ZXN0OiBLbm9ja291dE9ic2VydmFibGU8QnVmZmVyUmVxdWVzdFZpZXdNb2RlbD4gIFxyXG4gICAgcHVibGljIEJ1ZmZlclJlcXVlc3RTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWw+XHJcbiAgICBwdWJsaWMgRXJyb3JNZXNzYWdlOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIEhhc0Vycm9yOiBLbm9ja291dENvbXB1dGVkPGJvb2xlYW4+XHJcblxyXG4gICAgcHJpdmF0ZSBfb25CdWZmZXJSZXF1ZXN0U2V0Q2hhbmdlZDogRXZlbnREaXNwYXRjaGVyPEJ1ZmZlclJlcXVlc3RTZWxlY3RvclZpZXdNb2RlbCwgQnVmZmVyUmVxdWVzdFZpZXdNb2RlbFtdPlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKGJ1ZmZlclJlcXVlc3RTZXQ6IEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWxbXSA9IFtdKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuTmV3QnVmZmVyUmVxdWVzdCA9IGtvLm9ic2VydmFibGU8QnVmZmVyUmVxdWVzdFZpZXdNb2RlbD4oKTtcclxuICAgICAgICB0aGlzLkJ1ZmZlclJlcXVlc3RTZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8QnVmZmVyUmVxdWVzdFZpZXdNb2RlbD4oYnVmZmVyUmVxdWVzdFNldCk7XHJcbiAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UgPSBrby5vYnNlcnZhYmxlPHN0cmluZz4oXCJcIilcclxuICAgICAgICB0aGlzLkhhc0Vycm9yID0ga28uY29tcHV0ZWQ8Ym9vbGVhbj4oKCkgPT4gdGhpcy5FcnJvck1lc3NhZ2UoKS5sZW5ndGggPiAwKVxyXG5cclxuICAgICAgICB0aGlzLl9vbkJ1ZmZlclJlcXVlc3RTZXRDaGFuZ2VkID0gbmV3IEV2ZW50RGlzcGF0Y2hlcjxCdWZmZXJSZXF1ZXN0U2VsZWN0b3JWaWV3TW9kZWwsIEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWxbXT4oKTtcclxuXHJcbiAgICAgICAgdGhpcy5JbnRlcm5hbENyZWF0ZU5ld0J1ZmZlclJlcXVlc3QoKVxyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgfVxyXG5cclxuICAgIGdldCBPbkJ1ZmZlclJlcXVlc3RTZXRDaGFuZ2VkKCk6IElFdmVudDxCdWZmZXJSZXF1ZXN0U2VsZWN0b3JWaWV3TW9kZWwsIEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWxbXT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5fb25CdWZmZXJSZXF1ZXN0U2V0Q2hhbmdlZDtcclxuICAgIH1cclxuICAgIFxyXG4gICAgLy8gbWV0aG9kc1xyXG4gICAgcHJpdmF0ZSBJbnRlcm5hbFVwZGF0ZSgpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5VcGRhdGUoKVxyXG4gICAgICAgIHRoaXMuVmFsaWRhdGUoKVxyXG4gICAgfVxyXG5cclxuICAgIHByb3RlY3RlZCBVcGRhdGUoKVxyXG4gICAge1xyXG4gICAgICAgIC8vXHJcbiAgICB9XHJcblxyXG4gICAgcHJvdGVjdGVkIFZhbGlkYXRlKClcclxuICAgIHtcclxuICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIlwiKVxyXG5cclxuICAgICAgICBpZiAodGhpcy5OZXdCdWZmZXJSZXF1ZXN0KCkuSGFzRXJyb3IoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiUmVzb2x2ZSBhbGwgcmVtYWluaW5nIGVycm9ycyBiZWZvcmUgY29udGludWluZy5cIilcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHJvdGVjdGVkIENyZWF0ZU5ld0J1ZmZlclJlcXVlc3QoKVxyXG4gICAge1xyXG4gICAgICAgIGlmICh0aGlzLk5ld0J1ZmZlclJlcXVlc3QoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHJldHVybiBuZXcgQnVmZmVyUmVxdWVzdFZpZXdNb2RlbChuZXcgQnVmZmVyUmVxdWVzdE1vZGVsKHRoaXMuTmV3QnVmZmVyUmVxdWVzdCgpLlNhbXBsZVJhdGUoKSwgdGhpcy5OZXdCdWZmZXJSZXF1ZXN0KCkuR3JvdXBGaWx0ZXIoKSkpXHJcbiAgICAgICAgfVxyXG4gICAgICAgIGVsc2VcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHJldHVybiBuZXcgQnVmZmVyUmVxdWVzdFZpZXdNb2RlbChuZXcgQnVmZmVyUmVxdWVzdE1vZGVsKFNhbXBsZVJhdGVFbnVtLlNhbXBsZVJhdGVfMSwgXCIqXCIpKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIEludGVybmFsQ3JlYXRlTmV3QnVmZmVyUmVxdWVzdCgpXHJcbiAgICB7XHJcbiAgICAgICAgaWYgKHRoaXMuTmV3QnVmZmVyUmVxdWVzdCgpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5OZXdCdWZmZXJSZXF1ZXN0KCkuUHJvcGVydHlDaGFuZ2VkLnVuc3Vic2NyaWJlKHRoaXMuT25CdWZmZXJSZXF1ZXN0UHJvcGVydHlDaGFuZ2VkKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5OZXdCdWZmZXJSZXF1ZXN0KHRoaXMuQ3JlYXRlTmV3QnVmZmVyUmVxdWVzdCgpKVxyXG4gICAgICAgIHRoaXMuTmV3QnVmZmVyUmVxdWVzdCgpLlByb3BlcnR5Q2hhbmdlZC5zdWJzY3JpYmUodGhpcy5PbkJ1ZmZlclJlcXVlc3RQcm9wZXJ0eUNoYW5nZWQpXHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBPbkJ1ZmZlclJlcXVlc3RQcm9wZXJ0eUNoYW5nZWQgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgfVxyXG5cclxuICAgIC8vIGNvbW1hbmRzXHJcbiAgICBwdWJsaWMgQWRkQnVmZmVyUmVxdWVzdCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgbGV0IG5ld0J1ZmZlclJlcXVlc3Q6IEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWxcclxuXHJcbiAgICAgICAgaWYgKCF0aGlzLkhhc0Vycm9yKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkJ1ZmZlclJlcXVlc3RTZXQucHVzaCh0aGlzLk5ld0J1ZmZlclJlcXVlc3QoKSlcclxuICAgICAgICAgICAgdGhpcy5JbnRlcm5hbENyZWF0ZU5ld0J1ZmZlclJlcXVlc3QoKVxyXG4gICAgICAgICAgICB0aGlzLkludGVybmFsVXBkYXRlKClcclxuICAgICAgICAgICAgdGhpcy5fb25CdWZmZXJSZXF1ZXN0U2V0Q2hhbmdlZC5kaXNwYXRjaCh0aGlzLCB0aGlzLkJ1ZmZlclJlcXVlc3RTZXQoKSlcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIERlbGV0ZUJ1ZmZlclJlcXVlc3QgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuQnVmZmVyUmVxdWVzdFNldC5wb3AoKVxyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgICAgIHRoaXMuX29uQnVmZmVyUmVxdWVzdFNldENoYW5nZWQuZGlzcGF0Y2godGhpcywgdGhpcy5CdWZmZXJSZXF1ZXN0U2V0KCkpXHJcbiAgICB9XHJcbn0iLCJjbGFzcyBCdWZmZXJSZXF1ZXN0Vmlld01vZGVsXHJcbntcclxuICAgIHB1YmxpYyBTYW1wbGVSYXRlOiBLbm9ja291dE9ic2VydmFibGU8U2FtcGxlUmF0ZUVudW0+XHJcbiAgICBwdWJsaWMgR3JvdXBGaWx0ZXI6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgRXJyb3JNZXNzYWdlOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIEhhc0Vycm9yOiBLbm9ja291dENvbXB1dGVkPGJvb2xlYW4+XHJcblxyXG4gICAgcHVibGljIFNhbXBsZVJhdGVTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PFNhbXBsZVJhdGVFbnVtPlxyXG5cclxuICAgIHByaXZhdGUgX29uUHJvcGVydHlDaGFuZ2VkOiBFdmVudERpc3BhdGNoZXI8QnVmZmVyUmVxdWVzdFZpZXdNb2RlbCwgYW55PlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKG1vZGVsOiBCdWZmZXJSZXF1ZXN0TW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5TYW1wbGVSYXRlU2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PFNhbXBsZVJhdGVFbnVtPihFbnVtZXJhdGlvbkhlbHBlci5HZXRFbnVtVmFsdWVzKFwiU2FtcGxlUmF0ZUVudW1cIikpXHJcbiAgICAgICAgdGhpcy5TYW1wbGVSYXRlID0ga28ub2JzZXJ2YWJsZTxTYW1wbGVSYXRlRW51bT4obW9kZWwuU2FtcGxlUmF0ZSk7XHJcbiAgICAgICAgdGhpcy5Hcm91cEZpbHRlciA9IGtvLm9ic2VydmFibGU8c3RyaW5nPihtb2RlbC5Hcm91cEZpbHRlcik7XHJcblxyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KFwiXCIpXHJcbiAgICAgICAgdGhpcy5IYXNFcnJvciA9IGtvLmNvbXB1dGVkPGJvb2xlYW4+KCgpID0+IHRoaXMuRXJyb3JNZXNzYWdlKCkubGVuZ3RoID4gMClcclxuXHJcbiAgICAgICAgdGhpcy5fb25Qcm9wZXJ0eUNoYW5nZWQgPSBuZXcgRXZlbnREaXNwYXRjaGVyPEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWwsIGFueT4oKTtcclxuXHJcbiAgICAgICAgdGhpcy5TYW1wbGVSYXRlLnN1YnNjcmliZShuZXdWYWx1ZSA9PiB0aGlzLk9uUHJvcGVydHlDaGFuZ2VkKCkpXHJcbiAgICAgICAgdGhpcy5Hcm91cEZpbHRlci5zdWJzY3JpYmUobmV3VmFsdWUgPT4gdGhpcy5PblByb3BlcnR5Q2hhbmdlZCgpKVxyXG4gICAgfVxyXG5cclxuICAgIGdldCBQcm9wZXJ0eUNoYW5nZWQoKTogSUV2ZW50PEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWwsIGFueT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5fb25Qcm9wZXJ0eUNoYW5nZWQ7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIE9uUHJvcGVydHlDaGFuZ2VkID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLlZhbGlkYXRlKClcclxuICAgICAgICB0aGlzLl9vblByb3BlcnR5Q2hhbmdlZC5kaXNwYXRjaCh0aGlzLCBudWxsKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBWYWxpZGF0ZSgpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IHJlc3VsdDogYW55XHJcblxyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiXCIpXHJcblxyXG4gICAgICAgIHRoaXMuR3JvdXBGaWx0ZXIoKS5zcGxpdChcIjtcIikuZm9yRWFjaChncm91cEZpbHRlciA9PlxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgcmVzdWx0ID0gdGhpcy5DaGVja0dyb3VwRmlsdGVyKGdyb3VwRmlsdGVyKVxyXG5cclxuICAgICAgICAgICAgaWYgKHJlc3VsdC5IYXNFcnJvcilcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UocmVzdWx0LkVycm9yRGVzY3JpcHRpb24pXHJcblxyXG4gICAgICAgICAgICAgICAgcmV0dXJuXHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9KVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb1N0cmluZygpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIFwic2FtcGxlIHJhdGU6IFwiICsgRW51bWVyYXRpb25IZWxwZXIuR2V0RW51bUxvY2FsaXphdGlvbihcIlNhbXBsZVJhdGVFbnVtXCIsIHRoaXMuU2FtcGxlUmF0ZSgpKSArIFwiIC8gZ3JvdXAgZmlsdGVyOiAnXCIgKyB0aGlzLkdyb3VwRmlsdGVyKCkgKyBcIidcIlxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb01vZGVsKClcclxuICAgIHtcclxuICAgICAgICBsZXQgbW9kZWw6IGFueSA9IHtcclxuICAgICAgICAgICAgU2FtcGxlUmF0ZTogPFNhbXBsZVJhdGVFbnVtPnRoaXMuU2FtcGxlUmF0ZSgpLFxyXG4gICAgICAgICAgICBHcm91cEZpbHRlcjogPHN0cmluZz50aGlzLkdyb3VwRmlsdGVyKClcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJldHVybiBtb2RlbFxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgQ2hlY2tHcm91cEZpbHRlcih2YWx1ZTogc3RyaW5nKVxyXG4gICAge1xyXG4gICAgICAgIHZhciByZWdFeHA6IGFueVxyXG5cclxuICAgICAgICBpZiAodmFsdWUubGVuZ3RoID09PSAwKSB7XHJcbiAgICAgICAgICAgIHJldHVybiB7IEhhc0Vycm9yOiB0cnVlLCBFcnJvckRlc2NyaXB0aW9uOiBFcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0dyb3VwRmlsdGVyRW1wdHlcIl0gfVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmVnRXhwID0gbmV3IFJlZ0V4cChcIlteQS1aYS16MC05XyEqXVwiKVxyXG5cclxuICAgICAgICBpZiAocmVnRXhwLnRlc3QodmFsdWUpKSB7XHJcbiAgICAgICAgICAgIHJldHVybiB7IEhhc0Vycm9yOiB0cnVlLCBFcnJvckRlc2NyaXB0aW9uOiBFcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0ludmFsaWRDaGFyYWN0ZXJzXCJdIH1cclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJlZ0V4cCA9IG5ldyBSZWdFeHAoXCJeWzAtOV9dXCIpXHJcblxyXG4gICAgICAgIGlmIChyZWdFeHAudGVzdCh2YWx1ZSkpIHtcclxuICAgICAgICAgICAgcmV0dXJuIHsgSGFzRXJyb3I6IHRydWUsIEVycm9yRGVzY3JpcHRpb246IEVycm9yTWVzc2FnZVtcIlByb2plY3RfSW52YWxpZExlYWRpbmdDaGFyYWN0ZXJcIl0gfVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmVnRXhwID0gbmV3IFJlZ0V4cChcIl4hXCIpXHJcblxyXG4gICAgICAgIGlmIChyZWdFeHAudGVzdCh2YWx1ZSkgJiYgdmFsdWUubGVuZ3RoID09PSAxKSB7XHJcbiAgICAgICAgICAgIHJldHVybiB7IEhhc0Vycm9yOiB0cnVlLCBFcnJvckRlc2NyaXB0aW9uOiBFcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0RldGFjaGVkRXhjbGFtYXRpb25NYXJrTm90QWxsb3dlZFwiXSB9XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4ge1xyXG4gICAgICAgICAgICBIYXNFcnJvcjogZmFsc2UsXHJcbiAgICAgICAgICAgIEVycm9yRGVzY3JpcHRpb246IFwiXCJcclxuICAgICAgICB9XHJcbiAgICB9XHJcbn0iLCJjbGFzcyBEYXRhUG9ydFZpZXdNb2RlbFxyXG57XHJcbiAgICAvLyBmaWVsZHNcclxuICAgIHB1YmxpYyBOYW1lOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IERhdGFUeXBlOiBPbmVEYXNEYXRhVHlwZUVudW1cclxuICAgIHB1YmxpYyByZWFkb25seSBEYXRhRGlyZWN0aW9uOiBEYXRhRGlyZWN0aW9uRW51bVxyXG4gICAgcHVibGljIHJlYWRvbmx5IEVuZGlhbm5lc3M6IEVuZGlhbm5lc3NFbnVtXHJcblxyXG4gICAgcHVibGljIElzU2VsZWN0ZWQ6IEtub2Nrb3V0T2JzZXJ2YWJsZTxib29sZWFuPlxyXG4gICAgcHVibGljIEFzc29jaWF0ZWRDaGFubmVsSHViU2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxDaGFubmVsSHViVmlld01vZGVsPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IEFzc29jaWF0ZWREYXRhR2F0ZXdheTogRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgTGl2ZURlc2NyaXB0aW9uOiBLbm9ja291dENvbXB1dGVkPHN0cmluZz5cclxuXHJcbiAgICAvLyBjb25zdHJ1Y3RvcnNcclxuICAgIGNvbnN0cnVjdG9yKGRhdGFQb3J0TW9kZWw6IGFueSwgYXNzb2NpYXRlZERhdGFHYXRld2F5OiBEYXRhR2F0ZXdheVZpZXdNb2RlbEJhc2UpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5OYW1lID0ga28ub2JzZXJ2YWJsZShkYXRhUG9ydE1vZGVsLk5hbWUpXHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZSA9IGRhdGFQb3J0TW9kZWwuRGF0YVR5cGVcclxuICAgICAgICB0aGlzLkRhdGFEaXJlY3Rpb24gPSBkYXRhUG9ydE1vZGVsLkRhdGFEaXJlY3Rpb25cclxuICAgICAgICB0aGlzLkVuZGlhbm5lc3MgPSBkYXRhUG9ydE1vZGVsLkVuZGlhbm5lc3NcclxuXHJcbiAgICAgICAgdGhpcy5Jc1NlbGVjdGVkID0ga28ub2JzZXJ2YWJsZTxib29sZWFuPihmYWxzZSlcclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWRDaGFubmVsSHViU2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PENoYW5uZWxIdWJWaWV3TW9kZWw+KClcclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhR2F0ZXdheSA9IGFzc29jaWF0ZWREYXRhR2F0ZXdheVxyXG5cclxuICAgICAgICB0aGlzLkxpdmVEZXNjcmlwdGlvbiA9IGtvLmNvbXB1dGVkKCgpID0+XHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBsZXQgcmVzdWx0OiBzdHJpbmdcclxuXHJcbiAgICAgICAgICAgIHJlc3VsdCA9IFwiPGRpdiBjbGFzcz0ndGV4dC1sZWZ0Jz5cIiArIHRoaXMuTmFtZSgpICsgXCI8L2Rpdj48ZGl2IGNsYXNzPSd0ZXh0LWxlZnQnPlwiICsgRW51bWVyYXRpb25IZWxwZXIuR2V0RW51bUxvY2FsaXphdGlvbignT25lRGFzRGF0YVR5cGVFbnVtJywgdGhpcy5EYXRhVHlwZSkgKyBcIjwvZGl2PlwiXHJcblxyXG4gICAgICAgICAgICBpZiAodGhpcy5Bc3NvY2lhdGVkQ2hhbm5lbEh1YlNldCgpLmxlbmd0aCA+IDApXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZENoYW5uZWxIdWJTZXQoKS5mb3JFYWNoKGNoYW5uZWxIdWIgPT5cclxuICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICByZXN1bHQgKz0gXCI8L2JyID48ZGl2IGNsYXNzPSd0ZXh0LWxlZnQnPlwiICsgY2hhbm5lbEh1Yi5OYW1lKCkgKyBcIjwvZGl2PjxkaXYgY2xhc3M9J3RleHQtbGVmdCc+XCIgKyBFbnVtZXJhdGlvbkhlbHBlci5HZXRFbnVtTG9jYWxpemF0aW9uKCdPbmVEYXNEYXRhVHlwZUVudW0nLCBjaGFubmVsSHViLkRhdGFUeXBlKCkpICsgXCI8L2Rpdj5cIlxyXG4gICAgICAgICAgICAgICAgfSlcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgcmV0dXJuIHJlc3VsdFxyXG4gICAgICAgIH0pXHJcbiAgICB9XHJcblxyXG4gICAgLy8gbWV0aG9kc1xyXG4gICAgcHVibGljIEdldElkKCk6IHN0cmluZ1xyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLk5hbWUoKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb0Z1bGxRdWFsaWZpZWRJZGVudGlmaWVyKCk6IHN0cmluZ1xyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLkFzc29jaWF0ZWREYXRhR2F0ZXdheS5EZXNjcmlwdGlvbi5JZCArIFwiIChcIiArIHRoaXMuQXNzb2NpYXRlZERhdGFHYXRld2F5LkRlc2NyaXB0aW9uLkluc3RhbmNlSWQgKyBcIikgLyBcIiArIHRoaXMuR2V0SWQoKTtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgRXh0ZW5kTW9kZWwobW9kZWw6IGFueSlcclxuICAgIHtcclxuICAgICAgICAvL1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb01vZGVsKClcclxuICAgIHtcclxuICAgICAgICBsZXQgbW9kZWw6IGFueSA9IHtcclxuICAgICAgICAgICAgTmFtZTogPHN0cmluZz50aGlzLk5hbWUoKSxcclxuICAgICAgICAgICAgRGF0YVR5cGU6IDxPbmVEYXNEYXRhVHlwZUVudW0+dGhpcy5EYXRhVHlwZSxcclxuICAgICAgICAgICAgRGF0YURpcmVjdGlvbjogPERhdGFEaXJlY3Rpb25FbnVtPnRoaXMuRGF0YURpcmVjdGlvblxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5FeHRlbmRNb2RlbChtb2RlbClcclxuXHJcbiAgICAgICAgcmV0dXJuIG1vZGVsXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFJlc2V0QXNzb2NpYXRpb25zKG1haW50YWluV2Vha1JlZmVyZW5jZTogYm9vbGVhbilcclxuICAgIHtcclxuICAgICAgICBpZiAodGhpcy5Bc3NvY2lhdGVkQ2hhbm5lbEh1YlNldCgpLmxlbmd0aCA+IDApXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWRDaGFubmVsSHViU2V0KCkuZm9yRWFjaChjaGFubmVsSHViID0+IGNoYW5uZWxIdWIuUmVzZXRBc3NvY2lhdGlvbihtYWludGFpbldlYWtSZWZlcmVuY2UsIHRoaXMpKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxufSIsImFic3RyYWN0IGNsYXNzIFBsdWdpblZpZXdNb2RlbEJhc2Vcclxue1xyXG4gICAgcHVibGljIERlc2NyaXB0aW9uOiBQbHVnaW5EZXNjcmlwdGlvblZpZXdNb2RlbFxyXG4gICAgcHVibGljIFBsdWdpbklkZW50aWZpY2F0aW9uOiBQbHVnaW5JZGVudGlmaWNhdGlvblZpZXdNb2RlbFxyXG4gICAgcHVibGljIElzSW5TZXR0aW5nc01vZGU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxib29sZWFuPlxyXG5cclxuICAgIHByaXZhdGUgX21vZGVsOiBhbnlcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihwbHVnaW5TZXR0aW5nc01vZGVsOiBhbnksIHBsdWdpbklkZW50aWZpY2F0aW9uOiBQbHVnaW5JZGVudGlmaWNhdGlvblZpZXdNb2RlbClcclxuICAgIHtcclxuICAgICAgICB0aGlzLl9tb2RlbCA9IHBsdWdpblNldHRpbmdzTW9kZWxcclxuICAgICAgICB0aGlzLkRlc2NyaXB0aW9uID0gbmV3IFBsdWdpbkRlc2NyaXB0aW9uVmlld01vZGVsKHBsdWdpblNldHRpbmdzTW9kZWwuRGVzY3JpcHRpb24pXHJcbiAgICAgICAgdGhpcy5QbHVnaW5JZGVudGlmaWNhdGlvbiA9IHBsdWdpbklkZW50aWZpY2F0aW9uXHJcbiAgICAgICAgdGhpcy5Jc0luU2V0dGluZ3NNb2RlID0ga28ub2JzZXJ2YWJsZTxib29sZWFuPihmYWxzZSlcclxuICAgIH1cclxuXHJcbiAgICAvLyBtZXRob2RzXHJcbiAgICBwdWJsaWMgYWJzdHJhY3QgYXN5bmMgSW5pdGlhbGl6ZUFzeW5jKCk6IFByb21pc2U8YW55PlxyXG5cclxuICAgIHB1YmxpYyBTZW5kQWN0aW9uUmVxdWVzdCA9IGFzeW5jIChpbnN0YW5jZUlkOiBudW1iZXIsIG1ldGhvZE5hbWU6IHN0cmluZywgZGF0YTogYW55KSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiA8QWN0aW9uUmVzcG9uc2U+IGF3YWl0IENvbm5lY3Rpb25NYW5hZ2VyLkludm9rZVdlYkNsaWVudEh1YihcIlJlcXVlc3RBY3Rpb25cIiwgbmV3IEFjdGlvblJlcXVlc3QodGhpcy5EZXNjcmlwdGlvbi5JZCwgaW5zdGFuY2VJZCwgbWV0aG9kTmFtZSwgZGF0YSkpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEV4dGVuZE1vZGVsKG1vZGVsOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgLy9cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IG1vZGVsOiBhbnkgPSB7XHJcbiAgICAgICAgICAgICR0eXBlOiA8c3RyaW5nPnRoaXMuX21vZGVsLiR0eXBlLFxyXG4gICAgICAgICAgICBEZXNjcmlwdGlvbjogPFBsdWdpbkRlc2NyaXB0aW9uVmlld01vZGVsPnRoaXMuRGVzY3JpcHRpb24uVG9Nb2RlbCgpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0aGlzLkV4dGVuZE1vZGVsKG1vZGVsKVxyXG5cclxuICAgICAgICByZXR1cm4gbW9kZWxcclxuICAgIH1cclxuXHJcbiAgICAvLyBjb21tYW5kc1xyXG4gICAgcHVibGljIEVuYWJsZVNldHRpbmdzTW9kZSA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5Jc0luU2V0dGluZ3NNb2RlKHRydWUpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIERpc2FibGVTZXR0aW5nc01vZGUgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuSXNJblNldHRpbmdzTW9kZShmYWxzZSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9nZ2xlU2V0dGluZ3NNb2RlID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLklzSW5TZXR0aW5nc01vZGUoIXRoaXMuSXNJblNldHRpbmdzTW9kZSgpKVxyXG4gICAgfVxyXG59IiwiLy8vIDxyZWZlcmVuY2UgcGF0aD1cIlBsdWdpblZpZXdNb2RlbEJhc2UudHNcIi8+XHJcblxyXG5hYnN0cmFjdCBjbGFzcyBEYXRhR2F0ZXdheVZpZXdNb2RlbEJhc2UgZXh0ZW5kcyBQbHVnaW5WaWV3TW9kZWxCYXNlXHJcbntcclxuICAgIHB1YmxpYyByZWFkb25seSBNYXhpbXVtRGF0YXNldEFnZTogS25vY2tvdXRPYnNlcnZhYmxlPG51bWJlcj5cclxuICAgIHB1YmxpYyByZWFkb25seSBEYXRhUG9ydFNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8RGF0YVBvcnRWaWV3TW9kZWw+XHJcblxyXG4gICAgY29uc3RydWN0b3IobW9kZWwsIGlkZW50aWZpY2F0aW9uOiBQbHVnaW5JZGVudGlmaWNhdGlvblZpZXdNb2RlbClcclxuICAgIHtcclxuICAgICAgICBzdXBlcihtb2RlbCwgaWRlbnRpZmljYXRpb24pXHJcblxyXG4gICAgICAgIHRoaXMuTWF4aW11bURhdGFzZXRBZ2UgPSBrby5vYnNlcnZhYmxlPG51bWJlcj4obW9kZWwuTWF4aW11bURhdGFzZXRBZ2UpXHJcbiAgICAgICAgdGhpcy5EYXRhUG9ydFNldCA9IGtvLm9ic2VydmFibGVBcnJheTxEYXRhUG9ydFZpZXdNb2RlbD4oKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBFeHRlbmRNb2RlbChtb2RlbDogYW55KVxyXG4gICAge1xyXG4gICAgICAgIHN1cGVyLkV4dGVuZE1vZGVsKG1vZGVsKVxyXG5cclxuICAgICAgICBtb2RlbC5NYXhpbXVtRGF0YXNldEFnZSA9IDxudW1iZXI+TnVtYmVyLnBhcnNlSW50KDxhbnk+dGhpcy5NYXhpbXVtRGF0YXNldEFnZSgpKVxyXG4gICAgfVxyXG59IiwiYWJzdHJhY3QgY2xhc3MgRXh0ZW5kZWREYXRhR2F0ZXdheVZpZXdNb2RlbEJhc2UgZXh0ZW5kcyBEYXRhR2F0ZXdheVZpZXdNb2RlbEJhc2Vcclxue1xyXG4gICAgcHVibGljIE1vZHVsZVRvRGF0YVBvcnRNYXA6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PE9ic2VydmFibGVHcm91cDxEYXRhUG9ydFZpZXdNb2RlbD4+XHJcbiAgICBwdWJsaWMgT25lRGFzTW9kdWxlU2VsZWN0b3I6IEtub2Nrb3V0T2JzZXJ2YWJsZTxPbmVEYXNNb2R1bGVTZWxlY3RvclZpZXdNb2RlbD5cclxuXHJcbiAgICBjb25zdHJ1Y3Rvcihtb2RlbCwgaWRlbnRpZmljYXRpb246IFBsdWdpbklkZW50aWZpY2F0aW9uVmlld01vZGVsLCBvbmVEYXNNb2R1bGVTZWxlY3RvcjogT25lRGFzTW9kdWxlU2VsZWN0b3JWaWV3TW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgc3VwZXIobW9kZWwsIGlkZW50aWZpY2F0aW9uKVxyXG5cclxuICAgICAgICB0aGlzLk1vZHVsZVRvRGF0YVBvcnRNYXAgPSBrby5vYnNlcnZhYmxlQXJyYXkoKVxyXG4gICAgICAgIHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3IgPSBrby5vYnNlcnZhYmxlPE9uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsPihvbmVEYXNNb2R1bGVTZWxlY3RvcilcclxuXHJcbiAgICAgICAgaWYgKHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3IoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3IoKS5Pbk1vZHVsZVNldENoYW5nZWQuc3Vic2NyaWJlKChzZW5kZXIsIGFyZ3MpID0+XHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIHRoaXMuVXBkYXRlRGF0YVBvcnRTZXQoKVxyXG4gICAgICAgICAgICB9KVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgYXN5bmMgSW5pdGlhbGl6ZUFzeW5jKClcclxuICAgIHtcclxuICAgICAgICB0aGlzLlVwZGF0ZURhdGFQb3J0U2V0KClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVXBkYXRlRGF0YVBvcnRTZXQoKVxyXG4gICAge1xyXG4gICAgICAgIGxldCBpbmRleDogbnVtYmVyXHJcbiAgICAgICAgbGV0IG1vZHVsZVRvRGF0YVBvcnRNYXA6IE9ic2VydmFibGVHcm91cDxEYXRhUG9ydFZpZXdNb2RlbD5bXVxyXG5cclxuICAgICAgICBtb2R1bGVUb0RhdGFQb3J0TWFwID0gW11cclxuXHJcbiAgICAgICAgLy8gaW5wdXRzXHJcbiAgICAgICAgaW5kZXggPSAwXHJcblxyXG4gICAgICAgIG1vZHVsZVRvRGF0YVBvcnRNYXAgPSBtb2R1bGVUb0RhdGFQb3J0TWFwLmNvbmNhdCh0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yKCkuTW9kdWxlU2V0KCkuZmlsdGVyKG9uZURhc01vZHVsZSA9PiBvbmVEYXNNb2R1bGUuRGF0YURpcmVjdGlvbigpID09PSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dCkubWFwKG9uZURhc01vZHVsZSA9PlxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgbGV0IGdyb3VwOiBPYnNlcnZhYmxlR3JvdXA8RGF0YVBvcnRWaWV3TW9kZWw+XHJcblxyXG4gICAgICAgICAgICBncm91cCA9IG5ldyBPYnNlcnZhYmxlR3JvdXA8RGF0YVBvcnRWaWV3TW9kZWw+KG9uZURhc01vZHVsZS5Ub1N0cmluZygpLCB0aGlzLkNyZWF0ZURhdGFQb3J0U2V0KG9uZURhc01vZHVsZSwgaW5kZXgpKVxyXG4gICAgICAgICAgICBpbmRleCArPSBvbmVEYXNNb2R1bGUuU2l6ZSgpO1xyXG5cclxuICAgICAgICAgICAgcmV0dXJuIGdyb3VwXHJcbiAgICAgICAgfSkpXHJcblxyXG4gICAgICAgIC8vIG91dHB1dHNcclxuICAgICAgICBpbmRleCA9IDBcclxuXHJcbiAgICAgICAgbW9kdWxlVG9EYXRhUG9ydE1hcCA9IG1vZHVsZVRvRGF0YVBvcnRNYXAuY29uY2F0KHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3IoKS5Nb2R1bGVTZXQoKS5maWx0ZXIob25lRGFzTW9kdWxlID0+IG9uZURhc01vZHVsZS5EYXRhRGlyZWN0aW9uKCkgPT09IERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dCkubWFwKG9uZURhc01vZHVsZSA9PlxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgbGV0IGdyb3VwOiBPYnNlcnZhYmxlR3JvdXA8RGF0YVBvcnRWaWV3TW9kZWw+XHJcblxyXG4gICAgICAgICAgICBncm91cCA9IG5ldyBPYnNlcnZhYmxlR3JvdXA8RGF0YVBvcnRWaWV3TW9kZWw+KG9uZURhc01vZHVsZS5Ub1N0cmluZygpLCB0aGlzLkNyZWF0ZURhdGFQb3J0U2V0KG9uZURhc01vZHVsZSwgaW5kZXgpKVxyXG4gICAgICAgICAgICBpbmRleCArPSBvbmVEYXNNb2R1bGUuU2l6ZSgpO1xyXG5cclxuICAgICAgICAgICAgcmV0dXJuIGdyb3VwXHJcbiAgICAgICAgfSkpXHJcblxyXG4gICAgICAgIHRoaXMuTW9kdWxlVG9EYXRhUG9ydE1hcChtb2R1bGVUb0RhdGFQb3J0TWFwKVxyXG4gICAgICAgIHRoaXMuRGF0YVBvcnRTZXQoTWFwTWFueSh0aGlzLk1vZHVsZVRvRGF0YVBvcnRNYXAoKSwgZ3JvdXAgPT4gZ3JvdXAuTWVtYmVycygpKSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgQ3JlYXRlRGF0YVBvcnRTZXQob25lRGFzTW9kdWxlOiBPbmVEYXNNb2R1bGVWaWV3TW9kZWwsIGluZGV4OiBudW1iZXIpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IHByZWZpeDogc3RyaW5nXHJcblxyXG4gICAgICAgIHN3aXRjaCAob25lRGFzTW9kdWxlLkRhdGFEaXJlY3Rpb24oKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uSW5wdXQ6XHJcbiAgICAgICAgICAgICAgICBwcmVmaXggPSBcIklucHV0XCJcclxuICAgICAgICAgICAgICAgIGJyZWFrXHJcblxyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dDpcclxuICAgICAgICAgICAgICAgIHByZWZpeCA9IFwiT3V0cHV0XCJcclxuICAgICAgICAgICAgICAgIGJyZWFrXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gQXJyYXkuZnJvbShuZXcgQXJyYXkob25lRGFzTW9kdWxlLlNpemUoKSksICh4LCBpKSA9PiBcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHJldHVybiB7XHJcbiAgICAgICAgICAgICAgICBOYW1lOiA8c3RyaW5nPnByZWZpeCArIFwiIFwiICsgKGluZGV4ICsgaSksXHJcbiAgICAgICAgICAgICAgICBEYXRhVHlwZTogPE9uZURhc0RhdGFUeXBlRW51bT5vbmVEYXNNb2R1bGUuRGF0YVR5cGUoKSxcclxuICAgICAgICAgICAgICAgIERhdGFEaXJlY3Rpb246IDxEYXRhRGlyZWN0aW9uRW51bT5vbmVEYXNNb2R1bGUuRGF0YURpcmVjdGlvbigpXHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9KS5tYXAoZGF0YVBvcnRNb2RlbCA9PiBuZXcgRGF0YVBvcnRWaWV3TW9kZWwoZGF0YVBvcnRNb2RlbCwgdGhpcykpXHJcbiAgICB9XHJcbn0iLCIvLy8gPHJlZmVyZW5jZSBwYXRoPVwiUGx1Z2luVmlld01vZGVsQmFzZS50c1wiLz5cclxuXHJcbmFic3RyYWN0IGNsYXNzIERhdGFXcml0ZXJWaWV3TW9kZWxCYXNlIGV4dGVuZHMgUGx1Z2luVmlld01vZGVsQmFzZVxyXG57XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRmlsZUdyYW51bGFyaXR5OiBLbm9ja291dE9ic2VydmFibGU8RmlsZUdyYW51bGFyaXR5RW51bT5cclxuICAgIHB1YmxpYyByZWFkb25seSBCdWZmZXJSZXF1ZXN0U2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxCdWZmZXJSZXF1ZXN0Vmlld01vZGVsPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IEJ1ZmZlclJlcXVlc3RTZWxlY3RvcjogS25vY2tvdXRPYnNlcnZhYmxlPEJ1ZmZlclJlcXVlc3RTZWxlY3RvclZpZXdNb2RlbD5cclxuXHJcbiAgICBjb25zdHJ1Y3Rvcihtb2RlbCwgaWRlbnRpZmljYXRpb246IFBsdWdpbklkZW50aWZpY2F0aW9uVmlld01vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHN1cGVyKG1vZGVsLCBpZGVudGlmaWNhdGlvbilcclxuXHJcbiAgICAgICAgdGhpcy5GaWxlR3JhbnVsYXJpdHkgPSBrby5vYnNlcnZhYmxlPEZpbGVHcmFudWxhcml0eUVudW0+KG1vZGVsLkZpbGVHcmFudWxhcml0eSlcclxuICAgICAgICB0aGlzLkJ1ZmZlclJlcXVlc3RTZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8QnVmZmVyUmVxdWVzdFZpZXdNb2RlbD4obW9kZWwuQnVmZmVyUmVxdWVzdFNldC5tYXAoYnVmZmVyUmVxdWVzdCA9PiBuZXcgQnVmZmVyUmVxdWVzdFZpZXdNb2RlbChidWZmZXJSZXF1ZXN0KSkpXHJcblxyXG4gICAgICAgIHRoaXMuQnVmZmVyUmVxdWVzdFNlbGVjdG9yID0ga28ub2JzZXJ2YWJsZTxCdWZmZXJSZXF1ZXN0U2VsZWN0b3JWaWV3TW9kZWw+KG5ldyBCdWZmZXJSZXF1ZXN0U2VsZWN0b3JWaWV3TW9kZWwodGhpcy5CdWZmZXJSZXF1ZXN0U2V0KCkpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBFeHRlbmRNb2RlbChtb2RlbDogYW55KVxyXG4gICAge1xyXG4gICAgICAgIHN1cGVyLkV4dGVuZE1vZGVsKG1vZGVsKVxyXG5cclxuICAgICAgICBtb2RlbC5GaWxlR3JhbnVsYXJpdHkgPSA8RmlsZUdyYW51bGFyaXR5RW51bT50aGlzLkZpbGVHcmFudWxhcml0eSgpXHJcbiAgICAgICAgbW9kZWwuQnVmZmVyUmVxdWVzdFNldCA9IDxCdWZmZXJSZXF1ZXN0TW9kZWxbXT50aGlzLkJ1ZmZlclJlcXVlc3RTZXQoKS5tYXAoYnVmZmVyUmVxdWVzdCA9PiBidWZmZXJSZXF1ZXN0LlRvTW9kZWwoKSlcclxuICAgIH1cclxufSIsImNsYXNzIFBsdWdpbkRlc2NyaXB0aW9uVmlld01vZGVsXHJcbntcclxuICAgIHB1YmxpYyBQcm9kdWN0VmVyc2lvbjogbnVtYmVyXHJcbiAgICBwdWJsaWMgSWQ6IHN0cmluZ1xyXG4gICAgcHVibGljIEluc3RhbmNlSWQ6IG51bWJlclxyXG4gICAgcHVibGljIEluc3RhbmNlTmFtZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBJc0VuYWJsZWQ6IEtub2Nrb3V0T2JzZXJ2YWJsZTxib29sZWFuPlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKHBsdWdpbkRlc2NyaXB0aW9uTW9kZWw6IGFueSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLlByb2R1Y3RWZXJzaW9uID0gcGx1Z2luRGVzY3JpcHRpb25Nb2RlbC5Qcm9kdWN0VmVyc2lvblxyXG4gICAgICAgIHRoaXMuSWQgPSBwbHVnaW5EZXNjcmlwdGlvbk1vZGVsLklkXHJcbiAgICAgICAgdGhpcy5JbnN0YW5jZUlkID0gcGx1Z2luRGVzY3JpcHRpb25Nb2RlbC5JbnN0YW5jZUlkXHJcbiAgICAgICAgdGhpcy5JbnN0YW5jZU5hbWUgPSBrby5vYnNlcnZhYmxlPHN0cmluZz4ocGx1Z2luRGVzY3JpcHRpb25Nb2RlbC5JbnN0YW5jZU5hbWUpXHJcbiAgICAgICAgdGhpcy5Jc0VuYWJsZWQgPSBrby5vYnNlcnZhYmxlPGJvb2xlYW4+KHBsdWdpbkRlc2NyaXB0aW9uTW9kZWwuSXNFbmFibGVkKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb01vZGVsKClcclxuICAgIHtcclxuICAgICAgICB2YXIgbW9kZWw6IGFueSA9IHtcclxuICAgICAgICAgICAgUHJvZHVjdFZlcnNpb246IDxudW1iZXI+dGhpcy5Qcm9kdWN0VmVyc2lvbixcclxuICAgICAgICAgICAgSWQ6IDxzdHJpbmc+dGhpcy5JZCxcclxuICAgICAgICAgICAgSW5zdGFuY2VJZDogPG51bWJlcj50aGlzLkluc3RhbmNlSWQsXHJcbiAgICAgICAgICAgIEluc3RhbmNlTmFtZTogPHN0cmluZz50aGlzLkluc3RhbmNlTmFtZSgpLFxyXG4gICAgICAgICAgICBJc0VuYWJsZWQ6IDxib29sZWFuPnRoaXMuSXNFbmFibGVkKClcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJldHVybiBtb2RlbFxyXG4gICAgfVxyXG59IiwiY2xhc3MgUGx1Z2luSWRlbnRpZmljYXRpb25WaWV3TW9kZWxcclxue1xyXG4gICAgcHVibGljIFByb2R1Y3RWZXJzaW9uOiBzdHJpbmdcclxuICAgIHB1YmxpYyBJZDogc3RyaW5nXHJcbiAgICBwdWJsaWMgTmFtZTogc3RyaW5nXHJcbiAgICBwdWJsaWMgRGVzY3JpcHRpb246IHN0cmluZ1xyXG4gICAgcHVibGljIFZpZXdSZXNvdXJjZU5hbWU6IHN0cmluZ1xyXG4gICAgcHVibGljIFZpZXdNb2RlbFJlc291cmNlTmFtZTogc3RyaW5nXHJcblxyXG4gICAgY29uc3RydWN0b3IocGx1Z2luSWRlbnRpZmljYXRpb25Nb2RlbDogYW55KVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuUHJvZHVjdFZlcnNpb24gPSBwbHVnaW5JZGVudGlmaWNhdGlvbk1vZGVsLlByb2R1Y3RWZXJzaW9uXHJcbiAgICAgICAgdGhpcy5JZCA9IHBsdWdpbklkZW50aWZpY2F0aW9uTW9kZWwuSWRcclxuICAgICAgICB0aGlzLk5hbWUgPSBwbHVnaW5JZGVudGlmaWNhdGlvbk1vZGVsLk5hbWVcclxuICAgICAgICB0aGlzLkRlc2NyaXB0aW9uID0gcGx1Z2luSWRlbnRpZmljYXRpb25Nb2RlbC5EZXNjcmlwdGlvblxyXG4gICAgICAgIHRoaXMuVmlld1Jlc291cmNlTmFtZSA9IHBsdWdpbklkZW50aWZpY2F0aW9uTW9kZWwuVmlld1Jlc291cmNlTmFtZVxyXG4gICAgICAgIHRoaXMuVmlld01vZGVsUmVzb3VyY2VOYW1lID0gcGx1Z2luSWRlbnRpZmljYXRpb25Nb2RlbC5WaWV3TW9kZWxSZXNvdXJjZU5hbWVcclxuICAgIH1cclxufSJdfQ==