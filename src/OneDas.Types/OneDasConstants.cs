namespace OneDas
{
    public static class OneDasConstants
    {
        static OneDasConstants()
        {
            OneDasConstants.NativeSampleRate = 100;
            OneDasConstants.ChunkPeriod = 60;
            OneDasConstants.FileLoggerTemplate = "{Timestamp:yyyy-MM-ddTHH:mm:ss} {context} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}";
        }

        public static uint NativeSampleRate { get; }
        public static uint ChunkPeriod { get; }
        public static string FileLoggerTemplate { get; }
    }
}
