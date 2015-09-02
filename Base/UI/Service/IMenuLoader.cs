using Base.Service;

namespace Base.UI
{
    public interface IMenuLoader : IService
    {
        Menu Load();
    }
}
