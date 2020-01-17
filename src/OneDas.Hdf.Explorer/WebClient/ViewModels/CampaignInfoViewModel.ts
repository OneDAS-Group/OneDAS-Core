/// <reference path="HdfElementViewModelBase.ts"/>

class CampaignInfoViewModel extends HdfElementViewModelBase
{
    public readonly VariableInfos: VariableInfoViewModel[]
    public readonly GroupedVariableInfos: ObservableGroup<VariableInfoViewModel>[]

    constructor(campaignInfoModel: any)
    {
        super(campaignInfoModel.Name, null)

        this.VariableInfos = campaignInfoModel.VariableInfos.map(variableModel => new VariableInfoViewModel(variableModel, this)).sort((a, b) => a.GetDisplayName().localeCompare(b.GetDisplayName()))
        
        this.GroupedVariableInfos = ObservableGroupBy(
            this.VariableInfos,
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