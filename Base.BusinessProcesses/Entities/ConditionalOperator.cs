using System.ComponentModel;

namespace Base.BusinessProcesses.Entities
{
    public enum ConditionalOperator
    {
        [Description("Хотя бы один")]
        One = 0,
        [Description("Все")]
        All = 1
    }
}