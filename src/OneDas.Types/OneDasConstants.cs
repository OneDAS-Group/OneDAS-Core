namespace OneDas
{
    public static class OneDasConstants
    {
        static OneDasConstants()
        {
            OneDasConstants.NativeSampleRate = 100;
            OneDasConstants.ChunkPeriod = 60;
        }

        public static uint NativeSampleRate { get; }
        public static uint ChunkPeriod { get; }
    }
}
