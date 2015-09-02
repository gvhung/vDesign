using System;

namespace Base.BusinessProcesses.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    internal sealed class WorkflowStrategyAttribute : Attribute
    {
        public WorkflowStrategyAttribute(string identifier, string userFriendlyName)
        {
            Identifier = identifier;
            UserFriendlyName = userFriendlyName;
        }

        public string Identifier { get; private set; }
        public string UserFriendlyName { get; set; }
    }
}