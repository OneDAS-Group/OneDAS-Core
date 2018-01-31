class WebServerOptionsLightViewModel
{
    public OneDasName: string
    public AspBaseUrl: string
    public BaseDirectoryPath: string

    constructor(webServerOptionsLight: any)
    {
        this.OneDasName = webServerOptionsLight.OneDasName
        this.AspBaseUrl = webServerOptionsLight.AspBaseUrl
        this.BaseDirectoryPath = webServerOptionsLight.BaseDirectoryPath
    }
}