using Base.Security;
using Base.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using System.Xml.Linq;

namespace WebUI.Concrete
{
    public class MenuLoader : IMenuLoader
    {
        public Menu Load()
        {
            Menu menu = new Menu();

            try
            {
                string fileName = Path.Combine(HostingEnvironment.MapPath("/App_Data"), "Menu.xml");

                XDocument document = XDocument.Load(fileName);

                if (document != null)
                {
                    XElement root = document.Element("menu");

                    if (root != null)
                    {
                        menu.DefaultIcon = this.GetAttrValue(root, "defaulticon");

                        foreach (XElement item in root.Elements())
                        {
                            this.ItemParse(item, menu.Items);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return menu;
        }

        private void ItemParse(XElement el, List<MenuItem> menuitems, MenuItem parent = null)
        {
            MenuItem mi = new MenuItem()
            {
                Parent = parent,
                Title = this.GetAttrValue(el, "title"),
                Mnemonic = this.GetAttrValue(el, "mnemonic"),
                Icon = this.GetAttrValue(el, "icon"),
                URL = this.GetAttrValue(el, "url")
            };

            var forSysRoleValue = this.GetAttrValue(el, "for-system-role");
            if (!string.IsNullOrWhiteSpace(forSysRoleValue))
            {
                SystemRole role;
                if (Enum.TryParse(forSysRoleValue, out role))
                {
                    mi.ForSystemRole = role;
                }
            }


            menuitems.Add(mi);

            foreach (XElement item in el.Elements())
            {
                this.ItemParse(item, mi.Items, mi);
            }
        }

        private string GetAttrValue(XElement el, string nameAttr, string def = null)
        {
            XAttribute attr = el.Attribute(nameAttr);

            if (attr != null)
            {
                return attr.Value;
            }

            return def;
        }
    }
}