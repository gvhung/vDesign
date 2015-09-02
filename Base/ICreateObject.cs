using Base.Security;

namespace Base
{
    public interface ICreateObject
    {
        User Creator { get; set; }
    }
}
