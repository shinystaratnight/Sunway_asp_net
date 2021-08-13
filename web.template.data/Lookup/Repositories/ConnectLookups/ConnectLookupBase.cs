namespace Web.Template.Data.Lookup.Repositories.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web;
    using System.Xml;
    using Application.Interfaces.Services;
    using Application.Interfaces.User;
    using Application.User.Models;
    using Connect;
    using Domain.Interfaces.Entity;
    using Domain.Interfaces.Lookup.Repositories.Generic;
    using Intuitive;

    /// <summary>
    ///     Default repository handles basic CRUD operations
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="Web.Template.Domain.Interfaces.Lookup.Repositories.Generic.ILookupRepository{TEntity}" />
    /// <seealso cref="ILookupRepository{T}" />
    public abstract class ConnectLookupBase<TEntity> : ILookupRepository<TEntity>
        where TEntity : class, ILookup
    {
        /// <summary>
        ///     The cache lock object
        /// </summary>
        private static readonly object CacheLockObject = new object();

        /// <summary>
        ///     The asynchronous lookup
        /// </summary>
        private readonly IAsyncLookup asyncLookup;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConnectLookupBase{TEntity}" /> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public ConnectLookupBase(IAsyncLookup asyncLookup)
        {
            this.asyncLookup = asyncLookup;

            var result = HttpRuntime.Cache[CacheKey] as List<TEntity>;
            if (result == null)
                lock (CacheLockObject)
                {
                    result = HttpRuntime.Cache[CacheKey] as List<TEntity>;
                    if (result == null)
                        try
                        {
                            result = Setup();
                            HttpRuntime.Cache.Insert(CacheKey,
                                result,
                                null,
                                DateTime.Now.AddHours(12),
                                TimeSpan.Zero);
                        }
                        catch (Exception ex)
                        {
                            FileFunctions.AddLogEntry(
                                "Lookups",
                                $"exception in {typeof(TEntity)} Repo",
                                ex.Message);
                        }
                }

            Collection = result;
        }

        /// <summary>
        ///     Gets the display language identifier from the user session.
        /// </summary>
        /// <value>
        ///     The display language identifier from the user session.
        /// </value>
        private int DisplayLanguageId
        {
            get
            {
                var displayLanguageId = 0;
                try
                {
                    if (HttpContext.Current != null)
                    {
                        string userCookie = Intuitive.CookieFunctions.Cookies.GetValue("__UserDetails");
                        IUserSession user = new UserSession();
                        if (!string.IsNullOrEmpty(userCookie))
                        {
                            string decryptedCookie = Intuitive.Functions.Decrypt(userCookie);
                            user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserSession>(decryptedCookie, new Newtonsoft.Json.Converters.StringEnumConverter());
                        }

                        displayLanguageId = user?.SelectedLanguage?.Id ?? 0;
                    }
                }
                catch (Exception ex)
                {
                    displayLanguageId = 0;
                    FileFunctions.AddLogEntry("Async Lookup", "error getting language", ex.Message);
                }

                return displayLanguageId;
            }
        }

        /// <summary>
        ///     Gets or sets the collection.
        /// </summary>
        /// <value>
        ///     The collection.
        /// </value>
        protected List<TEntity> Collection { get; set; }

        /// <summary>
        ///     Gets the cache key.
        /// </summary>
        /// <value>
        ///     The cache key.
        /// </value>
        private string CacheKey => $"{typeof(TEntity)}_repositoryset_{DisplayLanguageId}";

        /// <summary>
        ///     Returns all members that match a specific predicate..
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="includeFunc">The include function.</param>
        /// <returns>
        ///     A List of items in the repository that match the specified predicate
        /// </returns>
        public virtual IEnumerable<TEntity> FindBy(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includeFunc)
        {
            var list = new List<TEntity>();
            if (Collection != null)
            {
                IQueryable<TEntity> query = Collection.AsQueryable();

                if (includeFunc != null)
                    list = includeFunc(query).Where(predicate).ToList();
                else
                    list = query.Where(predicate).ToList();
            }

            return list;
        }

        /// <summary>
        ///     Returns all members.
        /// </summary>
        /// <param name="includeFunc">The include function.</param>
        /// <param name="clearCache">
        ///     If set to <c>true</c>  then the collection will be refreshed in the cache prior to the
        ///     retrieve.
        /// </param>
        /// <returns>
        ///     All items in the repository
        /// </returns>
        public virtual IEnumerable<TEntity> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>> includeFunc, bool clearCache = false)
        {
            if (clearCache) UpdateCache();

            IQueryable<TEntity> resultWithEagerLoading;
            if (includeFunc != null)
                resultWithEagerLoading = includeFunc(Collection.AsQueryable());
            else if (Collection != null)
                resultWithEagerLoading = Collection.AsQueryable();
            else
                resultWithEagerLoading = new List<TEntity>().AsQueryable();

            return resultWithEagerLoading.ToList();
        }

        /// <summary>
        ///     Gets the a single item by ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="includeFunc">The include function.</param>
        /// <returns>
        ///     The item in the repository that matches the passed in ID
        /// </returns>
        public virtual TEntity GetSingle(int id, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeFunc)
        {
            IQueryable<TEntity> resultWithEagerLoading;
            if (includeFunc != null)
                resultWithEagerLoading = includeFunc(Collection.AsQueryable());
            else
                resultWithEagerLoading = Collection.AsQueryable();

            return resultWithEagerLoading.FirstOrDefault(e => e.Id == id);
        }

        /// <summary>
        ///     Updates the cache.
        /// </summary>
        private void UpdateCache()
        {
            List<TEntity> newCollection = Setup();

            var currentCache = HttpRuntime.Cache[CacheKey] as List<TEntity>;

            if (currentCache != null)
                HttpRuntime.Cache[CacheKey] = newCollection;
            else
                lock (CacheLockObject)
                {
                    currentCache = HttpRuntime.Cache[CacheKey] as List<TEntity>;
                    if (currentCache == null)
                        try
                        {
                            HttpRuntime.Cache.Insert(CacheKey,
                                newCollection,
                                null,
                                DateTime.Now.AddHours(12),
                                TimeSpan.Zero);
                        }
                        catch (Exception ex)
                        {
                            FileFunctions.AddLogEntry(
                                "Lookups",
                                $"exception in {typeof(TEntity)} Repo",
                                ex.Message);
                        }
                    else
                        HttpRuntime.Cache[CacheKey] = newCollection;
                }

            this.Collection = newCollection;
        }

        /// <summary>
        /// Gets the lookups XML.
        /// </summary>
        /// <param name="nodeName">Name of the node.</param>
        /// <param name="clearcache">if set to <c>true</c> [clearcache].</param>
        /// <returns>
        /// an xml document.
        /// </returns>
        public virtual XmlDocument GetLookupsXml(string nodeName, bool clearcache = false)
        {
            XmlDocument xml = asyncLookup.GetAsyncLookup(nodeName, clearcache);
            return xml;
        }

        /// <summary>
        ///     Setups this instance.
        /// </summary>
        /// <returns>A list of the entity</returns>
        protected virtual List<TEntity> Setup()
        {
            return new List<TEntity>();
        }
    }
}