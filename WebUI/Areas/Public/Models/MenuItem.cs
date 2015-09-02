using System.Collections.Generic;

namespace WebUI.Areas.Public.Models
{
    public class MenuItemVm
    {
        public MenuItemVm()
        {
            SubmenuItems = new List<MenuItemVm>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<MenuItemVm> SubmenuItems { get; set; }
    }
}