using System;

namespace Base.Ambient
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}
