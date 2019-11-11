let ViewModelConstructor = (model: any, identification: ExtensionIdentificationViewModel) => new CsvViewModel(model, identification)

class CsvViewModel extends DataWriterViewModelBase
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