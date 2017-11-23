abstract class PluginViewModelBase implements IModelizable
{
    public Description: PluginDescriptionViewModel
    public PluginIdentification: PluginIdentificationViewModel
    public IsInSettingsMode: KnockoutObservable<boolean>

    private Model: any

    constructor(pluginSettingsModel: any, pluginIdentification: PluginIdentificationViewModel)
    {
        this.Model = pluginSettingsModel
        this.Description = new PluginDescriptionViewModel(pluginSettingsModel.Description)
        this.PluginIdentification = pluginIdentification
        this.IsInSettingsMode = ko.observable<boolean>(false)
    }

    // methods
    public abstract async InitializeAsync(): Promise<any>

    public SendActionRequest = async (instanceId: number, methodName: string, data: any) =>
    {
        return <ActionResponse> await ConnectionManager.InvokeBroadcaster("RequestAction", new ActionRequest(this.Description.Id, instanceId, methodName, data))
    }

    public ExtendModel(model: any)
    {
        //
    }

    public ToModel()
    {
        let model: any = {
            $type: <string>this.Model.$type,
            Description: <PluginDescriptionViewModel>this.Description.ToModel()
        }

        this.ExtendModel(model)

        return model
    }

    // commands
    public EnableSettingsMode = () =>
    {
        this.IsInSettingsMode(true)
    }

    public DisableSettingsMode = () =>
    {
        this.IsInSettingsMode(false)
    }

    public ToggleSettingsMode = () =>
    {
        this.IsInSettingsMode(!this.IsInSettingsMode())
    }
}