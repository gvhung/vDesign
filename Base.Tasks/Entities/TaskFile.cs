using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Task.Entities
{
    [Table("TaskFiles")]
    public class TaskFile : FileData
    {
        public int TaskID { get; set; }
        public Task Task { get; set; }
    }
}
