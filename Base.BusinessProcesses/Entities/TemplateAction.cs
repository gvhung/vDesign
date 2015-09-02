using Base.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Base.BusinessProcesses.Entities
{
    public class TemplateAction : BaseObject
    {
        [DetailView(Name = "Наименование", Required = true)] [MaxLength(255)] [ListView]
        public string Title { get; set; }

        //[DetailView(Name = "Макрос", HideLabel = true, TabName = "[2]Макрос")] [PropertyDataType("LuaCode")]
        //public string Macros { get; set; }

        [PropertyDataType("Color")]
        [DetailView(Name = "Цвет")]
        public string Color { get; set; }

        public int TemplateID { get; set; }
        public virtual StageTemplate Template { get; set; }

        //[PropertyDataType("Macro"), DetailView(Name = "Построитель макросов", HideLabel = true, TabName = "[1]Конструктор действий")]
        //public virtual ICollection<TemplateMacroItem> MacrosTemplates { get; set; }

        public TemplateAction()
        {
            Color = "#6f5499";
        }
    }
}