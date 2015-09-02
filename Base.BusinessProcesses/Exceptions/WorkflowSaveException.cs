using System;
using System.Runtime.Serialization;

namespace Base.BusinessProcesses.Exceptions
{
    [Serializable]
    public class WorkflowSaveException : WorkflowException
    {
        public WorkflowSaveException()
        {
        }

        public WorkflowSaveException(string message) : base(message)
        {
        }

        public WorkflowSaveException(string message, Exception inner) : base(message, inner)
        {
        }

        protected WorkflowSaveException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}