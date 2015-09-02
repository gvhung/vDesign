using System;
using System.Runtime.Serialization;

namespace Base.Censorship
{
    [Serializable]
    public class CensorshipException : Exception
    {
        public CensorshipException()
        {
        }

        public CensorshipException(string message) : base(message)
        {
        }

        public CensorshipException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CensorshipException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}