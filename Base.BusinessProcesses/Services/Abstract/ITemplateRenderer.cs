using System.Collections.Generic;

namespace Base.BusinessProcesses.Services.Abstract
{
    public interface ITemplateRenderer
    {
        string Render(string template, BaseObject obj, IDictionary<string, string> additional = null);
    }
}