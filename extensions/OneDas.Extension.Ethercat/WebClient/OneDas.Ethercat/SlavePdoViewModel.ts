class SlavePdoViewModel
{
    public readonly Parent: SlaveInfoViewModel
    public readonly Name: string
    public readonly Index: number
    public readonly SyncManager: number
    public readonly VariableSet: SlaveVariableViewModel[]
    public readonly CompactView: KnockoutObservable<boolean>

    constructor(slavePdoModel: any, parent: SlaveInfoViewModel, dataGateway: DataGatewayViewModelBase)
    {
        this.Parent = parent
        this.Name = slavePdoModel.Name
        this.Index = slavePdoModel.Index
        this.SyncManager = slavePdoModel.SyncManager
        this.VariableSet = slavePdoModel.VariableSet.map(x => new SlaveVariableViewModel(x, this, dataGateway))
        this.CompactView = ko.observable(this.VariableSet.length === 1)
    }
}