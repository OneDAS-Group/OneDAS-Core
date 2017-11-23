namespace OneDas.Infrastructure
{
    /// <summary>
    /// The exception that is thrown, when the validation of the configuration was not successful.
    /// </summary>
    public class ValidationException : OneDasException
    {
        /// <summary>
        /// Initializes a new instance of the ConfigurationValidationException class with the specified message.
        /// </summary>
        /// <param name="message">The description of the configuration validation exception.</param>
        public ValidationException(string message) : base(message)
        {
        }
    }
}