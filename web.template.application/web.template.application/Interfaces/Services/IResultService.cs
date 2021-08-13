namespace Web.Template.Application.Interfaces.Services
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Results Service used to save and retrieve results from a store.
    /// </summary>
    public interface IResultService
    {
        /// <summary>
        /// Retrieves a result that matches specified token.
        /// </summary>
        /// <param name="searchToken">The search token.</param>
        /// <param name="componentToken">The component token.</param>
        /// <returns>
        /// A results model that matches the specified token
        /// </returns>
        IResult RetrieveResult(string searchToken, int componentToken);

        /// <summary>
        /// Retrieves a result that matches specified token.
        /// </summary>
        /// <param name="searchToken">The search token.</param>
        /// <returns>
        /// A results model that matches the specified token
        /// </returns>
        IResultsModel RetrieveResults(string searchToken);

        /// <summary>
        /// Retrieves the search model.
        /// </summary>
        /// <param name="searchToken">The search token.</param>
        /// <returns>The search model used to carry out the search</returns>
        ISearchModel RetrieveSearchModel(string searchToken);

        /// <summary>
        /// Retrieves the extra search model.
        /// </summary>
        /// <param name="searchToken">The search token.</param>
        /// <returns>The extra search model.</returns>
        IExtraSearchModel RetrieveExtraSearchModel(string searchToken);

        /// <summary>
        /// Saves the specified results models.
        /// </summary>
        /// <param name="resultsModels">The results models.</param>
        void SaveResults(List<IResultsModel> resultsModels);
    }
}