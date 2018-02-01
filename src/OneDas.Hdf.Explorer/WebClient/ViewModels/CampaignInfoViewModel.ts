/// <reference path="HdfElementViewModelBase.ts"/>

class CampaignInfoViewModel extends HdfElementViewModelBase
{
    public readonly VariableInfoSet: VariableInfoViewModel[]
    public readonly GroupedVariableInfoSet: ObservableGroup<VariableInfoViewModel>[]

    constructor(campaignInfoModel: any)
    {
        super(campaignInfoModel.Name, null)

        let variableInfoModelSet: any[]

        variableInfoModelSet = campaignInfoModel.VariableInfoSet
        this.VariableInfoSet = Object.keys(variableInfoModelSet).map(key => new VariableInfoViewModel(variableInfoModelSet[key], this)).sort((a, b) => a.GetDisplayName().localeCompare(b.GetDisplayName()))
        
        this.GroupedVariableInfoSet = ObservableGroupBy(
            this.VariableInfoSet,
            variableInfo => variableInfo.VariableNameSet[variableInfo.VariableNameSet.length - 1],
            variableInfo => variableInfo.VariableGroupSet[variableInfo.VariableGroupSet.length - 1],
            ""
        )
    }  

    public GetDisplayName()
    {
        return this.Name.slice(1).replace(new RegExp("/", 'g'), " / ")
    }
}