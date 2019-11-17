class ModbusTcpModuleModel extends OneDasModuleModel
{
    public StartingAddress: number
    public ObjectType: ModbusTcpObjectTypeEnum

    constructor(startingAddress: number, objectType: ModbusTcpObjectTypeEnum, dataType: OneDasDataTypeEnum, dataDirection: DataDirectionEnum, endianness: EndiannessEnum, size: number)
    {
        super(dataType, dataDirection, endianness, size)

        this.StartingAddress = startingAddress
        this.ObjectType = objectType
    }
}
