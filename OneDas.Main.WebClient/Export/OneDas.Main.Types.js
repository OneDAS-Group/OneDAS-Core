var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
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
    constructor(key, members = new Array()) {
        this.Key = key;
        this.Members = ko.observableArray(members);
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
    constructor(dataDirection, moduleSet = []) {
        // methods
        this.SetMaxBytes = (value) => {
            this.MaxBytes(value);
            this.Update();
        };
        this.OnModulePropertyChanged = () => {
            this.Validate();
        };
        // commands
        this.AddModule = () => {
            let newModule;
            if (!this.HasError()) {
                this.ModuleSet.push(this.NewModule());
                this.InternalCreateNewModule();
                this.Update();
                this._onModuleSetChanged.dispatch(this, this.ModuleSet());
            }
        };
        this.DeleteModule = () => {
            this.ModuleSet.pop();
            this.Update();
            this._onModuleSetChanged.dispatch(this, this.ModuleSet());
        };
        this.SettingsTemplateName = ko.observable("Project_OneDasModuleSettingsTemplate");
        this.NewModule = ko.observable();
        this.MaxBytes = ko.observable(Infinity);
        this.RemainingBytes = ko.observable(NaN);
        this.RemainingCount = ko.observable(NaN);
        this.ModuleSet = ko.observableArray(moduleSet);
        this.ErrorMessage = ko.observable("");
        this.HasError = ko.computed(() => this.ErrorMessage().length > 0);
        this.DataDirection = ko.observable(dataDirection);
        this._onModuleSetChanged = new EventDispatcher();
        this.InternalCreateNewModule();
        this.Update();
    }
    get OnModuleSetChanged() {
        return this._onModuleSetChanged;
    }
    Update() {
        let usedBytes;
        let remainingBytes;
        usedBytes = this.ModuleSet().map(oneDasModule => oneDasModule.GetByteCount()).reduce((previousValue, currentValue) => previousValue + currentValue, 0);
        remainingBytes = this.MaxBytes() - usedBytes;
        this.RemainingBytes(remainingBytes);
        this.RemainingCount(Math.floor(this.RemainingBytes() / ((this.NewModule().DataType() & 0x0FF) / 8)));
        this.NewModule().Size(Math.min(1, this.RemainingCount()));
    }
    Validate() {
        this.ErrorMessage("");
        if (this.NewModule().HasError()) {
            this.ErrorMessage("Resolve all remaining module errors before continuing.");
        }
        if (isFinite(this.RemainingBytes()) && (this.RemainingBytes() - this.NewModule().GetByteCount() < 0)) {
            this.ErrorMessage("Size of new module is too big.");
        }
    }
    CreateNewModule() {
        return new OneDasModuleViewModel(new OneDasModuleModel(OneDasDataTypeEnum.UINT16, this.DataDirection(), EndiannessEnum.LittleEndian, 1));
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
        this.OneDasDataType = dataPortModel.OneDasDataType;
        this.DataDirection = dataPortModel.DataDirection;
        this.Endianness = dataPortModel.Endianness;
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
            OneDasDataType: this.OneDasDataType,
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
    constructor(model, identification, oneDasInputModuleSelector, oneDasOutputModuleSelector) {
        super(model, identification);
        this.GroupedDataPortSet = ko.observableArray();
        this.OneDasInputModuleSelector = ko.observable(oneDasInputModuleSelector);
        this.OneDasOutputModuleSelector = ko.observable(oneDasOutputModuleSelector);
        if (this.OneDasInputModuleSelector()) {
            this.OneDasInputModuleSelector().OnModuleSetChanged.subscribe((sender, args) => {
                this.UpdateDataPortSet();
            });
        }
        if (this.OneDasOutputModuleSelector()) {
            this.OneDasOutputModuleSelector().OnModuleSetChanged.subscribe((sender, args) => {
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
        let groupedDataPortSet;
        groupedDataPortSet = [];
        // inputs
        if (this.OneDasInputModuleSelector()) {
            index = 0;
            groupedDataPortSet = groupedDataPortSet.concat(this.OneDasInputModuleSelector().ModuleSet().map(oneDasModule => {
                let group;
                group = new ObservableGroup(oneDasModule.ToString(), this.CreateDataPortSet(oneDasModule, index));
                index += oneDasModule.Size();
                return group;
            }));
        }
        // outputs
        if (this.OneDasOutputModuleSelector()) {
            index = 0;
            groupedDataPortSet = groupedDataPortSet.concat(this.OneDasOutputModuleSelector().ModuleSet().map(oneDasModule => {
                let group;
                group = new ObservableGroup(oneDasModule.ToString(), this.CreateDataPortSet(oneDasModule, index));
                index += oneDasModule.Size();
                return group;
            }));
        }
        this.GroupedDataPortSet(groupedDataPortSet);
        this.DataPortSet(MapMany(this.GroupedDataPortSet(), group => group.Members()));
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
                OneDasDataType: oneDasModule.DataType(),
                DataDirection: oneDasModule.DataDirection()
            };
        }).map(dataPortModel => new DataPortViewModel(dataPortModel, this));
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
