using System;
using System.Linq.Expressions;


namespace Base.DAL
{
    public interface IRepository<T> : IRepository where T : BaseObject
    {
        /// <summary>
        /// Gets all objects from database
        /// </summary>
        IExtendedQueryable<T> All();

        /// <summary>
        /// Gets objects from database by filter.
        /// </summary>
        /// <param name="predicate">Specified a filter</param>
        IExtendedQueryable<T> Filter(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Gets objects from database with filting and paging.
        /// </summary>
        /// <param name="filter">Specified a filter</param>
        /// <param name="total">Returns the total records count of the filter.</param>
        /// <param name="index">Specified the page index.</param>
        /// <param name="size">Specified the page size</param>
        IExtendedQueryable<T> Filter(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50);

        /// <summary>
        /// Gets the object(s) is exists in database by specified filter.
        /// </summary>
        /// <param name="predicate">Specified the filter expression</param>
        bool Contains(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Find object by keys.
        /// </summary>
        /// <param name="keys">Specified the search keys.</param>
        T Find(params object[] keys);

        /// <summary>
        /// Find object by specified expression.
        /// </summary>
        /// <param name="predicate"></param>
        T Find(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// TCreate a new object to database.
        /// </summary>
        /// <param name="t">Specified a new object to create.</param>
        T Create(T t);

        /// <summary>
        /// TDelete the object from database.
        /// </summary>
        /// <param name="t">Specified a existing object to delete.</param>        
        int Delete(T t);

        /// <summary>
        /// TDelete objects from database by specified filter expression.
        /// </summary>
        /// <param name="predicate"></param>
        int Delete(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// TUpdate object changes and save to database.
        /// </summary>
        /// <param name="t">Specified the object to save.</param>
        int Update(T t);

        /// <summary>
        /// Attach
        /// </summary>
        /// <param name="t">object attach.</param>
        void Attach(T t);

        /// <summary>
        /// Detach
        /// </summary>
        /// <param name="t">object attach.</param>
        void Detach(T t);

        /// <summary>
        /// Get the total objects count.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 
        /// </summary>
        IObjectSaver<T> GetObjectSaver(IUnitOfWork unitOfWork, T objSrc, T objDest);

        bool AutoDetectChangesEnabled { get; set; }
        bool ValidateOnSaveEnabled { get; set; }
    }

    public interface IRepository
    {

    }
}