/// <reference types="knockout" />
declare enum DataDirectionEnum {
    Input = 1,
    Output = 2,
}
declare enum FileGranularityEnum {
    Minute_1 = 60,
    Minute_10 = 600,
    Hour = 3600,
    Day = 86400,
}
declare enum OneDasDataTypeEnum {
    BOOLEAN = 8,
    UINT8 = 264,
    INT8 = 520,
    UINT16 = 272,
    INT16 = 528,
    UINT32 = 288,
    INT32 = 544,
    FLOAT32 = 800,
    FLOAT64 = 832,
}
declare enum OneDasStateEnum {
    Error = 1,
    Initialization = 2,
    Unconfigured = 3,
    ApplyConfiguration = 5,
    Ready = 6,
    Run = 7,
}
declare enum SampleRateEnum {
    SampleRate_100 = 1,
    SampleRate_25 = 4,
    SampleRate_5 = 20,
    SampleRate_1 = 100,
}
declare class ActionRequest {
    readonly PluginId: string;
    readonly InstanceId: number;
    readonly MethodName: string;
    readonly Data: any;
    constructor(pluginId: string, instanceId: number, methodName: string, data: any);
}
declare class ActionResponse {
    Data: any;
    constructor(data: any);
}
declare class EventDispatcher<TSender, TArgs> implements IEvent<TSender, TArgs> {
    private _subscriptions;
    subscribe(fn: (sender: TSender, args: TArgs) => void): void;
    unsubscribe(fn: (sender: TSender, args: TArgs) => void): void;
    dispatch(sender: TSender, args: TArgs): void;
}
interface IEvent<TSender, TArgs> {
    subscribe(fn: (sender: TSender, args: TArgs) => void): void;
    unsubscribe(fn: (sender: TSender, args: TArgs) => void): void;
}
declare class ChannelHubModel {
    Name: string;
    Group: string;
    OneDasDataType: OneDasDataTypeEnum;
    SampleRate: SampleRateEnum;
    Guid: string;
    CreationDateTime: string;
    Unit: string;
    TransferFunctionSet: any[];
    SerializerDataInputId: string;
    SerializerDataOutputIdSet: string[];
    constructor(name: string, group: string, oneDasDataType: OneDasDataTypeEnum, sampleRate: SampleRateEnum);
}
declare class OneDasModuleModel {
    DataType: OneDasDataTypeEnum;
    DataDirection: DataDirectionEnum;
    Size: number;
    constructor(dataType: OneDasDataTypeEnum, dataDirection: DataDirectionEnum, size: number);
}
declare class TransferFunctionModel {
    DateTime: string;
    Type: string;
    Option: string;
    Argument: string;
    constructor(dateTime: string, type: string, option: string, argument: string);
}
declare class ConnectionManager {
    static Broadcaster: any;
    static Initialize(enableLogging: boolean): void;
    static InvokeBroadcaster: (methodName: string, ...args: any[]) => Promise<any>;
}
declare class EnumerationHelper {
    static Description: {
        [index: string]: string;
    };
    static GetEnumLocalization: (typeName: string, value: any) => any;
    static GetEnumValues: (typeName: string) => number[];
}
declare let ErrorMessage: {
    [index: string]: string;
};
declare class ObservableGroup<T> {
    Key: string;
    Members: KnockoutObservableArray<T>;
    constructor(key: string, members?: T[]);
}
declare function ObservableGroupBy<T>(list: T[], nameGetter: (x: T) => string, groupNameGetter: (x: T) => string, filter: string): ObservableGroup<T>[];
declare function AddToGroupedArray<T>(item: T, groupName: string, observableGroupSet: ObservableGroup<T>[]): void;
declare function MapMany<TArrayElement, TSelect>(array: TArrayElement[], mapFunc: (item: TArrayElement) => TSelect[]): TSelect[];
declare class Guid {
    static NewGuid(): string;
}
declare let CheckNamingConvention: (value: string) => {
    HasError: boolean;
    ErrorDescription: string;
};
declare class PluginFactory {
    static CreatePluginViewModelAsync: (pluginType: string, pluginModel: any) => Promise<PluginViewModelBase>;
}
declare class PluginHive {
    static PluginIdentificationSet: Map<string, PluginIdentificationViewModel[]>;
    static Initialize: () => void;
    static FindPluginIdentification: (pluginTypeName: string, pluginId: string) => PluginIdentificationViewModel;
}
declare class ChannelHubViewModel {
    Name: KnockoutObservable<string>;
    Group: KnockoutObservable<string>;
    readonly OneDasDataType: KnockoutObservable<OneDasDataTypeEnum>;
    readonly SampleRate: KnockoutObservable<SampleRateEnum>;
    readonly Guid: string;
    readonly CreationDateTime: string;
    readonly Unit: KnockoutObservable<string>;
    readonly TransferFunctionSet: KnockoutObservableArray<TransferFunctionViewModel>;
    SelectedTransferFunction: KnockoutObservable<TransferFunctionViewModel>;
    EvaluatedTransferFunctionSet: ((value: number) => number)[];
    IsSelected: KnockoutObservable<boolean>;
    readonly AssociatedDataInput: KnockoutObservable<DataPortViewModel>;
    readonly AssociatedDataOutputSet: KnockoutObservableArray<DataPortViewModel>;
    private AssociatedDataInputId;
    private AssociatedDataOutputIdSet;
    constructor(channelHubModel: ChannelHubModel);
    GetTransformedValue: (value: any) => string;
    private CreateDefaultTransferFunction;
    IsAssociationAllowed(dataPort: DataPortViewModel): boolean;
    UpdateAssociation: (dataPort: DataPortViewModel) => void;
    SetAssociation(dataPort: DataPortViewModel): void;
    ResetAssociation(maintainWeakReference: boolean, ...dataPortSet: DataPortViewModel[]): void;
    ResetAllAssociations(maintainWeakReference: boolean): void;
    GetAssociatedDataInputId: () => string;
    GetAssociatedDataOutputIdSet: () => string[];
    ToModel(): {
        Name: string;
        Group: string;
        OneDasDataType: OneDasDataTypeEnum;
        SampleRate: SampleRateEnum;
        Guid: string;
        CreationDateTime: string;
        Unit: string;
        TransferFunctionSet: TransferFunctionModel[];
        SerializerDataInputId: string;
        SerializerDataOutputIdSet: string[];
    };
    AddTransferFunction: () => void;
    DeleteTransferFunction: () => void;
    NewTransferFunction: () => void;
    SelectTransferFunction: (transferFunction: TransferFunctionViewModel) => void;
}
declare class OneDasModuleViewModel {
    DataType: KnockoutObservable<OneDasDataTypeEnum>;
    DataDirection: KnockoutObservable<DataDirectionEnum>;
    Size: KnockoutObservable<number>;
    protected _onPropertyChanged: EventDispatcher<OneDasModuleViewModel, any>;
    constructor(model: OneDasModuleModel);
    readonly OnPropertyChanged: IEvent<OneDasModuleViewModel, any>;
    GetByteCount: (booleanBitSize?: number) => number;
    ToString(): string;
    ExtendModel(model: any): void;
    ToModel(): any;
}
declare abstract class OneDasModuleSelectorViewModelBase {
    AllowInputs: KnockoutObservable<boolean>;
    AllowOutputs: KnockoutObservable<boolean>;
    AllowBoolean: KnockoutObservable<boolean>;
    InputSettingsTemplateName: KnockoutObservable<string>;
    OutputSettingsTemplateName: KnockoutObservable<string>;
    NewInputModule: KnockoutObservable<OneDasModuleViewModel>;
    NewOutputModule: KnockoutObservable<OneDasModuleViewModel>;
    InputRemainingBytes: KnockoutObservable<number>;
    OutputRemainingBytes: KnockoutObservable<number>;
    InputRemainingCount: KnockoutObservable<number>;
    OutputRemainingCount: KnockoutObservable<number>;
    InputModuleSet: KnockoutObservableArray<OneDasModuleViewModel>;
    OutputModuleSet: KnockoutObservableArray<OneDasModuleViewModel>;
    private _onInputModuleSetChanged;
    private _onOutputModuleSetChanged;
    constructor(allowInputs: boolean, allowOutputs: boolean, allowBoolean: boolean);
    readonly OnInputModuleSetChanged: IEvent<OneDasModuleSelectorViewModelBase, OneDasModuleViewModel[]>;
    readonly OnOutputModuleSetChanged: IEvent<OneDasModuleSelectorViewModelBase, OneDasModuleViewModel[]>;
    abstract Update(): void;
    InternalCreateNewInputModule(): void;
    CreateNewInputModule(): OneDasModuleViewModel;
    InternalCreateNewOutputModule(): void;
    CreateNewOutputModule(): OneDasModuleViewModel;
    private OnModulePropertyChanged;
    AddInputModule: () => void;
    DeleteInputModule: () => void;
    AddOutputModule: () => void;
    DeleteOutputModule: () => void;
    CheckDataType(oneDasDataType: OneDasDataTypeEnum): void;
}
declare class TransferFunctionViewModel {
    DateTime: KnockoutObservable<string>;
    Type: KnockoutObservable<string>;
    Option: KnockoutObservable<string>;
    Argument: KnockoutObservable<string>;
    constructor(transferFunctionModel: TransferFunctionModel);
    ToModel(): TransferFunctionModel;
}
declare class DataPortViewModel {
    Name: KnockoutObservable<string>;
    readonly OneDasDataType: OneDasDataTypeEnum;
    readonly DataDirection: DataDirectionEnum;
    IsSelected: KnockoutObservable<boolean>;
    AssociatedChannelHubSet: KnockoutObservableArray<ChannelHubViewModel>;
    readonly AssociatedDataGateway: DataGatewayViewModelBase;
    readonly LiveDescription: KnockoutComputed<string>;
    constructor(dataPortModel: any, associatedDataGateway: DataGatewayViewModelBase);
    GetId(): string;
    ToFullQualifiedIdentifier(): string;
    ExtendModel(model: any): void;
    ToModel(): any;
    ResetAssociations(maintainWeakReference: boolean): void;
}
declare abstract class PluginViewModelBase {
    Description: PluginDescriptionViewModel;
    PluginIdentification: PluginIdentificationViewModel;
    IsInSettingsMode: KnockoutObservable<boolean>;
    private Model;
    constructor(pluginSettingsModel: any, pluginIdentification: PluginIdentificationViewModel);
    abstract InitializeAsync(): Promise<any>;
    SendActionRequest: (instanceId: number, methodName: string, data: any) => Promise<ActionResponse>;
    ExtendModel(model: any): void;
    ToModel(): any;
    EnableSettingsMode: () => void;
    DisableSettingsMode: () => void;
    ToggleSettingsMode: () => void;
}
declare abstract class DataGatewayViewModelBase extends PluginViewModelBase {
    readonly MaximumDatasetAge: KnockoutObservable<number>;
    readonly DataPortSet: KnockoutObservableArray<DataPortViewModel>;
    constructor(model: any, identification: PluginIdentificationViewModel);
    ExtendModel(model: any): void;
}
declare abstract class ExtendedDataGatewayViewModelBase extends DataGatewayViewModelBase {
    OneDasModuleSelector: KnockoutObservable<OneDasModuleSelectorViewModelBase>;
    GroupedDataPortSet: KnockoutObservableArray<ObservableGroup<DataPortViewModel>>;
    constructor(model: any, identification: PluginIdentificationViewModel, oneDasModuleSelector: OneDasModuleSelectorViewModelBase);
    InitializeAsync(): Promise<void>;
    UpdateDataPortSet(): void;
    CreateDataPortSet(oneDasModule: OneDasModuleViewModel, index: number): DataPortViewModel[];
    ExtendModel(model: any): void;
}
declare abstract class DataWriterViewModelBase extends PluginViewModelBase {
    readonly FileGranularity: KnockoutObservable<FileGranularityEnum>;
    constructor(model: any, identification: PluginIdentificationViewModel);
    ExtendModel(model: any): void;
}
declare class PluginDescriptionViewModel {
    ProductVersion: number;
    Id: string;
    InstanceId: number;
    IsEnabled: KnockoutObservable<boolean>;
    constructor(pluginDescriptionModel: any);
    ToModel(): any;
}
declare class PluginIdentificationViewModel {
    ProductVersion: string;
    Id: string;
    Name: string;
    Description: string;
    ViewResourceName: string;
    ViewModelResourceName: string;
    constructor(pluginIdentificationModel: any);
}
