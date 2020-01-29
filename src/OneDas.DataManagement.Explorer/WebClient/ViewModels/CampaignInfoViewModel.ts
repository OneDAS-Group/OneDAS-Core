/// <reference path="HdfElementViewModelBase.ts"/>

class CampaignInfoViewModel extends HdfElementViewModelBase
{
    public readonly Variables: VariableViewModel[]
    public readonly GroupedVariables: ObservableGroup<VariableViewModel>[]

    constructor(campaignModel: any)
    {
        super(campaignModel.Name, null)

        this.Variables = campaignModel.Variables.map(variableModel => new VariableViewModel(variableModel, this)).sort((a, b) => a.GetDisplayName().localeCompare(b.GetDisplayName()))
        
        this.GroupedVariables = ObservableGroupBy(
            this.Variables,
            variableInfo => variableInfo.VariableNames[variableInfo.VariableNames.length - 1],
            variableInfo => variableInfo.VariableGroups[variableInfo.VariableGroups.length - 1],
            ""
        )
    }  

    public GetDisplayName()
    {
        return this.Name.slice(1).replace(new RegExp("/", 'g'), " / ")
    }
}