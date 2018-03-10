class ControlViewModel extends WorkspaceBase
{
    public CampaignDescriptionSet: KnockoutObservableArray<OneDasCampaignDescriptionViewModel>

    constructor(activeProject: KnockoutObservable<OneDasProjectViewModel>)
    {
        super('control', 'Control', 'control.html', activeProject)

        this.CampaignDescriptionSet = ko.observableArray<OneDasCampaignDescriptionViewModel>()
    }

    // commands
    public GetCampaignDescriptions = async () =>
    {
        let campaignDescriptionSet: any[]

        try
        {
            campaignDescriptionSet = await ConnectionManager.InvokeWebClientHub('GetCampaignDescriptions')
            this.CampaignDescriptionSet(campaignDescriptionSet.map(campaignDescription => new OneDasCampaignDescriptionViewModel(campaignDescription)))
        }
        catch (e)
        {
            alert(e.message)
        }
    }

    public ActivateProject = async (campaignDescriptionViewModel: OneDasCampaignDescriptionViewModel) =>
    {
        try
        {
            await ConnectionManager.InvokeWebClientHub("ActivateProject", campaignDescriptionViewModel.ToModel())
        }
        catch (e)
        {
            alert(e.message)
        }
    }

    public StartOneDas = async () =>
    {
        try
        {
            await ConnectionManager.InvokeWebClientHub("StartOneDas")
        }
        catch (e)
        {
            alert(e.message)
        }
    }

    public PauseOneDas = async () => {
        try {
            await ConnectionManager.InvokeWebClientHub("PauseOneDas")
        }
        catch (e) {
            alert(e.message)
        }
    }

    public StopOneDas = async () =>
    {
        try
        {
            if (confirm("OneDAS engine will be stopped.")) {

                await ConnectionManager.InvokeWebClientHub("StopOneDas")

                this.ActiveProject(null)
            }
        }
        catch (e)
        {
            alert(e.message)
        }
    }
}