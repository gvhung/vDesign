using Base.Attributes;
using Base.BusinessProcesses.Services.Abstract;
using Framework.Maybe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class TemplateRenderer : ITemplateRenderer
    {
        private const string Pattern = @"\[(.*?)\]";

        public string Render(string template, BaseObject obj, IDictionary<string, string> additional = null)
        {
            if (String.IsNullOrEmpty(template)) return template;

            IEnumerable<PropertyInfo> props = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            return Regex.Replace(template, Pattern, m =>
            {
                string match = m.Groups[1].Value;

                return props.FirstOrDefault(x => x.Name == match || x.GetCustomAttribute<DetailViewAttribute>().With(a => a.Name) == match)
                    .With(x => x.GetValue(obj)).With(x => x.ToString()) ?? additional.With(x => x.ContainsKey(match).IfTrue(() => x[match])) ?? m.Groups[0].Value;
            });
        }
    }
}