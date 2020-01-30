abstract class InfoViewModelBase
{
    public Name: string
    public Parent: InfoViewModelBase

    constructor(name: string, parent: InfoViewModelBase)
    {
        this.Name = name
        this.Parent = parent
    }

    public abstract GetDisplayName(): string
}
