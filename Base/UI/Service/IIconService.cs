namespace Base.UI.Service
{
    using System.Collections.Generic;

    public interface IIconService
    {
        IList<IconsSet> GetIcons();
    }

    public class IconsSet
    {
        public string Title { get; set; }
        public IList<string> Icons { get; set; }
    }
}