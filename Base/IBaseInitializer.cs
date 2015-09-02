using Base.DAL;

namespace Base
{
    public interface IBaseInitializer
    {
        void Init(IUnitOfWork unitOfWork);
    }
}
