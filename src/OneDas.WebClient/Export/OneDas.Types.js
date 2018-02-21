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
class ChannelHubModel {
    constructor(name, group, dataType, sampleRate) {
        this.Name = name;
        this.Group = group;
        this.DataType = dataType;
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
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiT25lRGFzLlR5cGVzLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vT25lRGFzLlR5cGVzL0RhdGFEaXJlY3Rpb25FbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL0VuZGlhbm5lc3NFbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL0ZpbGVHcmFudWxhcml0eUVudW0udHMiLCIuLi9PbmVEYXMuVHlwZXMvT25lRGFzRGF0YVR5cGVFbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL09uZURhc1N0YXRlRW51bS50cyIsIi4uL09uZURhcy5UeXBlcy9TYW1wbGVSYXRlRW51bS50cyIsIi4uL0NvcmUvQWN0aW9uUmVxdWVzdC50cyIsIi4uL0NvcmUvQWN0aW9uUmVzcG9uc2UudHMiLCIuLi9Db3JlL0V2ZW50RGlzcGF0Y2hlci50cyIsIi4uL0NvcmUvSUV2ZW50LnRzIiwiLi4vQ29yZS9PbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtLnRzIiwiLi4vTW9kZWxzL0NoYW5uZWxIdWJNb2RlbC50cyIsIi4uL01vZGVscy9PbmVEYXNNb2R1bGVNb2RlbC50cyIsIi4uL01vZGVscy9UcmFuc2ZlckZ1bmN0aW9uTW9kZWwudHMiLCIuLi9TdGF0aWMvQ29ubmVjdGlvbk1hbmFnZXIudHMiLCIuLi9TdGF0aWMvRW51bWVyYXRpb25IZWxwZXIudHMiLCIuLi9TdGF0aWMvRXJyb3JNZXNzYWdlLnRzIiwiLi4vU3RhdGljL0hlbHBlci50cyIsIi4uL1N0YXRpYy9QbHVnaW5GYWN0b3J5LnRzIiwiLi4vU3RhdGljL1BsdWdpbkhpdmUudHMiLCIuLi9WaWV3TW9kZWxzL0NvcmUvQ2hhbm5lbEh1YlZpZXdNb2RlbC50cyIsIi4uL1ZpZXdNb2RlbHMvQ29yZS9PbmVEYXNNb2R1bGVWaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL0NvcmUvT25lRGFzTW9kdWxlU2VsZWN0b3JWaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL0NvcmUvVHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbC50cyIsIi4uL1ZpZXdNb2RlbHMvUGx1Z2luL0RhdGFQb3J0Vmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vUGx1Z2luVmlld01vZGVsQmFzZS50cyIsIi4uL1ZpZXdNb2RlbHMvUGx1Z2luL0RhdGFHYXRld2F5Vmlld01vZGVsQmFzZS50cyIsIi4uL1ZpZXdNb2RlbHMvUGx1Z2luL0V4dGVuZGVkRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vRGF0YVdyaXRlclZpZXdNb2RlbEJhc2UudHMiLCIuLi9WaWV3TW9kZWxzL1BsdWdpbi9QbHVnaW5EZXNjcmlwdGlvblZpZXdNb2RlbC50cyIsIi4uL1ZpZXdNb2RlbHMvUGx1Z2luL1BsdWdpbklkZW50aWZpY2F0aW9uVmlld01vZGVsLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLElBQUssaUJBSUo7QUFKRCxXQUFLLGlCQUFpQjtJQUVsQiwyREFBUyxDQUFBO0lBQ1QsNkRBQVUsQ0FBQTtBQUNkLENBQUMsRUFKSSxpQkFBaUIsS0FBakIsaUJBQWlCLFFBSXJCO0FDSkQsSUFBSyxjQUlKO0FBSkQsV0FBSyxjQUFjO0lBRWYsbUVBQWdCLENBQUE7SUFDaEIsNkRBQWEsQ0FBQTtBQUNqQixDQUFDLEVBSkksY0FBYyxLQUFkLGNBQWMsUUFJbEI7QUNKRCxJQUFLLG1CQU1KO0FBTkQsV0FBSyxtQkFBbUI7SUFFcEIsc0VBQWEsQ0FBQTtJQUNiLHlFQUFlLENBQUE7SUFDZixnRUFBVyxDQUFBO0lBQ1gsK0RBQVcsQ0FBQTtBQUNmLENBQUMsRUFOSSxtQkFBbUIsS0FBbkIsbUJBQW1CLFFBTXZCO0FDTkQsSUFBSyxrQkFXSjtBQVhELFdBQUssa0JBQWtCO0lBRW5CLGlFQUFlLENBQUE7SUFDZiwrREFBYSxDQUFBO0lBQ2IsNkRBQVksQ0FBQTtJQUNaLGlFQUFjLENBQUE7SUFDZCwrREFBYSxDQUFBO0lBQ2IsaUVBQWMsQ0FBQTtJQUNkLCtEQUFhLENBQUE7SUFDYixtRUFBZSxDQUFBO0lB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QUFDLFVBQVUsQ0FBVSxzQkFBc0IsQ0FBQyxTQUFTLENBQUMsQ0FBQTtJQUM3RSxDQUFDO0lBRU0sT0FBTztRQUVWLElBQUksS0FBSyxHQUFRO1lBQ2IsY0FBYyxFQUFVLElBQUksQ0FBQyxjQUFjO1lBQzNDLEVBQUUsRUFBVSxJQUFJLENBQUMsRUFBRTtZQUNuQixVQUFVLEVBQVUsSUFBSSxDQUFDLFVBQVU7WUFDbkMsU0FBUyxFQUFXLElBQUksQ0FBQyxTQUFTLEVBQUU7U0FDdkMsQ0FBQTtRQUVELE1BQU0sQ0FBQyxLQUFLLENBQUE7SUFDaEIsQ0FBQztDQUNKO0FDMUJEO0lBU0ksWUFBWSx5QkFBOEI7UUFFdEMsSUFBSSxDQUFDLGNBQWMsR0FBRyx5QkFBeUIsQ0FBQyxjQUFjLENBQUE7UUFDOUQsSUFBSSxDQUFDLEVBQUUsR0FBRyx5QkFBeUIsQ0FBQyxFQUFFLENBQUE7UUFDdEMsSUFBSSxDQUFDLElBQUksR0FBRyx5QkFBeUIsQ0FBQyxJQUFJLENBQUE7UUFDMUMsSUFBSSxDQUFDLFdBQVcsR0FBRyx5QkFBeUIsQ0FBQyxXQUFXLENBQUE7UUFDeEQsSUFBSSxDQUFDLGdCQUFnQixHQUFHLHlCQUF5QixDQUFDLGdCQUFnQixDQUFBO1FBQ2xFLElBQUksQ0FBQyxxQkFBcUIsR0FBRyx5QkFBeUIsQ0FBQyxxQkFBcUIsQ0FBQTtJQUNoRixDQUFDO0NBQ0oiLCJzb3VyY2VzQ29udGVudCI6WyJlbnVtIERhdGFEaXJlY3Rpb25FbnVtXHJcbntcclxuICAgIElucHV0ID0gMSxcclxuICAgIE91dHB1dCA9IDJcclxufSIsImVudW0gRW5kaWFubmVzc0VudW1cclxue1xyXG4gICAgTGl0dGxlRW5kaWFuID0gMSxcclxuICAgIEJpZ0VuZGlhbiA9IDJcclxufSIsImVudW0gRmlsZUdyYW51bGFyaXR5RW51bVxyXG57XHJcbiAgICBNaW51dGVfMSA9IDYwLFxyXG4gICAgTWludXRlXzEwID0gNjAwLFxyXG4gICAgSG91ciA9IDM2MDAsXHJcbiAgICBEYXkgPSA4NjQwMFxyXG59IiwiZW51bSBPbmVEYXNEYXRhVHlwZUVudW1cclxue1xyXG4gICAgQk9PTEVBTiA9IDB4MDA4LFxyXG4gICAgVUlOVDggPSAweDEwOCxcclxuICAgIElOVDggPSAweDIwOCxcclxuICAgIFVJTlQxNiA9IDB4MTEwLFxyXG4gICAgSU5UMTYgPSAweDIxMCxcclxuICAgIFVJTlQzMiA9IDB4MTIwLFxyXG4gICAgSU5UMzIgPSAweDIyMCxcclxuICAgIEZMT0FUMzIgPSAweDMyMCxcclxuICAgIEZMT0FUNjQgPSAweDM0MFxyXG59IiwiZW51bSBPbmVEYXNTdGF0ZUVudW1cclxue1xyXG4gICAgRXJyb3IgPSAxLFxyXG4gICAgSW5pdGlhbGl6YXRpb24gPSAyLFxyXG4gICAgVW5jb25maWd1cmVkID0gMyxcclxuICAgIEFwcGx5Q29uZmlndXJhdGlvbiA9IDUsXHJcbiAgICBSZWFkeSA9IDYsXHJcbiAgICBSdW4gPSA3XHJcbn0iLCJlbnVtIFNhbXBsZVJhdGVFbnVtXHJcbntcclxuICAgIFNhbXBsZVJhdGVfMTAwID0gMSxcclxuICAgIFNhbXBsZVJhdGVfMjUgPSA0LFxyXG4gICAgU2FtcGxlUmF0ZV81ID0gMjAsXHJcbiAgICBTYW1wbGVSYXRlXzEgPSAxMDBcclxufSIsImNsYXNzIEFjdGlvblJlcXVlc3Rcclxue1xyXG4gICAgcHVibGljIHJlYWRvbmx5IFBsdWdpbklkOiBzdHJpbmdcclxuICAgIHB1YmxpYyByZWFkb25seSBJbnN0YW5jZUlkOiBudW1iZXJcclxuICAgIHB1YmxpYyByZWFkb25seSBNZXRob2ROYW1lOiBzdHJpbmdcclxuICAgIHB1YmxpYyByZWFkb25seSBEYXRhOiBhbnlcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihwbHVnaW5JZDogc3RyaW5nLCBpbnN0YW5jZUlkOiBudW1iZXIsIG1ldGhvZE5hbWU6IHN0cmluZywgZGF0YTogYW55KVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuUGx1Z2luSWQgPSBwbHVnaW5JZDtcclxuICAgICAgICB0aGlzLkluc3RhbmNlSWQgPSBpbnN0YW5jZUlkO1xyXG4gICAgICAgIHRoaXMuTWV0aG9kTmFtZSA9IG1ldGhvZE5hbWU7XHJcbiAgICAgICAgdGhpcy5EYXRhID0gZGF0YTtcclxuICAgIH1cclxufSIsImNsYXNzIEFjdGlvblJlc3BvbnNlXHJcbntcclxuICAgIHB1YmxpYyBEYXRhOiBhbnlcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihkYXRhOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5EYXRhID0gZGF0YTtcclxuICAgIH1cclxufSIsImNsYXNzIEV2ZW50RGlzcGF0Y2hlcjxUU2VuZGVyLCBUQXJncz4gaW1wbGVtZW50cyBJRXZlbnQ8VFNlbmRlciwgVEFyZ3M+XHJcbntcclxuICAgIHByaXZhdGUgX3N1YnNjcmlwdGlvbnM6IEFycmF5PChzZW5kZXI6IFRTZW5kZXIsIGFyZ3M6IFRBcmdzKSA9PiB2b2lkPiA9IG5ldyBBcnJheTwoc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncykgPT4gdm9pZD4oKTtcclxuXHJcbiAgICBzdWJzY3JpYmUoZm46IChzZW5kZXI6IFRTZW5kZXIsIGFyZ3M6IFRBcmdzKSA9PiB2b2lkKTogdm9pZFxyXG4gICAge1xyXG4gICAgICAgIGlmIChmbilcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuX3N1YnNjcmlwdGlvbnMucHVzaChmbik7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHVuc3Vic2NyaWJlKGZuOiAoc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncykgPT4gdm9pZCk6IHZvaWRcclxuICAgIHtcclxuICAgICAgICBsZXQgaSA9IHRoaXMuX3N1YnNjcmlwdGlvbnMuaW5kZXhPZihmbik7XHJcblxyXG4gICAgICAgIGlmIChpID4gLTEpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLl9zdWJzY3JpcHRpb25zLnNwbGljZShpLCAxKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgZGlzcGF0Y2goc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncyk6IHZvaWRcclxuICAgIHtcclxuICAgICAgICBmb3IgKGxldCBoYW5kbGVyIG9mIHRoaXMuX3N1YnNjcmlwdGlvbnMpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBoYW5kbGVyKHNlbmRlciwgYXJncyk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG59IiwiaW50ZXJmYWNlIElFdmVudDxUU2VuZGVyLCBUQXJncz5cclxue1xyXG4gICAgc3Vic2NyaWJlKGZuOiAoc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncykgPT4gdm9pZCk6IHZvaWQ7XHJcbiAgICB1bnN1YnNjcmliZShmbjogKHNlbmRlcjogVFNlbmRlciwgYXJnczogVEFyZ3MpID0+IHZvaWQpOiB2b2lkO1xyXG59IiwiZW51bSBPbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtXHJcbntcclxuICAgIER1cGxleCA9IDEsXHJcbiAgICBJbnB1dE9ubHkgPSAyLFxyXG4gICAgT3V0cHV0T25seSA9IDMsXHJcbn0iLCJjbGFzcyBDaGFubmVsSHViTW9kZWxcclxue1xyXG4gICAgcHVibGljIE5hbWU6IHN0cmluZ1xyXG4gICAgcHVibGljIEdyb3VwOiBzdHJpbmdcclxuICAgIHB1YmxpYyBEYXRhVHlwZTogT25lRGFzRGF0YVR5cGVFbnVtXHJcbiAgICBwdWJsaWMgU2FtcGxlUmF0ZTogU2FtcGxlUmF0ZUVudW1cclxuICAgIHB1YmxpYyBHdWlkOiBzdHJpbmdcclxuICAgIHB1YmxpYyBDcmVhdGlvbkRhdGVUaW1lOiBzdHJpbmdcclxuICAgIHB1YmxpYyBVbml0OiBzdHJpbmdcclxuICAgIHB1YmxpYyBUcmFuc2ZlckZ1bmN0aW9uU2V0OiBhbnlbXVxyXG5cclxuICAgIHB1YmxpYyBTZXJpYWxpemVyRGF0YUlucHV0SWQ6IHN0cmluZ1xyXG4gICAgcHVibGljIFNlcmlhbGl6ZXJEYXRhT3V0cHV0SWRTZXQ6IHN0cmluZ1tdXHJcblxyXG4gICAgY29uc3RydWN0b3IobmFtZTogc3RyaW5nLCBncm91cDogc3RyaW5nLCBkYXRhVHlwZTogT25lRGFzRGF0YVR5cGVFbnVtLCBzYW1wbGVSYXRlOiBTYW1wbGVSYXRlRW51bSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLk5hbWUgPSBuYW1lO1xyXG4gICAgICAgIHRoaXMuR3JvdXAgPSBncm91cDtcclxuICAgICAgICB0aGlzLkRhdGFUeXBlID0gZGF0YVR5cGU7XHJcbiAgICAgICAgdGhpcy5TYW1wbGVSYXRlID0gc2FtcGxlUmF0ZTtcclxuICAgICAgICB0aGlzLkd1aWQgPSBHdWlkLk5ld0d1aWQoKVxyXG4gICAgICAgIHRoaXMuQ3JlYXRpb25EYXRlVGltZSA9IG5ldyBEYXRlKCkudG9JU09TdHJpbmcoKVxyXG4gICAgICAgIHRoaXMuVW5pdCA9IFwiXCJcclxuICAgICAgICB0aGlzLlRyYW5zZmVyRnVuY3Rpb25TZXQgPSBbXVxyXG5cclxuICAgICAgICB0aGlzLlNlcmlhbGl6ZXJEYXRhSW5wdXRJZCA9IFwiXCJcclxuICAgICAgICB0aGlzLlNlcmlhbGl6ZXJEYXRhT3V0cHV0SWRTZXQgPSBbXVxyXG4gICAgfVxyXG59IiwiY2xhc3MgT25lRGFzTW9kdWxlTW9kZWxcclxue1xyXG4gICAgcHVibGljIERhdGFUeXBlOiBPbmVEYXNEYXRhVHlwZUVudW1cclxuICAgIHB1YmxpYyBEYXRhRGlyZWN0aW9uOiBEYXRhRGlyZWN0aW9uRW51bVxyXG4gICAgcHVibGljIEVuZGlhbm5lc3M6IEVuZGlhbm5lc3NFbnVtXHJcbiAgICBwdWJsaWMgU2l6ZTogbnVtYmVyXHJcblxyXG4gICAgY29uc3RydWN0b3IoZGF0YVR5cGU6IE9uZURhc0RhdGFUeXBlRW51bSwgZGF0YURpcmVjdGlvbjogRGF0YURpcmVjdGlvbkVudW0sIGVuZGlhbm5lc3M6IEVuZGlhbm5lc3NFbnVtLCBzaXplOiBudW1iZXIpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZSA9IGRhdGFUeXBlXHJcbiAgICAgICAgdGhpcy5EYXRhRGlyZWN0aW9uID0gZGF0YURpcmVjdGlvblxyXG4gICAgICAgIHRoaXMuRW5kaWFubmVzcyA9IGVuZGlhbm5lc3NcclxuICAgICAgICB0aGlzLlNpemUgPSBzaXplXHJcbiAgICB9XHJcbn1cclxuIiwiY2xhc3MgVHJhbnNmZXJGdW5jdGlvbk1vZGVsXHJcbntcclxuICAgIHB1YmxpYyBEYXRlVGltZTogc3RyaW5nXHJcbiAgICBwdWJsaWMgVHlwZTogc3RyaW5nXHJcbiAgICBwdWJsaWMgT3B0aW9uOiBzdHJpbmdcclxuICAgIHB1YmxpYyBBcmd1bWVudDogc3RyaW5nXHJcblxyXG4gICAgY29uc3RydWN0b3IoZGF0ZVRpbWU6IHN0cmluZywgdHlwZTogc3RyaW5nLCBvcHRpb246IHN0cmluZywgYXJndW1lbnQ6IHN0cmluZylcclxuICAgIHtcclxuICAgICAgICB0aGlzLkRhdGVUaW1lID0gZGF0ZVRpbWVcclxuICAgICAgICB0aGlzLlR5cGUgPSB0eXBlXHJcbiAgICAgICAgdGhpcy5PcHRpb24gPSBvcHRpb25cclxuICAgICAgICB0aGlzLkFyZ3VtZW50ID0gYXJndW1lbnRcclxuICAgIH1cclxufSIsImRlY2xhcmUgdmFyIHNpZ25hbFI6IGFueVxyXG5cclxuY2xhc3MgQ29ubmVjdGlvbk1hbmFnZXJcclxue1xyXG4gICAgcHVibGljIHN0YXRpYyBXZWJDbGllbnRIdWI6IGFueSAvLyBpbXByb3ZlOiB1c2UgdHlwaW5nc1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgSW5pdGlhbGl6ZShlbmFibGVMb2dnaW5nOiBib29sZWFuKVxyXG4gICAge1xyXG4gICAgICAgIENvbm5lY3Rpb25NYW5hZ2VyLldlYkNsaWVudEh1YiA9IG5ldyBzaWduYWxSLkh1YkNvbm5lY3Rpb24oJy93ZWJjbGllbnRodWInKTtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIEludm9rZVdlYkNsaWVudEh1YiA9IGFzeW5jKG1ldGhvZE5hbWU6IHN0cmluZywgLi4uYXJnczogYW55W10pID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIGF3YWl0IFByb21pc2UucmVzb2x2ZShDb25uZWN0aW9uTWFuYWdlci5XZWJDbGllbnRIdWIuaW52b2tlKG1ldGhvZE5hbWUsIC4uLmFyZ3MpKVxyXG4gICAgfVxyXG59XHJcbiIsImNsYXNzIEVudW1lcmF0aW9uSGVscGVyXHJcbntcclxuICAgIHB1YmxpYyBzdGF0aWMgRGVzY3JpcHRpb246IHsgW2luZGV4OiBzdHJpbmddOiBzdHJpbmcgfSA9IHt9XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBHZXRFbnVtTG9jYWxpemF0aW9uID0gKHR5cGVOYW1lOiBzdHJpbmcsIHZhbHVlKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHZhciBrZXk6IHN0cmluZyA9IGV2YWwodHlwZU5hbWUgKyBcIltcIiArIHZhbHVlICsgXCJdXCIpXHJcbiAgICAgICAgcmV0dXJuIGV2YWwoXCJFbnVtZXJhdGlvbkhlbHBlci5EZXNjcmlwdGlvblsnXCIgKyB0eXBlTmFtZSArIFwiX1wiICsga2V5ICsgXCInXVwiKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgR2V0RW51bVZhbHVlcyA9ICh0eXBlTmFtZTogc3RyaW5nKSA9PlxyXG4gICAge1xyXG4gICAgICAgIGxldCB2YWx1ZXM6IGFueVtdXHJcblxyXG4gICAgICAgIHZhbHVlcyA9IGV2YWwoXCJPYmplY3Qua2V5cyhcIiArIHR5cGVOYW1lICsgXCIpLm1hcChrZXkgPT4gXCIgKyB0eXBlTmFtZSArIFwiW2tleV0pXCIpXHJcbiAgICAgICAgcmV0dXJuIDxudW1iZXJbXT52YWx1ZXMuZmlsdGVyKHZhbHVlID0+IHR5cGVvZiAodmFsdWUpID09PSBcIm51bWJlclwiKVxyXG4gICAgfVxyXG59IiwibGV0IEVycm9yTWVzc2FnZTogeyBbaW5kZXg6IHN0cmluZ106IHN0cmluZyB9ID0ge31cclxuRXJyb3JNZXNzYWdlW1wiTXVsdGlNYXBwaW5nRWRpdG9yVmlld01vZGVsX0ludmFsaWRTZXR0aW5nc1wiXSA9IFwiT25lIG9yIG1vcmUgc2V0dGluZ3MgYXJlIGludmFsaWQuXCJcclxuRXJyb3JNZXNzYWdlW1wiTXVsdGlNYXBwaW5nRWRpdG9yVmlld01vZGVsX1dyb25nRGF0YVR5cGVcIl0gPSBcIk9uZSBvciBtb3JlIHZhcmlhYmxlLWNoYW5uZWwgZGF0YSB0eXBlIGNvbWJpbmF0aW9ucyBhcmUgaW52YWxpZC5cIlxyXG5FcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0NoYW5uZWxBbHJlYWR5RXhpc3RzXCJdID0gXCJBIGNoYW5uZWwgd2l0aCB0aGF0IG5hbWUgYWxyZWFkeSBleGlzdHMuXCJcclxuRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9Jc0FscmVhZHlJbkdyb3VwXCJdID0gXCJUaGUgY2hhbm5lbCBpcyBhbHJlYWR5IGEgbWVtYmVyIG9mIHRoaXMgZ3JvdXAuXCJcclxuRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9JbnZhbGlkQ2hhcmFjdGVyc1wiXSA9IFwiVXNlIEEtWiwgYS16LCAwLTkgb3IgXy5cIlxyXG5FcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0ludmFsaWRMZWFkaW5nQ2hhcmFjdGVyXCJdID0gXCJVc2UgQS1aIG9yIGEteiBhcyBmaXJzdCBjaGFyYWN0ZXIuXCJcclxuRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9OYW1lRW1wdHlcIl0gPSBcIlRoZSBuYW1lIG11c3Qgbm90IGJlIGVtcHR5LlwiXHJcbiIsImNsYXNzIE9ic2VydmFibGVHcm91cDxUPlxyXG57XHJcbiAgICBLZXk6IHN0cmluZztcclxuICAgIE1lbWJlcnM6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PFQ+XHJcblxyXG4gICAgY29uc3RydWN0b3Ioa2V5OiBzdHJpbmcsIG1lbWJlcnM6IFRbXSA9IG5ldyBBcnJheTxUPigpKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuS2V5ID0ga2V5XHJcbiAgICAgICAgdGhpcy5NZW1iZXJzID0ga28ub2JzZXJ2YWJsZUFycmF5KG1lbWJlcnMpXHJcbiAgICB9XHJcbn1cclxuXHJcbmZ1bmN0aW9uIE9ic2VydmFibGVHcm91cEJ5PFQ+KGxpc3Q6IFRbXSwgbmFtZUdldHRlcjogKHg6IFQpID0+IHN0cmluZywgZ3JvdXBOYW1lR2V0dGVyOiAoeDogVCkgPT4gc3RyaW5nLCBmaWx0ZXI6IHN0cmluZyk6IE9ic2VydmFibGVHcm91cDxUPltdXHJcbntcclxuICAgIGxldCByZXN1bHQ6IE9ic2VydmFibGVHcm91cDxUPltdXHJcbiAgICBsZXQgcmVnRXhwOiBSZWdFeHBcclxuXHJcbiAgICByZXN1bHQgPSBbXVxyXG4gICAgcmVnRXhwID0gbmV3IFJlZ0V4cChmaWx0ZXIsIFwiaVwiKVxyXG5cclxuICAgIGxpc3QuZm9yRWFjaChlbGVtZW50ID0+XHJcbiAgICB7XHJcbiAgICAgICAgaWYgKHJlZ0V4cC50ZXN0KG5hbWVHZXR0ZXIoZWxlbWVudCkpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgZ3JvdXBOYW1lR2V0dGVyKGVsZW1lbnQpLnNwbGl0KFwiXFxuXCIpLmZvckVhY2goZ3JvdXBOYW1lID0+XHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIEFkZFRvR3JvdXBlZEFycmF5KGVsZW1lbnQsIGdyb3VwTmFtZSwgcmVzdWx0KVxyXG4gICAgICAgICAgICB9KVxyXG4gICAgICAgIH1cclxuICAgIH0pXHJcblxyXG4gICAgcmV0dXJuIHJlc3VsdFxyXG59XHJcblxyXG5mdW5jdGlvbiBBZGRUb0dyb3VwZWRBcnJheTxUPihpdGVtOiBULCBncm91cE5hbWU6IHN0cmluZywgb2JzZXJ2YWJsZUdyb3VwU2V0OiBPYnNlcnZhYmxlR3JvdXA8VD5bXSlcclxue1xyXG4gICAgbGV0IGdyb3VwOiBPYnNlcnZhYmxlR3JvdXA8VD5cclxuXHJcbiAgICBncm91cCA9IG9ic2VydmFibGVHcm91cFNldC5maW5kKHkgPT4geS5LZXkgPT09IGdyb3VwTmFtZSlcclxuXHJcbiAgICBpZiAoIWdyb3VwKVxyXG4gICAge1xyXG4gICAgICAgIGdyb3VwID0gbmV3IE9ic2VydmFibGVHcm91cDxUPihncm91cE5hbWUpXHJcbiAgICAgICAgb2JzZXJ2YWJsZUdyb3VwU2V0LnB1c2goZ3JvdXApXHJcbiAgICB9XHJcblxyXG4gICAgZ3JvdXAuTWVtYmVycy5wdXNoKGl0ZW0pXHJcbn1cclxuXHJcbi8vZnVuY3Rpb24gQWRkVG9Hcm91cGVkT2JzZXJ2YWJsZUFycmF5PFQ+KGl0ZW06IFQsIGdyb3VwTmFtZTogc3RyaW5nLCBvYnNlcnZhYmxlR3JvdXBTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PE9ic2VydmFibGVHcm91cDxUPj4pXHJcbi8ve1xyXG4vLyAgICBsZXQgZ3JvdXA6IE9ic2VydmFibGVHcm91cDxUPlxyXG5cclxuLy8gICAgZ3JvdXAgPSBvYnNlcnZhYmxlR3JvdXBTZXQoKS5maW5kKHkgPT4geS5LZXkgPT09IGdyb3VwTmFtZSlcclxuXHJcbi8vICAgIGlmICghZ3JvdXApXHJcbi8vICAgIHtcclxuLy8gICAgICAgIGdyb3VwID0gbmV3IE9ic2VydmFibGVHcm91cDxUPihncm91cE5hbWUpXHJcbi8vICAgICAgICBvYnNlcnZhYmxlR3JvdXBTZXQucHVzaChncm91cClcclxuLy8gICAgfVxyXG5cclxuLy8gICAgZ3JvdXAuTWVtYmVycy5wdXNoKGl0ZW0pXHJcbi8vfVxyXG5cclxuLy9mdW5jdGlvbiBSZW1vdmVGcm9tR3JvdXBlZE9ic2VydmFibGVBcnJheTxUPihpdGVtOiBULCBvYnNlcnZhYmxlR3JvdXBTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PE9ic2VydmFibGVHcm91cDxUPj4pXHJcbi8ve1xyXG4vLyAgICB2YXIgZ3JvdXA6IE9ic2VydmFibGVHcm91cDxUPlxyXG5cclxuLy8gICAgb2JzZXJ2YWJsZUdyb3VwU2V0KCkuc29tZSh4ID0+XHJcbi8vICAgIHtcclxuLy8gICAgICAgIGlmICh4Lk1lbWJlcnMoKS5pbmRleE9mKGl0ZW0pID4gLTEpXHJcbi8vICAgICAgICB7XHJcbi8vICAgICAgICAgICAgZ3JvdXAgPSB4XHJcblxyXG4vLyAgICAgICAgICAgIHJldHVybiB0cnVlXHJcbi8vICAgICAgICB9XHJcblxyXG4vLyAgICAgICAgcmV0dXJuIGZhbHNlXHJcbi8vICAgIH0pXHJcblxyXG4vLyAgICBpZiAoZ3JvdXApXHJcbi8vICAgIHtcclxuLy8gICAgICAgIGdyb3VwLk1lbWJlcnMucmVtb3ZlKGl0ZW0pXHJcblxyXG4vLyAgICAgICAgaWYgKGdyb3VwLk1lbWJlcnMoKS5sZW5ndGggPT09IDApXHJcbi8vICAgICAgICB7XHJcbi8vICAgICAgICAgICAgb2JzZXJ2YWJsZUdyb3VwU2V0LnJlbW92ZShncm91cClcclxuLy8gICAgICAgIH1cclxuXHJcbi8vICAgICAgICByZXR1cm4gdHJ1ZVxyXG4vLyAgICB9XHJcblxyXG4vLyAgICByZXR1cm4gZmFsc2VcclxuLy99XHJcblxyXG4vL2Z1bmN0aW9uIFVwZGF0ZUdyb3VwZWRPYnNlcnZhYmxlQXJyYXk8VD4oaXRlbTogVCwgZ3JvdXBOYW1lOiBzdHJpbmcsIG9ic2VydmFibGVHcm91cFNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8T2JzZXJ2YWJsZUdyb3VwPFQ+PilcclxuLy97XHJcbi8vICAgIFJlbW92ZUZyb21Hcm91cGVkT2JzZXJ2YWJsZUFycmF5KGl0ZW0sIG9ic2VydmFibGVHcm91cFNldClcclxuLy8gICAgQWRkVG9Hcm91cGVkT2JzZXJ2YWJsZUFycmF5KGl0ZW0sIGdyb3VwTmFtZSwgb2JzZXJ2YWJsZUdyb3VwU2V0KVxyXG4vL31cclxuXHJcbmZ1bmN0aW9uIE1hcE1hbnk8VEFycmF5RWxlbWVudCwgVFNlbGVjdD4oYXJyYXk6IFRBcnJheUVsZW1lbnRbXSwgbWFwRnVuYzogKGl0ZW06IFRBcnJheUVsZW1lbnQpID0+IFRTZWxlY3RbXSk6IFRTZWxlY3RbXVxyXG57XHJcbiAgICByZXR1cm4gYXJyYXkucmVkdWNlKChwcmV2aW91cywgY3VycmVudCwgaSkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gcHJldmlvdXMuY29uY2F0KG1hcEZ1bmMoY3VycmVudCkpO1xyXG4gICAgfSwgPFRTZWxlY3RbXT5bXSk7XHJcbn1cclxuXHJcbmNsYXNzIEd1aWRcclxue1xyXG4gICAgc3RhdGljIE5ld0d1aWQoKVxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiAneHh4eHh4eHgteHh4eC00eHh4LXl4eHgteHh4eHh4eHh4eHh4Jy5yZXBsYWNlKC9beHldL2csIGZ1bmN0aW9uIChjKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdmFyIHIgPSBNYXRoLnJhbmRvbSgpICogMTYgfCAwXHJcbiAgICAgICAgICAgIHZhciB2ID0gYyA9PT0gJ3gnID8gciA6IChyICYgMHgzIHwgMHg4KVxyXG5cclxuICAgICAgICAgICAgcmV0dXJuIHYudG9TdHJpbmcoMTYpXHJcbiAgICAgICAgfSlcclxuICAgIH1cclxufVxyXG5cclxubGV0IENoZWNrTmFtaW5nQ29udmVudGlvbiA9ICh2YWx1ZTogc3RyaW5nKSA9PlxyXG57XHJcbiAgICB2YXIgcmVnRXhwOiBhbnlcclxuXHJcbiAgICBpZiAodmFsdWUubGVuZ3RoID09PSAwKVxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB7IEhhc0Vycm9yOiB0cnVlLCBFcnJvckRlc2NyaXB0aW9uOiBFcnJvck1lc3NhZ2VbXCJQcm9qZWN0X05hbWVFbXB0eVwiXSB9XHJcbiAgICB9XHJcblxyXG4gICAgcmVnRXhwID0gbmV3IFJlZ0V4cChcIlteQS1aYS16MC05X11cIilcclxuXHJcbiAgICBpZiAocmVnRXhwLnRlc3QodmFsdWUpKVxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB7IEhhc0Vycm9yOiB0cnVlLCBFcnJvckRlc2NyaXB0aW9uOiBFcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0ludmFsaWRDaGFyYWN0ZXJzXCJdIH1cclxuICAgIH1cclxuXHJcbiAgICByZWdFeHAgPSBuZXcgUmVnRXhwKFwiXlswLTlfXVwiKVxyXG5cclxuICAgIGlmIChyZWdFeHAudGVzdCh2YWx1ZSkpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHsgSGFzRXJyb3I6IHRydWUsIEVycm9yRGVzY3JpcHRpb246IEVycm9yTWVzc2FnZVtcIlByb2plY3RfSW52YWxpZExlYWRpbmdDaGFyYWN0ZXJcIl0gfVxyXG4gICAgfVxyXG5cclxuICAgIHJldHVybiB7XHJcbiAgICAgICAgSGFzRXJyb3I6IGZhbHNlLFxyXG4gICAgICAgIEVycm9yRGVzY3JpcHRpb246IFwiXCJcclxuICAgIH1cclxufSIsImNsYXNzIFBsdWdpbkZhY3Rvcnlcclxue1xyXG4gICAgcHVibGljIHN0YXRpYyBDcmVhdGVQbHVnaW5WaWV3TW9kZWxBc3luYyA9IGFzeW5jIChwbHVnaW5UeXBlOiBzdHJpbmcsIHBsdWdpbk1vZGVsOiBhbnkpID0+XHJcbiAgICB7XHJcbiAgICAgICAgbGV0IHBsdWdpbklkZW50aWZpY2F0aW9uOiBQbHVnaW5JZGVudGlmaWNhdGlvblZpZXdNb2RlbFxyXG4gICAgICAgIGxldCBwbHVnaW5WaWV3TW9kZWw6IFBsdWdpblZpZXdNb2RlbEJhc2VcclxuICAgICAgICBsZXQgcGx1Z2luVmlld01vZGVsUmF3OiBzdHJpbmdcclxuXHJcbiAgICAgICAgcGx1Z2luSWRlbnRpZmljYXRpb24gPSBQbHVnaW5IaXZlLkZpbmRQbHVnaW5JZGVudGlmaWNhdGlvbihwbHVnaW5UeXBlLCBwbHVnaW5Nb2RlbC5EZXNjcmlwdGlvbi5JZClcclxuXHJcbiAgICAgICAgaWYgKHBsdWdpbklkZW50aWZpY2F0aW9uKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgcGx1Z2luVmlld01vZGVsUmF3ID0gYXdhaXQgQ29ubmVjdGlvbk1hbmFnZXIuSW52b2tlV2ViQ2xpZW50SHViKFwiR2V0UGx1Z2luU3RyaW5nUmVzb3VyY2VcIiwgcGx1Z2luTW9kZWwuRGVzY3JpcHRpb24uSWQsIHBsdWdpbklkZW50aWZpY2F0aW9uLlZpZXdNb2RlbFJlc291cmNlTmFtZSlcclxuICAgICAgICAgICAgcGx1Z2luVmlld01vZGVsID0gPFBsdWdpblZpZXdNb2RlbEJhc2U+bmV3IEZ1bmN0aW9uKHBsdWdpblZpZXdNb2RlbFJhdyArIFwiOyByZXR1cm4gVmlld01vZGVsQ29uc3RydWN0b3JcIikoKShwbHVnaW5Nb2RlbCwgcGx1Z2luSWRlbnRpZmljYXRpb24pXHJcblxyXG4gICAgICAgICAgICByZXR1cm4gcGx1Z2luVmlld01vZGVsXHJcbiAgICAgICAgfVxyXG4gICAgICAgIGVsc2VcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRocm93IG5ldyBFcnJvcihcIk5vIGNvcnJlc3BvbmRpbmcgcGx1Z2luIGRlc2NyaXB0aW9uIGZvdW5kLlwiKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxufSIsImNsYXNzIFBsdWdpbkhpdmVcclxue1xyXG4gICAgLy8gZmllbGRzXHJcbiAgICBwdWJsaWMgc3RhdGljIFBsdWdpbklkZW50aWZpY2F0aW9uU2V0OiBNYXA8c3RyaW5nLCBQbHVnaW5JZGVudGlmaWNhdGlvblZpZXdNb2RlbFtdPlxyXG5cclxuICAgIC8vIGNvbnN0cnVjdG9yc1xyXG4gICAgc3RhdGljIEluaXRpYWxpemUgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIFBsdWdpbkhpdmUuUGx1Z2luSWRlbnRpZmljYXRpb25TZXQgPSBuZXcgTWFwPHN0cmluZywgUGx1Z2luSWRlbnRpZmljYXRpb25WaWV3TW9kZWxbXT4oKVxyXG4gICAgfSAgIFxyXG5cclxuICAgIHN0YXRpYyBGaW5kUGx1Z2luSWRlbnRpZmljYXRpb24gPSAocGx1Z2luVHlwZU5hbWU6IHN0cmluZywgcGx1Z2luSWQ6IHN0cmluZykgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gUGx1Z2luSGl2ZS5QbHVnaW5JZGVudGlmaWNhdGlvblNldC5nZXQocGx1Z2luVHlwZU5hbWUpLmZpbmQocGx1Z2luSWRlbnRpZmljYXRpb24gPT4gcGx1Z2luSWRlbnRpZmljYXRpb24uSWQgPT09IHBsdWdpbklkKTtcclxuICAgIH1cclxufSIsImNsYXNzIENoYW5uZWxIdWJWaWV3TW9kZWxcclxue1xyXG4gICAgcHVibGljIE5hbWU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgR3JvdXA6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRGF0YVR5cGU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxPbmVEYXNEYXRhVHlwZUVudW0+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgU2FtcGxlUmF0ZTogS25vY2tvdXRPYnNlcnZhYmxlPFNhbXBsZVJhdGVFbnVtPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IEd1aWQ6IHN0cmluZ1xyXG4gICAgcHVibGljIHJlYWRvbmx5IENyZWF0aW9uRGF0ZVRpbWU6IHN0cmluZ1xyXG4gICAgcHVibGljIHJlYWRvbmx5IFVuaXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgVHJhbnNmZXJGdW5jdGlvblNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8VHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbD5cclxuICAgIHB1YmxpYyBTZWxlY3RlZFRyYW5zZmVyRnVuY3Rpb246IEtub2Nrb3V0T2JzZXJ2YWJsZTxUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsPlxyXG4gICAgcHVibGljIEV2YWx1YXRlZFRyYW5zZmVyRnVuY3Rpb25TZXQ6ICgodmFsdWU6IG51bWJlcikgPT4gbnVtYmVyKVtdXHJcbiAgICBwdWJsaWMgSXNTZWxlY3RlZDogS25vY2tvdXRPYnNlcnZhYmxlPGJvb2xlYW4+XHJcblxyXG4gICAgcHVibGljIHJlYWRvbmx5IEFzc29jaWF0ZWREYXRhSW5wdXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZTxEYXRhUG9ydFZpZXdNb2RlbD5cclxuICAgIHB1YmxpYyByZWFkb25seSBBc3NvY2lhdGVkRGF0YU91dHB1dFNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8RGF0YVBvcnRWaWV3TW9kZWw+XHJcblxyXG4gICAgcHJpdmF0ZSBBc3NvY2lhdGVkRGF0YUlucHV0SWQ6IHN0cmluZ1xyXG4gICAgcHJpdmF0ZSBBc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0OiBzdHJpbmdbXVxyXG5cclxuICAgIGNvbnN0cnVjdG9yKGNoYW5uZWxIdWJNb2RlbDogQ2hhbm5lbEh1Yk1vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuTmFtZSA9IGtvLm9ic2VydmFibGU8c3RyaW5nPihjaGFubmVsSHViTW9kZWwuTmFtZSlcclxuICAgICAgICB0aGlzLkdyb3VwID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KGNoYW5uZWxIdWJNb2RlbC5Hcm91cClcclxuICAgICAgICB0aGlzLkRhdGFUeXBlID0ga28ub2JzZXJ2YWJsZTxPbmVEYXNEYXRhVHlwZUVudW0+KGNoYW5uZWxIdWJNb2RlbC5EYXRhVHlwZSlcclxuICAgICAgICB0aGlzLlNhbXBsZVJhdGUgPSBrby5vYnNlcnZhYmxlPFNhbXBsZVJhdGVFbnVtPihjaGFubmVsSHViTW9kZWwuU2FtcGxlUmF0ZSlcclxuICAgICAgICB0aGlzLkd1aWQgPSBjaGFubmVsSHViTW9kZWwuR3VpZFxyXG4gICAgICAgIHRoaXMuQ3JlYXRpb25EYXRlVGltZSA9IGNoYW5uZWxIdWJNb2RlbC5DcmVhdGlvbkRhdGVUaW1lXHJcbiAgICAgICAgdGhpcy5Vbml0ID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KGNoYW5uZWxIdWJNb2RlbC5Vbml0KVxyXG4gICAgICAgIHRoaXMuVHJhbnNmZXJGdW5jdGlvblNldCA9IGtvLm9ic2VydmFibGVBcnJheTxUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsPihjaGFubmVsSHViTW9kZWwuVHJhbnNmZXJGdW5jdGlvblNldC5tYXAodGYgPT4gbmV3IFRyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWwodGYpKSlcclxuICAgICAgICB0aGlzLlNlbGVjdGVkVHJhbnNmZXJGdW5jdGlvbiA9IGtvLm9ic2VydmFibGU8VHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbD4odGhpcy5DcmVhdGVEZWZhdWx0VHJhbnNmZXJGdW5jdGlvbigpKVxyXG4gICAgICAgIHRoaXMuSXNTZWxlY3RlZCA9IGtvLm9ic2VydmFibGU8Ym9vbGVhbj4oZmFsc2UpXHJcblxyXG4gICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dCA9IGtvLm9ic2VydmFibGU8RGF0YVBvcnRWaWV3TW9kZWw+KClcclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0U2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PERhdGFQb3J0Vmlld01vZGVsPigpXHJcblxyXG4gICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dElkID0gY2hhbm5lbEh1Yk1vZGVsLlNlcmlhbGl6ZXJEYXRhSW5wdXRJZFxyXG4gICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldCA9IGNoYW5uZWxIdWJNb2RlbC5TZXJpYWxpemVyRGF0YU91dHB1dElkU2V0XHJcbiAgICAgICAgdGhpcy5FdmFsdWF0ZWRUcmFuc2ZlckZ1bmN0aW9uU2V0ID0gW11cclxuICAgIH1cclxuXHJcbiAgICAvLyBtZXRob2RzXHJcbiAgICBwdWJsaWMgR2V0VHJhbnNmb3JtZWRWYWx1ZSA9ICh2YWx1ZTogYW55KTogc3RyaW5nID0+IFxyXG4gICAge1xyXG4gICAgICAgIGlmICh2YWx1ZSA9PT0gXCJOYU5cIilcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHZhbHVlID0gTmFOXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0aGlzLkV2YWx1YXRlZFRyYW5zZmVyRnVuY3Rpb25TZXQuZm9yRWFjaCh0ZiA9PiB2YWx1ZSA9IHRmKHZhbHVlKSlcclxuXHJcbiAgICAgICAgcmV0dXJuIHZhbHVlXHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBDcmVhdGVEZWZhdWx0VHJhbnNmZXJGdW5jdGlvbiA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIG5ldyBUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsKG5ldyBUcmFuc2ZlckZ1bmN0aW9uTW9kZWwoXCIwMDAxLTAxLTAxVDAwOjAwOjAwWlwiLCBcInBvbHlub21pYWxcIiwgXCJwZXJtYW5lbnRcIiwgXCIxOzBcIikpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIElzQXNzb2NpYXRpb25BbGxvd2VkKGRhdGFQb3J0OiBEYXRhUG9ydFZpZXdNb2RlbClcclxuICAgIHtcclxuICAgICAgICByZXR1cm4gKGRhdGFQb3J0LkRhdGFUeXBlICYgMHhmZikgPT0gKHRoaXMuRGF0YVR5cGUoKSAmIDB4ZmYpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFVwZGF0ZUFzc29jaWF0aW9uID0gKGRhdGFQb3J0OiBEYXRhUG9ydFZpZXdNb2RlbCkgPT5cclxuICAgIHtcclxuICAgICAgICBzd2l0Y2ggKGRhdGFQb3J0LkRhdGFEaXJlY3Rpb24pXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLklucHV0OlxyXG5cclxuICAgICAgICAgICAgICAgIHRoaXMuUmVzZXRBc3NvY2lhdGlvbihmYWxzZSwgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0KCkpXHJcbiAgICAgICAgICAgICAgICBicmVha1xyXG5cclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5PdXRwdXQ6XHJcblxyXG4gICAgICAgICAgICAgICAgdGhpcy5SZXNldEFzc29jaWF0aW9uKGZhbHNlLCBkYXRhUG9ydClcclxuICAgICAgICAgICAgICAgIGJyZWFrXHJcblxyXG4gICAgICAgICAgICBkZWZhdWx0OlxyXG4gICAgICAgICAgICAgICAgdGhyb3cgbmV3IEVycm9yKFwiTm90IGltcGxlbWVudGVkLlwiKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuU2V0QXNzb2NpYXRpb24oZGF0YVBvcnQpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFNldEFzc29jaWF0aW9uKGRhdGFQb3J0OiBEYXRhUG9ydFZpZXdNb2RlbClcclxuICAgIHtcclxuICAgICAgICBkYXRhUG9ydC5Bc3NvY2lhdGVkQ2hhbm5lbEh1YlNldC5wdXNoKHRoaXMpXHJcblxyXG4gICAgICAgIHN3aXRjaCAoZGF0YVBvcnQuRGF0YURpcmVjdGlvbilcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uSW5wdXQ6XHJcblxyXG4gICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0KGRhdGFQb3J0KVxyXG4gICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0SWQgPSBkYXRhUG9ydC5Ub0Z1bGxRdWFsaWZpZWRJZGVudGlmaWVyKClcclxuXHJcbiAgICAgICAgICAgICAgICBicmVha1xyXG5cclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5PdXRwdXQ6XHJcblxyXG4gICAgICAgICAgICAgICAgbGV0IGRhdGFPdXRwdXRJZCA9IGRhdGFQb3J0LlRvRnVsbFF1YWxpZmllZElkZW50aWZpZXIoKVxyXG5cclxuICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRTZXQucHVzaChkYXRhUG9ydClcclxuXHJcbiAgICAgICAgICAgICAgICBpZiAodGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0LmluZGV4T2YoZGF0YU91dHB1dElkKSA8IDApXHJcbiAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0LnB1c2goZGF0YVBvcnQuVG9GdWxsUXVhbGlmaWVkSWRlbnRpZmllcigpKVxyXG4gICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgIGJyZWFrXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBSZXNldEFzc29jaWF0aW9uKG1haW50YWluV2Vha1JlZmVyZW5jZTogYm9vbGVhbiwgLi4uZGF0YVBvcnRTZXQ6IERhdGFQb3J0Vmlld01vZGVsW10pXHJcbiAgICB7XHJcbiAgICAgICAgZGF0YVBvcnRTZXQuZm9yRWFjaChkYXRhUG9ydCA9PlxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgaWYgKCFkYXRhUG9ydClcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuXHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIGRhdGFQb3J0LkFzc29jaWF0ZWRDaGFubmVsSHViU2V0LnJlbW92ZSh0aGlzKVxyXG5cclxuICAgICAgICAgICAgc3dpdGNoIChkYXRhUG9ydC5EYXRhRGlyZWN0aW9uKVxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLklucHV0OlxyXG5cclxuICAgICAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXQobnVsbClcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCFtYWludGFpbldlYWtSZWZlcmVuY2UpXHJcbiAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXRJZCA9IG51bGxcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGJyZWFrXHJcblxyXG4gICAgICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5PdXRwdXQ6XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRTZXQucmVtb3ZlKGRhdGFQb3J0KVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBpZiAoIW1haW50YWluV2Vha1JlZmVyZW5jZSlcclxuICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGxldCBpbmRleDogbnVtYmVyID0gdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0LmluZGV4T2YoZGF0YVBvcnQuVG9GdWxsUXVhbGlmaWVkSWRlbnRpZmllcigpKVxyXG5cclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKGluZGV4ID4gLTEpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldC5zcGxpY2UoaW5kZXgsIDEpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0gICBcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGJyZWFrXHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9KVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBSZXNldEFsbEFzc29jaWF0aW9ucyhtYWludGFpbldlYWtSZWZlcmVuY2U6IGJvb2xlYW4pXHJcbiAgICB7XHJcbiAgICAgICAgaWYgKHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dCgpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5SZXNldEFzc29jaWF0aW9uKG1haW50YWluV2Vha1JlZmVyZW5jZSwgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0KCkpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0aGlzLlJlc2V0QXNzb2NpYXRpb24obWFpbnRhaW5XZWFrUmVmZXJlbmNlLCAuLi50aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0U2V0KCkpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEdldEFzc29jaWF0ZWREYXRhSW5wdXRJZCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dElkO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBHZXRBc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0ID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0O1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb01vZGVsKClcclxuICAgIHtcclxuICAgICAgICByZXR1cm4ge1xyXG4gICAgICAgICAgICBOYW1lOiA8c3RyaW5nPnRoaXMuTmFtZSgpLFxyXG4gICAgICAgICAgICBHcm91cDogPHN0cmluZz50aGlzLkdyb3VwKCksXHJcbiAgICAgICAgICAgIERhdGFUeXBlOiA8T25lRGFzRGF0YVR5cGVFbnVtPnRoaXMuRGF0YVR5cGUoKSxcclxuICAgICAgICAgICAgU2FtcGxlUmF0ZTogPFNhbXBsZVJhdGVFbnVtPnRoaXMuU2FtcGxlUmF0ZSgpLFxyXG4gICAgICAgICAgICBHdWlkOiA8c3RyaW5nPnRoaXMuR3VpZCxcclxuICAgICAgICAgICAgQ3JlYXRpb25EYXRlVGltZTogPHN0cmluZz50aGlzLkNyZWF0aW9uRGF0ZVRpbWUsXHJcbiAgICAgICAgICAgIFVuaXQ6IDxzdHJpbmc+dGhpcy5Vbml0KCksXHJcbiAgICAgICAgICAgIFRyYW5zZmVyRnVuY3Rpb25TZXQ6IDxUcmFuc2ZlckZ1bmN0aW9uTW9kZWxbXT50aGlzLlRyYW5zZmVyRnVuY3Rpb25TZXQoKS5tYXAodGYgPT4gdGYuVG9Nb2RlbCgpKSxcclxuICAgICAgICAgICAgU2VyaWFsaXplckRhdGFJbnB1dElkOiA8c3RyaW5nPnRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dElkLFxyXG4gICAgICAgICAgICBTZXJpYWxpemVyRGF0YU91dHB1dElkU2V0OiA8c3RyaW5nW10+dGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dElkU2V0XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBBZGRUcmFuc2ZlckZ1bmN0aW9uID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLlRyYW5zZmVyRnVuY3Rpb25TZXQucHVzaCh0aGlzLlNlbGVjdGVkVHJhbnNmZXJGdW5jdGlvbigpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBEZWxldGVUcmFuc2ZlckZ1bmN0aW9uID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLlRyYW5zZmVyRnVuY3Rpb25TZXQucmVtb3ZlKHRoaXMuU2VsZWN0ZWRUcmFuc2ZlckZ1bmN0aW9uKCkpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIE5ld1RyYW5zZmVyRnVuY3Rpb24gPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuU2VsZWN0ZWRUcmFuc2ZlckZ1bmN0aW9uKHRoaXMuQ3JlYXRlRGVmYXVsdFRyYW5zZmVyRnVuY3Rpb24oKSlcclxuICAgIH1cclxuXHJcbiAgICAvLyBjb21tYW5kc1xyXG4gICAgcHVibGljIFNlbGVjdFRyYW5zZmVyRnVuY3Rpb24gPSAodHJhbnNmZXJGdW5jdGlvbjogVHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLlNlbGVjdGVkVHJhbnNmZXJGdW5jdGlvbih0cmFuc2ZlckZ1bmN0aW9uKVxyXG4gICAgfVxyXG59IiwiY2xhc3MgT25lRGFzTW9kdWxlVmlld01vZGVsXHJcbntcclxuICAgIHB1YmxpYyBEYXRhVHlwZTogS25vY2tvdXRPYnNlcnZhYmxlPE9uZURhc0RhdGFUeXBlRW51bT5cclxuICAgIHB1YmxpYyBEYXRhRGlyZWN0aW9uOiBLbm9ja291dE9ic2VydmFibGU8RGF0YURpcmVjdGlvbkVudW0+XHJcbiAgICBwdWJsaWMgRW5kaWFubmVzczogS25vY2tvdXRPYnNlcnZhYmxlPEVuZGlhbm5lc3NFbnVtPlxyXG4gICAgcHVibGljIFNpemU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxudW1iZXI+XHJcbiAgICBwdWJsaWMgTWF4U2l6ZTogS25vY2tvdXRPYnNlcnZhYmxlPG51bWJlcj5cclxuICAgIHB1YmxpYyBFcnJvck1lc3NhZ2U6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgSGFzRXJyb3I6IEtub2Nrb3V0Q29tcHV0ZWQ8Ym9vbGVhbj5cclxuXHJcbiAgICBwdWJsaWMgRGF0YVR5cGVTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PE9uZURhc0RhdGFUeXBlRW51bT5cclxuXHJcbiAgICBwcml2YXRlIF9vblByb3BlcnR5Q2hhbmdlZDogRXZlbnREaXNwYXRjaGVyPE9uZURhc01vZHVsZVZpZXdNb2RlbCwgYW55PlxyXG4gICAgcHJvdGVjdGVkIF9tb2RlbDogYW55XHJcblxyXG4gICAgY29uc3RydWN0b3Iob25lRGFzTW9kdWxlTW9kZWw6IE9uZURhc01vZHVsZU1vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuX21vZGVsID0gb25lRGFzTW9kdWxlTW9kZWxcclxuXHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZSA9IGtvLm9ic2VydmFibGU8T25lRGFzRGF0YVR5cGVFbnVtPihvbmVEYXNNb2R1bGVNb2RlbC5EYXRhVHlwZSlcclxuICAgICAgICB0aGlzLkRhdGFEaXJlY3Rpb24gPSBrby5vYnNlcnZhYmxlPERhdGFEaXJlY3Rpb25FbnVtPihvbmVEYXNNb2R1bGVNb2RlbC5EYXRhRGlyZWN0aW9uKVxyXG4gICAgICAgIHRoaXMuRW5kaWFubmVzcyA9IGtvLm9ic2VydmFibGU8RW5kaWFubmVzc0VudW0+KG9uZURhc01vZHVsZU1vZGVsLkVuZGlhbm5lc3MpXHJcbiAgICAgICAgdGhpcy5TaXplID0ga28ub2JzZXJ2YWJsZTxudW1iZXI+KG9uZURhc01vZHVsZU1vZGVsLlNpemUpXHJcbiAgICAgICAgdGhpcy5NYXhTaXplID0ga28ub2JzZXJ2YWJsZTxudW1iZXI+KEluZmluaXR5KVxyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KFwiXCIpXHJcbiAgICAgICAgdGhpcy5IYXNFcnJvciA9IGtvLmNvbXB1dGVkPGJvb2xlYW4+KCgpID0+IHRoaXMuRXJyb3JNZXNzYWdlKCkubGVuZ3RoID4gMClcclxuXHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZVNldCA9IGtvLm9ic2VydmFibGVBcnJheTxPbmVEYXNEYXRhVHlwZUVudW0+KEVudW1lcmF0aW9uSGVscGVyLkdldEVudW1WYWx1ZXMoJ09uZURhc0RhdGFUeXBlRW51bScpLmZpbHRlcihkYXRhVHlwZSA9PiBkYXRhVHlwZSAhPT0gT25lRGFzRGF0YVR5cGVFbnVtLkJPT0xFQU4pKVxyXG4gICAgICAgIHRoaXMuX29uUHJvcGVydHlDaGFuZ2VkID0gbmV3IEV2ZW50RGlzcGF0Y2hlcjxPbmVEYXNNb2R1bGVWaWV3TW9kZWwsIGFueT4oKTtcclxuXHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZS5zdWJzY3JpYmUobmV3VmFsdWUgPT4gdGhpcy5PblByb3BlcnR5Q2hhbmdlZCgpKVxyXG4gICAgICAgIHRoaXMuRGF0YURpcmVjdGlvbi5zdWJzY3JpYmUobmV3VmFsdWUgPT4gdGhpcy5PblByb3BlcnR5Q2hhbmdlZCgpKVxyXG4gICAgICAgIHRoaXMuU2l6ZS5zdWJzY3JpYmUobmV3VmFsdWUgPT4gdGhpcy5PblByb3BlcnR5Q2hhbmdlZCgpKVxyXG4gICAgfVxyXG5cclxuICAgIGdldCBQcm9wZXJ0eUNoYW5nZWQoKTogSUV2ZW50PE9uZURhc01vZHVsZVZpZXdNb2RlbCwgYW55PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLl9vblByb3BlcnR5Q2hhbmdlZDtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgT25Qcm9wZXJ0eUNoYW5nZWQgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuVmFsaWRhdGUoKVxyXG4gICAgICAgIHRoaXMuX29uUHJvcGVydHlDaGFuZ2VkLmRpc3BhdGNoKHRoaXMsIG51bGwpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEdldEJ5dGVDb3VudCA9IChib29sZWFuQml0U2l6ZT86IG51bWJlcikgPT5cclxuICAgIHtcclxuICAgICAgICBpZiAoYm9vbGVhbkJpdFNpemUgJiYgdGhpcy5EYXRhVHlwZSgpID09PSBPbmVEYXNEYXRhVHlwZUVudW0uQk9PTEVBTilcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGJvb2xlYW5CaXRTaXplID0gcGFyc2VJbnQoPGFueT5ib29sZWFuQml0U2l6ZSlcclxuXHJcbiAgICAgICAgICAgIHJldHVybiBNYXRoLmNlaWwoYm9vbGVhbkJpdFNpemUgKiB0aGlzLlNpemUoKSAvIDgpO1xyXG4gICAgICAgIH1cclxuICAgICAgICBlbHNlXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICByZXR1cm4gKHRoaXMuRGF0YVR5cGUoKSAmIDB4MEZGKSAvIDggKiB0aGlzLlNpemUoKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVmFsaWRhdGUoKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiXCIpXHJcblxyXG4gICAgICAgIGlmICh0aGlzLlNpemUoKSA8IDEgfHwgKGlzRmluaXRlKHRoaXMuTWF4U2l6ZSgpKSAmJiB0aGlzLlNpemUoKSA+IHRoaXMuTWF4U2l6ZSgpKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiU2l6ZSBtdXN0IGJlIHdpdGhpbiByYW5nZSAxLi5cIiArIHRoaXMuTWF4U2l6ZSgpICsgXCIuXCIpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb1N0cmluZygpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuU2l6ZSgpICsgXCJ4IFwiICsgRW51bWVyYXRpb25IZWxwZXIuR2V0RW51bUxvY2FsaXphdGlvbignT25lRGFzRGF0YVR5cGVFbnVtJywgdGhpcy5EYXRhVHlwZSgpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBFeHRlbmRNb2RlbChtb2RlbDogYW55KVxyXG4gICAge1xyXG4gICAgICAgIC8vXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvTW9kZWwoKVxyXG4gICAge1xyXG4gICAgICAgIGxldCBtb2RlbDogYW55ID0ge1xyXG4gICAgICAgICAgICAkdHlwZTogPHN0cmluZz50aGlzLl9tb2RlbC4kdHlwZSxcclxuICAgICAgICAgICAgRGF0YVR5cGU6IDxPbmVEYXNEYXRhVHlwZUVudW0+dGhpcy5EYXRhVHlwZSgpLFxyXG4gICAgICAgICAgICBTaXplOiA8bnVtYmVyPnRoaXMuU2l6ZSgpLFxyXG4gICAgICAgICAgICBEYXRhRGlyZWN0aW9uOiA8RGF0YURpcmVjdGlvbkVudW0+dGhpcy5EYXRhRGlyZWN0aW9uKCksXHJcbiAgICAgICAgICAgIEVuZGlhbm5lc3M6IDxFbmRpYW5uZXNzRW51bT50aGlzLkVuZGlhbm5lc3MoKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5FeHRlbmRNb2RlbChtb2RlbClcclxuXHJcbiAgICAgICAgcmV0dXJuIG1vZGVsXHJcbiAgICB9XHJcbn1cclxuIiwiY2xhc3MgT25lRGFzTW9kdWxlU2VsZWN0b3JWaWV3TW9kZWxcclxue1xyXG4gICAgcHVibGljIFNldHRpbmdzVGVtcGxhdGVOYW1lOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIE5ld01vZHVsZTogS25vY2tvdXRPYnNlcnZhYmxlPE9uZURhc01vZHVsZVZpZXdNb2RlbD4gIFxyXG4gICAgcHVibGljIE1heEJ5dGVzOiBLbm9ja291dE9ic2VydmFibGU8bnVtYmVyPlxyXG4gICAgcHVibGljIFJlbWFpbmluZ0J5dGVzOiBLbm9ja291dE9ic2VydmFibGU8bnVtYmVyPlxyXG4gICAgcHVibGljIFJlbWFpbmluZ0NvdW50OiBLbm9ja291dE9ic2VydmFibGU8bnVtYmVyPiAgICBcclxuICAgIHB1YmxpYyBNb2R1bGVTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PE9uZURhc01vZHVsZVZpZXdNb2RlbD5cclxuICAgIHB1YmxpYyBFcnJvck1lc3NhZ2U6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgSGFzRXJyb3I6IEtub2Nrb3V0Q29tcHV0ZWQ8Ym9vbGVhbj5cclxuXHJcbiAgICBwdWJsaWMgT25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlOiBLbm9ja291dE9ic2VydmFibGU8T25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlRW51bT5cclxuXHJcbiAgICBwcml2YXRlIF9vbk1vZHVsZVNldENoYW5nZWQ6IEV2ZW50RGlzcGF0Y2hlcjxPbmVEYXNNb2R1bGVTZWxlY3RvclZpZXdNb2RlbCwgT25lRGFzTW9kdWxlVmlld01vZGVsW10+XHJcblxyXG4gICAgY29uc3RydWN0b3Iob25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlOiBPbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtLCBtb2R1bGVTZXQ6IE9uZURhc01vZHVsZVZpZXdNb2RlbFtdID0gW10pXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5PbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGUgPSBrby5vYnNlcnZhYmxlPE9uZURhc01vZHVsZVNlbGVjdG9yTW9kZUVudW0+KG9uZURhc01vZHVsZVNlbGVjdG9yTW9kZSlcclxuXHJcbiAgICAgICAgdGhpcy5TZXR0aW5nc1RlbXBsYXRlTmFtZSA9IGtvLm9ic2VydmFibGUoXCJQcm9qZWN0X09uZURhc01vZHVsZVNldHRpbmdzVGVtcGxhdGVcIilcclxuICAgICAgICB0aGlzLk5ld01vZHVsZSA9IGtvLm9ic2VydmFibGU8T25lRGFzTW9kdWxlVmlld01vZGVsPigpO1xyXG4gICAgICAgIHRoaXMuTWF4Qnl0ZXMgPSBrby5vYnNlcnZhYmxlPG51bWJlcj4oSW5maW5pdHkpO1xyXG4gICAgICAgIHRoaXMuUmVtYWluaW5nQnl0ZXMgPSBrby5vYnNlcnZhYmxlPG51bWJlcj4oTmFOKTtcclxuICAgICAgICB0aGlzLlJlbWFpbmluZ0NvdW50ID0ga28ub2JzZXJ2YWJsZTxudW1iZXI+KE5hTik7XHJcbiAgICAgICAgdGhpcy5Nb2R1bGVTZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8T25lRGFzTW9kdWxlVmlld01vZGVsPihtb2R1bGVTZXQpO1xyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KFwiXCIpXHJcbiAgICAgICAgdGhpcy5IYXNFcnJvciA9IGtvLmNvbXB1dGVkPGJvb2xlYW4+KCgpID0+IHRoaXMuRXJyb3JNZXNzYWdlKCkubGVuZ3RoID4gMClcclxuXHJcbiAgICAgICAgdGhpcy5fb25Nb2R1bGVTZXRDaGFuZ2VkID0gbmV3IEV2ZW50RGlzcGF0Y2hlcjxPbmVEYXNNb2R1bGVTZWxlY3RvclZpZXdNb2RlbCwgT25lRGFzTW9kdWxlVmlld01vZGVsW10+KCk7XHJcblxyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxDcmVhdGVOZXdNb2R1bGUoKVxyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgfVxyXG5cclxuICAgIGdldCBPbk1vZHVsZVNldENoYW5nZWQoKTogSUV2ZW50PE9uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsLCBPbmVEYXNNb2R1bGVWaWV3TW9kZWxbXT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5fb25Nb2R1bGVTZXRDaGFuZ2VkO1xyXG4gICAgfVxyXG4gICAgXHJcbiAgICAvLyBtZXRob2RzXHJcbiAgICBwdWJsaWMgU2V0TWF4Qnl0ZXMgPSAodmFsdWU6IG51bWJlcikgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLk1heEJ5dGVzKHZhbHVlKVxyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBHZXRJbnB1dE1vZHVsZVNldCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuTW9kdWxlU2V0KCkuZmlsdGVyKG1vZHVsZSA9PiBtb2R1bGUuRGF0YURpcmVjdGlvbigpID09PSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgR2V0T3V0cHV0TW9kdWxlU2V0ID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5Nb2R1bGVTZXQoKS5maWx0ZXIobW9kdWxlID0+IG1vZHVsZS5EYXRhRGlyZWN0aW9uKCkgPT09IERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dClcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIEludGVybmFsVXBkYXRlKClcclxuICAgIHtcclxuICAgICAgICB0aGlzLlVwZGF0ZSgpXHJcbiAgICAgICAgdGhpcy5WYWxpZGF0ZSgpXHJcbiAgICB9XHJcblxyXG4gICAgcHJvdGVjdGVkIFVwZGF0ZSgpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IG1vZHVsZVNldDogT25lRGFzTW9kdWxlVmlld01vZGVsW11cclxuICAgICAgICBsZXQgcmVtYWluaW5nQnl0ZXM6IG51bWJlclxyXG5cclxuICAgICAgICBzd2l0Y2ggKHRoaXMuTmV3TW9kdWxlKCkuRGF0YURpcmVjdGlvbigpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dDpcclxuICAgICAgICAgICAgICAgIG1vZHVsZVNldCA9IHRoaXMuR2V0SW5wdXRNb2R1bGVTZXQoKVxyXG4gICAgICAgICAgICAgICAgYnJlYWs7XHJcblxyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dDpcclxuICAgICAgICAgICAgICAgIG1vZHVsZVNldCA9IHRoaXMuR2V0T3V0cHV0TW9kdWxlU2V0KClcclxuICAgICAgICAgICAgICAgIGJyZWFrO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmVtYWluaW5nQnl0ZXMgPSB0aGlzLk1heEJ5dGVzKCkgLSBtb2R1bGVTZXQubWFwKG9uZURhc01vZHVsZSA9PiBvbmVEYXNNb2R1bGUuR2V0Qnl0ZUNvdW50KCkpLnJlZHVjZSgocHJldmlvdXNWYWx1ZSwgY3VycmVudFZhbHVlKSA9PiBwcmV2aW91c1ZhbHVlICsgY3VycmVudFZhbHVlLCAwKVxyXG5cclxuICAgICAgICB0aGlzLlJlbWFpbmluZ0J5dGVzKHJlbWFpbmluZ0J5dGVzKVxyXG4gICAgICAgIHRoaXMuUmVtYWluaW5nQ291bnQoTWF0aC5mbG9vcih0aGlzLlJlbWFpbmluZ0J5dGVzKCkgLyAoKHRoaXMuTmV3TW9kdWxlKCkuRGF0YVR5cGUoKSAmIDB4MEZGKSAvIDgpKSlcclxuICAgIH1cclxuXHJcbiAgICBwcm90ZWN0ZWQgVmFsaWRhdGUoKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiXCIpXHJcblxyXG4gICAgICAgIGlmICh0aGlzLk5ld01vZHVsZSgpLkhhc0Vycm9yKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIlJlc29sdmUgYWxsIHJlbWFpbmluZyBtb2R1bGUgZXJyb3JzIGJlZm9yZSBjb250aW51aW5nLlwiKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaWYgKHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlKCkgPT09IE9uZURhc01vZHVsZVNlbGVjdG9yTW9kZUVudW0uSW5wdXRPbmx5ICYmIHRoaXMuTmV3TW9kdWxlKCkuRGF0YURpcmVjdGlvbigpID09IERhdGFEaXJlY3Rpb25FbnVtLk91dHB1dClcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiT25seSBpbnB1dCBtb2R1bGVzIGFyZSBhbGxvd2VkLlwiKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaWYgKHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlKCkgPT09IE9uZURhc01vZHVsZVNlbGVjdG9yTW9kZUVudW0uT3V0cHV0T25seSAmJiB0aGlzLk5ld01vZHVsZSgpLkRhdGFEaXJlY3Rpb24oKSA9PSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dClcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiT25seSBvdXRwdXQgbW9kdWxlcyBhcmUgYWxsb3dlZC5cIilcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmIChpc0Zpbml0ZSh0aGlzLlJlbWFpbmluZ0J5dGVzKCkpICYmICh0aGlzLlJlbWFpbmluZ0J5dGVzKCkgLSB0aGlzLk5ld01vZHVsZSgpLkdldEJ5dGVDb3VudCgpIDwgMCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLkVycm9yTWVzc2FnZShcIkJ5dGUgY291bnQgb2YgbmV3IG1vZHVsZSBpcyB0b28gaGlnaC5cIilcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmICh0aGlzLlJlbWFpbmluZ0NvdW50KCkgPD0gMClcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiVGhlIG1heGltdW0gbnVtYmVyIG9mIG1vZHVsZXMgaXMgcmVhY2hlZC5cIilcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHJvdGVjdGVkIENyZWF0ZU5ld01vZHVsZSgpXHJcbiAgICB7XHJcbiAgICAgICAgaWYgKHRoaXMuTmV3TW9kdWxlKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICByZXR1cm4gbmV3IE9uZURhc01vZHVsZVZpZXdNb2RlbChuZXcgT25lRGFzTW9kdWxlTW9kZWwodGhpcy5OZXdNb2R1bGUoKS5EYXRhVHlwZSgpLCB0aGlzLk5ld01vZHVsZSgpLkRhdGFEaXJlY3Rpb24oKSwgdGhpcy5OZXdNb2R1bGUoKS5FbmRpYW5uZXNzKCksIDEpKVxyXG4gICAgICAgIH1cclxuICAgICAgICBlbHNlXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICByZXR1cm4gbmV3IE9uZURhc01vZHVsZVZpZXdNb2RlbChuZXcgT25lRGFzTW9kdWxlTW9kZWwoT25lRGFzRGF0YVR5cGVFbnVtLlVJTlQxNiwgRGF0YURpcmVjdGlvbkVudW0uSW5wdXQsIEVuZGlhbm5lc3NFbnVtLkxpdHRsZUVuZGlhbiwgMSkpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgSW50ZXJuYWxDcmVhdGVOZXdNb2R1bGUoKVxyXG4gICAge1xyXG4gICAgICAgIGlmICh0aGlzLk5ld01vZHVsZSgpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5OZXdNb2R1bGUoKS5Qcm9wZXJ0eUNoYW5nZWQudW5zdWJzY3JpYmUodGhpcy5Pbk1vZHVsZVByb3BlcnR5Q2hhbmdlZClcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuTmV3TW9kdWxlKHRoaXMuQ3JlYXRlTmV3TW9kdWxlKCkpXHJcbiAgICAgICAgdGhpcy5OZXdNb2R1bGUoKS5Qcm9wZXJ0eUNoYW5nZWQuc3Vic2NyaWJlKHRoaXMuT25Nb2R1bGVQcm9wZXJ0eUNoYW5nZWQpXHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBPbk1vZHVsZVByb3BlcnR5Q2hhbmdlZCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5JbnRlcm5hbFVwZGF0ZSgpXHJcbiAgICB9XHJcblxyXG4gICAgLy8gY29tbWFuZHNcclxuICAgIHB1YmxpYyBBZGRNb2R1bGUgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIGxldCBuZXdNb2R1bGU6IE9uZURhc01vZHVsZVZpZXdNb2RlbFxyXG5cclxuICAgICAgICBpZiAoIXRoaXMuSGFzRXJyb3IoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuTW9kdWxlU2V0LnB1c2godGhpcy5OZXdNb2R1bGUoKSlcclxuICAgICAgICAgICAgdGhpcy5JbnRlcm5hbENyZWF0ZU5ld01vZHVsZSgpXHJcbiAgICAgICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgICAgICAgICB0aGlzLl9vbk1vZHVsZVNldENoYW5nZWQuZGlzcGF0Y2godGhpcywgdGhpcy5Nb2R1bGVTZXQoKSlcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIERlbGV0ZU1vZHVsZSA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5Nb2R1bGVTZXQucG9wKClcclxuICAgICAgICB0aGlzLkludGVybmFsVXBkYXRlKClcclxuICAgICAgICB0aGlzLl9vbk1vZHVsZVNldENoYW5nZWQuZGlzcGF0Y2godGhpcywgdGhpcy5Nb2R1bGVTZXQoKSlcclxuICAgIH1cclxufSIsImNsYXNzIFRyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWxcclxue1xyXG4gICAgcHVibGljIERhdGVUaW1lOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIFR5cGU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgT3B0aW9uOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIEFyZ3VtZW50OiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKHRyYW5zZmVyRnVuY3Rpb25Nb2RlbDogVHJhbnNmZXJGdW5jdGlvbk1vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuRGF0ZVRpbWUgPSBrby5vYnNlcnZhYmxlKHRyYW5zZmVyRnVuY3Rpb25Nb2RlbC5EYXRlVGltZSlcclxuICAgICAgICB0aGlzLlR5cGUgPSBrby5vYnNlcnZhYmxlKHRyYW5zZmVyRnVuY3Rpb25Nb2RlbC5UeXBlKVxyXG4gICAgICAgIHRoaXMuT3B0aW9uID0ga28ub2JzZXJ2YWJsZSh0cmFuc2ZlckZ1bmN0aW9uTW9kZWwuT3B0aW9uKVxyXG4gICAgICAgIHRoaXMuQXJndW1lbnQgPSBrby5vYnNlcnZhYmxlKHRyYW5zZmVyRnVuY3Rpb25Nb2RlbC5Bcmd1bWVudClcclxuICAgIH1cclxuXHJcbiAgICAvLyBtZXRob2RzXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIG5ldyBUcmFuc2ZlckZ1bmN0aW9uTW9kZWwodGhpcy5EYXRlVGltZSgpLCB0aGlzLlR5cGUoKSwgdGhpcy5PcHRpb24oKSwgdGhpcy5Bcmd1bWVudCgpKVxyXG4gICAgfVxyXG59IiwiY2xhc3MgRGF0YVBvcnRWaWV3TW9kZWxcclxue1xyXG4gICAgLy8gZmllbGRzXHJcbiAgICBwdWJsaWMgTmFtZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyByZWFkb25seSBEYXRhVHlwZTogT25lRGFzRGF0YVR5cGVFbnVtXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRGF0YURpcmVjdGlvbjogRGF0YURpcmVjdGlvbkVudW1cclxuICAgIHB1YmxpYyByZWFkb25seSBFbmRpYW5uZXNzOiBFbmRpYW5uZXNzRW51bVxyXG5cclxuICAgIHB1YmxpYyBJc1NlbGVjdGVkOiBLbm9ja291dE9ic2VydmFibGU8Ym9vbGVhbj5cclxuICAgIHB1YmxpYyBBc3NvY2lhdGVkQ2hhbm5lbEh1YlNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8Q2hhbm5lbEh1YlZpZXdNb2RlbD5cclxuICAgIHB1YmxpYyByZWFkb25seSBBc3NvY2lhdGVkRGF0YUdhdGV3YXk6IERhdGFHYXRld2F5Vmlld01vZGVsQmFzZVxyXG4gICAgcHVibGljIHJlYWRvbmx5IExpdmVEZXNjcmlwdGlvbjogS25vY2tvdXRDb21wdXRlZDxzdHJpbmc+XHJcblxyXG4gICAgLy8gY29uc3RydWN0b3JzXHJcbiAgICBjb25zdHJ1Y3RvcihkYXRhUG9ydE1vZGVsOiBhbnksIGFzc29jaWF0ZWREYXRhR2F0ZXdheTogRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuTmFtZSA9IGtvLm9ic2VydmFibGUoZGF0YVBvcnRNb2RlbC5OYW1lKVxyXG4gICAgICAgIHRoaXMuRGF0YVR5cGUgPSBkYXRhUG9ydE1vZGVsLkRhdGFUeXBlXHJcbiAgICAgICAgdGhpcy5EYXRhRGlyZWN0aW9uID0gZGF0YVBvcnRNb2RlbC5EYXRhRGlyZWN0aW9uXHJcbiAgICAgICAgdGhpcy5FbmRpYW5uZXNzID0gZGF0YVBvcnRNb2RlbC5FbmRpYW5uZXNzXHJcblxyXG4gICAgICAgIHRoaXMuSXNTZWxlY3RlZCA9IGtvLm9ic2VydmFibGU8Ym9vbGVhbj4oZmFsc2UpXHJcbiAgICAgICAgdGhpcy5Bc3NvY2lhdGVkQ2hhbm5lbEh1YlNldCA9IGtvLm9ic2VydmFibGVBcnJheTxDaGFubmVsSHViVmlld01vZGVsPigpXHJcbiAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YUdhdGV3YXkgPSBhc3NvY2lhdGVkRGF0YUdhdGV3YXlcclxuXHJcbiAgICAgICAgdGhpcy5MaXZlRGVzY3JpcHRpb24gPSBrby5jb21wdXRlZCgoKSA9PlxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgbGV0IHJlc3VsdDogc3RyaW5nXHJcblxyXG4gICAgICAgICAgICByZXN1bHQgPSBcIjxkaXYgY2xhc3M9J3RleHQtbGVmdCc+XCIgKyB0aGlzLk5hbWUoKSArIFwiPC9kaXY+PGRpdiBjbGFzcz0ndGV4dC1sZWZ0Jz5cIiArIEVudW1lcmF0aW9uSGVscGVyLkdldEVudW1Mb2NhbGl6YXRpb24oJ09uZURhc0RhdGFUeXBlRW51bScsIHRoaXMuRGF0YVR5cGUpICsgXCI8L2Rpdj5cIlxyXG5cclxuICAgICAgICAgICAgaWYgKHRoaXMuQXNzb2NpYXRlZENoYW5uZWxIdWJTZXQoKS5sZW5ndGggPiAwKVxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWRDaGFubmVsSHViU2V0KCkuZm9yRWFjaChjaGFubmVsSHViID0+XHJcbiAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgcmVzdWx0ICs9IFwiPC9iciA+PGRpdiBjbGFzcz0ndGV4dC1sZWZ0Jz5cIiArIGNoYW5uZWxIdWIuTmFtZSgpICsgXCI8L2Rpdj48ZGl2IGNsYXNzPSd0ZXh0LWxlZnQnPlwiICsgRW51bWVyYXRpb25IZWxwZXIuR2V0RW51bUxvY2FsaXphdGlvbignT25lRGFzRGF0YVR5cGVFbnVtJywgY2hhbm5lbEh1Yi5EYXRhVHlwZSgpKSArIFwiPC9kaXY+XCJcclxuICAgICAgICAgICAgICAgIH0pXHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIHJldHVybiByZXN1bHRcclxuICAgICAgICB9KVxyXG4gICAgfVxyXG5cclxuICAgIC8vIG1ldGhvZHNcclxuICAgIHB1YmxpYyBHZXRJZCgpOiBzdHJpbmdcclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5OYW1lKClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9GdWxsUXVhbGlmaWVkSWRlbnRpZmllcigpOiBzdHJpbmdcclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5Bc3NvY2lhdGVkRGF0YUdhdGV3YXkuRGVzY3JpcHRpb24uSWQgKyBcIiAoXCIgKyB0aGlzLkFzc29jaWF0ZWREYXRhR2F0ZXdheS5EZXNjcmlwdGlvbi5JbnN0YW5jZUlkICsgXCIpIC8gXCIgKyB0aGlzLkdldElkKCk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEV4dGVuZE1vZGVsKG1vZGVsOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgLy9cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IG1vZGVsOiBhbnkgPSB7XHJcbiAgICAgICAgICAgIE5hbWU6IDxzdHJpbmc+dGhpcy5OYW1lKCksXHJcbiAgICAgICAgICAgIERhdGFUeXBlOiA8T25lRGFzRGF0YVR5cGVFbnVtPnRoaXMuRGF0YVR5cGUsXHJcbiAgICAgICAgICAgIERhdGFEaXJlY3Rpb246IDxEYXRhRGlyZWN0aW9uRW51bT50aGlzLkRhdGFEaXJlY3Rpb25cclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuRXh0ZW5kTW9kZWwobW9kZWwpXHJcblxyXG4gICAgICAgIHJldHVybiBtb2RlbFxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBSZXNldEFzc29jaWF0aW9ucyhtYWludGFpbldlYWtSZWZlcmVuY2U6IGJvb2xlYW4pXHJcbiAgICB7XHJcbiAgICAgICAgaWYgKHRoaXMuQXNzb2NpYXRlZENoYW5uZWxIdWJTZXQoKS5sZW5ndGggPiAwKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkQ2hhbm5lbEh1YlNldCgpLmZvckVhY2goY2hhbm5lbEh1YiA9PiBjaGFubmVsSHViLlJlc2V0QXNzb2NpYXRpb24obWFpbnRhaW5XZWFrUmVmZXJlbmNlLCB0aGlzKSlcclxuICAgICAgICB9XHJcbiAgICB9XHJcbn0iLCJhYnN0cmFjdCBjbGFzcyBQbHVnaW5WaWV3TW9kZWxCYXNlXHJcbntcclxuICAgIHB1YmxpYyBEZXNjcmlwdGlvbjogUGx1Z2luRGVzY3JpcHRpb25WaWV3TW9kZWxcclxuICAgIHB1YmxpYyBQbHVnaW5JZGVudGlmaWNhdGlvbjogUGx1Z2luSWRlbnRpZmljYXRpb25WaWV3TW9kZWxcclxuICAgIHB1YmxpYyBJc0luU2V0dGluZ3NNb2RlOiBLbm9ja291dE9ic2VydmFibGU8Ym9vbGVhbj5cclxuXHJcbiAgICBwcml2YXRlIF9tb2RlbDogYW55XHJcblxyXG4gICAgY29uc3RydWN0b3IocGx1Z2luU2V0dGluZ3NNb2RlbDogYW55LCBwbHVnaW5JZGVudGlmaWNhdGlvbjogUGx1Z2luSWRlbnRpZmljYXRpb25WaWV3TW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5fbW9kZWwgPSBwbHVnaW5TZXR0aW5nc01vZGVsXHJcbiAgICAgICAgdGhpcy5EZXNjcmlwdGlvbiA9IG5ldyBQbHVnaW5EZXNjcmlwdGlvblZpZXdNb2RlbChwbHVnaW5TZXR0aW5nc01vZGVsLkRlc2NyaXB0aW9uKVxyXG4gICAgICAgIHRoaXMuUGx1Z2luSWRlbnRpZmljYXRpb24gPSBwbHVnaW5JZGVudGlmaWNhdGlvblxyXG4gICAgICAgIHRoaXMuSXNJblNldHRpbmdzTW9kZSA9IGtvLm9ic2VydmFibGU8Ym9vbGVhbj4oZmFsc2UpXHJcbiAgICB9XHJcblxyXG4gICAgLy8gbWV0aG9kc1xyXG4gICAgcHVibGljIGFic3RyYWN0IGFzeW5jIEluaXRpYWxpemVBc3luYygpOiBQcm9taXNlPGFueT5cclxuXHJcbiAgICBwdWJsaWMgU2VuZEFjdGlvblJlcXVlc3QgPSBhc3luYyAoaW5zdGFuY2VJZDogbnVtYmVyLCBtZXRob2ROYW1lOiBzdHJpbmcsIGRhdGE6IGFueSkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gPEFjdGlvblJlc3BvbnNlPiBhd2FpdCBDb25uZWN0aW9uTWFuYWdlci5JbnZva2VXZWJDbGllbnRIdWIoXCJSZXF1ZXN0QWN0aW9uXCIsIG5ldyBBY3Rpb25SZXF1ZXN0KHRoaXMuRGVzY3JpcHRpb24uSWQsIGluc3RhbmNlSWQsIG1ldGhvZE5hbWUsIGRhdGEpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBFeHRlbmRNb2RlbChtb2RlbDogYW55KVxyXG4gICAge1xyXG4gICAgICAgIC8vXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvTW9kZWwoKVxyXG4gICAge1xyXG4gICAgICAgIGxldCBtb2RlbDogYW55ID0ge1xyXG4gICAgICAgICAgICAkdHlwZTogPHN0cmluZz50aGlzLl9tb2RlbC4kdHlwZSxcclxuICAgICAgICAgICAgRGVzY3JpcHRpb246IDxQbHVnaW5EZXNjcmlwdGlvblZpZXdNb2RlbD50aGlzLkRlc2NyaXB0aW9uLlRvTW9kZWwoKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5FeHRlbmRNb2RlbChtb2RlbClcclxuXHJcbiAgICAgICAgcmV0dXJuIG1vZGVsXHJcbiAgICB9XHJcblxyXG4gICAgLy8gY29tbWFuZHNcclxuICAgIHB1YmxpYyBFbmFibGVTZXR0aW5nc01vZGUgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuSXNJblNldHRpbmdzTW9kZSh0cnVlKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBEaXNhYmxlU2V0dGluZ3NNb2RlID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLklzSW5TZXR0aW5nc01vZGUoZmFsc2UpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvZ2dsZVNldHRpbmdzTW9kZSA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5Jc0luU2V0dGluZ3NNb2RlKCF0aGlzLklzSW5TZXR0aW5nc01vZGUoKSlcclxuICAgIH1cclxufSIsIi8vLyA8cmVmZXJlbmNlIHBhdGg9XCJQbHVnaW5WaWV3TW9kZWxCYXNlLnRzXCIvPlxyXG5cclxuYWJzdHJhY3QgY2xhc3MgRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlIGV4dGVuZHMgUGx1Z2luVmlld01vZGVsQmFzZVxyXG57XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgTWF4aW11bURhdGFzZXRBZ2U6IEtub2Nrb3V0T2JzZXJ2YWJsZTxudW1iZXI+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRGF0YVBvcnRTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PERhdGFQb3J0Vmlld01vZGVsPlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKG1vZGVsLCBpZGVudGlmaWNhdGlvbjogUGx1Z2luSWRlbnRpZmljYXRpb25WaWV3TW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgc3VwZXIobW9kZWwsIGlkZW50aWZpY2F0aW9uKVxyXG5cclxuICAgICAgICB0aGlzLk1heGltdW1EYXRhc2V0QWdlID0ga28ub2JzZXJ2YWJsZTxudW1iZXI+KG1vZGVsLk1heGltdW1EYXRhc2V0QWdlKVxyXG4gICAgICAgIHRoaXMuRGF0YVBvcnRTZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8RGF0YVBvcnRWaWV3TW9kZWw+KClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgRXh0ZW5kTW9kZWwobW9kZWw6IGFueSlcclxuICAgIHtcclxuICAgICAgICBzdXBlci5FeHRlbmRNb2RlbChtb2RlbClcclxuXHJcbiAgICAgICAgbW9kZWwuTWF4aW11bURhdGFzZXRBZ2UgPSA8bnVtYmVyPk51bWJlci5wYXJzZUludCg8YW55PnRoaXMuTWF4aW11bURhdGFzZXRBZ2UoKSlcclxuICAgIH1cclxufSIsImFic3RyYWN0IGNsYXNzIEV4dGVuZGVkRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlIGV4dGVuZHMgRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlXHJcbntcclxuICAgIHB1YmxpYyBNb2R1bGVUb0RhdGFQb3J0TWFwOiBLbm9ja291dE9ic2VydmFibGVBcnJheTxPYnNlcnZhYmxlR3JvdXA8RGF0YVBvcnRWaWV3TW9kZWw+PlxyXG4gICAgcHVibGljIE9uZURhc01vZHVsZVNlbGVjdG9yOiBLbm9ja291dE9ic2VydmFibGU8T25lRGFzTW9kdWxlU2VsZWN0b3JWaWV3TW9kZWw+XHJcblxyXG4gICAgY29uc3RydWN0b3IobW9kZWwsIGlkZW50aWZpY2F0aW9uOiBQbHVnaW5JZGVudGlmaWNhdGlvblZpZXdNb2RlbCwgb25lRGFzTW9kdWxlU2VsZWN0b3I6IE9uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHN1cGVyKG1vZGVsLCBpZGVudGlmaWNhdGlvbilcclxuXHJcbiAgICAgICAgdGhpcy5Nb2R1bGVUb0RhdGFQb3J0TWFwID0ga28ub2JzZXJ2YWJsZUFycmF5KClcclxuICAgICAgICB0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yID0ga28ub2JzZXJ2YWJsZTxPbmVEYXNNb2R1bGVTZWxlY3RvclZpZXdNb2RlbD4ob25lRGFzTW9kdWxlU2VsZWN0b3IpXHJcblxyXG4gICAgICAgIGlmICh0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yKCkuT25Nb2R1bGVTZXRDaGFuZ2VkLnN1YnNjcmliZSgoc2VuZGVyLCBhcmdzKSA9PlxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICB0aGlzLlVwZGF0ZURhdGFQb3J0U2V0KClcclxuICAgICAgICAgICAgfSlcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIGFzeW5jIEluaXRpYWxpemVBc3luYygpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5VcGRhdGVEYXRhUG9ydFNldCgpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFVwZGF0ZURhdGFQb3J0U2V0KClcclxuICAgIHtcclxuICAgICAgICBsZXQgaW5kZXg6IG51bWJlclxyXG4gICAgICAgIGxldCBtb2R1bGVUb0RhdGFQb3J0TWFwOiBPYnNlcnZhYmxlR3JvdXA8RGF0YVBvcnRWaWV3TW9kZWw+W11cclxuXHJcbiAgICAgICAgbW9kdWxlVG9EYXRhUG9ydE1hcCA9IFtdXHJcblxyXG4gICAgICAgIC8vIGlucHV0c1xyXG4gICAgICAgIGluZGV4ID0gMFxyXG5cclxuICAgICAgICBtb2R1bGVUb0RhdGFQb3J0TWFwID0gbW9kdWxlVG9EYXRhUG9ydE1hcC5jb25jYXQodGhpcy5PbmVEYXNNb2R1bGVTZWxlY3RvcigpLk1vZHVsZVNldCgpLmZpbHRlcihvbmVEYXNNb2R1bGUgPT4gb25lRGFzTW9kdWxlLkRhdGFEaXJlY3Rpb24oKSA9PT0gRGF0YURpcmVjdGlvbkVudW0uSW5wdXQpLm1hcChvbmVEYXNNb2R1bGUgPT5cclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGxldCBncm91cDogT2JzZXJ2YWJsZUdyb3VwPERhdGFQb3J0Vmlld01vZGVsPlxyXG5cclxuICAgICAgICAgICAgZ3JvdXAgPSBuZXcgT2JzZXJ2YWJsZUdyb3VwPERhdGFQb3J0Vmlld01vZGVsPihvbmVEYXNNb2R1bGUuVG9TdHJpbmcoKSwgdGhpcy5DcmVhdGVEYXRhUG9ydFNldChvbmVEYXNNb2R1bGUsIGluZGV4KSlcclxuICAgICAgICAgICAgaW5kZXggKz0gb25lRGFzTW9kdWxlLlNpemUoKTtcclxuXHJcbiAgICAgICAgICAgIHJldHVybiBncm91cFxyXG4gICAgICAgIH0pKVxyXG5cclxuICAgICAgICAvLyBvdXRwdXRzXHJcbiAgICAgICAgaW5kZXggPSAwXHJcblxyXG4gICAgICAgIG1vZHVsZVRvRGF0YVBvcnRNYXAgPSBtb2R1bGVUb0RhdGFQb3J0TWFwLmNvbmNhdCh0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yKCkuTW9kdWxlU2V0KCkuZmlsdGVyKG9uZURhc01vZHVsZSA9PiBvbmVEYXNNb2R1bGUuRGF0YURpcmVjdGlvbigpID09PSBEYXRhRGlyZWN0aW9uRW51bS5PdXRwdXQpLm1hcChvbmVEYXNNb2R1bGUgPT5cclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGxldCBncm91cDogT2JzZXJ2YWJsZUdyb3VwPERhdGFQb3J0Vmlld01vZGVsPlxyXG5cclxuICAgICAgICAgICAgZ3JvdXAgPSBuZXcgT2JzZXJ2YWJsZUdyb3VwPERhdGFQb3J0Vmlld01vZGVsPihvbmVEYXNNb2R1bGUuVG9TdHJpbmcoKSwgdGhpcy5DcmVhdGVEYXRhUG9ydFNldChvbmVEYXNNb2R1bGUsIGluZGV4KSlcclxuICAgICAgICAgICAgaW5kZXggKz0gb25lRGFzTW9kdWxlLlNpemUoKTtcclxuXHJcbiAgICAgICAgICAgIHJldHVybiBncm91cFxyXG4gICAgICAgIH0pKVxyXG5cclxuICAgICAgICB0aGlzLk1vZHVsZVRvRGF0YVBvcnRNYXAobW9kdWxlVG9EYXRhUG9ydE1hcClcclxuICAgICAgICB0aGlzLkRhdGFQb3J0U2V0KE1hcE1hbnkodGhpcy5Nb2R1bGVUb0RhdGFQb3J0TWFwKCksIGdyb3VwID0+IGdyb3VwLk1lbWJlcnMoKSkpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIENyZWF0ZURhdGFQb3J0U2V0KG9uZURhc01vZHVsZTogT25lRGFzTW9kdWxlVmlld01vZGVsLCBpbmRleDogbnVtYmVyKVxyXG4gICAge1xyXG4gICAgICAgIGxldCBwcmVmaXg6IHN0cmluZ1xyXG5cclxuICAgICAgICBzd2l0Y2ggKG9uZURhc01vZHVsZS5EYXRhRGlyZWN0aW9uKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLklucHV0OlxyXG4gICAgICAgICAgICAgICAgcHJlZml4ID0gXCJJbnB1dFwiXHJcbiAgICAgICAgICAgICAgICBicmVha1xyXG5cclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5PdXRwdXQ6XHJcbiAgICAgICAgICAgICAgICBwcmVmaXggPSBcIk91dHB1dFwiXHJcbiAgICAgICAgICAgICAgICBicmVha1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmV0dXJuIEFycmF5LmZyb20obmV3IEFycmF5KG9uZURhc01vZHVsZS5TaXplKCkpLCAoeCwgaSkgPT4gXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICByZXR1cm4ge1xyXG4gICAgICAgICAgICAgICAgTmFtZTogPHN0cmluZz5wcmVmaXggKyBcIiBcIiArIChpbmRleCArIGkpLFxyXG4gICAgICAgICAgICAgICAgRGF0YVR5cGU6IDxPbmVEYXNEYXRhVHlwZUVudW0+b25lRGFzTW9kdWxlLkRhdGFUeXBlKCksXHJcbiAgICAgICAgICAgICAgICBEYXRhRGlyZWN0aW9uOiA8RGF0YURpcmVjdGlvbkVudW0+b25lRGFzTW9kdWxlLkRhdGFEaXJlY3Rpb24oKVxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfSkubWFwKGRhdGFQb3J0TW9kZWwgPT4gbmV3IERhdGFQb3J0Vmlld01vZGVsKGRhdGFQb3J0TW9kZWwsIHRoaXMpKVxyXG4gICAgfVxyXG59IiwiLy8vIDxyZWZlcmVuY2UgcGF0aD1cIlBsdWdpblZpZXdNb2RlbEJhc2UudHNcIi8+XHJcblxyXG5hYnN0cmFjdCBjbGFzcyBEYXRhV3JpdGVyVmlld01vZGVsQmFzZSBleHRlbmRzIFBsdWdpblZpZXdNb2RlbEJhc2Vcclxue1xyXG4gICAgcHVibGljIHJlYWRvbmx5IEZpbGVHcmFudWxhcml0eTogS25vY2tvdXRPYnNlcnZhYmxlPEZpbGVHcmFudWxhcml0eUVudW0+XHJcblxyXG4gICAgY29uc3RydWN0b3IobW9kZWwsIGlkZW50aWZpY2F0aW9uOiBQbHVnaW5JZGVudGlmaWNhdGlvblZpZXdNb2RlbClcclxuICAgIHtcclxuICAgICAgICBzdXBlcihtb2RlbCwgaWRlbnRpZmljYXRpb24pXHJcblxyXG4gICAgICAgIHRoaXMuRmlsZUdyYW51bGFyaXR5ID0ga28ub2JzZXJ2YWJsZTxGaWxlR3JhbnVsYXJpdHlFbnVtPihtb2RlbC5GaWxlR3JhbnVsYXJpdHkpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEV4dGVuZE1vZGVsKG1vZGVsOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgc3VwZXIuRXh0ZW5kTW9kZWwobW9kZWwpXHJcblxyXG4gICAgICAgIG1vZGVsLkZpbGVHcmFudWxhcml0eSA9IDxGaWxlR3JhbnVsYXJpdHlFbnVtPnRoaXMuRmlsZUdyYW51bGFyaXR5KClcclxuICAgIH1cclxufSIsImNsYXNzIFBsdWdpbkRlc2NyaXB0aW9uVmlld01vZGVsXHJcbntcclxuICAgIHB1YmxpYyBQcm9kdWN0VmVyc2lvbjogbnVtYmVyXHJcbiAgICBwdWJsaWMgSWQ6IHN0cmluZ1xyXG4gICAgcHVibGljIEluc3RhbmNlSWQ6IG51bWJlclxyXG4gICAgcHVibGljIElzRW5hYmxlZDogS25vY2tvdXRPYnNlcnZhYmxlPGJvb2xlYW4+XHJcblxyXG4gICAgY29uc3RydWN0b3IocGx1Z2luRGVzY3JpcHRpb25Nb2RlbDogYW55KVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuUHJvZHVjdFZlcnNpb24gPSBwbHVnaW5EZXNjcmlwdGlvbk1vZGVsLlByb2R1Y3RWZXJzaW9uXHJcbiAgICAgICAgdGhpcy5JZCA9IHBsdWdpbkRlc2NyaXB0aW9uTW9kZWwuSWRcclxuICAgICAgICB0aGlzLkluc3RhbmNlSWQgPSBwbHVnaW5EZXNjcmlwdGlvbk1vZGVsLkluc3RhbmNlSWRcclxuICAgICAgICB0aGlzLklzRW5hYmxlZCA9IGtvLm9ic2VydmFibGU8Ym9vbGVhbj4ocGx1Z2luRGVzY3JpcHRpb25Nb2RlbC5Jc0VuYWJsZWQpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvTW9kZWwoKVxyXG4gICAge1xyXG4gICAgICAgIHZhciBtb2RlbDogYW55ID0ge1xyXG4gICAgICAgICAgICBQcm9kdWN0VmVyc2lvbjogPG51bWJlcj50aGlzLlByb2R1Y3RWZXJzaW9uLFxyXG4gICAgICAgICAgICBJZDogPHN0cmluZz50aGlzLklkLFxyXG4gICAgICAgICAgICBJbnN0YW5jZUlkOiA8bnVtYmVyPnRoaXMuSW5zdGFuY2VJZCxcclxuICAgICAgICAgICAgSXNFbmFibGVkOiA8Ym9vbGVhbj50aGlzLklzRW5hYmxlZCgpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gbW9kZWxcclxuICAgIH1cclxufSIsImNsYXNzIFBsdWdpbklkZW50aWZpY2F0aW9uVmlld01vZGVsXHJcbntcclxuICAgIHB1YmxpYyBQcm9kdWN0VmVyc2lvbjogc3RyaW5nXHJcbiAgICBwdWJsaWMgSWQ6IHN0cmluZ1xyXG4gICAgcHVibGljIE5hbWU6IHN0cmluZ1xyXG4gICAgcHVibGljIERlc2NyaXB0aW9uOiBzdHJpbmdcclxuICAgIHB1YmxpYyBWaWV3UmVzb3VyY2VOYW1lOiBzdHJpbmdcclxuICAgIHB1YmxpYyBWaWV3TW9kZWxSZXNvdXJjZU5hbWU6IHN0cmluZ1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKHBsdWdpbklkZW50aWZpY2F0aW9uTW9kZWw6IGFueSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLlByb2R1Y3RWZXJzaW9uID0gcGx1Z2luSWRlbnRpZmljYXRpb25Nb2RlbC5Qcm9kdWN0VmVyc2lvblxyXG4gICAgICAgIHRoaXMuSWQgPSBwbHVnaW5JZGVudGlmaWNhdGlvbk1vZGVsLklkXHJcbiAgICAgICAgdGhpcy5OYW1lID0gcGx1Z2luSWRlbnRpZmljYXRpb25Nb2RlbC5OYW1lXHJcbiAgICAgICAgdGhpcy5EZXNjcmlwdGlvbiA9IHBsdWdpbklkZW50aWZpY2F0aW9uTW9kZWwuRGVzY3JpcHRpb25cclxuICAgICAgICB0aGlzLlZpZXdSZXNvdXJjZU5hbWUgPSBwbHVnaW5JZGVudGlmaWNhdGlvbk1vZGVsLlZpZXdSZXNvdXJjZU5hbWVcclxuICAgICAgICB0aGlzLlZpZXdNb2RlbFJlc291cmNlTmFtZSA9IHBsdWdpbklkZW50aWZpY2F0aW9uTW9kZWwuVmlld01vZGVsUmVzb3VyY2VOYW1lXHJcbiAgICB9XHJcbn0iXX0=