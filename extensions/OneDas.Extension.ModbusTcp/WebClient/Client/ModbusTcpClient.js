class ModbusTcpModuleModel extends OneDasModuleModel {
    constructor(startingAddress, objectType, dataType, dataDirection, endianness, size) {
        super(dataType, dataDirection, endianness, size);
        this.StartingAddress = startingAddress;
        this.ObjectType = objectType;
    }
}
class ModbusTcpModuleViewModel extends OneDasModuleViewModel {
    constructor(modbusTcpModuleModel) {
        super(modbusTcpModuleModel);
        switch (this.DataDirection()) {
            case DataDirectionEnum.Input:
                this.ObjectTypeSet = ko.observableArray(EnumerationHelper.GetEnumValues("ModbusTcpObjectTypeEnum").filter(objectType => objectType >= 3));
                break;
            case DataDirectionEnum.Output:
                this.ObjectTypeSet = ko.observableArray(EnumerationHelper.GetEnumValues("ModbusTcpObjectTypeEnum").filter(objectType => objectType === ModbusTcpObjectTypeEnum.HoldingRegister));
                break;
        }
        this.StartingAddress = ko.observable(modbusTcpModuleModel.StartingAddress);
        this.ObjectType = ko.observable(modbusTcpModuleModel.ObjectType);
        this.StartingAddress.subscribe(newValue => this.OnPropertyChanged());
        this.ObjectType.subscribe(newValue => this.OnPropertyChanged());
        // improve: better would be server side generation of correct module
        if (!this._model.$type) {
            this._model.$type = "OneDas.Extension.ModbusTcp.ModbusTcpModule, OneDas.Extension.ModbusTcp";
        }
    }
    Validate() {
        super.Validate();
        switch (this.DataDirection()) {
            case DataDirectionEnum.Input:
                switch (this.ObjectType()) {
                    case ModbusTcpObjectTypeEnum.HoldingRegister:
                    case ModbusTcpObjectTypeEnum.InputRegister:
                        if (this.GetByteCount() > 125 * 2) {
                            this.ErrorMessage("The number of registers per module must be within range 0..125.");
                        }
                        break;
                    case ModbusTcpObjectTypeEnum.Coil:
                    case ModbusTcpObjectTypeEnum.DiscreteInput:
                        if (this.GetByteCount() > 2000 * 2) {
                            this.ErrorMessage("The number of registers per module must be within range 0..2000.");
                        }
                        break;
                }
                break;
            case DataDirectionEnum.Output:
                switch (this.ObjectType()) {
                    case ModbusTcpObjectTypeEnum.HoldingRegister:
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
        return super.ToString() + ' - ' + EnumerationHelper.GetEnumLocalization("ModbusTcpObjectTypeEnum", this.ObjectType()) + " - address: " + this.StartingAddress();
    }
    ExtendModel(model) {
        super.ExtendModel(model);
        model.StartingAddress = this.StartingAddress(),
            model.ObjectType = this.ObjectType();
    }
}
var ModbusTcpObjectTypeEnum;
(function (ModbusTcpObjectTypeEnum) {
    ModbusTcpObjectTypeEnum[ModbusTcpObjectTypeEnum["DiscreteInput"] = 1] = "DiscreteInput";
    ModbusTcpObjectTypeEnum[ModbusTcpObjectTypeEnum["Coil"] = 2] = "Coil";
    ModbusTcpObjectTypeEnum[ModbusTcpObjectTypeEnum["InputRegister"] = 3] = "InputRegister";
    ModbusTcpObjectTypeEnum[ModbusTcpObjectTypeEnum["HoldingRegister"] = 4] = "HoldingRegister";
})(ModbusTcpObjectTypeEnum || (ModbusTcpObjectTypeEnum = {}));
window["ModbusTcpObjectTypeEnum"] = ModbusTcpObjectTypeEnum;
class ModbusTcpClientModuleSelectorViewModel extends OneDasModuleSelectorViewModel {
    constructor(oneDasModuleSelectorMode, moduleSet) {
        super(oneDasModuleSelectorMode, moduleSet);
        this.SettingsTemplateName = ko.observable("ModbusTcp_OneDasModuleSettingsTemplate");
    }
    CreateNewModule() {
        return new ModbusTcpClientModuleViewModel(new ModbusTcpModuleModel(0, ModbusTcpObjectTypeEnum.HoldingRegister, OneDasDataTypeEnum.UINT16, DataDirectionEnum.Input, EndiannessEnum.BigEndian, 1));
    }
}
class ModbusTcpClientModuleViewModel extends ModbusTcpModuleViewModel {
    Validate() {
        super.Validate();
        if (this.DataDirection() === DataDirectionEnum.Output && this.ObjectType() !== ModbusTcpObjectTypeEnum.HoldingRegister) {
            this.ErrorMessage("Only object type 'holding register' is allowed for output modules.");
        }
    }
}
let ViewModelConstructor = (model, identification) => new ModbusTcpClientViewModel(model, identification);
class ModbusTcpClientViewModel extends ExtendedDataGatewayViewModelBase {
    constructor(model, identification) {
        super(model, identification, new ModbusTcpClientModuleSelectorViewModel(OneDasModuleSelectorModeEnum.Duplex, model.ModuleSet.map(modbusTcpModuleModel => new ModbusTcpClientModuleViewModel(modbusTcpModuleModel))));
        EnumerationHelper.Description["ModbusTcpObjectTypeEnum_DiscreteInput"] = "Discrete Input (R)";
        EnumerationHelper.Description["ModbusTcpObjectTypeEnum_Coil"] = "Coil (RW)";
        EnumerationHelper.Description["ModbusTcpObjectTypeEnum_InputRegister"] = "Input Register (R)";
        EnumerationHelper.Description["ModbusTcpObjectTypeEnum_HoldingRegister"] = "Holding Register (RW)";
        this.RemoteIpAddress = ko.observable(model.RemoteIpAddress);
        this.Port = ko.observable(model.Port);
        this.UnitIdentifier = ko.observable(model.UnitIdentifier);
        this.FrameRateDivider = ko.observable(model.FrameRateDivider);
    }
    ExtendModel(model) {
        super.ExtendModel(model);
        model.RemoteIpAddress = this.RemoteIpAddress();
        model.Port = this.Port();
        model.UnitIdentifier = this.UnitIdentifier();
        model.FrameRateDivider = this.FrameRateDivider();
        model.ModuleSet = this.OneDasModuleSelector().ModuleSet().map(moduleModel => moduleModel.ToModel());
    }
}
