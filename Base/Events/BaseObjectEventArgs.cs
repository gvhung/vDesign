using Base.DAL;
using System;

namespace Base.Events
{
    public class BaseObjectEventArgs : EventArgs
    {
        public TypeEvent Type { get; set; }
        public BaseObject Object { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }
        public BaseObject ObjectSrc { get; set; }
    }

    public enum TypeEvent
    {
        OnGetAll = 0,
        OnGet,
        OnCreate,
        OnUpdate,
        OnDelete,
        OnChangeSortOrder,
        OnCreateOnGroundsOf,
    }
}
