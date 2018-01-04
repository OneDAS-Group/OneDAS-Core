namespace OneDas.Types.Settings
{
    public static class GlobalSettings
    {
        static GlobalSettings()
        {
            GlobalSettings.EventLogName = "OneDas";
            GlobalSettings.NativeSampleRate = 100;
            GlobalSettings.ChunkPeriod = 60;
        }

        public static string EventLogName { get; }
        public static uint NativeSampleRate { get; }
        public static uint ChunkPeriod { get; }
    }
}
