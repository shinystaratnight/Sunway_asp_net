namespace Web.Template.Application.Interfaces.Search
{
    using System;

    /// <summary>
    /// A Request Adaptor factory decides which request adaptor we want to used based on the response type.
    /// </summary>
    public interface ISearchRequestAdaptorFactory
    {
        /// <summary>
        /// Creates the type of the adaptor by response.
        /// </summary>
        /// <param name="searchResponseType">Type of the search response.</param>
        /// <returns>A Request Adaptor</returns>
        ISearchRequestAdapter CreateAdaptorByResponseType(Type searchResponseType);
    }
}