abstract class ModbusModuleViewModel extends OneDasModuleViewModel
{
    public StartingAddress: KnockoutObservable<number>
    public ObjectType: KnockoutObservable<ModbusObjectTypeEnum>
    public ObjectTypeSet: KnockoutObservableArray<ModbusObjectTypeEnum>

    constructor(modbusModuleModel: ModbusModuleModel)
    {
        super(modbusModuleModel)

        switch (this.DataDirection())
        {
            case DataDirectionEnum.Input:

                this.ObjectTypeSet = ko.observableArray<ModbusObjectTypeEnum>(EnumerationHelper.GetEnumValues("ModbusObjectTypeEnum").filter(objectType => objectType >= 3))

                break

            case DataDirectionEnum.Output:

                this.ObjectTypeSet = ko.observableArray<ModbusObjectTypeEnum>(EnumerationHelper.GetEnumValues("ModbusObjectTypeEnum").filter(objectType => objectType === ModbusObjectTypeEnum.HoldingRegister))

                break
        }

        this.StartingAddress = ko.observable<number>(modbusModuleModel.StartingAddress)
        this.ObjectType = ko.observable<ModbusObjectTypeEnum>(modbusModuleModel.ObjectType)

        this.StartingAddress.subscribe(newValue => this.OnPropertyChanged())
        this.ObjectType.subscribe(newValue => this.OnPropertyChanged())

        // improve: better would be server side generation of correct module
        if (!this._model.$type)
        {
            this._model.$type = "OneDas.Extension.Modbus.ModbusModule, OneDas.Extension.Modbus"
        }
    }

    public Validate()
    {
        super.Validate()

        switch (this.DataDirection())
        {
            case DataDirectionEnum.Input:

                switch (this.ObjectType())
                {
                    case ModbusObjectTypeEnum.HoldingRegister:
                    case ModbusObjectTypeEnum.InputRegister:

                        if (this.GetByteCount() > 125 * 2)
                        {
                            this.ErrorMessage("The number of registers per module must be within range 0..125.")
                        }

                        break

                    case ModbusObjectTypeEnum.Coil:
                    case ModbusObjectTypeEnum.DiscreteInput:

                        if (this.GetByteCount() > 2000 * 2) {
                            this.ErrorMessage("The number of registers per module must be within range 0..2000.")
                        }

                        break
                }

                break

            case DataDirectionEnum.Output:

                switch (this.ObjectType())
                {
                    case ModbusObjectTypeEnum.HoldingRegister:

                        if (this.GetByteCount() > 123 * 2) {
                            this.ErrorMessage("The number of registers per module must be within range 0..123.")
                        }

                        break
                }

                break
        }

        if (this.StartingAddress() < 0)
        {
            this.ErrorMessage("The starting address of a module must be within range 0..65535.")
        }

        if (this.StartingAddress() + this.Size() > 65536)
        {
            this.ErrorMessage("Starting address + module size exceeds register address range (0..65535).")
        }
    }

    public ToString()
    {
        return super.ToString() + ' - ' + EnumerationHelper.GetEnumLocalization("ModbusObjectTypeEnum", this.ObjectType()) + " - address: " + this.StartingAddress()
    }

    public ExtendModel(model: any)
    {
        super.ExtendModel(model)

        model.StartingAddress = <number>this.StartingAddress()
        model.ObjectType = <ModbusObjectTypeEnum>this.ObjectType()
    }
}