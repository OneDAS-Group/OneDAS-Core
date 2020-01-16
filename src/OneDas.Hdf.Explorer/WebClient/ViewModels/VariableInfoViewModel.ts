class VariableInfoViewModel extends HdfElementViewModelBase
{
    public DatasetInfos: DatasetInfoViewModel[]
    public VariableNames: string[]
    public variableGroups: string[]

    constructor(variableModel: any, parent: HdfElementViewModelBase)
    {
        super(variableModel.Name, parent)

        this.DatasetInfos = variableModel.DatasetInfos.map(datasetInfoModel => new DatasetInfoViewModel(datasetInfoModel, this))
        this.VariableNames = variableModel.VariableNames
        this.variableGroups = variableModel.variableGroups
    }  

    // methods
    public GetDisplayName(): string
    {
        return this.VariableNames[this.VariableNames.length - 1]
    }
}