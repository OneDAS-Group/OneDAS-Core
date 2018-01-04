class SlimOneDasSettingsViewModel
{
    public OneDasName: string
    public AspBaseUrl: string
    public BaseDirectoryPath: string

    constructor(slimOneDasSettings: any)
    {
        this.OneDasName = slimOneDasSettings.OneDasName
        this.AspBaseUrl = slimOneDasSettings.AspBaseUrl
        this.BaseDirectoryPath = slimOneDasSettings.BaseDirectoryPath
    }
}