using Framework.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.BusinessProcesses.Entities
{
    [EnableFullTextSearch]
    [Table("BPTasks")]
    // ReSharper disable once InconsistentNaming
    public class BPTask : Task.Entities.Task
    {
        public BPTask()
        {
            Auto = true;
        }
        public bool ForcedTask { get; set; } //Если создал куратор или админ когда взял на итсполнение
    }

    [EnableFullTextSearch]
    [Table("RelatedTasks")]
    public class RelatedTask : Task.Entities.Task
    {      
        public int TaskStepID { get; set; }
        public virtual TaskStep TaskStep { get; set; }
    }
}