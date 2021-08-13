namespace Web.Template.Application.Interfaces.Search
{
    using System.Threading.Tasks;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Interface IExtraSearchService
    /// </summary>
    public interface IExtraSearchService
    {
        /// <summary>
        /// Searches from basket.
        /// </summary>
        /// <param name="extraBasketSearchModel">The extra basket search model.</param>
        /// <returns>The result</returns>
        Task<IResultsModel> SearchFromBasket(IExtraBasketSearchModel extraBasketSearchModel);
    }
}