namespace OneDas.Plugin
{
    public interface IPluginSupporter
    {
        void Initialize();

        ActionResponse HandleActionRequest(ActionRequest actionRequest);
    }
}
