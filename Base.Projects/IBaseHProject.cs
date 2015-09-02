using Base.Projects.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Projects
{
    public interface IBaseHProject : IBaseProject, ICategorizedItem
    {
        //int? DefaultCategoryID { get; set; }
    }
}
