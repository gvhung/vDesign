using Base.Projects.Base;
using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Projects
{
    public abstract class BaseHProject : BaseProject, IBaseHProject
    {
        public int CategoryID { get; set; }

        public abstract BaseProjectCategory BaseProjectCategory { get; set; }

        #region ICategorizedItem
        HCategory ICategorizedItem.Category
        {
            get { return this.BaseProjectCategory; }
        }
        #endregion
    }
}
