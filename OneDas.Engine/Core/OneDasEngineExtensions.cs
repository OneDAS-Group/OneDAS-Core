using OneDas.Infrastructure;

namespace OneDas.Engine.Core
{
    public static class OneDasEngineExtensions
    {
        public static void Start(this OneDasEngine oneDasEngine)
        {
            oneDasEngine.OneDasState = OneDasState.Run;
        }

        public static void Stop(this OneDasEngine oneDasEngine)
        {
            oneDasEngine.OneDasState = OneDasState.Run;
        }

        public static void AcknowledgeError(this OneDasEngine oneDasEngin)
        {
            oneDasEngin.OneDasState = OneDasState.Initialization;
            oneDasEngin.OneDasState = OneDasState.Unconfigured;
        }
    }
}
