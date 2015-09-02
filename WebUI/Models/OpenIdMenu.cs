using Base.OpenID.Entities;
using Framework;
using System.Collections.Generic;
using System.Linq;

namespace WebUI.Models
{
    public class OpenIdMenu
    {
        public OpenIdMenu(IEnumerable<OpenIdConfig> config, OpenIdAuthMode mode, string antiForgeryToken, bool isVertical = false)
        {
            Items = config.Select(x => new OpenIdMenuItem
            {
                TypeName = x.Type.GetDescription(),
                AuthUrl = x.GetAuthQueryUrl(mode, antiForgeryToken),
                IconCssClass = x.IconCssClass
            });
            Mode = mode;
            IsVertical = isVertical;
        }

        public IEnumerable<OpenIdMenuItem> Items { get; set; }
        public OpenIdAuthMode Mode { get; set; }
        public bool IsVertical { get; set; }
    }

    public class OpenIdMenuItem
    {
        public string TypeName { get; set; }
        public string AuthUrl { get; set; }
        public string IconCssClass { get; set; }
    }
}