using Base.UI;
using System.Collections.Generic;
using WebUI.Controllers;

namespace WebUI.Models.Dashboard
{
    public class DashboardVm: BaseViewModel
    {
        public DashboardVm(IBaseController controller) : base(controller) { }
        public DashboardVm(BaseViewModel baseViewModel) : base(baseViewModel) { }
        public List<DashboardWidget> Widgets { get; set; }
    }
}