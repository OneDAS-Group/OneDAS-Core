class SlaveChannelViewModel extends DataPortViewModel
{
    public readonly Parent: SlavePdoViewModel
    public readonly Index: number
    public readonly SubIndex: number

    constructor(slaveChannelModel: any, parent: SlavePdoViewModel, dataGateway: DataGatewayViewModelBase)
    {
        super(slaveChannelModel, dataGateway)

        this.Parent = parent
        this.Index = slaveChannelModel.Index
        this.SubIndex = slaveChannelModel.SubIndex
    }

    public GetId(): string
    {
        return this.Parent.Parent.Csa + " / " + this.Parent.Name + " / " + this.Name()
    }
}