namespace Base.BusinessProcesses.Entities
{
    public class TaskStepUser : BaseStepUser
    {
        public int TaskStepID { get; set; }
        public virtual TaskStep TaskStep { get; set; }
    }
}