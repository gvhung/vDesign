using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Base.DAL
{
    public interface IObjectSaver<TEntity> where TEntity : BaseObject
    {
        bool IsNew { get; }
        TEntity Dest { get; }
        TEntity Src { get; }

        /// <summary>
        /// Returns new ObjectSaver with current context for derrived Type
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        IObjectSaver<TObject> AsObjectSaver<TObject>() where TObject : BaseObject;

        /// <summary>
        /// Prepare object to save for Many to Many relationship
        /// </summary>
        /// <typeparam name="TEntry">Object type that inherits from BaseObject, type of collection entry</typeparam>
        /// <param name="property">Many to Many navigation property</param>
        /// <returns>ObjectSaver for chain support</returns>
        IObjectSaver<TEntity> SaveManyToMany<TEntry>(Func<TEntity, ICollection<TEntry>> property)
            where TEntry : BaseObject;

        /// <summary>
        /// Prepare object to save for One object relationship
        /// </summary>
        /// <typeparam name="TProperty">Object type that inherits from BaseObject, type of entry</typeparam>
        /// <param name="property">One object navigation property</param>
        /// <returns>ObjectSaver for chain support</returns>
        IObjectSaver<TEntity> SaveOneObject<TProperty>(Expression<Func<TEntity, TProperty>> property)
            where TProperty : BaseObject;

        /// <summary>
        /// Prepare object to save for One to Many relationship
        /// </summary>
        /// <typeparam name="TEntry">Object type that inherits from BaseObject, type of collection entry</typeparam>
        /// <param name="property">Many to Many navigation property</param>
        /// <returns>ObjectSaver for chain support</returns>
        IObjectSaver<TEntity> SaveOneToMany<TEntry>(Func<TEntity, ICollection<TEntry>> property)
            where TEntry : BaseObject;

        /// <summary>
        /// Prepare object to save for One to Many relationship
        /// </summary>
        /// <typeparam name="TEntry">Object type that inherits from BaseObject, type of collection entry</typeparam>
        /// <param name="property">Many to Many navigation property</param>
        /// <param name="entrySaverDelegate">Entry saver delegate</param>
        /// <returns>ObjectSaver for chain support</returns>
        IObjectSaver<TEntity> SaveOneToMany<TEntry>(Func<TEntity, ICollection<TEntry>> property,
            Action<IObjectSaver<TEntry>> entrySaverDelegate) where TEntry : BaseObject;

        /// <summary>
        /// Prepare object to save for One to Many relationship
        /// </summary>
        /// <typeparam name="TEntry">Object type that inherits from BaseObject, type of collection entry</typeparam>
        /// <param name="property">Many to Many navigation property</param>
        /// <param name="makeHiddenWhenDelete">If true object hides instead deleteing</param>
        /// <returns>ObjectSaver for chain support</returns>
        IObjectSaver<TEntity> SaveOneToMany<TEntry>(Func<TEntity, ICollection<TEntry>> property,
            bool makeHiddenWhenDelete) where TEntry : BaseObject;

        /// <summary>
        /// Prepare object to save for One to Many relationship
        /// </summary>
        /// <typeparam name="TEntry">Object type that inherits from BaseObject, type of collection entry</typeparam>
        /// <param name="property">Many to Many navigation property</param>
        /// <param name="makeHiddenWhenDelete">If true object hides instead deleteing</param>
        /// <param name="entrySaverDelegate">Entry saver delegate</param>
        /// <returns>ObjectSaver for chain support</returns>
        IObjectSaver<TEntity> SaveOneToMany<TEntry>(Func<TEntity, ICollection<TEntry>> property,
            bool makeHiddenWhenDelete,
            Action<IObjectSaver<TEntry>> entrySaverDelegate) where TEntry : BaseObject;
    }
}