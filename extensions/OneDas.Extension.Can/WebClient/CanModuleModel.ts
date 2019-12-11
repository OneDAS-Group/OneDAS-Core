class CanModuleModel extends OneDasModuleModel
{
    public Identifier: number
    public FrameFormat: CanFrameFormatEnum

    constructor(identifier: number, frameFormat: CanFrameFormatEnum, dataType: OneDasDataTypeEnum, dataDirection: DataDirectionEnum, endianness: EndiannessEnum, size: number)
    {
        super(dataType, dataDirection, endianness, size)

        this.Identifier = identifier
        this.FrameFormat = frameFormat
    }
}
