using System.ComponentModel;

namespace Base.Task.Entities
{
    public enum TaskType
    {
        [Description("Напоминание")]
        Task = 0,
        [Description("Веха")]
        Note = 1
    }
}
