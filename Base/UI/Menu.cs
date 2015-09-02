using Base.Security;
using System.Collections.Generic;

namespace Base.UI
{
    public class Menu
    {
        public Menu()
        {
            Items = new List<MenuItem>();
        }

        public string DefaultIcon { get; set; }

        public List<MenuItem> Items { get; set; }   
    }   

    public class MenuItem
    {
        public MenuItem()
        {
            Items = new List<MenuItem>();
        }

        public string Title { get; set; }
        public string Icon { get; set; }
        public string Mnemonic { get; set; }
        public string URL { get; set; }
        public List<MenuItem> Items { get; set; }
        public MenuItem Parent { get; set; }
        public SystemRole? ForSystemRole { get; set; }
    }
}
