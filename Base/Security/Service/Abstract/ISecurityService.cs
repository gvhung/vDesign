using Base.DAL;
using Base.Security.ObjectAccess;
using Base.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Security.Service.Abstract
{
    public interface ISecurityService : IService
    {
        IQueryable<TObject> FilterByAccess<TObject>(IQueryable<TObject> query, IUnitOfWork unitOfWork) where TObject : BaseObject;
        void ThrowIfAccessDenied(IUnitOfWork unitOfWork, Type type, TypePermission typePermission);
        void ThrowIfAccessDenied(IUnitOfWork unitOfWork, Type type, int id, TypePermission typePermission, AccessType accessType);
        void ThrowIfAccessDenied(IUnitOfWork unitOfWork, Type type, int id, AccessType accessType);
        ObjectAccessItem GetObjectAccessItem(IUnitOfWork unitOfWork, Type objectType, int objectID);
        ObjectAccessItem CreateAndSaveAccessItem(IUnitOfWork unitOfWork, BaseObject obj);
        ObjectAccessItem CreateAndSaveAccessItem(IUnitOfWork unitOfWork, BaseObject obj, Func<ObjectAccessItem, ObjectAccessItem> modifier);
        ObjectAccessItem CreateAccessItem(IUnitOfWork unitOfWork, BaseObject obj, int catID);
        void OnObjectCreated(IUnitOfWork unitOfWork, BaseObject obj);
        void OnObjectDeleted(IUnitOfWork unitOfWork, BaseObject obj);
        void MakeReadOnly(Type type, int id);
        ObjectAccessItem RestoreAccess(int userID, Type type, int id);
        ObjectAccessItem RestoreAccess(Type type, int id);
        AccessType GetAccessType(IUnitOfWork unitOfWork, Type type, int id);
        AccessType GetAccessType(IUnitOfWork unitOfWork, BaseObject obj);
        IEnumerable<int> GetAllAccessedCategories(IUnitOfWork unitOfWork, User user);
        AccessType GetAccessType(IUnitOfWork unitOfWork, ISecurityUser securityUser, BaseObject obj);
        AccessType GetAccessType(IUnitOfWork unitOfWork, ISecurityUser securityUser, Type type, int id);        
    }
}