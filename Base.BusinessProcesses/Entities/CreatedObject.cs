namespace Base.BusinessProcesses.Entities
{
    public class CreatedObject : BaseObject
    {
        public int ObjectID { get; set; }
        public string Type { get; set; }

        
        public virtual CreateObjectStep ObjectStep { get; set; }
        
        public int? ObjectStepID { get; set; }
    }
}