namespace Web.Template.Data.Lookup.Repositories.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web;

    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Wrapper around LookupRepository to implement a second level cache of the data.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="Web.Template.Data.Lookup.Repositories.Generic.LookupRepository{TEntity}" />
    /// <seealso cref="LookupRepository{TEntity}" />
    public class CachedLookupRepository<TEntity> : LookupRepository<TEntity>
        where TEntity : class, ILookup
    {
        /// <summary>
        /// The cache lock object
        /// </summary>
        private static readonly object CacheLockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedLookupRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public CachedLookupRepository(DbContext dbContext)
            : base(dbContext)
        {
        }

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <value>
        /// The cache key.
        /// </value>
        private string CacheKey => $"{typeof(TEntity).ToString()}_repositoryset";

        /// <summary>
        /// Returns all members that match a specific predicate..
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="includeFunc">The include function.</param>
        /// <returns>
        /// A List of items in the repository that match the specified predicate
        /// </returns>
        public override IEnumerable<TEntity> FindBy(
            Expression<Func<TEntity, bool>> predicate, 
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includeFunc)
        {
            return this.GetAll(includeFunc).Where(predicate.Compile()).ToList();
        }

        /// <summary>
        /// Returns all members.
        /// </summary>
        /// <param name="includeFunc">The include function.</param>
        /// <param name="clearCache">if set to <c>true</c> [clear cache].</param>
        /// <returns>
        /// All items in the repository
        /// </returns>
        public override IEnumerable<TEntity> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>> includeFunc, bool clearCache = false)
        {
            List<TEntity> result = null;
            if (!clearCache)
            {
                result = HttpRuntime.Cache[this.CacheKey] as List<TEntity>;
            }
               
            if (result == null)
            {
                lock (CacheLockObject)
                {
                    result = HttpRuntime.Cache[this.CacheKey] as List<TEntity>;
                    if (result == null)
                    {
                        result = base.GetAll(includeFunc).ToList();
                        HttpRuntime.Cache.Insert(
                            this.CacheKey, 
                            result, 
                            null, 
                            DateTime.Now.AddSeconds(60), 
                            TimeSpan.Zero);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the a single item by ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="includeFunc">The include function.</param>
        /// <returns>
        /// The item in the repository that matches the passed in ID
        /// </returns>
        public override TEntity GetSingle(int id, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeFunc)
        {
            return this.GetAll(includeFunc).FirstOrDefault(x => x.Id == id);
        }
    }
}