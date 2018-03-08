class ChannelHubModel
{
    public Name: string
    public Group: string
    public DataType: OneDasDataTypeEnum
    public Guid: string
    public CreationDateTime: string
    public Unit: string
    public TransferFunctionSet: any[]
    public AssociatedDataInputId: string
    public AssociatedDataOutputIdSet: string[]

    constructor(name: string, group: string, dataType: OneDasDataTypeEnum)
    {
        this.Name = name;
        this.Group = group;
        this.DataType = dataType;
        this.Guid = Guid.NewGuid()
        this.CreationDateTime = new Date().toISOString()
        this.Unit = ""
        this.TransferFunctionSet = []
        this.AssociatedDataInputId = ""
        this.AssociatedDataOutputIdSet = []
    }
}