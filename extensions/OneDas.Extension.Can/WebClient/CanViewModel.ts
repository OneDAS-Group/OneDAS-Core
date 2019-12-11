let ViewModelConstructor = (model: any, identification: ExtensionIdentificationViewModel) => new CanViewModel(model, identification)

class CanViewModel extends ExtendedDataGatewayViewModelBase
{
    public CanDriverType: KnockoutObservable<CanDriverTypeEnum>
    public HardwareId: KnockoutObservable<string>
    public BitRate: KnockoutObservable<CiaBitRateEnum>
    public BusCoupling: KnockoutObservable<BusCouplingEnum>
    public FrameRateDivider: KnockoutObservable<number>

    constructor(model, identification: ExtensionIdentificationViewModel)
    {
        super(model, identification, new CanModuleSelectorViewModel(OneDasModuleSelectorModeEnum.Duplex, model.ModuleSet.map(canModuleModel => new CanModuleViewModel(canModuleModel))))

        EnumerationHelper.Description["BusCouplingEnum_Lowspeed"] = "Lowspeed"
        EnumerationHelper.Description["BusCouplingEnum_Highspeed"] = "Highspeed"

        EnumerationHelper.Description["CanDriverTypeEnum_IxxatUsbToCanV2Compact"] = "Ixxat USB-to-CAN V2 compact"

        EnumerationHelper.Description["CanFrameFormatEnum_Standard"] = "Standard (11-bit identifer)"
        EnumerationHelper.Description["CanFrameFormatEnum_Extended"] = "Extended (29-bit identifer)"

        EnumerationHelper.Description["CiaBitRateEnum_Cia10KBit"] = "10 KBit/s"
        EnumerationHelper.Description["CiaBitRateEnum_Cia20KBit"] = "20 KBit/s"
        EnumerationHelper.Description["CiaBitRateEnum_Cia50KBit"] = "50 KBit/s"
        EnumerationHelper.Description["CiaBitRateEnum_Cia125KBit"] = "125 KBit/s"
        EnumerationHelper.Description["CiaBitRateEnum_Cia500KBit"] = "500 KBit/s"
        EnumerationHelper.Description["CiaBitRateEnum_Cia800KBit"] = "800 KBit/s"
        EnumerationHelper.Description["CiaBitRateEnum_Cia1000KBit"] = "1000 KBit/s"

        this.CanDriverType = ko.observable<CanDriverTypeEnum>(model.CanDriverType)
        this.HardwareId = ko.observable<string>(model.HardwareId)
        this.BitRate = ko.observable<CiaBitRateEnum>(model.BitRate)
        this.BusCoupling = ko.observable<BusCouplingEnum>(model.BusCoupling)
        this.FrameRateDivider = ko.observable<number>(model.FrameRateDivider)

        this.OneDasModuleSelector().SetMaxBytes(128) error
    }

    // methods
    public ExtendModel(model: any)
    {
        super.ExtendModel(model)

        model.CanDriverType = <CanDriverTypeEnum>this.CanDriverType()
        model.HardwareId = <string>this.HardwareId()
        model.BitRate = <CiaBitRateEnum>this.BitRate()
        model.BusCoupling = <BusCouplingEnum>this.BusCoupling()
        model.FrameRateDivider = <number>this.FrameRateDivider()
        model.ModuleSet = <OneDasModuleModel[]>this.OneDasModuleSelector().ModuleSet().map(moduleModel => moduleModel.ToModel())
    }
}