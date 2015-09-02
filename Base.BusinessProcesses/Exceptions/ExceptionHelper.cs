namespace Base.BusinessProcesses.Exceptions
{
    public static class ExceptionHelper
    {
        public static ActionInvokeException ActionInvokeException(string message)
        {
            return new ActionInvokeException(message);
        }

        public static WorkflowSaveException WorkflowSaveException(string message)
        {
            return new WorkflowSaveException(message);
        }

        public static ActionNotFoundException ActionNotFoundException(string message)
        {
            return new ActionNotFoundException(message);
        }

        public static WorkflowException CriticalError(string message)
        {
            return new WorkflowException(message);
        }
    }
}