using Framework.Maybe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Base.DAL.EF
{
    public class ObjectSaver<TEntity> : IObjectSaver<TEntity> where TEntity : BaseObject
    {
        private readonly bool _isNew;
        private readonly TEntity _objDest;
        private readonly TEntity _objSrc;
        private readonly IUnitOfWork _unitOfWork;

        public ObjectSaver(IUnitOfWork unitOfWork, TEntity objSrc, TEntity objDest = null)
        {
            #region Contract.Requires

            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");

            if (objSrc == null) throw new ArgumentNullException("objSrc");

            #endregion

            _unitOfWork = unitOfWork;

            _objSrc = objSrc;

            _isNew = objSrc.ID == 0;

            if (_isNew)
            {
                _objDest = objSrc;
            }
            else
            {
                _objDest = objDest ?? _unitOfWork.GetRepository<TEntity>().Find(x => x.ID == _objSrc.ID);

                _objSrc.ToObject(_objDest);
            }
        }

        public bool IsNew { get { return _isNew; } }
        public TEntity Dest { get { return _objDest; } }
        public TEntity Src { get { return _objSrc; } }

        /// <summary>
        /// Returns new ObjectSaver with current context for derrived Type
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        public IObjectSaver<TObject> AsObjectSaver<TObject>() where TObject : BaseObject
        {
            return new ObjectSaver<TObject>(_unitOfWork, _objSrc as TObject);
        }

        #region Many to many

        /// <summary>
        /// Prepare object to save with Entity Framework for Many to Many relationship
        /// </summary>
        /// <typeparam name="TEntry">Object type that inherits from BaseObject, type of collection entry</typeparam>
        /// <param name="property">Many to Many navigation property</param>
        /// <returns>ObjectSaver for chain support</returns>
        public IObjectSaver<TEntity> SaveManyToMany<TEntry>(Func<TEntity, ICollection<TEntry>> property) where TEntry : BaseObject
        {
            ICollection<TEntry> objSrcPropValue = property(_objSrc);

            ICollection<TEntry> objDestPropValue = property(_objDest);

            if (objSrcPropValue != null)
            {
                int[] iDs = objSrcPropValue.Where(x => x.ID != 0).Select(x => x.ID).Distinct().ToArray();

                objDestPropValue.Clear();

                foreach (TEntry t in _unitOfWork.GetRepository<TEntry>().Filter(x => iDs.Contains(x.ID)).AsEnumerable())
                    objDestPropValue.Add(t);

            }
            else if (objDestPropValue != null)
            {
                objDestPropValue.Clear();
            }

            return this;
        }

        #endregion

        #region One object

        /// <summary>
        /// Prepare object to save with Entity Framework for One object relationship
        /// </summary>
        /// <typeparam name="TProperty">Object type that inherits from BaseObject, type of entry</typeparam>
        /// <param name="property">One object navigation property</param>
        /// <returns>ObjectSaver for chain support</returns>
        public IObjectSaver<TEntity> SaveOneObject<TProperty>(Expression<Func<TEntity, TProperty>> property)
            where TProperty : BaseObject
        {
            #region Contract.Requires

            //TODO: implement with contracts!

            if (property == null)
                throw new ArgumentNullException("property");

            var propertyFunc = property.Compile();
            var propertyExpression = property.Body as MemberExpression;

            if (propertyExpression == null)
                throw new Exception("propertyExpression");

            var propertyInfo = propertyExpression.Member as PropertyInfo;

            if (propertyInfo == null)
                throw new Exception("propertyInfo");

            #endregion

            TProperty objSrcPropValue = propertyFunc(_objSrc);

            propertyInfo.GetValue(_objDest); // awesome entity framework lazy loading impl :)

            //propertyInfo.SetValue(_objDest, objSrcPropValue != null ? _unitOfWork.GetRepository<TProperty>().Find(x => x.ID == objSrcPropValue.ID) : null);

            var newValue = objSrcPropValue;

            if (objSrcPropValue != null && objSrcPropValue.ID != 0)
            {
                var repository = _unitOfWork.GetRepository<TProperty>();

                newValue = repository.Find(x => x.ID == objSrcPropValue.ID);

                if (_unitOfWork.IsModifiedEntity(objSrcPropValue, BaseEntityState.Added))
                {
                    foreach (var modifiedEntity in _unitOfWork.GetModifiedEntities<TProperty>().Where(x => x.Value == BaseEntityState.Added || x.Key.ID == objSrcPropValue.ID))
                    {
                        repository.Detach(modifiedEntity.Key);    
                    }
                }
            }

            propertyInfo.SetValue(_objDest, newValue);

            return this;
        }

        #endregion

        //public ObjectSaver(IUnitOfWork unitOfWork, TEntity objSrc)
        //{
        //    #region Contract.Requires

        //    if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");

        //    if (unitOfWork == null) throw new ArgumentNullException("objSrc");

        //    #endregion

        //    _unitOfWork = unitOfWork;

        //    _objSrc = objSrc;

        //    _isNew = objSrc.ID == 0;

        //    if (_isNew)
        //    {
        //        _objDest = objSrc;
        //    }
        //    else
        //    {
        //        _objDest = _unitOfWork.GetRepository<TEntity>().Find(x => x.ID == _objSrc.ID);

        //        _objSrc.ToObject(_objDest);
        //    }
        //}

        public IObjectSaver<TOtherEntity> Create<TOtherEntity>(TOtherEntity objSrc, TOtherEntity objDest = null) where TOtherEntity : BaseObject
        {
            return new ObjectSaver<TOtherEntity>(_unitOfWork, objSrc, objDest);
        }

        #region One to many

        /// <summary>
        /// Prepare object to save with Entity Framework for One to Many relationship
        /// </summary>
        /// <typeparam name="TEntry">Object type that inherits from BaseObject, type of collection entry</typeparam>
        /// <param name="property">Many to Many navigation property</param>
        /// <returns>ObjectSaver for chain support</returns>
        public IObjectSaver<TEntity> SaveOneToMany<TEntry>(Func<TEntity, ICollection<TEntry>> property) where TEntry : BaseObject
        {
            return this.SaveOneToManyMethodImplementation(property, false, null);
        }

        /// <summary>
        /// Prepare object to save with Entity Framework for One to Many relationship
        /// </summary>
        /// <typeparam name="TEntry">Object type that inherits from BaseObject, type of collection entry</typeparam>
        /// <param name="property">Many to Many navigation property</param>
        /// <param name="entrySaverDelegate">Entry saver delegate</param>
        /// <returns>ObjectSaver for chain support</returns>
        public IObjectSaver<TEntity> SaveOneToMany<TEntry>(Func<TEntity, ICollection<TEntry>> property,
            Action<IObjectSaver<TEntry>> entrySaverDelegate) where TEntry : BaseObject
        {
            return this.SaveOneToManyMethodImplementation(property, false, entrySaverDelegate);
        }

        /// <summary>
        /// Prepare object to save with Entity Framework for One to Many relationship
        /// </summary>
        /// <typeparam name="TEntry">Object type that inherits from BaseObject, type of collection entry</typeparam>
        /// <param name="property">Many to Many navigation property</param>
        /// <param name="makeHiddenWhenDelete">If true object hides instead deleteing</param>
        /// <returns>ObjectSaver for chain support</returns>
        public IObjectSaver<TEntity> SaveOneToMany<TEntry>(Func<TEntity, ICollection<TEntry>> property,
            bool makeHiddenWhenDelete) where TEntry : BaseObject
        {
            return this.SaveOneToManyMethodImplementation(property, makeHiddenWhenDelete, null);
        }

        /// <summary>
        /// Prepare object to save with Entity Framework for One to Many relationship
        /// </summary>
        /// <typeparam name="TEntry">Object type that inherits from BaseObject, type of collection entry</typeparam>
        /// <param name="property">Many to Many navigation property</param>
        /// <param name="makeHiddenWhenDelete">If true object hides instead deleteing</param>
        /// <param name="entrySaverDelegate">Entry saver delegate</param>
        /// <returns>ObjectSaver for chain support</returns>
        public IObjectSaver<TEntity> SaveOneToMany<TEntry>(Func<TEntity, ICollection<TEntry>> property, bool makeHiddenWhenDelete,
            Action<IObjectSaver<TEntry>> entrySaverDelegate) where TEntry : BaseObject
        {
            return this.SaveOneToManyMethodImplementation(property, makeHiddenWhenDelete, entrySaverDelegate);
        }

        protected IObjectSaver<TEntity> SaveOneToManyMethodImplementation<TEntry>(Func<TEntity, ICollection<TEntry>> property, bool makeHiddenWhenDelete,
            Action<IObjectSaver<TEntry>> entrySaverDelegate) where TEntry : BaseObject
        {
            #region Contract.Requires

            if (property == null) throw new ArgumentNullException("property");

            #endregion

            ICollection<TEntry> newCollection = property(_objSrc).With(x => x.ToList());

            IRepository<TEntry> entryRepository = _unitOfWork.GetRepository<TEntry>();

            ICollection<TEntry> objectEntriesDest = property(_objDest);

            if (newCollection != null)
            {
                if (!_isNew)
                {
                    #region Update existing entries

                    ICollection<TEntry> toUpdateSrc = newCollection.Where(x => x.ID != 0 && !x.Hidden).ToList();

                    foreach (TEntry entry in toUpdateSrc)
                    {
                        ObjectSaver<TEntry> entrySaver = new ObjectSaver<TEntry>(_unitOfWork, entry);

                        if (entrySaverDelegate != null) entrySaverDelegate(entrySaver);
                    }

                    #endregion

                    #region Delete entries

                    IEnumerable<int> toUpdateIDs = toUpdateSrc.Select(x => x.ID);

                    ICollection<TEntry> toDeleteDest = objectEntriesDest.Where(x => !toUpdateIDs.Contains(x.ID) && x.ID != 0).ToList();

                    if (!makeHiddenWhenDelete)
                    {
                        foreach (TEntry entry in toDeleteDest)
                        {
                            objectEntriesDest.Remove(entry);

                            newCollection.Remove(entry);

                            entryRepository.Delete(entry);
                        }
                    }
                    else
                    {
                        foreach (TEntry entry in toDeleteDest)
                        {
                            entry.Hidden = true;

                            entryRepository.Update(entry);
                        }
                    }

                    #endregion
                }

                #region Create new entries

                ICollection<TEntry> toCreate = newCollection.Where(x => x.ID == 0).ToList();

                foreach (var entry in toCreate)
                {
                    ObjectSaver<TEntry> entrySaver = new ObjectSaver<TEntry>(_unitOfWork, entry);

                    if (entrySaverDelegate != null) entrySaverDelegate(entrySaver);

                    if (!_isNew && !objectEntriesDest.Contains(entrySaver.Dest))
                        objectEntriesDest.Add(entrySaver.Dest);
                }

                #endregion
            }
            else if (!_isNew && objectEntriesDest != null)
            {
                foreach (TEntry entry in objectEntriesDest.ToList())
                {
                    objectEntriesDest.Remove(entry);

                    entryRepository.Delete(entry);
                }
            }

            return this;
        }

        #endregion
    }
}
