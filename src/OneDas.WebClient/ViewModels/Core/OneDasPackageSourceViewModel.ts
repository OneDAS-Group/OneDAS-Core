class OneDasPackageSourceViewModel
{
    public Name: string
    public Address: string

    constructor(oneDasPackageSource: any)
    {
        this.Name = oneDasPackageSource.Name
        this.Address = oneDasPackageSource.Address
    }
}