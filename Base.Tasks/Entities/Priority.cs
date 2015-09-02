using System.ComponentModel;

namespace Base.Task.Entities
{
    public enum Priority
    {
        [Description("Высокий")]
        High = 0,
        [Description("Нормальный")]
        Normal = 1,
        [Description("Низкий")]
        Low = 2
    }
}