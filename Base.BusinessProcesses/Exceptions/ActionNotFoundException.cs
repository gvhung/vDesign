using System;
using System.Runtime.Serialization;

namespace Base.BusinessProcesses.Exceptions
{
    [Serializable]
    public class ActionNotFoundException : WorkflowException
    {
        public ActionNotFoundException()
        {
        }

        public ActionNotFoundException(string message) : base(message)
        {
        }

        public ActionNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ActionNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}