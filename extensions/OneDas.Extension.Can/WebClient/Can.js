let ViewModelConstructor = (model, identification) => new CanViewModel(model, identification);
class CanViewModel extends ExtendedDataGatewayViewModelBase {
    constructor(model, identification) {
        super(model, identification, new CanModuleSelectorViewModel(OneDasModuleSelectorModeEnum.Duplex, model.ModuleSet.map(canModuleModel => new CanModuleViewModel(canModuleModel))));
        EnumerationHelper.Description["BusCouplingEnum_Lowspeed"] = "Lowspeed";
        EnumerationHelper.Description["BusCouplingEnum_Highspeed"] = "Highspeed";
        EnumerationHelper.Description["CanDriverTypeEnum_IxxatUsbToCanV2Compact"] = "Ixxat USB-to-CAN V2 compact";
        EnumerationHelper.Description["CanFrameFormatEnum_Standard"] = "Standard (11-bit identifer)";
        EnumerationHelper.Description["CanFrameFormatEnum_Extended"] = "Extended (29-bit identifer)";
        EnumerationHelper.Description["CiaBitRateEnum_Cia10KBit"] = "10 KBit/s";
        EnumerationHelper.Description["CiaBitRateEnum_Cia20KBit"] = "20 KBit/s";
        EnumerationHelper.Description["CiaBitRateEnum_Cia50KBit"] = "50 KBit/s";
        EnumerationHelper.Description["CiaBitRateEnum_Cia125KBit"] = "125 KBit/s";
        EnumerationHelper.Description["CiaBitRateEnum_Cia500KBit"] = "500 KBit/s";
        EnumerationHelper.Description["CiaBitRateEnum_Cia800KBit"] = "800 KBit/s";
        EnumerationHelper.Description["CiaBitRateEnum_Cia1000KBit"] = "1000 KBit/s";
        this.CanDriverType = ko.observable(model.CanDriverType);
        this.HardwareId = ko.observable(model.HardwareId);
        this.BitRate = ko.observable(model.BitRate);
        this.BusCoupling = ko.observable(model.BusCoupling);
        this.FrameRateDivider = ko.observable(model.FrameRateDivider);
        this.OneDasModuleSelector().SetMaxBytes(128);
        error;
    }
    // methods
    ExtendModel(model) {
        super.ExtendModel(model);
        model.CanDriverType = this.CanDriverType();
        model.HardwareId = this.HardwareId();
        model.BitRate = this.BitRate();
        model.BusCoupling = this.BusCoupling();
        model.FrameRateDivider = this.FrameRateDivider();
        model.ModuleSet = this.OneDasModuleSelector().ModuleSet().map(moduleModel => moduleModel.ToModel());
    }
}
var CanFrameFormatEnum;
(function (CanFrameFormatEnum) {
    CanFrameFormatEnum[CanFrameFormatEnum["Standard"] = 1] = "Standard";
    CanFrameFormatEnum[CanFrameFormatEnum["Extended"] = 2] = "Extended";
})(CanFrameFormatEnum || (CanFrameFormatEnum = {}));
class CanModuleModel extends OneDasModuleModel {
    constructor(identifier, frameFormat, dataType, dataDirection, endianness, size) {
        super(dataType, dataDirection, endianness, size);
        this.Identifier = identifier;
        this.FrameFormat = frameFormat;
    }
}
class CanModuleViewModel extends OneDasModuleViewModel {
    constructor(canModuleModel) {
        super(canModuleModel);
        this.Identifier = ko.observable(canModuleModel.Identifier);
        this.FrameFormat = ko.observable(canModuleModel.FrameFormat);
        this.FrameFormatSet = ko.observableArray(EnumerationHelper.GetEnumValues("CanFrameFormatEnum").filter(objectType => objectType >= 3));
        this.Identifier.subscribe(newValue => this.OnPropertyChanged());
        this.FrameFormat.subscribe(newValue => this.OnPropertyChanged());
    }
    Validate() {
        super.Validate();
        switch (this.FrameFormat()) {
            case CanFrameFormatEnum.Standard:
                if (!(0 <= this.Identifier() || this.Identifier() <= Math.pow(2, 11) - 1)) {
                    this.ErrorMessage("The identifier of a standard frame must be between 0..2047.");
                }
                break;
            case CanFrameFormatEnum.Extended:
                if (!(0 <= this.Identifier() || this.Identifier() <= Math.pow(2, 29) - 1)) {
                    this.ErrorMessage("The identifier of an extended frame must be between 0..536870911.");
                }
                break;
        }
    }
    ToString() {
        return super.ToString() + ' - ' + EnumerationHelper.GetEnumLocalization("CanFrameFormatEnum", this.FrameFormat()) + " - identifier: " + this.Identifier();
    }
    ExtendModel(model) {
        super.ExtendModel(model);
        model.Identifier = this.Identifier();
        model.FrameFormat = this.FrameFormat();
    }
}
var BusCouplingEnum;
(function (BusCouplingEnum) {
    BusCouplingEnum[BusCouplingEnum["Lowspeed"] = 1] = "Lowspeed";
    BusCouplingEnum[BusCouplingEnum["Highspeed"] = 2] = "Highspeed";
})(BusCouplingEnum || (BusCouplingEnum = {}));
var CiaBitRateEnum;
(function (CiaBitRateEnum) {
    CiaBitRateEnum[CiaBitRateEnum["Cia10KBit"] = 1] = "Cia10KBit";
    CiaBitRateEnum[CiaBitRateEnum["Cia20KBit"] = 2] = "Cia20KBit";
    CiaBitRateEnum[CiaBitRateEnum["Cia50KBit"] = 3] = "Cia50KBit";
    CiaBitRateEnum[CiaBitRateEnum["Cia125KBit"] = 4] = "Cia125KBit";
    CiaBitRateEnum[CiaBitRateEnum["Cia500KBit"] = 5] = "Cia500KBit";
    CiaBitRateEnum[CiaBitRateEnum["Cia800KBit"] = 6] = "Cia800KBit";
    CiaBitRateEnum[CiaBitRateEnum["Cia1000KBit"] = 7] = "Cia1000KBit";
})(CiaBitRateEnum || (CiaBitRateEnum = {}));
class CanModuleSelectorViewModel extends OneDasModuleSelectorViewModel {
    constructor(oneDasModuleSelectorMode, moduleSet) {
        super(oneDasModuleSelectorMode, moduleSet);
        this.SettingsTemplateName = ko.observable("Can_OneDasModuleSettingsTemplate");
    }
    CreateNewModule() {
        return new CanModuleViewModel(new CanModuleModel(0, CanFrameFormatEnum.Standard, OneDasDataTypeEnum.INT32, DataDirectionEnum.Input, EndiannessEnum.BigEndian, 2));
    }
}
var CanDriverTypeEnum;
(function (CanDriverTypeEnum) {
    CanDriverTypeEnum[CanDriverTypeEnum["IxxatUsbToCanV2Compact"] = 1] = "IxxatUsbToCanV2Compact";
})(CanDriverTypeEnum || (CanDriverTypeEnum = {}));
