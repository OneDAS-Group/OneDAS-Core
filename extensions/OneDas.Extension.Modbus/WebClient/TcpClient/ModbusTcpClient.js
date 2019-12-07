class ModbusModuleModel extends OneDasModuleModel {
    constructor(startingAddress, objectType, dataType, dataDirection, endianness, size) {
        super(dataType, dataDirection, endianness, size);
        this.StartingAddress = startingAddress;
        this.ObjectType = objectType;
    }
}
class ModbusModuleViewModel extends OneDasModuleViewModel {
    constructor(modbusModuleModel) {
        super(modbusModuleModel);
        switch (this.DataDirection()) {
            case DataDirectionEnum.Input:
                this.ObjectTypeSet = ko.observableArray(EnumerationHelper.GetEnumValues("ModbusObjectTypeEnum").filter(objectType => objectType >= 3));
                break;
            case DataDirectionEnum.Output:
                this.ObjectTypeSet = ko.observableArray(EnumerationHelper.GetEnumValues("ModbusObjectTypeEnum").filter(objectType => objectType === ModbusObjectTypeEnum.HoldingRegister));
                break;
        }
        this.StartingAddress = ko.observable(modbusModuleModel.StartingAddress);
        this.ObjectType = ko.observable(modbusModuleModel.ObjectType);
        this.StartingAddress.subscribe(newValue => this.OnPropertyChanged());
        this.ObjectType.subscribe(newValue => this.OnPropertyChanged());
        // improve: better would be server side generation of correct module
        if (!this._model.$type) {
            this._model.$type = "OneDas.Extension.Modbus.ModbusTcpModule, OneDas.Extension.Modbus";
        }
    }
    Validate() {
        super.Validate();
        switch (this.DataDirection()) {
            case DataDirectionEnum.Input:
                switch (this.ObjectType()) {
                    case ModbusObjectTypeEnum.HoldingRegister:
                    case ModbusObjectTypeEnum.InputRegister:
                        if (this.GetByteCount() > 125 * 2) {
                            this.ErrorMessage("The number of registers per module must be within range 0..125.");
                        }
                        break;
                    case ModbusObjectTypeEnum.Coil:
                    case ModbusObjectTypeEnum.DiscreteInput:
                        if (this.GetByteCount() > 2000 * 2) {
                            this.ErrorMessage("The number of registers per module must be within range 0..2000.");
                        }
                        break;
                }
                break;
            case DataDirectionEnum.Output:
                switch (this.ObjectType()) {
                    case ModbusObjectTypeEnum.HoldingRegister:
                        if (this.GetByteCount() > 123 * 2) {
                            this.ErrorMessage("The number of registers per module must be within range 0..123.");
                        }
                        break;
                }
                break;
        }
        if (this.StartingAddress() < 0) {
            this.ErrorMessage("The starting address of a module must be within range 0..65535.");
        }
        if (this.StartingAddress() + this.Size() > 65536) {
            this.ErrorMessage("Starting address + module size exceeds register address range (0..65535).");
        }
    }
    ToString() {
        return super.ToString() + ' - ' + EnumerationHelper.GetEnumLocalization("ModbusObjectTypeEnum", this.ObjectType()) + " - address: " + this.StartingAddress();
    }
    ExtendModel(model) {
        super.ExtendModel(model);
        model.StartingAddress = this.StartingAddress(),
            model.ObjectType = this.ObjectType();
    }
}
var ModbusObjectTypeEnum;
(function (ModbusObjectTypeEnum) {
    ModbusObjectTypeEnum[ModbusObjectTypeEnum["DiscreteInput"] = 1] = "DiscreteInput";
    ModbusObjectTypeEnum[ModbusObjectTypeEnum["Coil"] = 2] = "Coil";
    ModbusObjectTypeEnum[ModbusObjectTypeEnum["InputRegister"] = 3] = "InputRegister";
    ModbusObjectTypeEnum[ModbusObjectTypeEnum["HoldingRegister"] = 4] = "HoldingRegister";
})(ModbusObjectTypeEnum || (ModbusObjectTypeEnum = {}));
window["ModbusObjectTypeEnum"] = ModbusObjectTypeEnum;
class ModbusClientModuleSelectorViewModel extends OneDasModuleSelectorViewModel {
    constructor(oneDasModuleSelectorMode, moduleSet) {
        super(oneDasModuleSelectorMode, moduleSet);
        this.SettingsTemplateName = ko.observable("ModbusTcp_OneDasModuleSettingsTemplate");
    }
    CreateNewModule() {
        return new ModbusClientModuleViewModel(new ModbusModuleModel(0, ModbusObjectTypeEnum.HoldingRegister, OneDasDataTypeEnum.UINT16, DataDirectionEnum.Input, EndiannessEnum.BigEndian, 1));
    }
}
class ModbusClientModuleViewModel extends ModbusModuleViewModel {
    Validate() {
        super.Validate();
        if (this.DataDirection() === DataDirectionEnum.Output && this.ObjectType() !== ModbusObjectTypeEnum.HoldingRegister) {
            this.ErrorMessage("Only object type 'holding register' is allowed for output modules.");
        }
    }
}
class ModbusClientViewModel extends ExtendedDataGatewayViewModelBase {
    constructor(model, identification) {
        super(model, identification, new ModbusClientModuleSelectorViewModel(OneDasModuleSelectorModeEnum.Duplex, model.ModuleSet.map(modbusModuleModel => new ModbusClientModuleViewModel(modbusModuleModel))));
        EnumerationHelper.Description["ModbusObjectTypeEnum_DiscreteInput"] = "Discrete Input (R)";
        EnumerationHelper.Description["ModbusObjectTypeEnum_Coil"] = "Coil (RW)";
        EnumerationHelper.Description["ModbusObjectTypeEnum_InputRegister"] = "Input Register (R)";
        EnumerationHelper.Description["ModbusObjectTypeEnum_HoldingRegister"] = "Holding Register (RW)";
        this.UnitIdentifier = ko.observable(model.UnitIdentifier);
        this.FrameRateDivider = ko.observable(model.FrameRateDivider);
    }
    ExtendModel(model) {
        super.ExtendModel(model);
        model.UnitIdentifier = this.UnitIdentifier();
        model.FrameRateDivider = this.FrameRateDivider();
        model.ModuleSet = this.OneDasModuleSelector().ModuleSet().map(moduleModel => moduleModel.ToModel());
    }
}
let ViewModelConstructor = (model, identification) => new ModbusTcpClientViewModel(model, identification);
class ModbusTcpClientViewModel extends ModbusClientViewModel {
    constructor(model, identification) {
        super(model, identification);
        this.RemoteIpAddress = ko.observable(model.RemoteIpAddress);
        this.Port = ko.observable(model.Port);
    }
    ExtendModel(model) {
        super.ExtendModel(model);
        model.RemoteIpAddress = this.RemoteIpAddress();
        model.Port = this.Port();
    }
}
var ParityEnum;
(function (ParityEnum) {
    ParityEnum[ParityEnum["None"] = 0] = "None";
    ParityEnum[ParityEnum["Odd"] = 1] = "Odd";
    ParityEnum[ParityEnum["Even"] = 2] = "Even";
    ParityEnum[ParityEnum["Mark"] = 3] = "Mark";
})(ParityEnum || (ParityEnum = {}));
var HandshakeEnum;
(function (HandshakeEnum) {
    HandshakeEnum[HandshakeEnum["None"] = 0] = "None";
    HandshakeEnum[HandshakeEnum["RequestToSend"] = 1] = "RequestToSend";
    HandshakeEnum[HandshakeEnum["RequestToSendXOnXOff"] = 2] = "RequestToSendXOnXOff";
    HandshakeEnum[HandshakeEnum["XOnXOff"] = 3] = "XOnXOff";
})(HandshakeEnum || (HandshakeEnum = {}));
var StopBits;
(function (StopBits) {
    StopBits[StopBits["None"] = 0] = "None";
    StopBits[StopBits["One"] = 1] = "One";
    StopBits[StopBits["Two"] = 2] = "Two";
    StopBits[StopBits["OnePointFive"] = 3] = "OnePointFive";
})(StopBits || (StopBits = {}));
