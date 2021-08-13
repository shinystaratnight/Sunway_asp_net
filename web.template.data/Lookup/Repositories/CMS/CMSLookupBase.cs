namespace Web.Template.Data.Lookup.Repositories.CMS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web;
    using System.Xml;

    using Web.Template.Application.Interfaces.User;
    using Web.Template.Data.Connect;
    using Web.Template.Domain.Interfaces.Entity;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Generic;

    /// <summary>
    /// Class CMSLookupBase.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    public abstract class CMSLookupBase<TEntity> : ILookupRepository<TEntity>
        where TEntity : class, ILookup
    {
        /// <summary>
        /// The cache lock object
        /// </summary>
        private static readonly object CacheLockObject = new object();

        /// <summary>
        /// The asynchronous lookup
        /// </summary>
        private readonly IAsyncLookup asyncLookup;

        /// <summary>
        /// Initializes a new instance of the <see cref="CMSLookupBase{TEntity}"/> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        protected CMSLookupBase(IAsyncLookup asyncLookup)
        {
            this.asyncLookup = asyncLookup;
            var displayLanguageId = 0;
            try
            {
                if (HttpContext.Current != null)
                {
                    var user = (IUserSession)HttpContext.Current.Session["userSession"];
                    if (user.SelectedLanguage != null)
                    {
                        displayLanguageId = user.SelectedLanguage.Id;
                    }
                    else
                    {
                        displayLanguageId = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                displayLanguageId = 0;
                Intuitive.FileFunctions.AddLogEntry("Async Lookup", "error getting language", ex.Message);
            }

            var result = HttpRuntime.Cache[$"{this.CacheKey}_{displayLanguageId}"] as List<TEntity>;
            if (result == null)
            {
                lock (CacheLockObject)
                {
                    result = HttpRuntime.Cache[$"{this.CacheKey}_{displayLanguageId}"] as List<TEntity>;
                    if (result == null)
                    {
                        try
                        {
                            result = this.Setup();
                            if (result?.Count > 0)
                            {
                                HttpRuntime.Cache.Insert(
                                    $"{this.CacheKey}_{displayLanguageId}", 
                                    result, 
                                    null, 
                                    DateTime.Now.AddHours(12), 
                                    TimeSpan.Zero);
                            }
                        }
                        catch (Exception ex)
                        {
                            Intuitive.FileFunctions.AddLogEntry(
                                "Lookups", 
                                $"exception in {typeof(TEntity)} Repo", 
                                ex.Message);
                        }
                    }
                }
            }

            this.Collection = result;
        }

        /// <summary>
        /// Gets or sets the collection.
        /// </summary>
        /// <value>The collection.</value>
        protected List<TEntity> Collection { get; set; }

        /// <summary>
        /// Gets or sets the cache key.
        /// </summary>
        /// <value>The cache key.</value>
        private string CacheKey => $"{typeof(TEntity)}_repositoryset";

        /// <summary>
        /// Returns all members that match a specific predicate..
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="includeFunc">The include function.</param>
        /// <returns>A List of items in the repository that match the specified predicate</returns>
        public IEnumerable<TEntity> FindBy(
            Expression<Func<TEntity, bool>> predicate, 
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includeFunc = null)
        {
            var list = new List<TEntity>();
            if (this.Collection != null)
            {
                var query = this.Collection.AsQueryable();

                if (includeFunc != null)
                {
                    list = includeFunc(query).Where(predicate).ToList();
                }
                else
                {
                    list = query.Where(predicate).ToList();
                }
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
        public IEnumerable<TEntity> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>> includeFunc = null, bool clearCache = false)
        {
            if (clearCache)
            {
                this.Setup();
            }

            IQueryable<TEntity> resultWithEagerLoading;
            if (includeFunc != null)
            {
                resultWithEagerLoading = includeFunc(this.Collection.AsQueryable());
            }
            else if (this.Collection != null)
            {
                resultWithEagerLoading = this.Collection.AsQueryable();
            }
            else
            {
                resultWithEagerLoading = new List<TEntity>().AsQueryable();
            }

            return resultWithEagerLoading.ToList();
        }

        /// <summary>
        /// Gets the lookups XML.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>an xml document.</returns>
        public XmlDocument GetLookupsXml(string objectType, int id)
        {
            var xml = this.asyncLookup.GetAsyncCMSLookup(objectType, id);
            return xml;
        }

        /// <summary>
        /// Gets the a single item by ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="includeFunc">The include function.</param>
        /// <returns>The item in the repository that matches the passed in ID</returns>
        public TEntity GetSingle(int id, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeFunc = null)
        {
            IQueryable<TEntity> resultWithEagerLoading;
            if (includeFunc != null)
            {
                resultWithEagerLoading = includeFunc(this.Collection.AsQueryable());
            }
            else
            {
                resultWithEagerLoading = this.Collection.AsQueryable();
            }

            return resultWithEagerLoading.FirstOrDefault(e => e.Id == id);
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>System.Collections.Generic.List&lt;TEntity&gt;.</returns>
        protected virtual List<TEntity> Setup()
        {
            return new List<TEntity>();
        }
    }
}