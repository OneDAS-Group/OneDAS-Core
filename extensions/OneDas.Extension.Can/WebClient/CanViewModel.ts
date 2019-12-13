let ViewModelConstructor = (model: any, identification: ExtensionIdentificationViewModel) => new CanViewModel(model, identification)

class CanViewModel extends ExtendedDataGatewayViewModelBase
{
    public DeviceDescriptionSet: KnockoutObservableArray<DeviceDescription>

    public CanDeviceType: KnockoutObservable<CanDeviceTypeEnum>
    public HardwareId: KnockoutObservable<string>
    public BitRate: KnockoutObservable<CiaBitRateEnum>
    public BusCoupling: KnockoutObservable<BusCouplingEnum>
    public FrameRateDivider: KnockoutObservable<number>

    public CanDeviceTypeSet: KnockoutObservableArray<CanDeviceTypeEnum>
    public CiaBitRateSet: KnockoutObservableArray<CiaBitRateEnum>
    public BusCouplingSet: KnockoutObservableArray<BusCouplingEnum>

    constructor(model, identification: ExtensionIdentificationViewModel)
    {
        super(model, identification, new CanModuleSelectorViewModel(OneDasModuleSelectorModeEnum.Duplex, model.ModuleSet.map(canModuleModel => new CanModuleViewModel(canModuleModel))))

        EnumerationHelper.Description["BusCouplingEnum_Lowspeed"] = "Lowspeed"
        EnumerationHelper.Description["BusCouplingEnum_Highspeed"] = "Highspeed"

        EnumerationHelper.Description["CanDeviceTypeEnum_IxxatUsbToCanV2Compact"] = "Ixxat USB-to-CAN V2 compact"

        EnumerationHelper.Description["CanFrameFormatEnum_Standard"] = "Standard (11-bit ID)"
        EnumerationHelper.Description["CanFrameFormatEnum_Extended"] = "Extended (29-bit ID)"

        EnumerationHelper.Description["CiaBitRateEnum_Cia10KBit"] = "10 KBit/s"
        EnumerationHelper.Description["CiaBitRateEnum_Cia20KBit"] = "20 KBit/s"
        EnumerationHelper.Description["CiaBitRateEnum_Cia50KBit"] = "50 KBit/s"
        EnumerationHelper.Description["CiaBitRateEnum_Cia125KBit"] = "125 KBit/s"
        EnumerationHelper.Description["CiaBitRateEnum_Cia500KBit"] = "500 KBit/s"
        EnumerationHelper.Description["CiaBitRateEnum_Cia800KBit"] = "800 KBit/s"
        EnumerationHelper.Description["CiaBitRateEnum_Cia1000KBit"] = "1000 KBit/s"

        this.DeviceDescriptionSet = ko.observableArray<DeviceDescription>()

        this.CanDeviceType = ko.observable<CanDeviceTypeEnum>(model.CanDeviceType)
        this.HardwareId = ko.observable<string>(model.HardwareId)
        this.BitRate = ko.observable<CiaBitRateEnum>(model.BitRate)
        this.BusCoupling = ko.observable<BusCouplingEnum>(model.BusCoupling)
        this.FrameRateDivider = ko.observable<number>(model.FrameRateDivider)

        this.CanDeviceTypeSet = ko.observableArray<CanDeviceTypeEnum>(EnumerationHelper.GetEnumValues("CanDeviceTypeEnum"))
        this.CiaBitRateSet = ko.observableArray<CiaBitRateEnum>(EnumerationHelper.GetEnumValues("CiaBitRateEnum"))
        this.BusCouplingSet = ko.observableArray<BusCouplingEnum>(EnumerationHelper.GetEnumValues("BusCouplingEnum"))
    }

    // methods
    public SelectDevice = (deviceDescription: DeviceDescription) => {
        this.HardwareId(deviceDescription.HardwareId)
    }

    public GetDevices = async () => {
        let actionResponse: ActionResponse
        let dictionary: any

        try {
            actionResponse = await this.SendActionRequest(0, "GetDevices", null)
            dictionary = actionResponse.Data

            this.DeviceDescriptionSet.removeAll()

            for (var key in dictionary) {
                if (key !== "$type") {
                    this.DeviceDescriptionSet.push(new DeviceDescription(key, dictionary[key]))
                }
            }
        }
        catch (e) {
            alert(e.message)
        }
    }

    public ExtendModel(model: any)
    {
        super.ExtendModel(model)

        model.CanDeviceType = <CanDeviceTypeEnum>this.CanDeviceType()
        model.HardwareId = <string>this.HardwareId()
        model.BitRate = <CiaBitRateEnum>this.BitRate()
        model.BusCoupling = <BusCouplingEnum>this.BusCoupling()
        model.FrameRateDivider = <number>this.FrameRateDivider()
        model.ModuleSet = <CanModuleModel[]>this.OneDasModuleSelector().ModuleSet().map(moduleModel => moduleModel.ToModel())
    }
}