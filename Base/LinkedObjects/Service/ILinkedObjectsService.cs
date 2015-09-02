using Base.DAL;
using Base.LinkedObjects.Entities;
using Base.Service;

namespace Base.LinkedObjects.Service
{
    public interface ILinkedObjectsService : IBaseObjectService<ListLinkedОbjects>
    {
        ListLinkedОbjects GetListLinkedObjects(IUnitOfWork unitOfWork, BaseObject obj);
        void AddLink(IUnitOfWork unitOfWork, BaseObject obj1, BaseObject obj2);
        void DeleteLink(IUnitOfWork unitOfWork, BaseObject obj1, BaseObject obj2);
        void DeleteAllLinks(IUnitOfWork unitOfWork, BaseObject obj);
        void OnObjectDeleted(IUnitOfWork unitOfWork, BaseObject obj);
    }
}
