class VariableInfoViewModel extends HdfElementViewModelBase
{
    public DatasetInfoSet: DatasetInfoViewModel[]
    public VariableNameSet: string[]
    public VariableGroupSet: string[]

    constructor(variableInfoModel: any, parent: HdfElementViewModelBase)
    {
        super(variableInfoModel.Name, parent)

        let datasetInfoModelSet: any[]

        datasetInfoModelSet = variableInfoModel.DatasetInfoSet

        this.DatasetInfoSet = Object.keys(datasetInfoModelSet).map(key => new DatasetInfoViewModel(datasetInfoModelSet[key], this))
        this.VariableNameSet = variableInfoModel.VariableNameSet
        this.VariableGroupSet = variableInfoModel.VariableGroupSet
    }  

    // methods
    public GetDisplayName(): string
    {
        return this.VariableNameSet[this.VariableNameSet.length - 1]
    }
}