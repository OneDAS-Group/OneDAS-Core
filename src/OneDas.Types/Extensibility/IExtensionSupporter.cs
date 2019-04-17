namespace OneDas.Extensibility
{
    public interface IExtensionSupporter
    {
        void Initialize();
        ActionResponse HandleActionRequest(ActionRequest actionRequest);
    }
}
