using System;
using System.Runtime.Serialization;

namespace Base.BusinessProcesses.Exceptions
{
    [Serializable]
    public class BadMacroException : Exception
    {
        public BadMacroException()
        {
        }

        public BadMacroException(string message) : base(message)
        {
        }

        public BadMacroException(string message, Exception inner) : base(message, inner)
        {
        }

        protected BadMacroException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}