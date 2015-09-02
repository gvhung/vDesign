using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Base.DAL
{
    public abstract class WrapperUnitOfWork: IUnitOfWork
    {
        private readonly IUnitOfWork _unitOfWork;

        protected WrapperUnitOfWork(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Guid ID
        {
            get { return _unitOfWork.ID; }
        }

        public int SaveChanges()
        {
            return _unitOfWork.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _unitOfWork.SaveChangesAsync();
        }

        public IObjectSaver<TObject> GetObjectSaver<TObject>(TObject objSrc, TObject objDest) where TObject : BaseObject
        {
            return _unitOfWork.GetObjectSaver(objSrc, objDest);
        }

        public IRepository<TObject> GetRepository<TObject>() where TObject : BaseObject
        {
            return _unitOfWork.GetRepository<TObject>();
        }

        public Dictionary<TEntity, BaseEntityState> GetModifiedEntities<TEntity>(bool recursive = true) where TEntity : BaseObject
        {
            return _unitOfWork.GetModifiedEntities<TEntity>(recursive);
        }
        
        public override int GetHashCode()
        {
            return _unitOfWork.GetHashCode();
        }

        public void Dispose()
        {
            
        }

        public bool IsModifiedEntity<TEntity>(TEntity entity, BaseEntityState modif) where TEntity : BaseObject
        {
            return _unitOfWork.IsModifiedEntity(entity, modif);
        }
    }
}
