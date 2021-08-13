namespace Web.Template.Application.Interfaces.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    ///     The service used to carry out searches
    /// </summary>
    public interface ISearchService : IDisposable
    {
        /// <summary>
        ///     Searches the specified search model.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        /// <returns>A results model</returns>
        Task<List<IResultsModel>> Search(ISearchModel searchModel);
    }
}