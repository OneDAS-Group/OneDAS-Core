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
class ModbusServerModuleSelectorViewModel extends OneDasModuleSelectorViewModel {
    constructor(oneDasModuleSelectorMode, moduleSet) {
        super(oneDasModuleSelectorMode, moduleSet);
        this.SettingsTemplateName = ko.observable("ModbusTcp_OneDasModuleSettingsTemplate");
    }
    CreateNewModule() {
        return new ModbusServerModuleViewModel(new ModbusModuleModel(0, ModbusObjectTypeEnum.HoldingRegister, OneDasDataTypeEnum.UINT16, DataDirectionEnum.Input, EndiannessEnum.BigEndian, 1));
    }
}
class ModbusServerModuleViewModel extends ModbusModuleViewModel {
    Validate() {
        super.Validate();
        // because INPUT register values do not change (they are readonly for clients) it is useless to allow server side read
        if (this.DataDirection() === DataDirectionEnum.Input && this.ObjectType() !== ModbusObjectTypeEnum.HoldingRegister) {
            this.ErrorMessage("Only object type 'holding register' is allowed for input modules.");
        }
    }
}
class ModbusServerViewModel extends ExtendedDataGatewayViewModelBase {
    constructor(model, identification) {
        super(model, identification, new ModbusServerModuleSelectorViewModel(OneDasModuleSelectorModeEnum.Duplex, model.ModuleSet.map(modbusModuleModel => new ModbusServerModuleViewModel(modbusModuleModel))));
        EnumerationHelper.Description["ModbusObjectTypeEnum_DiscreteInput"] = "Discrete Input (R)";
        EnumerationHelper.Description["ModbusObjectTypeEnum_Coil"] = "Coil (RW)";
        EnumerationHelper.Description["ModbusObjectTypeEnum_InputRegister"] = "Input Register (R)";
        EnumerationHelper.Description["ModbusObjectTypeEnum_HoldingRegister"] = "Holding Register (RW)";
        this.FrameRateDivider = ko.observable(model.FrameRateDivider);
    }
    ExtendModel(model) {
        super.ExtendModel(model);
        model.FrameRateDivider = this.FrameRateDivider();
        model.ModuleSet = this.OneDasModuleSelector().ModuleSet().map(moduleModel => moduleModel.ToModel());
    }
}
let ViewModelConstructor = (model, identification) => new ModbusRtuServerViewModel(model, identification);
class ModbusRtuServerViewModel extends ModbusServerViewModel {
    constructor(model, identification) {
        super(model, identification);
        this.UnitIdentifier = ko.observable(model.UnitIdentifier);
        this.Port = ko.observable(model.Port);
    }
    ExtendModel(model) {
        super.ExtendModel(model);
        model.LocalIpAddress = this.UnitIdentifier();
        model.Port = this.Port();
    }
}
