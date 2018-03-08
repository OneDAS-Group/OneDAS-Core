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
        this.DataType = ko.observable(oneDasModuleModel.DataType);
        this.DataDirection = ko.observable(oneDasModuleModel.DataDirection);
        this.Endianness = ko.observable(oneDasModuleModel.Endianness);
        this.Size = ko.observable(oneDasModuleModel.Size);
        this.MaxSize = ko.observable(Infinity);
        this.ErrorMessage = ko.observable("");
        this.HasError = ko.computed(() => this.ErrorMessage().length > 0);
        this.DataTypeSet = ko.observableArray(EnumerationHelper.GetEnumValues('OneDasDataTypeEnum').filter(dataType => dataType !== OneDasDataTypeEnum.BOOLEAN));
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
        this.SettingsTemplateName = ko.observable("Project_OneDasModuleSettingsTemplate");
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
class BufferRequestViewModel {
    constructor(model) {
        this.SampleRate = ko.observable(model.SampleRate);
        this.GroupFilter = ko.observable(model.GroupFilter);
    }
    ToModel() {
        return {
            SampleRate: this.SampleRate(),
            GroupFilter: this.GroupFilter()
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
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiT25lRGFzLlR5cGVzLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vT25lRGFzLlR5cGVzL0RhdGFEaXJlY3Rpb25FbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL0VuZGlhbm5lc3NFbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL0ZpbGVHcmFudWxhcml0eUVudW0udHMiLCIuLi9PbmVEYXMuVHlwZXMvT25lRGFzRGF0YVR5cGVFbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL09uZURhc1N0YXRlRW51bS50cyIsIi4uL09uZURhcy5UeXBlcy9TYW1wbGVSYXRlRW51bS50cyIsIi4uL0NvcmUvQWN0aW9uUmVxdWVzdC50cyIsIi4uL0NvcmUvQWN0aW9uUmVzcG9uc2UudHMiLCIuLi9Db3JlL0V2ZW50RGlzcGF0Y2hlci50cyIsIi4uL0NvcmUvSUV2ZW50LnRzIiwiLi4vQ29yZS9PbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtLnRzIiwiLi4vTW9kZWxzL0J1ZmZlclJlcXVlc3RNb2RlbC50cyIsIi4uL01vZGVscy9DaGFubmVsSHViTW9kZWwudHMiLCIuLi9Nb2RlbHMvT25lRGFzTW9kdWxlTW9kZWwudHMiLCIuLi9Nb2RlbHMvVHJhbnNmZXJGdW5jdGlvbk1vZGVsLnRzIiwiLi4vU3RhdGljL0Nvbm5lY3Rpb25NYW5hZ2VyLnRzIiwiLi4vU3RhdGljL0VudW1lcmF0aW9uSGVscGVyLnRzIiwiLi4vU3RhdGljL0Vycm9yTWVzc2FnZS50cyIsIi4uL1N0YXRpYy9IZWxwZXIudHMiLCIuLi9TdGF0aWMvUGx1Z2luRmFjdG9yeS50cyIsIi4uL1N0YXRpYy9QbHVnaW5IaXZlLnRzIiwiLi4vVmlld01vZGVscy9Db3JlL0NoYW5uZWxIdWJWaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL0NvcmUvT25lRGFzTW9kdWxlVmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9Db3JlL09uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9Db3JlL1RyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL1BsdWdpbi9CdWZmZXJSZXF1ZXN0Vmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vRGF0YVBvcnRWaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL1BsdWdpbi9QbHVnaW5WaWV3TW9kZWxCYXNlLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vRXh0ZW5kZWREYXRhR2F0ZXdheVZpZXdNb2RlbEJhc2UudHMiLCIuLi9WaWV3TW9kZWxzL1BsdWdpbi9EYXRhV3JpdGVyVmlld01vZGVsQmFzZS50cyIsIi4uL1ZpZXdNb2RlbHMvUGx1Z2luL1BsdWdpbkRlc2NyaXB0aW9uVmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vUGx1Z2luSWRlbnRpZmljYXRpb25WaWV3TW9kZWwudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsSUFBSyxpQkFJSjtBQUpELFdBQUssaUJBQWlCO0lBRWxCLDJEQUFTLENBQUE7SUFDVCw2REFBVSxDQUFBO0FBQ2QsQ0FBQyxFQUpJLGlCQUFpQixLQUFqQixpQkFBaUIsUUFJckI7QUNKRCxJQUFLLGNBSUo7QUFKRCxXQUFLLGNBQWM7SUFFZixtRUFBZ0IsQ0FBQTtJQUNoQiw2REFBYSxDQUFBO0FBQ2pCLENBQUMsRUFKSSxjQUFjLEtBQWQsY0FBYyxRQUlsQjtBQ0pELElBQUssbUJBTUo7QUFORCxXQUFLLG1CQUFtQjtJQUVwQixzRUFBYSxDQUFBO0lBQ2IseUVBQWUsQ0FBQTtJQUNmLGdFQUFXLENBQUE7SUFDWCwrREFBVyxDQUFBO0FBQ2YsQ0FBQyxFQU5JLG1CQUFtQixLQUFuQixtQkFBbUIsUUFNdkI7QUNORCxJQUFLLGtCQVdKO0FBWEQsV0FBSyxrQkFBa0I7SUFFbkIsaUVBQWUsQ0FBQTtJQUNmLCtEQUFhLENBQUE7SUFDYiw2REFBWSxDQUFB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QUFXLElBQUksQ0FBQyxTQUFTLEVBQUU7U0FDdkMsQ0FBQTtRQUVELE1BQU0sQ0FBQyxLQUFLLENBQUE7SUFDaEIsQ0FBQztDQUNKO0FDN0JEO0lBU0ksWUFBWSx5QkFBOEI7UUFFdEMsSUFBSSxDQUFDLGNBQWMsR0FBRyx5QkFBeUIsQ0FBQyxjQUFjLENBQUE7UUFDOUQsSUFBSSxDQUFDLEVBQUUsR0FBRyx5QkFBeUIsQ0FBQyxFQUFFLENBQUE7UUFDdEMsSUFBSSxDQUFDLElBQUksR0FBRyx5QkFBeUIsQ0FBQyxJQUFJLENBQUE7UUFDMUMsSUFBSSxDQUFDLFdBQVcsR0FBRyx5QkFBeUIsQ0FBQyxXQUFXLENBQUE7UUFDeEQsSUFBSSxDQUFDLGdCQUFnQixHQUFHLHlCQUF5QixDQUFDLGdCQUFnQixDQUFBO1FBQ2xFLElBQUksQ0FBQyxxQkFBcUIsR0FBRyx5QkFBeUIsQ0FBQyxxQkFBcUIsQ0FBQTtJQUNoRixDQUFDO0NBQ0oiLCJzb3VyY2VzQ29udGVudCI6WyJlbnVtIERhdGFEaXJlY3Rpb25FbnVtXHJcbntcclxuICAgIElucHV0ID0gMSxcclxuICAgIE91dHB1dCA9IDJcclxufSIsImVudW0gRW5kaWFubmVzc0VudW1cclxue1xyXG4gICAgTGl0dGxlRW5kaWFuID0gMSxcclxuICAgIEJpZ0VuZGlhbiA9IDJcclxufSIsImVudW0gRmlsZUdyYW51bGFyaXR5RW51bVxyXG57XHJcbiAgICBNaW51dGVfMSA9IDYwLFxyXG4gICAgTWludXRlXzEwID0gNjAwLFxyXG4gICAgSG91ciA9IDM2MDAsXHJcbiAgICBEYXkgPSA4NjQwMFxyXG59IiwiZW51bSBPbmVEYXNEYXRhVHlwZUVudW1cclxue1xyXG4gICAgQk9PTEVBTiA9IDB4MDA4LFxyXG4gICAgVUlOVDggPSAweDEwOCxcclxuICAgIElOVDggPSAweDIwOCxcclxuICAgIFVJTlQxNiA9IDB4MTEwLFxyXG4gICAgSU5UMTYgPSAweDIxMCxcclxuICAgIFVJTlQzMiA9IDB4MTIwLFxyXG4gICAgSU5UMzIgPSAweDIyMCxcclxuICAgIEZMT0FUMzIgPSAweDMyMCxcclxuICAgIEZMT0FUNjQgPSAweDM0MFxyXG59IiwiZW51bSBPbmVEYXNTdGF0ZUVudW1cclxue1xyXG4gICAgRXJyb3IgPSAxLFxyXG4gICAgSW5pdGlhbGl6YXRpb24gPSAyLFxyXG4gICAgVW5jb25maWd1cmVkID0gMyxcclxuICAgIEFwcGx5Q29uZmlndXJhdGlvbiA9IDUsXHJcbiAgICBSZWFkeSA9IDYsXHJcbiAgICBSdW4gPSA3XHJcbn0iLCJlbnVtIFNhbXBsZVJhdGVFbnVtXHJcbntcclxuICAgIFNhbXBsZVJhdGVfMTAwID0gMSxcclxuICAgIFNhbXBsZVJhdGVfMjUgPSA0LFxyXG4gICAgU2FtcGxlUmF0ZV81ID0gMjAsXHJcbiAgICBTYW1wbGVSYXRlXzEgPSAxMDBcclxufSIsImNsYXNzIEFjdGlvblJlcXVlc3Rcclxue1xyXG4gICAgcHVibGljIHJlYWRvbmx5IFBsdWdpbklkOiBzdHJpbmdcclxuICAgIHB1YmxpYyByZWFkb25seSBJbnN0YW5jZUlkOiBudW1iZXJcclxuICAgIHB1YmxpYyByZWFkb25seSBNZXRob2ROYW1lOiBzdHJpbmdcclxuICAgIHB1YmxpYyByZWFkb25seSBEYXRhOiBhbnlcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihwbHVnaW5JZDogc3RyaW5nLCBpbnN0YW5jZUlkOiBudW1iZXIsIG1ldGhvZE5hbWU6IHN0cmluZywgZGF0YTogYW55KVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuUGx1Z2luSWQgPSBwbHVnaW5JZDtcclxuICAgICAgICB0aGlzLkluc3RhbmNlSWQgPSBpbnN0YW5jZUlkO1xyXG4gICAgICAgIHRoaXMuTWV0aG9kTmFtZSA9IG1ldGhvZE5hbWU7XHJcbiAgICAgICAgdGhpcy5EYXRhID0gZGF0YTtcclxuICAgIH1cclxufSIsImNsYXNzIEFjdGlvblJlc3BvbnNlXHJcbntcclxuICAgIHB1YmxpYyBEYXRhOiBhbnlcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihkYXRhOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5EYXRhID0gZGF0YTtcclxuICAgIH1cclxufSIsImNsYXNzIEV2ZW50RGlzcGF0Y2hlcjxUU2VuZGVyLCBUQXJncz4gaW1wbGVtZW50cyBJRXZlbnQ8VFNlbmRlciwgVEFyZ3M+XHJcbntcclxuICAgIHByaXZhdGUgX3N1YnNjcmlwdGlvbnM6IEFycmF5PChzZW5kZXI6IFRTZW5kZXIsIGFyZ3M6IFRBcmdzKSA9PiB2b2lkPiA9IG5ldyBBcnJheTwoc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncykgPT4gdm9pZD4oKTtcclxuXHJcbiAgICBzdWJzY3JpYmUoZm46IChzZW5kZXI6IFRTZW5kZXIsIGFyZ3M6IFRBcmdzKSA9PiB2b2lkKTogdm9pZFxyXG4gICAge1xyXG4gICAgICAgIGlmIChmbilcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuX3N1YnNjcmlwdGlvbnMucHVzaChmbik7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHVuc3Vic2NyaWJlKGZuOiAoc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncykgPT4gdm9pZCk6IHZvaWRcclxuICAgIHtcclxuICAgICAgICBsZXQgaSA9IHRoaXMuX3N1YnNjcmlwdGlvbnMuaW5kZXhPZihmbik7XHJcblxyXG4gICAgICAgIGlmIChpID4gLTEpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLl9zdWJzY3JpcHRpb25zLnNwbGljZShpLCAxKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgZGlzcGF0Y2goc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncyk6IHZvaWRcclxuICAgIHtcclxuICAgICAgICBmb3IgKGxldCBoYW5kbGVyIG9mIHRoaXMuX3N1YnNjcmlwdGlvbnMpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBoYW5kbGVyKHNlbmRlciwgYXJncyk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG59IiwiaW50ZXJmYWNlIElFdmVudDxUU2VuZGVyLCBUQXJncz5cclxue1xyXG4gICAgc3Vic2NyaWJlKGZuOiAoc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncykgPT4gdm9pZCk6IHZvaWQ7XHJcbiAgICB1bnN1YnNjcmliZShmbjogKHNlbmRlcjogVFNlbmRlciwgYXJnczogVEFyZ3MpID0+IHZvaWQpOiB2b2lkO1xyXG59IiwiZW51bSBPbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtXHJcbntcclxuICAgIER1cGxleCA9IDEsXHJcbiAgICBJbnB1dE9ubHkgPSAyLFxyXG4gICAgT3V0cHV0T25seSA9IDMsXHJcbn0iLCJjbGFzcyBCdWZmZXJSZXF1ZXN0TW9kZWxcclxue1xyXG4gICAgcHVibGljIFNhbXBsZVJhdGU6IFNhbXBsZVJhdGVFbnVtXHJcbiAgICBwdWJsaWMgR3JvdXBGaWx0ZXI6IHN0cmluZ1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKHNhbXBsZVJhdGU6IFNhbXBsZVJhdGVFbnVtLCBncm91cEZpbHRlcjogc3RyaW5nKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuU2FtcGxlUmF0ZSA9IHNhbXBsZVJhdGU7XHJcbiAgICAgICAgdGhpcy5Hcm91cEZpbHRlciA9IGdyb3VwRmlsdGVyO1xyXG4gICAgfVxyXG59IiwiY2xhc3MgQ2hhbm5lbEh1Yk1vZGVsXHJcbntcclxuICAgIHB1YmxpYyBOYW1lOiBzdHJpbmdcclxuICAgIHB1YmxpYyBHcm91cDogc3RyaW5nXHJcbiAgICBwdWJsaWMgRGF0YVR5cGU6IE9uZURhc0RhdGFUeXBlRW51bVxyXG4gICAgcHVibGljIEd1aWQ6IHN0cmluZ1xyXG4gICAgcHVibGljIENyZWF0aW9uRGF0ZVRpbWU6IHN0cmluZ1xyXG4gICAgcHVibGljIFVuaXQ6IHN0cmluZ1xyXG4gICAgcHVibGljIFRyYW5zZmVyRnVuY3Rpb25TZXQ6IGFueVtdXHJcbiAgICBwdWJsaWMgQXNzb2NpYXRlZERhdGFJbnB1dElkOiBzdHJpbmdcclxuICAgIHB1YmxpYyBBc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0OiBzdHJpbmdbXVxyXG5cclxuICAgIGNvbnN0cnVjdG9yKG5hbWU6IHN0cmluZywgZ3JvdXA6IHN0cmluZywgZGF0YVR5cGU6IE9uZURhc0RhdGFUeXBlRW51bSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLk5hbWUgPSBuYW1lO1xyXG4gICAgICAgIHRoaXMuR3JvdXAgPSBncm91cDtcclxuICAgICAgICB0aGlzLkRhdGFUeXBlID0gZGF0YVR5cGU7XHJcbiAgICAgICAgdGhpcy5HdWlkID0gR3VpZC5OZXdHdWlkKClcclxuICAgICAgICB0aGlzLkNyZWF0aW9uRGF0ZVRpbWUgPSBuZXcgRGF0ZSgpLnRvSVNPU3RyaW5nKClcclxuICAgICAgICB0aGlzLlVuaXQgPSBcIlwiXHJcbiAgICAgICAgdGhpcy5UcmFuc2ZlckZ1bmN0aW9uU2V0ID0gW11cclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXRJZCA9IFwiXCJcclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQgPSBbXVxyXG4gICAgfVxyXG59IiwiY2xhc3MgT25lRGFzTW9kdWxlTW9kZWxcclxue1xyXG4gICAgcHVibGljIERhdGFUeXBlOiBPbmVEYXNEYXRhVHlwZUVudW1cclxuICAgIHB1YmxpYyBEYXRhRGlyZWN0aW9uOiBEYXRhRGlyZWN0aW9uRW51bVxyXG4gICAgcHVibGljIEVuZGlhbm5lc3M6IEVuZGlhbm5lc3NFbnVtXHJcbiAgICBwdWJsaWMgU2l6ZTogbnVtYmVyXHJcblxyXG4gICAgY29uc3RydWN0b3IoZGF0YVR5cGU6IE9uZURhc0RhdGFUeXBlRW51bSwgZGF0YURpcmVjdGlvbjogRGF0YURpcmVjdGlvbkVudW0sIGVuZGlhbm5lc3M6IEVuZGlhbm5lc3NFbnVtLCBzaXplOiBudW1iZXIpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZSA9IGRhdGFUeXBlXHJcbiAgICAgICAgdGhpcy5EYXRhRGlyZWN0aW9uID0gZGF0YURpcmVjdGlvblxyXG4gICAgICAgIHRoaXMuRW5kaWFubmVzcyA9IGVuZGlhbm5lc3NcclxuICAgICAgICB0aGlzLlNpemUgPSBzaXplXHJcbiAgICB9XHJcbn1cclxuIiwiY2xhc3MgVHJhbnNmZXJGdW5jdGlvbk1vZGVsXHJcbntcclxuICAgIHB1YmxpYyBEYXRlVGltZTogc3RyaW5nXHJcbiAgICBwdWJsaWMgVHlwZTogc3RyaW5nXHJcbiAgICBwdWJsaWMgT3B0aW9uOiBzdHJpbmdcclxuICAgIHB1YmxpYyBBcmd1bWVudDogc3RyaW5nXHJcblxyXG4gICAgY29uc3RydWN0b3IoZGF0ZVRpbWU6IHN0cmluZywgdHlwZTogc3RyaW5nLCBvcHRpb246IHN0cmluZywgYXJndW1lbnQ6IHN0cmluZylcclxuICAgIHtcclxuICAgICAgICB0aGlzLkRhdGVUaW1lID0gZGF0ZVRpbWVcclxuICAgICAgICB0aGlzLlR5cGUgPSB0eXBlXHJcbiAgICAgICAgdGhpcy5PcHRpb24gPSBvcHRpb25cclxuICAgICAgICB0aGlzLkFyZ3VtZW50ID0gYXJndW1lbnRcclxuICAgIH1cclxufSIsImRlY2xhcmUgdmFyIHNpZ25hbFI6IGFueVxyXG5cclxuY2xhc3MgQ29ubmVjdGlvbk1hbmFnZXJcclxue1xyXG4gICAgcHVibGljIHN0YXRpYyBXZWJDbGllbnRIdWI6IGFueSAvLyBpbXByb3ZlOiB1c2UgdHlwaW5nc1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgSW5pdGlhbGl6ZShlbmFibGVMb2dnaW5nOiBib29sZWFuKVxyXG4gICAge1xyXG4gICAgICAgIENvbm5lY3Rpb25NYW5hZ2VyLldlYkNsaWVudEh1YiA9IG5ldyBzaWduYWxSLkh1YkNvbm5lY3Rpb24oJy93ZWJjbGllbnRodWInKTtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIEludm9rZVdlYkNsaWVudEh1YiA9IGFzeW5jKG1ldGhvZE5hbWU6IHN0cmluZywgLi4uYXJnczogYW55W10pID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIGF3YWl0IFByb21pc2UucmVzb2x2ZShDb25uZWN0aW9uTWFuYWdlci5XZWJDbGllbnRIdWIuaW52b2tlKG1ldGhvZE5hbWUsIC4uLmFyZ3MpKVxyXG4gICAgfVxyXG59XHJcbiIsImNsYXNzIEVudW1lcmF0aW9uSGVscGVyXHJcbntcclxuICAgIHB1YmxpYyBzdGF0aWMgRGVzY3JpcHRpb246IHsgW2luZGV4OiBzdHJpbmddOiBzdHJpbmcgfSA9IHt9XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBHZXRFbnVtTG9jYWxpemF0aW9uID0gKHR5cGVOYW1lOiBzdHJpbmcsIHZhbHVlKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHZhciBrZXk6IHN0cmluZyA9IGV2YWwodHlwZU5hbWUgKyBcIltcIiArIHZhbHVlICsgXCJdXCIpXHJcbiAgICAgICAgcmV0dXJuIGV2YWwoXCJFbnVtZXJhdGlvbkhlbHBlci5EZXNjcmlwdGlvblsnXCIgKyB0eXBlTmFtZSArIFwiX1wiICsga2V5ICsgXCInXVwiKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgR2V0RW51bVZhbHVlcyA9ICh0eXBlTmFtZTogc3RyaW5nKSA9PlxyXG4gICAge1xyXG4gICAgICAgIGxldCB2YWx1ZXM6IGFueVtdXHJcblxyXG4gICAgICAgIHZhbHVlcyA9IGV2YWwoXCJPYmplY3Qua2V5cyhcIiArIHR5cGVOYW1lICsgXCIpLm1hcChrZXkgPT4gXCIgKyB0eXBlTmFtZSArIFwiW2tleV0pXCIpXHJcbiAgICAgICAgcmV0dXJuIDxudW1iZXJbXT52YWx1ZXMuZmlsdGVyKHZhbHVlID0+IHR5cGVvZiAodmFsdWUpID09PSBcIm51bWJlclwiKVxyXG4gICAgfVxyXG59IiwibGV0IEVycm9yTWVzc2FnZTogeyBbaW5kZXg6IHN0cmluZ106IHN0cmluZyB9ID0ge31cclxuRXJyb3JNZXNzYWdlW1wiTXVsdGlNYXBwaW5nRWRpdG9yVmlld01vZGVsX0ludmFsaWRTZXR0aW5nc1wiXSA9IFwiT25lIG9yIG1vcmUgc2V0dGluZ3MgYXJlIGludmFsaWQuXCJcclxuRXJyb3JNZXNzYWdlW1wiTXVsdGlNYXBwaW5nRWRpdG9yVmlld01vZGVsX1dyb25nRGF0YVR5cGVcIl0gPSBcIk9uZSBvciBtb3JlIHZhcmlhYmxlLWNoYW5uZWwgZGF0YSB0eXBlIGNvbWJpbmF0aW9ucyBhcmUgaW52YWxpZC5cIlxyXG5FcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0NoYW5uZWxBbHJlYWR5RXhpc3RzXCJdID0gXCJBIGNoYW5uZWwgd2l0aCB0aGF0IG5hbWUgYWxyZWFkeSBleGlzdHMuXCJcclxuRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9Jc0FscmVhZHlJbkdyb3VwXCJdID0gXCJUaGUgY2hhbm5lbCBpcyBhbHJlYWR5IGEgbWVtYmVyIG9mIHRoaXMgZ3JvdXAuXCJcclxuRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9JbnZhbGlkQ2hhcmFjdGVyc1wiXSA9IFwiVXNlIEEtWiwgYS16LCAwLTkgb3IgXy5cIlxyXG5FcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0ludmFsaWRMZWFkaW5nQ2hhcmFjdGVyXCJdID0gXCJVc2UgQS1aIG9yIGEteiBhcyBmaXJzdCBjaGFyYWN0ZXIuXCJcclxuRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9OYW1lRW1wdHlcIl0gPSBcIlRoZSBuYW1lIG11c3Qgbm90IGJlIGVtcHR5LlwiXHJcbiIsImNsYXNzIE9ic2VydmFibGVHcm91cDxUPlxyXG57XHJcbiAgICBLZXk6IHN0cmluZztcclxuICAgIE1lbWJlcnM6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PFQ+XHJcblxyXG4gICAgY29uc3RydWN0b3Ioa2V5OiBzdHJpbmcsIG1lbWJlcnM6IFRbXSA9IG5ldyBBcnJheTxUPigpKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuS2V5ID0ga2V5XHJcbiAgICAgICAgdGhpcy5NZW1iZXJzID0ga28ub2JzZXJ2YWJsZUFycmF5KG1lbWJlcnMpXHJcbiAgICB9XHJcbn1cclxuXHJcbmZ1bmN0aW9uIE9ic2VydmFibGVHcm91cEJ5PFQ+KGxpc3Q6IFRbXSwgbmFtZUdldHRlcjogKHg6IFQpID0+IHN0cmluZywgZ3JvdXBOYW1lR2V0dGVyOiAoeDogVCkgPT4gc3RyaW5nLCBmaWx0ZXI6IHN0cmluZyk6IE9ic2VydmFibGVHcm91cDxUPltdXHJcbntcclxuICAgIGxldCByZXN1bHQ6IE9ic2VydmFibGVHcm91cDxUPltdXHJcbiAgICBsZXQgcmVnRXhwOiBSZWdFeHBcclxuXHJcbiAgICByZXN1bHQgPSBbXVxyXG4gICAgcmVnRXhwID0gbmV3IFJlZ0V4cChmaWx0ZXIsIFwiaVwiKVxyXG5cclxuICAgIGxpc3QuZm9yRWFjaChlbGVtZW50ID0+XHJcbiAgICB7XHJcbiAgICAgICAgaWYgKHJlZ0V4cC50ZXN0KG5hbWVHZXR0ZXIoZWxlbWVudCkpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgZ3JvdXBOYW1lR2V0dGVyKGVsZW1lbnQpLnNwbGl0KFwiXFxuXCIpLmZvckVhY2goZ3JvdXBOYW1lID0+XHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIEFkZFRvR3JvdXBlZEFycmF5KGVsZW1lbnQsIGdyb3VwTmFtZSwgcmVzdWx0KVxyXG4gICAgICAgICAgICB9KVxyXG4gICAgICAgIH1cclxuICAgIH0pXHJcblxyXG4gICAgcmV0dXJuIHJlc3VsdFxyXG59XHJcblxyXG5mdW5jdGlvbiBBZGRUb0dyb3VwZWRBcnJheTxUPihpdGVtOiBULCBncm91cE5hbWU6IHN0cmluZywgb2JzZXJ2YWJsZUdyb3VwU2V0OiBPYnNlcnZhYmxlR3JvdXA8VD5bXSlcclxue1xyXG4gICAgbGV0IGdyb3VwOiBPYnNlcnZhYmxlR3JvdXA8VD5cclxuXHJcbiAgICBncm91cCA9IG9ic2VydmFibGVHcm91cFNldC5maW5kKHkgPT4geS5LZXkgPT09IGdyb3VwTmFtZSlcclxuXHJcbiAgICBpZiAoIWdyb3VwKVxyXG4gICAge1xyXG4gICAgICAgIGdyb3VwID0gbmV3IE9ic2VydmFibGVHcm91cDxUPihncm91cE5hbWUpXHJcbiAgICAgICAgb2JzZXJ2YWJsZUdyb3VwU2V0LnB1c2goZ3JvdXApXHJcbiAgICB9XHJcblxyXG4gICAgZ3JvdXAuTWVtYmVycy5wdXNoKGl0ZW0pXHJcbn1cclxuXHJcbi8vZnVuY3Rpb24gQWRkVG9Hcm91cGVkT2JzZXJ2YWJsZUFycmF5PFQ+KGl0ZW06IFQsIGdyb3VwTmFtZTogc3RyaW5nLCBvYnNlcnZhYmxlR3JvdXBTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PE9ic2VydmFibGVHcm91cDxUPj4pXHJcbi8ve1xyXG4vLyAgICBsZXQgZ3JvdXA6IE9ic2VydmFibGVHcm91cDxUPlxyXG5cclxuLy8gICAgZ3JvdXAgPSBvYnNlcnZhYmxlR3JvdXBTZXQoKS5maW5kKHkgPT4geS5LZXkgPT09IGdyb3VwTmFtZSlcclxuXHJcbi8vICAgIGlmICghZ3JvdXApXHJcbi8vICAgIHtcclxuLy8gICAgICAgIGdyb3VwID0gbmV3IE9ic2VydmFibGVHcm91cDxUPihncm91cE5hbWUpXHJcbi8vICAgICAgICBvYnNlcnZhYmxlR3JvdXBTZXQucHVzaChncm91cClcclxuLy8gICAgfVxyXG5cclxuLy8gICAgZ3JvdXAuTWVtYmVycy5wdXNoKGl0ZW0pXHJcbi8vfVxyXG5cclxuLy9mdW5jdGlvbiBSZW1vdmVGcm9tR3JvdXBlZE9ic2VydmFibGVBcnJheTxUPihpdGVtOiBULCBvYnNlcnZhYmxlR3JvdXBTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PE9ic2VydmFibGVHcm91cDxUPj4pXHJcbi8ve1xyXG4vLyAgICB2YXIgZ3JvdXA6IE9ic2VydmFibGVHcm91cDxUPlxyXG5cclxuLy8gICAgb2JzZXJ2YWJsZUdyb3VwU2V0KCkuc29tZSh4ID0+XHJcbi8vICAgIHtcclxuLy8gICAgICAgIGlmICh4Lk1lbWJlcnMoKS5pbmRleE9mKGl0ZW0pID4gLTEpXHJcbi8vICAgICAgICB7XHJcbi8vICAgICAgICAgICAgZ3JvdXAgPSB4XHJcblxyXG4vLyAgICAgICAgICAgIHJldHVybiB0cnVlXHJcbi8vICAgICAgICB9XHJcblxyXG4vLyAgICAgICAgcmV0dXJuIGZhbHNlXHJcbi8vICAgIH0pXHJcblxyXG4vLyAgICBpZiAoZ3JvdXApXHJcbi8vICAgIHtcclxuLy8gICAgICAgIGdyb3VwLk1lbWJlcnMucmVtb3ZlKGl0ZW0pXHJcblxyXG4vLyAgICAgICAgaWYgKGdyb3VwLk1lbWJlcnMoKS5sZW5ndGggPT09IDApXHJcbi8vICAgICAgICB7XHJcbi8vICAgICAgICAgICAgb2JzZXJ2YWJsZUdyb3VwU2V0LnJlbW92ZShncm91cClcclxuLy8gICAgICAgIH1cclxuXHJcbi8vICAgICAgICByZXR1cm4gdHJ1ZVxyXG4vLyAgICB9XHJcblxyXG4vLyAgICByZXR1cm4gZmFsc2VcclxuLy99XHJcblxyXG4vL2Z1bmN0aW9uIFVwZGF0ZUdyb3VwZWRPYnNlcnZhYmxlQXJyYXk8VD4oaXRlbTogVCwgZ3JvdXBOYW1lOiBzdHJpbmcsIG9ic2VydmFibGVHcm91cFNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8T2JzZXJ2YWJsZUdyb3VwPFQ+PilcclxuLy97XHJcbi8vICAgIFJlbW92ZUZyb21Hcm91cGVkT2JzZXJ2YWJsZUFycmF5KGl0ZW0sIG9ic2VydmFibGVHcm91cFNldClcclxuLy8gICAgQWRkVG9Hcm91cGVkT2JzZXJ2YWJsZUFycmF5KGl0ZW0sIGdyb3VwTmFtZSwgb2JzZXJ2YWJsZUdyb3VwU2V0KVxyXG4vL31cclxuXHJcbmZ1bmN0aW9uIE1hcE1hbnk8VEFycmF5RWxlbWVudCwgVFNlbGVjdD4oYXJyYXk6IFRBcnJheUVsZW1lbnRbXSwgbWFwRnVuYzogKGl0ZW06IFRBcnJheUVsZW1lbnQpID0+IFRTZWxlY3RbXSk6IFRTZWxlY3RbXVxyXG57XHJcbiAgICByZXR1cm4gYXJyYXkucmVkdWNlKChwcmV2aW91cywgY3VycmVudCwgaSkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gcHJldmlvdXMuY29uY2F0KG1hcEZ1bmMoY3VycmVudCkpO1xyXG4gICAgfSwgPFRTZWxlY3RbXT5bXSk7XHJcbn1cclxuXHJcbmNsYXNzIEd1aWRcclxue1xyXG4gICAgc3RhdGljIE5ld0d1aWQoKVxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiAneHh4eHh4eHgteHh4eC00eHh4LXl4eHgteHh4eHh4eHh4eHh4Jy5yZXBsYWNlKC9beHldL2csIGZ1bmN0aW9uIChjKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdmFyIHIgPSBNYXRoLnJhbmRvbSgpICogMTYgfCAwXHJcbiAgICAgICAgICAgIHZhciB2ID0gYyA9PT0gJ3gnID8gciA6IChyICYgMHgzIHwgMHg4KVxyXG5cclxuICAgICAgICAgICAgcmV0dXJuIHYudG9TdHJpbmcoMTYpXHJcbiAgICAgICAgfSlcclxuICAgIH1cclxufVxyXG5cclxubGV0IENoZWNrTmFtaW5nQ29udmVudGlvbiA9ICh2YWx1ZTogc3RyaW5nKSA9PlxyXG57XHJcbiAgICB2YXIgcmVnRXhwOiBhbnlcclxuXHJcbiAgICBpZiAodmFsdWUubGVuZ3RoID09PSAwKVxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB7IEhhc0Vycm9yOiB0cnVlLCBFcnJvckRlc2NyaXB0aW9uOiBFcnJvck1lc3NhZ2VbXCJQcm9qZWN0X05hbWVFbXB0eVwiXSB9XHJcbiAgICB9XHJcblxyXG4gICAgcmVnRXhwID0gbmV3IFJlZ0V4cChcIlteQS1aYS16MC05X11cIilcclxuXHJcbiAgICBpZiAocmVnRXhwLnRlc3QodmFsdWUpKVxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB7IEhhc0Vycm9yOiB0cnVlLCBFcnJvckRlc2NyaXB0aW9uOiBFcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0ludmFsaWRDaGFyYWN0ZXJzXCJdIH1cclxuICAgIH1cclxuXHJcbiAgICByZWdFeHAgPSBuZXcgUmVnRXhwKFwiXlswLTlfXVwiKVxyXG5cclxuICAgIGlmIChyZWdFeHAudGVzdCh2YWx1ZSkpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHsgSGFzRXJyb3I6IHRydWUsIEVycm9yRGVzY3JpcHRpb246IEVycm9yTWVzc2FnZVtcIlByb2plY3RfSW52YWxpZExlYWRpbmdDaGFyYWN0ZXJcIl0gfVxyXG4gICAgfVxyXG5cclxuICAgIHJldHVybiB7XHJcbiAgICAgICAgSGFzRXJyb3I6IGZhbHNlLFxyXG4gICAgICAgIEVycm9yRGVzY3JpcHRpb246IFwiXCJcclxuICAgIH1cclxufSIsImNsYXNzIFBsdWdpbkZhY3Rvcnlcclxue1xyXG4gICAgcHVibGljIHN0YXRpYyBDcmVhdGVQbHVnaW5WaWV3TW9kZWxBc3luYyA9IGFzeW5jIChwbHVnaW5UeXBlOiBzdHJpbmcsIHBsdWdpbk1vZGVsOiBhbnkpID0+XHJcbiAgICB7XHJcbiAgICAgICAgbGV0IHBsdWdpbklkZW50aWZpY2F0aW9uOiBQbHVnaW5JZGVudGlmaWNhdGlvblZpZXdNb2RlbFxyXG4gICAgICAgIGxldCBwbHVnaW5WaWV3TW9kZWw6IFBsdWdpblZpZXdNb2RlbEJhc2VcclxuICAgICAgICBsZXQgcGx1Z2luVmlld01vZGVsUmF3OiBzdHJpbmdcclxuXHJcbiAgICAgICAgcGx1Z2luSWRlbnRpZmljYXRpb24gPSBQbHVnaW5IaXZlLkZpbmRQbHVnaW5JZGVudGlmaWNhdGlvbihwbHVnaW5UeXBlLCBwbHVnaW5Nb2RlbC5EZXNjcmlwdGlvbi5JZClcclxuXHJcbiAgICAgICAgaWYgKHBsdWdpbklkZW50aWZpY2F0aW9uKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgcGx1Z2luVmlld01vZGVsUmF3ID0gYXdhaXQgQ29ubmVjdGlvbk1hbmFnZXIuSW52b2tlV2ViQ2xpZW50SHViKFwiR2V0UGx1Z2luU3RyaW5nUmVzb3VyY2VcIiwgcGx1Z2luTW9kZWwuRGVzY3JpcHRpb24uSWQsIHBsdWdpbklkZW50aWZpY2F0aW9uLlZpZXdNb2RlbFJlc291cmNlTmFtZSlcclxuICAgICAgICAgICAgcGx1Z2luVmlld01vZGVsID0gPFBsdWdpblZpZXdNb2RlbEJhc2U+bmV3IEZ1bmN0aW9uKHBsdWdpblZpZXdNb2RlbFJhdyArIFwiOyByZXR1cm4gVmlld01vZGVsQ29uc3RydWN0b3JcIikoKShwbHVnaW5Nb2RlbCwgcGx1Z2luSWRlbnRpZmljYXRpb24pXHJcblxyXG4gICAgICAgICAgICByZXR1cm4gcGx1Z2luVmlld01vZGVsXHJcbiAgICAgICAgfVxyXG4gICAgICAgIGVsc2VcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRocm93IG5ldyBFcnJvcihcIk5vIGNvcnJlc3BvbmRpbmcgcGx1Z2luIGRlc2NyaXB0aW9uIGZvdW5kLlwiKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxufSIsImNsYXNzIFBsdWdpbkhpdmVcclxue1xyXG4gICAgLy8gZmllbGRzXHJcbiAgICBwdWJsaWMgc3RhdGljIFBsdWdpbklkZW50aWZpY2F0aW9uU2V0OiBNYXA8c3RyaW5nLCBQbHVnaW5JZGVudGlmaWNhdGlvblZpZXdNb2RlbFtdPlxyXG5cclxuICAgIC8vIGNvbnN0cnVjdG9yc1xyXG4gICAgc3RhdGljIEluaXRpYWxpemUgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIFBsdWdpbkhpdmUuUGx1Z2luSWRlbnRpZmljYXRpb25TZXQgPSBuZXcgTWFwPHN0cmluZywgUGx1Z2luSWRlbnRpZmljYXRpb25WaWV3TW9kZWxbXT4oKVxyXG4gICAgfSAgIFxyXG5cclxuICAgIHN0YXRpYyBGaW5kUGx1Z2luSWRlbnRpZmljYXRpb24gPSAocGx1Z2luVHlwZU5hbWU6IHN0cmluZywgcGx1Z2luSWQ6IHN0cmluZykgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gUGx1Z2luSGl2ZS5QbHVnaW5JZGVudGlmaWNhdGlvblNldC5nZXQocGx1Z2luVHlwZU5hbWUpLmZpbmQocGx1Z2luSWRlbnRpZmljYXRpb24gPT4gcGx1Z2luSWRlbnRpZmljYXRpb24uSWQgPT09IHBsdWdpbklkKTtcclxuICAgIH1cclxufSIsImNsYXNzIENoYW5uZWxIdWJWaWV3TW9kZWxcclxue1xyXG4gICAgcHVibGljIE5hbWU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgR3JvdXA6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRGF0YVR5cGU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxPbmVEYXNEYXRhVHlwZUVudW0+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgR3VpZDogc3RyaW5nXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgQ3JlYXRpb25EYXRlVGltZTogc3RyaW5nXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgVW5pdDogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyByZWFkb25seSBUcmFuc2ZlckZ1bmN0aW9uU2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsPlxyXG4gICAgcHVibGljIFNlbGVjdGVkVHJhbnNmZXJGdW5jdGlvbjogS25vY2tvdXRPYnNlcnZhYmxlPFRyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWw+XHJcbiAgICBwdWJsaWMgRXZhbHVhdGVkVHJhbnNmZXJGdW5jdGlvblNldDogKCh2YWx1ZTogbnVtYmVyKSA9PiBudW1iZXIpW11cclxuICAgIHB1YmxpYyBJc1NlbGVjdGVkOiBLbm9ja291dE9ic2VydmFibGU8Ym9vbGVhbj5cclxuXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgQXNzb2NpYXRlZERhdGFJbnB1dDogS25vY2tvdXRPYnNlcnZhYmxlPERhdGFQb3J0Vmlld01vZGVsPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IEFzc29jaWF0ZWREYXRhT3V0cHV0U2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxEYXRhUG9ydFZpZXdNb2RlbD5cclxuXHJcbiAgICBwcml2YXRlIEFzc29jaWF0ZWREYXRhSW5wdXRJZDogc3RyaW5nXHJcbiAgICBwcml2YXRlIEFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQ6IHN0cmluZ1tdXHJcblxyXG4gICAgY29uc3RydWN0b3IoY2hhbm5lbEh1Yk1vZGVsOiBDaGFubmVsSHViTW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5OYW1lID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KGNoYW5uZWxIdWJNb2RlbC5OYW1lKVxyXG4gICAgICAgIHRoaXMuR3JvdXAgPSBrby5vYnNlcnZhYmxlPHN0cmluZz4oY2hhbm5lbEh1Yk1vZGVsLkdyb3VwKVxyXG4gICAgICAgIHRoaXMuRGF0YVR5cGUgPSBrby5vYnNlcnZhYmxlPE9uZURhc0RhdGFUeXBlRW51bT4oY2hhbm5lbEh1Yk1vZGVsLkRhdGFUeXBlKVxyXG4gICAgICAgIHRoaXMuR3VpZCA9IGNoYW5uZWxIdWJNb2RlbC5HdWlkXHJcbiAgICAgICAgdGhpcy5DcmVhdGlvbkRhdGVUaW1lID0gY2hhbm5lbEh1Yk1vZGVsLkNyZWF0aW9uRGF0ZVRpbWVcclxuICAgICAgICB0aGlzLlVuaXQgPSBrby5vYnNlcnZhYmxlPHN0cmluZz4oY2hhbm5lbEh1Yk1vZGVsLlVuaXQpXHJcbiAgICAgICAgdGhpcy5UcmFuc2ZlckZ1bmN0aW9uU2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PFRyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWw+KGNoYW5uZWxIdWJNb2RlbC5UcmFuc2ZlckZ1bmN0aW9uU2V0Lm1hcCh0ZiA9PiBuZXcgVHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbCh0ZikpKVxyXG4gICAgICAgIHRoaXMuU2VsZWN0ZWRUcmFuc2ZlckZ1bmN0aW9uID0ga28ub2JzZXJ2YWJsZTxUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsPih0aGlzLkNyZWF0ZURlZmF1bHRUcmFuc2ZlckZ1bmN0aW9uKCkpXHJcbiAgICAgICAgdGhpcy5Jc1NlbGVjdGVkID0ga28ub2JzZXJ2YWJsZTxib29sZWFuPihmYWxzZSlcclxuXHJcbiAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0ID0ga28ub2JzZXJ2YWJsZTxEYXRhUG9ydFZpZXdNb2RlbD4oKVxyXG4gICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRTZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8RGF0YVBvcnRWaWV3TW9kZWw+KClcclxuXHJcbiAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0SWQgPSBjaGFubmVsSHViTW9kZWwuQXNzb2NpYXRlZERhdGFJbnB1dElkXHJcbiAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0ID0gY2hhbm5lbEh1Yk1vZGVsLkFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXRcclxuICAgICAgICB0aGlzLkV2YWx1YXRlZFRyYW5zZmVyRnVuY3Rpb25TZXQgPSBbXVxyXG4gICAgfVxyXG5cclxuICAgIC8vIG1ldGhvZHNcclxuICAgIHB1YmxpYyBHZXRUcmFuc2Zvcm1lZFZhbHVlID0gKHZhbHVlOiBhbnkpOiBzdHJpbmcgPT4gXHJcbiAgICB7XHJcbiAgICAgICAgaWYgKHZhbHVlID09PSBcIk5hTlwiKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdmFsdWUgPSBOYU5cclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuRXZhbHVhdGVkVHJhbnNmZXJGdW5jdGlvblNldC5mb3JFYWNoKHRmID0+IHZhbHVlID0gdGYodmFsdWUpKVxyXG5cclxuICAgICAgICByZXR1cm4gdmFsdWVcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIENyZWF0ZURlZmF1bHRUcmFuc2ZlckZ1bmN0aW9uID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gbmV3IFRyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWwobmV3IFRyYW5zZmVyRnVuY3Rpb25Nb2RlbChcIjAwMDEtMDEtMDFUMDA6MDA6MDBaXCIsIFwicG9seW5vbWlhbFwiLCBcInBlcm1hbmVudFwiLCBcIjE7MFwiKSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgSXNBc3NvY2lhdGlvbkFsbG93ZWQoZGF0YVBvcnQ6IERhdGFQb3J0Vmlld01vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiAoZGF0YVBvcnQuRGF0YVR5cGUgJiAweGZmKSA9PSAodGhpcy5EYXRhVHlwZSgpICYgMHhmZilcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVXBkYXRlQXNzb2NpYXRpb24gPSAoZGF0YVBvcnQ6IERhdGFQb3J0Vmlld01vZGVsKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHN3aXRjaCAoZGF0YVBvcnQuRGF0YURpcmVjdGlvbilcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uSW5wdXQ6XHJcblxyXG4gICAgICAgICAgICAgICAgdGhpcy5SZXNldEFzc29jaWF0aW9uKGZhbHNlLCB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXQoKSlcclxuICAgICAgICAgICAgICAgIGJyZWFrXHJcblxyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dDpcclxuXHJcbiAgICAgICAgICAgICAgICB0aGlzLlJlc2V0QXNzb2NpYXRpb24oZmFsc2UsIGRhdGFQb3J0KVxyXG4gICAgICAgICAgICAgICAgYnJlYWtcclxuXHJcbiAgICAgICAgICAgIGRlZmF1bHQ6XHJcbiAgICAgICAgICAgICAgICB0aHJvdyBuZXcgRXJyb3IoXCJOb3QgaW1wbGVtZW50ZWQuXCIpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5TZXRBc3NvY2lhdGlvbihkYXRhUG9ydClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgU2V0QXNzb2NpYXRpb24oZGF0YVBvcnQ6IERhdGFQb3J0Vmlld01vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIGRhdGFQb3J0LkFzc29jaWF0ZWRDaGFubmVsSHViU2V0LnB1c2godGhpcylcclxuXHJcbiAgICAgICAgc3dpdGNoIChkYXRhUG9ydC5EYXRhRGlyZWN0aW9uKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dDpcclxuXHJcbiAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXQoZGF0YVBvcnQpXHJcbiAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXRJZCA9IGRhdGFQb3J0LlRvRnVsbFF1YWxpZmllZElkZW50aWZpZXIoKVxyXG5cclxuICAgICAgICAgICAgICAgIGJyZWFrXHJcblxyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dDpcclxuXHJcbiAgICAgICAgICAgICAgICBsZXQgZGF0YU91dHB1dElkID0gZGF0YVBvcnQuVG9GdWxsUXVhbGlmaWVkSWRlbnRpZmllcigpXHJcblxyXG4gICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dFNldC5wdXNoKGRhdGFQb3J0KVxyXG5cclxuICAgICAgICAgICAgICAgIGlmICh0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQuaW5kZXhPZihkYXRhT3V0cHV0SWQpIDwgMClcclxuICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQucHVzaChkYXRhUG9ydC5Ub0Z1bGxRdWFsaWZpZWRJZGVudGlmaWVyKCkpXHJcbiAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgYnJlYWtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFJlc2V0QXNzb2NpYXRpb24obWFpbnRhaW5XZWFrUmVmZXJlbmNlOiBib29sZWFuLCAuLi5kYXRhUG9ydFNldDogRGF0YVBvcnRWaWV3TW9kZWxbXSlcclxuICAgIHtcclxuICAgICAgICBkYXRhUG9ydFNldC5mb3JFYWNoKGRhdGFQb3J0ID0+XHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBpZiAoIWRhdGFQb3J0KVxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICByZXR1cm5cclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgZGF0YVBvcnQuQXNzb2NpYXRlZENoYW5uZWxIdWJTZXQucmVtb3ZlKHRoaXMpXHJcblxyXG4gICAgICAgICAgICBzd2l0Y2ggKGRhdGFQb3J0LkRhdGFEaXJlY3Rpb24pXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uSW5wdXQ6XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dChudWxsKVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBpZiAoIW1haW50YWluV2Vha1JlZmVyZW5jZSlcclxuICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dElkID0gbnVsbFxyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgYnJlYWtcclxuXHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dDpcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dFNldC5yZW1vdmUoZGF0YVBvcnQpXHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmICghbWFpbnRhaW5XZWFrUmVmZXJlbmNlKVxyXG4gICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgbGV0IGluZGV4OiBudW1iZXIgPSB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQuaW5kZXhPZihkYXRhUG9ydC5Ub0Z1bGxRdWFsaWZpZWRJZGVudGlmaWVyKCkpXHJcblxyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAoaW5kZXggPiAtMSlcclxuICAgICAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0LnNwbGljZShpbmRleCwgMSlcclxuICAgICAgICAgICAgICAgICAgICAgICAgfSAgIFxyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgYnJlYWtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH0pXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFJlc2V0QWxsQXNzb2NpYXRpb25zKG1haW50YWluV2Vha1JlZmVyZW5jZTogYm9vbGVhbilcclxuICAgIHtcclxuICAgICAgICBpZiAodGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0KCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLlJlc2V0QXNzb2NpYXRpb24obWFpbnRhaW5XZWFrUmVmZXJlbmNlLCB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXQoKSlcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuUmVzZXRBc3NvY2lhdGlvbihtYWludGFpbldlYWtSZWZlcmVuY2UsIC4uLnRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRTZXQoKSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgR2V0QXNzb2NpYXRlZERhdGFJbnB1dElkID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0SWQ7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEdldEFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQ7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvTW9kZWwoKVxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB7XHJcbiAgICAgICAgICAgIE5hbWU6IDxzdHJpbmc+dGhpcy5OYW1lKCksXHJcbiAgICAgICAgICAgIEdyb3VwOiA8c3RyaW5nPnRoaXMuR3JvdXAoKSxcclxuICAgICAgICAgICAgRGF0YVR5cGU6IDxPbmVEYXNEYXRhVHlwZUVudW0+dGhpcy5EYXRhVHlwZSgpLFxyXG4gICAgICAgICAgICBHdWlkOiA8c3RyaW5nPnRoaXMuR3VpZCxcclxuICAgICAgICAgICAgQ3JlYXRpb25EYXRlVGltZTogPHN0cmluZz50aGlzLkNyZWF0aW9uRGF0ZVRpbWUsXHJcbiAgICAgICAgICAgIFVuaXQ6IDxzdHJpbmc+dGhpcy5Vbml0KCksXHJcbiAgICAgICAgICAgIFRyYW5zZmVyRnVuY3Rpb25TZXQ6IDxUcmFuc2ZlckZ1bmN0aW9uTW9kZWxbXT50aGlzLlRyYW5zZmVyRnVuY3Rpb25TZXQoKS5tYXAodGYgPT4gdGYuVG9Nb2RlbCgpKSxcclxuICAgICAgICAgICAgQXNzb2NpYXRlZERhdGFJbnB1dElkOiA8c3RyaW5nPnRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dElkLFxyXG4gICAgICAgICAgICBBc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0OiA8c3RyaW5nW10+dGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBBZGRUcmFuc2ZlckZ1bmN0aW9uID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLlRyYW5zZmVyRnVuY3Rpb25TZXQucHVzaCh0aGlzLlNlbGVjdGVkVHJhbnNmZXJGdW5jdGlvbigpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBEZWxldGVUcmFuc2ZlckZ1bmN0aW9uID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLlRyYW5zZmVyRnVuY3Rpb25TZXQucmVtb3ZlKHRoaXMuU2VsZWN0ZWRUcmFuc2ZlckZ1bmN0aW9uKCkpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIE5ld1RyYW5zZmVyRnVuY3Rpb24gPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuU2VsZWN0ZWRUcmFuc2ZlckZ1bmN0aW9uKHRoaXMuQ3JlYXRlRGVmYXVsdFRyYW5zZmVyRnVuY3Rpb24oKSlcclxuICAgIH1cclxuXHJcbiAgICAvLyBjb21tYW5kc1xyXG4gICAgcHVibGljIFNlbGVjdFRyYW5zZmVyRnVuY3Rpb24gPSAodHJhbnNmZXJGdW5jdGlvbjogVHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLlNlbGVjdGVkVHJhbnNmZXJGdW5jdGlvbih0cmFuc2ZlckZ1bmN0aW9uKVxyXG4gICAgfVxyXG59IiwiY2xhc3MgT25lRGFzTW9kdWxlVmlld01vZGVsXHJcbntcclxuICAgIHB1YmxpYyBEYXRhVHlwZTogS25vY2tvdXRPYnNlcnZhYmxlPE9uZURhc0RhdGFUeXBlRW51bT5cclxuICAgIHB1YmxpYyBEYXRhRGlyZWN0aW9uOiBLbm9ja291dE9ic2VydmFibGU8RGF0YURpcmVjdGlvbkVudW0+XHJcbiAgICBwdWJsaWMgRW5kaWFubmVzczogS25vY2tvdXRPYnNlcnZhYmxlPEVuZGlhbm5lc3NFbnVtPlxyXG4gICAgcHVibGljIFNpemU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxudW1iZXI+XHJcbiAgICBwdWJsaWMgTWF4U2l6ZTogS25vY2tvdXRPYnNlcnZhYmxlPG51bWJlcj5cclxuICAgIHB1YmxpYyBFcnJvck1lc3NhZ2U6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgSGFzRXJyb3I6IEtub2Nrb3V0Q29tcHV0ZWQ8Ym9vbGVhbj5cclxuXHJcbiAgICBwdWJsaWMgRGF0YVR5cGVTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PE9uZURhc0RhdGFUeXBlRW51bT5cclxuXHJcbiAgICBwcml2YXRlIF9vblByb3BlcnR5Q2hhbmdlZDogRXZlbnREaXNwYXRjaGVyPE9uZURhc01vZHVsZVZpZXdNb2RlbCwgYW55PlxyXG4gICAgcHJvdGVjdGVkIF9tb2RlbDogYW55XHJcblxyXG4gICAgY29uc3RydWN0b3Iob25lRGFzTW9kdWxlTW9kZWw6IE9uZURhc01vZHVsZU1vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuX21vZGVsID0gb25lRGFzTW9kdWxlTW9kZWxcclxuXHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZSA9IGtvLm9ic2VydmFibGU8T25lRGFzRGF0YVR5cGVFbnVtPihvbmVEYXNNb2R1bGVNb2RlbC5EYXRhVHlwZSlcclxuICAgICAgICB0aGlzLkRhdGFEaXJlY3Rpb24gPSBrby5vYnNlcnZhYmxlPERhdGFEaXJlY3Rpb25FbnVtPihvbmVEYXNNb2R1bGVNb2RlbC5EYXRhRGlyZWN0aW9uKVxyXG4gICAgICAgIHRoaXMuRW5kaWFubmVzcyA9IGtvLm9ic2VydmFibGU8RW5kaWFubmVzc0VudW0+KG9uZURhc01vZHVsZU1vZGVsLkVuZGlhbm5lc3MpXHJcbiAgICAgICAgdGhpcy5TaXplID0ga28ub2JzZXJ2YWJsZTxudW1iZXI+KG9uZURhc01vZHVsZU1vZGVsLlNpemUpXHJcbiAgICAgICAgdGhpcy5NYXhTaXplID0ga28ub2JzZXJ2YWJsZTxudW1iZXI+KEluZmluaXR5KVxyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KFwiXCIpXHJcbiAgICAgICAgdGhpcy5IYXNFcnJvciA9IGtvLmNvbXB1dGVkPGJvb2xlYW4+KCgpID0+IHRoaXMuRXJyb3JNZXNzYWdlKCkubGVuZ3RoID4gMClcclxuXHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZVNldCA9IGtvLm9ic2VydmFibGVBcnJheTxPbmVEYXNEYXRhVHlwZUVudW0+KEVudW1lcmF0aW9uSGVscGVyLkdldEVudW1WYWx1ZXMoJ09uZURhc0RhdGFUeXBlRW51bScpLmZpbHRlcihkYXRhVHlwZSA9PiBkYXRhVHlwZSAhPT0gT25lRGFzRGF0YVR5cGVFbnVtLkJPT0xFQU4pKVxyXG4gICAgICAgIHRoaXMuX29uUHJvcGVydHlDaGFuZ2VkID0gbmV3IEV2ZW50RGlzcGF0Y2hlcjxPbmVEYXNNb2R1bGVWaWV3TW9kZWwsIGFueT4oKTtcclxuXHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZS5zdWJzY3JpYmUobmV3VmFsdWUgPT4gdGhpcy5PblByb3BlcnR5Q2hhbmdlZCgpKVxyXG4gICAgICAgIHRoaXMuRGF0YURpcmVjdGlvbi5zdWJzY3JpYmUobmV3VmFsdWUgPT4gdGhpcy5PblByb3BlcnR5Q2hhbmdlZCgpKVxyXG4gICAgICAgIHRoaXMuU2l6ZS5zdWJzY3JpYmUobmV3VmFsdWUgPT4gdGhpcy5PblByb3BlcnR5Q2hhbmdlZCgpKVxyXG4gICAgfVxyXG5cclxuICAgIGdldCBQcm9wZXJ0eUNoYW5nZWQoKTogSUV2ZW50PE9uZURhc01vZHVsZVZpZXdNb2RlbCwgYW55PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLl9vblByb3BlcnR5Q2hhbmdlZDtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgT25Qcm9wZXJ0eUNoYW5nZWQgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuVmFsaWRhdGUoKVxyXG4gICAgICAgIHRoaXMuX29uUHJvcGVydHlDaGFuZ2VkLmRpc3BhdGNoKHRoaXMsIG51bGwpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEdldEJ5dGVDb3VudCA9IChib29sZWFuQml0U2l6ZT86IG51bWJlcikgPT5cclxuICAgIHtcclxuICAgICAgICBpZiAoYm9vbGVhbkJpdFNpemUgJiYgdGhpcy5EYXRhVHlwZSgpID09PSBPbmVEYXNEYXRhVHlwZUVudW0uQk9PTEVBTilcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGJvb2xlYW5CaXRTaXplID0gcGFyc2VJbnQoPGFueT5ib29sZWFuQml0U2l6ZSlcclxuXHJcbiAgICAgICAgICAgIHJldHVybiBNYXRoLmNlaWwoYm9vbGVhbkJpdFNpemUgKiB0aGlzLlNpemUoKSAvIDgpO1xyXG4gICAgICAgIH1cclxuICAgICAgICBlbHNlXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICByZXR1cm4gKHRoaXMuRGF0YVR5cGUoKSAmIDB4MEZGKSAvIDggKiB0aGlzLlNpemUoKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVmFsaWRhdGUoKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiXCIpXHJcblxyXG4gICAgICAgIGlmICh0aGlzLlNpemUoKSA8IDEgfHwgKGlzRmluaXRlKHRoaXMuTWF4U2l6ZSgpKSAmJiB0aGlzLlNpemUoKSA+IHRoaXMuTWF4U2l6ZSgpKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiU2l6ZSBtdXN0IGJlIHdpdGhpbiByYW5nZSAxLi5cIiArIHRoaXMuTWF4U2l6ZSgpICsgXCIuXCIpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb1N0cmluZygpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuU2l6ZSgpICsgXCJ4IFwiICsgRW51bWVyYXRpb25IZWxwZXIuR2V0RW51bUxvY2FsaXphdGlvbignT25lRGFzRGF0YVR5cGVFbnVtJywgdGhpcy5EYXRhVHlwZSgpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBFeHRlbmRNb2RlbChtb2RlbDogYW55KVxyXG4gICAge1xyXG4gICAgICAgIC8vXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvTW9kZWwoKVxyXG4gICAge1xyXG4gICAgICAgIGxldCBtb2RlbDogYW55ID0ge1xyXG4gICAgICAgICAgICAkdHlwZTogPHN0cmluZz50aGlzLl9tb2RlbC4kdHlwZSxcclxuICAgICAgICAgICAgRGF0YVR5cGU6IDxPbmVEYXNEYXRhVHlwZUVudW0+dGhpcy5EYXRhVHlwZSgpLFxyXG4gICAgICAgICAgICBTaXplOiA8bnVtYmVyPnRoaXMuU2l6ZSgpLFxyXG4gICAgICAgICAgICBEYXRhRGlyZWN0aW9uOiA8RGF0YURpcmVjdGlvbkVudW0+dGhpcy5EYXRhRGlyZWN0aW9uKCksXHJcbiAgICAgICAgICAgIEVuZGlhbm5lc3M6IDxFbmRpYW5uZXNzRW51bT50aGlzLkVuZGlhbm5lc3MoKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5FeHRlbmRNb2RlbChtb2RlbClcclxuXHJcbiAgICAgICAgcmV0dXJuIG1vZGVsXHJcbiAgICB9XHJcbn1cclxuIiwiY2xhc3MgT25lRGFzTW9kdWxlU2VsZWN0b3JWaWV3TW9kZWxcclxue1xyXG4gICAgcHVibGljIFNldHRpbmdzVGVtcGxhdGVOYW1lOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIE5ld01vZHVsZTogS25vY2tvdXRPYnNlcnZhYmxlPE9uZURhc01vZHVsZVZpZXdNb2RlbD4gIFxyXG4gICAgcHVibGljIE1heEJ5dGVzOiBLbm9ja291dE9ic2VydmFibGU8bnVtYmVyPlxyXG4gICAgcHVibGljIFJlbWFpbmluZ0J5dGVzOiBLbm9ja291dE9ic2VydmFibGU8bnVtYmVyPlxyXG4gICAgcHVibGljIFJlbWFpbmluZ0NvdW50OiBLbm9ja291dE9ic2VydmFibGU8bnVtYmVyPiAgICBcclxuICAgIHB1YmxpYyBNb2R1bGVTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PE9uZURhc01vZHVsZVZpZXdNb2RlbD5cclxuICAgIHB1YmxpYyBFcnJvck1lc3NhZ2U6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgSGFzRXJyb3I6IEtub2Nrb3V0Q29tcHV0ZWQ8Ym9vbGVhbj5cclxuXHJcbiAgICBwdWJsaWMgT25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlOiBLbm9ja291dE9ic2VydmFibGU8T25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlRW51bT5cclxuXHJcbiAgICBwcml2YXRlIF9vbk1vZHVsZVNldENoYW5nZWQ6IEV2ZW50RGlzcGF0Y2hlcjxPbmVEYXNNb2R1bGVTZWxlY3RvclZpZXdNb2RlbCwgT25lRGFzTW9kdWxlVmlld01vZGVsW10+XHJcblxyXG4gICAgY29uc3RydWN0b3Iob25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlOiBPbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtLCBtb2R1bGVTZXQ6IE9uZURhc01vZHVsZVZpZXdNb2RlbFtdID0gW10pXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5PbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGUgPSBrby5vYnNlcnZhYmxlPE9uZURhc01vZHVsZVNlbGVjdG9yTW9kZUVudW0+KG9uZURhc01vZHVsZVNlbGVjdG9yTW9kZSlcclxuXHJcbiAgICAgICAgdGhpcy5TZXR0aW5nc1RlbXBsYXRlTmFtZSA9IGtvLm9ic2VydmFibGUoXCJQcm9qZWN0X09uZURhc01vZHVsZVNldHRpbmdzVGVtcGxhdGVcIilcclxuICAgICAgICB0aGlzLk5ld01vZHVsZSA9IGtvLm9ic2VydmFibGU8T25lRGFzTW9kdWxlVmlld01vZGVsPigpO1xyXG4gICAgICAgIHRoaXMuTWF4Qnl0ZXMgPSBrby5vYnNlcnZhYmxlPG51bWJlcj4oSW5maW5pdHkpO1xyXG4gICAgICAgIHRoaXMuUmVtYWluaW5nQnl0ZXMgPSBrby5vYnNlcnZhYmxlPG51bWJlcj4oTmFOKTtcclxuICAgICAgICB0aGlzLlJlbWFpbmluZ0NvdW50ID0ga28ub2JzZXJ2YWJsZTxudW1iZXI+KE5hTik7XHJcbiAgICAgICAgdGhpcy5Nb2R1bGVTZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8T25lRGFzTW9kdWxlVmlld01vZGVsPihtb2R1bGVTZXQpO1xyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KFwiXCIpXHJcbiAgICAgICAgdGhpcy5IYXNFcnJvciA9IGtvLmNvbXB1dGVkPGJvb2xlYW4+KCgpID0+IHRoaXMuRXJyb3JNZXNzYWdlKCkubGVuZ3RoID4gMClcclxuXHJcbiAgICAgICAgdGhpcy5fb25Nb2R1bGVTZXRDaGFuZ2VkID0gbmV3IEV2ZW50RGlzcGF0Y2hlcjxPbmVEYXNNb2R1bGVTZWxlY3RvclZpZXdNb2RlbCwgT25lRGFzTW9kdWxlVmlld01vZGVsW10+KCk7XHJcblxyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxDcmVhdGVOZXdNb2R1bGUoKVxyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgfVxyXG5cclxuICAgIGdldCBPbk1vZHVsZVNldENoYW5nZWQoKTogSUV2ZW50PE9uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsLCBPbmVEYXNNb2R1bGVWaWV3TW9kZWxbXT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5fb25Nb2R1bGVTZXRDaGFuZ2VkO1xyXG4gICAgfVxyXG4gICAgXHJcbiAgICAvLyBtZXRob2RzXHJcbiAgICBwdWJsaWMgU2V0TWF4Qnl0ZXMgPSAodmFsdWU6IG51bWJlcikgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLk1heEJ5dGVzKHZhbHVlKVxyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBHZXRJbnB1dE1vZHVsZVNldCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuTW9kdWxlU2V0KCkuZmlsdGVyKG1vZHVsZSA9PiBtb2R1bGUuRGF0YURpcmVjdGlvbigpID09PSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgR2V0T3V0cHV0TW9kdWxlU2V0ID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5Nb2R1bGVTZXQoKS5maWx0ZXIobW9kdWxlID0+IG1vZHVsZS5EYXRhRGlyZWN0aW9uKCkgPT09IERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dClcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIEludGVybmFsVXBkYXRlKClcclxuICAgIHtcclxuICAgICAgICB0aGlzLlVwZGF0ZSgpXHJcbiAgICAgICAgdGhpcy5WYWxpZGF0ZSgpXHJcbiAgICB9XHJcblxyXG4gICAgcHJvdGVjdGVkIFVwZGF0ZSgpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IG1vZHVsZVNldDogT25lRGFzTW9kdWxlVmlld01vZGVsW11cclxuICAgICAgICBsZXQgcmVtYWluaW5nQnl0ZXM6IG51bWJlclxyXG5cclxuICAgICAgICBzd2l0Y2ggKHRoaXMuTmV3TW9kdWxlKCkuRGF0YURpcmVjdGlvbigpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dDpcclxuICAgICAgICAgICAgICAgIG1vZHVsZVNldCA9IHRoaXMuR2V0SW5wdXRNb2R1bGVTZXQoKVxyXG4gICAgICAgICAgICAgICAgYnJlYWs7XHJcblxyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dDpcclxuICAgICAgICAgICAgICAgIG1vZHVsZVNldCA9IHRoaXMuR2V0T3V0cHV0TW9kdWxlU2V0KClcclxuICAgICAgICAgICAgICAgIGJyZWFrO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmVtYWluaW5nQnl0ZXMgPSB0aGlzLk1heEJ5dGVzKCkgLSBtb2R1bGVTZXQubWFwKG9uZURhc01vZHVsZSA9PiBvbmVEYXNNb2R1bGUuR2V0Qnl0ZUNvdW50KCkpLnJlZHVjZSgocHJldmlvdXNWYWx1ZSwgY3VycmVudFZhbHVlKSA9PiBwcmV2aW91c1ZhbHVlICsgY3VycmVudFZhbHVlLCAwKVxyXG5cclxuICAgICAgICB0aGlzLlJlbWFpbmluZ0J5dGVzKHJlbWFpbmluZ0J5dGVzKVxyXG4gICAgICAgIHRoaXMuUmVtYWluaW5nQ291bnQoTWF0aC5mbG9vcih0aGlzLlJlbWFpbmluZ0J5dGVzKCkgLyAoKHRoaXMuTmV3TW9kdWxlKCkuRGF0YVR5cGUoKSAmIDB4MEZGKSAvIDgpKSlcclxuICAgIH1cclxuXHJcbiAgICBwcm90ZWN0ZWQgVmFsaWRhdGUoKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiXCIpXHJcblxyXG4gICAgICAgIGlmICh0aGlzLk5ld01vZHVsZSgpLkhhc0Vycm9yKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIlJlc29sdmUgYWxsIHJlbWFpbmluZyBtb2R1bGUgZXJyb3JzIGJlZm9yZSBjb250aW51aW5nLlwiKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaWYgKHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlKCkgPT09IE9uZURhc01vZHVsZVNlbGVjdG9yTW9kZUVudW0uSW5wdXRPbmx5ICYmIHRoaXMuTmV3TW9kdWxlKCkuRGF0YURpcmVjdGlvbigpID09IERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dClcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiT25seSBpbnB1dCBtb2R1bGVzIGFyZSBhbGxvd2VkLlwiKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaWYgKHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlKCkgPT09IE9uZURhc01vZHVsZVNlbGVjdG9yTW9kZUVudW0uT3V0cHV0T25seSAmJiB0aGlzLk5ld01vZHVsZSgpLkRhdGFEaXJlY3Rpb24oKSA9PSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dClcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiT25seSBvdXRwdXQgbW9kdWxlcyBhcmUgYWxsb3dlZC5cIilcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmIChpc0Zpbml0ZSh0aGlzLlJlbWFpbmluZ0J5dGVzKCkpICYmICh0aGlzLlJlbWFpbmluZ0J5dGVzKCkgLSB0aGlzLk5ld01vZHVsZSgpLkdldEJ5dGVDb3VudCgpIDwgMCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIkJ5dGUgY291bnQgb2YgbmV3IG1vZHVsZSBpcyB0b28gaGlnaC5cIilcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmICh0aGlzLlJlbWFpbmluZ0NvdW50KCkgPD0gMClcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiVGhlIG1heGltdW0gbnVtYmVyIG9mIG1vZHVsZXMgaXMgcmVhY2hlZC5cIilcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHJvdGVjdGVkIENyZWF0ZU5ld01vZHVsZSgpXHJcbiAgICB7XHJcbiAgICAgICAgaWYgKHRoaXMuTmV3TW9kdWxlKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICByZXR1cm4gbmV3IE9uZURhc01vZHVsZVZpZXdNb2RlbChuZXcgT25lRGFzTW9kdWxlTW9kZWwodGhpcy5OZXdNb2R1bGUoKS5EYXRhVHlwZSgpLCB0aGlzLk5ld01vZHVsZSgpLkRhdGFEaXJlY3Rpb24oKSwgdGhpcy5OZXdNb2R1bGUoKS5FbmRpYW5uZXNzKCksIDEpKVxyXG4gICAgICAgIH1cclxuICAgICAgICBlbHNlXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICByZXR1cm4gbmV3IE9uZURhc01vZHVsZVZpZXdNb2RlbChuZXcgT25lRGFzTW9kdWxlTW9kZWwoT25lRGFzRGF0YVR5cGVFbnVtLlVJTlQxNiwgRGF0YURpcmVjdGlvbkVudW0uSW5wdXQsIEVuZGlhbm5lc3NFbnVtLkxpdHRsZUVuZGlhbiwgMSkpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgSW50ZXJuYWxDcmVhdGVOZXdNb2R1bGUoKVxyXG4gICAge1xyXG4gICAgICAgIGlmICh0aGlzLk5ld01vZHVsZSgpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5OZXdNb2R1bGUoKS5Qcm9wZXJ0eUNoYW5nZWQudW5zdWJzY3JpYmUodGhpcy5Pbk1vZHVsZVByb3BlcnR5Q2hhbmdlZClcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuTmV3TW9kdWxlKHRoaXMuQ3JlYXRlTmV3TW9kdWxlKCkpXHJcbiAgICAgICAgdGhpcy5OZXdNb2R1bGUoKS5Qcm9wZXJ0eUNoYW5nZWQuc3Vic2NyaWJlKHRoaXMuT25Nb2R1bGVQcm9wZXJ0eUNoYW5nZWQpXHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBPbk1vZHVsZVByb3BlcnR5Q2hhbmdlZCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5JbnRlcm5hbFVwZGF0ZSgpXHJcbiAgICB9XHJcblxyXG4gICAgLy8gY29tbWFuZHNcclxuICAgIHB1YmxpYyBBZGRNb2R1bGUgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIGxldCBuZXdNb2R1bGU6IE9uZURhc01vZHVsZVZpZXdNb2RlbFxyXG5cclxuICAgICAgICBpZiAoIXRoaXMuSGFzRXJyb3IoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuTW9kdWxlU2V0LnB1c2godGhpcy5OZXdNb2R1bGUoKSlcclxuICAgICAgICAgICAgdGhpcy5JbnRlcm5hbENyZWF0ZU5ld01vZHVsZSgpXHJcbiAgICAgICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgICAgICAgICB0aGlzLl9vbk1vZHVsZVNldENoYW5nZWQuZGlzcGF0Y2godGhpcywgdGhpcy5Nb2R1bGVTZXQoKSlcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIERlbGV0ZU1vZHVsZSA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5Nb2R1bGVTZXQucG9wKClcclxuICAgICAgICB0aGlzLkludGVybmFsVXBkYXRlKClcclxuICAgICAgICB0aGlzLl9vbk1vZHVsZVNldENoYW5nZWQuZGlzcGF0Y2godGhpcywgdGhpcy5Nb2R1bGVTZXQoKSlcclxuICAgIH1cclxufSIsImNsYXNzIFRyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWxcclxue1xyXG4gICAgcHVibGljIERhdGVUaW1lOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIFR5cGU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgT3B0aW9uOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIEFyZ3VtZW50OiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKHRyYW5zZmVyRnVuY3Rpb25Nb2RlbDogVHJhbnNmZXJGdW5jdGlvbk1vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuRGF0ZVRpbWUgPSBrby5vYnNlcnZhYmxlKHRyYW5zZmVyRnVuY3Rpb25Nb2RlbC5EYXRlVGltZSlcclxuICAgICAgICB0aGlzLlR5cGUgPSBrby5vYnNlcnZhYmxlKHRyYW5zZmVyRnVuY3Rpb25Nb2RlbC5UeXBlKVxyXG4gICAgICAgIHRoaXMuT3B0aW9uID0ga28ub2JzZXJ2YWJsZSh0cmFuc2ZlckZ1bmN0aW9uTW9kZWwuT3B0aW9uKVxyXG4gICAgICAgIHRoaXMuQXJndW1lbnQgPSBrby5vYnNlcnZhYmxlKHRyYW5zZmVyRnVuY3Rpb25Nb2RlbC5Bcmd1bWVudClcclxuICAgIH1cclxuXHJcbiAgICAvLyBtZXRob2RzXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIG5ldyBUcmFuc2ZlckZ1bmN0aW9uTW9kZWwodGhpcy5EYXRlVGltZSgpLCB0aGlzLlR5cGUoKSwgdGhpcy5PcHRpb24oKSwgdGhpcy5Bcmd1bWVudCgpKVxyXG4gICAgfVxyXG59IiwiY2xhc3MgQnVmZmVyUmVxdWVzdFZpZXdNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgU2FtcGxlUmF0ZTogS25vY2tvdXRPYnNlcnZhYmxlPFNhbXBsZVJhdGVFbnVtPlxyXG4gICAgcHVibGljIEdyb3VwRmlsdGVyOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKG1vZGVsOiBCdWZmZXJSZXF1ZXN0TW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5TYW1wbGVSYXRlID0ga28ub2JzZXJ2YWJsZTxTYW1wbGVSYXRlRW51bT4obW9kZWwuU2FtcGxlUmF0ZSk7XHJcbiAgICAgICAgdGhpcy5Hcm91cEZpbHRlciA9IGtvLm9ic2VydmFibGU8c3RyaW5nPihtb2RlbC5Hcm91cEZpbHRlcik7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvTW9kZWwoKSB7XHJcbiAgICAgICAgcmV0dXJuIHtcclxuICAgICAgICAgICAgU2FtcGxlUmF0ZTogPFNhbXBsZVJhdGVFbnVtPnRoaXMuU2FtcGxlUmF0ZSgpLFxyXG4gICAgICAgICAgICBHcm91cEZpbHRlcjogPHN0cmluZz50aGlzLkdyb3VwRmlsdGVyKClcclxuICAgICAgICB9XHJcbiAgICB9XHJcbn0iLCJjbGFzcyBEYXRhUG9ydFZpZXdNb2RlbFxyXG57XHJcbiAgICAvLyBmaWVsZHNcclxuICAgIHB1YmxpYyBOYW1lOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IERhdGFUeXBlOiBPbmVEYXNEYXRhVHlwZUVudW1cclxuICAgIHB1YmxpYyByZWFkb25seSBEYXRhRGlyZWN0aW9uOiBEYXRhRGlyZWN0aW9uRW51bVxyXG4gICAgcHVibGljIHJlYWRvbmx5IEVuZGlhbm5lc3M6IEVuZGlhbm5lc3NFbnVtXHJcblxyXG4gICAgcHVibGljIElzU2VsZWN0ZWQ6IEtub2Nrb3V0T2JzZXJ2YWJsZTxib29sZWFuPlxyXG4gICAgcHVibGljIEFzc29jaWF0ZWRDaGFubmVsSHViU2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxDaGFubmVsSHViVmlld01vZGVsPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IEFzc29jaWF0ZWREYXRhR2F0ZXdheTogRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgTGl2ZURlc2NyaXB0aW9uOiBLbm9ja291dENvbXB1dGVkPHN0cmluZz5cclxuXHJcbiAgICAvLyBjb25zdHJ1Y3RvcnNcclxuICAgIGNvbnN0cnVjdG9yKGRhdGFQb3J0TW9kZWw6IGFueSwgYXNzb2NpYXRlZERhdGFHYXRld2F5OiBEYXRhR2F0ZXdheVZpZXdNb2RlbEJhc2UpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5OYW1lID0ga28ub2JzZXJ2YWJsZShkYXRhUG9ydE1vZGVsLk5hbWUpXHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZSA9IGRhdGFQb3J0TW9kZWwuRGF0YVR5cGVcclxuICAgICAgICB0aGlzLkRhdGFEaXJlY3Rpb24gPSBkYXRhUG9ydE1vZGVsLkRhdGFEaXJlY3Rpb25cclxuICAgICAgICB0aGlzLkVuZGlhbm5lc3MgPSBkYXRhUG9ydE1vZGVsLkVuZGlhbm5lc3NcclxuXHJcbiAgICAgICAgdGhpcy5Jc1NlbGVjdGVkID0ga28ub2JzZXJ2YWJsZTxib29sZWFuPihmYWxzZSlcclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWRDaGFubmVsSHViU2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PENoYW5uZWxIdWJWaWV3TW9kZWw+KClcclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhR2F0ZXdheSA9IGFzc29jaWF0ZWREYXRhR2F0ZXdheVxyXG5cclxuICAgICAgICB0aGlzLkxpdmVEZXNjcmlwdGlvbiA9IGtvLmNvbXB1dGVkKCgpID0+XHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBsZXQgcmVzdWx0OiBzdHJpbmdcclxuXHJcbiAgICAgICAgICAgIHJlc3VsdCA9IFwiPGRpdiBjbGFzcz0ndGV4dC1sZWZ0Jz5cIiArIHRoaXMuTmFtZSgpICsgXCI8L2Rpdj48ZGl2IGNsYXNzPSd0ZXh0LWxlZnQnPlwiICsgRW51bWVyYXRpb25IZWxwZXIuR2V0RW51bUxvY2FsaXphdGlvbignT25lRGFzRGF0YVR5cGVFbnVtJywgdGhpcy5EYXRhVHlwZSkgKyBcIjwvZGl2PlwiXHJcblxyXG4gICAgICAgICAgICBpZiAodGhpcy5Bc3NvY2lhdGVkQ2hhbm5lbEh1YlNldCgpLmxlbmd0aCA+IDApXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZENoYW5uZWxIdWJTZXQoKS5mb3JFYWNoKGNoYW5uZWxIdWIgPT5cclxuICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICByZXN1bHQgKz0gXCI8L2JyID48ZGl2IGNsYXNzPSd0ZXh0LWxlZnQnPlwiICsgY2hhbm5lbEh1Yi5OYW1lKCkgKyBcIjwvZGl2PjxkaXYgY2xhc3M9J3RleHQtbGVmdCc+XCIgKyBFbnVtZXJhdGlvbkhlbHBlci5HZXRFbnVtTG9jYWxpemF0aW9uKCdPbmVEYXNEYXRhVHlwZUVudW0nLCBjaGFubmVsSHViLkRhdGFUeXBlKCkpICsgXCI8L2Rpdj5cIlxyXG4gICAgICAgICAgICAgICAgfSlcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgcmV0dXJuIHJlc3VsdFxyXG4gICAgICAgIH0pXHJcbiAgICB9XHJcblxyXG4gICAgLy8gbWV0aG9kc1xyXG4gICAgcHVibGljIEdldElkKCk6IHN0cmluZ1xyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLk5hbWUoKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb0Z1bGxRdWFsaWZpZWRJZGVudGlmaWVyKCk6IHN0cmluZ1xyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLkFzc29jaWF0ZWREYXRhR2F0ZXdheS5EZXNjcmlwdGlvbi5JZCArIFwiIChcIiArIHRoaXMuQXNzb2NpYXRlZERhdGFHYXRld2F5LkRlc2NyaXB0aW9uLkluc3RhbmNlSWQgKyBcIikgLyBcIiArIHRoaXMuR2V0SWQoKTtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgRXh0ZW5kTW9kZWwobW9kZWw6IGFueSlcclxuICAgIHtcclxuICAgICAgICAvL1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb01vZGVsKClcclxuICAgIHtcclxuICAgICAgICBsZXQgbW9kZWw6IGFueSA9IHtcclxuICAgICAgICAgICAgTmFtZTogPHN0cmluZz50aGlzLk5hbWUoKSxcclxuICAgICAgICAgICAgRGF0YVR5cGU6IDxPbmVEYXNEYXRhVHlwZUVudW0+dGhpcy5EYXRhVHlwZSxcclxuICAgICAgICAgICAgRGF0YURpcmVjdGlvbjogPERhdGFEaXJlY3Rpb25FbnVtPnRoaXMuRGF0YURpcmVjdGlvblxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5FeHRlbmRNb2RlbChtb2RlbClcclxuXHJcbiAgICAgICAgcmV0dXJuIG1vZGVsXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFJlc2V0QXNzb2NpYXRpb25zKG1haW50YWluV2Vha1JlZmVyZW5jZTogYm9vbGVhbilcclxuICAgIHtcclxuICAgICAgICBpZiAodGhpcy5Bc3NvY2lhdGVkQ2hhbm5lbEh1YlNldCgpLmxlbmd0aCA+IDApXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWRDaGFubmVsSHViU2V0KCkuZm9yRWFjaChjaGFubmVsSHViID0+IGNoYW5uZWxIdWIuUmVzZXRBc3NvY2lhdGlvbihtYWludGFpbldlYWtSZWZlcmVuY2UsIHRoaXMpKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxufSIsImFic3RyYWN0IGNsYXNzIFBsdWdpblZpZXdNb2RlbEJhc2Vcclxue1xyXG4gICAgcHVibGljIERlc2NyaXB0aW9uOiBQbHVnaW5EZXNjcmlwdGlvblZpZXdNb2RlbFxyXG4gICAgcHVibGljIFBsdWdpbklkZW50aWZpY2F0aW9uOiBQbHVnaW5JZGVudGlmaWNhdGlvblZpZXdNb2RlbFxyXG4gICAgcHVibGljIElzSW5TZXR0aW5nc01vZGU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxib29sZWFuPlxyXG5cclxuICAgIHByaXZhdGUgX21vZGVsOiBhbnlcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihwbHVnaW5TZXR0aW5nc01vZGVsOiBhbnksIHBsdWdpbklkZW50aWZpY2F0aW9uOiBQbHVnaW5JZGVudGlmaWNhdGlvblZpZXdNb2RlbClcclxuICAgIHtcclxuICAgICAgICB0aGlzLl9tb2RlbCA9IHBsdWdpblNldHRpbmdzTW9kZWxcclxuICAgICAgICB0aGlzLkRlc2NyaXB0aW9uID0gbmV3IFBsdWdpbkRlc2NyaXB0aW9uVmlld01vZGVsKHBsdWdpblNldHRpbmdzTW9kZWwuRGVzY3JpcHRpb24pXHJcbiAgICAgICAgdGhpcy5QbHVnaW5JZGVudGlmaWNhdGlvbiA9IHBsdWdpbklkZW50aWZpY2F0aW9uXHJcbiAgICAgICAgdGhpcy5Jc0luU2V0dGluZ3NNb2RlID0ga28ub2JzZXJ2YWJsZTxib29sZWFuPihmYWxzZSlcclxuICAgIH1cclxuXHJcbiAgICAvLyBtZXRob2RzXHJcbiAgICBwdWJsaWMgYWJzdHJhY3QgYXN5bmMgSW5pdGlhbGl6ZUFzeW5jKCk6IFByb21pc2U8YW55PlxyXG5cclxuICAgIHB1YmxpYyBTZW5kQWN0aW9uUmVxdWVzdCA9IGFzeW5jIChpbnN0YW5jZUlkOiBudW1iZXIsIG1ldGhvZE5hbWU6IHN0cmluZywgZGF0YTogYW55KSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiA8QWN0aW9uUmVzcG9uc2U+IGF3YWl0IENvbm5lY3Rpb25NYW5hZ2VyLkludm9rZVdlYkNsaWVudEh1YihcIlJlcXVlc3RBY3Rpb25cIiwgbmV3IEFjdGlvblJlcXVlc3QodGhpcy5EZXNjcmlwdGlvbi5JZCwgaW5zdGFuY2VJZCwgbWV0aG9kTmFtZSwgZGF0YSkpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEV4dGVuZE1vZGVsKG1vZGVsOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgLy9cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IG1vZGVsOiBhbnkgPSB7XHJcbiAgICAgICAgICAgICR0eXBlOiA8c3RyaW5nPnRoaXMuX21vZGVsLiR0eXBlLFxyXG4gICAgICAgICAgICBEZXNjcmlwdGlvbjogPFBsdWdpbkRlc2NyaXB0aW9uVmlld01vZGVsPnRoaXMuRGVzY3JpcHRpb24uVG9Nb2RlbCgpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0aGlzLkV4dGVuZE1vZGVsKG1vZGVsKVxyXG5cclxuICAgICAgICByZXR1cm4gbW9kZWxcclxuICAgIH1cclxuXHJcbiAgICAvLyBjb21tYW5kc1xyXG4gICAgcHVibGljIEVuYWJsZVNldHRpbmdzTW9kZSA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5Jc0luU2V0dGluZ3NNb2RlKHRydWUpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIERpc2FibGVTZXR0aW5nc01vZGUgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuSXNJblNldHRpbmdzTW9kZShmYWxzZSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9nZ2xlU2V0dGluZ3NNb2RlID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLklzSW5TZXR0aW5nc01vZGUoIXRoaXMuSXNJblNldHRpbmdzTW9kZSgpKVxyXG4gICAgfVxyXG59IiwiLy8vIDxyZWZlcmVuY2UgcGF0aD1cIlBsdWdpblZpZXdNb2RlbEJhc2UudHNcIi8+XHJcblxyXG5hYnN0cmFjdCBjbGFzcyBEYXRhR2F0ZXdheVZpZXdNb2RlbEJhc2UgZXh0ZW5kcyBQbHVnaW5WaWV3TW9kZWxCYXNlXHJcbntcclxuICAgIHB1YmxpYyByZWFkb25seSBNYXhpbXVtRGF0YXNldEFnZTogS25vY2tvdXRPYnNlcnZhYmxlPG51bWJlcj5cclxuICAgIHB1YmxpYyByZWFkb25seSBEYXRhUG9ydFNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8RGF0YVBvcnRWaWV3TW9kZWw+XHJcblxyXG4gICAgY29uc3RydWN0b3IobW9kZWwsIGlkZW50aWZpY2F0aW9uOiBQbHVnaW5JZGVudGlmaWNhdGlvblZpZXdNb2RlbClcclxuICAgIHtcclxuICAgICAgICBzdXBlcihtb2RlbCwgaWRlbnRpZmljYXRpb24pXHJcblxyXG4gICAgICAgIHRoaXMuTWF4aW11bURhdGFzZXRBZ2UgPSBrby5vYnNlcnZhYmxlPG51bWJlcj4obW9kZWwuTWF4aW11bURhdGFzZXRBZ2UpXHJcbiAgICAgICAgdGhpcy5EYXRhUG9ydFNldCA9IGtvLm9ic2VydmFibGVBcnJheTxEYXRhUG9ydFZpZXdNb2RlbD4oKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBFeHRlbmRNb2RlbChtb2RlbDogYW55KVxyXG4gICAge1xyXG4gICAgICAgIHN1cGVyLkV4dGVuZE1vZGVsKG1vZGVsKVxyXG5cclxuICAgICAgICBtb2RlbC5NYXhpbXVtRGF0YXNldEFnZSA9IDxudW1iZXI+TnVtYmVyLnBhcnNlSW50KDxhbnk+dGhpcy5NYXhpbXVtRGF0YXNldEFnZSgpKVxyXG4gICAgfVxyXG59IiwiYWJzdHJhY3QgY2xhc3MgRXh0ZW5kZWREYXRhR2F0ZXdheVZpZXdNb2RlbEJhc2UgZXh0ZW5kcyBEYXRhR2F0ZXdheVZpZXdNb2RlbEJhc2Vcclxue1xyXG4gICAgcHVibGljIE1vZHVsZVRvRGF0YVBvcnRNYXA6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PE9ic2VydmFibGVHcm91cDxEYXRhUG9ydFZpZXdNb2RlbD4+XHJcbiAgICBwdWJsaWMgT25lRGFzTW9kdWxlU2VsZWN0b3I6IEtub2Nrb3V0T2JzZXJ2YWJsZTxPbmVEYXNNb2R1bGVTZWxlY3RvclZpZXdNb2RlbD5cclxuXHJcbiAgICBjb25zdHJ1Y3Rvcihtb2RlbCwgaWRlbnRpZmljYXRpb246IFBsdWdpbklkZW50aWZpY2F0aW9uVmlld01vZGVsLCBvbmVEYXNNb2R1bGVTZWxlY3RvcjogT25lRGFzTW9kdWxlU2VsZWN0b3JWaWV3TW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgc3VwZXIobW9kZWwsIGlkZW50aWZpY2F0aW9uKVxyXG5cclxuICAgICAgICB0aGlzLk1vZHVsZVRvRGF0YVBvcnRNYXAgPSBrby5vYnNlcnZhYmxlQXJyYXkoKVxyXG4gICAgICAgIHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3IgPSBrby5vYnNlcnZhYmxlPE9uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsPihvbmVEYXNNb2R1bGVTZWxlY3RvcilcclxuXHJcbiAgICAgICAgaWYgKHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3IoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3IoKS5Pbk1vZHVsZVNldENoYW5nZWQuc3Vic2NyaWJlKChzZW5kZXIsIGFyZ3MpID0+XHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIHRoaXMuVXBkYXRlRGF0YVBvcnRTZXQoKVxyXG4gICAgICAgICAgICB9KVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgYXN5bmMgSW5pdGlhbGl6ZUFzeW5jKClcclxuICAgIHtcclxuICAgICAgICB0aGlzLlVwZGF0ZURhdGFQb3J0U2V0KClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVXBkYXRlRGF0YVBvcnRTZXQoKVxyXG4gICAge1xyXG4gICAgICAgIGxldCBpbmRleDogbnVtYmVyXHJcbiAgICAgICAgbGV0IG1vZHVsZVRvRGF0YVBvcnRNYXA6IE9ic2VydmFibGVHcm91cDxEYXRhUG9ydFZpZXdNb2RlbD5bXVxyXG5cclxuICAgICAgICBtb2R1bGVUb0RhdGFQb3J0TWFwID0gW11cclxuXHJcbiAgICAgICAgLy8gaW5wdXRzXHJcbiAgICAgICAgaW5kZXggPSAwXHJcblxyXG4gICAgICAgIG1vZHVsZVRvRGF0YVBvcnRNYXAgPSBtb2R1bGVUb0RhdGFQb3J0TWFwLmNvbmNhdCh0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yKCkuTW9kdWxlU2V0KCkuZmlsdGVyKG9uZURhc01vZHVsZSA9PiBvbmVEYXNNb2R1bGUuRGF0YURpcmVjdGlvbigpID09PSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dCkubWFwKG9uZURhc01vZHVsZSA9PlxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgbGV0IGdyb3VwOiBPYnNlcnZhYmxlR3JvdXA8RGF0YVBvcnRWaWV3TW9kZWw+XHJcblxyXG4gICAgICAgICAgICBncm91cCA9IG5ldyBPYnNlcnZhYmxlR3JvdXA8RGF0YVBvcnRWaWV3TW9kZWw+KG9uZURhc01vZHVsZS5Ub1N0cmluZygpLCB0aGlzLkNyZWF0ZURhdGFQb3J0U2V0KG9uZURhc01vZHVsZSwgaW5kZXgpKVxyXG4gICAgICAgICAgICBpbmRleCArPSBvbmVEYXNNb2R1bGUuU2l6ZSgpO1xyXG5cclxuICAgICAgICAgICAgcmV0dXJuIGdyb3VwXHJcbiAgICAgICAgfSkpXHJcblxyXG4gICAgICAgIC8vIG91dHB1dHNcclxuICAgICAgICBpbmRleCA9IDBcclxuXHJcbiAgICAgICAgbW9kdWxlVG9EYXRhUG9ydE1hcCA9IG1vZHVsZVRvRGF0YVBvcnRNYXAuY29uY2F0KHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3IoKS5Nb2R1bGVTZXQoKS5maWx0ZXIob25lRGFzTW9kdWxlID0+IG9uZURhc01vZHVsZS5EYXRhRGlyZWN0aW9uKCkgPT09IERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dCkubWFwKG9uZURhc01vZHVsZSA9PlxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgbGV0IGdyb3VwOiBPYnNlcnZhYmxlR3JvdXA8RGF0YVBvcnRWaWV3TW9kZWw+XHJcblxyXG4gICAgICAgICAgICBncm91cCA9IG5ldyBPYnNlcnZhYmxlR3JvdXA8RGF0YVBvcnRWaWV3TW9kZWw+KG9uZURhc01vZHVsZS5Ub1N0cmluZygpLCB0aGlzLkNyZWF0ZURhdGFQb3J0U2V0KG9uZURhc01vZHVsZSwgaW5kZXgpKVxyXG4gICAgICAgICAgICBpbmRleCArPSBvbmVEYXNNb2R1bGUuU2l6ZSgpO1xyXG5cclxuICAgICAgICAgICAgcmV0dXJuIGdyb3VwXHJcbiAgICAgICAgfSkpXHJcblxyXG4gICAgICAgIHRoaXMuTW9kdWxlVG9EYXRhUG9ydE1hcChtb2R1bGVUb0RhdGFQb3J0TWFwKVxyXG4gICAgICAgIHRoaXMuRGF0YVBvcnRTZXQoTWFwTWFueSh0aGlzLk1vZHVsZVRvRGF0YVBvcnRNYXAoKSwgZ3JvdXAgPT4gZ3JvdXAuTWVtYmVycygpKSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgQ3JlYXRlRGF0YVBvcnRTZXQob25lRGFzTW9kdWxlOiBPbmVEYXNNb2R1bGVWaWV3TW9kZWwsIGluZGV4OiBudW1iZXIpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IHByZWZpeDogc3RyaW5nXHJcblxyXG4gICAgICAgIHN3aXRjaCAob25lRGFzTW9kdWxlLkRhdGFEaXJlY3Rpb24oKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uSW5wdXQ6XHJcbiAgICAgICAgICAgICAgICBwcmVmaXggPSBcIklucHV0XCJcclxuICAgICAgICAgICAgICAgIGJyZWFrXHJcblxyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dDpcclxuICAgICAgICAgICAgICAgIHByZWZpeCA9IFwiT3V0cHV0XCJcclxuICAgICAgICAgICAgICAgIGJyZWFrXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gQXJyYXkuZnJvbShuZXcgQXJyYXkob25lRGFzTW9kdWxlLlNpemUoKSksICh4LCBpKSA9PiBcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHJldHVybiB7XHJcbiAgICAgICAgICAgICAgICBOYW1lOiA8c3RyaW5nPnByZWZpeCArIFwiIFwiICsgKGluZGV4ICsgaSksXHJcbiAgICAgICAgICAgICAgICBEYXRhVHlwZTogPE9uZURhc0RhdGFUeXBlRW51bT5vbmVEYXNNb2R1bGUuRGF0YVR5cGUoKSxcclxuICAgICAgICAgICAgICAgIERhdGFEaXJlY3Rpb246IDxEYXRhRGlyZWN0aW9uRW51bT5vbmVEYXNNb2R1bGUuRGF0YURpcmVjdGlvbigpXHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9KS5tYXAoZGF0YVBvcnRNb2RlbCA9PiBuZXcgRGF0YVBvcnRWaWV3TW9kZWwoZGF0YVBvcnRNb2RlbCwgdGhpcykpXHJcbiAgICB9XHJcbn0iLCIvLy8gPHJlZmVyZW5jZSBwYXRoPVwiUGx1Z2luVmlld01vZGVsQmFzZS50c1wiLz5cclxuXHJcbmFic3RyYWN0IGNsYXNzIERhdGFXcml0ZXJWaWV3TW9kZWxCYXNlIGV4dGVuZHMgUGx1Z2luVmlld01vZGVsQmFzZVxyXG57XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRmlsZUdyYW51bGFyaXR5OiBLbm9ja291dE9ic2VydmFibGU8RmlsZUdyYW51bGFyaXR5RW51bT5cclxuICAgIHB1YmxpYyByZWFkb25seSBCdWZmZXJSZXF1ZXN0U2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxCdWZmZXJSZXF1ZXN0Vmlld01vZGVsPlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKG1vZGVsLCBpZGVudGlmaWNhdGlvbjogUGx1Z2luSWRlbnRpZmljYXRpb25WaWV3TW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgc3VwZXIobW9kZWwsIGlkZW50aWZpY2F0aW9uKVxyXG5cclxuICAgICAgICB0aGlzLkZpbGVHcmFudWxhcml0eSA9IGtvLm9ic2VydmFibGU8RmlsZUdyYW51bGFyaXR5RW51bT4obW9kZWwuRmlsZUdyYW51bGFyaXR5KVxyXG4gICAgICAgIHRoaXMuQnVmZmVyUmVxdWVzdFNldCA9IGtvLm9ic2VydmFibGVBcnJheTxCdWZmZXJSZXF1ZXN0Vmlld01vZGVsPihtb2RlbC5CdWZmZXJSZXF1ZXN0U2V0Lm1hcChidWZmZXJSZXF1ZXN0ID0+IG5ldyBCdWZmZXJSZXF1ZXN0Vmlld01vZGVsKGJ1ZmZlclJlcXVlc3QpKSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgRXh0ZW5kTW9kZWwobW9kZWw6IGFueSlcclxuICAgIHtcclxuICAgICAgICBzdXBlci5FeHRlbmRNb2RlbChtb2RlbClcclxuXHJcbiAgICAgICAgbW9kZWwuRmlsZUdyYW51bGFyaXR5ID0gPEZpbGVHcmFudWxhcml0eUVudW0+dGhpcy5GaWxlR3JhbnVsYXJpdHkoKVxyXG4gICAgICAgIG1vZGVsLkJ1ZmZlclJlcXVlc3RTZXQgPSA8QnVmZmVyUmVxdWVzdE1vZGVsW10+dGhpcy5CdWZmZXJSZXF1ZXN0U2V0KCkubWFwKGJ1ZmZlclJlcXVlc3QgPT4gYnVmZmVyUmVxdWVzdC5Ub01vZGVsKCkpXHJcbiAgICB9XHJcbn0iLCJjbGFzcyBQbHVnaW5EZXNjcmlwdGlvblZpZXdNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgUHJvZHVjdFZlcnNpb246IG51bWJlclxyXG4gICAgcHVibGljIElkOiBzdHJpbmdcclxuICAgIHB1YmxpYyBJbnN0YW5jZUlkOiBudW1iZXJcclxuICAgIHB1YmxpYyBJbnN0YW5jZU5hbWU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgSXNFbmFibGVkOiBLbm9ja291dE9ic2VydmFibGU8Ym9vbGVhbj5cclxuXHJcbiAgICBjb25zdHJ1Y3RvcihwbHVnaW5EZXNjcmlwdGlvbk1vZGVsOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5Qcm9kdWN0VmVyc2lvbiA9IHBsdWdpbkRlc2NyaXB0aW9uTW9kZWwuUHJvZHVjdFZlcnNpb25cclxuICAgICAgICB0aGlzLklkID0gcGx1Z2luRGVzY3JpcHRpb25Nb2RlbC5JZFxyXG4gICAgICAgIHRoaXMuSW5zdGFuY2VJZCA9IHBsdWdpbkRlc2NyaXB0aW9uTW9kZWwuSW5zdGFuY2VJZFxyXG4gICAgICAgIHRoaXMuSW5zdGFuY2VOYW1lID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KHBsdWdpbkRlc2NyaXB0aW9uTW9kZWwuSW5zdGFuY2VOYW1lKVxyXG4gICAgICAgIHRoaXMuSXNFbmFibGVkID0ga28ub2JzZXJ2YWJsZTxib29sZWFuPihwbHVnaW5EZXNjcmlwdGlvbk1vZGVsLklzRW5hYmxlZClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpXHJcbiAgICB7XHJcbiAgICAgICAgdmFyIG1vZGVsOiBhbnkgPSB7XHJcbiAgICAgICAgICAgIFByb2R1Y3RWZXJzaW9uOiA8bnVtYmVyPnRoaXMuUHJvZHVjdFZlcnNpb24sXHJcbiAgICAgICAgICAgIElkOiA8c3RyaW5nPnRoaXMuSWQsXHJcbiAgICAgICAgICAgIEluc3RhbmNlSWQ6IDxudW1iZXI+dGhpcy5JbnN0YW5jZUlkLFxyXG4gICAgICAgICAgICBJbnN0YW5jZU5hbWU6IDxzdHJpbmc+dGhpcy5JbnN0YW5jZU5hbWUoKSxcclxuICAgICAgICAgICAgSXNFbmFibGVkOiA8Ym9vbGVhbj50aGlzLklzRW5hYmxlZCgpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gbW9kZWxcclxuICAgIH1cclxufSIsImNsYXNzIFBsdWdpbklkZW50aWZpY2F0aW9uVmlld01vZGVsXHJcbntcclxuICAgIHB1YmxpYyBQcm9kdWN0VmVyc2lvbjogc3RyaW5nXHJcbiAgICBwdWJsaWMgSWQ6IHN0cmluZ1xyXG4gICAgcHVibGljIE5hbWU6IHN0cmluZ1xyXG4gICAgcHVibGljIERlc2NyaXB0aW9uOiBzdHJpbmdcclxuICAgIHB1YmxpYyBWaWV3UmVzb3VyY2VOYW1lOiBzdHJpbmdcclxuICAgIHB1YmxpYyBWaWV3TW9kZWxSZXNvdXJjZU5hbWU6IHN0cmluZ1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKHBsdWdpbklkZW50aWZpY2F0aW9uTW9kZWw6IGFueSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLlByb2R1Y3RWZXJzaW9uID0gcGx1Z2luSWRlbnRpZmljYXRpb25Nb2RlbC5Qcm9kdWN0VmVyc2lvblxyXG4gICAgICAgIHRoaXMuSWQgPSBwbHVnaW5JZGVudGlmaWNhdGlvbk1vZGVsLklkXHJcbiAgICAgICAgdGhpcy5OYW1lID0gcGx1Z2luSWRlbnRpZmljYXRpb25Nb2RlbC5OYW1lXHJcbiAgICAgICAgdGhpcy5EZXNjcmlwdGlvbiA9IHBsdWdpbklkZW50aWZpY2F0aW9uTW9kZWwuRGVzY3JpcHRpb25cclxuICAgICAgICB0aGlzLlZpZXdSZXNvdXJjZU5hbWUgPSBwbHVnaW5JZGVudGlmaWNhdGlvbk1vZGVsLlZpZXdSZXNvdXJjZU5hbWVcclxuICAgICAgICB0aGlzLlZpZXdNb2RlbFJlc291cmNlTmFtZSA9IHBsdWdpbklkZW50aWZpY2F0aW9uTW9kZWwuVmlld01vZGVsUmVzb3VyY2VOYW1lXHJcbiAgICB9XHJcbn0iXX0=