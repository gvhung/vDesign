using Base.DAL.EF.Extensions;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Base.DAL.EF
{
    public class EFRepository<TObject> : IRepository<TObject> where TObject : BaseObject
    {
        private DbContext _context = null;

        public EFRepository(DbContext context)
        {
            _context = context;
        }

        private DbSet<TObject> DbSet
        {
            get
            {
                return _context.Set<TObject>();
            }
        }

        public virtual IExtendedQueryable<TObject> All()
        {
            return DbSet.AsExtendedQueryable();
        }

        public virtual IExtendedQueryable<TObject> Filter(Expression<Func<TObject, bool>> predicate)
        {
            return DbSet.Where(predicate).AsExtendedQueryable();
        }

        public virtual IExtendedQueryable<TObject> Filter(Expression<Func<TObject, bool>> filter, out int total, int index = 0, int size = 50)
        {
            int skipCount = index * size;

            var _resetSet = filter != null ? DbSet.Where(filter).AsQueryable() : DbSet.AsQueryable();

            _resetSet = skipCount == 0 ? _resetSet.Take(size) : _resetSet.Skip(skipCount).Take(size);

            total = _resetSet.Count();

            return _resetSet.AsExtendedQueryable();
        }

        public bool Contains(Expression<Func<TObject, bool>> predicate)
        {
            return DbSet.Count(predicate) > 0;
        }

        public virtual TObject Find(params object[] keys)
        {
            return DbSet.Find(keys);
        }

        public virtual TObject Find(Expression<Func<TObject, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
        }

        public virtual TObject Create(TObject TObject)
        {
            var newEntry = DbSet.Add(TObject);
            return newEntry;
        }

        public virtual int Count
        {
            get
            {
                return DbSet.Count();
            }
        }

        public virtual int Delete(TObject TObject)
        {
            DbSet.Remove(TObject);
            return 0;
        }

        public virtual int Update(TObject TObject)
        {
            DbSet.Attach(TObject);
            _context.Entry<TObject>(TObject).State = EntityState.Modified;
            return 0;
        }

        public virtual void Attach(TObject TObject)
        {
            DbSet.Attach(TObject);
        }

        public virtual void Detach(TObject TObject)
        {
            _context.Entry<TObject>(TObject).State = EntityState.Detached;
        }

        public virtual int Delete(Expression<Func<TObject, bool>> predicate)
        {
            var objects = Filter(predicate);
            foreach (var obj in objects)
                DbSet.Remove(obj);
            return 0;
        }

        public IObjectSaver<TObject> GetObjectSaver(IUnitOfWork unitOfWork, TObject objSrc, TObject objDest)
        {
            return new ObjectSaver<TObject>(unitOfWork, objSrc, objDest);
        }



        public bool AutoDetectChangesEnabled
        {
            get { return _context.Configuration.AutoDetectChangesEnabled; }
            set { _context.Configuration.AutoDetectChangesEnabled = value; }
        }

        public bool ValidateOnSaveEnabled
        {
            get { return _context.Configuration.ValidateOnSaveEnabled; }
            set { _context.Configuration.ValidateOnSaveEnabled = value; }
        }
    }
}
