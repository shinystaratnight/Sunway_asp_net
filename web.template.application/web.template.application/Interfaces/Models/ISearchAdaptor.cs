namespace Web.Template.Application.Interfaces.Models
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    /// <summary>
    ///     Interface for class that wraps around a Property search allowing us to switch it from connect if needs be.
    /// </summary>
    public interface ISearchAdaptor
    {
        /// <summary>
        /// Perform a package search
        /// </summary>
        /// <typeparam name="T">The type of the connect response</typeparam>
        /// <param name="searchModel">The search model.</param>
        /// <param name="token">The token.</param>
        /// <param name="context">The context.</param>
        /// <returns>The package search results</returns>
        Task<IPackageResultsModel> PackageSearch<T>(ISearchModel searchModel, CancellationToken token, HttpContext context) where T : class, iVectorConnectResponse, new();

        /// <summary>
        /// Searches the specified search model.
        /// </summary>
        /// <typeparam name="T">The type of the connect response</typeparam>
        /// <param name="searchModel">The search model.</param>
        /// <param name="token">The token.</param>
        /// <param name="context">The context.</param>
        /// <returns>A ResultsCollection model</returns>
        Task<IResultsModel> Search<T>(ISearchModel searchModel, CancellationToken token, HttpContext context) where T : class, iVectorConnectResponse, new();

        /// <summary>
        /// Extras the search.
        /// </summary>
        /// <typeparam name="T">The type of the connect response</typeparam>
        /// <param name="searchModel">The search model.</param>
        /// <param name="context">The context.</param>
        /// <returns>The results model.</returns>
        Task<IResultsModel> ExtraSearch<T>(IExtraSearchModel searchModel, HttpContext context) where T : class, iVectorConnectResponse, new();
    }
}