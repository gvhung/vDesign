namespace Base.BusinessProcesses.Entities
{
    public class TemplateMacroItem : BaseMacroItem
    {
        public int? ActionID { get; set; }
        public virtual TemplateAction Action { get; set; }
    }
}