namespace Base.BusinessProcesses.Entities
{
    public abstract class BaseMacroItem : BaseObject
    {
        public BaseMacroItem()
        {
        }

        public BaseMacroItem(BaseMacroItem src)
        {
            Member = src.Member;
            Value = src.Value;
        }

        public string Member { get; set; }
        public string Value { get; set; }
    }
}