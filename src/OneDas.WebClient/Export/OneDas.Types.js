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
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiT25lRGFzLlR5cGVzLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vT25lRGFzLlR5cGVzL0RhdGFEaXJlY3Rpb25FbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL0VuZGlhbm5lc3NFbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL0ZpbGVHcmFudWxhcml0eUVudW0udHMiLCIuLi9PbmVEYXMuVHlwZXMvT25lRGFzRGF0YVR5cGVFbnVtLnRzIiwiLi4vT25lRGFzLlR5cGVzL09uZURhc1N0YXRlRW51bS50cyIsIi4uL09uZURhcy5UeXBlcy9TYW1wbGVSYXRlRW51bS50cyIsIi4uL0NvcmUvQWN0aW9uUmVxdWVzdC50cyIsIi4uL0NvcmUvQWN0aW9uUmVzcG9uc2UudHMiLCIuLi9Db3JlL0V2ZW50RGlzcGF0Y2hlci50cyIsIi4uL0NvcmUvSUV2ZW50LnRzIiwiLi4vQ29yZS9PbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtLnRzIiwiLi4vTW9kZWxzL0J1ZmZlclJlcXVlc3RNb2RlbC50cyIsIi4uL01vZGVscy9DaGFubmVsSHViTW9kZWwudHMiLCIuLi9Nb2RlbHMvT25lRGFzTW9kdWxlTW9kZWwudHMiLCIuLi9Nb2RlbHMvVHJhbnNmZXJGdW5jdGlvbk1vZGVsLnRzIiwiLi4vU3RhdGljL0Nvbm5lY3Rpb25NYW5hZ2VyLnRzIiwiLi4vU3RhdGljL0VudW1lcmF0aW9uSGVscGVyLnRzIiwiLi4vU3RhdGljL0Vycm9yTWVzc2FnZS50cyIsIi4uL1N0YXRpYy9IZWxwZXIudHMiLCIuLi9TdGF0aWMvUGx1Z2luRmFjdG9yeS50cyIsIi4uL1N0YXRpYy9QbHVnaW5IaXZlLnRzIiwiLi4vVmlld01vZGVscy9Db3JlL0NoYW5uZWxIdWJWaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL0NvcmUvT25lRGFzTW9kdWxlVmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9Db3JlL09uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9Db3JlL1RyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL1BsdWdpbi9CdWZmZXJSZXF1ZXN0Vmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vRGF0YVBvcnRWaWV3TW9kZWwudHMiLCIuLi9WaWV3TW9kZWxzL1BsdWdpbi9QbHVnaW5WaWV3TW9kZWxCYXNlLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vRGF0YUdhdGV3YXlWaWV3TW9kZWxCYXNlLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vRXh0ZW5kZWREYXRhR2F0ZXdheVZpZXdNb2RlbEJhc2UudHMiLCIuLi9WaWV3TW9kZWxzL1BsdWdpbi9EYXRhV3JpdGVyVmlld01vZGVsQmFzZS50cyIsIi4uL1ZpZXdNb2RlbHMvUGx1Z2luL1BsdWdpbkRlc2NyaXB0aW9uVmlld01vZGVsLnRzIiwiLi4vVmlld01vZGVscy9QbHVnaW4vUGx1Z2luSWRlbnRpZmljYXRpb25WaWV3TW9kZWwudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsSUFBSyxpQkFJSjtBQUpELFdBQUssaUJBQWlCO0lBRWxCLDJEQUFTLENBQUE7SUFDVCw2REFBVSxDQUFBO0FBQ2QsQ0FBQyxFQUpJLGlCQUFpQixLQUFqQixpQkFBaUIsUUFJckI7QUNKRCxJQUFLLGNBSUo7QUFKRCxXQUFLLGNBQWM7SUFFZixtRUFBZ0IsQ0FBQTtJQUNoQiw2REFBYSxDQUFBO0FBQ2pCLENBQUMsRUFKSSxjQUFjLEtBQWQsY0FBYyxRQUlsQjtBQ0pELElBQUssbUJBTUo7QUFORCxXQUFLLG1CQUFtQjtJQUVwQixzRUFBYSxDQUFBO0lBQ2IseUVBQWUsQ0FBQTtJQUNmLGdFQUFXLENBQUE7SUFDWCwrREFBVyxDQUFBO0FBQ2YsQ0FBQyxFQU5JLG1CQUFtQixLQUFuQixtQkFBbUIsUUFNdkI7QUNORCxJQUFLLGtCQVdKO0FBWEQsV0FBSyxrQkFBa0I7SUFFbkIsaUVBQWUsQ0FBQTtJQUNmLCtEQUFhLENBQUE7SUFDYiw2REFBWSxDQUFB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QUFXLElBQUksQ0FBQyxTQUFTLEVBQUU7U0FDdkMsQ0FBQTtRQUVELE1BQU0sQ0FBQyxLQUFLLENBQUE7SUFDaEIsQ0FBQztDQUNKO0FDN0JEO0lBU0ksWUFBWSx5QkFBOEI7UUFFdEMsSUFBSSxDQUFDLGNBQWMsR0FBRyx5QkFBeUIsQ0FBQyxjQUFjLENBQUE7UUFDOUQsSUFBSSxDQUFDLEVBQUUsR0FBRyx5QkFBeUIsQ0FBQyxFQUFFLENBQUE7UUFDdEMsSUFBSSxDQUFDLElBQUksR0FBRyx5QkFBeUIsQ0FBQyxJQUFJLENBQUE7UUFDMUMsSUFBSSxDQUFDLFdBQVcsR0FBRyx5QkFBeUIsQ0FBQyxXQUFXLENBQUE7UUFDeEQsSUFBSSxDQUFDLGdCQUFnQixHQUFHLHlCQUF5QixDQUFDLGdCQUFnQixDQUFBO1FBQ2xFLElBQUksQ0FBQyxxQkFBcUIsR0FBRyx5QkFBeUIsQ0FBQyxxQkFBcUIsQ0FBQTtJQUNoRixDQUFDO0NBQ0oiLCJzb3VyY2VzQ29udGVudCI6WyJlbnVtIERhdGFEaXJlY3Rpb25FbnVtXHJcbntcclxuICAgIElucHV0ID0gMSxcclxuICAgIE91dHB1dCA9IDJcclxufSIsImVudW0gRW5kaWFubmVzc0VudW1cclxue1xyXG4gICAgTGl0dGxlRW5kaWFuID0gMSxcclxuICAgIEJpZ0VuZGlhbiA9IDJcclxufSIsImVudW0gRmlsZUdyYW51bGFyaXR5RW51bVxyXG57XHJcbiAgICBNaW51dGVfMSA9IDYwLFxyXG4gICAgTWludXRlXzEwID0gNjAwLFxyXG4gICAgSG91ciA9IDM2MDAsXHJcbiAgICBEYXkgPSA4NjQwMFxyXG59IiwiZW51bSBPbmVEYXNEYXRhVHlwZUVudW1cclxue1xyXG4gICAgQk9PTEVBTiA9IDB4MDA4LFxyXG4gICAgVUlOVDggPSAweDEwOCxcclxuICAgIElOVDggPSAweDIwOCxcclxuICAgIFVJTlQxNiA9IDB4MTEwLFxyXG4gICAgSU5UMTYgPSAweDIxMCxcclxuICAgIFVJTlQzMiA9IDB4MTIwLFxyXG4gICAgSU5UMzIgPSAweDIyMCxcclxuICAgIEZMT0FUMzIgPSAweDMyMCxcclxuICAgIEZMT0FUNjQgPSAweDM0MFxyXG59IiwiZW51bSBPbmVEYXNTdGF0ZUVudW1cclxue1xyXG4gICAgRXJyb3IgPSAxLFxyXG4gICAgSW5pdGlhbGl6YXRpb24gPSAyLFxyXG4gICAgVW5jb25maWd1cmVkID0gMyxcclxuICAgIEFwcGx5Q29uZmlndXJhdGlvbiA9IDUsXHJcbiAgICBSZWFkeSA9IDYsXHJcbiAgICBSdW4gPSA3XHJcbn0iLCJlbnVtIFNhbXBsZVJhdGVFbnVtXHJcbntcclxuICAgIFNhbXBsZVJhdGVfMTAwID0gMSxcclxuICAgIFNhbXBsZVJhdGVfMjUgPSA0LFxyXG4gICAgU2FtcGxlUmF0ZV81ID0gMjAsXHJcbiAgICBTYW1wbGVSYXRlXzEgPSAxMDBcclxufSIsImNsYXNzIEFjdGlvblJlcXVlc3Rcclxue1xyXG4gICAgcHVibGljIHJlYWRvbmx5IFBsdWdpbklkOiBzdHJpbmdcclxuICAgIHB1YmxpYyByZWFkb25seSBJbnN0YW5jZUlkOiBudW1iZXJcclxuICAgIHB1YmxpYyByZWFkb25seSBNZXRob2ROYW1lOiBzdHJpbmdcclxuICAgIHB1YmxpYyByZWFkb25seSBEYXRhOiBhbnlcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihwbHVnaW5JZDogc3RyaW5nLCBpbnN0YW5jZUlkOiBudW1iZXIsIG1ldGhvZE5hbWU6IHN0cmluZywgZGF0YTogYW55KVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuUGx1Z2luSWQgPSBwbHVnaW5JZDtcclxuICAgICAgICB0aGlzLkluc3RhbmNlSWQgPSBpbnN0YW5jZUlkO1xyXG4gICAgICAgIHRoaXMuTWV0aG9kTmFtZSA9IG1ldGhvZE5hbWU7XHJcbiAgICAgICAgdGhpcy5EYXRhID0gZGF0YTtcclxuICAgIH1cclxufSIsImNsYXNzIEFjdGlvblJlc3BvbnNlXHJcbntcclxuICAgIHB1YmxpYyBEYXRhOiBhbnlcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihkYXRhOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5EYXRhID0gZGF0YTtcclxuICAgIH1cclxufSIsImNsYXNzIEV2ZW50RGlzcGF0Y2hlcjxUU2VuZGVyLCBUQXJncz4gaW1wbGVtZW50cyBJRXZlbnQ8VFNlbmRlciwgVEFyZ3M+XHJcbntcclxuICAgIHByaXZhdGUgX3N1YnNjcmlwdGlvbnM6IEFycmF5PChzZW5kZXI6IFRTZW5kZXIsIGFyZ3M6IFRBcmdzKSA9PiB2b2lkPiA9IG5ldyBBcnJheTwoc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncykgPT4gdm9pZD4oKTtcclxuXHJcbiAgICBzdWJzY3JpYmUoZm46IChzZW5kZXI6IFRTZW5kZXIsIGFyZ3M6IFRBcmdzKSA9PiB2b2lkKTogdm9pZFxyXG4gICAge1xyXG4gICAgICAgIGlmIChmbilcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuX3N1YnNjcmlwdGlvbnMucHVzaChmbik7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHVuc3Vic2NyaWJlKGZuOiAoc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncykgPT4gdm9pZCk6IHZvaWRcclxuICAgIHtcclxuICAgICAgICBsZXQgaSA9IHRoaXMuX3N1YnNjcmlwdGlvbnMuaW5kZXhPZihmbik7XHJcblxyXG4gICAgICAgIGlmIChpID4gLTEpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLl9zdWJzY3JpcHRpb25zLnNwbGljZShpLCAxKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgZGlzcGF0Y2goc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncyk6IHZvaWRcclxuICAgIHtcclxuICAgICAgICBmb3IgKGxldCBoYW5kbGVyIG9mIHRoaXMuX3N1YnNjcmlwdGlvbnMpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBoYW5kbGVyKHNlbmRlciwgYXJncyk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG59IiwiaW50ZXJmYWNlIElFdmVudDxUU2VuZGVyLCBUQXJncz5cclxue1xyXG4gICAgc3Vic2NyaWJlKGZuOiAoc2VuZGVyOiBUU2VuZGVyLCBhcmdzOiBUQXJncykgPT4gdm9pZCk6IHZvaWQ7XHJcbiAgICB1bnN1YnNjcmliZShmbjogKHNlbmRlcjogVFNlbmRlciwgYXJnczogVEFyZ3MpID0+IHZvaWQpOiB2b2lkO1xyXG59IiwiZW51bSBPbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtXHJcbntcclxuICAgIER1cGxleCA9IDEsXHJcbiAgICBJbnB1dE9ubHkgPSAyLFxyXG4gICAgT3V0cHV0T25seSA9IDMsXHJcbn0iLCJjbGFzcyBCdWZmZXJSZXF1ZXN0TW9kZWxcclxue1xyXG4gICAgcHVibGljIFNhbXBsZVJhdGU6IFNhbXBsZVJhdGVFbnVtXHJcbiAgICBwdWJsaWMgR3JvdXBGaWx0ZXI6IHN0cmluZ1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKHNhbXBsZVJhdGU6IFNhbXBsZVJhdGVFbnVtLCBncm91cEZpbHRlcjogc3RyaW5nKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuU2FtcGxlUmF0ZSA9IHNhbXBsZVJhdGU7XHJcbiAgICAgICAgdGhpcy5Hcm91cEZpbHRlciA9IGdyb3VwRmlsdGVyO1xyXG4gICAgfVxyXG59IiwiY2xhc3MgQ2hhbm5lbEh1Yk1vZGVsXHJcbntcclxuICAgIHB1YmxpYyBOYW1lOiBzdHJpbmdcclxuICAgIHB1YmxpYyBHcm91cDogc3RyaW5nXHJcbiAgICBwdWJsaWMgRGF0YVR5cGU6IE9uZURhc0RhdGFUeXBlRW51bVxyXG4gICAgcHVibGljIEd1aWQ6IHN0cmluZ1xyXG4gICAgcHVibGljIENyZWF0aW9uRGF0ZVRpbWU6IHN0cmluZ1xyXG4gICAgcHVibGljIFVuaXQ6IHN0cmluZ1xyXG4gICAgcHVibGljIFRyYW5zZmVyRnVuY3Rpb25TZXQ6IGFueVtdXHJcblxyXG4gICAgcHVibGljIFNlcmlhbGl6ZXJEYXRhSW5wdXRJZDogc3RyaW5nXHJcbiAgICBwdWJsaWMgU2VyaWFsaXplckRhdGFPdXRwdXRJZFNldDogc3RyaW5nW11cclxuXHJcbiAgICBjb25zdHJ1Y3RvcihuYW1lOiBzdHJpbmcsIGdyb3VwOiBzdHJpbmcsIGRhdGFUeXBlOiBPbmVEYXNEYXRhVHlwZUVudW0pXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5OYW1lID0gbmFtZTtcclxuICAgICAgICB0aGlzLkdyb3VwID0gZ3JvdXA7XHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZSA9IGRhdGFUeXBlO1xyXG4gICAgICAgIHRoaXMuR3VpZCA9IEd1aWQuTmV3R3VpZCgpXHJcbiAgICAgICAgdGhpcy5DcmVhdGlvbkRhdGVUaW1lID0gbmV3IERhdGUoKS50b0lTT1N0cmluZygpXHJcbiAgICAgICAgdGhpcy5Vbml0ID0gXCJcIlxyXG4gICAgICAgIHRoaXMuVHJhbnNmZXJGdW5jdGlvblNldCA9IFtdXHJcblxyXG4gICAgICAgIHRoaXMuU2VyaWFsaXplckRhdGFJbnB1dElkID0gXCJcIlxyXG4gICAgICAgIHRoaXMuU2VyaWFsaXplckRhdGFPdXRwdXRJZFNldCA9IFtdXHJcbiAgICB9XHJcbn0iLCJjbGFzcyBPbmVEYXNNb2R1bGVNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgRGF0YVR5cGU6IE9uZURhc0RhdGFUeXBlRW51bVxyXG4gICAgcHVibGljIERhdGFEaXJlY3Rpb246IERhdGFEaXJlY3Rpb25FbnVtXHJcbiAgICBwdWJsaWMgRW5kaWFubmVzczogRW5kaWFubmVzc0VudW1cclxuICAgIHB1YmxpYyBTaXplOiBudW1iZXJcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihkYXRhVHlwZTogT25lRGFzRGF0YVR5cGVFbnVtLCBkYXRhRGlyZWN0aW9uOiBEYXRhRGlyZWN0aW9uRW51bSwgZW5kaWFubmVzczogRW5kaWFubmVzc0VudW0sIHNpemU6IG51bWJlcilcclxuICAgIHtcclxuICAgICAgICB0aGlzLkRhdGFUeXBlID0gZGF0YVR5cGVcclxuICAgICAgICB0aGlzLkRhdGFEaXJlY3Rpb24gPSBkYXRhRGlyZWN0aW9uXHJcbiAgICAgICAgdGhpcy5FbmRpYW5uZXNzID0gZW5kaWFubmVzc1xyXG4gICAgICAgIHRoaXMuU2l6ZSA9IHNpemVcclxuICAgIH1cclxufVxyXG4iLCJjbGFzcyBUcmFuc2ZlckZ1bmN0aW9uTW9kZWxcclxue1xyXG4gICAgcHVibGljIERhdGVUaW1lOiBzdHJpbmdcclxuICAgIHB1YmxpYyBUeXBlOiBzdHJpbmdcclxuICAgIHB1YmxpYyBPcHRpb246IHN0cmluZ1xyXG4gICAgcHVibGljIEFyZ3VtZW50OiBzdHJpbmdcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihkYXRlVGltZTogc3RyaW5nLCB0eXBlOiBzdHJpbmcsIG9wdGlvbjogc3RyaW5nLCBhcmd1bWVudDogc3RyaW5nKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuRGF0ZVRpbWUgPSBkYXRlVGltZVxyXG4gICAgICAgIHRoaXMuVHlwZSA9IHR5cGVcclxuICAgICAgICB0aGlzLk9wdGlvbiA9IG9wdGlvblxyXG4gICAgICAgIHRoaXMuQXJndW1lbnQgPSBhcmd1bWVudFxyXG4gICAgfVxyXG59IiwiZGVjbGFyZSB2YXIgc2lnbmFsUjogYW55XHJcblxyXG5jbGFzcyBDb25uZWN0aW9uTWFuYWdlclxyXG57XHJcbiAgICBwdWJsaWMgc3RhdGljIFdlYkNsaWVudEh1YjogYW55IC8vIGltcHJvdmU6IHVzZSB0eXBpbmdzXHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBJbml0aWFsaXplKGVuYWJsZUxvZ2dpbmc6IGJvb2xlYW4pXHJcbiAgICB7XHJcbiAgICAgICAgQ29ubmVjdGlvbk1hbmFnZXIuV2ViQ2xpZW50SHViID0gbmV3IHNpZ25hbFIuSHViQ29ubmVjdGlvbignL3dlYmNsaWVudGh1YicpO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgSW52b2tlV2ViQ2xpZW50SHViID0gYXN5bmMobWV0aG9kTmFtZTogc3RyaW5nLCAuLi5hcmdzOiBhbnlbXSkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gYXdhaXQgUHJvbWlzZS5yZXNvbHZlKENvbm5lY3Rpb25NYW5hZ2VyLldlYkNsaWVudEh1Yi5pbnZva2UobWV0aG9kTmFtZSwgLi4uYXJncykpXHJcbiAgICB9XHJcbn1cclxuIiwiY2xhc3MgRW51bWVyYXRpb25IZWxwZXJcclxue1xyXG4gICAgcHVibGljIHN0YXRpYyBEZXNjcmlwdGlvbjogeyBbaW5kZXg6IHN0cmluZ106IHN0cmluZyB9ID0ge31cclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIEdldEVudW1Mb2NhbGl6YXRpb24gPSAodHlwZU5hbWU6IHN0cmluZywgdmFsdWUpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdmFyIGtleTogc3RyaW5nID0gZXZhbCh0eXBlTmFtZSArIFwiW1wiICsgdmFsdWUgKyBcIl1cIilcclxuICAgICAgICByZXR1cm4gZXZhbChcIkVudW1lcmF0aW9uSGVscGVyLkRlc2NyaXB0aW9uWydcIiArIHR5cGVOYW1lICsgXCJfXCIgKyBrZXkgKyBcIiddXCIpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBHZXRFbnVtVmFsdWVzID0gKHR5cGVOYW1lOiBzdHJpbmcpID0+XHJcbiAgICB7XHJcbiAgICAgICAgbGV0IHZhbHVlczogYW55W11cclxuXHJcbiAgICAgICAgdmFsdWVzID0gZXZhbChcIk9iamVjdC5rZXlzKFwiICsgdHlwZU5hbWUgKyBcIikubWFwKGtleSA9PiBcIiArIHR5cGVOYW1lICsgXCJba2V5XSlcIilcclxuICAgICAgICByZXR1cm4gPG51bWJlcltdPnZhbHVlcy5maWx0ZXIodmFsdWUgPT4gdHlwZW9mICh2YWx1ZSkgPT09IFwibnVtYmVyXCIpXHJcbiAgICB9XHJcbn0iLCJsZXQgRXJyb3JNZXNzYWdlOiB7IFtpbmRleDogc3RyaW5nXTogc3RyaW5nIH0gPSB7fVxyXG5FcnJvck1lc3NhZ2VbXCJNdWx0aU1hcHBpbmdFZGl0b3JWaWV3TW9kZWxfSW52YWxpZFNldHRpbmdzXCJdID0gXCJPbmUgb3IgbW9yZSBzZXR0aW5ncyBhcmUgaW52YWxpZC5cIlxyXG5FcnJvck1lc3NhZ2VbXCJNdWx0aU1hcHBpbmdFZGl0b3JWaWV3TW9kZWxfV3JvbmdEYXRhVHlwZVwiXSA9IFwiT25lIG9yIG1vcmUgdmFyaWFibGUtY2hhbm5lbCBkYXRhIHR5cGUgY29tYmluYXRpb25zIGFyZSBpbnZhbGlkLlwiXHJcbkVycm9yTWVzc2FnZVtcIlByb2plY3RfQ2hhbm5lbEFscmVhZHlFeGlzdHNcIl0gPSBcIkEgY2hhbm5lbCB3aXRoIHRoYXQgbmFtZSBhbHJlYWR5IGV4aXN0cy5cIlxyXG5FcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0lzQWxyZWFkeUluR3JvdXBcIl0gPSBcIlRoZSBjaGFubmVsIGlzIGFscmVhZHkgYSBtZW1iZXIgb2YgdGhpcyBncm91cC5cIlxyXG5FcnJvck1lc3NhZ2VbXCJQcm9qZWN0X0ludmFsaWRDaGFyYWN0ZXJzXCJdID0gXCJVc2UgQS1aLCBhLXosIDAtOSBvciBfLlwiXHJcbkVycm9yTWVzc2FnZVtcIlByb2plY3RfSW52YWxpZExlYWRpbmdDaGFyYWN0ZXJcIl0gPSBcIlVzZSBBLVogb3IgYS16IGFzIGZpcnN0IGNoYXJhY3Rlci5cIlxyXG5FcnJvck1lc3NhZ2VbXCJQcm9qZWN0X05hbWVFbXB0eVwiXSA9IFwiVGhlIG5hbWUgbXVzdCBub3QgYmUgZW1wdHkuXCJcclxuIiwiY2xhc3MgT2JzZXJ2YWJsZUdyb3VwPFQ+XHJcbntcclxuICAgIEtleTogc3RyaW5nO1xyXG4gICAgTWVtYmVyczogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8VD5cclxuXHJcbiAgICBjb25zdHJ1Y3RvcihrZXk6IHN0cmluZywgbWVtYmVyczogVFtdID0gbmV3IEFycmF5PFQ+KCkpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5LZXkgPSBrZXlcclxuICAgICAgICB0aGlzLk1lbWJlcnMgPSBrby5vYnNlcnZhYmxlQXJyYXkobWVtYmVycylcclxuICAgIH1cclxufVxyXG5cclxuZnVuY3Rpb24gT2JzZXJ2YWJsZUdyb3VwQnk8VD4obGlzdDogVFtdLCBuYW1lR2V0dGVyOiAoeDogVCkgPT4gc3RyaW5nLCBncm91cE5hbWVHZXR0ZXI6ICh4OiBUKSA9PiBzdHJpbmcsIGZpbHRlcjogc3RyaW5nKTogT2JzZXJ2YWJsZUdyb3VwPFQ+W11cclxue1xyXG4gICAgbGV0IHJlc3VsdDogT2JzZXJ2YWJsZUdyb3VwPFQ+W11cclxuICAgIGxldCByZWdFeHA6IFJlZ0V4cFxyXG5cclxuICAgIHJlc3VsdCA9IFtdXHJcbiAgICByZWdFeHAgPSBuZXcgUmVnRXhwKGZpbHRlciwgXCJpXCIpXHJcblxyXG4gICAgbGlzdC5mb3JFYWNoKGVsZW1lbnQgPT5cclxuICAgIHtcclxuICAgICAgICBpZiAocmVnRXhwLnRlc3QobmFtZUdldHRlcihlbGVtZW50KSkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBncm91cE5hbWVHZXR0ZXIoZWxlbWVudCkuc3BsaXQoXCJcXG5cIikuZm9yRWFjaChncm91cE5hbWUgPT5cclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgQWRkVG9Hcm91cGVkQXJyYXkoZWxlbWVudCwgZ3JvdXBOYW1lLCByZXN1bHQpXHJcbiAgICAgICAgICAgIH0pXHJcbiAgICAgICAgfVxyXG4gICAgfSlcclxuXHJcbiAgICByZXR1cm4gcmVzdWx0XHJcbn1cclxuXHJcbmZ1bmN0aW9uIEFkZFRvR3JvdXBlZEFycmF5PFQ+KGl0ZW06IFQsIGdyb3VwTmFtZTogc3RyaW5nLCBvYnNlcnZhYmxlR3JvdXBTZXQ6IE9ic2VydmFibGVHcm91cDxUPltdKVxyXG57XHJcbiAgICBsZXQgZ3JvdXA6IE9ic2VydmFibGVHcm91cDxUPlxyXG5cclxuICAgIGdyb3VwID0gb2JzZXJ2YWJsZUdyb3VwU2V0LmZpbmQoeSA9PiB5LktleSA9PT0gZ3JvdXBOYW1lKVxyXG5cclxuICAgIGlmICghZ3JvdXApXHJcbiAgICB7XHJcbiAgICAgICAgZ3JvdXAgPSBuZXcgT2JzZXJ2YWJsZUdyb3VwPFQ+KGdyb3VwTmFtZSlcclxuICAgICAgICBvYnNlcnZhYmxlR3JvdXBTZXQucHVzaChncm91cClcclxuICAgIH1cclxuXHJcbiAgICBncm91cC5NZW1iZXJzLnB1c2goaXRlbSlcclxufVxyXG5cclxuLy9mdW5jdGlvbiBBZGRUb0dyb3VwZWRPYnNlcnZhYmxlQXJyYXk8VD4oaXRlbTogVCwgZ3JvdXBOYW1lOiBzdHJpbmcsIG9ic2VydmFibGVHcm91cFNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8T2JzZXJ2YWJsZUdyb3VwPFQ+PilcclxuLy97XHJcbi8vICAgIGxldCBncm91cDogT2JzZXJ2YWJsZUdyb3VwPFQ+XHJcblxyXG4vLyAgICBncm91cCA9IG9ic2VydmFibGVHcm91cFNldCgpLmZpbmQoeSA9PiB5LktleSA9PT0gZ3JvdXBOYW1lKVxyXG5cclxuLy8gICAgaWYgKCFncm91cClcclxuLy8gICAge1xyXG4vLyAgICAgICAgZ3JvdXAgPSBuZXcgT2JzZXJ2YWJsZUdyb3VwPFQ+KGdyb3VwTmFtZSlcclxuLy8gICAgICAgIG9ic2VydmFibGVHcm91cFNldC5wdXNoKGdyb3VwKVxyXG4vLyAgICB9XHJcblxyXG4vLyAgICBncm91cC5NZW1iZXJzLnB1c2goaXRlbSlcclxuLy99XHJcblxyXG4vL2Z1bmN0aW9uIFJlbW92ZUZyb21Hcm91cGVkT2JzZXJ2YWJsZUFycmF5PFQ+KGl0ZW06IFQsIG9ic2VydmFibGVHcm91cFNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8T2JzZXJ2YWJsZUdyb3VwPFQ+PilcclxuLy97XHJcbi8vICAgIHZhciBncm91cDogT2JzZXJ2YWJsZUdyb3VwPFQ+XHJcblxyXG4vLyAgICBvYnNlcnZhYmxlR3JvdXBTZXQoKS5zb21lKHggPT5cclxuLy8gICAge1xyXG4vLyAgICAgICAgaWYgKHguTWVtYmVycygpLmluZGV4T2YoaXRlbSkgPiAtMSlcclxuLy8gICAgICAgIHtcclxuLy8gICAgICAgICAgICBncm91cCA9IHhcclxuXHJcbi8vICAgICAgICAgICAgcmV0dXJuIHRydWVcclxuLy8gICAgICAgIH1cclxuXHJcbi8vICAgICAgICByZXR1cm4gZmFsc2VcclxuLy8gICAgfSlcclxuXHJcbi8vICAgIGlmIChncm91cClcclxuLy8gICAge1xyXG4vLyAgICAgICAgZ3JvdXAuTWVtYmVycy5yZW1vdmUoaXRlbSlcclxuXHJcbi8vICAgICAgICBpZiAoZ3JvdXAuTWVtYmVycygpLmxlbmd0aCA9PT0gMClcclxuLy8gICAgICAgIHtcclxuLy8gICAgICAgICAgICBvYnNlcnZhYmxlR3JvdXBTZXQucmVtb3ZlKGdyb3VwKVxyXG4vLyAgICAgICAgfVxyXG5cclxuLy8gICAgICAgIHJldHVybiB0cnVlXHJcbi8vICAgIH1cclxuXHJcbi8vICAgIHJldHVybiBmYWxzZVxyXG4vL31cclxuXHJcbi8vZnVuY3Rpb24gVXBkYXRlR3JvdXBlZE9ic2VydmFibGVBcnJheTxUPihpdGVtOiBULCBncm91cE5hbWU6IHN0cmluZywgb2JzZXJ2YWJsZUdyb3VwU2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxPYnNlcnZhYmxlR3JvdXA8VD4+KVxyXG4vL3tcclxuLy8gICAgUmVtb3ZlRnJvbUdyb3VwZWRPYnNlcnZhYmxlQXJyYXkoaXRlbSwgb2JzZXJ2YWJsZUdyb3VwU2V0KVxyXG4vLyAgICBBZGRUb0dyb3VwZWRPYnNlcnZhYmxlQXJyYXkoaXRlbSwgZ3JvdXBOYW1lLCBvYnNlcnZhYmxlR3JvdXBTZXQpXHJcbi8vfVxyXG5cclxuZnVuY3Rpb24gTWFwTWFueTxUQXJyYXlFbGVtZW50LCBUU2VsZWN0PihhcnJheTogVEFycmF5RWxlbWVudFtdLCBtYXBGdW5jOiAoaXRlbTogVEFycmF5RWxlbWVudCkgPT4gVFNlbGVjdFtdKTogVFNlbGVjdFtdXHJcbntcclxuICAgIHJldHVybiBhcnJheS5yZWR1Y2UoKHByZXZpb3VzLCBjdXJyZW50LCBpKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiBwcmV2aW91cy5jb25jYXQobWFwRnVuYyhjdXJyZW50KSk7XHJcbiAgICB9LCA8VFNlbGVjdFtdPltdKTtcclxufVxyXG5cclxuY2xhc3MgR3VpZFxyXG57XHJcbiAgICBzdGF0aWMgTmV3R3VpZCgpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuICd4eHh4eHh4eC14eHh4LTR4eHgteXh4eC14eHh4eHh4eHh4eHgnLnJlcGxhY2UoL1t4eV0vZywgZnVuY3Rpb24gKGMpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB2YXIgciA9IE1hdGgucmFuZG9tKCkgKiAxNiB8IDBcclxuICAgICAgICAgICAgdmFyIHYgPSBjID09PSAneCcgPyByIDogKHIgJiAweDMgfCAweDgpXHJcblxyXG4gICAgICAgICAgICByZXR1cm4gdi50b1N0cmluZygxNilcclxuICAgICAgICB9KVxyXG4gICAgfVxyXG59XHJcblxyXG5sZXQgQ2hlY2tOYW1pbmdDb252ZW50aW9uID0gKHZhbHVlOiBzdHJpbmcpID0+XHJcbntcclxuICAgIHZhciByZWdFeHA6IGFueVxyXG5cclxuICAgIGlmICh2YWx1ZS5sZW5ndGggPT09IDApXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHsgSGFzRXJyb3I6IHRydWUsIEVycm9yRGVzY3JpcHRpb246IEVycm9yTWVzc2FnZVtcIlByb2plY3RfTmFtZUVtcHR5XCJdIH1cclxuICAgIH1cclxuXHJcbiAgICByZWdFeHAgPSBuZXcgUmVnRXhwKFwiW15BLVphLXowLTlfXVwiKVxyXG5cclxuICAgIGlmIChyZWdFeHAudGVzdCh2YWx1ZSkpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHsgSGFzRXJyb3I6IHRydWUsIEVycm9yRGVzY3JpcHRpb246IEVycm9yTWVzc2FnZVtcIlByb2plY3RfSW52YWxpZENoYXJhY3RlcnNcIl0gfVxyXG4gICAgfVxyXG5cclxuICAgIHJlZ0V4cCA9IG5ldyBSZWdFeHAoXCJeWzAtOV9dXCIpXHJcblxyXG4gICAgaWYgKHJlZ0V4cC50ZXN0KHZhbHVlKSlcclxuICAgIHtcclxuICAgICAgICByZXR1cm4geyBIYXNFcnJvcjogdHJ1ZSwgRXJyb3JEZXNjcmlwdGlvbjogRXJyb3JNZXNzYWdlW1wiUHJvamVjdF9JbnZhbGlkTGVhZGluZ0NoYXJhY3RlclwiXSB9XHJcbiAgICB9XHJcblxyXG4gICAgcmV0dXJuIHtcclxuICAgICAgICBIYXNFcnJvcjogZmFsc2UsXHJcbiAgICAgICAgRXJyb3JEZXNjcmlwdGlvbjogXCJcIlxyXG4gICAgfVxyXG59IiwiY2xhc3MgUGx1Z2luRmFjdG9yeVxyXG57XHJcbiAgICBwdWJsaWMgc3RhdGljIENyZWF0ZVBsdWdpblZpZXdNb2RlbEFzeW5jID0gYXN5bmMgKHBsdWdpblR5cGU6IHN0cmluZywgcGx1Z2luTW9kZWw6IGFueSkgPT5cclxuICAgIHtcclxuICAgICAgICBsZXQgcGx1Z2luSWRlbnRpZmljYXRpb246IFBsdWdpbklkZW50aWZpY2F0aW9uVmlld01vZGVsXHJcbiAgICAgICAgbGV0IHBsdWdpblZpZXdNb2RlbDogUGx1Z2luVmlld01vZGVsQmFzZVxyXG4gICAgICAgIGxldCBwbHVnaW5WaWV3TW9kZWxSYXc6IHN0cmluZ1xyXG5cclxuICAgICAgICBwbHVnaW5JZGVudGlmaWNhdGlvbiA9IFBsdWdpbkhpdmUuRmluZFBsdWdpbklkZW50aWZpY2F0aW9uKHBsdWdpblR5cGUsIHBsdWdpbk1vZGVsLkRlc2NyaXB0aW9uLklkKVxyXG5cclxuICAgICAgICBpZiAocGx1Z2luSWRlbnRpZmljYXRpb24pXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBwbHVnaW5WaWV3TW9kZWxSYXcgPSBhd2FpdCBDb25uZWN0aW9uTWFuYWdlci5JbnZva2VXZWJDbGllbnRIdWIoXCJHZXRQbHVnaW5TdHJpbmdSZXNvdXJjZVwiLCBwbHVnaW5Nb2RlbC5EZXNjcmlwdGlvbi5JZCwgcGx1Z2luSWRlbnRpZmljYXRpb24uVmlld01vZGVsUmVzb3VyY2VOYW1lKVxyXG4gICAgICAgICAgICBwbHVnaW5WaWV3TW9kZWwgPSA8UGx1Z2luVmlld01vZGVsQmFzZT5uZXcgRnVuY3Rpb24ocGx1Z2luVmlld01vZGVsUmF3ICsgXCI7IHJldHVybiBWaWV3TW9kZWxDb25zdHJ1Y3RvclwiKSgpKHBsdWdpbk1vZGVsLCBwbHVnaW5JZGVudGlmaWNhdGlvbilcclxuXHJcbiAgICAgICAgICAgIHJldHVybiBwbHVnaW5WaWV3TW9kZWxcclxuICAgICAgICB9XHJcbiAgICAgICAgZWxzZVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhyb3cgbmV3IEVycm9yKFwiTm8gY29ycmVzcG9uZGluZyBwbHVnaW4gZGVzY3JpcHRpb24gZm91bmQuXCIpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG59IiwiY2xhc3MgUGx1Z2luSGl2ZVxyXG57XHJcbiAgICAvLyBmaWVsZHNcclxuICAgIHB1YmxpYyBzdGF0aWMgUGx1Z2luSWRlbnRpZmljYXRpb25TZXQ6IE1hcDxzdHJpbmcsIFBsdWdpbklkZW50aWZpY2F0aW9uVmlld01vZGVsW10+XHJcblxyXG4gICAgLy8gY29uc3RydWN0b3JzXHJcbiAgICBzdGF0aWMgSW5pdGlhbGl6ZSA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgUGx1Z2luSGl2ZS5QbHVnaW5JZGVudGlmaWNhdGlvblNldCA9IG5ldyBNYXA8c3RyaW5nLCBQbHVnaW5JZGVudGlmaWNhdGlvblZpZXdNb2RlbFtdPigpXHJcbiAgICB9ICAgXHJcblxyXG4gICAgc3RhdGljIEZpbmRQbHVnaW5JZGVudGlmaWNhdGlvbiA9IChwbHVnaW5UeXBlTmFtZTogc3RyaW5nLCBwbHVnaW5JZDogc3RyaW5nKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiBQbHVnaW5IaXZlLlBsdWdpbklkZW50aWZpY2F0aW9uU2V0LmdldChwbHVnaW5UeXBlTmFtZSkuZmluZChwbHVnaW5JZGVudGlmaWNhdGlvbiA9PiBwbHVnaW5JZGVudGlmaWNhdGlvbi5JZCA9PT0gcGx1Z2luSWQpO1xyXG4gICAgfVxyXG59IiwiY2xhc3MgQ2hhbm5lbEh1YlZpZXdNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgTmFtZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBHcm91cDogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyByZWFkb25seSBEYXRhVHlwZTogS25vY2tvdXRPYnNlcnZhYmxlPE9uZURhc0RhdGFUeXBlRW51bT5cclxuICAgIHB1YmxpYyByZWFkb25seSBHdWlkOiBzdHJpbmdcclxuICAgIHB1YmxpYyByZWFkb25seSBDcmVhdGlvbkRhdGVUaW1lOiBzdHJpbmdcclxuICAgIHB1YmxpYyByZWFkb25seSBVbml0OiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IFRyYW5zZmVyRnVuY3Rpb25TZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PFRyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWw+XHJcbiAgICBwdWJsaWMgU2VsZWN0ZWRUcmFuc2ZlckZ1bmN0aW9uOiBLbm9ja291dE9ic2VydmFibGU8VHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbD5cclxuICAgIHB1YmxpYyBFdmFsdWF0ZWRUcmFuc2ZlckZ1bmN0aW9uU2V0OiAoKHZhbHVlOiBudW1iZXIpID0+IG51bWJlcilbXVxyXG4gICAgcHVibGljIElzU2VsZWN0ZWQ6IEtub2Nrb3V0T2JzZXJ2YWJsZTxib29sZWFuPlxyXG5cclxuICAgIHB1YmxpYyByZWFkb25seSBBc3NvY2lhdGVkRGF0YUlucHV0OiBLbm9ja291dE9ic2VydmFibGU8RGF0YVBvcnRWaWV3TW9kZWw+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgQXNzb2NpYXRlZERhdGFPdXRwdXRTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PERhdGFQb3J0Vmlld01vZGVsPlxyXG5cclxuICAgIHByaXZhdGUgQXNzb2NpYXRlZERhdGFJbnB1dElkOiBzdHJpbmdcclxuICAgIHByaXZhdGUgQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldDogc3RyaW5nW11cclxuXHJcbiAgICBjb25zdHJ1Y3RvcihjaGFubmVsSHViTW9kZWw6IENoYW5uZWxIdWJNb2RlbClcclxuICAgIHtcclxuICAgICAgICB0aGlzLk5hbWUgPSBrby5vYnNlcnZhYmxlPHN0cmluZz4oY2hhbm5lbEh1Yk1vZGVsLk5hbWUpXHJcbiAgICAgICAgdGhpcy5Hcm91cCA9IGtvLm9ic2VydmFibGU8c3RyaW5nPihjaGFubmVsSHViTW9kZWwuR3JvdXApXHJcbiAgICAgICAgdGhpcy5EYXRhVHlwZSA9IGtvLm9ic2VydmFibGU8T25lRGFzRGF0YVR5cGVFbnVtPihjaGFubmVsSHViTW9kZWwuRGF0YVR5cGUpXHJcbiAgICAgICAgdGhpcy5HdWlkID0gY2hhbm5lbEh1Yk1vZGVsLkd1aWRcclxuICAgICAgICB0aGlzLkNyZWF0aW9uRGF0ZVRpbWUgPSBjaGFubmVsSHViTW9kZWwuQ3JlYXRpb25EYXRlVGltZVxyXG4gICAgICAgIHRoaXMuVW5pdCA9IGtvLm9ic2VydmFibGU8c3RyaW5nPihjaGFubmVsSHViTW9kZWwuVW5pdClcclxuICAgICAgICB0aGlzLlRyYW5zZmVyRnVuY3Rpb25TZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8VHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbD4oY2hhbm5lbEh1Yk1vZGVsLlRyYW5zZmVyRnVuY3Rpb25TZXQubWFwKHRmID0+IG5ldyBUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsKHRmKSkpXHJcbiAgICAgICAgdGhpcy5TZWxlY3RlZFRyYW5zZmVyRnVuY3Rpb24gPSBrby5vYnNlcnZhYmxlPFRyYW5zZmVyRnVuY3Rpb25WaWV3TW9kZWw+KHRoaXMuQ3JlYXRlRGVmYXVsdFRyYW5zZmVyRnVuY3Rpb24oKSlcclxuICAgICAgICB0aGlzLklzU2VsZWN0ZWQgPSBrby5vYnNlcnZhYmxlPGJvb2xlYW4+KGZhbHNlKVxyXG5cclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXQgPSBrby5vYnNlcnZhYmxlPERhdGFQb3J0Vmlld01vZGVsPigpXHJcbiAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dFNldCA9IGtvLm9ic2VydmFibGVBcnJheTxEYXRhUG9ydFZpZXdNb2RlbD4oKVxyXG5cclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXRJZCA9IGNoYW5uZWxIdWJNb2RlbC5TZXJpYWxpemVyRGF0YUlucHV0SWRcclxuICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQgPSBjaGFubmVsSHViTW9kZWwuU2VyaWFsaXplckRhdGFPdXRwdXRJZFNldFxyXG4gICAgICAgIHRoaXMuRXZhbHVhdGVkVHJhbnNmZXJGdW5jdGlvblNldCA9IFtdXHJcbiAgICB9XHJcblxyXG4gICAgLy8gbWV0aG9kc1xyXG4gICAgcHVibGljIEdldFRyYW5zZm9ybWVkVmFsdWUgPSAodmFsdWU6IGFueSk6IHN0cmluZyA9PiBcclxuICAgIHtcclxuICAgICAgICBpZiAodmFsdWUgPT09IFwiTmFOXCIpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB2YWx1ZSA9IE5hTlxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5FdmFsdWF0ZWRUcmFuc2ZlckZ1bmN0aW9uU2V0LmZvckVhY2godGYgPT4gdmFsdWUgPSB0Zih2YWx1ZSkpXHJcblxyXG4gICAgICAgIHJldHVybiB2YWx1ZVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgQ3JlYXRlRGVmYXVsdFRyYW5zZmVyRnVuY3Rpb24gPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiBuZXcgVHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbChuZXcgVHJhbnNmZXJGdW5jdGlvbk1vZGVsKFwiMDAwMS0wMS0wMVQwMDowMDowMFpcIiwgXCJwb2x5bm9taWFsXCIsIFwicGVybWFuZW50XCIsIFwiMTswXCIpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBJc0Fzc29jaWF0aW9uQWxsb3dlZChkYXRhUG9ydDogRGF0YVBvcnRWaWV3TW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIChkYXRhUG9ydC5EYXRhVHlwZSAmIDB4ZmYpID09ICh0aGlzLkRhdGFUeXBlKCkgJiAweGZmKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBVcGRhdGVBc3NvY2lhdGlvbiA9IChkYXRhUG9ydDogRGF0YVBvcnRWaWV3TW9kZWwpID0+XHJcbiAgICB7XHJcbiAgICAgICAgc3dpdGNoIChkYXRhUG9ydC5EYXRhRGlyZWN0aW9uKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dDpcclxuXHJcbiAgICAgICAgICAgICAgICB0aGlzLlJlc2V0QXNzb2NpYXRpb24oZmFsc2UsIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dCgpKVxyXG4gICAgICAgICAgICAgICAgYnJlYWtcclxuXHJcbiAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uT3V0cHV0OlxyXG5cclxuICAgICAgICAgICAgICAgIHRoaXMuUmVzZXRBc3NvY2lhdGlvbihmYWxzZSwgZGF0YVBvcnQpXHJcbiAgICAgICAgICAgICAgICBicmVha1xyXG5cclxuICAgICAgICAgICAgZGVmYXVsdDpcclxuICAgICAgICAgICAgICAgIHRocm93IG5ldyBFcnJvcihcIk5vdCBpbXBsZW1lbnRlZC5cIik7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0aGlzLlNldEFzc29jaWF0aW9uKGRhdGFQb3J0KVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBTZXRBc3NvY2lhdGlvbihkYXRhUG9ydDogRGF0YVBvcnRWaWV3TW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgZGF0YVBvcnQuQXNzb2NpYXRlZENoYW5uZWxIdWJTZXQucHVzaCh0aGlzKVxyXG5cclxuICAgICAgICBzd2l0Y2ggKGRhdGFQb3J0LkRhdGFEaXJlY3Rpb24pXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLklucHV0OlxyXG5cclxuICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dChkYXRhUG9ydClcclxuICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dElkID0gZGF0YVBvcnQuVG9GdWxsUXVhbGlmaWVkSWRlbnRpZmllcigpXHJcblxyXG4gICAgICAgICAgICAgICAgYnJlYWtcclxuXHJcbiAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uT3V0cHV0OlxyXG5cclxuICAgICAgICAgICAgICAgIGxldCBkYXRhT3V0cHV0SWQgPSBkYXRhUG9ydC5Ub0Z1bGxRdWFsaWZpZWRJZGVudGlmaWVyKClcclxuXHJcbiAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0U2V0LnB1c2goZGF0YVBvcnQpXHJcblxyXG4gICAgICAgICAgICAgICAgaWYgKHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldC5pbmRleE9mKGRhdGFPdXRwdXRJZCkgPCAwKVxyXG4gICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldC5wdXNoKGRhdGFQb3J0LlRvRnVsbFF1YWxpZmllZElkZW50aWZpZXIoKSlcclxuICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICBicmVha1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgUmVzZXRBc3NvY2lhdGlvbihtYWludGFpbldlYWtSZWZlcmVuY2U6IGJvb2xlYW4sIC4uLmRhdGFQb3J0U2V0OiBEYXRhUG9ydFZpZXdNb2RlbFtdKVxyXG4gICAge1xyXG4gICAgICAgIGRhdGFQb3J0U2V0LmZvckVhY2goZGF0YVBvcnQgPT5cclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGlmICghZGF0YVBvcnQpXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIHJldHVyblxyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICBkYXRhUG9ydC5Bc3NvY2lhdGVkQ2hhbm5lbEh1YlNldC5yZW1vdmUodGhpcylcclxuXHJcbiAgICAgICAgICAgIHN3aXRjaCAoZGF0YVBvcnQuRGF0YURpcmVjdGlvbilcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dDpcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0KG51bGwpXHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmICghbWFpbnRhaW5XZWFrUmVmZXJlbmNlKVxyXG4gICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0SWQgPSBudWxsXHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBicmVha1xyXG5cclxuICAgICAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uT3V0cHV0OlxyXG5cclxuICAgICAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0U2V0LnJlbW92ZShkYXRhUG9ydClcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCFtYWludGFpbldlYWtSZWZlcmVuY2UpXHJcbiAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBsZXQgaW5kZXg6IG51bWJlciA9IHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldC5pbmRleE9mKGRhdGFQb3J0LlRvRnVsbFF1YWxpZmllZElkZW50aWZpZXIoKSlcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmIChpbmRleCA+IC0xKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXQuc3BsaWNlKGluZGV4LCAxKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9ICAgXHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBicmVha1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgUmVzZXRBbGxBc3NvY2lhdGlvbnMobWFpbnRhaW5XZWFrUmVmZXJlbmNlOiBib29sZWFuKVxyXG4gICAge1xyXG4gICAgICAgIGlmICh0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXQoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuUmVzZXRBc3NvY2lhdGlvbihtYWludGFpbldlYWtSZWZlcmVuY2UsIHRoaXMuQXNzb2NpYXRlZERhdGFJbnB1dCgpKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5SZXNldEFzc29jaWF0aW9uKG1haW50YWluV2Vha1JlZmVyZW5jZSwgLi4udGhpcy5Bc3NvY2lhdGVkRGF0YU91dHB1dFNldCgpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBHZXRBc3NvY2lhdGVkRGF0YUlucHV0SWQgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLkFzc29jaWF0ZWREYXRhSW5wdXRJZDtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgR2V0QXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuQXNzb2NpYXRlZERhdGFPdXRwdXRJZFNldDtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHtcclxuICAgICAgICAgICAgTmFtZTogPHN0cmluZz50aGlzLk5hbWUoKSxcclxuICAgICAgICAgICAgR3JvdXA6IDxzdHJpbmc+dGhpcy5Hcm91cCgpLFxyXG4gICAgICAgICAgICBEYXRhVHlwZTogPE9uZURhc0RhdGFUeXBlRW51bT50aGlzLkRhdGFUeXBlKCksXHJcbiAgICAgICAgICAgIEd1aWQ6IDxzdHJpbmc+dGhpcy5HdWlkLFxyXG4gICAgICAgICAgICBDcmVhdGlvbkRhdGVUaW1lOiA8c3RyaW5nPnRoaXMuQ3JlYXRpb25EYXRlVGltZSxcclxuICAgICAgICAgICAgVW5pdDogPHN0cmluZz50aGlzLlVuaXQoKSxcclxuICAgICAgICAgICAgVHJhbnNmZXJGdW5jdGlvblNldDogPFRyYW5zZmVyRnVuY3Rpb25Nb2RlbFtdPnRoaXMuVHJhbnNmZXJGdW5jdGlvblNldCgpLm1hcCh0ZiA9PiB0Zi5Ub01vZGVsKCkpLFxyXG4gICAgICAgICAgICBTZXJpYWxpemVyRGF0YUlucHV0SWQ6IDxzdHJpbmc+dGhpcy5Bc3NvY2lhdGVkRGF0YUlucHV0SWQsXHJcbiAgICAgICAgICAgIFNlcmlhbGl6ZXJEYXRhT3V0cHV0SWRTZXQ6IDxzdHJpbmdbXT50aGlzLkFzc29jaWF0ZWREYXRhT3V0cHV0SWRTZXRcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEFkZFRyYW5zZmVyRnVuY3Rpb24gPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuVHJhbnNmZXJGdW5jdGlvblNldC5wdXNoKHRoaXMuU2VsZWN0ZWRUcmFuc2ZlckZ1bmN0aW9uKCkpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIERlbGV0ZVRyYW5zZmVyRnVuY3Rpb24gPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuVHJhbnNmZXJGdW5jdGlvblNldC5yZW1vdmUodGhpcy5TZWxlY3RlZFRyYW5zZmVyRnVuY3Rpb24oKSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgTmV3VHJhbnNmZXJGdW5jdGlvbiA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5TZWxlY3RlZFRyYW5zZmVyRnVuY3Rpb24odGhpcy5DcmVhdGVEZWZhdWx0VHJhbnNmZXJGdW5jdGlvbigpKVxyXG4gICAgfVxyXG5cclxuICAgIC8vIGNvbW1hbmRzXHJcbiAgICBwdWJsaWMgU2VsZWN0VHJhbnNmZXJGdW5jdGlvbiA9ICh0cmFuc2ZlckZ1bmN0aW9uOiBUcmFuc2ZlckZ1bmN0aW9uVmlld01vZGVsKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuU2VsZWN0ZWRUcmFuc2ZlckZ1bmN0aW9uKHRyYW5zZmVyRnVuY3Rpb24pXHJcbiAgICB9XHJcbn0iLCJjbGFzcyBPbmVEYXNNb2R1bGVWaWV3TW9kZWxcclxue1xyXG4gICAgcHVibGljIERhdGFUeXBlOiBLbm9ja291dE9ic2VydmFibGU8T25lRGFzRGF0YVR5cGVFbnVtPlxyXG4gICAgcHVibGljIERhdGFEaXJlY3Rpb246IEtub2Nrb3V0T2JzZXJ2YWJsZTxEYXRhRGlyZWN0aW9uRW51bT5cclxuICAgIHB1YmxpYyBFbmRpYW5uZXNzOiBLbm9ja291dE9ic2VydmFibGU8RW5kaWFubmVzc0VudW0+XHJcbiAgICBwdWJsaWMgU2l6ZTogS25vY2tvdXRPYnNlcnZhYmxlPG51bWJlcj5cclxuICAgIHB1YmxpYyBNYXhTaXplOiBLbm9ja291dE9ic2VydmFibGU8bnVtYmVyPlxyXG4gICAgcHVibGljIEVycm9yTWVzc2FnZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBIYXNFcnJvcjogS25vY2tvdXRDb21wdXRlZDxib29sZWFuPlxyXG5cclxuICAgIHB1YmxpYyBEYXRhVHlwZVNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8T25lRGFzRGF0YVR5cGVFbnVtPlxyXG5cclxuICAgIHByaXZhdGUgX29uUHJvcGVydHlDaGFuZ2VkOiBFdmVudERpc3BhdGNoZXI8T25lRGFzTW9kdWxlVmlld01vZGVsLCBhbnk+XHJcbiAgICBwcm90ZWN0ZWQgX21vZGVsOiBhbnlcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihvbmVEYXNNb2R1bGVNb2RlbDogT25lRGFzTW9kdWxlTW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5fbW9kZWwgPSBvbmVEYXNNb2R1bGVNb2RlbFxyXG5cclxuICAgICAgICB0aGlzLkRhdGFUeXBlID0ga28ub2JzZXJ2YWJsZTxPbmVEYXNEYXRhVHlwZUVudW0+KG9uZURhc01vZHVsZU1vZGVsLkRhdGFUeXBlKVxyXG4gICAgICAgIHRoaXMuRGF0YURpcmVjdGlvbiA9IGtvLm9ic2VydmFibGU8RGF0YURpcmVjdGlvbkVudW0+KG9uZURhc01vZHVsZU1vZGVsLkRhdGFEaXJlY3Rpb24pXHJcbiAgICAgICAgdGhpcy5FbmRpYW5uZXNzID0ga28ub2JzZXJ2YWJsZTxFbmRpYW5uZXNzRW51bT4ob25lRGFzTW9kdWxlTW9kZWwuRW5kaWFubmVzcylcclxuICAgICAgICB0aGlzLlNpemUgPSBrby5vYnNlcnZhYmxlPG51bWJlcj4ob25lRGFzTW9kdWxlTW9kZWwuU2l6ZSlcclxuICAgICAgICB0aGlzLk1heFNpemUgPSBrby5vYnNlcnZhYmxlPG51bWJlcj4oSW5maW5pdHkpXHJcbiAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UgPSBrby5vYnNlcnZhYmxlPHN0cmluZz4oXCJcIilcclxuICAgICAgICB0aGlzLkhhc0Vycm9yID0ga28uY29tcHV0ZWQ8Ym9vbGVhbj4oKCkgPT4gdGhpcy5FcnJvck1lc3NhZ2UoKS5sZW5ndGggPiAwKVxyXG5cclxuICAgICAgICB0aGlzLkRhdGFUeXBlU2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PE9uZURhc0RhdGFUeXBlRW51bT4oRW51bWVyYXRpb25IZWxwZXIuR2V0RW51bVZhbHVlcygnT25lRGFzRGF0YVR5cGVFbnVtJykuZmlsdGVyKGRhdGFUeXBlID0+IGRhdGFUeXBlICE9PSBPbmVEYXNEYXRhVHlwZUVudW0uQk9PTEVBTikpXHJcbiAgICAgICAgdGhpcy5fb25Qcm9wZXJ0eUNoYW5nZWQgPSBuZXcgRXZlbnREaXNwYXRjaGVyPE9uZURhc01vZHVsZVZpZXdNb2RlbCwgYW55PigpO1xyXG5cclxuICAgICAgICB0aGlzLkRhdGFUeXBlLnN1YnNjcmliZShuZXdWYWx1ZSA9PiB0aGlzLk9uUHJvcGVydHlDaGFuZ2VkKCkpXHJcbiAgICAgICAgdGhpcy5EYXRhRGlyZWN0aW9uLnN1YnNjcmliZShuZXdWYWx1ZSA9PiB0aGlzLk9uUHJvcGVydHlDaGFuZ2VkKCkpXHJcbiAgICAgICAgdGhpcy5TaXplLnN1YnNjcmliZShuZXdWYWx1ZSA9PiB0aGlzLk9uUHJvcGVydHlDaGFuZ2VkKCkpXHJcbiAgICB9XHJcblxyXG4gICAgZ2V0IFByb3BlcnR5Q2hhbmdlZCgpOiBJRXZlbnQ8T25lRGFzTW9kdWxlVmlld01vZGVsLCBhbnk+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuX29uUHJvcGVydHlDaGFuZ2VkO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBPblByb3BlcnR5Q2hhbmdlZCA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5WYWxpZGF0ZSgpXHJcbiAgICAgICAgdGhpcy5fb25Qcm9wZXJ0eUNoYW5nZWQuZGlzcGF0Y2godGhpcywgbnVsbClcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgR2V0Qnl0ZUNvdW50ID0gKGJvb2xlYW5CaXRTaXplPzogbnVtYmVyKSA9PlxyXG4gICAge1xyXG4gICAgICAgIGlmIChib29sZWFuQml0U2l6ZSAmJiB0aGlzLkRhdGFUeXBlKCkgPT09IE9uZURhc0RhdGFUeXBlRW51bS5CT09MRUFOKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgYm9vbGVhbkJpdFNpemUgPSBwYXJzZUludCg8YW55PmJvb2xlYW5CaXRTaXplKVxyXG5cclxuICAgICAgICAgICAgcmV0dXJuIE1hdGguY2VpbChib29sZWFuQml0U2l6ZSAqIHRoaXMuU2l6ZSgpIC8gOCk7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIGVsc2VcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHJldHVybiAodGhpcy5EYXRhVHlwZSgpICYgMHgwRkYpIC8gOCAqIHRoaXMuU2l6ZSgpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBWYWxpZGF0ZSgpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJcIilcclxuXHJcbiAgICAgICAgaWYgKHRoaXMuU2l6ZSgpIDwgMSB8fCAoaXNGaW5pdGUodGhpcy5NYXhTaXplKCkpICYmIHRoaXMuU2l6ZSgpID4gdGhpcy5NYXhTaXplKCkpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJTaXplIG11c3QgYmUgd2l0aGluIHJhbmdlIDEuLlwiICsgdGhpcy5NYXhTaXplKCkgKyBcIi5cIilcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvU3RyaW5nKClcclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5TaXplKCkgKyBcInggXCIgKyBFbnVtZXJhdGlvbkhlbHBlci5HZXRFbnVtTG9jYWxpemF0aW9uKCdPbmVEYXNEYXRhVHlwZUVudW0nLCB0aGlzLkRhdGFUeXBlKCkpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEV4dGVuZE1vZGVsKG1vZGVsOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgLy9cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IG1vZGVsOiBhbnkgPSB7XHJcbiAgICAgICAgICAgICR0eXBlOiA8c3RyaW5nPnRoaXMuX21vZGVsLiR0eXBlLFxyXG4gICAgICAgICAgICBEYXRhVHlwZTogPE9uZURhc0RhdGFUeXBlRW51bT50aGlzLkRhdGFUeXBlKCksXHJcbiAgICAgICAgICAgIFNpemU6IDxudW1iZXI+dGhpcy5TaXplKCksXHJcbiAgICAgICAgICAgIERhdGFEaXJlY3Rpb246IDxEYXRhRGlyZWN0aW9uRW51bT50aGlzLkRhdGFEaXJlY3Rpb24oKSxcclxuICAgICAgICAgICAgRW5kaWFubmVzczogPEVuZGlhbm5lc3NFbnVtPnRoaXMuRW5kaWFubmVzcygpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0aGlzLkV4dGVuZE1vZGVsKG1vZGVsKVxyXG5cclxuICAgICAgICByZXR1cm4gbW9kZWxcclxuICAgIH1cclxufVxyXG4iLCJjbGFzcyBPbmVEYXNNb2R1bGVTZWxlY3RvclZpZXdNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgU2V0dGluZ3NUZW1wbGF0ZU5hbWU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgTmV3TW9kdWxlOiBLbm9ja291dE9ic2VydmFibGU8T25lRGFzTW9kdWxlVmlld01vZGVsPiAgXHJcbiAgICBwdWJsaWMgTWF4Qnl0ZXM6IEtub2Nrb3V0T2JzZXJ2YWJsZTxudW1iZXI+XHJcbiAgICBwdWJsaWMgUmVtYWluaW5nQnl0ZXM6IEtub2Nrb3V0T2JzZXJ2YWJsZTxudW1iZXI+XHJcbiAgICBwdWJsaWMgUmVtYWluaW5nQ291bnQ6IEtub2Nrb3V0T2JzZXJ2YWJsZTxudW1iZXI+ICAgIFxyXG4gICAgcHVibGljIE1vZHVsZVNldDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8T25lRGFzTW9kdWxlVmlld01vZGVsPlxyXG4gICAgcHVibGljIEVycm9yTWVzc2FnZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBIYXNFcnJvcjogS25vY2tvdXRDb21wdXRlZDxib29sZWFuPlxyXG5cclxuICAgIHB1YmxpYyBPbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxPbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGVFbnVtPlxyXG5cclxuICAgIHByaXZhdGUgX29uTW9kdWxlU2V0Q2hhbmdlZDogRXZlbnREaXNwYXRjaGVyPE9uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsLCBPbmVEYXNNb2R1bGVWaWV3TW9kZWxbXT5cclxuXHJcbiAgICBjb25zdHJ1Y3RvcihvbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGU6IE9uZURhc01vZHVsZVNlbGVjdG9yTW9kZUVudW0sIG1vZHVsZVNldDogT25lRGFzTW9kdWxlVmlld01vZGVsW10gPSBbXSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLk9uZURhc01vZHVsZVNlbGVjdG9yTW9kZSA9IGtvLm9ic2VydmFibGU8T25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlRW51bT4ob25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlKVxyXG5cclxuICAgICAgICB0aGlzLlNldHRpbmdzVGVtcGxhdGVOYW1lID0ga28ub2JzZXJ2YWJsZShcIlByb2plY3RfT25lRGFzTW9kdWxlU2V0dGluZ3NUZW1wbGF0ZVwiKVxyXG4gICAgICAgIHRoaXMuTmV3TW9kdWxlID0ga28ub2JzZXJ2YWJsZTxPbmVEYXNNb2R1bGVWaWV3TW9kZWw+KCk7XHJcbiAgICAgICAgdGhpcy5NYXhCeXRlcyA9IGtvLm9ic2VydmFibGU8bnVtYmVyPihJbmZpbml0eSk7XHJcbiAgICAgICAgdGhpcy5SZW1haW5pbmdCeXRlcyA9IGtvLm9ic2VydmFibGU8bnVtYmVyPihOYU4pO1xyXG4gICAgICAgIHRoaXMuUmVtYWluaW5nQ291bnQgPSBrby5vYnNlcnZhYmxlPG51bWJlcj4oTmFOKTtcclxuICAgICAgICB0aGlzLk1vZHVsZVNldCA9IGtvLm9ic2VydmFibGVBcnJheTxPbmVEYXNNb2R1bGVWaWV3TW9kZWw+KG1vZHVsZVNldCk7XHJcbiAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UgPSBrby5vYnNlcnZhYmxlPHN0cmluZz4oXCJcIilcclxuICAgICAgICB0aGlzLkhhc0Vycm9yID0ga28uY29tcHV0ZWQ8Ym9vbGVhbj4oKCkgPT4gdGhpcy5FcnJvck1lc3NhZ2UoKS5sZW5ndGggPiAwKVxyXG5cclxuICAgICAgICB0aGlzLl9vbk1vZHVsZVNldENoYW5nZWQgPSBuZXcgRXZlbnREaXNwYXRjaGVyPE9uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsLCBPbmVEYXNNb2R1bGVWaWV3TW9kZWxbXT4oKTtcclxuXHJcbiAgICAgICAgdGhpcy5JbnRlcm5hbENyZWF0ZU5ld01vZHVsZSgpXHJcbiAgICAgICAgdGhpcy5JbnRlcm5hbFVwZGF0ZSgpXHJcbiAgICB9XHJcblxyXG4gICAgZ2V0IE9uTW9kdWxlU2V0Q2hhbmdlZCgpOiBJRXZlbnQ8T25lRGFzTW9kdWxlU2VsZWN0b3JWaWV3TW9kZWwsIE9uZURhc01vZHVsZVZpZXdNb2RlbFtdPlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLl9vbk1vZHVsZVNldENoYW5nZWQ7XHJcbiAgICB9XHJcbiAgICBcclxuICAgIC8vIG1ldGhvZHNcclxuICAgIHB1YmxpYyBTZXRNYXhCeXRlcyA9ICh2YWx1ZTogbnVtYmVyKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuTWF4Qnl0ZXModmFsdWUpXHJcbiAgICAgICAgdGhpcy5JbnRlcm5hbFVwZGF0ZSgpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEdldElucHV0TW9kdWxlU2V0ID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5Nb2R1bGVTZXQoKS5maWx0ZXIobW9kdWxlID0+IG1vZHVsZS5EYXRhRGlyZWN0aW9uKCkgPT09IERhdGFEaXJlY3Rpb25FbnVtLklucHV0KVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBHZXRPdXRwdXRNb2R1bGVTZXQgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHJldHVybiB0aGlzLk1vZHVsZVNldCgpLmZpbHRlcihtb2R1bGUgPT4gbW9kdWxlLkRhdGFEaXJlY3Rpb24oKSA9PT0gRGF0YURpcmVjdGlvbkVudW0uT3V0cHV0KVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuVXBkYXRlKClcclxuICAgICAgICB0aGlzLlZhbGlkYXRlKClcclxuICAgIH1cclxuXHJcbiAgICBwcm90ZWN0ZWQgVXBkYXRlKClcclxuICAgIHtcclxuICAgICAgICBsZXQgbW9kdWxlU2V0OiBPbmVEYXNNb2R1bGVWaWV3TW9kZWxbXVxyXG4gICAgICAgIGxldCByZW1haW5pbmdCeXRlczogbnVtYmVyXHJcblxyXG4gICAgICAgIHN3aXRjaCAodGhpcy5OZXdNb2R1bGUoKS5EYXRhRGlyZWN0aW9uKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBjYXNlIERhdGFEaXJlY3Rpb25FbnVtLklucHV0OlxyXG4gICAgICAgICAgICAgICAgbW9kdWxlU2V0ID0gdGhpcy5HZXRJbnB1dE1vZHVsZVNldCgpXHJcbiAgICAgICAgICAgICAgICBicmVhaztcclxuXHJcbiAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uT3V0cHV0OlxyXG4gICAgICAgICAgICAgICAgbW9kdWxlU2V0ID0gdGhpcy5HZXRPdXRwdXRNb2R1bGVTZXQoKVxyXG4gICAgICAgICAgICAgICAgYnJlYWs7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZW1haW5pbmdCeXRlcyA9IHRoaXMuTWF4Qnl0ZXMoKSAtIG1vZHVsZVNldC5tYXAob25lRGFzTW9kdWxlID0+IG9uZURhc01vZHVsZS5HZXRCeXRlQ291bnQoKSkucmVkdWNlKChwcmV2aW91c1ZhbHVlLCBjdXJyZW50VmFsdWUpID0+IHByZXZpb3VzVmFsdWUgKyBjdXJyZW50VmFsdWUsIDApXHJcblxyXG4gICAgICAgIHRoaXMuUmVtYWluaW5nQnl0ZXMocmVtYWluaW5nQnl0ZXMpXHJcbiAgICAgICAgdGhpcy5SZW1haW5pbmdDb3VudChNYXRoLmZsb29yKHRoaXMuUmVtYWluaW5nQnl0ZXMoKSAvICgodGhpcy5OZXdNb2R1bGUoKS5EYXRhVHlwZSgpICYgMHgwRkYpIC8gOCkpKVxyXG4gICAgfVxyXG5cclxuICAgIHByb3RlY3RlZCBWYWxpZGF0ZSgpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJcIilcclxuXHJcbiAgICAgICAgaWYgKHRoaXMuTmV3TW9kdWxlKCkuSGFzRXJyb3IoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiUmVzb2x2ZSBhbGwgcmVtYWluaW5nIG1vZHVsZSBlcnJvcnMgYmVmb3JlIGNvbnRpbnVpbmcuXCIpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBpZiAodGhpcy5PbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGUoKSA9PT0gT25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlRW51bS5JbnB1dE9ubHkgJiYgdGhpcy5OZXdNb2R1bGUoKS5EYXRhRGlyZWN0aW9uKCkgPT0gRGF0YURpcmVjdGlvbkVudW0uT3V0cHV0KVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJPbmx5IGlucHV0IG1vZHVsZXMgYXJlIGFsbG93ZWQuXCIpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBpZiAodGhpcy5PbmVEYXNNb2R1bGVTZWxlY3Rvck1vZGUoKSA9PT0gT25lRGFzTW9kdWxlU2VsZWN0b3JNb2RlRW51bS5PdXRwdXRPbmx5ICYmIHRoaXMuTmV3TW9kdWxlKCkuRGF0YURpcmVjdGlvbigpID09IERhdGFEaXJlY3Rpb25FbnVtLklucHV0KVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJPbmx5IG91dHB1dCBtb2R1bGVzIGFyZSBhbGxvd2VkLlwiKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaWYgKGlzRmluaXRlKHRoaXMuUmVtYWluaW5nQnl0ZXMoKSkgJiYgKHRoaXMuUmVtYWluaW5nQnl0ZXMoKSAtIHRoaXMuTmV3TW9kdWxlKCkuR2V0Qnl0ZUNvdW50KCkgPCAwKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuRXJyb3JNZXNzYWdlKFwiQnl0ZSBjb3VudCBvZiBuZXcgbW9kdWxlIGlzIHRvbyBoaWdoLlwiKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaWYgKHRoaXMuUmVtYWluaW5nQ291bnQoKSA8PSAwKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5FcnJvck1lc3NhZ2UoXCJUaGUgbWF4aW11bSBudW1iZXIgb2YgbW9kdWxlcyBpcyByZWFjaGVkLlwiKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwcm90ZWN0ZWQgQ3JlYXRlTmV3TW9kdWxlKClcclxuICAgIHtcclxuICAgICAgICBpZiAodGhpcy5OZXdNb2R1bGUoKSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHJldHVybiBuZXcgT25lRGFzTW9kdWxlVmlld01vZGVsKG5ldyBPbmVEYXNNb2R1bGVNb2RlbCh0aGlzLk5ld01vZHVsZSgpLkRhdGFUeXBlKCksIHRoaXMuTmV3TW9kdWxlKCkuRGF0YURpcmVjdGlvbigpLCB0aGlzLk5ld01vZHVsZSgpLkVuZGlhbm5lc3MoKSwgMSkpXHJcbiAgICAgICAgfVxyXG4gICAgICAgIGVsc2VcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHJldHVybiBuZXcgT25lRGFzTW9kdWxlVmlld01vZGVsKG5ldyBPbmVEYXNNb2R1bGVNb2RlbChPbmVEYXNEYXRhVHlwZUVudW0uVUlOVDE2LCBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dCwgRW5kaWFubmVzc0VudW0uTGl0dGxlRW5kaWFuLCAxKSlcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBJbnRlcm5hbENyZWF0ZU5ld01vZHVsZSgpXHJcbiAgICB7XHJcbiAgICAgICAgaWYgKHRoaXMuTmV3TW9kdWxlKCkpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB0aGlzLk5ld01vZHVsZSgpLlByb3BlcnR5Q2hhbmdlZC51bnN1YnNjcmliZSh0aGlzLk9uTW9kdWxlUHJvcGVydHlDaGFuZ2VkKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5OZXdNb2R1bGUodGhpcy5DcmVhdGVOZXdNb2R1bGUoKSlcclxuICAgICAgICB0aGlzLk5ld01vZHVsZSgpLlByb3BlcnR5Q2hhbmdlZC5zdWJzY3JpYmUodGhpcy5Pbk1vZHVsZVByb3BlcnR5Q2hhbmdlZClcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIE9uTW9kdWxlUHJvcGVydHlDaGFuZ2VkID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLkludGVybmFsVXBkYXRlKClcclxuICAgIH1cclxuXHJcbiAgICAvLyBjb21tYW5kc1xyXG4gICAgcHVibGljIEFkZE1vZHVsZSA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgbGV0IG5ld01vZHVsZTogT25lRGFzTW9kdWxlVmlld01vZGVsXHJcblxyXG4gICAgICAgIGlmICghdGhpcy5IYXNFcnJvcigpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5Nb2R1bGVTZXQucHVzaCh0aGlzLk5ld01vZHVsZSgpKVxyXG4gICAgICAgICAgICB0aGlzLkludGVybmFsQ3JlYXRlTmV3TW9kdWxlKClcclxuICAgICAgICAgICAgdGhpcy5JbnRlcm5hbFVwZGF0ZSgpXHJcbiAgICAgICAgICAgIHRoaXMuX29uTW9kdWxlU2V0Q2hhbmdlZC5kaXNwYXRjaCh0aGlzLCB0aGlzLk1vZHVsZVNldCgpKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgRGVsZXRlTW9kdWxlID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLk1vZHVsZVNldC5wb3AoKVxyXG4gICAgICAgIHRoaXMuSW50ZXJuYWxVcGRhdGUoKVxyXG4gICAgICAgIHRoaXMuX29uTW9kdWxlU2V0Q2hhbmdlZC5kaXNwYXRjaCh0aGlzLCB0aGlzLk1vZHVsZVNldCgpKVxyXG4gICAgfVxyXG59IiwiY2xhc3MgVHJhbnNmZXJGdW5jdGlvblZpZXdNb2RlbFxyXG57XHJcbiAgICBwdWJsaWMgRGF0ZVRpbWU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgVHlwZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBPcHRpb246IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgQXJndW1lbnQ6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcblxyXG4gICAgY29uc3RydWN0b3IodHJhbnNmZXJGdW5jdGlvbk1vZGVsOiBUcmFuc2ZlckZ1bmN0aW9uTW9kZWwpXHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5EYXRlVGltZSA9IGtvLm9ic2VydmFibGUodHJhbnNmZXJGdW5jdGlvbk1vZGVsLkRhdGVUaW1lKVxyXG4gICAgICAgIHRoaXMuVHlwZSA9IGtvLm9ic2VydmFibGUodHJhbnNmZXJGdW5jdGlvbk1vZGVsLlR5cGUpXHJcbiAgICAgICAgdGhpcy5PcHRpb24gPSBrby5vYnNlcnZhYmxlKHRyYW5zZmVyRnVuY3Rpb25Nb2RlbC5PcHRpb24pXHJcbiAgICAgICAgdGhpcy5Bcmd1bWVudCA9IGtvLm9ic2VydmFibGUodHJhbnNmZXJGdW5jdGlvbk1vZGVsLkFyZ3VtZW50KVxyXG4gICAgfVxyXG5cclxuICAgIC8vIG1ldGhvZHNcclxuICAgIHB1YmxpYyBUb01vZGVsKClcclxuICAgIHtcclxuICAgICAgICByZXR1cm4gbmV3IFRyYW5zZmVyRnVuY3Rpb25Nb2RlbCh0aGlzLkRhdGVUaW1lKCksIHRoaXMuVHlwZSgpLCB0aGlzLk9wdGlvbigpLCB0aGlzLkFyZ3VtZW50KCkpXHJcbiAgICB9XHJcbn0iLCJjbGFzcyBCdWZmZXJSZXF1ZXN0Vmlld01vZGVsXHJcbntcclxuICAgIHB1YmxpYyBTYW1wbGVSYXRlOiBLbm9ja291dE9ic2VydmFibGU8U2FtcGxlUmF0ZUVudW0+XHJcbiAgICBwdWJsaWMgR3JvdXBGaWx0ZXI6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcblxyXG4gICAgY29uc3RydWN0b3IobW9kZWw6IEJ1ZmZlclJlcXVlc3RNb2RlbClcclxuICAgIHtcclxuICAgICAgICB0aGlzLlNhbXBsZVJhdGUgPSBrby5vYnNlcnZhYmxlPFNhbXBsZVJhdGVFbnVtPihtb2RlbC5TYW1wbGVSYXRlKTtcclxuICAgICAgICB0aGlzLkdyb3VwRmlsdGVyID0ga28ub2JzZXJ2YWJsZTxzdHJpbmc+KG1vZGVsLkdyb3VwRmlsdGVyKTtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgVG9Nb2RlbCgpIHtcclxuICAgICAgICByZXR1cm4ge1xyXG4gICAgICAgICAgICBTYW1wbGVSYXRlOiA8U2FtcGxlUmF0ZUVudW0+dGhpcy5TYW1wbGVSYXRlKCksXHJcbiAgICAgICAgICAgIEdyb3VwRmlsdGVyOiA8c3RyaW5nPnRoaXMuR3JvdXBGaWx0ZXIoKVxyXG4gICAgICAgIH1cclxuICAgIH1cclxufSIsImNsYXNzIERhdGFQb3J0Vmlld01vZGVsXHJcbntcclxuICAgIC8vIGZpZWxkc1xyXG4gICAgcHVibGljIE5hbWU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRGF0YVR5cGU6IE9uZURhc0RhdGFUeXBlRW51bVxyXG4gICAgcHVibGljIHJlYWRvbmx5IERhdGFEaXJlY3Rpb246IERhdGFEaXJlY3Rpb25FbnVtXHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgRW5kaWFubmVzczogRW5kaWFubmVzc0VudW1cclxuXHJcbiAgICBwdWJsaWMgSXNTZWxlY3RlZDogS25vY2tvdXRPYnNlcnZhYmxlPGJvb2xlYW4+XHJcbiAgICBwdWJsaWMgQXNzb2NpYXRlZENoYW5uZWxIdWJTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PENoYW5uZWxIdWJWaWV3TW9kZWw+XHJcbiAgICBwdWJsaWMgcmVhZG9ubHkgQXNzb2NpYXRlZERhdGFHYXRld2F5OiBEYXRhR2F0ZXdheVZpZXdNb2RlbEJhc2VcclxuICAgIHB1YmxpYyByZWFkb25seSBMaXZlRGVzY3JpcHRpb246IEtub2Nrb3V0Q29tcHV0ZWQ8c3RyaW5nPlxyXG5cclxuICAgIC8vIGNvbnN0cnVjdG9yc1xyXG4gICAgY29uc3RydWN0b3IoZGF0YVBvcnRNb2RlbDogYW55LCBhc3NvY2lhdGVkRGF0YUdhdGV3YXk6IERhdGFHYXRld2F5Vmlld01vZGVsQmFzZSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLk5hbWUgPSBrby5vYnNlcnZhYmxlKGRhdGFQb3J0TW9kZWwuTmFtZSlcclxuICAgICAgICB0aGlzLkRhdGFUeXBlID0gZGF0YVBvcnRNb2RlbC5EYXRhVHlwZVxyXG4gICAgICAgIHRoaXMuRGF0YURpcmVjdGlvbiA9IGRhdGFQb3J0TW9kZWwuRGF0YURpcmVjdGlvblxyXG4gICAgICAgIHRoaXMuRW5kaWFubmVzcyA9IGRhdGFQb3J0TW9kZWwuRW5kaWFubmVzc1xyXG5cclxuICAgICAgICB0aGlzLklzU2VsZWN0ZWQgPSBrby5vYnNlcnZhYmxlPGJvb2xlYW4+KGZhbHNlKVxyXG4gICAgICAgIHRoaXMuQXNzb2NpYXRlZENoYW5uZWxIdWJTZXQgPSBrby5vYnNlcnZhYmxlQXJyYXk8Q2hhbm5lbEh1YlZpZXdNb2RlbD4oKVxyXG4gICAgICAgIHRoaXMuQXNzb2NpYXRlZERhdGFHYXRld2F5ID0gYXNzb2NpYXRlZERhdGFHYXRld2F5XHJcblxyXG4gICAgICAgIHRoaXMuTGl2ZURlc2NyaXB0aW9uID0ga28uY29tcHV0ZWQoKCkgPT5cclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGxldCByZXN1bHQ6IHN0cmluZ1xyXG5cclxuICAgICAgICAgICAgcmVzdWx0ID0gXCI8ZGl2IGNsYXNzPSd0ZXh0LWxlZnQnPlwiICsgdGhpcy5OYW1lKCkgKyBcIjwvZGl2PjxkaXYgY2xhc3M9J3RleHQtbGVmdCc+XCIgKyBFbnVtZXJhdGlvbkhlbHBlci5HZXRFbnVtTG9jYWxpemF0aW9uKCdPbmVEYXNEYXRhVHlwZUVudW0nLCB0aGlzLkRhdGFUeXBlKSArIFwiPC9kaXY+XCJcclxuXHJcbiAgICAgICAgICAgIGlmICh0aGlzLkFzc29jaWF0ZWRDaGFubmVsSHViU2V0KCkubGVuZ3RoID4gMClcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgdGhpcy5Bc3NvY2lhdGVkQ2hhbm5lbEh1YlNldCgpLmZvckVhY2goY2hhbm5lbEh1YiA9PlxyXG4gICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgIHJlc3VsdCArPSBcIjwvYnIgPjxkaXYgY2xhc3M9J3RleHQtbGVmdCc+XCIgKyBjaGFubmVsSHViLk5hbWUoKSArIFwiPC9kaXY+PGRpdiBjbGFzcz0ndGV4dC1sZWZ0Jz5cIiArIEVudW1lcmF0aW9uSGVscGVyLkdldEVudW1Mb2NhbGl6YXRpb24oJ09uZURhc0RhdGFUeXBlRW51bScsIGNoYW5uZWxIdWIuRGF0YVR5cGUoKSkgKyBcIjwvZGl2PlwiXHJcbiAgICAgICAgICAgICAgICB9KVxyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICByZXR1cm4gcmVzdWx0XHJcbiAgICAgICAgfSlcclxuICAgIH1cclxuXHJcbiAgICAvLyBtZXRob2RzXHJcbiAgICBwdWJsaWMgR2V0SWQoKTogc3RyaW5nXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuTmFtZSgpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvRnVsbFF1YWxpZmllZElkZW50aWZpZXIoKTogc3RyaW5nXHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuQXNzb2NpYXRlZERhdGFHYXRld2F5LkRlc2NyaXB0aW9uLklkICsgXCIgKFwiICsgdGhpcy5Bc3NvY2lhdGVkRGF0YUdhdGV3YXkuRGVzY3JpcHRpb24uSW5zdGFuY2VJZCArIFwiKSAvIFwiICsgdGhpcy5HZXRJZCgpO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBFeHRlbmRNb2RlbChtb2RlbDogYW55KVxyXG4gICAge1xyXG4gICAgICAgIC8vXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIFRvTW9kZWwoKVxyXG4gICAge1xyXG4gICAgICAgIGxldCBtb2RlbDogYW55ID0ge1xyXG4gICAgICAgICAgICBOYW1lOiA8c3RyaW5nPnRoaXMuTmFtZSgpLFxyXG4gICAgICAgICAgICBEYXRhVHlwZTogPE9uZURhc0RhdGFUeXBlRW51bT50aGlzLkRhdGFUeXBlLFxyXG4gICAgICAgICAgICBEYXRhRGlyZWN0aW9uOiA8RGF0YURpcmVjdGlvbkVudW0+dGhpcy5EYXRhRGlyZWN0aW9uXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0aGlzLkV4dGVuZE1vZGVsKG1vZGVsKVxyXG5cclxuICAgICAgICByZXR1cm4gbW9kZWxcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgUmVzZXRBc3NvY2lhdGlvbnMobWFpbnRhaW5XZWFrUmVmZXJlbmNlOiBib29sZWFuKVxyXG4gICAge1xyXG4gICAgICAgIGlmICh0aGlzLkFzc29jaWF0ZWRDaGFubmVsSHViU2V0KCkubGVuZ3RoID4gMClcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMuQXNzb2NpYXRlZENoYW5uZWxIdWJTZXQoKS5mb3JFYWNoKGNoYW5uZWxIdWIgPT4gY2hhbm5lbEh1Yi5SZXNldEFzc29jaWF0aW9uKG1haW50YWluV2Vha1JlZmVyZW5jZSwgdGhpcykpXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG59IiwiYWJzdHJhY3QgY2xhc3MgUGx1Z2luVmlld01vZGVsQmFzZVxyXG57XHJcbiAgICBwdWJsaWMgRGVzY3JpcHRpb246IFBsdWdpbkRlc2NyaXB0aW9uVmlld01vZGVsXHJcbiAgICBwdWJsaWMgUGx1Z2luSWRlbnRpZmljYXRpb246IFBsdWdpbklkZW50aWZpY2F0aW9uVmlld01vZGVsXHJcbiAgICBwdWJsaWMgSXNJblNldHRpbmdzTW9kZTogS25vY2tvdXRPYnNlcnZhYmxlPGJvb2xlYW4+XHJcblxyXG4gICAgcHJpdmF0ZSBfbW9kZWw6IGFueVxyXG5cclxuICAgIGNvbnN0cnVjdG9yKHBsdWdpblNldHRpbmdzTW9kZWw6IGFueSwgcGx1Z2luSWRlbnRpZmljYXRpb246IFBsdWdpbklkZW50aWZpY2F0aW9uVmlld01vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuX21vZGVsID0gcGx1Z2luU2V0dGluZ3NNb2RlbFxyXG4gICAgICAgIHRoaXMuRGVzY3JpcHRpb24gPSBuZXcgUGx1Z2luRGVzY3JpcHRpb25WaWV3TW9kZWwocGx1Z2luU2V0dGluZ3NNb2RlbC5EZXNjcmlwdGlvbilcclxuICAgICAgICB0aGlzLlBsdWdpbklkZW50aWZpY2F0aW9uID0gcGx1Z2luSWRlbnRpZmljYXRpb25cclxuICAgICAgICB0aGlzLklzSW5TZXR0aW5nc01vZGUgPSBrby5vYnNlcnZhYmxlPGJvb2xlYW4+KGZhbHNlKVxyXG4gICAgfVxyXG5cclxuICAgIC8vIG1ldGhvZHNcclxuICAgIHB1YmxpYyBhYnN0cmFjdCBhc3luYyBJbml0aWFsaXplQXN5bmMoKTogUHJvbWlzZTxhbnk+XHJcblxyXG4gICAgcHVibGljIFNlbmRBY3Rpb25SZXF1ZXN0ID0gYXN5bmMgKGluc3RhbmNlSWQ6IG51bWJlciwgbWV0aG9kTmFtZTogc3RyaW5nLCBkYXRhOiBhbnkpID0+XHJcbiAgICB7XHJcbiAgICAgICAgcmV0dXJuIDxBY3Rpb25SZXNwb25zZT4gYXdhaXQgQ29ubmVjdGlvbk1hbmFnZXIuSW52b2tlV2ViQ2xpZW50SHViKFwiUmVxdWVzdEFjdGlvblwiLCBuZXcgQWN0aW9uUmVxdWVzdCh0aGlzLkRlc2NyaXB0aW9uLklkLCBpbnN0YW5jZUlkLCBtZXRob2ROYW1lLCBkYXRhKSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgRXh0ZW5kTW9kZWwobW9kZWw6IGFueSlcclxuICAgIHtcclxuICAgICAgICAvL1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb01vZGVsKClcclxuICAgIHtcclxuICAgICAgICBsZXQgbW9kZWw6IGFueSA9IHtcclxuICAgICAgICAgICAgJHR5cGU6IDxzdHJpbmc+dGhpcy5fbW9kZWwuJHR5cGUsXHJcbiAgICAgICAgICAgIERlc2NyaXB0aW9uOiA8UGx1Z2luRGVzY3JpcHRpb25WaWV3TW9kZWw+dGhpcy5EZXNjcmlwdGlvbi5Ub01vZGVsKClcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuRXh0ZW5kTW9kZWwobW9kZWwpXHJcblxyXG4gICAgICAgIHJldHVybiBtb2RlbFxyXG4gICAgfVxyXG5cclxuICAgIC8vIGNvbW1hbmRzXHJcbiAgICBwdWJsaWMgRW5hYmxlU2V0dGluZ3NNb2RlID0gKCkgPT5cclxuICAgIHtcclxuICAgICAgICB0aGlzLklzSW5TZXR0aW5nc01vZGUodHJ1ZSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgRGlzYWJsZVNldHRpbmdzTW9kZSA9ICgpID0+XHJcbiAgICB7XHJcbiAgICAgICAgdGhpcy5Jc0luU2V0dGluZ3NNb2RlKGZhbHNlKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb2dnbGVTZXR0aW5nc01vZGUgPSAoKSA9PlxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuSXNJblNldHRpbmdzTW9kZSghdGhpcy5Jc0luU2V0dGluZ3NNb2RlKCkpXHJcbiAgICB9XHJcbn0iLCIvLy8gPHJlZmVyZW5jZSBwYXRoPVwiUGx1Z2luVmlld01vZGVsQmFzZS50c1wiLz5cclxuXHJcbmFic3RyYWN0IGNsYXNzIERhdGFHYXRld2F5Vmlld01vZGVsQmFzZSBleHRlbmRzIFBsdWdpblZpZXdNb2RlbEJhc2Vcclxue1xyXG4gICAgcHVibGljIHJlYWRvbmx5IE1heGltdW1EYXRhc2V0QWdlOiBLbm9ja291dE9ic2VydmFibGU8bnVtYmVyPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IERhdGFQb3J0U2V0OiBLbm9ja291dE9ic2VydmFibGVBcnJheTxEYXRhUG9ydFZpZXdNb2RlbD5cclxuXHJcbiAgICBjb25zdHJ1Y3Rvcihtb2RlbCwgaWRlbnRpZmljYXRpb246IFBsdWdpbklkZW50aWZpY2F0aW9uVmlld01vZGVsKVxyXG4gICAge1xyXG4gICAgICAgIHN1cGVyKG1vZGVsLCBpZGVudGlmaWNhdGlvbilcclxuXHJcbiAgICAgICAgdGhpcy5NYXhpbXVtRGF0YXNldEFnZSA9IGtvLm9ic2VydmFibGU8bnVtYmVyPihtb2RlbC5NYXhpbXVtRGF0YXNldEFnZSlcclxuICAgICAgICB0aGlzLkRhdGFQb3J0U2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PERhdGFQb3J0Vmlld01vZGVsPigpXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIEV4dGVuZE1vZGVsKG1vZGVsOiBhbnkpXHJcbiAgICB7XHJcbiAgICAgICAgc3VwZXIuRXh0ZW5kTW9kZWwobW9kZWwpXHJcblxyXG4gICAgICAgIG1vZGVsLk1heGltdW1EYXRhc2V0QWdlID0gPG51bWJlcj5OdW1iZXIucGFyc2VJbnQoPGFueT50aGlzLk1heGltdW1EYXRhc2V0QWdlKCkpXHJcbiAgICB9XHJcbn0iLCJhYnN0cmFjdCBjbGFzcyBFeHRlbmRlZERhdGFHYXRld2F5Vmlld01vZGVsQmFzZSBleHRlbmRzIERhdGFHYXRld2F5Vmlld01vZGVsQmFzZVxyXG57XHJcbiAgICBwdWJsaWMgTW9kdWxlVG9EYXRhUG9ydE1hcDogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8T2JzZXJ2YWJsZUdyb3VwPERhdGFQb3J0Vmlld01vZGVsPj5cclxuICAgIHB1YmxpYyBPbmVEYXNNb2R1bGVTZWxlY3RvcjogS25vY2tvdXRPYnNlcnZhYmxlPE9uZURhc01vZHVsZVNlbGVjdG9yVmlld01vZGVsPlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKG1vZGVsLCBpZGVudGlmaWNhdGlvbjogUGx1Z2luSWRlbnRpZmljYXRpb25WaWV3TW9kZWwsIG9uZURhc01vZHVsZVNlbGVjdG9yOiBPbmVEYXNNb2R1bGVTZWxlY3RvclZpZXdNb2RlbClcclxuICAgIHtcclxuICAgICAgICBzdXBlcihtb2RlbCwgaWRlbnRpZmljYXRpb24pXHJcblxyXG4gICAgICAgIHRoaXMuTW9kdWxlVG9EYXRhUG9ydE1hcCA9IGtvLm9ic2VydmFibGVBcnJheSgpXHJcbiAgICAgICAgdGhpcy5PbmVEYXNNb2R1bGVTZWxlY3RvciA9IGtvLm9ic2VydmFibGU8T25lRGFzTW9kdWxlU2VsZWN0b3JWaWV3TW9kZWw+KG9uZURhc01vZHVsZVNlbGVjdG9yKVxyXG5cclxuICAgICAgICBpZiAodGhpcy5PbmVEYXNNb2R1bGVTZWxlY3RvcigpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGhpcy5PbmVEYXNNb2R1bGVTZWxlY3RvcigpLk9uTW9kdWxlU2V0Q2hhbmdlZC5zdWJzY3JpYmUoKHNlbmRlciwgYXJncykgPT5cclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgdGhpcy5VcGRhdGVEYXRhUG9ydFNldCgpXHJcbiAgICAgICAgICAgIH0pXHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBhc3luYyBJbml0aWFsaXplQXN5bmMoKVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuVXBkYXRlRGF0YVBvcnRTZXQoKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBVcGRhdGVEYXRhUG9ydFNldCgpXHJcbiAgICB7XHJcbiAgICAgICAgbGV0IGluZGV4OiBudW1iZXJcclxuICAgICAgICBsZXQgbW9kdWxlVG9EYXRhUG9ydE1hcDogT2JzZXJ2YWJsZUdyb3VwPERhdGFQb3J0Vmlld01vZGVsPltdXHJcblxyXG4gICAgICAgIG1vZHVsZVRvRGF0YVBvcnRNYXAgPSBbXVxyXG5cclxuICAgICAgICAvLyBpbnB1dHNcclxuICAgICAgICBpbmRleCA9IDBcclxuXHJcbiAgICAgICAgbW9kdWxlVG9EYXRhUG9ydE1hcCA9IG1vZHVsZVRvRGF0YVBvcnRNYXAuY29uY2F0KHRoaXMuT25lRGFzTW9kdWxlU2VsZWN0b3IoKS5Nb2R1bGVTZXQoKS5maWx0ZXIob25lRGFzTW9kdWxlID0+IG9uZURhc01vZHVsZS5EYXRhRGlyZWN0aW9uKCkgPT09IERhdGFEaXJlY3Rpb25FbnVtLklucHV0KS5tYXAob25lRGFzTW9kdWxlID0+XHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBsZXQgZ3JvdXA6IE9ic2VydmFibGVHcm91cDxEYXRhUG9ydFZpZXdNb2RlbD5cclxuXHJcbiAgICAgICAgICAgIGdyb3VwID0gbmV3IE9ic2VydmFibGVHcm91cDxEYXRhUG9ydFZpZXdNb2RlbD4ob25lRGFzTW9kdWxlLlRvU3RyaW5nKCksIHRoaXMuQ3JlYXRlRGF0YVBvcnRTZXQob25lRGFzTW9kdWxlLCBpbmRleCkpXHJcbiAgICAgICAgICAgIGluZGV4ICs9IG9uZURhc01vZHVsZS5TaXplKCk7XHJcblxyXG4gICAgICAgICAgICByZXR1cm4gZ3JvdXBcclxuICAgICAgICB9KSlcclxuXHJcbiAgICAgICAgLy8gb3V0cHV0c1xyXG4gICAgICAgIGluZGV4ID0gMFxyXG5cclxuICAgICAgICBtb2R1bGVUb0RhdGFQb3J0TWFwID0gbW9kdWxlVG9EYXRhUG9ydE1hcC5jb25jYXQodGhpcy5PbmVEYXNNb2R1bGVTZWxlY3RvcigpLk1vZHVsZVNldCgpLmZpbHRlcihvbmVEYXNNb2R1bGUgPT4gb25lRGFzTW9kdWxlLkRhdGFEaXJlY3Rpb24oKSA9PT0gRGF0YURpcmVjdGlvbkVudW0uT3V0cHV0KS5tYXAob25lRGFzTW9kdWxlID0+XHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBsZXQgZ3JvdXA6IE9ic2VydmFibGVHcm91cDxEYXRhUG9ydFZpZXdNb2RlbD5cclxuXHJcbiAgICAgICAgICAgIGdyb3VwID0gbmV3IE9ic2VydmFibGVHcm91cDxEYXRhUG9ydFZpZXdNb2RlbD4ob25lRGFzTW9kdWxlLlRvU3RyaW5nKCksIHRoaXMuQ3JlYXRlRGF0YVBvcnRTZXQob25lRGFzTW9kdWxlLCBpbmRleCkpXHJcbiAgICAgICAgICAgIGluZGV4ICs9IG9uZURhc01vZHVsZS5TaXplKCk7XHJcblxyXG4gICAgICAgICAgICByZXR1cm4gZ3JvdXBcclxuICAgICAgICB9KSlcclxuXHJcbiAgICAgICAgdGhpcy5Nb2R1bGVUb0RhdGFQb3J0TWFwKG1vZHVsZVRvRGF0YVBvcnRNYXApXHJcbiAgICAgICAgdGhpcy5EYXRhUG9ydFNldChNYXBNYW55KHRoaXMuTW9kdWxlVG9EYXRhUG9ydE1hcCgpLCBncm91cCA9PiBncm91cC5NZW1iZXJzKCkpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBDcmVhdGVEYXRhUG9ydFNldChvbmVEYXNNb2R1bGU6IE9uZURhc01vZHVsZVZpZXdNb2RlbCwgaW5kZXg6IG51bWJlcilcclxuICAgIHtcclxuICAgICAgICBsZXQgcHJlZml4OiBzdHJpbmdcclxuXHJcbiAgICAgICAgc3dpdGNoIChvbmVEYXNNb2R1bGUuRGF0YURpcmVjdGlvbigpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgY2FzZSBEYXRhRGlyZWN0aW9uRW51bS5JbnB1dDpcclxuICAgICAgICAgICAgICAgIHByZWZpeCA9IFwiSW5wdXRcIlxyXG4gICAgICAgICAgICAgICAgYnJlYWtcclxuXHJcbiAgICAgICAgICAgIGNhc2UgRGF0YURpcmVjdGlvbkVudW0uT3V0cHV0OlxyXG4gICAgICAgICAgICAgICAgcHJlZml4ID0gXCJPdXRwdXRcIlxyXG4gICAgICAgICAgICAgICAgYnJlYWtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJldHVybiBBcnJheS5mcm9tKG5ldyBBcnJheShvbmVEYXNNb2R1bGUuU2l6ZSgpKSwgKHgsIGkpID0+IFxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgcmV0dXJuIHtcclxuICAgICAgICAgICAgICAgIE5hbWU6IDxzdHJpbmc+cHJlZml4ICsgXCIgXCIgKyAoaW5kZXggKyBpKSxcclxuICAgICAgICAgICAgICAgIERhdGFUeXBlOiA8T25lRGFzRGF0YVR5cGVFbnVtPm9uZURhc01vZHVsZS5EYXRhVHlwZSgpLFxyXG4gICAgICAgICAgICAgICAgRGF0YURpcmVjdGlvbjogPERhdGFEaXJlY3Rpb25FbnVtPm9uZURhc01vZHVsZS5EYXRhRGlyZWN0aW9uKClcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH0pLm1hcChkYXRhUG9ydE1vZGVsID0+IG5ldyBEYXRhUG9ydFZpZXdNb2RlbChkYXRhUG9ydE1vZGVsLCB0aGlzKSlcclxuICAgIH1cclxufSIsIi8vLyA8cmVmZXJlbmNlIHBhdGg9XCJQbHVnaW5WaWV3TW9kZWxCYXNlLnRzXCIvPlxyXG5cclxuYWJzdHJhY3QgY2xhc3MgRGF0YVdyaXRlclZpZXdNb2RlbEJhc2UgZXh0ZW5kcyBQbHVnaW5WaWV3TW9kZWxCYXNlXHJcbntcclxuICAgIHB1YmxpYyByZWFkb25seSBGaWxlR3JhbnVsYXJpdHk6IEtub2Nrb3V0T2JzZXJ2YWJsZTxGaWxlR3JhbnVsYXJpdHlFbnVtPlxyXG4gICAgcHVibGljIHJlYWRvbmx5IEJ1ZmZlclJlcXVlc3RTZXQ6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWw+XHJcblxyXG4gICAgY29uc3RydWN0b3IobW9kZWwsIGlkZW50aWZpY2F0aW9uOiBQbHVnaW5JZGVudGlmaWNhdGlvblZpZXdNb2RlbClcclxuICAgIHtcclxuICAgICAgICBzdXBlcihtb2RlbCwgaWRlbnRpZmljYXRpb24pXHJcblxyXG4gICAgICAgIHRoaXMuRmlsZUdyYW51bGFyaXR5ID0ga28ub2JzZXJ2YWJsZTxGaWxlR3JhbnVsYXJpdHlFbnVtPihtb2RlbC5GaWxlR3JhbnVsYXJpdHkpXHJcbiAgICAgICAgdGhpcy5CdWZmZXJSZXF1ZXN0U2V0ID0ga28ub2JzZXJ2YWJsZUFycmF5PEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWw+KG1vZGVsLkJ1ZmZlclJlcXVlc3RTZXQubWFwKGJ1ZmZlclJlcXVlc3QgPT4gbmV3IEJ1ZmZlclJlcXVlc3RWaWV3TW9kZWwoYnVmZmVyUmVxdWVzdCkpKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBFeHRlbmRNb2RlbChtb2RlbDogYW55KVxyXG4gICAge1xyXG4gICAgICAgIHN1cGVyLkV4dGVuZE1vZGVsKG1vZGVsKVxyXG5cclxuICAgICAgICBtb2RlbC5GaWxlR3JhbnVsYXJpdHkgPSA8RmlsZUdyYW51bGFyaXR5RW51bT50aGlzLkZpbGVHcmFudWxhcml0eSgpXHJcbiAgICAgICAgbW9kZWwuQnVmZmVyUmVxdWVzdFNldCA9IDxCdWZmZXJSZXF1ZXN0TW9kZWxbXT50aGlzLkJ1ZmZlclJlcXVlc3RTZXQoKS5tYXAoYnVmZmVyUmVxdWVzdCA9PiBidWZmZXJSZXF1ZXN0LlRvTW9kZWwoKSlcclxuICAgIH1cclxufSIsImNsYXNzIFBsdWdpbkRlc2NyaXB0aW9uVmlld01vZGVsXHJcbntcclxuICAgIHB1YmxpYyBQcm9kdWN0VmVyc2lvbjogbnVtYmVyXHJcbiAgICBwdWJsaWMgSWQ6IHN0cmluZ1xyXG4gICAgcHVibGljIEluc3RhbmNlSWQ6IG51bWJlclxyXG4gICAgcHVibGljIEluc3RhbmNlTmFtZTogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz5cclxuICAgIHB1YmxpYyBJc0VuYWJsZWQ6IEtub2Nrb3V0T2JzZXJ2YWJsZTxib29sZWFuPlxyXG5cclxuICAgIGNvbnN0cnVjdG9yKHBsdWdpbkRlc2NyaXB0aW9uTW9kZWw6IGFueSlcclxuICAgIHtcclxuICAgICAgICB0aGlzLlByb2R1Y3RWZXJzaW9uID0gcGx1Z2luRGVzY3JpcHRpb25Nb2RlbC5Qcm9kdWN0VmVyc2lvblxyXG4gICAgICAgIHRoaXMuSWQgPSBwbHVnaW5EZXNjcmlwdGlvbk1vZGVsLklkXHJcbiAgICAgICAgdGhpcy5JbnN0YW5jZUlkID0gcGx1Z2luRGVzY3JpcHRpb25Nb2RlbC5JbnN0YW5jZUlkXHJcbiAgICAgICAgdGhpcy5JbnN0YW5jZU5hbWUgPSBrby5vYnNlcnZhYmxlPHN0cmluZz4ocGx1Z2luRGVzY3JpcHRpb25Nb2RlbC5JbnN0YW5jZU5hbWUpXHJcbiAgICAgICAgdGhpcy5Jc0VuYWJsZWQgPSBrby5vYnNlcnZhYmxlPGJvb2xlYW4+KHBsdWdpbkRlc2NyaXB0aW9uTW9kZWwuSXNFbmFibGVkKVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBUb01vZGVsKClcclxuICAgIHtcclxuICAgICAgICB2YXIgbW9kZWw6IGFueSA9IHtcclxuICAgICAgICAgICAgUHJvZHVjdFZlcnNpb246IDxudW1iZXI+dGhpcy5Qcm9kdWN0VmVyc2lvbixcclxuICAgICAgICAgICAgSWQ6IDxzdHJpbmc+dGhpcy5JZCxcclxuICAgICAgICAgICAgSW5zdGFuY2VJZDogPG51bWJlcj50aGlzLkluc3RhbmNlSWQsXHJcbiAgICAgICAgICAgIEluc3RhbmNlTmFtZTogPHN0cmluZz50aGlzLkluc3RhbmNlTmFtZSgpLFxyXG4gICAgICAgICAgICBJc0VuYWJsZWQ6IDxib29sZWFuPnRoaXMuSXNFbmFibGVkKClcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJldHVybiBtb2RlbFxyXG4gICAgfVxyXG59IiwiY2xhc3MgUGx1Z2luSWRlbnRpZmljYXRpb25WaWV3TW9kZWxcclxue1xyXG4gICAgcHVibGljIFByb2R1Y3RWZXJzaW9uOiBzdHJpbmdcclxuICAgIHB1YmxpYyBJZDogc3RyaW5nXHJcbiAgICBwdWJsaWMgTmFtZTogc3RyaW5nXHJcbiAgICBwdWJsaWMgRGVzY3JpcHRpb246IHN0cmluZ1xyXG4gICAgcHVibGljIFZpZXdSZXNvdXJjZU5hbWU6IHN0cmluZ1xyXG4gICAgcHVibGljIFZpZXdNb2RlbFJlc291cmNlTmFtZTogc3RyaW5nXHJcblxyXG4gICAgY29uc3RydWN0b3IocGx1Z2luSWRlbnRpZmljYXRpb25Nb2RlbDogYW55KVxyXG4gICAge1xyXG4gICAgICAgIHRoaXMuUHJvZHVjdFZlcnNpb24gPSBwbHVnaW5JZGVudGlmaWNhdGlvbk1vZGVsLlByb2R1Y3RWZXJzaW9uXHJcbiAgICAgICAgdGhpcy5JZCA9IHBsdWdpbklkZW50aWZpY2F0aW9uTW9kZWwuSWRcclxuICAgICAgICB0aGlzLk5hbWUgPSBwbHVnaW5JZGVudGlmaWNhdGlvbk1vZGVsLk5hbWVcclxuICAgICAgICB0aGlzLkRlc2NyaXB0aW9uID0gcGx1Z2luSWRlbnRpZmljYXRpb25Nb2RlbC5EZXNjcmlwdGlvblxyXG4gICAgICAgIHRoaXMuVmlld1Jlc291cmNlTmFtZSA9IHBsdWdpbklkZW50aWZpY2F0aW9uTW9kZWwuVmlld1Jlc291cmNlTmFtZVxyXG4gICAgICAgIHRoaXMuVmlld01vZGVsUmVzb3VyY2VOYW1lID0gcGx1Z2luSWRlbnRpZmljYXRpb25Nb2RlbC5WaWV3TW9kZWxSZXNvdXJjZU5hbWVcclxuICAgIH1cclxufSJdfQ==