
namespace Base
{
    public interface ICategorizedItem : IBaseObject
    {
        int CategoryID { get; set; }
        HCategory Category { get; }
    }
}
