namespace Web.Template.Domain.Interfaces.Lookup.Repositories.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// The base interface the look up repositories will extend
    /// </summary>
    /// <typeparam name="T">The class of entity the repository will contain</typeparam>
    public interface ILookupRepository<T>
        where T : class
    {
        /// <summary>
        /// Returns all members that match a specific predicate..
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="includeFunc">The include function.</param>
        /// <returns>
        /// A List of items in the repository that match the specified predicate
        /// </returns>
        IEnumerable<T> FindBy(
            Expression<Func<T, bool>> predicate, 
            Func<IQueryable<T>, IQueryable<T>> includeFunc = null);

        /// <summary>
        /// Returns all members.
        /// </summary>
        /// <param name="includeFunc">The include function.</param>
        /// <param name="clearcache">If set to <c>true</c>  then the collection will be refreshed in the cache prior to the retrieve.</param>
        /// <returns>
        /// All items in the repository
        /// </returns>
        IEnumerable<T> GetAll(Func<IQueryable<T>, IQueryable<T>> includeFunc = null, bool clearcache = false);

        /// <summary>
        /// Gets the a single item by ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="includeFunc">The include function.</param>
        /// <returns>
        /// The item in the repository that matches the passed in ID
        /// </returns>
        T GetSingle(int id, Func<IQueryable<T>, IQueryable<T>> includeFunc = null);
    }
}