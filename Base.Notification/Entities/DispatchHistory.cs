using Base.Entities.Complex;
using System;

namespace Base.Notification.Entities
{
    public class DispatchHistory : BaseObject
    {
        public LinkBaseObject Entity { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? SendDate { get; set; }

        public bool IsSended { get; set; }
    }
}
