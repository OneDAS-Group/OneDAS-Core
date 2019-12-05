class ModbusModuleModel extends OneDasModuleModel
{
    public StartingAddress: number
    public ObjectType: ModbusObjectTypeEnum

    constructor(startingAddress: number, objectType: ModbusObjectTypeEnum, dataType: OneDasDataTypeEnum, dataDirection: DataDirectionEnum, endianness: EndiannessEnum, size: number)
    {
        super(dataType, dataDirection, endianness, size)

        this.StartingAddress = startingAddress
        this.ObjectType = objectType
    }
}
