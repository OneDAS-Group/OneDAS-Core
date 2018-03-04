using System;
using System.Reflection;

namespace OneDas.Common
{
    public static class EngineUtilities
    {
        public static Exception UnwrapException(Exception exception)
        {
            if (exception is TargetInvocationException || exception is AggregateException)
            {
                return exception.InnerException;
            }

            return exception;
        }
    }
}
