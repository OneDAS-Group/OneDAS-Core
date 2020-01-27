abstract class HdfElementViewModelBase
{
    public Name: string
    public Parent: HdfElementViewModelBase

    constructor(name: string, parent: HdfElementViewModelBase)
    {
        this.Name = name
        this.Parent = parent
    }

    public abstract GetDisplayName(): string
}
