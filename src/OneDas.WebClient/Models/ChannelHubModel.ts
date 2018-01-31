class ChannelHubModel
{
    public Name: string
    public Group: string
    public DataType: OneDasDataTypeEnum
    public SampleRate: SampleRateEnum
    public Guid: string
    public CreationDateTime: string
    public Unit: string
    public TransferFunctionSet: any[]

    public SerializerDataInputId: string
    public SerializerDataOutputIdSet: string[]

    constructor(name: string, group: string, dataType: OneDasDataTypeEnum, sampleRate: SampleRateEnum)
    {
        this.Name = name;
        this.Group = group;
        this.DataType = dataType;
        this.SampleRate = sampleRate;
        this.Guid = Guid.NewGuid()
        this.CreationDateTime = new Date().toISOString()
        this.Unit = ""
        this.TransferFunctionSet = []

        this.SerializerDataInputId = ""
        this.SerializerDataOutputIdSet = []
    }
}