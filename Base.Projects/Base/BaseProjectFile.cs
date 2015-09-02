using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Projects.Base
{
    public abstract class BaseProjectFile : FileData
    {
        
        public int ProjectID { get; set; }

        public abstract IBaseProject BaseProject { get; set; }
    }
}
