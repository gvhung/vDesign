using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.DAL.SoapClient
{
    public class WebEntityEntry
    {
        readonly object _entity;

        public WebEntityEntry(object entity)
        {
            _entity = entity;
            State = BaseEntityState.Unchanged;
        }

        public object Entity { get { return _entity; } }

        public BaseEntityState State { get; set; }
    }
}
