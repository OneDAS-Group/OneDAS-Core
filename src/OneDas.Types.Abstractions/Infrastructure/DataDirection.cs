namespace OneDas.Infrastructure
{
    /// <summary>
    /// Specifies the data direction of the OneDAS data.
    /// </summary>
    public enum DataDirection
    {
        /// <summary>
        /// This is an input to measure data.
        /// </summary>
        Input = 1,

        /// <summary>
        /// This is an output for system control.
        /// </summary>
        Output = 2
    }
}