namespace OneDas.Infrastructure
{
    /// <summary>
    /// Specifies the sample rate of the OneDAS data.
    /// </summary>
    public enum SampleRate
    {
        /// <summary>
        /// 100 Hz
        /// </summary>
        SampleRate_100 = 1,

        /// <summary>
        /// 25 Hz
        /// </summary>
        SampleRate_25 = 4,

        /// <summary>
        /// 5 Hz
        /// </summary>
        SampleRate_5 = 20,

        /// <summary>
        /// 1 Hz
        /// </summary>
        SampleRate_1 = 100
    }
}