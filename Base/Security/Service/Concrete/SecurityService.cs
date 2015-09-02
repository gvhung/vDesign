using Base.Ambient;
using Base.DAL;
using Base.Security.ObjectAccess;
using Base.Security.ObjectAccess.Policies;
using Base.Security.ObjectAccess.Services;
using Base.Security.Service.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Base.Security.Service.Concrete
{
    public class SecurityService : ISecurityService
    {
        private readonly IObjectAccessItemService _accessItemService;
        private readonly IAccessPolicyFactory _policyFactory;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public SecurityService(IObjectAccessItemService accessItemService, IAccessPolicyFactory policyFactory, IUnitOfWorkFactory unitOfWorkFactory)
        {
            _accessItemService = accessItemService;
            _policyFactory = policyFactory;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        private static bool IsSystemUnitOfWork(IUnitOfWork unitOfWork)
        {
            return unitOfWork is ISystemUnitOfWork;
        }

        public void ThrowIfAccessDenied(IUnitOfWork unitOfWork, Type type, TypePermission typePermission)
        {
            if (IsSystemUnitOfWork(unitOfWork)) return;
            
            if (!AppContext.SecurityUser.IsPermission(type, typePermission))
                throw new AccessDeniedException("Отказано в доступе на тип");
        }

        public void ThrowIfAccessDenied(IUnitOfWork unitOfWork, Type type, int id, AccessType accessType)
        {
            if (IsSystemUnitOfWork(unitOfWork)) return;

            if (!AppContext.SecurityUser.IsAdmin && typeof(IAccessibleObject).IsAssignableFrom(type))
                if (this.GetAcessibleObjectIDs(unitOfWork, type.FullName, accessType).All(x => x != id))
                    throw new AccessDeniedException("Отказано в доступе на объект");
        }

        public void ThrowIfAccessDenied(IUnitOfWork unitOfWork, Type type, int id, TypePermission typePermission, AccessType accessType)
        {
            if (IsSystemUnitOfWork(unitOfWork)) return;

            this.ThrowIfAccessDenied(unitOfWork, type, typePermission);

            if (!AppContext.SecurityUser.IsAdmin && typeof(IAccessibleObject).IsAssignableFrom(type))
                if (this.GetAcessibleObjectIDs(unitOfWork, type.FullName, accessType).All(x => x != id))
                    throw new AccessDeniedException("Отказано в доступе на объект");
        }

        public IQueryable<TObject> FilterByAccess<TObject>(IQueryable<TObject> query, IUnitOfWork unitOfWork) where TObject : BaseObject
        {
            if (IsSystemUnitOfWork(unitOfWork)) return query;

            var objectType = typeof(TObject);

            if (!AppContext.SecurityUser.IsPermission(objectType, TypePermission.Read))
                return query.Where(x => false);

            if (!AppContext.SecurityUser.IsAdmin && typeof(IAccessibleObject).IsAssignableFrom(objectType))
            {
                var accessibleObjectIDs = this.GetAcessibleObjectIDs(unitOfWork, objectType.FullName, AccessType.Read);

                query = query.Where(x => accessibleObjectIDs.Contains(x.ID));
            }

            return query;
        }

        public AccessType GetAccessType(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return GetAccessType(unitOfWork, obj.GetType().GetBaseObjectType(), obj.ID);
        }

        public AccessType GetAccessType(IUnitOfWork unitOfWork, ISecurityUser securityUser, BaseObject obj)
        {
            return GetAccessType(unitOfWork, obj.GetType().GetBaseObjectType(), obj.ID);
        }

        public AccessType GetAccessType(IUnitOfWork unitOfWork, ISecurityUser securityUser, Type type, int id)
        {
            if (securityUser.IsAdmin || IsSystemUnitOfWork(unitOfWork)) return AccessType.Full;

            var accessType = AccessType.None;

            string typyStr = type.GetBaseObjectType().FullName;

            var access = _accessItemService.GetAll(unitOfWork).FirstOrDefault(m => m.Object.ID == id && m.Object.FullName == typyStr);

            if (access != null)
            {
                if (access.ReadAll) accessType |= AccessType.Read;
                if (access.UpdateAll) accessType |= AccessType.Update;
                if (access.UpdateAll) accessType |= AccessType.Delete;

                if (accessType == (AccessType.Full))
                    return accessType;

                var user = unitOfWork.GetRepository<User>().All().FirstOrDefault(x => !x.Hidden && x.ID == securityUser.ID);

                if (user != null)
                {
                    var allCategories = this.GetAllAccessedCategories(unitOfWork, user);

                    var allCategoryItems = access.UserCategories.Where(x => allCategories.Contains(x.UserCategoryID)).ToList();

                    foreach (var item in allCategoryItems)
                    {
                        if (item.Read) accessType |= AccessType.Read;
                        if (item.Update) accessType |= AccessType.Update;
                        if (item.Delete) accessType |= AccessType.Delete;

                        if (accessType == (AccessType.Full))
                            return accessType;
                    }

                    foreach (var item in access.Users.Where(x => x.UserID == user.ID))
                    {
                        if (item.Read) accessType |= AccessType.Read;
                        if (item.Update) accessType |= AccessType.Update;
                        if (item.Delete) accessType |= AccessType.Delete;

                        if (accessType == (AccessType.Full))
                            return accessType;
                    }
                }
            }

            return accessType;
        }

        public AccessType GetAccessType(IUnitOfWork unitOfWork, Type type, int id)
        {
            return GetAccessType(unitOfWork, AppContext.SecurityUser, type, id);
        }

        public IEnumerable<int> GetAllAccessedCategories(IUnitOfWork unitOfWork, User user)
        {
            var categoiesIDs = new List<int> { user.CategoryID };

            if (user.UserCategory.sys_all_parents != null)
                categoiesIDs.AddRange(user.UserCategory.sys_all_parents.Split(HCategory.Seperator)
                    .Select(HCategory.IdToInt).Reverse());

            var companies =
                unitOfWork.GetRepository<UserCategory>()
                    .All()
                    .Where(x => x.Company)
                    .Select(x => x.ID);

            return this.GetAccessedCategories(categoiesIDs, companies).ToList();
        }

        // | n1 -> n2 -> c3 -> n4 | => | n1 -> n2 -> c3 |; 
        // Возвращаем последовательность, перебирая последовательно parents до первой компании включительно;
        // n1, n2, n4 - не компании
        // c3 - компания
        //
        private IEnumerable<int> GetAccessedCategories(IEnumerable<int> parents, IEnumerable<int> companies)
        {
            bool companyFound = false;

            foreach (var parent in parents)
            {
                if (companyFound) yield break;

                if (companies.Contains(parent))
                    companyFound = true;

                yield return parent;
            }
        }

        private IEnumerable<int> GetAcessibleObjectIDs(IUnitOfWork unitOfWork, string objectType, AccessType accessType)
        {
            var user = unitOfWork.GetRepository<User>().All().FirstOrDefault(x => x.ID == AppContext.SecurityUser.ID);

            if (user != null)
            {
                var allAccessedCategories = this.GetAllAccessedCategories(unitOfWork, user);

                switch (accessType)
                {
                    case AccessType.Read:
                        {
                            var userAccessesIDs = unitOfWork.GetRepository<UserAccess>().All()
                                .Where(x => x.UserID == AppContext.SecurityUser.ID && (x.Read || x.Update || x.Delete))
                                .Select(x => x.ID);

                            var categoryAccessesIDs = unitOfWork.GetRepository<UserCategoryAccess>().All()
                                .Where(x => !x.Hidden && allAccessedCategories.Contains(x.UserCategoryID) && (x.Read || x.Update || x.Delete)).Select(x => x.ID);

                            return unitOfWork.GetRepository<ObjectAccessItem>().All()
                                .Where(x => x.Object.FullName == objectType && (x.ReadAll
                                                                             || x.UpdateAll
                                                                             || x.DeleteAll
                                                                             || x.UserCategories.Any(c => categoryAccessesIDs.Contains(c.ID))
                                                                             || x.Users.Any(c => userAccessesIDs.Contains(c.ID))))
                                .Select(x => x.Object.ID).Distinct().ToList();
                        }
                    case AccessType.Update:
                        {
                            var userAccessesIDs = unitOfWork.GetRepository<UserAccess>().All()
                                .Where(x => x.UserID == AppContext.SecurityUser.ID && x.Update).Select(x => x.ID);

                            var categoryAccessesIDs = unitOfWork.GetRepository<UserCategoryAccess>().All()
                                .Where(x => !x.Hidden && allAccessedCategories.Contains(x.UserCategoryID) && x.Update)
                                .Select(x => x.ID);

                            return unitOfWork.GetRepository<ObjectAccessItem>().All()
                                .Where(x => x.Object.FullName == objectType && (x.UpdateAll
                                                                             || x.UserCategories.Any(c => categoryAccessesIDs.Contains(c.ID))
                                                                             || x.Users.Any(c => userAccessesIDs.Contains(c.ID))))
                                .Select(x => x.Object.ID).Distinct().ToList();
                        }
                    case AccessType.Delete:
                        {
                            var userAccessesIDs = unitOfWork.GetRepository<UserAccess>().All()
                                .Where(x => x.UserID == AppContext.SecurityUser.ID && x.Delete).Select(x => x.ID);

                            var categoryAccessesIDs = unitOfWork.GetRepository<UserCategoryAccess>().All()
                                .Where(x => !x.Hidden && allAccessedCategories.Contains(x.UserCategoryID) && x.Delete)
                                .Select(x => x.ID);

                            return unitOfWork.GetRepository<ObjectAccessItem>().All()
                                .Where(x => x.Object.FullName == objectType && (x.DeleteAll
                                                                             || x.UserCategories.Any(c => categoryAccessesIDs.Contains(c.ID))
                                                                             || x.Users.Any(c => userAccessesIDs.Contains(c.ID))))
                                .Select(x => x.Object.ID).Distinct().ToList();
                        }
                }
            }

            return Enumerable.Empty<int>();
        }

        public ObjectAccessItem GetObjectAccessItem(IUnitOfWork unitOfWork, Type objectType, int objectID)
        {
            var typyStr = objectType.GetBaseObjectType().FullName;

            var access = _accessItemService.GetAll(unitOfWork).FirstOrDefault(m => m.Object.ID == objectID && m.Object.FullName == typyStr);

            if (access == null)
                throw new AccessDeniedException("Конфигурая доступа не была найдена");

            if (AppContext.SecurityUser.IsAdmin || access.ChangeAccessAll)
                return access;

            var user = unitOfWork.GetRepository<User>().All().FirstOrDefault(x => x.ID == AppContext.SecurityUser.ID);

            if (user == null) throw new AccessDeniedException("Доступ запрещен");

            var accessedCategories = this.GetAllAccessedCategories(unitOfWork, user);

            if (access.Users.Any(x => x.UserID == user.ID && x.ChangeAccess)
                || access.UserCategories.Where(x => accessedCategories.Contains(x.UserCategoryID)).Any(x => x.ChangeAccess))
            {
                return access;
            }

            throw new AccessDeniedException("Доступ запрещен");
        }

        public ObjectAccessItem CreateAccessItem(IUnitOfWork unitOfWork, Type type, int id)
        {
            return this.GetAccessItemImpl(AppContext.SecurityUser.ID, type, new ObjectAccessItem(AppContext.SecurityUser, type, id));
        }

        public ObjectAccessItem CreateAccessItem(IUnitOfWork unitOfWork, BaseObject obj)
        {
            var type = obj.GetType().GetBaseObjectType();

            return this.GetAccessItemImpl(AppContext.SecurityUser.ID, type, new ObjectAccessItem(AppContext.SecurityUser, obj));
        }
        public ObjectAccessItem CreateAccessItem(int userID, Type type, int id)
        {
            return this.GetAccessItemImpl(userID, type, new ObjectAccessItem(userID, type, id));
        }

        private ObjectAccessItem GetAccessItemImpl(int userID, Type type, ObjectAccessItem accessItem)
        {
            var policeses = type.GetCustomAttributes<AccessPolicyAttribute>().Select(x => x.AccessPolicy).ToList();

            if (!policeses.Any())
                policeses.Add(typeof(DefaultAccessPolicy));

            return policeses
                .Select(x => _policyFactory.GetAccessPolicy(x))
                .Aggregate(accessItem, (current, policy) => policy.InitializeAccessItem(current, userID, type));
        }

        public virtual ObjectAccessItem CreateAndSaveAccessItem(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return _accessItemService.Create(unitOfWork, CreateAccessItem(unitOfWork, obj));
        }

        public ObjectAccessItem CreateAndSaveAccessItem(IUnitOfWork unitOfWork, BaseObject obj, Func<ObjectAccessItem, ObjectAccessItem> modifier)
        {
            return _accessItemService.Create(unitOfWork, modifier(CreateAccessItem(unitOfWork, obj)));
        }

        public ObjectAccessItem CreateAccessItem(IUnitOfWork unitOfWork, BaseObject obj, int catID)
        {
            var accessItem = new ObjectAccessItem(AppContext.SecurityUser, obj);

            accessItem.ReadAll = accessItem.UpdateAll = accessItem.DeleteAll = false;

            accessItem.UserCategories = new Collection<UserCategoryAccess>()
            {
                new UserCategoryAccess()
                {
                    ObjectAccessItem = accessItem,
                    UserCategoryID = catID
                }
            };

            _accessItemService.Create(unitOfWork, accessItem);

            return accessItem;
        }

        public void MakeReadOnly(Type type, int id)
        {
            using (var uofw = _unitOfWorkFactory.CreateSystem())
            {
                var access = GetObjectAccessItem(uofw, type, id);

                if (access != null)
                {
                    access.UpdateAll = false;
                    access.DeleteAll = false;
                    access.ChangeAccessAll = false;

                    foreach (var user in access.Users)
                    {
                        user.Update = false;
                        user.Delete = false;
                        user.ChangeAccess = false;
                    }

                    foreach (var cat in access.UserCategories)
                    {
                        cat.Update = false;
                        cat.Delete = false;
                        cat.ChangeAccess = false;
                    }

                    _accessItemService.Update(uofw, access);

                    uofw.SaveChanges();
                }
                else
                {
                    throw new AccessDeniedException("Конфигурая доступа не была найдена");
                }
            }
        }

        public ObjectAccessItem RestoreAccess(int userID, Type type, int id)
        {
            return RestoreAccessImpl(type, id, userID);
        }

        public ObjectAccessItem RestoreAccess(Type type, int id)
        {
            return RestoreAccessImpl(type, id);
        }

        private ObjectAccessItem RestoreAccessImpl(Type type, int id, int? userID = null)
        {
            using (var uofw = _unitOfWorkFactory.CreateSystem())
            {
                var access = GetObjectAccessItem(uofw, type, id);

                if (access != null)
                {
                    var initialAccess = this.CreateAccessItem(userID ?? access.CreatorID, type, id);

                    access.ReadAll = initialAccess.ReadAll;
                    access.UpdateAll = initialAccess.UpdateAll;
                    access.DeleteAll = initialAccess.DeleteAll;
                    access.ChangeAccessAll = initialAccess.ChangeAccessAll;

                    var userRepo = uofw.GetRepository<UserAccess>();

                    foreach (var user in access.Users.ToList())
                        userRepo.Delete(user);

                    foreach (var user in initialAccess.Users)
                    {
                        user.ObjectAccessItem = access;
                        userRepo.Create(user);
                    }

                    var catRepo = uofw.GetRepository<UserCategoryAccess>();

                    foreach (var cat in access.UserCategories.ToList())
                        catRepo.Delete(cat);

                    foreach (var cat in initialAccess.UserCategories)
                    {
                        cat.ObjectAccessItem = access;
                        catRepo.Create(cat);
                    }

                    //foreach (
                    //    var pair in
                    //        access.Users.Join(initialAccess.Users, userAccess => userAccess.UserID, userAccess => userAccess.UserID,
                    //            (acc, initAcc) => new {Access = acc, InitAccess = initAcc}))
                    //{
                    //    pair.Access.Read = pair.InitAccess.Read;
                    //    pair.Access.Update = pair.InitAccess.Update;
                    //    pair.Access.Delete = pair.InitAccess.Delete;
                    //    pair.Access.ChangeAccess = pair.InitAccess.ChangeAccess;
                    //}

                    //foreach (
                    //    var pair in
                    //        access.UserCategories.Join(initialAccess.UserCategories, categoryAccess => categoryAccess.UserCategoryID,
                    //            categoryAccess => categoryAccess.UserCategoryID,
                    //            (acc, initAcc) => new {Access = acc, InitAccess = initAcc}))
                    //{
                    //    pair.Access.Read = pair.InitAccess.Read;
                    //    pair.Access.Update = pair.InitAccess.Update;
                    //    pair.Access.Delete = pair.InitAccess.Delete;
                    //    pair.Access.ChangeAccess = pair.InitAccess.ChangeAccess;
                    //}

                    var accessItem = _accessItemService.Update(uofw, access);

                    return accessItem;
                }

                throw new AccessDeniedException("Конфигурция доступа не была найдена");
            }
        }

        public void OnObjectCreated(IUnitOfWork unitOfWork, BaseObject obj)
        {
            string type = obj.GetType().GetBaseObjectType().FullName;

            var access =
                _accessItemService.GetAll(unitOfWork)
                    .FirstOrDefault(x => x.Object.FullName == type && x.Object.ID == obj.ID);

            if (access == null)
                this.CreateAndSaveAccessItem(unitOfWork, obj);
        }

        public void OnObjectDeleted(IUnitOfWork unitOfWork, BaseObject obj)
        {
            string type = obj.GetType().GetBaseObjectType().FullName;

            var access =
                _accessItemService.GetAll(unitOfWork)
                    .FirstOrDefault(x => x.Object.FullName == type && x.Object.ID == obj.ID);

            if (access != null)
            {
                access.Hidden = true;
            }
        }
    }
}