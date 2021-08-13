namespace Web.Template.Data.Lookup.Repositories.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;

    using Web.Template.Domain.Interfaces.Entity;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Generic;

    /// <summary>
    /// Default repository handles basic CRUD operations
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Generic.ILookupRepository{TEntity}" />
    /// <seealso cref="ILookupRepository{T}" />
    public abstract class LookupRepository<TEntity> : ILookupRepository<TEntity>
        where TEntity : class, ILookup
    {
        /// <summary>
        /// The database set
        /// </summary>
        protected readonly DbSet<TEntity> DbSet;

        /// <summary>
        /// The context
        /// </summary>
        private readonly DbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="LookupRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public LookupRepository(DbContext dbContext)
        {
            DbContext context;
            dbContext.Database.Log = msg => Trace.WriteLine(msg);

            this.context = dbContext;
            this.DbSet = this.context.Set<TEntity>();
        }

        /// <summary>
        /// Returns all members that match a specific predicate..
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="includeFunc">The include function.</param>
        /// <returns>
        /// A List of items in the repository that match the specified predicate
        /// </returns>
        public virtual IEnumerable<TEntity> FindBy(
            Expression<Func<TEntity, bool>> predicate, 
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includeFunc)
        {
            var query = this.DbSet.AsQueryable();
            List<TEntity> list;
            if (includeFunc != null)
            {
                list = includeFunc(query).Where(predicate).ToList();
            }
            else
            {
                list = query.Where(predicate).ToList();
            }

            return list;
        }

        /// <summary>
        /// Returns all members.
        /// </summary>
        /// <param name="includeFunc">The include function.</param>
        /// <param name="clearCache">if set to <c>true</c> [clear cache].</param>
        /// <returns>
        /// All items in the repository
        /// </returns>
        public virtual IEnumerable<TEntity> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>> includeFunc, bool clearCache = false)
        {
            IQueryable<TEntity> resultWithEagerLoading;
            if (includeFunc != null)
            {
                resultWithEagerLoading = includeFunc(this.DbSet.AsQueryable());
            }
            else
            {
                resultWithEagerLoading = this.DbSet.AsQueryable();
            }

            return resultWithEagerLoading.ToList();
        }

        /// <summary>
        /// Gets the a single item by ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="includeFunc">The include function.</param>
        /// <returns>
        /// The item in the repository that matches the passed in ID
        /// </returns>
        public virtual TEntity GetSingle(int id, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeFunc)
        {
            IQueryable<TEntity> resultWithEagerLoading;
            if (includeFunc != null)
            {
                resultWithEagerLoading = includeFunc(this.DbSet.AsQueryable());
            }
            else
            {
                resultWithEagerLoading = this.DbSet.AsQueryable();
            }

            return resultWithEagerLoading.FirstOrDefault(e => e.Id == id);
        }
    }
}