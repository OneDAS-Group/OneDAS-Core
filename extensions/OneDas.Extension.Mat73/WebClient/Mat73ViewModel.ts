let ViewModelConstructor = (model: any, identification: ExtensionIdentificationViewModel) => new Mat73ViewModel(model, identification)

class Mat73ViewModel extends DataWriterViewModelBase
{
    constructor(model, identification: ExtensionIdentificationViewModel)
    {
        super(model, identification)
    }

    // methods
    public async InitializeAsync()
    {
        //
    }

    public ExtendModel(model: any)
    {
        super.ExtendModel(model)
    }
}