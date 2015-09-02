using System;
using System.Runtime.Serialization;

namespace Base.Exceptions
{
    [Serializable]
    public class UpdateConcurrencyException : Exception
    {
        public UpdateConcurrencyException()
        {
        }

        public UpdateConcurrencyException(string message) : base(message)
        {
        }

        public UpdateConcurrencyException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UpdateConcurrencyException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}