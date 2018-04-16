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
    constructor(extensionId, instanceId, methodName, data) {
        this.ExtensionId = extensionId;
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
class ExtensionFactory {
}
ExtensionFactory.CreateExtensionViewModelAsync = (extensionType, extensionModel) => __awaiter(this, void 0, void 0, function* () {
    let extensionIdentification;
    let extensionViewModel;
    let extensionViewModelRaw;
    extensionIdentification = ExtensionHive.FindExtensionIdentification(extensionType, extensionModel.Description.Id);
    if (extensionIdentification) {
        extensionViewModelRaw = yield ConnectionManager.InvokeWebClientHub("GetExtensionStringResource", extensionModel.Description.Id, extensionIdentification.ViewModelResourceName);
        extensionViewModel = new Function(extensionViewModelRaw + "; return ViewModelConstructor")()(extensionModel, extensionIdentification);
        return extensionViewModel;
    }
    else {
        throw new Error("No corresponding extension description for extension ID '" + extensionModel.Description.Id + "' found.");
    }
});
class ExtensionHive {
}
// constructors
ExtensionHive.Initialize = () => {
    ExtensionHive.ExtensionIdentificationSet = new Map();
};
ExtensionHive.FindExtensionIdentification = (extensionTypeName, extensionId) => {
    return ExtensionHive.ExtensionIdentificationSet.get(extensionTypeName).find(extensionIdentification => extensionIdentification.Id === extensionId);
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
class ExtensionViewModelBase {
    constructor(extensionSettingsModel, extensionIdentification) {
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
        this._model = extensionSettingsModel;
        this.Description = new ExtensionDescriptionViewModel(extensionSettingsModel.Description);
        this.ExtensionIdentification = extensionIdentification;
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
/// <reference path="ExtensionViewModelBase.ts"/>
class DataGatewayViewModelBase extends ExtensionViewModelBase {
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
/// <reference path="ExtensionViewModelBase.ts"/>
class DataWriterViewModelBase extends ExtensionViewModelBase {
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
class ExtensionDescriptionViewModel {
    constructor(extensionDescriptionModel) {
        this.ProductVersion = extensionDescriptionModel.ProductVersion;
        this.Id = extensionDescriptionModel.Id;
        this.InstanceId = extensionDescriptionModel.InstanceId;
        this.InstanceName = ko.observable(extensionDescriptionModel.InstanceName);
        this.IsEnabled = ko.observable(extensionDescriptionModel.IsEnabled);
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
class ExtensionIdentificationViewModel {
    constructor(extensionIdentificationModel) {
        this.ProductVersion = extensionIdentificationModel.ProductVersion;
        this.Id = extensionIdentificationModel.Id;
        this.Name = extensionIdentificationModel.Name;
        this.Description = extensionIdentificationModel.Description;
        this.ViewResourceName = extensionIdentificationModel.ViewResourceName;
        this.ViewModelResourceName = extensionIdentificationModel.ViewModelResourceName;
    }
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiT25lRGFzLlR5cGVzLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vT25lRGFzLlR5cGVzL0RhdGFEaXJlY3Rpb25FbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL0VuZGlhbm5lc3NFbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL0ZpbGVHcmFudWxhcml0eUVudW0udHMiLCIuLi9PbmVEYXMuVHlwZXMvT25lRGFzRGF0YVR5cGVFbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL09uZURhc1N0YXRlRW51bS50cyIsIi4uL09uZURhcy5UeXBlcy9TYW1wbGVSYXRlRW51bS50cyIsIi4uL0NvcmUvQWN0aW9uUmVxdWVzdC50cyIsIi4uL0NvcmUvQWN0aW9uUmVzcG9uc2UudHMiLCIuLi9Db3JlL0V2ZW50RGlzcGF0Y2hlci50cyIsIi4uL0NvcmUvSUV2ZW50LnRzIiwiLi4vQ29yZS9PbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtLnRzIiwiLi4vTW9kZWxzL0J1ZmZlclJlcXVlc3RNb2RlbC50cyIsIi4uL01vZGVscy9DaGFubmVsSHViTW9kZWwudHMiLCIuLi9Nb2RlbHMvT25lRGFzTW9kdWxlTW9kZWwudHMiLCIuLi9Nb2RlbHMvVHJhbnNmZXJGdW5jdGlvbk1vZGVsLnRzIiwiLi4vU3RhdGljL0Nvbm5lY3Rpb25NYW5hZ2VyLnRzIiwiLi4vU3RhdGljL0VudW1lcmF0aW9uSGVscGVyLnRzIiwiLi4vU3RhdGljL0Vycm9yTWVzc2FnZS50cyIsIi4uL1N0YXRpYy9IZWxwZXIudHMiLCIuLi9TdGF0aWMvRXh0ZW5zaW9uRmFjdG9yeS50cyIsIi4uL1N0YXRpYy9FeHRlbnNpb25IaXZlLnRzIiwiLi4vVmlld01vZGVscy9Db3JlL0NoYW5uZWxIdWJWaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL0NvcmUvT25lRGFzTW9kdWxlVmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9Db3JlL09uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9Db3JlL1RyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL0V4dGVuc2lvbi9CdWZmZXJSZXF1ZXN0U2VsZWN0b3JWaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL0V4dGVuc2lvbi9CdWZmZXJSZXF1ZXN0Vmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9FeHRlbnNpb24vRGF0YVBvcnRWaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL0V4dGVuc2lvbi9FeHRlbnNpb25WaWV3TW9kZWxCYXNlLnRzIiwiLi4vVmlld01vZGVscy9FeHRlbnNpb24vRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlLnRzIiwiLi4vVmlld01vZGVscy9FeHRlbnNpb24vRXh0ZW5kZWREYXRhR2F0ZXdheVZpZXdNb2RlbEJhc2UudHMiLCIuLi9WaWV3TW9kZWxzL0V4dGVuc2lvbi9EYXRhV3JpdGVyVmlld01vZGVsQmFzZS50cyIsIi4uL1ZpZXdNb2RlbHMvRXh0ZW5zaW9uL0V4dGVuc2lvbkRlc2NyaXB0aW9uVmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9FeHRlbnNpb24vRXh0ZW5zaW9uSWRlbnRpZmljYXRpb25WaWV3TW9kZWwudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsSUFBSyxpQkFJSjtBQUpELFdBQUssaUJBQWlCO0lBRWxCLDJEQUFTLENBQUE7SUFDVCw2REFBVSxDQUFBO0FBQ2QsQ0FBQyxFQUpJLGlCQUFpQixLQUFqQixpQkFBaUIsUUFJckI7QUNKRCxJQUFLLGNBSUo7QUFKRCxXQUFLLGNBQWM7SUFFZixtRUFBZ0IsQ0FBQTtJQUNoQiw2REFBYSxDQUFBO0FBQ2pCLENBQUMsRUFKSSxjQUFjLEtBQWQsY0FBYyxRQUlsQjtBQ0pELElBQUssbUJBTUo7QUFORCxXQUFLLG1CQUFtQjtJQUVwQixzRUFBYSxDQUFBO0lBQ2IseUVBQWUsQ0FBQTtJQUNmLGdFQUFXLENBQUE7SUFDWCwrREFBVyxDQUFBO0FBQ2YsQ0FBQyxFQU5JLG1CQUFtQixL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SUFBSSxDQUFBO1FBQzdDLElBQUksQ0FBQyxXQUFXLEdBQUcsNEJBQTRCLENBQUMsV0FBVyxDQUFBO1FBQzNELElBQUksQ0FBQyxnQkFBZ0IsR0FBRyw0QkFBNEIsQ0FBQyxnQkFBZ0IsQ0FBQTtRQUNyRSxJQUFJLENBQUMscUJBQXFCLEdBQUcsNEJBQTRCLENBQUMscUJBQXFCLENBQUE7SUFDbkYsQ0FBQztDQUNKIiwic291cmNlc0NvbnRlbnQiOlsiZW51bSBEYXRhRGlyZWN0aW9uRW51bVxyXG57XHJcbiAgICBJbnB1dCA9IDEsXHJcbiAgICBPdXRwdXQgPSAyXHJcbn0iLCJlbnVtIEVuZGlhbm5lc3NFbnVtXHJcbntcclxuICAgIExpdHRsZUVuZGlhbiA9IDEsXHJcbiAgICBCaWdFbmRpYW4gPSAyXHJcbn0iLCJlbnVtIEZpbGVHcmFudWxhcml0eUVudW1cclxue1xyXG4gICAgTWludXRlXzEgPSA2MCxcclxuICAgIE1pbnV0ZV8xMCA9IDYwMCxcclxuICAgIEhvdXIgPSAzNjAwLFxyXG4gICAgRGF5ID0gODY0MDBcclxufSIsImVudW0gT25lRGFzRGF0YVR5cGVFbnVtXHJcbntcclxuICAgIEJPT0xFQU4gPSAweDAwOCxcclxuICAgIFVJTlQ4ID0gMHgxMDgsXHJcbiAgICBJTlQ4ID0gMHgyMDgsXHJcbiAgICBVSU5UMTYgPSAweDExMCxcclxuICAgIElOVDE2ID0gMHgyMTAsXHJcbiAgICBVSU5UMzIgPSAweDEyMCxcclxuICAgIElOVDMyID0gMHgyMjAsXHJcbiAgICBGTE9BVDMyID0gMHgzMjAsXHJcbiAgICBGTE9BVDY0ID0gMHgzNDBcclxufSIsImVudW0gT25lRGFzU3RhdGVFbnVtXHJcbntcclxuICAgIEVycm9yID0gMSxcclxuICAgIEluaXRpYWxpemF0aW9uID0gMixcclxuICAgIElkbGUgPSAzLFxyXG4gICAgQXBwbHlDb25maWd1cmF0aW9uID0gNCxcclxuICAgIFJlYWR5ID0gNSxcclxuICAgIFJ1biA9IDZcclxufSIsImVudW0gU2FtcGxlUmF0ZUVudW1cclxue1xyXG4gICAgU2FtcGxlUmF0ZV8xMDAgPSAxLFxyXG4gICAgU2FtcGxlUmF0ZV8yNSA9IDQsXHJcbiAgICBTYW1wbGVSYXRlXzUgPSAyMCxcclxuICAgIFNhbXBsZVJhdGVfMSA9IDEwMFxyXG59IiwiY2xhc3MgQWN0aW9uUmVxdWVzdFxyXG57XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRXh0ZW5zaW9uSWQ6IHN0cmluZ1xyXG4gICAgcHVibGljIHJlYWRvbmx5IEluc3RhbmNlSWQ6IG51bWJlclxyXG4gICAgcHVibGljIHJlYWRvbmx5IE1ldGhvZE5hbWU6IHN0cmluZ1xyXG4gICAgcHVibGljIHJlYWRvbmx5IERhdGE6IGFueVxyXG5cclxuICAgIGNvbnN0cnVjdG9yKGV4dGVuc2lvbklkOiBzdHJpbmcsIGluc3RhbmNlSWQ6IG51bWJlciwgbWV0aG9kTmFtZTogc3RyaW5nLCBkYXRhOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5FeHRlbnNpb25JZCA9IGV4dGVuc2lvbklkO1xyXG4gICAgICAgIHRoaXMuSW5zdGFuY2VJZCA9IGluc3RhbmNlSWQ7XHJcbiAgICAgICAgdGhpcy5NZXRob2ROYW1lID0gbWV0aG9kTmFtZTtcclxuICAgICAgICB0aGlzLkRhdGEgPSBkYXRhO1xyXG4gICAgfVxyXG59IiwiY2xhc3MgQWN0aW9uUmVzcG9uc2Vcclxue1xyXG4gICAgcHVibGljIERhdGE6IGFueVxyXG5cclxuICAgIGNvbnN0cnVjdG9yKGRhdGE6IGFueSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLkRhdGEgPSBkYXRhO1xyXG4gICAgfVxyXG59IiwiY2xhc3MgRXZlbnREaXNwYXRjaGVyPFRTZW5kZXIsIFRBcmdzPiBpbXBsZW1lbnRzIElFdmVudDxUU2VuZGVyLCBUQXJncz5cclxue1xyXG4gICAgcHJpdmF0ZSBfc3Vic2NyaXB0aW9uczogQXJyYXk8KHNlbmRlcjogVFNlbmRlciwgYXJnczogVEFyZ3MpID0+IHZvaWQ+ID0gbmV3IEFycmF5PChzZW5kZXI6IFRTZW5kZXIsIGFyZ3M6IFRBcmdzKSA9PiB2b2lkPigpO1xyXG5cclxuICAgIHN1YnNjcmliZShmbjogKHNlbmRlcjogVFNlbmRlciwgYXJnczogVEFyZ3MpID0+IHZvaWQpOiB2b2lkXHJcbiAgICB7XHJcbiAgICAgICAgaWYgKGZuKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5fc3Vic2NyaXB0aW9ucy5wdXNoKGZuKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgdW5zdWJzY3JpYmUoZm46IChzZW5kZXI6IFRTZW5kZXIsIGFyZ3M6IFRBcmdzKSA9PiB2b2lkKTogdm9pZFxyXG4gICAge1xyXG4gICAgICAgIGxldCBpID0gdGhpcy5fc3Vic2NyaXB0aW9ucy5pbmRleE9mKGZuKTtcclxuXHJcbiAgICAgICAgaWYgKGkgPiAtMSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuX3N1YnNjcmlwdGlvbnMuc3BsaWNlKGksIDEpO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBkaXNwYXRjaChzZW5kZXI6IFRTZW5kZXIsIGFyZ3M6IFRBcmdzKTogdm9pZFxyXG4gICAge1xyXG4gICAgICAgIGZvciAobGV0IGhhbmRsZXIgb2YgdGhpcy5fc3Vic2NyaXB0aW9ucylcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGhhbmRsZXIoc2VuZGVyLCBhcmdzKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcbn0iLCJpbnRlcmZhY2UgSUV2ZW50PFRTZW5kZXIsIFRBcmdzPlxyXG57XHJcbiAgICBzdWJzY3JpYmUoZm46IChzZW5kZXI6IFRTZW5kZXIsIGFyZ3M6IFRBcmdzKSA9PiB2b2lkKTogdm9pZDtcclxuICAgIHVuc3Vic2NyaWJlKGZuOiAoc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncykgPT4gdm9pZCk6IHZvaWQ7XHJcbn0iLCJlbnVtIE9uZURhc01vZHVsZVNlbGVjdG9yTW9kZUVudW1cclxue1xyXG4gICAgRHVwbGV4ID0gMSxcclxuICAgIElucHV0T25seSA9IDIsXHJcbiAgICBPdXRwdXRPbmx5ID0gMyxcclxufSIsImNsYXNzIEJ1ZmZlclJlcXVlc3RNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgU2FtcGxlUmF0ZTogU2FtcGxlUmF0ZUVudW1cclxuICAgIHB1YmxpYyBHcm91cEZpbHRlcjogc3RyaW5nXHJcblxyXG4gICAgY29uc3RydWN0b3Ioc2FtcGxlUmF0ZTogU2FtcGxlUmF0ZUVudW0sIGdyb3VwRmlsdGVyOiBzdHJpbmcpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5TYW1wbGVSYXRlID0gc2FtcGxlUmF0ZTtcclxuICAgICAgICB0aGlzLkdyb3VwRmlsdGVyID0gZ3JvdXBGaWx0ZXI7XHJcbiAgICB9XHJcbn0iLCJjbGFzcyBDaGFubmVsSHViTW9kZWxcclxue1xyXG4gICAgcHVibGljIE5hbWU6IHN0cmluZ1xyXG4gICAgcHVibGljIEdyb3VwOiBzdHJpbmdcclxuICAgIHB1YmxpYyBEYXRhVHlwZTogT25lRGFzRGF0YVR5cGVFbnVtXHJcbiAgICBwdWJsaWMgR3VpZDogc3RyaW5nXHJcbiAgICBwdWJsaWMgQ3JlYXRpb25EYXRlVGltZTogc3RyaW5nXHJcbiAgICBwdWJsaWMgVW5pdDogc3RyaW5nXHJcbiAgICBwdWJsaWMgVHJhbnNmZXJGdW5jdGlvblNldDogYW55W11cclxuICAgIHB1YmxpYyBBc3NvY2lhdGVkRGF0YUlucHV0SWQ6IHN0cmluZ1xyXG4gICAgcHVibGljIEFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQ6IHN0cmluZ1tdXHJcblxyXG4gICAgY29uc3RydWN0b3IobmFtZTogc3RyaW5nLCBncm91cDogc3RyaW5nLCBkYXRhVHlwZTogT25lRGFzRGF0YVR5cGVFbnVtKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuTmFtZSA9IG5hbWU7XHJcbiAgICAgICAgdGhpcy5Hcm91cCA9IGdyb3VwO1xyXG4gICAgICAgIHRoaXMuRGF0YVR5cGUgPSBkYXRhVHlwZTtcclxuICAgICAgICB0aGlzLkd1aWQgPSBHdWlkLk5ld0d1aWQoKVxyXG4gICAgICAgIHRoaXMuQ3JlYXRpb25EYXRlVGltZSA9IG5ldyBEYXRlKCkudG9JU09TdHJpbmcoKVxyXG4gICAgICAgIHRoaXMuVW5pdCA9IFwiXCJcclxuICAgICAgICB0aGlzLlRyYW5zZmVyRnVuY3Rpb25TZXQgPSBbXVxyXG4gICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dElkID0gXCJcIlxyXG4gICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldCA9IFtdXHJcbiAgICB9XHJcbn0iLCJjbGFzcyBPbmVEYXNNb2R1bGVNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgRGF0YVR5cGU6IE9uZURhc0RhdGFUeXBlRW51bVxyXG4gICAgcHVibGljIERhdGFEaXJlY3Rpb246IERhdGFEaXJlY3Rpb25FbnVtXHJcbiAgICBwdWJsaWMgRW5kaWFubmVzczogRW5kaWFubmVzc0VudW1cclxuICAgIHB1YmxpYyBTaXplOiBudW1iZXJcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihkYXRhVHlwZTogT25lRGFzRGF0YVR5cGVFbnVtLCBkYXRhRGlyZWN0aW9uOiBEYXRhRGlyZWN0aW9uRW51bSwgZW5kaWFubmVzczogRW5kaWFubmVzc0VudW0sIHNpemU6IG51bWJlcilcclxuICAgIHtcclxuICAgICAgICB0aGlzLkRhdGFUeXBlID0gZGF0YVR5cGVcclxuICAgICAgICB0aGlzLkRhdGFEaXJlY3Rpb24gPSBkYXRhRGlyZWN0aW9uXHJcbiAgICAgICAgdGhpcy5FbmRpYW5uZXNzID0gZW5kaWFubmVzc1xyXG4gICAgICAgIHRoaXMuU2l6ZSA9IHNpemVcclxuICAgIH1cclxufVxyXG4iLCJjbGFzcyBUcmFuc2ZlckZ1bmN0aW9uTW9kZWxcclxue1xyXG4gICAgcHVibGljIERhdGVUaW1lOiBzdHJpbmdcclxuICAgIHB1YmxpYyBUeXBlOiBzdHJpbmdcclxuICAgIHB1YmxpYyBPcHRpb246IHN0cmluZ1xyXG4gICAgcHVibGljIEFyZ3VtZW50OiBzdHJpbmdcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihkYXRlVGltZTogc3RyaW5nLCB0eXBlOiBzdHJpbmcsIG9wdGlvbjogc3RyaW5nLCBhcmd1bWVudDogc3RyaW5nKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuRGF0ZVRpbWUgPSBkYXRlVGltZVxyXG4gICAgICAgIHRoaXMuVHlwZSA9IHR5cGVcclxuICAgICAgICB0aGlzLk9wdGlvbiA9IG9wdGlvblxyXG4gICAgICAgIHRoaXMuQXJndW1lbnQgPSBhcmd1bWVudFxyXG4gICAgfVxyXG59IiwiZGVjbGFyZSB2YXIgc2lnbmFsUjogYW55XHJcblxyXG5jbGFzcyBDb25uZWN0aW9uTWFuYWdlclxyXG57XHJcbiAgICBwdWJsaWMgc3RhdGljIFdlYkNsaWVudEh1YjogYW55IC8vIGltcHJvdmU6IHVzZSB0eXBpbmdzXHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBJbml0aWFsaXplKGVuYWJsZUxvZ2dpbmc6IGJvb2xlYW4pXHJcbiAgICB7XHJcbiAgICAgICAgQ29ubmVjdGlvbk1hbmFnZXIuV2ViQ2xpZW50SHViID0gbmV3IHNpZ25hbFIuSHViQ29ubmVjdGlvbignL3dlYmNsaWVudGh1YicpO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgSW52b2tlV2ViQ2xpZW50SHViID0gYXN5bmMobWV0aG9kTmFtZTogc3RyaW5nLCAuLi5hcmdzOiBhbnlbXSkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gYXdhaXQgUHJvbWlzZS5yZXNvbHZlKENvbm5lY3Rpb25NYW5hZ2VyLldlYkNsaWVudEh1Yi5pbnZva2UobWV0aG9kTmFtZSwgLi4uYXJncykpXHJcbiAgICB9XHJcbn1cclxuIiwiY2xhc3MgRW51bWVyYXRpb25IZWxwZXJcclxue1xyXG4gICAgcHVibGljIHN0YXRpYyBEZXNjcmlwdGlvbjogeyBbaW5kZXg6IHN0cmluZ106IHN0cmluZyB9ID0ge31cclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIEdldEVudW1Mb2NhbGl6YXRpb24gPSAodHlwZU5hbWU6IHN0cmluZywgdmFsdWUpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdmFyIGtleTogc3RyaW5nID0gZXZhbCh0eXBlTmFtZSArIFwiW1wiICsgdmFsdWUgKyBcIl1cIilcclxuICAgICAgICByZXR1cm4gZXZhbChcIkVudW1lcmF0aW9uSGVscGVyLkRlc2NyaXB0aW9uWydcIiArIHR5cGVOYW1lICsgXCJfXCIgKyBrZXkgKyBcIiddXCIpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBHZXRFbnVtVmFsdWVzID0gKHR5cGVOYW1lOiBzdHJpbmcpID0+XHJcbiAgICB7XHJcbiAgICAgICAgbGV0IHZhbHVlczogYW55W11cclxuXHJcbiAgICAgICAgdmFsdWVzID0gZXZhbChcIk9iamVjdC5rZXlzKFwiICsgdHlwZU5hbWUgKyBcIikubWFwKGtleSA9PiBcIiArIHR5cGVOYW1lICsgXCJba2V5XSlcIilcclxuICAgICAgICByZXR1cm4gPG51bWJlcltdPnZhbHVlcy5maWx0ZXIodmFsdWUgPT4gdHlwZW9mICh2YWx1ZSkgPT09IFwibnVtYmVyXCIpXHJcbiAgICB9XHJcbn0iLCJsZXQgRXJyb3JNZXNzYWdlOiB7IFtpbmRleDogc3RyaW5nXTogc3RyaW5nIH0gPSB7fVxyXG5FcnJvck1lc3NhZ2VbXCJNdWx0aU1hcHBpbmdFZGl0b3JWaWV3TW9kZWxfSW52YWxpZFNldHRpbmdzXCJdID0gXCJPbmUgb3IgbW9yZSBzZXR0aW5ncyBhcmUgaW52YWxpZC5cIlxyXG5FcnJvck1lc3NhZ2VbXCJNdWx0aU1hcHBpbmdFZGl0b3JWaWV3TW9kZWxfV3JvbmdEYXRhVHlwZVwiXSA9IFwiT25lIG9yIG1vcmUgdmFyaWFibGUtY2hhbm5lbCBkYXRhIHR5cGUgY29tYmluYXRpb25zIGFyZSBpbnZhbGlkLlwiXHJcbkVycm9yTWVzc2FnZVtcIlByb2plY3RfQ2hhbm5lbEFscmVhZHlFeGlzdHNcIl0gPSBcIkEgY2hhbm5lbCB3aXRoIHRoYXQgbmFtZSBhbHJlYWR5IGV4aXN0cy5cIlxyXG5FcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0RldGFjaGVkRXhjbGFtYXRpb25NYXJrTm90QWxsb3dlZFwiXSA9IFwiQSBkZXRhY2hlZCBleGNsYW1hdGlvbiBtYXJrIGlzIG5vdCBhbGxvd2VkLlwiXHJcbkVycm9yTWVzc2FnZVtcIlByb2plY3RfR3JvdXBGaWx0ZXJFbXB0eVwiXSA9IFwiVGhlIGdyb3VwIGZpbHRlciBtdXN0IG5vdCBiZSBlbXB0eS5cIlxyXG5FcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0lzQWxyZWFkeUluR3JvdXBcIl0gPSBcIlRoZSBjaGFubmVsIGlzIGFscmVhZHkgYSBtZW1iZXIgb2YgdGhpcyBncm91cC5cIlxyXG5FcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0ludmFsaWRDaGFyYWN0ZXJzXCJdID0gXCJVc2UgQS1aLCBhLXosIDAtOSBvciBfLlwiXHJcbkVycm9yTWVzc2FnZVtcIlByb2plY3RfSW52YWxpZExlYWRpbmdDaGFyYWN0ZXJcIl0gPSBcIlVzZSBBLVogb3IgYS16IGFzIGZpcnN0IGNoYXJhY3Rlci5cIlxyXG5FcnJvck1lc3NhZ2VbXCJQcm9qZWN0X05hbWVFbXB0eVwiXSA9IFwiVGhlIG5hbWUgbXVzdCBub3QgYmUgZW1wdHkuXCJcclxuIiwiY2xhc3MgT2JzZXJ2YWJsZUdyb3VwPFQ+XHJcbntcclxuICAgIEtleTogc3RyaW5nO1xyXG4gICAgTWVtYmVyczogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8VD5cclxuXHJcbiAgICBjb25zdHJ1Y3RvcihrZXk6IHN0cmluZywgbWVtYmVyczogVFtdID0gbmV3IEFycmF5PFQ+KCkpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5LZXkgPSBrZXlcclxuICAgICAgICB0aGlzLk1lbWJlcnMgPSBrby5vYnNlcnZhYmxlQXJyYXkobWVtYmVycylcclxuICAgIH1cclxufVxyXG5cclxuZnVuY3Rpb24gT2JzZXJ2YWJsZUdyb3VwQnk8VD4obGlzdDogVFtdLCBuYW1lR2V0dGVyOiAoeDogVCkgPT4gc3RyaW5nLCBncm91cE5hbWVHZXR0ZXI6ICh4OiBUKSA9PiBzdHJpbmcsIGZpbHRlcjogc3RyaW5nKTogT2JzZXJ2YWJsZUdyb3VwPFQ+W11cclxue1xyXG4gICAgbGV0IHJlc3VsdDogT2JzZXJ2YWJsZUdyb3VwPFQ+W11cclxuICAgIGxldCByZWdFeHA6IFJlZ0V4cFxyXG5cclxuICAgIHJlc3VsdCA9IFtdXHJcbiAgICByZWdFeHAgPSBuZXcgUmVnRXhwKGZpbHRlciwgXCJpXCIpXHJcblxyXG4gICAgbGlzdC5mb3JFYWNoKGVsZW1lbnQgPT5cclxuICAgIHtcclxuICAgICAgICBpZiAocmVnRXhwLnRlc3QobmFtZUdldHRlcihlbGVtZW50KSkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBncm91cE5hbWVHZXR0ZXIoZWxlbWVudCkuc3BsaXQoXCJcXG5cIikuZm9yRWFjaChncm91cE5hbWUgPT5cclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgQWRkVG9Hcm91cGVkQXJyYXkoZWxlbWVudCwgZ3JvdXBOYW1lLCByZXN1bHQpXHJcbiAgICAgICAgICAgIH0pXHJcbiAgICAgICAgfVxyXG4gICAgfSlcclxuXHJcbiAgICByZXR1cm4gcmVzdWx0XHJcbn1cclxuXHJcbmZ1bmN0aW9uIEFkZFRvR3JvdXBlZEFycmF5PFQ+KGl0ZW06IFQsIGdyb3VwTmFtZTogc3RyaW5nLCBvYnNlcnZhYmxlR3JvdXBTZXQ6IE9ic2VydmFibGVHcm91cDxUPltdKVxyXG57XHJcbiAgICBsZXQgZ3JvdXA6IE9ic2VydmFibGVHcm91cDxUPlxyXG5cclxuICAgIGdyb3VwID0gb2JzZXJ2YWJsZUdyb3VwU2V0LmZpbmQoeSA9PiB5LktleSA9PT0gZ3JvdXBOYW1lKVxyXG5cclxuICAgIGlmICghZ3JvdXApXHJcbiAgICB7XHJcbiAgICAgICAgZ3JvdXAgPSBuZXcgT2JzZXJ2YWJsZUdyb3VwPFQ+KGdyb3VwTmFtZSlcclxuICAgICAgICBvYnNlcnZhYmxlR3JvdXBTZXQucHVzaChncm91cClcclxuICAgIH1cclxuXHJcbiAgICBncm91cC5NZW1iZXJzLnB1c2goaXRlbSlcclxufVxyXG5cclxuLy9mdW5jdGlvbiBBZGRUb0dyb3VwZWRPYnNlcnZhYmxlQXJyYXk8VD4oaXRlbTogVCwgZ3JvdXBOYW1lOiBzdHJpbmcsIG9ic2VydmFibGVHcm91cFNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8T2JzZXJ2YWJsZUdyb3VwPFQ+PilcclxuLy97XHJcbi8vICAgIGxldCBncm91cDogT2JzZXJ2YWJsZUdyb3VwPFQ+XHJcblxyXG4vLyAgICBncm91cCA9IG9ic2VydmFibGVHcm91cFNldCgpLmZpbmQoeSA9PiB5LktleSA9PT0gZ3JvdXBOYW1lKVxyXG5cclxuLy8gICAgaWYgKCFncm91cClcclxuLy8gICAge1xyXG4vLyAgICAgICAgZ3JvdXAgPSBuZXcgT2JzZXJ2YWJsZUdyb3VwPFQ+KGdyb3VwTmFtZSlcclxuLy8gICAgICAgIG9ic2VydmFibGVHcm91cFNldC5wdXNoKGdyb3VwKVxyXG4vLyAgICB9XHJcblxyXG4vLyAgICBncm91cC5NZW1iZXJzLnB1c2goaXRlbSlcclxuLy99XHJcblxyXG4vL2Z1bmN0aW9uIFJlbW92ZUZyb21Hcm91cGVkT2JzZXJ2YWJsZUFycmF5PFQ+KGl0ZW06IFQsIG9ic2VydmFibGVHcm91cFNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8T2JzZXJ2YWJsZUdyb3VwPFQ+PilcclxuLy97XHJcbi8vICAgIHZhciBncm91cDogT2JzZXJ2YWJsZUdyb3VwPFQ+XHJcblxyXG4vLyAgICBvYnNlcnZhYmxlR3JvdXBTZXQoKS5zb21lKHggPT5cclxuLy8gICAge1xyXG4vLyAgICAgICAgaWYgKHguTWVtYmVycygpLmluZGV4T2YoaXRlbSkgPiAtMSlcclxuLy8gICAgICAgIHtcclxuLy8gICAgICAgICAgICBncm91cCA9IHhcclxuXHJcbi8vICAgICAgICAgICAgcmV0dXJuIHRydWVcclxuLy8gICAgICAgIH1cclxuXHJcbi8vICAgICAgICByZXR1cm4gZmFsc2VcclxuLy8gICAgfSlcclxuXHJcbi8vICAgIGlmIChncm91cClcclxuLy8gICAge1xyXG4vLyAgICAgICAgZ3JvdXAuTWVtYmVycy5yZW1vdmUoaXRlbSlcclxuXHJcbi8vICAgICAgICBpZiAoZ3JvdXAuTWVtYmVycygpLmxlbmd0aCA9PT0gMClcclxuLy8gICAgICAgIHtcclxuLy8gICAgICAgICAgICBvYnNlcnZhYmxlR3JvdXBTZXQucmVtb3ZlKGdyb3VwKVxyXG4vLyAgICAgICAgfVxyXG5cclxuLy8gICAgICAgIHJldHVybiB0cnVlXHJcbi8vICAgIH1cclxuXHJcbi8vICAgIHJldHVybiBmYWxzZVxyXG4vL31cclxuXHJcbi8vZnVuY3Rpb24gVXBkYXRlR3JvdXBlZE9ic2VydmFibGVBcnJheTxUPihpdGVtOiBULCBncm91cE5hbWU6IHN0cmluZywgb2JzZXJ2YWJsZUdyb3VwU2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxPYnNlcnZhYmxlR3JvdXA8VD4+KVxyXG4vL3tcclxuLy8gICAgUmVtb3ZlRnJvbUdyb3VwZWRPYnNlcnZhYmxlQXJyYXkoaXRlbSwgb2JzZXJ2YWJsZUdyb3VwU2V0KVxyXG4vLyAgICBBZGRUb0dyb3VwZWRPYnNlcnZhYmxlQXJyYXkoaXRlbSwgZ3JvdXBOYW1lLCBvYnNlcnZhYmxlR3JvdXBTZXQpXHJcbi8vfVxyXG5cclxuZnVuY3Rpb24gTWFwTWFueTxUQXJyYXlFbGVtZW50LCBUU2VsZWN0PihhcnJheTogVEFycmF5RWxlbWVudFtdLCBtYXBGdW5jOiAoaXRlbTogVEFycmF5RWxlbWVudCkgPT4gVFNlbGVjdFtdKTogVFNlbGVjdFtdXHJcbntcclxuICAgIHJldHVybiBhcnJheS5yZWR1Y2UoKHByZXZpb3VzLCBjdXJyZW50LCBpKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiBwcmV2aW91cy5jb25jYXQobWFwRnVuYyhjdXJyZW50KSk7XHJcbiAgICB9LCA8VFNlbGVjdFtdPltdKTtcclxufVxyXG5cclxuY2xhc3MgR3VpZFxyXG57XHJcbiAgICBzdGF0aWMgTmV3R3VpZCgpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuICd4eHh4eHh4eC14eHh4LTR4eHgteXh4eC14eHh4eHh4eHh4eHgnLnJlcGxhY2UoL1t4eV0vZywgZnVuY3Rpb24gKGMpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB2YXIgciA9IE1hdGgucmFuZG9tKCkgKiAxNiB8IDBcclxuICAgICAgICAgICAgdmFyIHYgPSBjID09PSAneCcgPyByIDogKHIgJiAweDMgfCAweDgpXHJcblxyXG4gICAgICAgICAgICByZXR1cm4gdi50b1N0cmluZygxNilcclxuICAgICAgICB9KVxyXG4gICAgfVxyXG59XHJcblxyXG5mdW5jdGlvbiBkZWxheShtczogbnVtYmVyKVxyXG57XHJcbiAgICByZXR1cm4gbmV3IFByb21pc2UocmVzb2x2ZSA9PiBzZXRUaW1lb3V0KHJlc29sdmUsIG1zKSk7XHJcbn1cclxuXHJcbmxldCBDaGVja05hbWluZ0NvbnZlbnRpb24gPSAodmFsdWU6IHN0cmluZykgPT5cclxue1xyXG4gICAgdmFyIHJlZ0V4cDogYW55XHJcblxyXG4gICAgaWYgKHZhbHVlLmxlbmd0aCA9PT0gMClcclxuICAgIHtcclxuICAgICAgICByZXR1cm4geyBIYXNFcnJvcjogdHJ1ZSwgRXJyb3JEZXNjcmlwdGlvbjogRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9OYW1lRW1wdHlcIl0gfVxyXG4gICAgfVxyXG5cclxuICAgIHJlZ0V4cCA9IG5ldyBSZWdFeHAoXCJbXkEtWmEtejAtOV9dXCIpXHJcblxyXG4gICAgaWYgKHJlZ0V4cC50ZXN0KHZhbHVlKSlcclxuICAgIHtcclxuICAgICAgICByZXR1cm4geyBIYXNFcnJvcjogdHJ1ZSwgRXJyb3JEZXNjcmlwdGlvbjogRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9JbnZhbGlkQ2hhcmFjdGVyc1wiXSB9XHJcbiAgICB9XHJcblxyXG4gICAgcmVnRXhwID0gbmV3IFJlZ0V4cChcIl5bMC05X11cIilcclxuXHJcbiAgICBpZiAocmVnRXhwLnRlc3QodmFsdWUpKVxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB7IEhhc0Vycm9yOiB0cnVlLCBFcnJvckRlc2NyaXB0aW9uOiBFcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0ludmFsaWRMZWFkaW5nQ2hhcmFjdGVyXCJdIH1cclxuICAgIH1cclxuXHJcbiAgICByZXR1cm4ge1xyXG4gICAgICAgIEhhc0Vycm9yOiBmYWxzZSxcclxuICAgICAgICBFcnJvckRlc2NyaXB0aW9uOiBcIlwiXHJcbiAgICB9XHJcbn0iLCJjbGFzcyBFeHRlbnNpb25GYWN0b3J5XHJcbntcclxuICAgIHB1YmxpYyBzdGF0aWMgQ3JlYXRlRXh0ZW5zaW9uVmlld01vZGVsQXN5bmMgPSBhc3luYyAoZXh0ZW5zaW9uVHlwZTogc3RyaW5nLCBleHRlbnNpb25Nb2RlbDogYW55KSA9PlxyXG4gICAge1xyXG4gICAgICAgIGxldCBleHRlbnNpb25JZGVudGlmaWNhdGlvbjogRXh0ZW5zaW9uSWRlbnRpZmljYXRpb25WaWV3TW9kZWxcclxuICAgICAgICBsZXQgZXh0ZW5zaW9uVmlld01vZGVsOiBFeHRlbnNpb25WaWV3TW9kZWxCYXNlXHJcbiAgICAgICAgbGV0IGV4dGVuc2lvblZpZXdNb2RlbFJhdzogc3RyaW5nXHJcblxyXG4gICAgICAgIGV4dGVuc2lvbklkZW50aWZpY2F0aW9uID0gRXh0ZW5zaW9uSGl2ZS5GaW5kRXh0ZW5zaW9uSWRlbnRpZmljYXRpb24oZXh0ZW5zaW9uVHlwZSwgZXh0ZW5zaW9uTW9kZWwuRGVzY3JpcHRpb24uSWQpXHJcblxyXG4gICAgICAgIGlmIChleHRlbnNpb25JZGVudGlmaWNhdGlvbilcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGV4dGVuc2lvblZpZXdNb2RlbFJhdyA9IGF3YWl0IENvbm5lY3Rpb25NYW5hZ2VyLkludm9rZVdlYkNsaWVudEh1YihcIkdldEV4dGVuc2lvblN0cmluZ1Jlc291cmNlXCIsIGV4dGVuc2lvbk1vZGVsLkRlc2NyaXB0aW9uLklkLCBleHRlbnNpb25JZGVudGlmaWNhdGlvbi5WaWV3TW9kZWxSZXNvdXJjZU5hbWUpXHJcbiAgICAgICAgICAgIGV4dGVuc2lvblZpZXdNb2RlbCA9IDxFeHRlbnNpb25WaWV3TW9kZWxCYXNlPm5ldyBGdW5jdGlvbihleHRlbnNpb25WaWV3TW9kZWxSYXcgKyBcIjsgcmV0dXJuIFZpZXdNb2RlbENvbnN0cnVjdG9yXCIpKCkoZXh0ZW5zaW9uTW9kZWwsIGV4dGVuc2lvbklkZW50aWZpY2F0aW9uKVxyXG5cclxuICAgICAgICAgICAgcmV0dXJuIGV4dGVuc2lvblZpZXdNb2RlbFxyXG4gICAgICAgIH1cclxuICAgICAgICBlbHNlXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aHJvdyBuZXcgRXJyb3IoXCJObyBjb3JyZXNwb25kaW5nIGV4dGVuc2lvbiBkZXNjcmlwdGlvbiBmb3IgZXh0ZW5zaW9uIElEICdcIiArIGV4dGVuc2lvbk1vZGVsLkRlc2NyaXB0aW9uLklkICsgXCInIGZvdW5kLlwiKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxufSIsImNsYXNzIEV4dGVuc2lvbkhpdmVcclxue1xyXG4gICAgLy8gZmllbGRzXHJcbiAgICBwdWJsaWMgc3RhdGljIEV4dGVuc2lvbklkZW50aWZpY2F0aW9uU2V0OiBNYXA8c3RyaW5nLCBFeHRlbnNpb25JZGVudGlmaWNhdGlvblZpZXdNb2RlbFtdPlxyXG5cclxuICAgIC8vIGNvbnN0cnVjdG9yc1xyXG4gICAgc3RhdGljIEluaXRpYWxpemUgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIEV4dGVuc2lvbkhpdmUuRXh0ZW5zaW9uSWRlbnRpZmljYXRpb25TZXQgPSBuZXcgTWFwPHN0cmluZywgRXh0ZW5zaW9uSWRlbnRpZmljYXRpb25WaWV3TW9kZWxbXT4oKVxyXG4gICAgfSAgIFxyXG5cclxuICAgIHN0YXRpYyBGaW5kRXh0ZW5zaW9uSWRlbnRpZmljYXRpb24gPSAoZXh0ZW5zaW9uVHlwZU5hbWU6IHN0cmluZywgZXh0ZW5zaW9uSWQ6IHN0cmluZykgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gRXh0ZW5zaW9uSGl2ZS5FeHRlbnNpb25JZGVudGlmaWNhdGlvblNldC5nZXQoZXh0ZW5zaW9uVHlwZU5hbWUpLmZpbmQoZXh0ZW5zaW9uSWRlbnRpZmljYXRpb24gPT4gZXh0ZW5zaW9uSWRlbnRpZmljYXRpb24uSWQgPT09IGV4dGVuc2lvbklkKTtcclxuICAgIH1cclxufSIsImNsYXNzIENoYW5uZWxIdWJWaWV3TW9kZWxcclxue1xyXG4gICAgcHVibGljIE5hbWU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgR3JvdXA6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRGF0YVR5cGU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxPbmVEYXNEYXRhVHlwZUVudW0+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgR3VpZDogc3RyaW5nXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgQ3JlYXRpb25EYXRlVGltZTogc3RyaW5nXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgVW5pdDogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyByZWFkb25seSBUcmFuc2ZlckZ1bmN0aW9uU2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsPlxyXG4gICAgcHVibGljIFNlbGVjdGVkVHJhbnNmZXJGdW5jdGlvbjogS25vY2tvdXRPYnNlcnZhYmxlPFRyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWw+XHJcbiAgICBwdWJsaWMgRXZhbHVhdGVkVHJhbnNmZXJGdW5jdGlvblNldDogKCh2YWx1ZTogbnVtYmVyKSA9PiBudW1iZXIpW11cclxuICAgIHB1YmxpYyBJc1NlbGVjdGVkOiBLbm9ja291dE9ic2VydmFibGU8Ym9vbGVhbj5cclxuXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRGF0YVR5cGVTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PE9uZURhc0RhdGFUeXBlRW51bT5cclxuXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgQXNzb2NpYXRlZERhdGFJbnB1dDogS25vY2tvdXRPYnNlcnZhYmxlPERhdGFQb3J0Vmlld01vZGVsPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IEFzc29jaWF0ZWREYXRhT3V0cHV0U2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxEYXRhUG9ydFZpZXdNb2RlbD5cclxuXHJcbiAgICBwcml2YXRlIEFzc29jaWF0ZWREYXRhSW5wdXRJZDogc3RyaW5nXHJcbiAgICBwcml2YXRlIEFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQ6IHN0cmluZ1tdXHJcblxyXG4gICAgY29uc3RydWN0b3IoY2hhbm5lbEh1Yk1vZGVsOiBDaGFubmVsSHViTW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5OYW1lID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KGNoYW5uZWxIdWJNb2RlbC5OYW1lKVxyXG4gICAgICAgIHRoaXMuR3JvdXAgPSBrby5vYnNlcnZhYmxlPHN0cmluZz4oY2hhbm5lbEh1Yk1vZGVsLkdyb3VwKVxyXG4gICAgICAgIHRoaXMuRGF0YVR5cGVTZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8T25lRGFzRGF0YVR5cGVFbnVtPihFbnVtZXJhdGlvbkhlbHBlci5HZXRFbnVtVmFsdWVzKCdPbmVEYXNEYXRhVHlwZUVudW0nKSlcclxuICAgICAgICB0aGlzLkRhdGFUeXBlID0ga28ub2JzZXJ2YWJsZTxPbmVEYXNEYXRhVHlwZUVudW0+KGNoYW5uZWxIdWJNb2RlbC5EYXRhVHlwZSlcclxuICAgICAgICB0aGlzLkd1aWQgPSBjaGFubmVsSHViTW9kZWwuR3VpZFxyXG4gICAgICAgIHRoaXMuQ3JlYXRpb25EYXRlVGltZSA9IGNoYW5uZWxIdWJNb2RlbC5DcmVhdGlvbkRhdGVUaW1lXHJcbiAgICAgICAgdGhpcy5Vbml0ID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KGNoYW5uZWxIdWJNb2RlbC5Vbml0KVxyXG4gICAgICAgIHRoaXMuVHJhbnNmZXJGdW5jdGlvblNldCA9IGtvLm9ic2VydmFibGVBcnJheTxUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsPihjaGFubmVsSHViTW9kZWwuVHJhbnNmZXJGdW5jdGlvblNldC5tYXAodGYgPT4gbmV3IFRyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWwodGYpKSlcclxuICAgICAgICB0aGlzLlNlbGVjdGVkVHJhbnNmZXJGdW5jdGlvbiA9IGtvLm9ic2VydmFibGU8VHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbD4odGhpcy5DcmVhdGVEZWZhdWx0VHJhbnNmZXJGdW5jdGlvbigpKVxyXG4gICAgICAgIHRoaXMuSXNTZWxlY3RlZCA9IGtvLm9ic2VydmFibGU8Ym9vbGVhbj4oZmFsc2UpXHJcblxyXG4gICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dCA9IGtvLm9ic2VydmFibGU8RGF0YVBvcnRWaWV3TW9kZWw+KClcclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0U2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PERhdGFQb3J0Vmlld01vZGVsPigpXHJcblxyXG4gICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dElkID0gY2hhbm5lbEh1Yk1vZGVsLkFzc29jaWF0ZWREYXRhSW5wdXRJZFxyXG4gICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldCA9IGNoYW5uZWxIdWJNb2RlbC5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0XHJcbiAgICAgICAgdGhpcy5FdmFsdWF0ZWRUcmFuc2ZlckZ1bmN0aW9uU2V0ID0gW11cclxuICAgIH1cclxuXHJcbiAgICAvLyBtZXRob2RzXHJcbiAgICBwdWJsaWMgR2V0VHJhbnNmb3JtZWRWYWx1ZSA9ICh2YWx1ZTogYW55KTogbnVtYmVyID0+IFxyXG4gICAge1xyXG4gICAgICAgIGlmICh2YWx1ZSA9PT0gXCJOYU5cIilcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHZhbHVlID0gTmFOXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0aGlzLkV2YWx1YXRlZFRyYW5zZmVyRnVuY3Rpb25TZXQuZm9yRWFjaCh0ZiA9PiB2YWx1ZSA9IHRmKHZhbHVlKSlcclxuXHJcbiAgICAgICAgcmV0dXJuIHZhbHVlXHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBDcmVhdGVEZWZhdWx0VHJhbnNmZXJGdW5jdGlvbiA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIG5ldyBUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsKG5ldyBUcmFuc2ZlckZ1bmN0aW9uTW9kZWwoXCIwMDAxLTAxLTAxVDAwOjAwOjAwWlwiLCBcInBvbHlub21pYWxcIiwgXCJwZXJtYW5lbnRcIiwgXCIxOzBcIikpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIElzQXNzb2NpYXRpb25BbGxvd2VkKGRhdGFQb3J0OiBEYXRhUG9ydFZpZXdNb2RlbClcclxuICAgIHtcclxuICAgICAgICByZXR1cm4gKGRhdGFQb3J0LkRhdGFUeXBlICYgMHhmZikgPT0gKHRoaXMuRGF0YVR5cGUoKSAmIDB4ZmYpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFVwZGF0ZUFzc29jaWF0aW9uID0gKGRhdGFQb3J0OiBEYXRhUG9ydFZpZXdNb2RlbCkgPT5cclxuICAgIHtcclxuICAgICAgICBzd2l0Y2ggKGRhdGFQb3J0LkRhdGFEaXJlY3Rpb24pXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLklucHV0OlxyXG5cclxuICAgICAgICAgICAgICAgIHRoaXMuUmVzZXRBc3NvY2lhdGlvbihmYWxzZSwgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0KCkpXHJcbiAgICAgICAgICAgICAgICBicmVha1xyXG5cclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5PdXRwdXQ6XHJcblxyXG4gICAgICAgICAgICAgICAgdGhpcy5SZXNldEFzc29jaWF0aW9uKGZhbHNlLCBkYXRhUG9ydClcclxuICAgICAgICAgICAgICAgIGJyZWFrXHJcblxyXG4gICAgICAgICAgICBkZWZhdWx0OlxyXG4gICAgICAgICAgICAgICAgdGhyb3cgbmV3IEVycm9yKFwiTm90IGltcGxlbWVudGVkLlwiKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuU2V0QXNzb2NpYXRpb24oZGF0YVBvcnQpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFNldEFzc29jaWF0aW9uKGRhdGFQb3J0OiBEYXRhUG9ydFZpZXdNb2RlbClcclxuICAgIHtcclxuICAgICAgICBkYXRhUG9ydC5Bc3NvY2lhdGVkQ2hhbm5lbEh1YlNldC5wdXNoKHRoaXMpXHJcblxyXG4gICAgICAgIHN3aXRjaCAoZGF0YVBvcnQuRGF0YURpcmVjdGlvbilcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uSW5wdXQ6XHJcblxyXG4gICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0KGRhdGFQb3J0KVxyXG4gICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0SWQgPSBkYXRhUG9ydC5Ub0Z1bGxRdWFsaWZpZWRJZGVudGlmaWVyKClcclxuXHJcbiAgICAgICAgICAgICAgICBicmVha1xyXG5cclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5PdXRwdXQ6XHJcblxyXG4gICAgICAgICAgICAgICAgbGV0IGRhdGFPdXRwdXRJZCA9IGRhdGFQb3J0LlRvRnVsbFF1YWxpZmllZElkZW50aWZpZXIoKVxyXG5cclxuICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRTZXQucHVzaChkYXRhUG9ydClcclxuXHJcbiAgICAgICAgICAgICAgICBpZiAodGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0LmluZGV4T2YoZGF0YU91dHB1dElkKSA8IDApXHJcbiAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0LnB1c2goZGF0YVBvcnQuVG9GdWxsUXVhbGlmaWVkSWRlbnRpZmllcigpKVxyXG4gICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgIGJyZWFrXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBSZXNldEFzc29jaWF0aW9uKG1haW50YWluV2Vha1JlZmVyZW5jZTogYm9vbGVhbiwgLi4uZGF0YVBvcnRTZXQ6IERhdGFQb3J0Vmlld01vZGVsW10pXHJcbiAgICB7XHJcbiAgICAgICAgZGF0YVBvcnRTZXQuZm9yRWFjaChkYXRhUG9ydCA9PlxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgaWYgKCFkYXRhUG9ydClcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuXHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIGRhdGFQb3J0LkFzc29jaWF0ZWRDaGFubmVsSHViU2V0LnJlbW92ZSh0aGlzKVxyXG5cclxuICAgICAgICAgICAgc3dpdGNoIChkYXRhUG9ydC5EYXRhRGlyZWN0aW9uKVxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLklucHV0OlxyXG5cclxuICAgICAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXQobnVsbClcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCFtYWludGFpbldlYWtSZWZlcmVuY2UpXHJcbiAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXRJZCA9IG51bGxcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGJyZWFrXHJcblxyXG4gICAgICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5PdXRwdXQ6XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRTZXQucmVtb3ZlKGRhdGFQb3J0KVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBpZiAoIW1haW50YWluV2Vha1JlZmVyZW5jZSlcclxuICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGxldCBpbmRleDogbnVtYmVyID0gdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0LmluZGV4T2YoZGF0YVBvcnQuVG9GdWxsUXVhbGlmaWVkSWRlbnRpZmllcigpKVxyXG5cclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKGluZGV4ID4gLTEpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldC5zcGxpY2UoaW5kZXgsIDEpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0gICBcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGJyZWFrXHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9KVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBSZXNldEFsbEFzc29jaWF0aW9ucyhtYWludGFpbldlYWtSZWZlcmVuY2U6IGJvb2xlYW4pXHJcbiAgICB7XHJcbiAgICAgICAgaWYgKHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dCgpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5SZXNldEFzc29jaWF0aW9uKG1haW50YWluV2Vha1JlZmVyZW5jZSwgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0KCkpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0aGlzLlJlc2V0QXNzb2NpYXRpb24obWFpbnRhaW5XZWFrUmVmZXJlbmNlLCAuLi50aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0U2V0KCkpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEdldEFzc29jaWF0ZWREYXRhSW5wdXRJZCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dElkO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBHZXRBc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0ID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0O1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb01vZGVsKClcclxuICAgIHtcclxuICAgICAgICByZXR1cm4ge1xyXG4gICAgICAgICAgICBOYW1lOiA8c3RyaW5nPnRoaXMuTmFtZSgpLFxyXG4gICAgICAgICAgICBHcm91cDogPHN0cmluZz50aGlzLkdyb3VwKCksXHJcbiAgICAgICAgICAgIERhdGFUeXBlOiA8T25lRGFzRGF0YVR5cGVFbnVtPnRoaXMuRGF0YVR5cGUoKSxcclxuICAgICAgICAgICAgR3VpZDogPHN0cmluZz50aGlzLkd1aWQsXHJcbiAgICAgICAgICAgIENyZWF0aW9uRGF0ZVRpbWU6IDxzdHJpbmc+dGhpcy5DcmVhdGlvbkRhdGVUaW1lLFxyXG4gICAgICAgICAgICBVbml0OiA8c3RyaW5nPnRoaXMuVW5pdCgpLFxyXG4gICAgICAgICAgICBUcmFuc2ZlckZ1bmN0aW9uU2V0OiA8VHJhbnNmZXJGdW5jdGlvbk1vZGVsW10+dGhpcy5UcmFuc2ZlckZ1bmN0aW9uU2V0KCkubWFwKHRmID0+IHRmLlRvTW9kZWwoKSksXHJcbiAgICAgICAgICAgIEFzc29jaWF0ZWREYXRhSW5wdXRJZDogPHN0cmluZz50aGlzLkFzc29jaWF0ZWREYXRhSW5wdXRJZCxcclxuICAgICAgICAgICAgQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldDogPHN0cmluZ1tdPnRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldFxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgQWRkVHJhbnNmZXJGdW5jdGlvbiA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5UcmFuc2ZlckZ1bmN0aW9uU2V0LnB1c2godGhpcy5TZWxlY3RlZFRyYW5zZmVyRnVuY3Rpb24oKSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgRGVsZXRlVHJhbnNmZXJGdW5jdGlvbiA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5UcmFuc2ZlckZ1bmN0aW9uU2V0LnJlbW92ZSh0aGlzLlNlbGVjdGVkVHJhbnNmZXJGdW5jdGlvbigpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBOZXdUcmFuc2ZlckZ1bmN0aW9uID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLlNlbGVjdGVkVHJhbnNmZXJGdW5jdGlvbih0aGlzLkNyZWF0ZURlZmF1bHRUcmFuc2ZlckZ1bmN0aW9uKCkpXHJcbiAgICB9XHJcblxyXG4gICAgLy8gY29tbWFuZHNcclxuICAgIHB1YmxpYyBTZWxlY3RUcmFuc2ZlckZ1bmN0aW9uID0gKHRyYW5zZmVyRnVuY3Rpb246IFRyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWwpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5TZWxlY3RlZFRyYW5zZmVyRnVuY3Rpb24odHJhbnNmZXJGdW5jdGlvbilcclxuICAgIH1cclxufSIsImNsYXNzIE9uZURhc01vZHVsZVZpZXdNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgRGF0YVR5cGU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxPbmVEYXNEYXRhVHlwZUVudW0+XHJcbiAgICBwdWJsaWMgRGF0YURpcmVjdGlvbjogS25vY2tvdXRPYnNlcnZhYmxlPERhdGFEaXJlY3Rpb25FbnVtPlxyXG4gICAgcHVibGljIEVuZGlhbm5lc3M6IEtub2Nrb3V0T2JzZXJ2YWJsZTxFbmRpYW5uZXNzRW51bT5cclxuICAgIHB1YmxpYyBTaXplOiBLbm9ja291dE9ic2VydmFibGU8bnVtYmVyPlxyXG4gICAgcHVibGljIE1heFNpemU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxudW1iZXI+XHJcbiAgICBwdWJsaWMgRXJyb3JNZXNzYWdlOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIEhhc0Vycm9yOiBLbm9ja291dENvbXB1dGVkPGJvb2xlYW4+XHJcblxyXG4gICAgcHVibGljIERhdGFUeXBlU2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxPbmVEYXNEYXRhVHlwZUVudW0+XHJcblxyXG4gICAgcHJpdmF0ZSBfb25Qcm9wZXJ0eUNoYW5nZWQ6IEV2ZW50RGlzcGF0Y2hlcjxPbmVEYXNNb2R1bGVWaWV3TW9kZWwsIGFueT5cclxuICAgIHByb3RlY3RlZCBfbW9kZWw6IGFueVxyXG5cclxuICAgIGNvbnN0cnVjdG9yKG9uZURhc01vZHVsZU1vZGVsOiBPbmVEYXNNb2R1bGVNb2RlbClcclxuICAgIHtcclxuICAgICAgICB0aGlzLl9tb2RlbCA9IG9uZURhc01vZHVsZU1vZGVsXHJcblxyXG4gICAgICAgIHRoaXMuRGF0YVR5cGVTZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8T25lRGFzRGF0YVR5cGVFbnVtPihFbnVtZXJhdGlvbkhlbHBlci5HZXRFbnVtVmFsdWVzKCdPbmVEYXNEYXRhVHlwZUVudW0nKS5maWx0ZXIoZGF0YVR5cGUgPT4gZGF0YVR5cGUgIT09IE9uZURhc0RhdGFUeXBlRW51bS5CT09MRUFOKSlcclxuICAgICAgICB0aGlzLkRhdGFUeXBlID0ga28ub2JzZXJ2YWJsZTxPbmVEYXNEYXRhVHlwZUVudW0+KG9uZURhc01vZHVsZU1vZGVsLkRhdGFUeXBlKVxyXG4gICAgICAgIHRoaXMuRGF0YURpcmVjdGlvbiA9IGtvLm9ic2VydmFibGU8RGF0YURpcmVjdGlvbkVudW0+KG9uZURhc01vZHVsZU1vZGVsLkRhdGFEaXJlY3Rpb24pXHJcbiAgICAgICAgdGhpcy5FbmRpYW5uZXNzID0ga28ub2JzZXJ2YWJsZTxFbmRpYW5uZXNzRW51bT4ob25lRGFzTW9kdWxlTW9kZWwuRW5kaWFubmVzcylcclxuICAgICAgICB0aGlzLlNpemUgPSBrby5vYnNlcnZhYmxlPG51bWJlcj4ob25lRGFzTW9kdWxlTW9kZWwuU2l6ZSlcclxuICAgICAgICB0aGlzLk1heFNpemUgPSBrby5vYnNlcnZhYmxlPG51bWJlcj4oSW5maW5pdHkpXHJcbiAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UgPSBrby5vYnNlcnZhYmxlPHN0cmluZz4oXCJcIilcclxuICAgICAgICB0aGlzLkhhc0Vycm9yID0ga28uY29tcHV0ZWQ8Ym9vbGVhbj4oKCkgPT4gdGhpcy5FcnJvck1lc3NhZ2UoKS5sZW5ndGggPiAwKVxyXG5cclxuICAgICAgICB0aGlzLl9vblByb3BlcnR5Q2hhbmdlZCA9IG5ldyBFdmVudERpc3BhdGNoZXI8T25lRGFzTW9kdWxlVmlld01vZGVsLCBhbnk+KCk7XHJcblxyXG4gICAgICAgIHRoaXMuRGF0YVR5cGUuc3Vic2NyaWJlKG5ld1ZhbHVlID0+IHRoaXMuT25Qcm9wZXJ0eUNoYW5nZWQoKSlcclxuICAgICAgICB0aGlzLkRhdGFEaXJlY3Rpb24uc3Vic2NyaWJlKG5ld1ZhbHVlID0+IHRoaXMuT25Qcm9wZXJ0eUNoYW5nZWQoKSlcclxuICAgICAgICB0aGlzLlNpemUuc3Vic2NyaWJlKG5ld1ZhbHVlID0+IHRoaXMuT25Qcm9wZXJ0eUNoYW5nZWQoKSlcclxuICAgIH1cclxuXHJcbiAgICBnZXQgUHJvcGVydHlDaGFuZ2VkKCk6IElFdmVudDxPbmVEYXNNb2R1bGVWaWV3TW9kZWwsIGFueT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5fb25Qcm9wZXJ0eUNoYW5nZWQ7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIE9uUHJvcGVydHlDaGFuZ2VkID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLlZhbGlkYXRlKClcclxuICAgICAgICB0aGlzLl9vblByb3BlcnR5Q2hhbmdlZC5kaXNwYXRjaCh0aGlzLCBudWxsKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBHZXRCeXRlQ291bnQgPSAoYm9vbGVhbkJpdFNpemU/OiBudW1iZXIpID0+XHJcbiAgICB7XHJcbiAgICAgICAgaWYgKGJvb2xlYW5CaXRTaXplICYmIHRoaXMuRGF0YVR5cGUoKSA9PT0gT25lRGFzRGF0YVR5cGVFbnVtLkJPT0xFQU4pXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBib29sZWFuQml0U2l6ZSA9IHBhcnNlSW50KDxhbnk+Ym9vbGVhbkJpdFNpemUpXHJcblxyXG4gICAgICAgICAgICByZXR1cm4gTWF0aC5jZWlsKGJvb2xlYW5CaXRTaXplICogdGhpcy5TaXplKCkgLyA4KTtcclxuICAgICAgICB9XHJcbiAgICAgICAgZWxzZVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgcmV0dXJuICh0aGlzLkRhdGFUeXBlKCkgJiAweDBGRikgLyA4ICogdGhpcy5TaXplKClcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFZhbGlkYXRlKClcclxuICAgIHtcclxuICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIlwiKVxyXG5cclxuICAgICAgICBpZiAodGhpcy5TaXplKCkgPCAxIHx8IChpc0Zpbml0ZSh0aGlzLk1heFNpemUoKSkgJiYgdGhpcy5TaXplKCkgPiB0aGlzLk1heFNpemUoKSkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIlNpemUgbXVzdCBiZSB3aXRoaW4gcmFuZ2UgMS4uXCIgKyB0aGlzLk1heFNpemUoKSArIFwiLlwiKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9TdHJpbmcoKVxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLlNpemUoKSArIFwieCBcIiArIEVudW1lcmF0aW9uSGVscGVyLkdldEVudW1Mb2NhbGl6YXRpb24oJ09uZURhc0RhdGFUeXBlRW51bScsIHRoaXMuRGF0YVR5cGUoKSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgRXh0ZW5kTW9kZWwobW9kZWw6IGFueSlcclxuICAgIHtcclxuICAgICAgICAvL1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb01vZGVsKClcclxuICAgIHtcclxuICAgICAgICBsZXQgbW9kZWw6IGFueSA9IHtcclxuICAgICAgICAgICAgJHR5cGU6IDxzdHJpbmc+dGhpcy5fbW9kZWwuJHR5cGUsXHJcbiAgICAgICAgICAgIERhdGFUeXBlOiA8T25lRGFzRGF0YVR5cGVFbnVtPnRoaXMuRGF0YVR5cGUoKSxcclxuICAgICAgICAgICAgU2l6ZTogPG51bWJlcj50aGlzLlNpemUoKSxcclxuICAgICAgICAgICAgRGF0YURpcmVjdGlvbjogPERhdGFEaXJlY3Rpb25FbnVtPnRoaXMuRGF0YURpcmVjdGlvbigpLFxyXG4gICAgICAgICAgICBFbmRpYW5uZXNzOiA8RW5kaWFubmVzc0VudW0+dGhpcy5FbmRpYW5uZXNzKClcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuRXh0ZW5kTW9kZWwobW9kZWwpXHJcblxyXG4gICAgICAgIHJldHVybiBtb2RlbFxyXG4gICAgfVxyXG59XHJcbiIsImNsYXNzIE9uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsXHJcbntcclxuICAgIHB1YmxpYyBTZXR0aW5nc1RlbXBsYXRlTmFtZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBOZXdNb2R1bGU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxPbmVEYXNNb2R1bGVWaWV3TW9kZWw+ICBcclxuICAgIHB1YmxpYyBNYXhCeXRlczogS25vY2tvdXRPYnNlcnZhYmxlPG51bWJlcj5cclxuICAgIHB1YmxpYyBSZW1haW5pbmdCeXRlczogS25vY2tvdXRPYnNlcnZhYmxlPG51bWJlcj5cclxuICAgIHB1YmxpYyBSZW1haW5pbmdDb3VudDogS25vY2tvdXRPYnNlcnZhYmxlPG51bWJlcj4gICAgXHJcbiAgICBwdWJsaWMgTW9kdWxlU2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxPbmVEYXNNb2R1bGVWaWV3TW9kZWw+XHJcbiAgICBwdWJsaWMgRXJyb3JNZXNzYWdlOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIEhhc0Vycm9yOiBLbm9ja291dENvbXB1dGVkPGJvb2xlYW4+XHJcblxyXG4gICAgcHVibGljIE9uZURhc01vZHVsZVNlbGVjdG9yTW9kZTogS25vY2tvdXRPYnNlcnZhYmxlPE9uZURhc01vZHVsZVNlbGVjdG9yTW9kZUVudW0+XHJcblxyXG4gICAgcHJpdmF0ZSBfb25Nb2R1bGVTZXRDaGFuZ2VkOiBFdmVudERpc3BhdGNoZXI8T25lRGFzTW9kdWxlU2VsZWN0b3JWaWV3TW9kZWwsIE9uZURhc01vZHVsZVZpZXdNb2RlbFtdPlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKG9uZURhc01vZHVsZVNlbGVjdG9yTW9kZTogT25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlRW51bSwgbW9kdWxlU2V0OiBPbmVEYXNNb2R1bGVWaWV3TW9kZWxbXSA9IFtdKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlID0ga28ub2JzZXJ2YWJsZTxPbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtPihvbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGUpXHJcblxyXG4gICAgICAgIHRoaXMuU2V0dGluZ3NUZW1wbGF0ZU5hbWUgPSBrby5vYnNlcnZhYmxlKFwiUHJvamVjdF9PbmVEYXNNb2R1bGVUZW1wbGF0ZVwiKVxyXG4gICAgICAgIHRoaXMuTmV3TW9kdWxlID0ga28ub2JzZXJ2YWJsZTxPbmVEYXNNb2R1bGVWaWV3TW9kZWw+KCk7XHJcbiAgICAgICAgdGhpcy5NYXhCeXRlcyA9IGtvLm9ic2VydmFibGU8bnVtYmVyPihJbmZpbml0eSk7XHJcbiAgICAgICAgdGhpcy5SZW1haW5pbmdCeXRlcyA9IGtvLm9ic2VydmFibGU8bnVtYmVyPihOYU4pO1xyXG4gICAgICAgIHRoaXMuUmVtYWluaW5nQ291bnQgPSBrby5vYnNlcnZhYmxlPG51bWJlcj4oTmFOKTtcclxuICAgICAgICB0aGlzLk1vZHVsZVNldCA9IGtvLm9ic2VydmFibGVBcnJheTxPbmVEYXNNb2R1bGVWaWV3TW9kZWw+KG1vZHVsZVNldCk7XHJcbiAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UgPSBrby5vYnNlcnZhYmxlPHN0cmluZz4oXCJcIilcclxuICAgICAgICB0aGlzLkhhc0Vycm9yID0ga28uY29tcHV0ZWQ8Ym9vbGVhbj4oKCkgPT4gdGhpcy5FcnJvck1lc3NhZ2UoKS5sZW5ndGggPiAwKVxyXG5cclxuICAgICAgICB0aGlzLl9vbk1vZHVsZVNldENoYW5nZWQgPSBuZXcgRXZlbnREaXNwYXRjaGVyPE9uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsLCBPbmVEYXNNb2R1bGVWaWV3TW9kZWxbXT4oKTtcclxuXHJcbiAgICAgICAgdGhpcy5JbnRlcm5hbENyZWF0ZU5ld01vZHVsZSgpXHJcbiAgICAgICAgdGhpcy5JbnRlcm5hbFVwZGF0ZSgpXHJcbiAgICB9XHJcblxyXG4gICAgZ2V0IE9uTW9kdWxlU2V0Q2hhbmdlZCgpOiBJRXZlbnQ8T25lRGFzTW9kdWxlU2VsZWN0b3JWaWV3TW9kZWwsIE9uZURhc01vZHVsZVZpZXdNb2RlbFtdPlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLl9vbk1vZHVsZVNldENoYW5nZWQ7XHJcbiAgICB9XHJcbiAgICBcclxuICAgIC8vIG1ldGhvZHNcclxuICAgIHB1YmxpYyBTZXRNYXhCeXRlcyA9ICh2YWx1ZTogbnVtYmVyKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuTWF4Qnl0ZXModmFsdWUpXHJcbiAgICAgICAgdGhpcy5JbnRlcm5hbFVwZGF0ZSgpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEdldElucHV0TW9kdWxlU2V0ID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5Nb2R1bGVTZXQoKS5maWx0ZXIobW9kdWxlID0+IG1vZHVsZS5EYXRhRGlyZWN0aW9uKCkgPT09IERhdGFEaXJlY3Rpb25FbnVtLklucHV0KVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBHZXRPdXRwdXRNb2R1bGVTZXQgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLk1vZHVsZVNldCgpLmZpbHRlcihtb2R1bGUgPT4gbW9kdWxlLkRhdGFEaXJlY3Rpb24oKSA9PT0gRGF0YURpcmVjdGlvbkVudW0uT3V0cHV0KVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuVXBkYXRlKClcclxuICAgICAgICB0aGlzLlZhbGlkYXRlKClcclxuICAgIH1cclxuXHJcbiAgICBwcm90ZWN0ZWQgVXBkYXRlKClcclxuICAgIHtcclxuICAgICAgICBsZXQgbW9kdWxlU2V0OiBPbmVEYXNNb2R1bGVWaWV3TW9kZWxbXVxyXG4gICAgICAgIGxldCByZW1haW5pbmdCeXRlczogbnVtYmVyXHJcblxyXG4gICAgICAgIHN3aXRjaCAodGhpcy5OZXdNb2R1bGUoKS5EYXRhRGlyZWN0aW9uKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLklucHV0OlxyXG4gICAgICAgICAgICAgICAgbW9kdWxlU2V0ID0gdGhpcy5HZXRJbnB1dE1vZHVsZVNldCgpXHJcbiAgICAgICAgICAgICAgICBicmVhaztcclxuXHJcbiAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uT3V0cHV0OlxyXG4gICAgICAgICAgICAgICAgbW9kdWxlU2V0ID0gdGhpcy5HZXRPdXRwdXRNb2R1bGVTZXQoKVxyXG4gICAgICAgICAgICAgICAgYnJlYWs7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZW1haW5pbmdCeXRlcyA9IHRoaXMuTWF4Qnl0ZXMoKSAtIG1vZHVsZVNldC5tYXAob25lRGFzTW9kdWxlID0+IG9uZURhc01vZHVsZS5HZXRCeXRlQ291bnQoKSkucmVkdWNlKChwcmV2aW91c1ZhbHVlLCBjdXJyZW50VmFsdWUpID0+IHByZXZpb3VzVmFsdWUgKyBjdXJyZW50VmFsdWUsIDApXHJcblxyXG4gICAgICAgIHRoaXMuUmVtYWluaW5nQnl0ZXMocmVtYWluaW5nQnl0ZXMpXHJcbiAgICAgICAgdGhpcy5SZW1haW5pbmdDb3VudChNYXRoLmZsb29yKHRoaXMuUmVtYWluaW5nQnl0ZXMoKSAvICgodGhpcy5OZXdNb2R1bGUoKS5EYXRhVHlwZSgpICYgMHgwRkYpIC8gOCkpKVxyXG4gICAgfVxyXG5cclxuICAgIHByb3RlY3RlZCBWYWxpZGF0ZSgpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJcIilcclxuXHJcbiAgICAgICAgaWYgKHRoaXMuTmV3TW9kdWxlKCkuSGFzRXJyb3IoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiUmVzb2x2ZSBhbGwgcmVtYWluaW5nIG1vZHVsZSBlcnJvcnMgYmVmb3JlIGNvbnRpbnVpbmcuXCIpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBpZiAodGhpcy5PbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGUoKSA9PT0gT25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlRW51bS5JbnB1dE9ubHkgJiYgdGhpcy5OZXdNb2R1bGUoKS5EYXRhRGlyZWN0aW9uKCkgPT0gRGF0YURpcmVjdGlvbkVudW0uT3V0cHV0KVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJPbmx5IGlucHV0IG1vZHVsZXMgYXJlIGFsbG93ZWQuXCIpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBpZiAodGhpcy5PbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGUoKSA9PT0gT25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlRW51bS5PdXRwdXRPbmx5ICYmIHRoaXMuTmV3TW9kdWxlKCkuRGF0YURpcmVjdGlvbigpID09IERhdGFEaXJlY3Rpb25FbnVtLklucHV0KVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJPbmx5IG91dHB1dCBtb2R1bGVzIGFyZSBhbGxvd2VkLlwiKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaWYgKGlzRmluaXRlKHRoaXMuUmVtYWluaW5nQnl0ZXMoKSkgJiYgKHRoaXMuUmVtYWluaW5nQnl0ZXMoKSAtIHRoaXMuTmV3TW9kdWxlKCkuR2V0Qnl0ZUNvdW50KCkgPCAwKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiQnl0ZSBjb3VudCBvZiBuZXcgbW9kdWxlIGlzIHRvbyBoaWdoLlwiKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaWYgKHRoaXMuUmVtYWluaW5nQ291bnQoKSA8PSAwKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJUaGUgbWF4aW11bSBudW1iZXIgb2YgbW9kdWxlcyBpcyByZWFjaGVkLlwiKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwcm90ZWN0ZWQgQ3JlYXRlTmV3TW9kdWxlKClcclxuICAgIHtcclxuICAgICAgICBpZiAodGhpcy5OZXdNb2R1bGUoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHJldHVybiBuZXcgT25lRGFzTW9kdWxlVmlld01vZGVsKG5ldyBPbmVEYXNNb2R1bGVNb2RlbCh0aGlzLk5ld01vZHVsZSgpLkRhdGFUeXBlKCksIHRoaXMuTmV3TW9kdWxlKCkuRGF0YURpcmVjdGlvbigpLCB0aGlzLk5ld01vZHVsZSgpLkVuZGlhbm5lc3MoKSwgMSkpXHJcbiAgICAgICAgfVxyXG4gICAgICAgIGVsc2VcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHJldHVybiBuZXcgT25lRGFzTW9kdWxlVmlld01vZGVsKG5ldyBPbmVEYXNNb2R1bGVNb2RlbChPbmVEYXNEYXRhVHlwZUVudW0uVUlOVDE2LCBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dCwgRW5kaWFubmVzc0VudW0uTGl0dGxlRW5kaWFuLCAxKSlcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBJbnRlcm5hbENyZWF0ZU5ld01vZHVsZSgpXHJcbiAgICB7XHJcbiAgICAgICAgaWYgKHRoaXMuTmV3TW9kdWxlKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLk5ld01vZHVsZSgpLlByb3BlcnR5Q2hhbmdlZC51bnN1YnNjcmliZSh0aGlzLk9uTW9kdWxlUHJvcGVydHlDaGFuZ2VkKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5OZXdNb2R1bGUodGhpcy5DcmVhdGVOZXdNb2R1bGUoKSlcclxuICAgICAgICB0aGlzLk5ld01vZHVsZSgpLlByb3BlcnR5Q2hhbmdlZC5zdWJzY3JpYmUodGhpcy5Pbk1vZHVsZVByb3BlcnR5Q2hhbmdlZClcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIE9uTW9kdWxlUHJvcGVydHlDaGFuZ2VkID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLkludGVybmFsVXBkYXRlKClcclxuICAgIH1cclxuXHJcbiAgICAvLyBjb21tYW5kc1xyXG4gICAgcHVibGljIEFkZE1vZHVsZSA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgbGV0IG5ld01vZHVsZTogT25lRGFzTW9kdWxlVmlld01vZGVsXHJcblxyXG4gICAgICAgIGlmICghdGhpcy5IYXNFcnJvcigpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5Nb2R1bGVTZXQucHVzaCh0aGlzLk5ld01vZHVsZSgpKVxyXG4gICAgICAgICAgICB0aGlzLkludGVybmFsQ3JlYXRlTmV3TW9kdWxlKClcclxuICAgICAgICAgICAgdGhpcy5JbnRlcm5hbFVwZGF0ZSgpXHJcbiAgICAgICAgICAgIHRoaXMuX29uTW9kdWxlU2V0Q2hhbmdlZC5kaXNwYXRjaCh0aGlzLCB0aGlzLk1vZHVsZVNldCgpKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgRGVsZXRlTW9kdWxlID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLk1vZHVsZVNldC5wb3AoKVxyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgICAgIHRoaXMuX29uTW9kdWxlU2V0Q2hhbmdlZC5kaXNwYXRjaCh0aGlzLCB0aGlzLk1vZHVsZVNldCgpKVxyXG4gICAgfVxyXG59IiwiY2xhc3MgVHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgRGF0ZVRpbWU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgVHlwZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBPcHRpb246IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgQXJndW1lbnQ6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcblxyXG4gICAgY29uc3RydWN0b3IodHJhbnNmZXJGdW5jdGlvbk1vZGVsOiBUcmFuc2ZlckZ1bmN0aW9uTW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5EYXRlVGltZSA9IGtvLm9ic2VydmFibGUodHJhbnNmZXJGdW5jdGlvbk1vZGVsLkRhdGVUaW1lKVxyXG4gICAgICAgIHRoaXMuVHlwZSA9IGtvLm9ic2VydmFibGUodHJhbnNmZXJGdW5jdGlvbk1vZGVsLlR5cGUpXHJcbiAgICAgICAgdGhpcy5PcHRpb24gPSBrby5vYnNlcnZhYmxlKHRyYW5zZmVyRnVuY3Rpb25Nb2RlbC5PcHRpb24pXHJcbiAgICAgICAgdGhpcy5Bcmd1bWVudCA9IGtvLm9ic2VydmFibGUodHJhbnNmZXJGdW5jdGlvbk1vZGVsLkFyZ3VtZW50KVxyXG4gICAgfVxyXG5cclxuICAgIC8vIG1ldGhvZHNcclxuICAgIHB1YmxpYyBUb01vZGVsKClcclxuICAgIHtcclxuICAgICAgICByZXR1cm4gbmV3IFRyYW5zZmVyRnVuY3Rpb25Nb2RlbCh0aGlzLkRhdGVUaW1lKCksIHRoaXMuVHlwZSgpLCB0aGlzLk9wdGlvbigpLCB0aGlzLkFyZ3VtZW50KCkpXHJcbiAgICB9XHJcbn0iLCJjbGFzcyBCdWZmZXJSZXF1ZXN0U2VsZWN0b3JWaWV3TW9kZWxcclxue1xyXG4gICAgcHVibGljIE5ld0J1ZmZlclJlcXVlc3Q6IEtub2Nrb3V0T2JzZXJ2YWJsZTxCdWZmZXJSZXF1ZXN0Vmlld01vZGVsPiAgXHJcbiAgICBwdWJsaWMgQnVmZmVyUmVxdWVzdFNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8QnVmZmVyUmVxdWVzdFZpZXdNb2RlbD5cclxuICAgIHB1YmxpYyBFcnJvck1lc3NhZ2U6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgSGFzRXJyb3I6IEtub2Nrb3V0Q29tcHV0ZWQ8Ym9vbGVhbj5cclxuXHJcbiAgICBwcml2YXRlIF9vbkJ1ZmZlclJlcXVlc3RTZXRDaGFuZ2VkOiBFdmVudERpc3BhdGNoZXI8QnVmZmVyUmVxdWVzdFNlbGVjdG9yVmlld01vZGVsLCBCdWZmZXJSZXF1ZXN0Vmlld01vZGVsW10+XHJcblxyXG4gICAgY29uc3RydWN0b3IoYnVmZmVyUmVxdWVzdFNldDogQnVmZmVyUmVxdWVzdFZpZXdNb2RlbFtdID0gW10pXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5OZXdCdWZmZXJSZXF1ZXN0ID0ga28ub2JzZXJ2YWJsZTxCdWZmZXJSZXF1ZXN0Vmlld01vZGVsPigpO1xyXG4gICAgICAgIHRoaXMuQnVmZmVyUmVxdWVzdFNldCA9IGtvLm9ic2VydmFibGVBcnJheTxCdWZmZXJSZXF1ZXN0Vmlld01vZGVsPihidWZmZXJSZXF1ZXN0U2V0KTtcclxuICAgICAgICB0aGlzLkVycm9yTWVzc2FnZSA9IGtvLm9ic2VydmFibGU8c3RyaW5nPihcIlwiKVxyXG4gICAgICAgIHRoaXMuSGFzRXJyb3IgPSBrby5jb21wdXRlZDxib29sZWFuPigoKSA9PiB0aGlzLkVycm9yTWVzc2FnZSgpLmxlbmd0aCA+IDApXHJcblxyXG4gICAgICAgIHRoaXMuX29uQnVmZmVyUmVxdWVzdFNldENoYW5nZWQgPSBuZXcgRXZlbnREaXNwYXRjaGVyPEJ1ZmZlclJlcXVlc3RTZWxlY3RvclZpZXdNb2RlbCwgQnVmZmVyUmVxdWVzdFZpZXdNb2RlbFtdPigpO1xyXG5cclxuICAgICAgICB0aGlzLkludGVybmFsQ3JlYXRlTmV3QnVmZmVyUmVxdWVzdCgpXHJcbiAgICAgICAgdGhpcy5JbnRlcm5hbFVwZGF0ZSgpXHJcbiAgICB9XHJcblxyXG4gICAgZ2V0IE9uQnVmZmVyUmVxdWVzdFNldENoYW5nZWQoKTogSUV2ZW50PEJ1ZmZlclJlcXVlc3RTZWxlY3RvclZpZXdNb2RlbCwgQnVmZmVyUmVxdWVzdFZpZXdNb2RlbFtdPlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLl9vbkJ1ZmZlclJlcXVlc3RTZXRDaGFuZ2VkO1xyXG4gICAgfVxyXG4gICAgXHJcbiAgICAvLyBtZXRob2RzXHJcbiAgICBwcml2YXRlIEludGVybmFsVXBkYXRlKClcclxuICAgIHtcclxuICAgICAgICB0aGlzLlVwZGF0ZSgpXHJcbiAgICAgICAgdGhpcy5WYWxpZGF0ZSgpXHJcbiAgICB9XHJcblxyXG4gICAgcHJvdGVjdGVkIFVwZGF0ZSgpXHJcbiAgICB7XHJcbiAgICAgICAgLy9cclxuICAgIH1cclxuXHJcbiAgICBwcm90ZWN0ZWQgVmFsaWRhdGUoKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiXCIpXHJcblxyXG4gICAgICAgIGlmICh0aGlzLk5ld0J1ZmZlclJlcXVlc3QoKS5IYXNFcnJvcigpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJSZXNvbHZlIGFsbCByZW1haW5pbmcgZXJyb3JzIGJlZm9yZSBjb250aW51aW5nLlwiKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwcm90ZWN0ZWQgQ3JlYXRlTmV3QnVmZmVyUmVxdWVzdCgpXHJcbiAgICB7XHJcbiAgICAgICAgaWYgKHRoaXMuTmV3QnVmZmVyUmVxdWVzdCgpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgcmV0dXJuIG5ldyBCdWZmZXJSZXF1ZXN0Vmlld01vZGVsKG5ldyBCdWZmZXJSZXF1ZXN0TW9kZWwodGhpcy5OZXdCdWZmZXJSZXF1ZXN0KCkuU2FtcGxlUmF0ZSgpLCB0aGlzLk5ld0J1ZmZlclJlcXVlc3QoKS5Hcm91cEZpbHRlcigpKSlcclxuICAgICAgICB9XHJcbiAgICAgICAgZWxzZVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgcmV0dXJuIG5ldyBCdWZmZXJSZXF1ZXN0Vmlld01vZGVsKG5ldyBCdWZmZXJSZXF1ZXN0TW9kZWwoU2FtcGxlUmF0ZUVudW0uU2FtcGxlUmF0ZV8xLCBcIipcIikpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgSW50ZXJuYWxDcmVhdGVOZXdCdWZmZXJSZXF1ZXN0KClcclxuICAgIHtcclxuICAgICAgICBpZiAodGhpcy5OZXdCdWZmZXJSZXF1ZXN0KCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLk5ld0J1ZmZlclJlcXVlc3QoKS5Qcm9wZXJ0eUNoYW5nZWQudW5zdWJzY3JpYmUodGhpcy5PbkJ1ZmZlclJlcXVlc3RQcm9wZXJ0eUNoYW5nZWQpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0aGlzLk5ld0J1ZmZlclJlcXVlc3QodGhpcy5DcmVhdGVOZXdCdWZmZXJSZXF1ZXN0KCkpXHJcbiAgICAgICAgdGhpcy5OZXdCdWZmZXJSZXF1ZXN0KCkuUHJvcGVydHlDaGFuZ2VkLnN1YnNjcmliZSh0aGlzLk9uQnVmZmVyUmVxdWVzdFByb3BlcnR5Q2hhbmdlZClcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIE9uQnVmZmVyUmVxdWVzdFByb3BlcnR5Q2hhbmdlZCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5JbnRlcm5hbFVwZGF0ZSgpXHJcbiAgICB9XHJcblxyXG4gICAgLy8gY29tbWFuZHNcclxuICAgIHB1YmxpYyBBZGRCdWZmZXJSZXF1ZXN0ID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICBsZXQgbmV3QnVmZmVyUmVxdWVzdDogQnVmZmVyUmVxdWVzdFZpZXdNb2RlbFxyXG5cclxuICAgICAgICBpZiAoIXRoaXMuSGFzRXJyb3IoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuQnVmZmVyUmVxdWVzdFNldC5wdXNoKHRoaXMuTmV3QnVmZmVyUmVxdWVzdCgpKVxyXG4gICAgICAgICAgICB0aGlzLkludGVybmFsQ3JlYXRlTmV3QnVmZmVyUmVxdWVzdCgpXHJcbiAgICAgICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgICAgICAgICB0aGlzLl9vbkJ1ZmZlclJlcXVlc3RTZXRDaGFuZ2VkLmRpc3BhdGNoKHRoaXMsIHRoaXMuQnVmZmVyUmVxdWVzdFNldCgpKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgRGVsZXRlQnVmZmVyUmVxdWVzdCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5CdWZmZXJSZXF1ZXN0U2V0LnBvcCgpXHJcbiAgICAgICAgdGhpcy5JbnRlcm5hbFVwZGF0ZSgpXHJcbiAgICAgICAgdGhpcy5fb25CdWZmZXJSZXF1ZXN0U2V0Q2hhbmdlZC5kaXNwYXRjaCh0aGlzLCB0aGlzLkJ1ZmZlclJlcXVlc3RTZXQoKSlcclxuICAgIH1cclxufSIsImNsYXNzIEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWxcclxue1xyXG4gICAgcHVibGljIFNhbXBsZVJhdGU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxTYW1wbGVSYXRlRW51bT5cclxuICAgIHB1YmxpYyBHcm91cEZpbHRlcjogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBFcnJvck1lc3NhZ2U6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgSGFzRXJyb3I6IEtub2Nrb3V0Q29tcHV0ZWQ8Ym9vbGVhbj5cclxuXHJcbiAgICBwdWJsaWMgU2FtcGxlUmF0ZVNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8U2FtcGxlUmF0ZUVudW0+XHJcblxyXG4gICAgcHJpdmF0ZSBfb25Qcm9wZXJ0eUNoYW5nZWQ6IEV2ZW50RGlzcGF0Y2hlcjxCdWZmZXJSZXF1ZXN0Vmlld01vZGVsLCBhbnk+XHJcblxyXG4gICAgY29uc3RydWN0b3IobW9kZWw6IEJ1ZmZlclJlcXVlc3RNb2RlbClcclxuICAgIHtcclxuICAgICAgICB0aGlzLlNhbXBsZVJhdGVTZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8U2FtcGxlUmF0ZUVudW0+KEVudW1lcmF0aW9uSGVscGVyLkdldEVudW1WYWx1ZXMoXCJTYW1wbGVSYXRlRW51bVwiKSlcclxuICAgICAgICB0aGlzLlNhbXBsZVJhdGUgPSBrby5vYnNlcnZhYmxlPFNhbXBsZVJhdGVFbnVtPihtb2RlbC5TYW1wbGVSYXRlKTtcclxuICAgICAgICB0aGlzLkdyb3VwRmlsdGVyID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KG1vZGVsLkdyb3VwRmlsdGVyKTtcclxuXHJcbiAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UgPSBrby5vYnNlcnZhYmxlPHN0cmluZz4oXCJcIilcclxuICAgICAgICB0aGlzLkhhc0Vycm9yID0ga28uY29tcHV0ZWQ8Ym9vbGVhbj4oKCkgPT4gdGhpcy5FcnJvck1lc3NhZ2UoKS5sZW5ndGggPiAwKVxyXG5cclxuICAgICAgICB0aGlzLl9vblByb3BlcnR5Q2hhbmdlZCA9IG5ldyBFdmVudERpc3BhdGNoZXI8QnVmZmVyUmVxdWVzdFZpZXdNb2RlbCwgYW55PigpO1xyXG5cclxuICAgICAgICB0aGlzLlNhbXBsZVJhdGUuc3Vic2NyaWJlKG5ld1ZhbHVlID0+IHRoaXMuT25Qcm9wZXJ0eUNoYW5nZWQoKSlcclxuICAgICAgICB0aGlzLkdyb3VwRmlsdGVyLnN1YnNjcmliZShuZXdWYWx1ZSA9PiB0aGlzLk9uUHJvcGVydHlDaGFuZ2VkKCkpXHJcbiAgICB9XHJcblxyXG4gICAgZ2V0IFByb3BlcnR5Q2hhbmdlZCgpOiBJRXZlbnQ8QnVmZmVyUmVxdWVzdFZpZXdNb2RlbCwgYW55PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLl9vblByb3BlcnR5Q2hhbmdlZDtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgT25Qcm9wZXJ0eUNoYW5nZWQgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuVmFsaWRhdGUoKVxyXG4gICAgICAgIHRoaXMuX29uUHJvcGVydHlDaGFuZ2VkLmRpc3BhdGNoKHRoaXMsIG51bGwpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFZhbGlkYXRlKClcclxuICAgIHtcclxuICAgICAgICBsZXQgcmVzdWx0OiBhbnlcclxuXHJcbiAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJcIilcclxuXHJcbiAgICAgICAgdGhpcy5Hcm91cEZpbHRlcigpLnNwbGl0KFwiO1wiKS5mb3JFYWNoKGdyb3VwRmlsdGVyID0+XHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICByZXN1bHQgPSB0aGlzLkNoZWNrR3JvdXBGaWx0ZXIoZ3JvdXBGaWx0ZXIpXHJcblxyXG4gICAgICAgICAgICBpZiAocmVzdWx0Lkhhc0Vycm9yKVxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShyZXN1bHQuRXJyb3JEZXNjcmlwdGlvbilcclxuXHJcbiAgICAgICAgICAgICAgICByZXR1cm5cclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH0pXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvU3RyaW5nKClcclxuICAgIHtcclxuICAgICAgICByZXR1cm4gXCJzYW1wbGUgcmF0ZTogXCIgKyBFbnVtZXJhdGlvbkhlbHBlci5HZXRFbnVtTG9jYWxpemF0aW9uKFwiU2FtcGxlUmF0ZUVudW1cIiwgdGhpcy5TYW1wbGVSYXRlKCkpICsgXCIgLyBncm91cCBmaWx0ZXI6ICdcIiArIHRoaXMuR3JvdXBGaWx0ZXIoKSArIFwiJ1wiXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvTW9kZWwoKVxyXG4gICAge1xyXG4gICAgICAgIGxldCBtb2RlbDogYW55ID0ge1xyXG4gICAgICAgICAgICBTYW1wbGVSYXRlOiA8U2FtcGxlUmF0ZUVudW0+dGhpcy5TYW1wbGVSYXRlKCksXHJcbiAgICAgICAgICAgIEdyb3VwRmlsdGVyOiA8c3RyaW5nPnRoaXMuR3JvdXBGaWx0ZXIoKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmV0dXJuIG1vZGVsXHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBDaGVja0dyb3VwRmlsdGVyKHZhbHVlOiBzdHJpbmcpXHJcbiAgICB7XHJcbiAgICAgICAgdmFyIHJlZ0V4cDogYW55XHJcblxyXG4gICAgICAgIGlmICh2YWx1ZS5sZW5ndGggPT09IDApIHtcclxuICAgICAgICAgICAgcmV0dXJuIHsgSGFzRXJyb3I6IHRydWUsIEVycm9yRGVzY3JpcHRpb246IEVycm9yTWVzc2FnZVtcIlByb2plY3RfR3JvdXBGaWx0ZXJFbXB0eVwiXSB9XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZWdFeHAgPSBuZXcgUmVnRXhwKFwiW15BLVphLXowLTlfISpdXCIpXHJcblxyXG4gICAgICAgIGlmIChyZWdFeHAudGVzdCh2YWx1ZSkpIHtcclxuICAgICAgICAgICAgcmV0dXJuIHsgSGFzRXJyb3I6IHRydWUsIEVycm9yRGVzY3JpcHRpb246IEVycm9yTWVzc2FnZVtcIlByb2plY3RfSW52YWxpZENoYXJhY3RlcnNcIl0gfVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmVnRXhwID0gbmV3IFJlZ0V4cChcIl5bMC05X11cIilcclxuXHJcbiAgICAgICAgaWYgKHJlZ0V4cC50ZXN0KHZhbHVlKSkge1xyXG4gICAgICAgICAgICByZXR1cm4geyBIYXNFcnJvcjogdHJ1ZSwgRXJyb3JEZXNjcmlwdGlvbjogRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9JbnZhbGlkTGVhZGluZ0NoYXJhY3RlclwiXSB9XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZWdFeHAgPSBuZXcgUmVnRXhwKFwiXiFcIilcclxuXHJcbiAgICAgICAgaWYgKHJlZ0V4cC50ZXN0KHZhbHVlKSAmJiB2YWx1ZS5sZW5ndGggPT09IDEpIHtcclxuICAgICAgICAgICAgcmV0dXJuIHsgSGFzRXJyb3I6IHRydWUsIEVycm9yRGVzY3JpcHRpb246IEVycm9yTWVzc2FnZVtcIlByb2plY3RfRGV0YWNoZWRFeGNsYW1hdGlvbk1hcmtOb3RBbGxvd2VkXCJdIH1cclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJldHVybiB7XHJcbiAgICAgICAgICAgIEhhc0Vycm9yOiBmYWxzZSxcclxuICAgICAgICAgICAgRXJyb3JEZXNjcmlwdGlvbjogXCJcIlxyXG4gICAgICAgIH1cclxuICAgIH1cclxufSIsImNsYXNzIERhdGFQb3J0Vmlld01vZGVsXHJcbntcclxuICAgIC8vIGZpZWxkc1xyXG4gICAgcHVibGljIE5hbWU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRGF0YVR5cGU6IE9uZURhc0RhdGFUeXBlRW51bVxyXG4gICAgcHVibGljIHJlYWRvbmx5IERhdGFEaXJlY3Rpb246IERhdGFEaXJlY3Rpb25FbnVtXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRW5kaWFubmVzczogRW5kaWFubmVzc0VudW1cclxuXHJcbiAgICBwdWJsaWMgSXNTZWxlY3RlZDogS25vY2tvdXRPYnNlcnZhYmxlPGJvb2xlYW4+XHJcbiAgICBwdWJsaWMgQXNzb2NpYXRlZENoYW5uZWxIdWJTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PENoYW5uZWxIdWJWaWV3TW9kZWw+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgQXNzb2NpYXRlZERhdGFHYXRld2F5OiBEYXRhR2F0ZXdheVZpZXdNb2RlbEJhc2VcclxuICAgIHB1YmxpYyByZWFkb25seSBMaXZlRGVzY3JpcHRpb246IEtub2Nrb3V0Q29tcHV0ZWQ8c3RyaW5nPlxyXG5cclxuICAgIC8vIGNvbnN0cnVjdG9yc1xyXG4gICAgY29uc3RydWN0b3IoZGF0YVBvcnRNb2RlbDogYW55LCBhc3NvY2lhdGVkRGF0YUdhdGV3YXk6IERhdGFHYXRld2F5Vmlld01vZGVsQmFzZSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLk5hbWUgPSBrby5vYnNlcnZhYmxlKGRhdGFQb3J0TW9kZWwuTmFtZSlcclxuICAgICAgICB0aGlzLkRhdGFUeXBlID0gZGF0YVBvcnRNb2RlbC5EYXRhVHlwZVxyXG4gICAgICAgIHRoaXMuRGF0YURpcmVjdGlvbiA9IGRhdGFQb3J0TW9kZWwuRGF0YURpcmVjdGlvblxyXG4gICAgICAgIHRoaXMuRW5kaWFubmVzcyA9IGRhdGFQb3J0TW9kZWwuRW5kaWFubmVzc1xyXG5cclxuICAgICAgICB0aGlzLklzU2VsZWN0ZWQgPSBrby5vYnNlcnZhYmxlPGJvb2xlYW4+KGZhbHNlKVxyXG4gICAgICAgIHRoaXMuQXNzb2NpYXRlZENoYW5uZWxIdWJTZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8Q2hhbm5lbEh1YlZpZXdNb2RlbD4oKVxyXG4gICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFHYXRld2F5ID0gYXNzb2NpYXRlZERhdGFHYXRld2F5XHJcblxyXG4gICAgICAgIHRoaXMuTGl2ZURlc2NyaXB0aW9uID0ga28uY29tcHV0ZWQoKCkgPT5cclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGxldCByZXN1bHQ6IHN0cmluZ1xyXG5cclxuICAgICAgICAgICAgcmVzdWx0ID0gXCI8ZGl2IGNsYXNzPSd0ZXh0LWxlZnQnPlwiICsgdGhpcy5OYW1lKCkgKyBcIjwvZGl2PjxkaXYgY2xhc3M9J3RleHQtbGVmdCc+XCIgKyBFbnVtZXJhdGlvbkhlbHBlci5HZXRFbnVtTG9jYWxpemF0aW9uKCdPbmVEYXNEYXRhVHlwZUVudW0nLCB0aGlzLkRhdGFUeXBlKSArIFwiPC9kaXY+XCJcclxuXHJcbiAgICAgICAgICAgIGlmICh0aGlzLkFzc29jaWF0ZWRDaGFubmVsSHViU2V0KCkubGVuZ3RoID4gMClcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkQ2hhbm5lbEh1YlNldCgpLmZvckVhY2goY2hhbm5lbEh1YiA9PlxyXG4gICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgIHJlc3VsdCArPSBcIjwvYnIgPjxkaXYgY2xhc3M9J3RleHQtbGVmdCc+XCIgKyBjaGFubmVsSHViLk5hbWUoKSArIFwiPC9kaXY+PGRpdiBjbGFzcz0ndGV4dC1sZWZ0Jz5cIiArIEVudW1lcmF0aW9uSGVscGVyLkdldEVudW1Mb2NhbGl6YXRpb24oJ09uZURhc0RhdGFUeXBlRW51bScsIGNoYW5uZWxIdWIuRGF0YVR5cGUoKSkgKyBcIjwvZGl2PlwiXHJcbiAgICAgICAgICAgICAgICB9KVxyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICByZXR1cm4gcmVzdWx0XHJcbiAgICAgICAgfSlcclxuICAgIH1cclxuXHJcbiAgICAvLyBtZXRob2RzXHJcbiAgICBwdWJsaWMgR2V0SWQoKTogc3RyaW5nXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuTmFtZSgpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvRnVsbFF1YWxpZmllZElkZW50aWZpZXIoKTogc3RyaW5nXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuQXNzb2NpYXRlZERhdGFHYXRld2F5LkRlc2NyaXB0aW9uLklkICsgXCIgKFwiICsgdGhpcy5Bc3NvY2lhdGVkRGF0YUdhdGV3YXkuRGVzY3JpcHRpb24uSW5zdGFuY2VJZCArIFwiKSAvIFwiICsgdGhpcy5HZXRJZCgpO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBFeHRlbmRNb2RlbChtb2RlbDogYW55KVxyXG4gICAge1xyXG4gICAgICAgIC8vXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvTW9kZWwoKVxyXG4gICAge1xyXG4gICAgICAgIGxldCBtb2RlbDogYW55ID0ge1xyXG4gICAgICAgICAgICBOYW1lOiA8c3RyaW5nPnRoaXMuTmFtZSgpLFxyXG4gICAgICAgICAgICBEYXRhVHlwZTogPE9uZURhc0RhdGFUeXBlRW51bT50aGlzLkRhdGFUeXBlLFxyXG4gICAgICAgICAgICBEYXRhRGlyZWN0aW9uOiA8RGF0YURpcmVjdGlvbkVudW0+dGhpcy5EYXRhRGlyZWN0aW9uXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0aGlzLkV4dGVuZE1vZGVsKG1vZGVsKVxyXG5cclxuICAgICAgICByZXR1cm4gbW9kZWxcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgUmVzZXRBc3NvY2lhdGlvbnMobWFpbnRhaW5XZWFrUmVmZXJlbmNlOiBib29sZWFuKVxyXG4gICAge1xyXG4gICAgICAgIGlmICh0aGlzLkFzc29jaWF0ZWRDaGFubmVsSHViU2V0KCkubGVuZ3RoID4gMClcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZENoYW5uZWxIdWJTZXQoKS5mb3JFYWNoKGNoYW5uZWxIdWIgPT4gY2hhbm5lbEh1Yi5SZXNldEFzc29jaWF0aW9uKG1haW50YWluV2Vha1JlZmVyZW5jZSwgdGhpcykpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG59IiwiYWJzdHJhY3QgY2xhc3MgRXh0ZW5zaW9uVmlld01vZGVsQmFzZVxyXG57XHJcbiAgICBwdWJsaWMgRGVzY3JpcHRpb246IEV4dGVuc2lvbkRlc2NyaXB0aW9uVmlld01vZGVsXHJcbiAgICBwdWJsaWMgRXh0ZW5zaW9uSWRlbnRpZmljYXRpb246IEV4dGVuc2lvbklkZW50aWZpY2F0aW9uVmlld01vZGVsXHJcbiAgICBwdWJsaWMgSXNJblNldHRpbmdzTW9kZTogS25vY2tvdXRPYnNlcnZhYmxlPGJvb2xlYW4+XHJcblxyXG4gICAgcHJpdmF0ZSBfbW9kZWw6IGFueVxyXG5cclxuICAgIGNvbnN0cnVjdG9yKGV4dGVuc2lvblNldHRpbmdzTW9kZWw6IGFueSwgZXh0ZW5zaW9uSWRlbnRpZmljYXRpb246IEV4dGVuc2lvbklkZW50aWZpY2F0aW9uVmlld01vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuX21vZGVsID0gZXh0ZW5zaW9uU2V0dGluZ3NNb2RlbFxyXG4gICAgICAgIHRoaXMuRGVzY3JpcHRpb24gPSBuZXcgRXh0ZW5zaW9uRGVzY3JpcHRpb25WaWV3TW9kZWwoZXh0ZW5zaW9uU2V0dGluZ3NNb2RlbC5EZXNjcmlwdGlvbilcclxuICAgICAgICB0aGlzLkV4dGVuc2lvbklkZW50aWZpY2F0aW9uID0gZXh0ZW5zaW9uSWRlbnRpZmljYXRpb25cclxuICAgICAgICB0aGlzLklzSW5TZXR0aW5nc01vZGUgPSBrby5vYnNlcnZhYmxlPGJvb2xlYW4+KGZhbHNlKVxyXG4gICAgfVxyXG5cclxuICAgIC8vIG1ldGhvZHNcclxuICAgIHB1YmxpYyBhYnN0cmFjdCBhc3luYyBJbml0aWFsaXplQXN5bmMoKTogUHJvbWlzZTxhbnk+XHJcblxyXG4gICAgcHVibGljIFNlbmRBY3Rpb25SZXF1ZXN0ID0gYXN5bmMgKGluc3RhbmNlSWQ6IG51bWJlciwgbWV0aG9kTmFtZTogc3RyaW5nLCBkYXRhOiBhbnkpID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIDxBY3Rpb25SZXNwb25zZT4gYXdhaXQgQ29ubmVjdGlvbk1hbmFnZXIuSW52b2tlV2ViQ2xpZW50SHViKFwiUmVxdWVzdEFjdGlvblwiLCBuZXcgQWN0aW9uUmVxdWVzdCh0aGlzLkRlc2NyaXB0aW9uLklkLCBpbnN0YW5jZUlkLCBtZXRob2ROYW1lLCBkYXRhKSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgRXh0ZW5kTW9kZWwobW9kZWw6IGFueSlcclxuICAgIHtcclxuICAgICAgICAvL1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb01vZGVsKClcclxuICAgIHtcclxuICAgICAgICBsZXQgbW9kZWw6IGFueSA9IHtcclxuICAgICAgICAgICAgJHR5cGU6IDxzdHJpbmc+dGhpcy5fbW9kZWwuJHR5cGUsXHJcbiAgICAgICAgICAgIERlc2NyaXB0aW9uOiA8RXh0ZW5zaW9uRGVzY3JpcHRpb25WaWV3TW9kZWw+dGhpcy5EZXNjcmlwdGlvbi5Ub01vZGVsKClcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuRXh0ZW5kTW9kZWwobW9kZWwpXHJcblxyXG4gICAgICAgIHJldHVybiBtb2RlbFxyXG4gICAgfVxyXG5cclxuICAgIC8vIGNvbW1hbmRzXHJcbiAgICBwdWJsaWMgRW5hYmxlU2V0dGluZ3NNb2RlID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLklzSW5TZXR0aW5nc01vZGUodHJ1ZSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgRGlzYWJsZVNldHRpbmdzTW9kZSA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5Jc0luU2V0dGluZ3NNb2RlKGZhbHNlKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb2dnbGVTZXR0aW5nc01vZGUgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuSXNJblNldHRpbmdzTW9kZSghdGhpcy5Jc0luU2V0dGluZ3NNb2RlKCkpXHJcbiAgICB9XHJcbn0iLCIvLy8gPHJlZmVyZW5jZSBwYXRoPVwiRXh0ZW5zaW9uVmlld01vZGVsQmFzZS50c1wiLz5cclxuXHJcbmFic3RyYWN0IGNsYXNzIERhdGFHYXRld2F5Vmlld01vZGVsQmFzZSBleHRlbmRzIEV4dGVuc2lvblZpZXdNb2RlbEJhc2Vcclxue1xyXG4gICAgcHVibGljIHJlYWRvbmx5IE1heGltdW1EYXRhc2V0QWdlOiBLbm9ja291dE9ic2VydmFibGU8bnVtYmVyPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IERhdGFQb3J0U2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxEYXRhUG9ydFZpZXdNb2RlbD5cclxuXHJcbiAgICBjb25zdHJ1Y3Rvcihtb2RlbCwgaWRlbnRpZmljYXRpb246IEV4dGVuc2lvbklkZW50aWZpY2F0aW9uVmlld01vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHN1cGVyKG1vZGVsLCBpZGVudGlmaWNhdGlvbilcclxuXHJcbiAgICAgICAgdGhpcy5NYXhpbXVtRGF0YXNldEFnZSA9IGtvLm9ic2VydmFibGU8bnVtYmVyPihtb2RlbC5NYXhpbXVtRGF0YXNldEFnZSlcclxuICAgICAgICB0aGlzLkRhdGFQb3J0U2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PERhdGFQb3J0Vmlld01vZGVsPigpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEV4dGVuZE1vZGVsKG1vZGVsOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgc3VwZXIuRXh0ZW5kTW9kZWwobW9kZWwpXHJcblxyXG4gICAgICAgIG1vZGVsLk1heGltdW1EYXRhc2V0QWdlID0gPG51bWJlcj5OdW1iZXIucGFyc2VJbnQoPGFueT50aGlzLk1heGltdW1EYXRhc2V0QWdlKCkpXHJcbiAgICB9XHJcbn0iLCJhYnN0cmFjdCBjbGFzcyBFeHRlbmRlZERhdGFHYXRld2F5Vmlld01vZGVsQmFzZSBleHRlbmRzIERhdGFHYXRld2F5Vmlld01vZGVsQmFzZVxyXG57XHJcbiAgICBwdWJsaWMgTW9kdWxlVG9EYXRhUG9ydE1hcDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8T2JzZXJ2YWJsZUdyb3VwPERhdGFQb3J0Vmlld01vZGVsPj5cclxuICAgIHB1YmxpYyBPbmVEYXNNb2R1bGVTZWxlY3RvcjogS25vY2tvdXRPYnNlcnZhYmxlPE9uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsPlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKG1vZGVsLCBpZGVudGlmaWNhdGlvbjogRXh0ZW5zaW9uSWRlbnRpZmljYXRpb25WaWV3TW9kZWwsIG9uZURhc01vZHVsZVNlbGVjdG9yOiBPbmVEYXNNb2R1bGVTZWxlY3RvclZpZXdNb2RlbClcclxuICAgIHtcclxuICAgICAgICBzdXBlcihtb2RlbCwgaWRlbnRpZmljYXRpb24pXHJcblxyXG4gICAgICAgIHRoaXMuTW9kdWxlVG9EYXRhUG9ydE1hcCA9IGtvLm9ic2VydmFibGVBcnJheSgpXHJcbiAgICAgICAgdGhpcy5PbmVEYXNNb2R1bGVTZWxlY3RvciA9IGtvLm9ic2VydmFibGU8T25lRGFzTW9kdWxlU2VsZWN0b3JWaWV3TW9kZWw+KG9uZURhc01vZHVsZVNlbGVjdG9yKVxyXG5cclxuICAgICAgICBpZiAodGhpcy5PbmVEYXNNb2R1bGVTZWxlY3RvcigpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5PbmVEYXNNb2R1bGVTZWxlY3RvcigpLk9uTW9kdWxlU2V0Q2hhbmdlZC5zdWJzY3JpYmUoKHNlbmRlciwgYXJncykgPT5cclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgdGhpcy5VcGRhdGVEYXRhUG9ydFNldCgpXHJcbiAgICAgICAgICAgIH0pXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBhc3luYyBJbml0aWFsaXplQXN5bmMoKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuVXBkYXRlRGF0YVBvcnRTZXQoKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBVcGRhdGVEYXRhUG9ydFNldCgpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IGluZGV4OiBudW1iZXJcclxuICAgICAgICBsZXQgbW9kdWxlVG9EYXRhUG9ydE1hcDogT2JzZXJ2YWJsZUdyb3VwPERhdGFQb3J0Vmlld01vZGVsPltdXHJcblxyXG4gICAgICAgIG1vZHVsZVRvRGF0YVBvcnRNYXAgPSBbXVxyXG5cclxuICAgICAgICAvLyBpbnB1dHNcclxuICAgICAgICBpbmRleCA9IDBcclxuXHJcbiAgICAgICAgbW9kdWxlVG9EYXRhUG9ydE1hcCA9IG1vZHVsZVRvRGF0YVBvcnRNYXAuY29uY2F0KHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3IoKS5Nb2R1bGVTZXQoKS5maWx0ZXIob25lRGFzTW9kdWxlID0+IG9uZURhc01vZHVsZS5EYXRhRGlyZWN0aW9uKCkgPT09IERhdGFEaXJlY3Rpb25FbnVtLklucHV0KS5tYXAob25lRGFzTW9kdWxlID0+XHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBsZXQgZ3JvdXA6IE9ic2VydmFibGVHcm91cDxEYXRhUG9ydFZpZXdNb2RlbD5cclxuXHJcbiAgICAgICAgICAgIGdyb3VwID0gbmV3IE9ic2VydmFibGVHcm91cDxEYXRhUG9ydFZpZXdNb2RlbD4ob25lRGFzTW9kdWxlLlRvU3RyaW5nKCksIHRoaXMuQ3JlYXRlRGF0YVBvcnRTZXQob25lRGFzTW9kdWxlLCBpbmRleCkpXHJcbiAgICAgICAgICAgIGluZGV4ICs9IG9uZURhc01vZHVsZS5TaXplKCk7XHJcblxyXG4gICAgICAgICAgICByZXR1cm4gZ3JvdXBcclxuICAgICAgICB9KSlcclxuXHJcbiAgICAgICAgLy8gb3V0cHV0c1xyXG4gICAgICAgIGluZGV4ID0gMFxyXG5cclxuICAgICAgICBtb2R1bGVUb0RhdGFQb3J0TWFwID0gbW9kdWxlVG9EYXRhUG9ydE1hcC5jb25jYXQodGhpcy5PbmVEYXNNb2R1bGVTZWxlY3RvcigpLk1vZHVsZVNldCgpLmZpbHRlcihvbmVEYXNNb2R1bGUgPT4gb25lRGFzTW9kdWxlLkRhdGFEaXJlY3Rpb24oKSA9PT0gRGF0YURpcmVjdGlvbkVudW0uT3V0cHV0KS5tYXAob25lRGFzTW9kdWxlID0+XHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBsZXQgZ3JvdXA6IE9ic2VydmFibGVHcm91cDxEYXRhUG9ydFZpZXdNb2RlbD5cclxuXHJcbiAgICAgICAgICAgIGdyb3VwID0gbmV3IE9ic2VydmFibGVHcm91cDxEYXRhUG9ydFZpZXdNb2RlbD4ob25lRGFzTW9kdWxlLlRvU3RyaW5nKCksIHRoaXMuQ3JlYXRlRGF0YVBvcnRTZXQob25lRGFzTW9kdWxlLCBpbmRleCkpXHJcbiAgICAgICAgICAgIGluZGV4ICs9IG9uZURhc01vZHVsZS5TaXplKCk7XHJcblxyXG4gICAgICAgICAgICByZXR1cm4gZ3JvdXBcclxuICAgICAgICB9KSlcclxuXHJcbiAgICAgICAgdGhpcy5Nb2R1bGVUb0RhdGFQb3J0TWFwKG1vZHVsZVRvRGF0YVBvcnRNYXApXHJcbiAgICAgICAgdGhpcy5EYXRhUG9ydFNldChNYXBNYW55KHRoaXMuTW9kdWxlVG9EYXRhUG9ydE1hcCgpLCBncm91cCA9PiBncm91cC5NZW1iZXJzKCkpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBDcmVhdGVEYXRhUG9ydFNldChvbmVEYXNNb2R1bGU6IE9uZURhc01vZHVsZVZpZXdNb2RlbCwgaW5kZXg6IG51bWJlcilcclxuICAgIHtcclxuICAgICAgICBsZXQgcHJlZml4OiBzdHJpbmdcclxuXHJcbiAgICAgICAgc3dpdGNoIChvbmVEYXNNb2R1bGUuRGF0YURpcmVjdGlvbigpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dDpcclxuICAgICAgICAgICAgICAgIHByZWZpeCA9IFwiSW5wdXRcIlxyXG4gICAgICAgICAgICAgICAgYnJlYWtcclxuXHJcbiAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uT3V0cHV0OlxyXG4gICAgICAgICAgICAgICAgcHJlZml4ID0gXCJPdXRwdXRcIlxyXG4gICAgICAgICAgICAgICAgYnJlYWtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJldHVybiBBcnJheS5mcm9tKG5ldyBBcnJheShvbmVEYXNNb2R1bGUuU2l6ZSgpKSwgKHgsIGkpID0+IFxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgcmV0dXJuIHtcclxuICAgICAgICAgICAgICAgIE5hbWU6IDxzdHJpbmc+cHJlZml4ICsgXCIgXCIgKyAoaW5kZXggKyBpKSxcclxuICAgICAgICAgICAgICAgIERhdGFUeXBlOiA8T25lRGFzRGF0YVR5cGVFbnVtPm9uZURhc01vZHVsZS5EYXRhVHlwZSgpLFxyXG4gICAgICAgICAgICAgICAgRGF0YURpcmVjdGlvbjogPERhdGFEaXJlY3Rpb25FbnVtPm9uZURhc01vZHVsZS5EYXRhRGlyZWN0aW9uKClcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH0pLm1hcChkYXRhUG9ydE1vZGVsID0+IG5ldyBEYXRhUG9ydFZpZXdNb2RlbChkYXRhUG9ydE1vZGVsLCB0aGlzKSlcclxuICAgIH1cclxufSIsIi8vLyA8cmVmZXJlbmNlIHBhdGg9XCJFeHRlbnNpb25WaWV3TW9kZWxCYXNlLnRzXCIvPlxyXG5cclxuYWJzdHJhY3QgY2xhc3MgRGF0YVdyaXRlclZpZXdNb2RlbEJhc2UgZXh0ZW5kcyBFeHRlbnNpb25WaWV3TW9kZWxCYXNlXHJcbntcclxuICAgIHB1YmxpYyByZWFkb25seSBGaWxlR3JhbnVsYXJpdHk6IEtub2Nrb3V0T2JzZXJ2YWJsZTxGaWxlR3JhbnVsYXJpdHlFbnVtPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IEJ1ZmZlclJlcXVlc3RTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWw+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgQnVmZmVyUmVxdWVzdFNlbGVjdG9yOiBLbm9ja291dE9ic2VydmFibGU8QnVmZmVyUmVxdWVzdFNlbGVjdG9yVmlld01vZGVsPlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKG1vZGVsLCBpZGVudGlmaWNhdGlvbjogRXh0ZW5zaW9uSWRlbnRpZmljYXRpb25WaWV3TW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgc3VwZXIobW9kZWwsIGlkZW50aWZpY2F0aW9uKVxyXG5cclxuICAgICAgICB0aGlzLkZpbGVHcmFudWxhcml0eSA9IGtvLm9ic2VydmFibGU8RmlsZUdyYW51bGFyaXR5RW51bT4obW9kZWwuRmlsZUdyYW51bGFyaXR5KVxyXG4gICAgICAgIHRoaXMuQnVmZmVyUmVxdWVzdFNldCA9IGtvLm9ic2VydmFibGVBcnJheTxCdWZmZXJSZXF1ZXN0Vmlld01vZGVsPihtb2RlbC5CdWZmZXJSZXF1ZXN0U2V0Lm1hcChidWZmZXJSZXF1ZXN0ID0+IG5ldyBCdWZmZXJSZXF1ZXN0Vmlld01vZGVsKGJ1ZmZlclJlcXVlc3QpKSlcclxuXHJcbiAgICAgICAgdGhpcy5CdWZmZXJSZXF1ZXN0U2VsZWN0b3IgPSBrby5vYnNlcnZhYmxlPEJ1ZmZlclJlcXVlc3RTZWxlY3RvclZpZXdNb2RlbD4obmV3IEJ1ZmZlclJlcXVlc3RTZWxlY3RvclZpZXdNb2RlbCh0aGlzLkJ1ZmZlclJlcXVlc3RTZXQoKSkpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEV4dGVuZE1vZGVsKG1vZGVsOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgc3VwZXIuRXh0ZW5kTW9kZWwobW9kZWwpXHJcblxyXG4gICAgICAgIG1vZGVsLkZpbGVHcmFudWxhcml0eSA9IDxGaWxlR3JhbnVsYXJpdHlFbnVtPnRoaXMuRmlsZUdyYW51bGFyaXR5KClcclxuICAgICAgICBtb2RlbC5CdWZmZXJSZXF1ZXN0U2V0ID0gPEJ1ZmZlclJlcXVlc3RNb2RlbFtdPnRoaXMuQnVmZmVyUmVxdWVzdFNldCgpLm1hcChidWZmZXJSZXF1ZXN0ID0+IGJ1ZmZlclJlcXVlc3QuVG9Nb2RlbCgpKVxyXG4gICAgfVxyXG59IiwiY2xhc3MgRXh0ZW5zaW9uRGVzY3JpcHRpb25WaWV3TW9kZWxcclxue1xyXG4gICAgcHVibGljIFByb2R1Y3RWZXJzaW9uOiBudW1iZXJcclxuICAgIHB1YmxpYyBJZDogc3RyaW5nXHJcbiAgICBwdWJsaWMgSW5zdGFuY2VJZDogbnVtYmVyXHJcbiAgICBwdWJsaWMgSW5zdGFuY2VOYW1lOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIElzRW5hYmxlZDogS25vY2tvdXRPYnNlcnZhYmxlPGJvb2xlYW4+XHJcblxyXG4gICAgY29uc3RydWN0b3IoZXh0ZW5zaW9uRGVzY3JpcHRpb25Nb2RlbDogYW55KVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuUHJvZHVjdFZlcnNpb24gPSBleHRlbnNpb25EZXNjcmlwdGlvbk1vZGVsLlByb2R1Y3RWZXJzaW9uXHJcbiAgICAgICAgdGhpcy5JZCA9IGV4dGVuc2lvbkRlc2NyaXB0aW9uTW9kZWwuSWRcclxuICAgICAgICB0aGlzLkluc3RhbmNlSWQgPSBleHRlbnNpb25EZXNjcmlwdGlvbk1vZGVsLkluc3RhbmNlSWRcclxuICAgICAgICB0aGlzLkluc3RhbmNlTmFtZSA9IGtvLm9ic2VydmFibGU8c3RyaW5nPihleHRlbnNpb25EZXNjcmlwdGlvbk1vZGVsLkluc3RhbmNlTmFtZSlcclxuICAgICAgICB0aGlzLklzRW5hYmxlZCA9IGtvLm9ic2VydmFibGU8Ym9vbGVhbj4oZXh0ZW5zaW9uRGVzY3JpcHRpb25Nb2RlbC5Jc0VuYWJsZWQpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvTW9kZWwoKVxyXG4gICAge1xyXG4gICAgICAgIHZhciBtb2RlbDogYW55ID0ge1xyXG4gICAgICAgICAgICBQcm9kdWN0VmVyc2lvbjogPG51bWJlcj50aGlzLlByb2R1Y3RWZXJzaW9uLFxyXG4gICAgICAgICAgICBJZDogPHN0cmluZz50aGlzLklkLFxyXG4gICAgICAgICAgICBJbnN0YW5jZUlkOiA8bnVtYmVyPnRoaXMuSW5zdGFuY2VJZCxcclxuICAgICAgICAgICAgSW5zdGFuY2VOYW1lOiA8c3RyaW5nPnRoaXMuSW5zdGFuY2VOYW1lKCksXHJcbiAgICAgICAgICAgIElzRW5hYmxlZDogPGJvb2xlYW4+dGhpcy5Jc0VuYWJsZWQoKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmV0dXJuIG1vZGVsXHJcbiAgICB9XHJcbn0iLCJjbGFzcyBFeHRlbnNpb25JZGVudGlmaWNhdGlvblZpZXdNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgUHJvZHVjdFZlcnNpb246IHN0cmluZ1xyXG4gICAgcHVibGljIElkOiBzdHJpbmdcclxuICAgIHB1YmxpYyBOYW1lOiBzdHJpbmdcclxuICAgIHB1YmxpYyBEZXNjcmlwdGlvbjogc3RyaW5nXHJcbiAgICBwdWJsaWMgVmlld1Jlc291cmNlTmFtZTogc3RyaW5nXHJcbiAgICBwdWJsaWMgVmlld01vZGVsUmVzb3VyY2VOYW1lOiBzdHJpbmdcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihleHRlbnNpb25JZGVudGlmaWNhdGlvbk1vZGVsOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5Qcm9kdWN0VmVyc2lvbiA9IGV4dGVuc2lvbklkZW50aWZpY2F0aW9uTW9kZWwuUHJvZHVjdFZlcnNpb25cclxuICAgICAgICB0aGlzLklkID0gZXh0ZW5zaW9uSWRlbnRpZmljYXRpb25Nb2RlbC5JZFxyXG4gICAgICAgIHRoaXMuTmFtZSA9IGV4dGVuc2lvbklkZW50aWZpY2F0aW9uTW9kZWwuTmFtZVxyXG4gICAgICAgIHRoaXMuRGVzY3JpcHRpb24gPSBleHRlbnNpb25JZGVudGlmaWNhdGlvbk1vZGVsLkRlc2NyaXB0aW9uXHJcbiAgICAgICAgdGhpcy5WaWV3UmVzb3VyY2VOYW1lID0gZXh0ZW5zaW9uSWRlbnRpZmljYXRpb25Nb2RlbC5WaWV3UmVzb3VyY2VOYW1lXHJcbiAgICAgICAgdGhpcy5WaWV3TW9kZWxSZXNvdXJjZU5hbWUgPSBleHRlbnNpb25JZGVudGlmaWNhdGlvbk1vZGVsLlZpZXdNb2RlbFJlc291cmNlTmFtZVxyXG4gICAgfVxyXG59Il19