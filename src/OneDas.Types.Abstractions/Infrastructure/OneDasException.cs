using System;

namespace OneDas.Infrastructure
{
    /// <summary>
    /// The exception that is thrown, when the validation of the configuration was not successful.
    /// </summary>
    public class OneDasException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ConfigurationValidationException class with the specified message.
        /// </summary>
        /// <param name="message">The description of the configuration validation exception.</param>
        public OneDasException(string message) : base(message)
        {
        }
    }
}