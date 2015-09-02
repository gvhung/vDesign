namespace WebUI.Concrete
{
    using Base.Service;
    using Base.UI.Service;
    using ExCSS;
    using Framework.Maybe;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public class IconService : IIconService
    {
        private readonly IPathHelper _pathHelper;

        public IconService(IPathHelper pathHelper)
        {
            this._pathHelper = pathHelper;
        }

        public IList<IconsSet> GetIcons()
        {
            string path = Path.Combine(_pathHelper.GetContentDirectory(), "css", "fonts");

            var parser = new Parser();

            var list = new List<IconsSet>();

            foreach (var fileContent in Directory.GetFiles(path, "*.css")
                .Select(x => File.ReadAllText(x, Encoding.Default)))
            {
                var stylesheet = parser.Parse(fileContent);

                IconsSet set = new IconsSet
                                   {
                                       Icons =
                                           stylesheet.StyleRules.Select(x => x.Selector.ToString())
                                           .Where(x => x.Contains(":before"))
                                           .Select(x => x.Replace(":before", string.Empty))
                                           .Select(
                                               x =>
                                               string.Format(
                                                   "{0} {1}",
                                                   x.Split(new[] { '.', '-' })[1],
                                                   x.Replace(".", string.Empty)))
                                           .ToList(),
                                       Title =
                                           fileContent.Split(new[] { '\r', '\n' })
                                           .FirstOrDefault()
                                           .With(x => Regex.Match(x, @"/\*(.*?)\*/").Groups[1].Value)
                                   };

                list.Add(set);
            }

            return list;
        }
    }
}