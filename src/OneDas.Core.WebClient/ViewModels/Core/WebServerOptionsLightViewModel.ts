class WebServerOptionsLightViewModel
{
    public OneDasName: string
    public AspBaseUrl: string
    public BaseDirectoryPath: string
    public PackageSourceSet: OneDasPackageSourceViewModel[]

    constructor(webServerOptionsLight: any)
    {
        this.OneDasName = webServerOptionsLight.OneDasName
        this.AspBaseUrl = webServerOptionsLight.AspBaseUrl
        this.BaseDirectoryPath = webServerOptionsLight.BaseDirectoryPath
        this.PackageSourceSet = webServerOptionsLight.PackageSourceSet.map(packageSourceModel => new OneDasPackageSourceViewModel(packageSourceModel))
    }
}