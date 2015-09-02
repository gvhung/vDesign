using System;

namespace Base.Ambient
{
    public class DefaultDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now
        {
            get { return DateTime.Now; }
        }
    }
}
