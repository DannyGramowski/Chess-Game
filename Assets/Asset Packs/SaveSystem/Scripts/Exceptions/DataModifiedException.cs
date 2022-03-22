using System;

namespace SaveSystem.Exceptions
{
    class DataModifiedException : Exception
    {
        public DataModifiedException(string message) : base(message)
        {
        }

        public DataModifiedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

