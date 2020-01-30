class VariableViewModel extends InfoViewModelBase
{
    public Datasets: DatasetViewModel[]
    public VariableNames: string[]
    public VariableGroups: string[]

    constructor(variableModel: any, parent: InfoViewModelBase)
    {
        super(variableModel.Name, parent)

        this.Datasets = variableModel.Datasets.map(datasetInfoModel => new DatasetViewModel(datasetInfoModel, this))
        this.VariableNames = variableModel.VariableNames
        this.VariableGroups = variableModel.VariableGroups
    }  

    // methods
    public GetDisplayName(): string
    {
        return this.VariableNames[this.VariableNames.length - 1]
    }
}