using System;

namespace Google.Authenticator
{
    public class MissingDependencyException : Exception
    {
        public MissingDependencyException(string message) : base(message)
        {
        }

        public MissingDependencyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}