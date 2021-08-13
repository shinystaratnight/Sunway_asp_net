namespace Web.Template.Application.Interfaces.Results
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Builds up a IResult using an IVC search result
    /// </summary>
    public interface IConnectResultsAdaptor
    {
        /// <summary>
        /// Gets the type of the response.
        /// </summary>
        /// <value>
        /// The type of the response.
        /// </value>
        Type ResponseType { get; }

        /// <summary>
        /// Creates the specified connect result.
        /// </summary>
        /// <param name="connectResponse">The connect response.</param>
        /// <param name="searchModel">The search model.</param>
        /// <param name="context">The context.</param>
        /// <returns>A single property result</returns>
        List<IResultsModel> Create(iVectorConnectResponse connectResponse,  ISearchModel searchModel, HttpContext context);
    }
}