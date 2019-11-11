class SlaveVariableViewModel extends DataPortViewModel
{
    public readonly Parent: SlavePdoViewModel
    public readonly Index: number
    public readonly SubIndex: number

    constructor(slaveVariableModel: any, parent: SlavePdoViewModel, dataGateway: DataGatewayViewModelBase)
    {
        super(slaveVariableModel, dataGateway)

        this.Parent = parent
        this.Index = slaveVariableModel.Index
        this.SubIndex = slaveVariableModel.SubIndex
    }

    public GetId(): string
    {
        return this.Parent.Parent.Csa + " / " + this.Parent.Name + " / " + this.Name()
    }
}