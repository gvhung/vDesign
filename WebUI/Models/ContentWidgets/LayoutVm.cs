using Base.Attributes;

namespace WebUI.Models.ContentWidgets
{
    public class LayoutVm : WidgetVm
    {
        [DetailView("Минимальный размер")]
        public int MinHeight { get; set; }

        public LayoutVm()
        {
            this.MinHeight = 300;
        }
    }
}