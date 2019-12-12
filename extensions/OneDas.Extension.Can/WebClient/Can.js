var BusCouplingEnum;
(function (BusCouplingEnum) {
    BusCouplingEnum[BusCouplingEnum["Lowspeed"] = 1] = "Lowspeed";
    BusCouplingEnum[BusCouplingEnum["Highspeed"] = 2] = "Highspeed";
})(BusCouplingEnum || (BusCouplingEnum = {}));
window["BusCouplingEnum"] = BusCouplingEnum;
var CanDriverTypeEnum;
(function (CanDriverTypeEnum) {
    CanDriverTypeEnum[CanDriverTypeEnum["IxxatUsbToCanV2Compact"] = 1] = "IxxatUsbToCanV2Compact";
})(CanDriverTypeEnum || (CanDriverTypeEnum = {}));
window["CanDriverTypeEnum"] = CanDriverTypeEnum;
var CanFrameFormatEnum;
(function (CanFrameFormatEnum) {
    CanFrameFormatEnum[CanFrameFormatEnum["Standard"] = 1] = "Standard";
    CanFrameFormatEnum[CanFrameFormatEnum["Extended"] = 2] = "Extended";
})(CanFrameFormatEnum || (CanFrameFormatEnum = {}));
window["CanFrameFormatEnum"] = CanFrameFormatEnum;
class CanModuleModel extends OneDasModuleModel {
    constructor(identifier, frameFormat, dataType, dataDirection, endianness, size) {
        super(dataType, dataDirection, endianness, size);
        this.Identifier = identifier;
        this.FrameFormat = frameFormat;
    }
}
class CanModuleSelectorViewModel extends OneDasModuleSelectorViewModel {
    constructor(oneDasModuleSelectorMode, moduleSet) {
        super(oneDasModuleSelectorMode, moduleSet);
        this.SettingsTemplateName = ko.observable("Can_CanModuleSettingsTemplate");
    }
    CreateNewModule() {
        return new CanModuleViewModel(new CanModuleModel(0, CanFrameFormatEnum.Standard, OneDasDataTypeEnum.INT32, DataDirectionEnum.Input, EndiannessEnum.BigEndian, 2));
    }
}
class CanModuleViewModel extends OneDasModuleViewModel {
    constructor(canModuleModel) {
        super(canModuleModel);
        this.Identifier = ko.observable(canModuleModel.Identifier);
        this.FrameFormat = ko.observable(canModuleModel.FrameFormat);
        this.CanFrameFormatSet = ko.observableArray(EnumerationHelper.GetEnumValues("CanFrameFormatEnum"));
        this.Identifier.subscribe(_ => this.OnPropertyChanged());
        this.FrameFormat.subscribe(_ => this.OnPropertyChanged());
        this.MaxSize(8);
        // improve: better would be server side generation of correct module
        if (!this._model.$type) {
            this._model.$type = "OneDas.Extension.Can.CanModule, OneDas.Extension.Can";
        }
    }
    Validate() {
        super.Validate();
        switch (this.FrameFormat()) {
            case CanFrameFormatEnum.Standard:
                if (!(0 <= this.Identifier() && this.Identifier() <= Math.pow(2, 11) - 1)) {
                    this.ErrorMessage("The identifier of a standard frame must be between 0..2047.");
                }
                break;
            case CanFrameFormatEnum.Extended:
                if (!(0 <= this.Identifier() && this.Identifier() <= Math.pow(2, 29) - 1)) {
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
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
let ViewModelConstructor = (model, identification) => new CanViewModel(model, identification);
class CanViewModel extends ExtendedDataGatewayViewModelBase {
    constructor(model, identification) {
        super(model, identification, new CanModuleSelectorViewModel(OneDasModuleSelectorModeEnum.Duplex, model.ModuleSet.map(canModuleModel => new CanModuleViewModel(canModuleModel))));
        // methods
        this.SelectDevice = (deviceDescription) => {
            this.HardwareId(deviceDescription.HardwareId);
        };
        this.GetDevices = () => __awaiter(this, void 0, void 0, function* () {
            let actionResponse;
            let dictionary;
            try {
                actionResponse = yield this.SendActionRequest(0, "GetDevices", null);
                dictionary = actionResponse.Data;
                this.DeviceDescriptionSet.removeAll();
                for (var key in dictionary) {
                    if (key !== "$type") {
                        this.DeviceDescriptionSet.push(new DeviceDescription(key, dictionary[key]));
                    }
                }
            }
            catch (e) {
                alert(e.message);
            }
        });
        EnumerationHelper.Description["BusCouplingEnum_Lowspeed"] = "Lowspeed";
        EnumerationHelper.Description["BusCouplingEnum_Highspeed"] = "Highspeed";
        EnumerationHelper.Description["CanDriverTypeEnum_IxxatUsbToCanV2Compact"] = "Ixxat USB-to-CAN V2 compact";
        EnumerationHelper.Description["CanFrameFormatEnum_Standard"] = "Standard (11-bit ID)";
        EnumerationHelper.Description["CanFrameFormatEnum_Extended"] = "Extended (29-bit ID)";
        EnumerationHelper.Description["CiaBitRateEnum_Cia10KBit"] = "10 KBit/s";
        EnumerationHelper.Description["CiaBitRateEnum_Cia20KBit"] = "20 KBit/s";
        EnumerationHelper.Description["CiaBitRateEnum_Cia50KBit"] = "50 KBit/s";
        EnumerationHelper.Description["CiaBitRateEnum_Cia125KBit"] = "125 KBit/s";
        EnumerationHelper.Description["CiaBitRateEnum_Cia500KBit"] = "500 KBit/s";
        EnumerationHelper.Description["CiaBitRateEnum_Cia800KBit"] = "800 KBit/s";
        EnumerationHelper.Description["CiaBitRateEnum_Cia1000KBit"] = "1000 KBit/s";
        this.DeviceDescriptionSet = ko.observableArray();
        this.CanDriverType = ko.observable(model.CanDriverType);
        this.HardwareId = ko.observable(model.HardwareId);
        this.BitRate = ko.observable(model.BitRate);
        this.BusCoupling = ko.observable(model.BusCoupling);
        this.FrameRateDivider = ko.observable(model.FrameRateDivider);
        this.CanDriverTypeSet = ko.observableArray(EnumerationHelper.GetEnumValues("CanDriverTypeEnum"));
        this.CiaBitRateSet = ko.observableArray(EnumerationHelper.GetEnumValues("CiaBitRateEnum"));
        this.BusCouplingSet = ko.observableArray(EnumerationHelper.GetEnumValues("BusCouplingEnum"));
    }
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
window["CiaBitRateEnum"] = CiaBitRateEnum;
class DeviceDescription {
    constructor(name, hardwareId) {
        this.Name = name;
        this.HardwareId = hardwareId;
    }
}
