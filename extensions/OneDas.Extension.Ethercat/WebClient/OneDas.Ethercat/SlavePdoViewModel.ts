class SlavePdoViewModel
{
    public readonly Parent: SlaveInfoViewModel
    public readonly Name: string
    public readonly Index: number
    public readonly SyncManager: number
    public readonly ChannelSet: SlaveChannelViewModel[]
    public readonly CompactView: KnockoutObservable<boolean>

    constructor(slavePdoModel: any, parent: SlaveInfoViewModel, dataGateway: DataGatewayViewModelBase)
    {
        this.Parent = parent
        this.Name = slavePdoModel.Name
        this.Index = slavePdoModel.Index
        this.SyncManager = slavePdoModel.SyncManager
        this.ChannelSet = slavePdoModel.ChannelSet.map(x => new SlaveChannelViewModel(x, this, dataGateway))
        this.CompactView = ko.observable(this.ChannelSet.length === 1)
    }
}