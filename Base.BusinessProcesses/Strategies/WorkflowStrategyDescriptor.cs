using Base.BusinessProcesses.Attributes;
using System;
using System.Reflection;

namespace Base.BusinessProcesses.Strategies
{
    public class WorkflowStrategyDescriptor
    {
        internal WorkflowStrategyDescriptor(Type type)
        {
            var attr = type.GetCustomAttribute<WorkflowStrategyAttribute>();
            if (attr != null)
            {
                Identifier = attr.Identifier;
                UserFriendlyName = attr.UserFriendlyName ?? Identifier;
            }
            else
            {
                Identifier = Guid.NewGuid().ToString("D");
                UserFriendlyName = Identifier;
            }

            Type = type;
        }

        public string UserFriendlyName { get; set; }
        public string Identifier { get; set; }
        public Type Type { get; set; }
    }

    public class WorkflowCommonStrategyDescriptor : WorkflowStrategyDescriptor
    {
        internal WorkflowCommonStrategyDescriptor(Type type)
            : base(type)
        {
        }

        public Type EntityType { get; internal set; }
    }
}