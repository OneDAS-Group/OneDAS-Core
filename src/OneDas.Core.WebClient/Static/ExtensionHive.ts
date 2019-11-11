class ExtensionHive
{
    // fields
    public static ExtensionIdentificationSet: Map<string, ExtensionIdentificationViewModel[]>

    // constructors
    static Initialize = () =>
    {
        ExtensionHive.ExtensionIdentificationSet = new Map<string, ExtensionIdentificationViewModel[]>()
    }   

    static FindExtensionIdentification = (extensionTypeName: string, extensionId: string) =>
    {
        return ExtensionHive.ExtensionIdentificationSet.get(extensionTypeName).find(extensionIdentification => extensionIdentification.Id === extensionId);
    }
}