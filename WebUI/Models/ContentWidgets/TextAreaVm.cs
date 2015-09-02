using Base.Attributes;

namespace WebUI.Models.ContentWidgets
{
    public class TextAreaVm : WidgetVm
    {
        [DetailView("Текст", HideLabel = true)]
        [PropertyDataType(PropertyDataType.Html)]
        public string Value { get; set; }


        public TextAreaVm()
        {
            this.Value = "Введите текст";
        }
    }
}