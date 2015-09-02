using System.Linq;

namespace Base.DAL
{
    public interface IExtendedQueryable<out T> : IOrderedQueryable<T>
    {   
    }
}
