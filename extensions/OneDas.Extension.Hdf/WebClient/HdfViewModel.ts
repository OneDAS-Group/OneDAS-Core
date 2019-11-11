let ViewModelConstructor = (model: any, identification: ExtensionIdentificationViewModel) => new HdfViewModel(model, identification)

class HdfViewModel extends DataWriterViewModelBase
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