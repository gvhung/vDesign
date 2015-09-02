using Base.DAL;
using Base.Entities.Complex;
using Base.LinkedObjects.Entities;
using Base.Security;
using Base.Service;
using System;
using System.Linq;

namespace Base.LinkedObjects.Service
{
    public class LinkedObjectsService : BaseObjectService<ListLinkedОbjects>, ILinkedObjectsService
    {
        public LinkedObjectsService(IBaseObjectServiceFacade facade) : base(facade) { }

        public override IQueryable<ListLinkedОbjects> GetAll(IUnitOfWork unitOfWork, bool? hidden = false)
        {
            throw new NotImplementedException();
        }

        public override ListLinkedОbjects Get(IUnitOfWork unitOfWork, int id)
        {
            throw new NotImplementedException();
        }

        public override ListLinkedОbjects Create(IUnitOfWork unitOfWork, ListLinkedОbjects obj)
        {
            throw new NotImplementedException();
        }

        public override ListLinkedОbjects Update(IUnitOfWork unitOfWork, ListLinkedОbjects obj)
        {
            this.SecurityService.ThrowIfAccessDenied(unitOfWork, typeof(ListLinkedОbjects), TypePermission.Write);

            return null;
        }

        public ListLinkedОbjects GetListLinkedObjects(IUnitOfWork unitOfWork, BaseObject obj)
        {
            this.SecurityService.ThrowIfAccessDenied(unitOfWork, typeof(ListLinkedОbjects), TypePermission.Read);

            var lbo = new LinkBaseObject(obj);

            string type = lbo.FullName;

            var repository = unitOfWork.GetRepository<LinkBaseObjects>();

            return new ListLinkedОbjects()
            {
                ID = -1,
                Obj = lbo,
                Links = repository.All()
                    .Where(x => !x.Hidden && x.Obj1.ID == obj.ID && x.Obj1.FullName == type).Select(x => x.Obj2).ToList()
                    .Union(repository.All().Where(x => !x.Hidden && x.Obj2.ID == obj.ID && x.Obj2.FullName == type).Select(x => x.Obj1).ToList()).Distinct().Select(x => new Link()
                    {
                        ID = -1,
                        Obj = x
                    }).ToList()
            };
        }

        public void AddLink(IUnitOfWork unitOfWork, BaseObject obj1, BaseObject obj2)
        {
            this.SecurityService.ThrowIfAccessDenied(unitOfWork, typeof(ListLinkedОbjects), TypePermission.Create);

            var lbo1 = new LinkBaseObject(obj1);
            var lbo2 = new LinkBaseObject(obj2);

            string type1 = lbo1.FullName;
            string type2 = lbo2.FullName;

            var repository = unitOfWork.GetRepository<LinkBaseObjects>();

            if (!repository.All().Where(x => !x.Hidden).Any(x => 
                (x.Obj1.ID == obj1.ID && x.Obj1.FullName == type1 && x.Obj2.ID == obj2.ID && x.Obj2.FullName == type2) ||
                (x.Obj1.ID == obj2.ID && x.Obj1.FullName == type2 && x.Obj2.ID == obj1.ID && x.Obj2.FullName == type1)))
            {
                repository.Create(new LinkBaseObjects()
                {
                    Obj1 = lbo1,
                    Obj2 = lbo2
                });
            }
        }

        public void DeleteLink(IUnitOfWork unitOfWork, BaseObject obj1, BaseObject obj2)
        {
            this.SecurityService.ThrowIfAccessDenied(unitOfWork, typeof(ListLinkedОbjects), TypePermission.Delete);

            var lbo1 = new LinkBaseObject(obj1);
            var lbo2 = new LinkBaseObject(obj2);

            string type1 = lbo1.FullName;
            string type2 = lbo2.FullName;

            var repository = unitOfWork.GetRepository<LinkBaseObjects>();

            var link = repository.Find(x => 
                (x.Obj1.ID == obj1.ID && x.Obj1.FullName == type1 && x.Obj2.ID == obj2.ID && x.Obj2.FullName == type2) || 
                (x.Obj1.ID == obj2.ID && x.Obj1.FullName == type2 && x.Obj2.ID == obj1.ID && x.Obj2.FullName == type1));

            if (link != null)
            {
                repository.Delete(link);
            }
        }

        public void DeleteAllLinks(IUnitOfWork unitOfWork, BaseObject obj)
        {
            this.SecurityService.ThrowIfAccessDenied(unitOfWork, typeof(ListLinkedОbjects), TypePermission.Delete);

            var lbo = new LinkBaseObject(obj);
            
            string type = lbo.FullName;
            
            var repository = unitOfWork.GetRepository<LinkBaseObjects>();

            foreach (var link in repository.All().Where(x => (x.Obj1.ID == obj.ID && x.Obj1.FullName == type) || (x.Obj2.ID == obj.ID && x.Obj2.FullName == type)))
            {
                repository.Delete(link);
            }
        }


        public void OnObjectDeleted(IUnitOfWork unitOfWork, BaseObject obj)
        {
            this.DeleteAllLinks(unitOfWork, obj);
        }
    }
}
