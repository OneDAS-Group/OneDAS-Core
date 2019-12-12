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
    OneDasDataTypeEnum[OneDasDataTypeEnum["UINT64"] = 320] = "UINT64";
    OneDasDataTypeEnum[OneDasDataTypeEnum["INT64"] = 576] = "INT64";
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
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class ConnectionManager {
    static Initialize(enableLogging) {
        ConnectionManager.WebClientHub = new signalR.HubConnectionBuilder()
            .configureLogging(signalR.LogLevel.Information)
            .withUrl('/webclienthub')
            .build();
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
        this.DataType.subscribe(_ => this.OnPropertyChanged());
        this.DataDirection.subscribe(_ => this.OnPropertyChanged());
        this.Size.subscribe(_ => this.OnPropertyChanged());
    }
    get PropertyChanged() {
        return this._onPropertyChanged;
    }
    Validate() {
        this.ErrorMessage("");
        if (this.Size() < 1 || (isFinite(this.MaxSize()) && this.GetByteCount() > this.MaxSize())) {
            this.ErrorMessage("Size must be within range 1.." + this.MaxSize() + " bytes.");
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
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiT25lRGFzLlR5cGVzLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vT25lRGFzLlR5cGVzL0RhdGFEaXJlY3Rpb25FbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL0VuZGlhbm5lc3NFbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL0ZpbGVHcmFudWxhcml0eUVudW0udHMiLCIuLi9PbmVEYXMuVHlwZXMvT25lRGFzRGF0YVR5cGVFbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL09uZURhc1N0YXRlRW51bS50cyIsIi4uL09uZURhcy5UeXBlcy9TYW1wbGVSYXRlRW51bS50cyIsIi4uL0NvcmUvQWN0aW9uUmVxdWVzdC50cyIsIi4uL0NvcmUvQWN0aW9uUmVzcG9uc2UudHMiLCIuLi9Db3JlL0V2ZW50RGlzcGF0Y2hlci50cyIsIi4uL0NvcmUvSUV2ZW50LnRzIiwiLi4vQ29yZS9PbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtLnRzIiwiLi4vTW9kZWxzL0J1ZmZlclJlcXVlc3RNb2RlbC50cyIsIi4uL01vZGVscy9DaGFubmVsSHViTW9kZWwudHMiLCIuLi9Nb2RlbHMvT25lRGFzTW9kdWxlTW9kZWwudHMiLCIuLi9Nb2RlbHMvVHJhbnNmZXJGdW5jdGlvbk1vZGVsLnRzIiwiLi4vU3RhdGljL0Nvbm5lY3Rpb25NYW5hZ2VyLnRzIiwiLi4vU3RhdGljL0VudW1lcmF0aW9uSGVscGVyLnRzIiwiLi4vU3RhdGljL0Vycm9yTWVzc2FnZS50cyIsIi4uL1N0YXRpYy9IZWxwZXIudHMiLCIuLi9TdGF0aWMvRXh0ZW5zaW9uRmFjdG9yeS50cyIsIi4uL1N0YXRpYy9FeHRlbnNpb25IaXZlLnRzIiwiLi4vVmlld01vZGVscy9Db3JlL0NoYW5uZWxIdWJWaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL0NvcmUvT25lRGFzTW9kdWxlVmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9Db3JlL09uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9Db3JlL1RyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL0V4dGVuc2lvbi9CdWZmZXJSZXF1ZXN0U2VsZWN0b3JWaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL0V4dGVuc2lvbi9CdWZmZXJSZXF1ZXN0Vmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9FeHRlbnNpb24vRGF0YVBvcnRWaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL0V4dGVuc2lvbi9FeHRlbnNpb25WaWV3TW9kZWxCYXNlLnRzIiwiLi4vVmlld01vZGVscy9FeHRlbnNpb24vRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlLnRzIiwiLi4vVmlld01vZGVscy9FeHRlbnNpb24vRXh0ZW5kZWREYXRhR2F0ZXdheVZpZXdNb2RlbEJhc2UudHMiLCIuLi9WaWV3TW9kZWxzL0V4dGVuc2lvbi9EYXRhV3JpdGVyVmlld01vZGVsQmFzZS50cyIsIi4uL1ZpZXdNb2RlbHMvRXh0ZW5zaW9uL0V4dGVuc2lvbkRlc2NyaXB0aW9uVmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9FeHRlbnNpb24vRXh0ZW5zaW9uSWRlbnRpZmljYXRpb25WaWV3TW9kZWwudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsSUFBSyxpQkFJSjtBQUpELFdBQUssaUJBQWlCO0lBRWxCLDJEQUFTLENBQUE7SUFDVCw2REFBVSxDQUFBO0FBQ2QsQ0FBQyxFQUpJLGlCQUFpQixLQUFqQixpQkFBaUIsUUFJckI7QUNKRCxJQUFLLGNBSUo7QUFKRCxXQUFLLGNBQWM7SUFFZixtRUFBZ0IsQ0FBQTtJQUNoQiw2REFBYSxDQUFBO0FBQ2pCLENBQUMsRUFKSSxjQUFjLEtBQWQsY0FBYyxRQUlsQjtBQ0pELElBQUssbUJBTUo7QUFORCxXQUFLLG1CQUFtQjtJQUVwQixzRUFBYSxDQUFBO0lBQ2IseUVBQWUsQ0FBQTtJQUNmLGdFQUFXLENBQUE7SUFDWCwrREFBVyxDQUFBO0FBQ2YsQ0FBQyxFQU5JLG1CQUFtQixL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QUF5QixDQUFDLFNBQVMsQ0FBQyxDQUFBO0lBQ2hGLENBQUM7SUFFTSxPQUFPO1FBRVYsSUFBSSxLQUFLLEdBQVE7WUFDYixjQUFjLEVBQVUsSUFBSSxDQUFDLGNBQWM7WUFDM0MsRUFBRSxFQUFVLElBQUksQ0FBQyxFQUFFO1lBQ25CLFVBQVUsRUFBVSxJQUFJLENBQUMsVUFBVTtZQUNuQyxZQUFZLEVBQVUsSUFBSSxDQUFDLFlBQVksRUFBRTtZQUN6QyxTQUFTLEVBQVcsSUFBSSxDQUFDLFNBQVMsRUFBRTtTQUN2QyxDQUFBO1FBRUQsT0FBTyxLQUFLLENBQUE7SUFDaEIsQ0FBQztDQUNKO0FDN0JELE1BQU0sZ0NBQWdDO0lBU2xDLFlBQVksNEJBQWlDO1FBRXpDLElBQUksQ0FBQyxjQUFjLEdBQUcsNEJBQTRCLENBQUMsY0FBYyxDQUFBO1FBQ2pFLElBQUksQ0FBQyxFQUFFLEdBQUcsNEJBQTRCLENBQUMsRUFBRSxDQUFBO1FBQ3pDLElBQUksQ0FBQyxJQUFJLEdBQUcsNEJBQTRCLENBQUMsSUFBSSxDQUFBO1FBQzdDLElBQUksQ0FBQyxXQUFXLEdBQUcsNEJBQTRCLENBQUMsV0FBVyxDQUFBO1FBQzNELElBQUksQ0FBQyxnQkFBZ0IsR0FBRyw0QkFBNEIsQ0FBQyxnQkFBZ0IsQ0FBQTtRQUNyRSxJQUFJLENBQUMscUJBQXFCLEdBQUcsNEJBQTRCLENBQUMscUJBQXFCLENBQUE7SUFDbkYsQ0FBQztDQUNKIiwic291cmNlc0NvbnRlbnQiOlsiZW51bSBEYXRhRGlyZWN0aW9uRW51bVxyXG57XHJcbiAgICBJbnB1dCA9IDEsXHJcbiAgICBPdXRwdXQgPSAyXHJcbn0iLCJlbnVtIEVuZGlhbm5lc3NFbnVtXHJcbntcclxuICAgIExpdHRsZUVuZGlhbiA9IDEsXHJcbiAgICBCaWdFbmRpYW4gPSAyXHJcbn0iLCJlbnVtIEZpbGVHcmFudWxhcml0eUVudW1cclxue1xyXG4gICAgTWludXRlXzEgPSA2MCxcclxuICAgIE1pbnV0ZV8xMCA9IDYwMCxcclxuICAgIEhvdXIgPSAzNjAwLFxyXG4gICAgRGF5ID0gODY0MDBcclxufSIsImVudW0gT25lRGFzRGF0YVR5cGVFbnVtXHJcbntcclxuICAgIEJPT0xFQU4gPSAweDAwOCxcclxuICAgIFVJTlQ4ID0gMHgxMDgsXHJcbiAgICBJTlQ4ID0gMHgyMDgsXHJcbiAgICBVSU5UMTYgPSAweDExMCxcclxuICAgIElOVDE2ID0gMHgyMTAsXHJcbiAgICBVSU5UMzIgPSAweDEyMCxcclxuICAgIElOVDMyID0gMHgyMjAsXHJcbiAgICBVSU5UNjQgPSAweDE0MCxcclxuICAgIElOVDY0ID0gMHgyNDAsXHJcbiAgICBGTE9BVDMyID0gMHgzMjAsXHJcbiAgICBGTE9BVDY0ID0gMHgzNDBcclxufSIsImVudW0gT25lRGFzU3RhdGVFbnVtXHJcbntcclxuICAgIEVycm9yID0gMSxcclxuICAgIEluaXRpYWxpemF0aW9uID0gMixcclxuICAgIElkbGUgPSAzLFxyXG4gICAgQXBwbHlDb25maWd1cmF0aW9uID0gNCxcclxuICAgIFJlYWR5ID0gNSxcclxuICAgIFJ1biA9IDZcclxufSIsImVudW0gU2FtcGxlUmF0ZUVudW1cclxue1xyXG4gICAgU2FtcGxlUmF0ZV8xMDAgPSAxLFxyXG4gICAgU2FtcGxlUmF0ZV8yNSA9IDQsXHJcbiAgICBTYW1wbGVSYXRlXzUgPSAyMCxcclxuICAgIFNhbXBsZVJhdGVfMSA9IDEwMFxyXG59IiwiY2xhc3MgQWN0aW9uUmVxdWVzdFxyXG57XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRXh0ZW5zaW9uSWQ6IHN0cmluZ1xyXG4gICAgcHVibGljIHJlYWRvbmx5IEluc3RhbmNlSWQ6IG51bWJlclxyXG4gICAgcHVibGljIHJlYWRvbmx5IE1ldGhvZE5hbWU6IHN0cmluZ1xyXG4gICAgcHVibGljIHJlYWRvbmx5IERhdGE6IGFueVxyXG5cclxuICAgIGNvbnN0cnVjdG9yKGV4dGVuc2lvbklkOiBzdHJpbmcsIGluc3RhbmNlSWQ6IG51bWJlciwgbWV0aG9kTmFtZTogc3RyaW5nLCBkYXRhOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5FeHRlbnNpb25JZCA9IGV4dGVuc2lvbklkO1xyXG4gICAgICAgIHRoaXMuSW5zdGFuY2VJZCA9IGluc3RhbmNlSWQ7XHJcbiAgICAgICAgdGhpcy5NZXRob2ROYW1lID0gbWV0aG9kTmFtZTtcclxuICAgICAgICB0aGlzLkRhdGEgPSBkYXRhO1xyXG4gICAgfVxyXG59IiwiY2xhc3MgQWN0aW9uUmVzcG9uc2Vcclxue1xyXG4gICAgcHVibGljIERhdGE6IGFueVxyXG5cclxuICAgIGNvbnN0cnVjdG9yKGRhdGE6IGFueSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLkRhdGEgPSBkYXRhO1xyXG4gICAgfVxyXG59IiwiY2xhc3MgRXZlbnREaXNwYXRjaGVyPFRTZW5kZXIsIFRBcmdzPiBpbXBsZW1lbnRzIElFdmVudDxUU2VuZGVyLCBUQXJncz5cclxue1xyXG4gICAgcHJpdmF0ZSBfc3Vic2NyaXB0aW9uczogQXJyYXk8KHNlbmRlcjogVFNlbmRlciwgYXJnczogVEFyZ3MpID0+IHZvaWQ+ID0gbmV3IEFycmF5PChzZW5kZXI6IFRTZW5kZXIsIGFyZ3M6IFRBcmdzKSA9PiB2b2lkPigpO1xyXG5cclxuICAgIHN1YnNjcmliZShmbjogKHNlbmRlcjogVFNlbmRlciwgYXJnczogVEFyZ3MpID0+IHZvaWQpOiB2b2lkXHJcbiAgICB7XHJcbiAgICAgICAgaWYgKGZuKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5fc3Vic2NyaXB0aW9ucy5wdXNoKGZuKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgdW5zdWJzY3JpYmUoZm46IChzZW5kZXI6IFRTZW5kZXIsIGFyZ3M6IFRBcmdzKSA9PiB2b2lkKTogdm9pZFxyXG4gICAge1xyXG4gICAgICAgIGxldCBpID0gdGhpcy5fc3Vic2NyaXB0aW9ucy5pbmRleE9mKGZuKTtcclxuXHJcbiAgICAgICAgaWYgKGkgPiAtMSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuX3N1YnNjcmlwdGlvbnMuc3BsaWNlKGksIDEpO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBkaXNwYXRjaChzZW5kZXI6IFRTZW5kZXIsIGFyZ3M6IFRBcmdzKTogdm9pZFxyXG4gICAge1xyXG4gICAgICAgIGZvciAobGV0IGhhbmRsZXIgb2YgdGhpcy5fc3Vic2NyaXB0aW9ucylcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGhhbmRsZXIoc2VuZGVyLCBhcmdzKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcbn0iLCJpbnRlcmZhY2UgSUV2ZW50PFRTZW5kZXIsIFRBcmdzPlxyXG57XHJcbiAgICBzdWJzY3JpYmUoZm46IChzZW5kZXI6IFRTZW5kZXIsIGFyZ3M6IFRBcmdzKSA9PiB2b2lkKTogdm9pZDtcclxuICAgIHVuc3Vic2NyaWJlKGZuOiAoc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncykgPT4gdm9pZCk6IHZvaWQ7XHJcbn0iLCJlbnVtIE9uZURhc01vZHVsZVNlbGVjdG9yTW9kZUVudW1cclxue1xyXG4gICAgRHVwbGV4ID0gMSxcclxuICAgIElucHV0T25seSA9IDIsXHJcbiAgICBPdXRwdXRPbmx5ID0gMyxcclxufSIsImNsYXNzIEJ1ZmZlclJlcXVlc3RNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgU2FtcGxlUmF0ZTogU2FtcGxlUmF0ZUVudW1cclxuICAgIHB1YmxpYyBHcm91cEZpbHRlcjogc3RyaW5nXHJcblxyXG4gICAgY29uc3RydWN0b3Ioc2FtcGxlUmF0ZTogU2FtcGxlUmF0ZUVudW0sIGdyb3VwRmlsdGVyOiBzdHJpbmcpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5TYW1wbGVSYXRlID0gc2FtcGxlUmF0ZTtcclxuICAgICAgICB0aGlzLkdyb3VwRmlsdGVyID0gZ3JvdXBGaWx0ZXI7XHJcbiAgICB9XHJcbn0iLCJjbGFzcyBDaGFubmVsSHViTW9kZWxcclxue1xyXG4gICAgcHVibGljIE5hbWU6IHN0cmluZ1xyXG4gICAgcHVibGljIEdyb3VwOiBzdHJpbmdcclxuICAgIHB1YmxpYyBEYXRhVHlwZTogT25lRGFzRGF0YVR5cGVFbnVtXHJcbiAgICBwdWJsaWMgR3VpZDogc3RyaW5nXHJcbiAgICBwdWJsaWMgQ3JlYXRpb25EYXRlVGltZTogc3RyaW5nXHJcbiAgICBwdWJsaWMgVW5pdDogc3RyaW5nXHJcbiAgICBwdWJsaWMgVHJhbnNmZXJGdW5jdGlvblNldDogYW55W11cclxuICAgIHB1YmxpYyBBc3NvY2lhdGVkRGF0YUlucHV0SWQ6IHN0cmluZ1xyXG4gICAgcHVibGljIEFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQ6IHN0cmluZ1tdXHJcblxyXG4gICAgY29uc3RydWN0b3IobmFtZTogc3RyaW5nLCBncm91cDogc3RyaW5nLCBkYXRhVHlwZTogT25lRGFzRGF0YVR5cGVFbnVtKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuTmFtZSA9IG5hbWU7XHJcbiAgICAgICAgdGhpcy5Hcm91cCA9IGdyb3VwO1xyXG4gICAgICAgIHRoaXMuRGF0YVR5cGUgPSBkYXRhVHlwZTtcclxuICAgICAgICB0aGlzLkd1aWQgPSBHdWlkLk5ld0d1aWQoKVxyXG4gICAgICAgIHRoaXMuQ3JlYXRpb25EYXRlVGltZSA9IG5ldyBEYXRlKCkudG9JU09TdHJpbmcoKVxyXG4gICAgICAgIHRoaXMuVW5pdCA9IFwiXCJcclxuICAgICAgICB0aGlzLlRyYW5zZmVyRnVuY3Rpb25TZXQgPSBbXVxyXG4gICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dElkID0gXCJcIlxyXG4gICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldCA9IFtdXHJcbiAgICB9XHJcbn0iLCJjbGFzcyBPbmVEYXNNb2R1bGVNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgRGF0YVR5cGU6IE9uZURhc0RhdGFUeXBlRW51bVxyXG4gICAgcHVibGljIERhdGFEaXJlY3Rpb246IERhdGFEaXJlY3Rpb25FbnVtXHJcbiAgICBwdWJsaWMgRW5kaWFubmVzczogRW5kaWFubmVzc0VudW1cclxuICAgIHB1YmxpYyBTaXplOiBudW1iZXJcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihkYXRhVHlwZTogT25lRGFzRGF0YVR5cGVFbnVtLCBkYXRhRGlyZWN0aW9uOiBEYXRhRGlyZWN0aW9uRW51bSwgZW5kaWFubmVzczogRW5kaWFubmVzc0VudW0sIHNpemU6IG51bWJlcilcclxuICAgIHtcclxuICAgICAgICB0aGlzLkRhdGFUeXBlID0gZGF0YVR5cGVcclxuICAgICAgICB0aGlzLkRhdGFEaXJlY3Rpb24gPSBkYXRhRGlyZWN0aW9uXHJcbiAgICAgICAgdGhpcy5FbmRpYW5uZXNzID0gZW5kaWFubmVzc1xyXG4gICAgICAgIHRoaXMuU2l6ZSA9IHNpemVcclxuICAgIH1cclxufVxyXG4iLCJjbGFzcyBUcmFuc2ZlckZ1bmN0aW9uTW9kZWxcclxue1xyXG4gICAgcHVibGljIERhdGVUaW1lOiBzdHJpbmdcclxuICAgIHB1YmxpYyBUeXBlOiBzdHJpbmdcclxuICAgIHB1YmxpYyBPcHRpb246IHN0cmluZ1xyXG4gICAgcHVibGljIEFyZ3VtZW50OiBzdHJpbmdcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihkYXRlVGltZTogc3RyaW5nLCB0eXBlOiBzdHJpbmcsIG9wdGlvbjogc3RyaW5nLCBhcmd1bWVudDogc3RyaW5nKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuRGF0ZVRpbWUgPSBkYXRlVGltZVxyXG4gICAgICAgIHRoaXMuVHlwZSA9IHR5cGVcclxuICAgICAgICB0aGlzLk9wdGlvbiA9IG9wdGlvblxyXG4gICAgICAgIHRoaXMuQXJndW1lbnQgPSBhcmd1bWVudFxyXG4gICAgfVxyXG59IiwiZGVjbGFyZSB2YXIgc2lnbmFsUjogYW55XHJcblxyXG5jbGFzcyBDb25uZWN0aW9uTWFuYWdlclxyXG57XHJcbiAgICBwdWJsaWMgc3RhdGljIFdlYkNsaWVudEh1YjogYW55IC8vIGltcHJvdmU6IHVzZSB0eXBpbmdzXHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBJbml0aWFsaXplKGVuYWJsZUxvZ2dpbmc6IGJvb2xlYW4pXHJcbiAgICB7XHJcbiAgICAgICAgQ29ubmVjdGlvbk1hbmFnZXIuV2ViQ2xpZW50SHViID0gbmV3IHNpZ25hbFIuSHViQ29ubmVjdGlvbkJ1aWxkZXIoKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAuY29uZmlndXJlTG9nZ2luZyhzaWduYWxSLkxvZ0xldmVsLkluZm9ybWF0aW9uKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAud2l0aFVybCgnL3dlYmNsaWVudGh1YicpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIC5idWlsZCgpO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgSW52b2tlV2ViQ2xpZW50SHViID0gYXN5bmMobWV0aG9kTmFtZTogc3RyaW5nLCAuLi5hcmdzOiBhbnlbXSkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gYXdhaXQgUHJvbWlzZS5yZXNvbHZlKENvbm5lY3Rpb25NYW5hZ2VyLldlYkNsaWVudEh1Yi5pbnZva2UobWV0aG9kTmFtZSwgLi4uYXJncykpXHJcbiAgICB9XHJcbn1cclxuIiwiY2xhc3MgRW51bWVyYXRpb25IZWxwZXJcclxue1xyXG4gICAgcHVibGljIHN0YXRpYyBEZXNjcmlwdGlvbjogeyBbaW5kZXg6IHN0cmluZ106IHN0cmluZyB9ID0ge31cclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIEdldEVudW1Mb2NhbGl6YXRpb24gPSAodHlwZU5hbWU6IHN0cmluZywgdmFsdWUpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdmFyIGtleTogc3RyaW5nID0gZXZhbCh0eXBlTmFtZSArIFwiW1wiICsgdmFsdWUgKyBcIl1cIilcclxuICAgICAgICByZXR1cm4gZXZhbChcIkVudW1lcmF0aW9uSGVscGVyLkRlc2NyaXB0aW9uWydcIiArIHR5cGVOYW1lICsgXCJfXCIgKyBrZXkgKyBcIiddXCIpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBHZXRFbnVtVmFsdWVzID0gKHR5cGVOYW1lOiBzdHJpbmcpID0+XHJcbiAgICB7XHJcbiAgICAgICAgbGV0IHZhbHVlczogYW55W11cclxuXHJcbiAgICAgICAgdmFsdWVzID0gZXZhbChcIk9iamVjdC5rZXlzKFwiICsgdHlwZU5hbWUgKyBcIikubWFwKGtleSA9PiBcIiArIHR5cGVOYW1lICsgXCJba2V5XSlcIilcclxuICAgICAgICByZXR1cm4gPG51bWJlcltdPnZhbHVlcy5maWx0ZXIodmFsdWUgPT4gdHlwZW9mICh2YWx1ZSkgPT09IFwibnVtYmVyXCIpXHJcbiAgICB9XHJcbn0iLCJsZXQgRXJyb3JNZXNzYWdlOiB7IFtpbmRleDogc3RyaW5nXTogc3RyaW5nIH0gPSB7fVxyXG5FcnJvck1lc3NhZ2VbXCJNdWx0aU1hcHBpbmdFZGl0b3JWaWV3TW9kZWxfSW52YWxpZFNldHRpbmdzXCJdID0gXCJPbmUgb3IgbW9yZSBzZXR0aW5ncyBhcmUgaW52YWxpZC5cIlxyXG5FcnJvck1lc3NhZ2VbXCJNdWx0aU1hcHBpbmdFZGl0b3JWaWV3TW9kZWxfV3JvbmdEYXRhVHlwZVwiXSA9IFwiT25lIG9yIG1vcmUgdmFyaWFibGUtY2hhbm5lbCBkYXRhIHR5cGUgY29tYmluYXRpb25zIGFyZSBpbnZhbGlkLlwiXHJcbkVycm9yTWVzc2FnZVtcIlByb2plY3RfQ2hhbm5lbEFscmVhZHlFeGlzdHNcIl0gPSBcIkEgY2hhbm5lbCB3aXRoIHRoYXQgbmFtZSBhbHJlYWR5IGV4aXN0cy5cIlxyXG5FcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0RldGFjaGVkRXhjbGFtYXRpb25NYXJrTm90QWxsb3dlZFwiXSA9IFwiQSBkZXRhY2hlZCBleGNsYW1hdGlvbiBtYXJrIGlzIG5vdCBhbGxvd2VkLlwiXHJcbkVycm9yTWVzc2FnZVtcIlByb2plY3RfR3JvdXBGaWx0ZXJFbXB0eVwiXSA9IFwiVGhlIGdyb3VwIGZpbHRlciBtdXN0IG5vdCBiZSBlbXB0eS5cIlxyXG5FcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0lzQWxyZWFkeUluR3JvdXBcIl0gPSBcIlRoZSBjaGFubmVsIGlzIGFscmVhZHkgYSBtZW1iZXIgb2YgdGhpcyBncm91cC5cIlxyXG5FcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0ludmFsaWRDaGFyYWN0ZXJzXCJdID0gXCJVc2UgQS1aLCBhLXosIDAtOSBvciBfLlwiXHJcbkVycm9yTWVzc2FnZVtcIlByb2plY3RfSW52YWxpZExlYWRpbmdDaGFyYWN0ZXJcIl0gPSBcIlVzZSBBLVogb3IgYS16IGFzIGZpcnN0IGNoYXJhY3Rlci5cIlxyXG5FcnJvck1lc3NhZ2VbXCJQcm9qZWN0X05hbWVFbXB0eVwiXSA9IFwiVGhlIG5hbWUgbXVzdCBub3QgYmUgZW1wdHkuXCJcclxuIiwiY2xhc3MgT2JzZXJ2YWJsZUdyb3VwPFQ+XHJcbntcclxuICAgIEtleTogc3RyaW5nO1xyXG4gICAgTWVtYmVyczogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8VD5cclxuXHJcbiAgICBjb25zdHJ1Y3RvcihrZXk6IHN0cmluZywgbWVtYmVyczogVFtdID0gbmV3IEFycmF5PFQ+KCkpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5LZXkgPSBrZXlcclxuICAgICAgICB0aGlzLk1lbWJlcnMgPSBrby5vYnNlcnZhYmxlQXJyYXkobWVtYmVycylcclxuICAgIH1cclxufVxyXG5cclxuZnVuY3Rpb24gT2JzZXJ2YWJsZUdyb3VwQnk8VD4obGlzdDogVFtdLCBuYW1lR2V0dGVyOiAoeDogVCkgPT4gc3RyaW5nLCBncm91cE5hbWVHZXR0ZXI6ICh4OiBUKSA9PiBzdHJpbmcsIGZpbHRlcjogc3RyaW5nKTogT2JzZXJ2YWJsZUdyb3VwPFQ+W11cclxue1xyXG4gICAgbGV0IHJlc3VsdDogT2JzZXJ2YWJsZUdyb3VwPFQ+W11cclxuICAgIGxldCByZWdFeHA6IFJlZ0V4cFxyXG5cclxuICAgIHJlc3VsdCA9IFtdXHJcbiAgICByZWdFeHAgPSBuZXcgUmVnRXhwKGZpbHRlciwgXCJpXCIpXHJcblxyXG4gICAgbGlzdC5mb3JFYWNoKGVsZW1lbnQgPT5cclxuICAgIHtcclxuICAgICAgICBpZiAocmVnRXhwLnRlc3QobmFtZUdldHRlcihlbGVtZW50KSkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBncm91cE5hbWVHZXR0ZXIoZWxlbWVudCkuc3BsaXQoXCJcXG5cIikuZm9yRWFjaChncm91cE5hbWUgPT5cclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgQWRkVG9Hcm91cGVkQXJyYXkoZWxlbWVudCwgZ3JvdXBOYW1lLCByZXN1bHQpXHJcbiAgICAgICAgICAgIH0pXHJcbiAgICAgICAgfVxyXG4gICAgfSlcclxuXHJcbiAgICByZXR1cm4gcmVzdWx0XHJcbn1cclxuXHJcbmZ1bmN0aW9uIEFkZFRvR3JvdXBlZEFycmF5PFQ+KGl0ZW06IFQsIGdyb3VwTmFtZTogc3RyaW5nLCBvYnNlcnZhYmxlR3JvdXBTZXQ6IE9ic2VydmFibGVHcm91cDxUPltdKVxyXG57XHJcbiAgICBsZXQgZ3JvdXA6IE9ic2VydmFibGVHcm91cDxUPlxyXG5cclxuICAgIGdyb3VwID0gb2JzZXJ2YWJsZUdyb3VwU2V0LmZpbmQoeSA9PiB5LktleSA9PT0gZ3JvdXBOYW1lKVxyXG5cclxuICAgIGlmICghZ3JvdXApXHJcbiAgICB7XHJcbiAgICAgICAgZ3JvdXAgPSBuZXcgT2JzZXJ2YWJsZUdyb3VwPFQ+KGdyb3VwTmFtZSlcclxuICAgICAgICBvYnNlcnZhYmxlR3JvdXBTZXQucHVzaChncm91cClcclxuICAgIH1cclxuXHJcbiAgICBncm91cC5NZW1iZXJzLnB1c2goaXRlbSlcclxufVxyXG5cclxuLy9mdW5jdGlvbiBBZGRUb0dyb3VwZWRPYnNlcnZhYmxlQXJyYXk8VD4oaXRlbTogVCwgZ3JvdXBOYW1lOiBzdHJpbmcsIG9ic2VydmFibGVHcm91cFNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8T2JzZXJ2YWJsZUdyb3VwPFQ+PilcclxuLy97XHJcbi8vICAgIGxldCBncm91cDogT2JzZXJ2YWJsZUdyb3VwPFQ+XHJcblxyXG4vLyAgICBncm91cCA9IG9ic2VydmFibGVHcm91cFNldCgpLmZpbmQoeSA9PiB5LktleSA9PT0gZ3JvdXBOYW1lKVxyXG5cclxuLy8gICAgaWYgKCFncm91cClcclxuLy8gICAge1xyXG4vLyAgICAgICAgZ3JvdXAgPSBuZXcgT2JzZXJ2YWJsZUdyb3VwPFQ+KGdyb3VwTmFtZSlcclxuLy8gICAgICAgIG9ic2VydmFibGVHcm91cFNldC5wdXNoKGdyb3VwKVxyXG4vLyAgICB9XHJcblxyXG4vLyAgICBncm91cC5NZW1iZXJzLnB1c2goaXRlbSlcclxuLy99XHJcblxyXG4vL2Z1bmN0aW9uIFJlbW92ZUZyb21Hcm91cGVkT2JzZXJ2YWJsZUFycmF5PFQ+KGl0ZW06IFQsIG9ic2VydmFibGVHcm91cFNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8T2JzZXJ2YWJsZUdyb3VwPFQ+PilcclxuLy97XHJcbi8vICAgIHZhciBncm91cDogT2JzZXJ2YWJsZUdyb3VwPFQ+XHJcblxyXG4vLyAgICBvYnNlcnZhYmxlR3JvdXBTZXQoKS5zb21lKHggPT5cclxuLy8gICAge1xyXG4vLyAgICAgICAgaWYgKHguTWVtYmVycygpLmluZGV4T2YoaXRlbSkgPiAtMSlcclxuLy8gICAgICAgIHtcclxuLy8gICAgICAgICAgICBncm91cCA9IHhcclxuXHJcbi8vICAgICAgICAgICAgcmV0dXJuIHRydWVcclxuLy8gICAgICAgIH1cclxuXHJcbi8vICAgICAgICByZXR1cm4gZmFsc2VcclxuLy8gICAgfSlcclxuXHJcbi8vICAgIGlmIChncm91cClcclxuLy8gICAge1xyXG4vLyAgICAgICAgZ3JvdXAuTWVtYmVycy5yZW1vdmUoaXRlbSlcclxuXHJcbi8vICAgICAgICBpZiAoZ3JvdXAuTWVtYmVycygpLmxlbmd0aCA9PT0gMClcclxuLy8gICAgICAgIHtcclxuLy8gICAgICAgICAgICBvYnNlcnZhYmxlR3JvdXBTZXQucmVtb3ZlKGdyb3VwKVxyXG4vLyAgICAgICAgfVxyXG5cclxuLy8gICAgICAgIHJldHVybiB0cnVlXHJcbi8vICAgIH1cclxuXHJcbi8vICAgIHJldHVybiBmYWxzZVxyXG4vL31cclxuXHJcbi8vZnVuY3Rpb24gVXBkYXRlR3JvdXBlZE9ic2VydmFibGVBcnJheTxUPihpdGVtOiBULCBncm91cE5hbWU6IHN0cmluZywgb2JzZXJ2YWJsZUdyb3VwU2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxPYnNlcnZhYmxlR3JvdXA8VD4+KVxyXG4vL3tcclxuLy8gICAgUmVtb3ZlRnJvbUdyb3VwZWRPYnNlcnZhYmxlQXJyYXkoaXRlbSwgb2JzZXJ2YWJsZUdyb3VwU2V0KVxyXG4vLyAgICBBZGRUb0dyb3VwZWRPYnNlcnZhYmxlQXJyYXkoaXRlbSwgZ3JvdXBOYW1lLCBvYnNlcnZhYmxlR3JvdXBTZXQpXHJcbi8vfVxyXG5cclxuZnVuY3Rpb24gTWFwTWFueTxUQXJyYXlFbGVtZW50LCBUU2VsZWN0PihhcnJheTogVEFycmF5RWxlbWVudFtdLCBtYXBGdW5jOiAoaXRlbTogVEFycmF5RWxlbWVudCkgPT4gVFNlbGVjdFtdKTogVFNlbGVjdFtdXHJcbntcclxuICAgIHJldHVybiBhcnJheS5yZWR1Y2UoKHByZXZpb3VzLCBjdXJyZW50LCBpKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiBwcmV2aW91cy5jb25jYXQobWFwRnVuYyhjdXJyZW50KSk7XHJcbiAgICB9LCA8VFNlbGVjdFtdPltdKTtcclxufVxyXG5cclxuY2xhc3MgR3VpZFxyXG57XHJcbiAgICBzdGF0aWMgTmV3R3VpZCgpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuICd4eHh4eHh4eC14eHh4LTR4eHgteXh4eC14eHh4eHh4eHh4eHgnLnJlcGxhY2UoL1t4eV0vZywgZnVuY3Rpb24gKGMpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB2YXIgciA9IE1hdGgucmFuZG9tKCkgKiAxNiB8IDBcclxuICAgICAgICAgICAgdmFyIHYgPSBjID09PSAneCcgPyByIDogKHIgJiAweDMgfCAweDgpXHJcblxyXG4gICAgICAgICAgICByZXR1cm4gdi50b1N0cmluZygxNilcclxuICAgICAgICB9KVxyXG4gICAgfVxyXG59XHJcblxyXG5mdW5jdGlvbiBkZWxheShtczogbnVtYmVyKVxyXG57XHJcbiAgICByZXR1cm4gbmV3IFByb21pc2UocmVzb2x2ZSA9PiBzZXRUaW1lb3V0KHJlc29sdmUsIG1zKSk7XHJcbn1cclxuXHJcbmxldCBDaGVja05hbWluZ0NvbnZlbnRpb24gPSAodmFsdWU6IHN0cmluZykgPT5cclxue1xyXG4gICAgdmFyIHJlZ0V4cDogYW55XHJcblxyXG4gICAgaWYgKHZhbHVlLmxlbmd0aCA9PT0gMClcclxuICAgIHtcclxuICAgICAgICByZXR1cm4geyBIYXNFcnJvcjogdHJ1ZSwgRXJyb3JEZXNjcmlwdGlvbjogRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9OYW1lRW1wdHlcIl0gfVxyXG4gICAgfVxyXG5cclxuICAgIHJlZ0V4cCA9IG5ldyBSZWdFeHAoXCJbXkEtWmEtejAtOV9dXCIpXHJcblxyXG4gICAgaWYgKHJlZ0V4cC50ZXN0KHZhbHVlKSlcclxuICAgIHtcclxuICAgICAgICByZXR1cm4geyBIYXNFcnJvcjogdHJ1ZSwgRXJyb3JEZXNjcmlwdGlvbjogRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9JbnZhbGlkQ2hhcmFjdGVyc1wiXSB9XHJcbiAgICB9XHJcblxyXG4gICAgcmVnRXhwID0gbmV3IFJlZ0V4cChcIl5bMC05X11cIilcclxuXHJcbiAgICBpZiAocmVnRXhwLnRlc3QodmFsdWUpKVxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB7IEhhc0Vycm9yOiB0cnVlLCBFcnJvckRlc2NyaXB0aW9uOiBFcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0ludmFsaWRMZWFkaW5nQ2hhcmFjdGVyXCJdIH1cclxuICAgIH1cclxuXHJcbiAgICByZXR1cm4ge1xyXG4gICAgICAgIEhhc0Vycm9yOiBmYWxzZSxcclxuICAgICAgICBFcnJvckRlc2NyaXB0aW9uOiBcIlwiXHJcbiAgICB9XHJcbn0iLCJjbGFzcyBFeHRlbnNpb25GYWN0b3J5XHJcbntcclxuICAgIHB1YmxpYyBzdGF0aWMgQ3JlYXRlRXh0ZW5zaW9uVmlld01vZGVsQXN5bmMgPSBhc3luYyAoZXh0ZW5zaW9uVHlwZTogc3RyaW5nLCBleHRlbnNpb25Nb2RlbDogYW55KSA9PlxyXG4gICAge1xyXG4gICAgICAgIGxldCBleHRlbnNpb25JZGVudGlmaWNhdGlvbjogRXh0ZW5zaW9uSWRlbnRpZmljYXRpb25WaWV3TW9kZWxcclxuICAgICAgICBsZXQgZXh0ZW5zaW9uVmlld01vZGVsOiBFeHRlbnNpb25WaWV3TW9kZWxCYXNlXHJcbiAgICAgICAgbGV0IGV4dGVuc2lvblZpZXdNb2RlbFJhdzogc3RyaW5nXHJcblxyXG4gICAgICAgIGV4dGVuc2lvbklkZW50aWZpY2F0aW9uID0gRXh0ZW5zaW9uSGl2ZS5GaW5kRXh0ZW5zaW9uSWRlbnRpZmljYXRpb24oZXh0ZW5zaW9uVHlwZSwgZXh0ZW5zaW9uTW9kZWwuRGVzY3JpcHRpb24uSWQpXHJcblxyXG4gICAgICAgIGlmIChleHRlbnNpb25JZGVudGlmaWNhdGlvbilcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGV4dGVuc2lvblZpZXdNb2RlbFJhdyA9IGF3YWl0IENvbm5lY3Rpb25NYW5hZ2VyLkludm9rZVdlYkNsaWVudEh1YihcIkdldEV4dGVuc2lvblN0cmluZ1Jlc291cmNlXCIsIGV4dGVuc2lvbk1vZGVsLkRlc2NyaXB0aW9uLklkLCBleHRlbnNpb25JZGVudGlmaWNhdGlvbi5WaWV3TW9kZWxSZXNvdXJjZU5hbWUpXHJcbiAgICAgICAgICAgIGV4dGVuc2lvblZpZXdNb2RlbCA9IDxFeHRlbnNpb25WaWV3TW9kZWxCYXNlPm5ldyBGdW5jdGlvbihleHRlbnNpb25WaWV3TW9kZWxSYXcgKyBcIjsgcmV0dXJuIFZpZXdNb2RlbENvbnN0cnVjdG9yXCIpKCkoZXh0ZW5zaW9uTW9kZWwsIGV4dGVuc2lvbklkZW50aWZpY2F0aW9uKVxyXG5cclxuICAgICAgICAgICAgcmV0dXJuIGV4dGVuc2lvblZpZXdNb2RlbFxyXG4gICAgICAgIH1cclxuICAgICAgICBlbHNlXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aHJvdyBuZXcgRXJyb3IoXCJObyBjb3JyZXNwb25kaW5nIGV4dGVuc2lvbiBkZXNjcmlwdGlvbiBmb3IgZXh0ZW5zaW9uIElEICdcIiArIGV4dGVuc2lvbk1vZGVsLkRlc2NyaXB0aW9uLklkICsgXCInIGZvdW5kLlwiKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxufSIsImNsYXNzIEV4dGVuc2lvbkhpdmVcclxue1xyXG4gICAgLy8gZmllbGRzXHJcbiAgICBwdWJsaWMgc3RhdGljIEV4dGVuc2lvbklkZW50aWZpY2F0aW9uU2V0OiBNYXA8c3RyaW5nLCBFeHRlbnNpb25JZGVudGlmaWNhdGlvblZpZXdNb2RlbFtdPlxyXG5cclxuICAgIC8vIGNvbnN0cnVjdG9yc1xyXG4gICAgc3RhdGljIEluaXRpYWxpemUgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIEV4dGVuc2lvbkhpdmUuRXh0ZW5zaW9uSWRlbnRpZmljYXRpb25TZXQgPSBuZXcgTWFwPHN0cmluZywgRXh0ZW5zaW9uSWRlbnRpZmljYXRpb25WaWV3TW9kZWxbXT4oKVxyXG4gICAgfSAgIFxyXG5cclxuICAgIHN0YXRpYyBGaW5kRXh0ZW5zaW9uSWRlbnRpZmljYXRpb24gPSAoZXh0ZW5zaW9uVHlwZU5hbWU6IHN0cmluZywgZXh0ZW5zaW9uSWQ6IHN0cmluZykgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gRXh0ZW5zaW9uSGl2ZS5FeHRlbnNpb25JZGVudGlmaWNhdGlvblNldC5nZXQoZXh0ZW5zaW9uVHlwZU5hbWUpLmZpbmQoZXh0ZW5zaW9uSWRlbnRpZmljYXRpb24gPT4gZXh0ZW5zaW9uSWRlbnRpZmljYXRpb24uSWQgPT09IGV4dGVuc2lvbklkKTtcclxuICAgIH1cclxufSIsImNsYXNzIENoYW5uZWxIdWJWaWV3TW9kZWxcclxue1xyXG4gICAgcHVibGljIE5hbWU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgR3JvdXA6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRGF0YVR5cGU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxPbmVEYXNEYXRhVHlwZUVudW0+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgR3VpZDogc3RyaW5nXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgQ3JlYXRpb25EYXRlVGltZTogc3RyaW5nXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgVW5pdDogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyByZWFkb25seSBUcmFuc2ZlckZ1bmN0aW9uU2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsPlxyXG4gICAgcHVibGljIFNlbGVjdGVkVHJhbnNmZXJGdW5jdGlvbjogS25vY2tvdXRPYnNlcnZhYmxlPFRyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWw+XHJcbiAgICBwdWJsaWMgRXZhbHVhdGVkVHJhbnNmZXJGdW5jdGlvblNldDogKCh2YWx1ZTogbnVtYmVyKSA9PiBudW1iZXIpW11cclxuICAgIHB1YmxpYyBJc1NlbGVjdGVkOiBLbm9ja291dE9ic2VydmFibGU8Ym9vbGVhbj5cclxuXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRGF0YVR5cGVTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PE9uZURhc0RhdGFUeXBlRW51bT5cclxuXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgQXNzb2NpYXRlZERhdGFJbnB1dDogS25vY2tvdXRPYnNlcnZhYmxlPERhdGFQb3J0Vmlld01vZGVsPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IEFzc29jaWF0ZWREYXRhT3V0cHV0U2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxEYXRhUG9ydFZpZXdNb2RlbD5cclxuXHJcbiAgICBwcml2YXRlIEFzc29jaWF0ZWREYXRhSW5wdXRJZDogc3RyaW5nXHJcbiAgICBwcml2YXRlIEFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQ6IHN0cmluZ1tdXHJcblxyXG4gICAgY29uc3RydWN0b3IoY2hhbm5lbEh1Yk1vZGVsOiBDaGFubmVsSHViTW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5OYW1lID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KGNoYW5uZWxIdWJNb2RlbC5OYW1lKVxyXG4gICAgICAgIHRoaXMuR3JvdXAgPSBrby5vYnNlcnZhYmxlPHN0cmluZz4oY2hhbm5lbEh1Yk1vZGVsLkdyb3VwKVxyXG4gICAgICAgIHRoaXMuRGF0YVR5cGVTZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8T25lRGFzRGF0YVR5cGVFbnVtPihFbnVtZXJhdGlvbkhlbHBlci5HZXRFbnVtVmFsdWVzKCdPbmVEYXNEYXRhVHlwZUVudW0nKSlcclxuICAgICAgICB0aGlzLkRhdGFUeXBlID0ga28ub2JzZXJ2YWJsZTxPbmVEYXNEYXRhVHlwZUVudW0+KGNoYW5uZWxIdWJNb2RlbC5EYXRhVHlwZSlcclxuICAgICAgICB0aGlzLkd1aWQgPSBjaGFubmVsSHViTW9kZWwuR3VpZFxyXG4gICAgICAgIHRoaXMuQ3JlYXRpb25EYXRlVGltZSA9IGNoYW5uZWxIdWJNb2RlbC5DcmVhdGlvbkRhdGVUaW1lXHJcbiAgICAgICAgdGhpcy5Vbml0ID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KGNoYW5uZWxIdWJNb2RlbC5Vbml0KVxyXG4gICAgICAgIHRoaXMuVHJhbnNmZXJGdW5jdGlvblNldCA9IGtvLm9ic2VydmFibGVBcnJheTxUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsPihjaGFubmVsSHViTW9kZWwuVHJhbnNmZXJGdW5jdGlvblNldC5tYXAodGYgPT4gbmV3IFRyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWwodGYpKSlcclxuICAgICAgICB0aGlzLlNlbGVjdGVkVHJhbnNmZXJGdW5jdGlvbiA9IGtvLm9ic2VydmFibGU8VHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbD4odGhpcy5DcmVhdGVEZWZhdWx0VHJhbnNmZXJGdW5jdGlvbigpKVxyXG4gICAgICAgIHRoaXMuSXNTZWxlY3RlZCA9IGtvLm9ic2VydmFibGU8Ym9vbGVhbj4oZmFsc2UpXHJcblxyXG4gICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dCA9IGtvLm9ic2VydmFibGU8RGF0YVBvcnRWaWV3TW9kZWw+KClcclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0U2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PERhdGFQb3J0Vmlld01vZGVsPigpXHJcblxyXG4gICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dElkID0gY2hhbm5lbEh1Yk1vZGVsLkFzc29jaWF0ZWREYXRhSW5wdXRJZFxyXG4gICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldCA9IGNoYW5uZWxIdWJNb2RlbC5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0XHJcbiAgICAgICAgdGhpcy5FdmFsdWF0ZWRUcmFuc2ZlckZ1bmN0aW9uU2V0ID0gW11cclxuICAgIH1cclxuXHJcbiAgICAvLyBtZXRob2RzXHJcbiAgICBwdWJsaWMgR2V0VHJhbnNmb3JtZWRWYWx1ZSA9ICh2YWx1ZTogYW55KTogbnVtYmVyID0+IFxyXG4gICAge1xyXG4gICAgICAgIGlmICh2YWx1ZSA9PT0gXCJOYU5cIilcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHZhbHVlID0gTmFOXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0aGlzLkV2YWx1YXRlZFRyYW5zZmVyRnVuY3Rpb25TZXQuZm9yRWFjaCh0ZiA9PiB2YWx1ZSA9IHRmKHZhbHVlKSlcclxuXHJcbiAgICAgICAgcmV0dXJuIHZhbHVlXHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBDcmVhdGVEZWZhdWx0VHJhbnNmZXJGdW5jdGlvbiA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIG5ldyBUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsKG5ldyBUcmFuc2ZlckZ1bmN0aW9uTW9kZWwoXCIwMDAxLTAxLTAxVDAwOjAwOjAwWlwiLCBcInBvbHlub21pYWxcIiwgXCJwZXJtYW5lbnRcIiwgXCIxOzBcIikpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIElzQXNzb2NpYXRpb25BbGxvd2VkKGRhdGFQb3J0OiBEYXRhUG9ydFZpZXdNb2RlbClcclxuICAgIHtcclxuICAgICAgICByZXR1cm4gKGRhdGFQb3J0LkRhdGFUeXBlICYgMHhmZikgPT0gKHRoaXMuRGF0YVR5cGUoKSAmIDB4ZmYpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFVwZGF0ZUFzc29jaWF0aW9uID0gKGRhdGFQb3J0OiBEYXRhUG9ydFZpZXdNb2RlbCkgPT5cclxuICAgIHtcclxuICAgICAgICBzd2l0Y2ggKGRhdGFQb3J0LkRhdGFEaXJlY3Rpb24pXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLklucHV0OlxyXG5cclxuICAgICAgICAgICAgICAgIHRoaXMuUmVzZXRBc3NvY2lhdGlvbihmYWxzZSwgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0KCkpXHJcbiAgICAgICAgICAgICAgICBicmVha1xyXG5cclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5PdXRwdXQ6XHJcblxyXG4gICAgICAgICAgICAgICAgdGhpcy5SZXNldEFzc29jaWF0aW9uKGZhbHNlLCBkYXRhUG9ydClcclxuICAgICAgICAgICAgICAgIGJyZWFrXHJcblxyXG4gICAgICAgICAgICBkZWZhdWx0OlxyXG4gICAgICAgICAgICAgICAgdGhyb3cgbmV3IEVycm9yKFwiTm90IGltcGxlbWVudGVkLlwiKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuU2V0QXNzb2NpYXRpb24oZGF0YVBvcnQpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFNldEFzc29jaWF0aW9uKGRhdGFQb3J0OiBEYXRhUG9ydFZpZXdNb2RlbClcclxuICAgIHtcclxuICAgICAgICBkYXRhUG9ydC5Bc3NvY2lhdGVkQ2hhbm5lbEh1YlNldC5wdXNoKHRoaXMpXHJcblxyXG4gICAgICAgIHN3aXRjaCAoZGF0YVBvcnQuRGF0YURpcmVjdGlvbilcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uSW5wdXQ6XHJcblxyXG4gICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0KGRhdGFQb3J0KVxyXG4gICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0SWQgPSBkYXRhUG9ydC5Ub0Z1bGxRdWFsaWZpZWRJZGVudGlmaWVyKClcclxuXHJcbiAgICAgICAgICAgICAgICBicmVha1xyXG5cclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5PdXRwdXQ6XHJcblxyXG4gICAgICAgICAgICAgICAgbGV0IGRhdGFPdXRwdXRJZCA9IGRhdGFQb3J0LlRvRnVsbFF1YWxpZmllZElkZW50aWZpZXIoKVxyXG5cclxuICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRTZXQucHVzaChkYXRhUG9ydClcclxuXHJcbiAgICAgICAgICAgICAgICBpZiAodGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0LmluZGV4T2YoZGF0YU91dHB1dElkKSA8IDApXHJcbiAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0LnB1c2goZGF0YVBvcnQuVG9GdWxsUXVhbGlmaWVkSWRlbnRpZmllcigpKVxyXG4gICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgIGJyZWFrXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBSZXNldEFzc29jaWF0aW9uKG1haW50YWluV2Vha1JlZmVyZW5jZTogYm9vbGVhbiwgLi4uZGF0YVBvcnRTZXQ6IERhdGFQb3J0Vmlld01vZGVsW10pXHJcbiAgICB7XHJcbiAgICAgICAgZGF0YVBvcnRTZXQuZm9yRWFjaChkYXRhUG9ydCA9PlxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgaWYgKCFkYXRhUG9ydClcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuXHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIGRhdGFQb3J0LkFzc29jaWF0ZWRDaGFubmVsSHViU2V0LnJlbW92ZSh0aGlzKVxyXG5cclxuICAgICAgICAgICAgc3dpdGNoIChkYXRhUG9ydC5EYXRhRGlyZWN0aW9uKVxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLklucHV0OlxyXG5cclxuICAgICAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXQobnVsbClcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCFtYWludGFpbldlYWtSZWZlcmVuY2UpXHJcbiAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXRJZCA9IG51bGxcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGJyZWFrXHJcblxyXG4gICAgICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5PdXRwdXQ6XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRTZXQucmVtb3ZlKGRhdGFQb3J0KVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBpZiAoIW1haW50YWluV2Vha1JlZmVyZW5jZSlcclxuICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGxldCBpbmRleDogbnVtYmVyID0gdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0LmluZGV4T2YoZGF0YVBvcnQuVG9GdWxsUXVhbGlmaWVkSWRlbnRpZmllcigpKVxyXG5cclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKGluZGV4ID4gLTEpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldC5zcGxpY2UoaW5kZXgsIDEpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0gICBcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGJyZWFrXHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9KVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBSZXNldEFsbEFzc29jaWF0aW9ucyhtYWludGFpbldlYWtSZWZlcmVuY2U6IGJvb2xlYW4pXHJcbiAgICB7XHJcbiAgICAgICAgaWYgKHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dCgpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5SZXNldEFzc29jaWF0aW9uKG1haW50YWluV2Vha1JlZmVyZW5jZSwgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0KCkpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0aGlzLlJlc2V0QXNzb2NpYXRpb24obWFpbnRhaW5XZWFrUmVmZXJlbmNlLCAuLi50aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0U2V0KCkpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEdldEFzc29jaWF0ZWREYXRhSW5wdXRJZCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dElkO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBHZXRBc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0ID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0O1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb01vZGVsKClcclxuICAgIHtcclxuICAgICAgICByZXR1cm4ge1xyXG4gICAgICAgICAgICBOYW1lOiA8c3RyaW5nPnRoaXMuTmFtZSgpLFxyXG4gICAgICAgICAgICBHcm91cDogPHN0cmluZz50aGlzLkdyb3VwKCksXHJcbiAgICAgICAgICAgIERhdGFUeXBlOiA8T25lRGFzRGF0YVR5cGVFbnVtPnRoaXMuRGF0YVR5cGUoKSxcclxuICAgICAgICAgICAgR3VpZDogPHN0cmluZz50aGlzLkd1aWQsXHJcbiAgICAgICAgICAgIENyZWF0aW9uRGF0ZVRpbWU6IDxzdHJpbmc+dGhpcy5DcmVhdGlvbkRhdGVUaW1lLFxyXG4gICAgICAgICAgICBVbml0OiA8c3RyaW5nPnRoaXMuVW5pdCgpLFxyXG4gICAgICAgICAgICBUcmFuc2ZlckZ1bmN0aW9uU2V0OiA8VHJhbnNmZXJGdW5jdGlvbk1vZGVsW10+dGhpcy5UcmFuc2ZlckZ1bmN0aW9uU2V0KCkubWFwKHRmID0+IHRmLlRvTW9kZWwoKSksXHJcbiAgICAgICAgICAgIEFzc29jaWF0ZWREYXRhSW5wdXRJZDogPHN0cmluZz50aGlzLkFzc29jaWF0ZWREYXRhSW5wdXRJZCxcclxuICAgICAgICAgICAgQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldDogPHN0cmluZ1tdPnRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldFxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgQWRkVHJhbnNmZXJGdW5jdGlvbiA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5UcmFuc2ZlckZ1bmN0aW9uU2V0LnB1c2godGhpcy5TZWxlY3RlZFRyYW5zZmVyRnVuY3Rpb24oKSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgRGVsZXRlVHJhbnNmZXJGdW5jdGlvbiA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5UcmFuc2ZlckZ1bmN0aW9uU2V0LnJlbW92ZSh0aGlzLlNlbGVjdGVkVHJhbnNmZXJGdW5jdGlvbigpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBOZXdUcmFuc2ZlckZ1bmN0aW9uID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLlNlbGVjdGVkVHJhbnNmZXJGdW5jdGlvbih0aGlzLkNyZWF0ZURlZmF1bHRUcmFuc2ZlckZ1bmN0aW9uKCkpXHJcbiAgICB9XHJcblxyXG4gICAgLy8gY29tbWFuZHNcclxuICAgIHB1YmxpYyBTZWxlY3RUcmFuc2ZlckZ1bmN0aW9uID0gKHRyYW5zZmVyRnVuY3Rpb246IFRyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWwpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5TZWxlY3RlZFRyYW5zZmVyRnVuY3Rpb24odHJhbnNmZXJGdW5jdGlvbilcclxuICAgIH1cclxufSIsImNsYXNzIE9uZURhc01vZHVsZVZpZXdNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgRGF0YVR5cGU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxPbmVEYXNEYXRhVHlwZUVudW0+XHJcbiAgICBwdWJsaWMgRGF0YURpcmVjdGlvbjogS25vY2tvdXRPYnNlcnZhYmxlPERhdGFEaXJlY3Rpb25FbnVtPlxyXG4gICAgcHVibGljIEVuZGlhbm5lc3M6IEtub2Nrb3V0T2JzZXJ2YWJsZTxFbmRpYW5uZXNzRW51bT5cclxuICAgIHB1YmxpYyBTaXplOiBLbm9ja291dE9ic2VydmFibGU8bnVtYmVyPlxyXG4gICAgcHVibGljIE1heFNpemU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxudW1iZXI+XHJcbiAgICBwdWJsaWMgRXJyb3JNZXNzYWdlOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIEhhc0Vycm9yOiBLbm9ja291dENvbXB1dGVkPGJvb2xlYW4+XHJcblxyXG4gICAgcHVibGljIERhdGFUeXBlU2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxPbmVEYXNEYXRhVHlwZUVudW0+XHJcblxyXG4gICAgcHJpdmF0ZSBfb25Qcm9wZXJ0eUNoYW5nZWQ6IEV2ZW50RGlzcGF0Y2hlcjxPbmVEYXNNb2R1bGVWaWV3TW9kZWwsIGFueT5cclxuICAgIHByb3RlY3RlZCBfbW9kZWw6IGFueVxyXG5cclxuICAgIGNvbnN0cnVjdG9yKG9uZURhc01vZHVsZU1vZGVsOiBPbmVEYXNNb2R1bGVNb2RlbClcclxuICAgIHtcclxuICAgICAgICB0aGlzLl9tb2RlbCA9IG9uZURhc01vZHVsZU1vZGVsXHJcblxyXG4gICAgICAgIHRoaXMuRGF0YVR5cGVTZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8T25lRGFzRGF0YVR5cGVFbnVtPihFbnVtZXJhdGlvbkhlbHBlci5HZXRFbnVtVmFsdWVzKCdPbmVEYXNEYXRhVHlwZUVudW0nKS5maWx0ZXIoZGF0YVR5cGUgPT4gZGF0YVR5cGUgIT09IE9uZURhc0RhdGFUeXBlRW51bS5CT09MRUFOKSlcclxuICAgICAgICB0aGlzLkRhdGFUeXBlID0ga28ub2JzZXJ2YWJsZTxPbmVEYXNEYXRhVHlwZUVudW0+KG9uZURhc01vZHVsZU1vZGVsLkRhdGFUeXBlKVxyXG4gICAgICAgIHRoaXMuRGF0YURpcmVjdGlvbiA9IGtvLm9ic2VydmFibGU8RGF0YURpcmVjdGlvbkVudW0+KG9uZURhc01vZHVsZU1vZGVsLkRhdGFEaXJlY3Rpb24pXHJcbiAgICAgICAgdGhpcy5FbmRpYW5uZXNzID0ga28ub2JzZXJ2YWJsZTxFbmRpYW5uZXNzRW51bT4ob25lRGFzTW9kdWxlTW9kZWwuRW5kaWFubmVzcylcclxuICAgICAgICB0aGlzLlNpemUgPSBrby5vYnNlcnZhYmxlPG51bWJlcj4ob25lRGFzTW9kdWxlTW9kZWwuU2l6ZSlcclxuICAgICAgICB0aGlzLk1heFNpemUgPSBrby5vYnNlcnZhYmxlPG51bWJlcj4oSW5maW5pdHkpXHJcbiAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UgPSBrby5vYnNlcnZhYmxlPHN0cmluZz4oXCJcIilcclxuICAgICAgICB0aGlzLkhhc0Vycm9yID0ga28uY29tcHV0ZWQ8Ym9vbGVhbj4oKCkgPT4gdGhpcy5FcnJvck1lc3NhZ2UoKS5sZW5ndGggPiAwKVxyXG5cclxuICAgICAgICB0aGlzLl9vblByb3BlcnR5Q2hhbmdlZCA9IG5ldyBFdmVudERpc3BhdGNoZXI8T25lRGFzTW9kdWxlVmlld01vZGVsLCBhbnk+KCk7XHJcblxyXG4gICAgICAgIHRoaXMuRGF0YVR5cGUuc3Vic2NyaWJlKF8gPT4gdGhpcy5PblByb3BlcnR5Q2hhbmdlZCgpKVxyXG4gICAgICAgIHRoaXMuRGF0YURpcmVjdGlvbi5zdWJzY3JpYmUoXyA9PiB0aGlzLk9uUHJvcGVydHlDaGFuZ2VkKCkpXHJcbiAgICAgICAgdGhpcy5TaXplLnN1YnNjcmliZShfID0+IHRoaXMuT25Qcm9wZXJ0eUNoYW5nZWQoKSlcclxuICAgIH1cclxuXHJcbiAgICBnZXQgUHJvcGVydHlDaGFuZ2VkKCk6IElFdmVudDxPbmVEYXNNb2R1bGVWaWV3TW9kZWwsIGFueT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5fb25Qcm9wZXJ0eUNoYW5nZWQ7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIE9uUHJvcGVydHlDaGFuZ2VkID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLlZhbGlkYXRlKClcclxuICAgICAgICB0aGlzLl9vblByb3BlcnR5Q2hhbmdlZC5kaXNwYXRjaCh0aGlzLCBudWxsKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBHZXRCeXRlQ291bnQgPSAoYm9vbGVhbkJpdFNpemU/OiBudW1iZXIpID0+XHJcbiAgICB7XHJcbiAgICAgICAgaWYgKGJvb2xlYW5CaXRTaXplICYmIHRoaXMuRGF0YVR5cGUoKSA9PT0gT25lRGFzRGF0YVR5cGVFbnVtLkJPT0xFQU4pXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBib29sZWFuQml0U2l6ZSA9IHBhcnNlSW50KDxhbnk+Ym9vbGVhbkJpdFNpemUpXHJcblxyXG4gICAgICAgICAgICByZXR1cm4gTWF0aC5jZWlsKGJvb2xlYW5CaXRTaXplICogdGhpcy5TaXplKCkgLyA4KTtcclxuICAgICAgICB9XHJcbiAgICAgICAgZWxzZVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgcmV0dXJuICh0aGlzLkRhdGFUeXBlKCkgJiAweDBGRikgLyA4ICogdGhpcy5TaXplKClcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFZhbGlkYXRlKClcclxuICAgIHtcclxuICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIlwiKVxyXG5cclxuICAgICAgICBpZiAodGhpcy5TaXplKCkgPCAxIHx8IChpc0Zpbml0ZSh0aGlzLk1heFNpemUoKSkgJiYgdGhpcy5HZXRCeXRlQ291bnQoKSA+IHRoaXMuTWF4U2l6ZSgpKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiU2l6ZSBtdXN0IGJlIHdpdGhpbiByYW5nZSAxLi5cIiArIHRoaXMuTWF4U2l6ZSgpICsgXCIgYnl0ZXMuXCIpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb1N0cmluZygpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuU2l6ZSgpICsgXCJ4IFwiICsgRW51bWVyYXRpb25IZWxwZXIuR2V0RW51bUxvY2FsaXphdGlvbignT25lRGFzRGF0YVR5cGVFbnVtJywgdGhpcy5EYXRhVHlwZSgpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBFeHRlbmRNb2RlbChtb2RlbDogYW55KVxyXG4gICAge1xyXG4gICAgICAgIC8vXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvTW9kZWwoKVxyXG4gICAge1xyXG4gICAgICAgIGxldCBtb2RlbDogYW55ID0ge1xyXG4gICAgICAgICAgICAkdHlwZTogPHN0cmluZz50aGlzLl9tb2RlbC4kdHlwZSxcclxuICAgICAgICAgICAgRGF0YVR5cGU6IDxPbmVEYXNEYXRhVHlwZUVudW0+dGhpcy5EYXRhVHlwZSgpLFxyXG4gICAgICAgICAgICBTaXplOiA8bnVtYmVyPnRoaXMuU2l6ZSgpLFxyXG4gICAgICAgICAgICBEYXRhRGlyZWN0aW9uOiA8RGF0YURpcmVjdGlvbkVudW0+dGhpcy5EYXRhRGlyZWN0aW9uKCksXHJcbiAgICAgICAgICAgIEVuZGlhbm5lc3M6IDxFbmRpYW5uZXNzRW51bT50aGlzLkVuZGlhbm5lc3MoKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5FeHRlbmRNb2RlbChtb2RlbClcclxuXHJcbiAgICAgICAgcmV0dXJuIG1vZGVsXHJcbiAgICB9XHJcbn1cclxuIiwiY2xhc3MgT25lRGFzTW9kdWxlU2VsZWN0b3JWaWV3TW9kZWxcclxue1xyXG4gICAgcHVibGljIFNldHRpbmdzVGVtcGxhdGVOYW1lOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIE5ld01vZHVsZTogS25vY2tvdXRPYnNlcnZhYmxlPE9uZURhc01vZHVsZVZpZXdNb2RlbD4gIFxyXG4gICAgcHVibGljIE1heEJ5dGVzOiBLbm9ja291dE9ic2VydmFibGU8bnVtYmVyPlxyXG4gICAgcHVibGljIFJlbWFpbmluZ0J5dGVzOiBLbm9ja291dE9ic2VydmFibGU8bnVtYmVyPlxyXG4gICAgcHVibGljIFJlbWFpbmluZ0NvdW50OiBLbm9ja291dE9ic2VydmFibGU8bnVtYmVyPiAgICBcclxuICAgIHB1YmxpYyBNb2R1bGVTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PE9uZURhc01vZHVsZVZpZXdNb2RlbD5cclxuICAgIHB1YmxpYyBFcnJvck1lc3NhZ2U6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgSGFzRXJyb3I6IEtub2Nrb3V0Q29tcHV0ZWQ8Ym9vbGVhbj5cclxuXHJcbiAgICBwdWJsaWMgT25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlOiBLbm9ja291dE9ic2VydmFibGU8T25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlRW51bT5cclxuXHJcbiAgICBwcml2YXRlIF9vbk1vZHVsZVNldENoYW5nZWQ6IEV2ZW50RGlzcGF0Y2hlcjxPbmVEYXNNb2R1bGVTZWxlY3RvclZpZXdNb2RlbCwgT25lRGFzTW9kdWxlVmlld01vZGVsW10+XHJcblxyXG4gICAgY29uc3RydWN0b3Iob25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlOiBPbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtLCBtb2R1bGVTZXQ6IE9uZURhc01vZHVsZVZpZXdNb2RlbFtdID0gW10pXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5PbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGUgPSBrby5vYnNlcnZhYmxlPE9uZURhc01vZHVsZVNlbGVjdG9yTW9kZUVudW0+KG9uZURhc01vZHVsZVNlbGVjdG9yTW9kZSlcclxuXHJcbiAgICAgICAgdGhpcy5TZXR0aW5nc1RlbXBsYXRlTmFtZSA9IGtvLm9ic2VydmFibGUoXCJQcm9qZWN0X09uZURhc01vZHVsZVRlbXBsYXRlXCIpXHJcbiAgICAgICAgdGhpcy5OZXdNb2R1bGUgPSBrby5vYnNlcnZhYmxlPE9uZURhc01vZHVsZVZpZXdNb2RlbD4oKTtcclxuICAgICAgICB0aGlzLk1heEJ5dGVzID0ga28ub2JzZXJ2YWJsZTxudW1iZXI+KEluZmluaXR5KTtcclxuICAgICAgICB0aGlzLlJlbWFpbmluZ0J5dGVzID0ga28ub2JzZXJ2YWJsZTxudW1iZXI+KE5hTik7XHJcbiAgICAgICAgdGhpcy5SZW1haW5pbmdDb3VudCA9IGtvLm9ic2VydmFibGU8bnVtYmVyPihOYU4pO1xyXG4gICAgICAgIHRoaXMuTW9kdWxlU2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PE9uZURhc01vZHVsZVZpZXdNb2RlbD4obW9kdWxlU2V0KTtcclxuICAgICAgICB0aGlzLkVycm9yTWVzc2FnZSA9IGtvLm9ic2VydmFibGU8c3RyaW5nPihcIlwiKVxyXG4gICAgICAgIHRoaXMuSGFzRXJyb3IgPSBrby5jb21wdXRlZDxib29sZWFuPigoKSA9PiB0aGlzLkVycm9yTWVzc2FnZSgpLmxlbmd0aCA+IDApXHJcblxyXG4gICAgICAgIHRoaXMuX29uTW9kdWxlU2V0Q2hhbmdlZCA9IG5ldyBFdmVudERpc3BhdGNoZXI8T25lRGFzTW9kdWxlU2VsZWN0b3JWaWV3TW9kZWwsIE9uZURhc01vZHVsZVZpZXdNb2RlbFtdPigpO1xyXG5cclxuICAgICAgICB0aGlzLkludGVybmFsQ3JlYXRlTmV3TW9kdWxlKClcclxuICAgICAgICB0aGlzLkludGVybmFsVXBkYXRlKClcclxuICAgIH1cclxuXHJcbiAgICBnZXQgT25Nb2R1bGVTZXRDaGFuZ2VkKCk6IElFdmVudDxPbmVEYXNNb2R1bGVTZWxlY3RvclZpZXdNb2RlbCwgT25lRGFzTW9kdWxlVmlld01vZGVsW10+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuX29uTW9kdWxlU2V0Q2hhbmdlZDtcclxuICAgIH1cclxuICAgIFxyXG4gICAgLy8gbWV0aG9kc1xyXG4gICAgcHVibGljIFNldE1heEJ5dGVzID0gKHZhbHVlOiBudW1iZXIpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5NYXhCeXRlcyh2YWx1ZSlcclxuICAgICAgICB0aGlzLkludGVybmFsVXBkYXRlKClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgR2V0SW5wdXRNb2R1bGVTZXQgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLk1vZHVsZVNldCgpLmZpbHRlcihtb2R1bGUgPT4gbW9kdWxlLkRhdGFEaXJlY3Rpb24oKSA9PT0gRGF0YURpcmVjdGlvbkVudW0uSW5wdXQpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEdldE91dHB1dE1vZHVsZVNldCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuTW9kdWxlU2V0KCkuZmlsdGVyKG1vZHVsZSA9PiBtb2R1bGUuRGF0YURpcmVjdGlvbigpID09PSBEYXRhRGlyZWN0aW9uRW51bS5PdXRwdXQpXHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBJbnRlcm5hbFVwZGF0ZSgpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5VcGRhdGUoKVxyXG4gICAgICAgIHRoaXMuVmFsaWRhdGUoKVxyXG4gICAgfVxyXG5cclxuICAgIHByb3RlY3RlZCBVcGRhdGUoKVxyXG4gICAge1xyXG4gICAgICAgIGxldCBtb2R1bGVTZXQ6IE9uZURhc01vZHVsZVZpZXdNb2RlbFtdXHJcbiAgICAgICAgbGV0IHJlbWFpbmluZ0J5dGVzOiBudW1iZXJcclxuXHJcbiAgICAgICAgc3dpdGNoICh0aGlzLk5ld01vZHVsZSgpLkRhdGFEaXJlY3Rpb24oKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uSW5wdXQ6XHJcbiAgICAgICAgICAgICAgICBtb2R1bGVTZXQgPSB0aGlzLkdldElucHV0TW9kdWxlU2V0KClcclxuICAgICAgICAgICAgICAgIGJyZWFrO1xyXG5cclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5PdXRwdXQ6XHJcbiAgICAgICAgICAgICAgICBtb2R1bGVTZXQgPSB0aGlzLkdldE91dHB1dE1vZHVsZVNldCgpXHJcbiAgICAgICAgICAgICAgICBicmVhaztcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJlbWFpbmluZ0J5dGVzID0gdGhpcy5NYXhCeXRlcygpIC0gbW9kdWxlU2V0Lm1hcChvbmVEYXNNb2R1bGUgPT4gb25lRGFzTW9kdWxlLkdldEJ5dGVDb3VudCgpKS5yZWR1Y2UoKHByZXZpb3VzVmFsdWUsIGN1cnJlbnRWYWx1ZSkgPT4gcHJldmlvdXNWYWx1ZSArIGN1cnJlbnRWYWx1ZSwgMClcclxuXHJcbiAgICAgICAgdGhpcy5SZW1haW5pbmdCeXRlcyhyZW1haW5pbmdCeXRlcylcclxuICAgICAgICB0aGlzLlJlbWFpbmluZ0NvdW50KE1hdGguZmxvb3IodGhpcy5SZW1haW5pbmdCeXRlcygpIC8gKCh0aGlzLk5ld01vZHVsZSgpLkRhdGFUeXBlKCkgJiAweDBGRikgLyA4KSkpXHJcbiAgICB9XHJcblxyXG4gICAgcHJvdGVjdGVkIFZhbGlkYXRlKClcclxuICAgIHtcclxuICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIlwiKVxyXG5cclxuICAgICAgICBpZiAodGhpcy5OZXdNb2R1bGUoKS5IYXNFcnJvcigpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJSZXNvbHZlIGFsbCByZW1haW5pbmcgbW9kdWxlIGVycm9ycyBiZWZvcmUgY29udGludWluZy5cIilcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmICh0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yTW9kZSgpID09PSBPbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtLklucHV0T25seSAmJiB0aGlzLk5ld01vZHVsZSgpLkRhdGFEaXJlY3Rpb24oKSA9PSBEYXRhRGlyZWN0aW9uRW51bS5PdXRwdXQpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIk9ubHkgaW5wdXQgbW9kdWxlcyBhcmUgYWxsb3dlZC5cIilcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmICh0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yTW9kZSgpID09PSBPbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtLk91dHB1dE9ubHkgJiYgdGhpcy5OZXdNb2R1bGUoKS5EYXRhRGlyZWN0aW9uKCkgPT0gRGF0YURpcmVjdGlvbkVudW0uSW5wdXQpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIk9ubHkgb3V0cHV0IG1vZHVsZXMgYXJlIGFsbG93ZWQuXCIpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBpZiAoaXNGaW5pdGUodGhpcy5SZW1haW5pbmdCeXRlcygpKSAmJiAodGhpcy5SZW1haW5pbmdCeXRlcygpIC0gdGhpcy5OZXdNb2R1bGUoKS5HZXRCeXRlQ291bnQoKSA8IDApKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJCeXRlIGNvdW50IG9mIG5ldyBtb2R1bGUgaXMgdG9vIGhpZ2guXCIpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBpZiAodGhpcy5SZW1haW5pbmdDb3VudCgpIDw9IDApXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIlRoZSBtYXhpbXVtIG51bWJlciBvZiBtb2R1bGVzIGlzIHJlYWNoZWQuXCIpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHByb3RlY3RlZCBDcmVhdGVOZXdNb2R1bGUoKVxyXG4gICAge1xyXG4gICAgICAgIGlmICh0aGlzLk5ld01vZHVsZSgpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgcmV0dXJuIG5ldyBPbmVEYXNNb2R1bGVWaWV3TW9kZWwobmV3IE9uZURhc01vZHVsZU1vZGVsKHRoaXMuTmV3TW9kdWxlKCkuRGF0YVR5cGUoKSwgdGhpcy5OZXdNb2R1bGUoKS5EYXRhRGlyZWN0aW9uKCksIHRoaXMuTmV3TW9kdWxlKCkuRW5kaWFubmVzcygpLCAxKSlcclxuICAgICAgICB9XHJcbiAgICAgICAgZWxzZVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgcmV0dXJuIG5ldyBPbmVEYXNNb2R1bGVWaWV3TW9kZWwobmV3IE9uZURhc01vZHVsZU1vZGVsKE9uZURhc0RhdGFUeXBlRW51bS5VSU5UMTYsIERhdGFEaXJlY3Rpb25FbnVtLklucHV0LCBFbmRpYW5uZXNzRW51bS5MaXR0bGVFbmRpYW4sIDEpKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIEludGVybmFsQ3JlYXRlTmV3TW9kdWxlKClcclxuICAgIHtcclxuICAgICAgICBpZiAodGhpcy5OZXdNb2R1bGUoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuTmV3TW9kdWxlKCkuUHJvcGVydHlDaGFuZ2VkLnVuc3Vic2NyaWJlKHRoaXMuT25Nb2R1bGVQcm9wZXJ0eUNoYW5nZWQpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0aGlzLk5ld01vZHVsZSh0aGlzLkNyZWF0ZU5ld01vZHVsZSgpKVxyXG4gICAgICAgIHRoaXMuTmV3TW9kdWxlKCkuUHJvcGVydHlDaGFuZ2VkLnN1YnNjcmliZSh0aGlzLk9uTW9kdWxlUHJvcGVydHlDaGFuZ2VkKVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgT25Nb2R1bGVQcm9wZXJ0eUNoYW5nZWQgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgfVxyXG5cclxuICAgIC8vIGNvbW1hbmRzXHJcbiAgICBwdWJsaWMgQWRkTW9kdWxlID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICBsZXQgbmV3TW9kdWxlOiBPbmVEYXNNb2R1bGVWaWV3TW9kZWxcclxuXHJcbiAgICAgICAgaWYgKCF0aGlzLkhhc0Vycm9yKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLk1vZHVsZVNldC5wdXNoKHRoaXMuTmV3TW9kdWxlKCkpXHJcbiAgICAgICAgICAgIHRoaXMuSW50ZXJuYWxDcmVhdGVOZXdNb2R1bGUoKVxyXG4gICAgICAgICAgICB0aGlzLkludGVybmFsVXBkYXRlKClcclxuICAgICAgICAgICAgdGhpcy5fb25Nb2R1bGVTZXRDaGFuZ2VkLmRpc3BhdGNoKHRoaXMsIHRoaXMuTW9kdWxlU2V0KCkpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBEZWxldGVNb2R1bGUgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuTW9kdWxlU2V0LnBvcCgpXHJcbiAgICAgICAgdGhpcy5JbnRlcm5hbFVwZGF0ZSgpXHJcbiAgICAgICAgdGhpcy5fb25Nb2R1bGVTZXRDaGFuZ2VkLmRpc3BhdGNoKHRoaXMsIHRoaXMuTW9kdWxlU2V0KCkpXHJcbiAgICB9XHJcbn0iLCJjbGFzcyBUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsXHJcbntcclxuICAgIHB1YmxpYyBEYXRlVGltZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBUeXBlOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIE9wdGlvbjogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBBcmd1bWVudDogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuXHJcbiAgICBjb25zdHJ1Y3Rvcih0cmFuc2ZlckZ1bmN0aW9uTW9kZWw6IFRyYW5zZmVyRnVuY3Rpb25Nb2RlbClcclxuICAgIHtcclxuICAgICAgICB0aGlzLkRhdGVUaW1lID0ga28ub2JzZXJ2YWJsZSh0cmFuc2ZlckZ1bmN0aW9uTW9kZWwuRGF0ZVRpbWUpXHJcbiAgICAgICAgdGhpcy5UeXBlID0ga28ub2JzZXJ2YWJsZSh0cmFuc2ZlckZ1bmN0aW9uTW9kZWwuVHlwZSlcclxuICAgICAgICB0aGlzLk9wdGlvbiA9IGtvLm9ic2VydmFibGUodHJhbnNmZXJGdW5jdGlvbk1vZGVsLk9wdGlvbilcclxuICAgICAgICB0aGlzLkFyZ3VtZW50ID0ga28ub2JzZXJ2YWJsZSh0cmFuc2ZlckZ1bmN0aW9uTW9kZWwuQXJndW1lbnQpXHJcbiAgICB9XHJcblxyXG4gICAgLy8gbWV0aG9kc1xyXG4gICAgcHVibGljIFRvTW9kZWwoKVxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiBuZXcgVHJhbnNmZXJGdW5jdGlvbk1vZGVsKHRoaXMuRGF0ZVRpbWUoKSwgdGhpcy5UeXBlKCksIHRoaXMuT3B0aW9uKCksIHRoaXMuQXJndW1lbnQoKSlcclxuICAgIH1cclxufSIsImNsYXNzIEJ1ZmZlclJlcXVlc3RTZWxlY3RvclZpZXdNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgTmV3QnVmZmVyUmVxdWVzdDogS25vY2tvdXRPYnNlcnZhYmxlPEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWw+ICBcclxuICAgIHB1YmxpYyBCdWZmZXJSZXF1ZXN0U2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxCdWZmZXJSZXF1ZXN0Vmlld01vZGVsPlxyXG4gICAgcHVibGljIEVycm9yTWVzc2FnZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBIYXNFcnJvcjogS25vY2tvdXRDb21wdXRlZDxib29sZWFuPlxyXG5cclxuICAgIHByaXZhdGUgX29uQnVmZmVyUmVxdWVzdFNldENoYW5nZWQ6IEV2ZW50RGlzcGF0Y2hlcjxCdWZmZXJSZXF1ZXN0U2VsZWN0b3JWaWV3TW9kZWwsIEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWxbXT5cclxuXHJcbiAgICBjb25zdHJ1Y3RvcihidWZmZXJSZXF1ZXN0U2V0OiBCdWZmZXJSZXF1ZXN0Vmlld01vZGVsW10gPSBbXSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLk5ld0J1ZmZlclJlcXVlc3QgPSBrby5vYnNlcnZhYmxlPEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWw+KCk7XHJcbiAgICAgICAgdGhpcy5CdWZmZXJSZXF1ZXN0U2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWw+KGJ1ZmZlclJlcXVlc3RTZXQpO1xyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KFwiXCIpXHJcbiAgICAgICAgdGhpcy5IYXNFcnJvciA9IGtvLmNvbXB1dGVkPGJvb2xlYW4+KCgpID0+IHRoaXMuRXJyb3JNZXNzYWdlKCkubGVuZ3RoID4gMClcclxuXHJcbiAgICAgICAgdGhpcy5fb25CdWZmZXJSZXF1ZXN0U2V0Q2hhbmdlZCA9IG5ldyBFdmVudERpc3BhdGNoZXI8QnVmZmVyUmVxdWVzdFNlbGVjdG9yVmlld01vZGVsLCBCdWZmZXJSZXF1ZXN0Vmlld01vZGVsW10+KCk7XHJcblxyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxDcmVhdGVOZXdCdWZmZXJSZXF1ZXN0KClcclxuICAgICAgICB0aGlzLkludGVybmFsVXBkYXRlKClcclxuICAgIH1cclxuXHJcbiAgICBnZXQgT25CdWZmZXJSZXF1ZXN0U2V0Q2hhbmdlZCgpOiBJRXZlbnQ8QnVmZmVyUmVxdWVzdFNlbGVjdG9yVmlld01vZGVsLCBCdWZmZXJSZXF1ZXN0Vmlld01vZGVsW10+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuX29uQnVmZmVyUmVxdWVzdFNldENoYW5nZWQ7XHJcbiAgICB9XHJcbiAgICBcclxuICAgIC8vIG1ldGhvZHNcclxuICAgIHByaXZhdGUgSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuVXBkYXRlKClcclxuICAgICAgICB0aGlzLlZhbGlkYXRlKClcclxuICAgIH1cclxuXHJcbiAgICBwcm90ZWN0ZWQgVXBkYXRlKClcclxuICAgIHtcclxuICAgICAgICAvL1xyXG4gICAgfVxyXG5cclxuICAgIHByb3RlY3RlZCBWYWxpZGF0ZSgpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJcIilcclxuXHJcbiAgICAgICAgaWYgKHRoaXMuTmV3QnVmZmVyUmVxdWVzdCgpLkhhc0Vycm9yKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIlJlc29sdmUgYWxsIHJlbWFpbmluZyBlcnJvcnMgYmVmb3JlIGNvbnRpbnVpbmcuXCIpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHByb3RlY3RlZCBDcmVhdGVOZXdCdWZmZXJSZXF1ZXN0KClcclxuICAgIHtcclxuICAgICAgICBpZiAodGhpcy5OZXdCdWZmZXJSZXF1ZXN0KCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICByZXR1cm4gbmV3IEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWwobmV3IEJ1ZmZlclJlcXVlc3RNb2RlbCh0aGlzLk5ld0J1ZmZlclJlcXVlc3QoKS5TYW1wbGVSYXRlKCksIHRoaXMuTmV3QnVmZmVyUmVxdWVzdCgpLkdyb3VwRmlsdGVyKCkpKVxyXG4gICAgICAgIH1cclxuICAgICAgICBlbHNlXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICByZXR1cm4gbmV3IEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWwobmV3IEJ1ZmZlclJlcXVlc3RNb2RlbChTYW1wbGVSYXRlRW51bS5TYW1wbGVSYXRlXzEsIFwiKlwiKSlcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBJbnRlcm5hbENyZWF0ZU5ld0J1ZmZlclJlcXVlc3QoKVxyXG4gICAge1xyXG4gICAgICAgIGlmICh0aGlzLk5ld0J1ZmZlclJlcXVlc3QoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuTmV3QnVmZmVyUmVxdWVzdCgpLlByb3BlcnR5Q2hhbmdlZC51bnN1YnNjcmliZSh0aGlzLk9uQnVmZmVyUmVxdWVzdFByb3BlcnR5Q2hhbmdlZClcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuTmV3QnVmZmVyUmVxdWVzdCh0aGlzLkNyZWF0ZU5ld0J1ZmZlclJlcXVlc3QoKSlcclxuICAgICAgICB0aGlzLk5ld0J1ZmZlclJlcXVlc3QoKS5Qcm9wZXJ0eUNoYW5nZWQuc3Vic2NyaWJlKHRoaXMuT25CdWZmZXJSZXF1ZXN0UHJvcGVydHlDaGFuZ2VkKVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgT25CdWZmZXJSZXF1ZXN0UHJvcGVydHlDaGFuZ2VkID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLkludGVybmFsVXBkYXRlKClcclxuICAgIH1cclxuXHJcbiAgICAvLyBjb21tYW5kc1xyXG4gICAgcHVibGljIEFkZEJ1ZmZlclJlcXVlc3QgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIGxldCBuZXdCdWZmZXJSZXF1ZXN0OiBCdWZmZXJSZXF1ZXN0Vmlld01vZGVsXHJcblxyXG4gICAgICAgIGlmICghdGhpcy5IYXNFcnJvcigpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5CdWZmZXJSZXF1ZXN0U2V0LnB1c2godGhpcy5OZXdCdWZmZXJSZXF1ZXN0KCkpXHJcbiAgICAgICAgICAgIHRoaXMuSW50ZXJuYWxDcmVhdGVOZXdCdWZmZXJSZXF1ZXN0KClcclxuICAgICAgICAgICAgdGhpcy5JbnRlcm5hbFVwZGF0ZSgpXHJcbiAgICAgICAgICAgIHRoaXMuX29uQnVmZmVyUmVxdWVzdFNldENoYW5nZWQuZGlzcGF0Y2godGhpcywgdGhpcy5CdWZmZXJSZXF1ZXN0U2V0KCkpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBEZWxldGVCdWZmZXJSZXF1ZXN0ID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLkJ1ZmZlclJlcXVlc3RTZXQucG9wKClcclxuICAgICAgICB0aGlzLkludGVybmFsVXBkYXRlKClcclxuICAgICAgICB0aGlzLl9vbkJ1ZmZlclJlcXVlc3RTZXRDaGFuZ2VkLmRpc3BhdGNoKHRoaXMsIHRoaXMuQnVmZmVyUmVxdWVzdFNldCgpKVxyXG4gICAgfVxyXG59IiwiY2xhc3MgQnVmZmVyUmVxdWVzdFZpZXdNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgU2FtcGxlUmF0ZTogS25vY2tvdXRPYnNlcnZhYmxlPFNhbXBsZVJhdGVFbnVtPlxyXG4gICAgcHVibGljIEdyb3VwRmlsdGVyOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIEVycm9yTWVzc2FnZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBIYXNFcnJvcjogS25vY2tvdXRDb21wdXRlZDxib29sZWFuPlxyXG5cclxuICAgIHB1YmxpYyBTYW1wbGVSYXRlU2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxTYW1wbGVSYXRlRW51bT5cclxuXHJcbiAgICBwcml2YXRlIF9vblByb3BlcnR5Q2hhbmdlZDogRXZlbnREaXNwYXRjaGVyPEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWwsIGFueT5cclxuXHJcbiAgICBjb25zdHJ1Y3Rvcihtb2RlbDogQnVmZmVyUmVxdWVzdE1vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuU2FtcGxlUmF0ZVNldCA9IGtvLm9ic2VydmFibGVBcnJheTxTYW1wbGVSYXRlRW51bT4oRW51bWVyYXRpb25IZWxwZXIuR2V0RW51bVZhbHVlcyhcIlNhbXBsZVJhdGVFbnVtXCIpKVxyXG4gICAgICAgIHRoaXMuU2FtcGxlUmF0ZSA9IGtvLm9ic2VydmFibGU8U2FtcGxlUmF0ZUVudW0+KG1vZGVsLlNhbXBsZVJhdGUpO1xyXG4gICAgICAgIHRoaXMuR3JvdXBGaWx0ZXIgPSBrby5vYnNlcnZhYmxlPHN0cmluZz4obW9kZWwuR3JvdXBGaWx0ZXIpO1xyXG5cclxuICAgICAgICB0aGlzLkVycm9yTWVzc2FnZSA9IGtvLm9ic2VydmFibGU8c3RyaW5nPihcIlwiKVxyXG4gICAgICAgIHRoaXMuSGFzRXJyb3IgPSBrby5jb21wdXRlZDxib29sZWFuPigoKSA9PiB0aGlzLkVycm9yTWVzc2FnZSgpLmxlbmd0aCA+IDApXHJcblxyXG4gICAgICAgIHRoaXMuX29uUHJvcGVydHlDaGFuZ2VkID0gbmV3IEV2ZW50RGlzcGF0Y2hlcjxCdWZmZXJSZXF1ZXN0Vmlld01vZGVsLCBhbnk+KCk7XHJcblxyXG4gICAgICAgIHRoaXMuU2FtcGxlUmF0ZS5zdWJzY3JpYmUobmV3VmFsdWUgPT4gdGhpcy5PblByb3BlcnR5Q2hhbmdlZCgpKVxyXG4gICAgICAgIHRoaXMuR3JvdXBGaWx0ZXIuc3Vic2NyaWJlKG5ld1ZhbHVlID0+IHRoaXMuT25Qcm9wZXJ0eUNoYW5nZWQoKSlcclxuICAgIH1cclxuXHJcbiAgICBnZXQgUHJvcGVydHlDaGFuZ2VkKCk6IElFdmVudDxCdWZmZXJSZXF1ZXN0Vmlld01vZGVsLCBhbnk+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuX29uUHJvcGVydHlDaGFuZ2VkO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBPblByb3BlcnR5Q2hhbmdlZCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5WYWxpZGF0ZSgpXHJcbiAgICAgICAgdGhpcy5fb25Qcm9wZXJ0eUNoYW5nZWQuZGlzcGF0Y2godGhpcywgbnVsbClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVmFsaWRhdGUoKVxyXG4gICAge1xyXG4gICAgICAgIGxldCByZXN1bHQ6IGFueVxyXG5cclxuICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIlwiKVxyXG5cclxuICAgICAgICB0aGlzLkdyb3VwRmlsdGVyKCkuc3BsaXQoXCI7XCIpLmZvckVhY2goZ3JvdXBGaWx0ZXIgPT5cclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHJlc3VsdCA9IHRoaXMuQ2hlY2tHcm91cEZpbHRlcihncm91cEZpbHRlcilcclxuXHJcbiAgICAgICAgICAgIGlmIChyZXN1bHQuSGFzRXJyb3IpXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKHJlc3VsdC5FcnJvckRlc2NyaXB0aW9uKVxyXG5cclxuICAgICAgICAgICAgICAgIHJldHVyblxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9TdHJpbmcoKVxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiBcInNhbXBsZSByYXRlOiBcIiArIEVudW1lcmF0aW9uSGVscGVyLkdldEVudW1Mb2NhbGl6YXRpb24oXCJTYW1wbGVSYXRlRW51bVwiLCB0aGlzLlNhbXBsZVJhdGUoKSkgKyBcIiAvIGdyb3VwIGZpbHRlcjogJ1wiICsgdGhpcy5Hcm91cEZpbHRlcigpICsgXCInXCJcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IG1vZGVsOiBhbnkgPSB7XHJcbiAgICAgICAgICAgIFNhbXBsZVJhdGU6IDxTYW1wbGVSYXRlRW51bT50aGlzLlNhbXBsZVJhdGUoKSxcclxuICAgICAgICAgICAgR3JvdXBGaWx0ZXI6IDxzdHJpbmc+dGhpcy5Hcm91cEZpbHRlcigpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gbW9kZWxcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIENoZWNrR3JvdXBGaWx0ZXIodmFsdWU6IHN0cmluZylcclxuICAgIHtcclxuICAgICAgICB2YXIgcmVnRXhwOiBhbnlcclxuXHJcbiAgICAgICAgaWYgKHZhbHVlLmxlbmd0aCA9PT0gMCkge1xyXG4gICAgICAgICAgICByZXR1cm4geyBIYXNFcnJvcjogdHJ1ZSwgRXJyb3JEZXNjcmlwdGlvbjogRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9Hcm91cEZpbHRlckVtcHR5XCJdIH1cclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJlZ0V4cCA9IG5ldyBSZWdFeHAoXCJbXkEtWmEtejAtOV8hKl1cIilcclxuXHJcbiAgICAgICAgaWYgKHJlZ0V4cC50ZXN0KHZhbHVlKSkge1xyXG4gICAgICAgICAgICByZXR1cm4geyBIYXNFcnJvcjogdHJ1ZSwgRXJyb3JEZXNjcmlwdGlvbjogRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9JbnZhbGlkQ2hhcmFjdGVyc1wiXSB9XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZWdFeHAgPSBuZXcgUmVnRXhwKFwiXlswLTlfXVwiKVxyXG5cclxuICAgICAgICBpZiAocmVnRXhwLnRlc3QodmFsdWUpKSB7XHJcbiAgICAgICAgICAgIHJldHVybiB7IEhhc0Vycm9yOiB0cnVlLCBFcnJvckRlc2NyaXB0aW9uOiBFcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0ludmFsaWRMZWFkaW5nQ2hhcmFjdGVyXCJdIH1cclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJlZ0V4cCA9IG5ldyBSZWdFeHAoXCJeIVwiKVxyXG5cclxuICAgICAgICBpZiAocmVnRXhwLnRlc3QodmFsdWUpICYmIHZhbHVlLmxlbmd0aCA9PT0gMSkge1xyXG4gICAgICAgICAgICByZXR1cm4geyBIYXNFcnJvcjogdHJ1ZSwgRXJyb3JEZXNjcmlwdGlvbjogRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9EZXRhY2hlZEV4Y2xhbWF0aW9uTWFya05vdEFsbG93ZWRcIl0gfVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmV0dXJuIHtcclxuICAgICAgICAgICAgSGFzRXJyb3I6IGZhbHNlLFxyXG4gICAgICAgICAgICBFcnJvckRlc2NyaXB0aW9uOiBcIlwiXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG59IiwiY2xhc3MgRGF0YVBvcnRWaWV3TW9kZWxcclxue1xyXG4gICAgLy8gZmllbGRzXHJcbiAgICBwdWJsaWMgTmFtZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyByZWFkb25seSBEYXRhVHlwZTogT25lRGFzRGF0YVR5cGVFbnVtXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRGF0YURpcmVjdGlvbjogRGF0YURpcmVjdGlvbkVudW1cclxuICAgIHB1YmxpYyByZWFkb25seSBFbmRpYW5uZXNzOiBFbmRpYW5uZXNzRW51bVxyXG5cclxuICAgIHB1YmxpYyBJc1NlbGVjdGVkOiBLbm9ja291dE9ic2VydmFibGU8Ym9vbGVhbj5cclxuICAgIHB1YmxpYyBBc3NvY2lhdGVkQ2hhbm5lbEh1YlNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8Q2hhbm5lbEh1YlZpZXdNb2RlbD5cclxuICAgIHB1YmxpYyByZWFkb25seSBBc3NvY2lhdGVkRGF0YUdhdGV3YXk6IERhdGFHYXRld2F5Vmlld01vZGVsQmFzZVxyXG4gICAgcHVibGljIHJlYWRvbmx5IExpdmVEZXNjcmlwdGlvbjogS25vY2tvdXRDb21wdXRlZDxzdHJpbmc+XHJcblxyXG4gICAgLy8gY29uc3RydWN0b3JzXHJcbiAgICBjb25zdHJ1Y3RvcihkYXRhUG9ydE1vZGVsOiBhbnksIGFzc29jaWF0ZWREYXRhR2F0ZXdheTogRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuTmFtZSA9IGtvLm9ic2VydmFibGUoZGF0YVBvcnRNb2RlbC5OYW1lKVxyXG4gICAgICAgIHRoaXMuRGF0YVR5cGUgPSBkYXRhUG9ydE1vZGVsLkRhdGFUeXBlXHJcbiAgICAgICAgdGhpcy5EYXRhRGlyZWN0aW9uID0gZGF0YVBvcnRNb2RlbC5EYXRhRGlyZWN0aW9uXHJcbiAgICAgICAgdGhpcy5FbmRpYW5uZXNzID0gZGF0YVBvcnRNb2RlbC5FbmRpYW5uZXNzXHJcblxyXG4gICAgICAgIHRoaXMuSXNTZWxlY3RlZCA9IGtvLm9ic2VydmFibGU8Ym9vbGVhbj4oZmFsc2UpXHJcbiAgICAgICAgdGhpcy5Bc3NvY2lhdGVkQ2hhbm5lbEh1YlNldCA9IGtvLm9ic2VydmFibGVBcnJheTxDaGFubmVsSHViVmlld01vZGVsPigpXHJcbiAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YUdhdGV3YXkgPSBhc3NvY2lhdGVkRGF0YUdhdGV3YXlcclxuXHJcbiAgICAgICAgdGhpcy5MaXZlRGVzY3JpcHRpb24gPSBrby5jb21wdXRlZCgoKSA9PlxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgbGV0IHJlc3VsdDogc3RyaW5nXHJcblxyXG4gICAgICAgICAgICByZXN1bHQgPSBcIjxkaXYgY2xhc3M9J3RleHQtbGVmdCc+XCIgKyB0aGlzLk5hbWUoKSArIFwiPC9kaXY+PGRpdiBjbGFzcz0ndGV4dC1sZWZ0Jz5cIiArIEVudW1lcmF0aW9uSGVscGVyLkdldEVudW1Mb2NhbGl6YXRpb24oJ09uZURhc0RhdGFUeXBlRW51bScsIHRoaXMuRGF0YVR5cGUpICsgXCI8L2Rpdj5cIlxyXG5cclxuICAgICAgICAgICAgaWYgKHRoaXMuQXNzb2NpYXRlZENoYW5uZWxIdWJTZXQoKS5sZW5ndGggPiAwKVxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWRDaGFubmVsSHViU2V0KCkuZm9yRWFjaChjaGFubmVsSHViID0+XHJcbiAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgcmVzdWx0ICs9IFwiPC9iciA+PGRpdiBjbGFzcz0ndGV4dC1sZWZ0Jz5cIiArIGNoYW5uZWxIdWIuTmFtZSgpICsgXCI8L2Rpdj48ZGl2IGNsYXNzPSd0ZXh0LWxlZnQnPlwiICsgRW51bWVyYXRpb25IZWxwZXIuR2V0RW51bUxvY2FsaXphdGlvbignT25lRGFzRGF0YVR5cGVFbnVtJywgY2hhbm5lbEh1Yi5EYXRhVHlwZSgpKSArIFwiPC9kaXY+XCJcclxuICAgICAgICAgICAgICAgIH0pXHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIHJldHVybiByZXN1bHRcclxuICAgICAgICB9KVxyXG4gICAgfVxyXG5cclxuICAgIC8vIG1ldGhvZHNcclxuICAgIHB1YmxpYyBHZXRJZCgpOiBzdHJpbmdcclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5OYW1lKClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9GdWxsUXVhbGlmaWVkSWRlbnRpZmllcigpOiBzdHJpbmdcclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5Bc3NvY2lhdGVkRGF0YUdhdGV3YXkuRGVzY3JpcHRpb24uSWQgKyBcIiAoXCIgKyB0aGlzLkFzc29jaWF0ZWREYXRhR2F0ZXdheS5EZXNjcmlwdGlvbi5JbnN0YW5jZUlkICsgXCIpIC8gXCIgKyB0aGlzLkdldElkKCk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEV4dGVuZE1vZGVsKG1vZGVsOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgLy9cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IG1vZGVsOiBhbnkgPSB7XHJcbiAgICAgICAgICAgIE5hbWU6IDxzdHJpbmc+dGhpcy5OYW1lKCksXHJcbiAgICAgICAgICAgIERhdGFUeXBlOiA8T25lRGFzRGF0YVR5cGVFbnVtPnRoaXMuRGF0YVR5cGUsXHJcbiAgICAgICAgICAgIERhdGFEaXJlY3Rpb246IDxEYXRhRGlyZWN0aW9uRW51bT50aGlzLkRhdGFEaXJlY3Rpb25cclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuRXh0ZW5kTW9kZWwobW9kZWwpXHJcblxyXG4gICAgICAgIHJldHVybiBtb2RlbFxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBSZXNldEFzc29jaWF0aW9ucyhtYWludGFpbldlYWtSZWZlcmVuY2U6IGJvb2xlYW4pXHJcbiAgICB7XHJcbiAgICAgICAgaWYgKHRoaXMuQXNzb2NpYXRlZENoYW5uZWxIdWJTZXQoKS5sZW5ndGggPiAwKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkQ2hhbm5lbEh1YlNldCgpLmZvckVhY2goY2hhbm5lbEh1YiA9PiBjaGFubmVsSHViLlJlc2V0QXNzb2NpYXRpb24obWFpbnRhaW5XZWFrUmVmZXJlbmNlLCB0aGlzKSlcclxuICAgICAgICB9XHJcbiAgICB9XHJcbn0iLCJhYnN0cmFjdCBjbGFzcyBFeHRlbnNpb25WaWV3TW9kZWxCYXNlXHJcbntcclxuICAgIHB1YmxpYyBEZXNjcmlwdGlvbjogRXh0ZW5zaW9uRGVzY3JpcHRpb25WaWV3TW9kZWxcclxuICAgIHB1YmxpYyBFeHRlbnNpb25JZGVudGlmaWNhdGlvbjogRXh0ZW5zaW9uSWRlbnRpZmljYXRpb25WaWV3TW9kZWxcclxuICAgIHB1YmxpYyBJc0luU2V0dGluZ3NNb2RlOiBLbm9ja291dE9ic2VydmFibGU8Ym9vbGVhbj5cclxuXHJcbiAgICBwcml2YXRlIF9tb2RlbDogYW55XHJcblxyXG4gICAgY29uc3RydWN0b3IoZXh0ZW5zaW9uU2V0dGluZ3NNb2RlbDogYW55LCBleHRlbnNpb25JZGVudGlmaWNhdGlvbjogRXh0ZW5zaW9uSWRlbnRpZmljYXRpb25WaWV3TW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5fbW9kZWwgPSBleHRlbnNpb25TZXR0aW5nc01vZGVsXHJcbiAgICAgICAgdGhpcy5EZXNjcmlwdGlvbiA9IG5ldyBFeHRlbnNpb25EZXNjcmlwdGlvblZpZXdNb2RlbChleHRlbnNpb25TZXR0aW5nc01vZGVsLkRlc2NyaXB0aW9uKVxyXG4gICAgICAgIHRoaXMuRXh0ZW5zaW9uSWRlbnRpZmljYXRpb24gPSBleHRlbnNpb25JZGVudGlmaWNhdGlvblxyXG4gICAgICAgIHRoaXMuSXNJblNldHRpbmdzTW9kZSA9IGtvLm9ic2VydmFibGU8Ym9vbGVhbj4oZmFsc2UpXHJcbiAgICB9XHJcblxyXG4gICAgLy8gbWV0aG9kc1xyXG4gICAgcHVibGljIGFic3RyYWN0IGFzeW5jIEluaXRpYWxpemVBc3luYygpOiBQcm9taXNlPGFueT5cclxuXHJcbiAgICBwdWJsaWMgU2VuZEFjdGlvblJlcXVlc3QgPSBhc3luYyAoaW5zdGFuY2VJZDogbnVtYmVyLCBtZXRob2ROYW1lOiBzdHJpbmcsIGRhdGE6IGFueSkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gPEFjdGlvblJlc3BvbnNlPiBhd2FpdCBDb25uZWN0aW9uTWFuYWdlci5JbnZva2VXZWJDbGllbnRIdWIoXCJSZXF1ZXN0QWN0aW9uXCIsIG5ldyBBY3Rpb25SZXF1ZXN0KHRoaXMuRGVzY3JpcHRpb24uSWQsIGluc3RhbmNlSWQsIG1ldGhvZE5hbWUsIGRhdGEpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBFeHRlbmRNb2RlbChtb2RlbDogYW55KVxyXG4gICAge1xyXG4gICAgICAgIC8vXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvTW9kZWwoKVxyXG4gICAge1xyXG4gICAgICAgIGxldCBtb2RlbDogYW55ID0ge1xyXG4gICAgICAgICAgICAkdHlwZTogPHN0cmluZz50aGlzLl9tb2RlbC4kdHlwZSxcclxuICAgICAgICAgICAgRGVzY3JpcHRpb246IDxFeHRlbnNpb25EZXNjcmlwdGlvblZpZXdNb2RlbD50aGlzLkRlc2NyaXB0aW9uLlRvTW9kZWwoKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5FeHRlbmRNb2RlbChtb2RlbClcclxuXHJcbiAgICAgICAgcmV0dXJuIG1vZGVsXHJcbiAgICB9XHJcblxyXG4gICAgLy8gY29tbWFuZHNcclxuICAgIHB1YmxpYyBFbmFibGVTZXR0aW5nc01vZGUgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuSXNJblNldHRpbmdzTW9kZSh0cnVlKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBEaXNhYmxlU2V0dGluZ3NNb2RlID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLklzSW5TZXR0aW5nc01vZGUoZmFsc2UpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvZ2dsZVNldHRpbmdzTW9kZSA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5Jc0luU2V0dGluZ3NNb2RlKCF0aGlzLklzSW5TZXR0aW5nc01vZGUoKSlcclxuICAgIH1cclxufSIsIi8vLyA8cmVmZXJlbmNlIHBhdGg9XCJFeHRlbnNpb25WaWV3TW9kZWxCYXNlLnRzXCIvPlxyXG5cclxuYWJzdHJhY3QgY2xhc3MgRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlIGV4dGVuZHMgRXh0ZW5zaW9uVmlld01vZGVsQmFzZVxyXG57XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgTWF4aW11bURhdGFzZXRBZ2U6IEtub2Nrb3V0T2JzZXJ2YWJsZTxudW1iZXI+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRGF0YVBvcnRTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PERhdGFQb3J0Vmlld01vZGVsPlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKG1vZGVsLCBpZGVudGlmaWNhdGlvbjogRXh0ZW5zaW9uSWRlbnRpZmljYXRpb25WaWV3TW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgc3VwZXIobW9kZWwsIGlkZW50aWZpY2F0aW9uKVxyXG5cclxuICAgICAgICB0aGlzLk1heGltdW1EYXRhc2V0QWdlID0ga28ub2JzZXJ2YWJsZTxudW1iZXI+KG1vZGVsLk1heGltdW1EYXRhc2V0QWdlKVxyXG4gICAgICAgIHRoaXMuRGF0YVBvcnRTZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8RGF0YVBvcnRWaWV3TW9kZWw+KClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgRXh0ZW5kTW9kZWwobW9kZWw6IGFueSlcclxuICAgIHtcclxuICAgICAgICBzdXBlci5FeHRlbmRNb2RlbChtb2RlbClcclxuXHJcbiAgICAgICAgbW9kZWwuTWF4aW11bURhdGFzZXRBZ2UgPSA8bnVtYmVyPk51bWJlci5wYXJzZUludCg8YW55PnRoaXMuTWF4aW11bURhdGFzZXRBZ2UoKSlcclxuICAgIH1cclxufSIsImFic3RyYWN0IGNsYXNzIEV4dGVuZGVkRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlIGV4dGVuZHMgRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlXHJcbntcclxuICAgIHB1YmxpYyBNb2R1bGVUb0RhdGFQb3J0TWFwOiBLbm9ja291dE9ic2VydmFibGVBcnJheTxPYnNlcnZhYmxlR3JvdXA8RGF0YVBvcnRWaWV3TW9kZWw+PlxyXG4gICAgcHVibGljIE9uZURhc01vZHVsZVNlbGVjdG9yOiBLbm9ja291dE9ic2VydmFibGU8T25lRGFzTW9kdWxlU2VsZWN0b3JWaWV3TW9kZWw+XHJcblxyXG4gICAgY29uc3RydWN0b3IobW9kZWwsIGlkZW50aWZpY2F0aW9uOiBFeHRlbnNpb25JZGVudGlmaWNhdGlvblZpZXdNb2RlbCwgb25lRGFzTW9kdWxlU2VsZWN0b3I6IE9uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHN1cGVyKG1vZGVsLCBpZGVudGlmaWNhdGlvbilcclxuXHJcbiAgICAgICAgdGhpcy5Nb2R1bGVUb0RhdGFQb3J0TWFwID0ga28ub2JzZXJ2YWJsZUFycmF5KClcclxuICAgICAgICB0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yID0ga28ub2JzZXJ2YWJsZTxPbmVEYXNNb2R1bGVTZWxlY3RvclZpZXdNb2RlbD4ob25lRGFzTW9kdWxlU2VsZWN0b3IpXHJcblxyXG4gICAgICAgIGlmICh0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yKCkuT25Nb2R1bGVTZXRDaGFuZ2VkLnN1YnNjcmliZSgoc2VuZGVyLCBhcmdzKSA9PlxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICB0aGlzLlVwZGF0ZURhdGFQb3J0U2V0KClcclxuICAgICAgICAgICAgfSlcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIGFzeW5jIEluaXRpYWxpemVBc3luYygpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5VcGRhdGVEYXRhUG9ydFNldCgpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFVwZGF0ZURhdGFQb3J0U2V0KClcclxuICAgIHtcclxuICAgICAgICBsZXQgaW5kZXg6IG51bWJlclxyXG4gICAgICAgIGxldCBtb2R1bGVUb0RhdGFQb3J0TWFwOiBPYnNlcnZhYmxlR3JvdXA8RGF0YVBvcnRWaWV3TW9kZWw+W11cclxuXHJcbiAgICAgICAgbW9kdWxlVG9EYXRhUG9ydE1hcCA9IFtdXHJcblxyXG4gICAgICAgIC8vIGlucHV0c1xyXG4gICAgICAgIGluZGV4ID0gMFxyXG5cclxuICAgICAgICBtb2R1bGVUb0RhdGFQb3J0TWFwID0gbW9kdWxlVG9EYXRhUG9ydE1hcC5jb25jYXQodGhpcy5PbmVEYXNNb2R1bGVTZWxlY3RvcigpLk1vZHVsZVNldCgpLmZpbHRlcihvbmVEYXNNb2R1bGUgPT4gb25lRGFzTW9kdWxlLkRhdGFEaXJlY3Rpb24oKSA9PT0gRGF0YURpcmVjdGlvbkVudW0uSW5wdXQpLm1hcChvbmVEYXNNb2R1bGUgPT5cclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGxldCBncm91cDogT2JzZXJ2YWJsZUdyb3VwPERhdGFQb3J0Vmlld01vZGVsPlxyXG5cclxuICAgICAgICAgICAgZ3JvdXAgPSBuZXcgT2JzZXJ2YWJsZUdyb3VwPERhdGFQb3J0Vmlld01vZGVsPihvbmVEYXNNb2R1bGUuVG9TdHJpbmcoKSwgdGhpcy5DcmVhdGVEYXRhUG9ydFNldChvbmVEYXNNb2R1bGUsIGluZGV4KSlcclxuICAgICAgICAgICAgaW5kZXggKz0gb25lRGFzTW9kdWxlLlNpemUoKTtcclxuXHJcbiAgICAgICAgICAgIHJldHVybiBncm91cFxyXG4gICAgICAgIH0pKVxyXG5cclxuICAgICAgICAvLyBvdXRwdXRzXHJcbiAgICAgICAgaW5kZXggPSAwXHJcblxyXG4gICAgICAgIG1vZHVsZVRvRGF0YVBvcnRNYXAgPSBtb2R1bGVUb0RhdGFQb3J0TWFwLmNvbmNhdCh0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yKCkuTW9kdWxlU2V0KCkuZmlsdGVyKG9uZURhc01vZHVsZSA9PiBvbmVEYXNNb2R1bGUuRGF0YURpcmVjdGlvbigpID09PSBEYXRhRGlyZWN0aW9uRW51bS5PdXRwdXQpLm1hcChvbmVEYXNNb2R1bGUgPT5cclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGxldCBncm91cDogT2JzZXJ2YWJsZUdyb3VwPERhdGFQb3J0Vmlld01vZGVsPlxyXG5cclxuICAgICAgICAgICAgZ3JvdXAgPSBuZXcgT2JzZXJ2YWJsZUdyb3VwPERhdGFQb3J0Vmlld01vZGVsPihvbmVEYXNNb2R1bGUuVG9TdHJpbmcoKSwgdGhpcy5DcmVhdGVEYXRhUG9ydFNldChvbmVEYXNNb2R1bGUsIGluZGV4KSlcclxuICAgICAgICAgICAgaW5kZXggKz0gb25lRGFzTW9kdWxlLlNpemUoKTtcclxuXHJcbiAgICAgICAgICAgIHJldHVybiBncm91cFxyXG4gICAgICAgIH0pKVxyXG5cclxuICAgICAgICB0aGlzLk1vZHVsZVRvRGF0YVBvcnRNYXAobW9kdWxlVG9EYXRhUG9ydE1hcClcclxuICAgICAgICB0aGlzLkRhdGFQb3J0U2V0KE1hcE1hbnkodGhpcy5Nb2R1bGVUb0RhdGFQb3J0TWFwKCksIGdyb3VwID0+IGdyb3VwLk1lbWJlcnMoKSkpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIENyZWF0ZURhdGFQb3J0U2V0KG9uZURhc01vZHVsZTogT25lRGFzTW9kdWxlVmlld01vZGVsLCBpbmRleDogbnVtYmVyKVxyXG4gICAge1xyXG4gICAgICAgIGxldCBwcmVmaXg6IHN0cmluZ1xyXG5cclxuICAgICAgICBzd2l0Y2ggKG9uZURhc01vZHVsZS5EYXRhRGlyZWN0aW9uKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLklucHV0OlxyXG4gICAgICAgICAgICAgICAgcHJlZml4ID0gXCJJbnB1dFwiXHJcbiAgICAgICAgICAgICAgICBicmVha1xyXG5cclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5PdXRwdXQ6XHJcbiAgICAgICAgICAgICAgICBwcmVmaXggPSBcIk91dHB1dFwiXHJcbiAgICAgICAgICAgICAgICBicmVha1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmV0dXJuIEFycmF5LmZyb20obmV3IEFycmF5KG9uZURhc01vZHVsZS5TaXplKCkpLCAoeCwgaSkgPT4gXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICByZXR1cm4ge1xyXG4gICAgICAgICAgICAgICAgTmFtZTogPHN0cmluZz5wcmVmaXggKyBcIiBcIiArIChpbmRleCArIGkpLFxyXG4gICAgICAgICAgICAgICAgRGF0YVR5cGU6IDxPbmVEYXNEYXRhVHlwZUVudW0+b25lRGFzTW9kdWxlLkRhdGFUeXBlKCksXHJcbiAgICAgICAgICAgICAgICBEYXRhRGlyZWN0aW9uOiA8RGF0YURpcmVjdGlvbkVudW0+b25lRGFzTW9kdWxlLkRhdGFEaXJlY3Rpb24oKVxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfSkubWFwKGRhdGFQb3J0TW9kZWwgPT4gbmV3IERhdGFQb3J0Vmlld01vZGVsKGRhdGFQb3J0TW9kZWwsIHRoaXMpKVxyXG4gICAgfVxyXG59IiwiLy8vIDxyZWZlcmVuY2UgcGF0aD1cIkV4dGVuc2lvblZpZXdNb2RlbEJhc2UudHNcIi8+XHJcblxyXG5hYnN0cmFjdCBjbGFzcyBEYXRhV3JpdGVyVmlld01vZGVsQmFzZSBleHRlbmRzIEV4dGVuc2lvblZpZXdNb2RlbEJhc2Vcclxue1xyXG4gICAgcHVibGljIHJlYWRvbmx5IEZpbGVHcmFudWxhcml0eTogS25vY2tvdXRPYnNlcnZhYmxlPEZpbGVHcmFudWxhcml0eUVudW0+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgQnVmZmVyUmVxdWVzdFNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8QnVmZmVyUmVxdWVzdFZpZXdNb2RlbD5cclxuICAgIHB1YmxpYyByZWFkb25seSBCdWZmZXJSZXF1ZXN0U2VsZWN0b3I6IEtub2Nrb3V0T2JzZXJ2YWJsZTxCdWZmZXJSZXF1ZXN0U2VsZWN0b3JWaWV3TW9kZWw+XHJcblxyXG4gICAgY29uc3RydWN0b3IobW9kZWwsIGlkZW50aWZpY2F0aW9uOiBFeHRlbnNpb25JZGVudGlmaWNhdGlvblZpZXdNb2RlbClcclxuICAgIHtcclxuICAgICAgICBzdXBlcihtb2RlbCwgaWRlbnRpZmljYXRpb24pXHJcblxyXG4gICAgICAgIHRoaXMuRmlsZUdyYW51bGFyaXR5ID0ga28ub2JzZXJ2YWJsZTxGaWxlR3JhbnVsYXJpdHlFbnVtPihtb2RlbC5GaWxlR3JhbnVsYXJpdHkpXHJcbiAgICAgICAgdGhpcy5CdWZmZXJSZXF1ZXN0U2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWw+KG1vZGVsLkJ1ZmZlclJlcXVlc3RTZXQubWFwKGJ1ZmZlclJlcXVlc3QgPT4gbmV3IEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWwoYnVmZmVyUmVxdWVzdCkpKVxyXG5cclxuICAgICAgICB0aGlzLkJ1ZmZlclJlcXVlc3RTZWxlY3RvciA9IGtvLm9ic2VydmFibGU8QnVmZmVyUmVxdWVzdFNlbGVjdG9yVmlld01vZGVsPihuZXcgQnVmZmVyUmVxdWVzdFNlbGVjdG9yVmlld01vZGVsKHRoaXMuQnVmZmVyUmVxdWVzdFNldCgpKSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgRXh0ZW5kTW9kZWwobW9kZWw6IGFueSlcclxuICAgIHtcclxuICAgICAgICBzdXBlci5FeHRlbmRNb2RlbChtb2RlbClcclxuXHJcbiAgICAgICAgbW9kZWwuRmlsZUdyYW51bGFyaXR5ID0gPEZpbGVHcmFudWxhcml0eUVudW0+dGhpcy5GaWxlR3JhbnVsYXJpdHkoKVxyXG4gICAgICAgIG1vZGVsLkJ1ZmZlclJlcXVlc3RTZXQgPSA8QnVmZmVyUmVxdWVzdE1vZGVsW10+dGhpcy5CdWZmZXJSZXF1ZXN0U2V0KCkubWFwKGJ1ZmZlclJlcXVlc3QgPT4gYnVmZmVyUmVxdWVzdC5Ub01vZGVsKCkpXHJcbiAgICB9XHJcbn0iLCJjbGFzcyBFeHRlbnNpb25EZXNjcmlwdGlvblZpZXdNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgUHJvZHVjdFZlcnNpb246IG51bWJlclxyXG4gICAgcHVibGljIElkOiBzdHJpbmdcclxuICAgIHB1YmxpYyBJbnN0YW5jZUlkOiBudW1iZXJcclxuICAgIHB1YmxpYyBJbnN0YW5jZU5hbWU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgSXNFbmFibGVkOiBLbm9ja291dE9ic2VydmFibGU8Ym9vbGVhbj5cclxuXHJcbiAgICBjb25zdHJ1Y3RvcihleHRlbnNpb25EZXNjcmlwdGlvbk1vZGVsOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5Qcm9kdWN0VmVyc2lvbiA9IGV4dGVuc2lvbkRlc2NyaXB0aW9uTW9kZWwuUHJvZHVjdFZlcnNpb25cclxuICAgICAgICB0aGlzLklkID0gZXh0ZW5zaW9uRGVzY3JpcHRpb25Nb2RlbC5JZFxyXG4gICAgICAgIHRoaXMuSW5zdGFuY2VJZCA9IGV4dGVuc2lvbkRlc2NyaXB0aW9uTW9kZWwuSW5zdGFuY2VJZFxyXG4gICAgICAgIHRoaXMuSW5zdGFuY2VOYW1lID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KGV4dGVuc2lvbkRlc2NyaXB0aW9uTW9kZWwuSW5zdGFuY2VOYW1lKVxyXG4gICAgICAgIHRoaXMuSXNFbmFibGVkID0ga28ub2JzZXJ2YWJsZTxib29sZWFuPihleHRlbnNpb25EZXNjcmlwdGlvbk1vZGVsLklzRW5hYmxlZClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpXHJcbiAgICB7XHJcbiAgICAgICAgdmFyIG1vZGVsOiBhbnkgPSB7XHJcbiAgICAgICAgICAgIFByb2R1Y3RWZXJzaW9uOiA8bnVtYmVyPnRoaXMuUHJvZHVjdFZlcnNpb24sXHJcbiAgICAgICAgICAgIElkOiA8c3RyaW5nPnRoaXMuSWQsXHJcbiAgICAgICAgICAgIEluc3RhbmNlSWQ6IDxudW1iZXI+dGhpcy5JbnN0YW5jZUlkLFxyXG4gICAgICAgICAgICBJbnN0YW5jZU5hbWU6IDxzdHJpbmc+dGhpcy5JbnN0YW5jZU5hbWUoKSxcclxuICAgICAgICAgICAgSXNFbmFibGVkOiA8Ym9vbGVhbj50aGlzLklzRW5hYmxlZCgpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gbW9kZWxcclxuICAgIH1cclxufSIsImNsYXNzIEV4dGVuc2lvbklkZW50aWZpY2F0aW9uVmlld01vZGVsXHJcbntcclxuICAgIHB1YmxpYyBQcm9kdWN0VmVyc2lvbjogc3RyaW5nXHJcbiAgICBwdWJsaWMgSWQ6IHN0cmluZ1xyXG4gICAgcHVibGljIE5hbWU6IHN0cmluZ1xyXG4gICAgcHVibGljIERlc2NyaXB0aW9uOiBzdHJpbmdcclxuICAgIHB1YmxpYyBWaWV3UmVzb3VyY2VOYW1lOiBzdHJpbmdcclxuICAgIHB1YmxpYyBWaWV3TW9kZWxSZXNvdXJjZU5hbWU6IHN0cmluZ1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKGV4dGVuc2lvbklkZW50aWZpY2F0aW9uTW9kZWw6IGFueSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLlByb2R1Y3RWZXJzaW9uID0gZXh0ZW5zaW9uSWRlbnRpZmljYXRpb25Nb2RlbC5Qcm9kdWN0VmVyc2lvblxyXG4gICAgICAgIHRoaXMuSWQgPSBleHRlbnNpb25JZGVudGlmaWNhdGlvbk1vZGVsLklkXHJcbiAgICAgICAgdGhpcy5OYW1lID0gZXh0ZW5zaW9uSWRlbnRpZmljYXRpb25Nb2RlbC5OYW1lXHJcbiAgICAgICAgdGhpcy5EZXNjcmlwdGlvbiA9IGV4dGVuc2lvbklkZW50aWZpY2F0aW9uTW9kZWwuRGVzY3JpcHRpb25cclxuICAgICAgICB0aGlzLlZpZXdSZXNvdXJjZU5hbWUgPSBleHRlbnNpb25JZGVudGlmaWNhdGlvbk1vZGVsLlZpZXdSZXNvdXJjZU5hbWVcclxuICAgICAgICB0aGlzLlZpZXdNb2RlbFJlc291cmNlTmFtZSA9IGV4dGVuc2lvbklkZW50aWZpY2F0aW9uTW9kZWwuVmlld01vZGVsUmVzb3VyY2VOYW1lXHJcbiAgICB9XHJcbn0iXX0=