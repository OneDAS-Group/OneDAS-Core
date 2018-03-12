class VariableInfoViewModel extends HdfElementViewModelBase
{
    public DatasetInfoSet: DatasetInfoViewModel[]
    public VariableNameSet: string[]
    public VariableGroupSet: string[]

    constructor(variableInfoModel: any, parent: HdfElementViewModelBase)
    {
        super(variableInfoModel.Name, parent)

        this.DatasetInfoSet = variableInfoModel.DatasetInfoSet.map(datasetInfoModel => new DatasetInfoViewModel(datasetInfoModel, this))
        this.VariableNameSet = variableInfoModel.VariableNameSet
        this.VariableGroupSet = variableInfoModel.VariableGroupSet
    }  

    // methods
    public GetDisplayName(): string
    {
        return this.VariableNameSet[this.VariableNameSet.length - 1]
    }
}