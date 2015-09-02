
namespace WebUI.Models.Dashboard.Widgets
{
    public class ActiveUserVm : DashboardWidgetVm
    {   
    }

    public class ActiveUserItem
    {
        public string Login { get; set; }
        public int CountRequest { get; set; }
    }
}