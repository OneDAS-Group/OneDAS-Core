abstract class ModbusTcpModuleViewModel extends OneDasModuleViewModel
{
    public StartingAddress: KnockoutObservable<number>
    public ObjectType: KnockoutObservable<ModbusTcpObjectTypeEnum>
    public ObjectTypeSet: KnockoutObservableArray<ModbusTcpObjectTypeEnum>

    constructor(modbusTcpModuleModel: ModbusTcpModuleModel)
    {
        super(modbusTcpModuleModel)

        switch (this.DataDirection())
        {
            case DataDirectionEnum.Input:

                this.ObjectTypeSet = ko.observableArray<ModbusTcpObjectTypeEnum>(EnumerationHelper.GetEnumValues("ModbusTcpObjectTypeEnum").filter(objectType => objectType >= 3))

                break

            case DataDirectionEnum.Output:

                this.ObjectTypeSet = ko.observableArray<ModbusTcpObjectTypeEnum>(EnumerationHelper.GetEnumValues("ModbusTcpObjectTypeEnum").filter(objectType => objectType === ModbusTcpObjectTypeEnum.HoldingRegister))

                break
        }

        this.StartingAddress = ko.observable<number>(modbusTcpModuleModel.StartingAddress)
        this.ObjectType = ko.observable<ModbusTcpObjectTypeEnum>(modbusTcpModuleModel.ObjectType)

        this.StartingAddress.subscribe(newValue => this.OnPropertyChanged())
        this.ObjectType.subscribe(newValue => this.OnPropertyChanged())

        // improve: better would be server side generation of correct module
        if (!this._model.$type)
        {
            this._model.$type = "OneDas.Extension.ModbusTcp.ModbusTcpModule, OneDas.Extension.ModbusTcp"
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
                    case ModbusTcpObjectTypeEnum.HoldingRegister:
                    case ModbusTcpObjectTypeEnum.InputRegister:

                        if (this.GetByteCount() > 125 * 2)
                        {
                            this.ErrorMessage("The number of registers per module must be within range 0..125.")
                        }

                        break

                    case ModbusTcpObjectTypeEnum.Coil:
                    case ModbusTcpObjectTypeEnum.DiscreteInput:

                        if (this.GetByteCount() > 2000 * 2) {
                            this.ErrorMessage("The number of registers per module must be within range 0..2000.")
                        }

                        break
                }

                break

            case DataDirectionEnum.Output:

                switch (this.ObjectType())
                {
                    case ModbusTcpObjectTypeEnum.HoldingRegister:

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
        return super.ToString() + ' - ' + EnumerationHelper.GetEnumLocalization("ModbusTcpObjectTypeEnum", this.ObjectType()) + " - address: " + this.StartingAddress()
    }

    public ExtendModel(model: any)
    {
        super.ExtendModel(model)

        model.StartingAddress = <number>this.StartingAddress(),
        model.ObjectType = <ModbusTcpObjectTypeEnum>this.ObjectType()
    }
}