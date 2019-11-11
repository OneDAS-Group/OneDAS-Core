abstract class SlaveExtensionViewModelBase extends ExtensionViewModelBase
{
    public SlaveInfo: SlaveInfoViewModel
}

window[SlaveExtensionViewModelBase.name] = SlaveExtensionViewModelBase