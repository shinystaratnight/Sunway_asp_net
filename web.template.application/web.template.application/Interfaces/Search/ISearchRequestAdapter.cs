namespace Web.Template.Application.Interfaces.Search
{
    using System;
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    ///     Interface For classes that want to take a search model and produce a Connect property search request
    /// </summary>
    public interface ISearchRequestAdapter
    {
        /// <summary>
        ///     Gets the type of the request.
        /// </summary>
        /// <value>
        ///     The type of the request.
        /// </value>
        Type ResponseType { get; }

        /// <summary>
        /// Creates a search connect search request using a WebTemplate search model.
        /// </summary>
        /// <param name="searchmodel">The search model.</param>
        /// <param name="context">The context.</param>
        /// <returns>A Connect Property search request</returns>
        iVectorConnectRequest Create(ISearchModel searchmodel, HttpContext context);
    }
}