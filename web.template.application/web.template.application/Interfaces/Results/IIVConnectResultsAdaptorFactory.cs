namespace Web.Template.Application.Interfaces.Results
{
    using System;

    /// <summary>
    /// Connect Result adaptor factories decide which Result adaptor to use based on the response type
    /// </summary>
    public interface IIVConnectResultsAdaptorFactory
    {
        /// <summary>
        /// Creates the type of the adaptor by response.
        /// </summary>
        /// <param name="responseType">Type of the response.</param>
        /// <returns>A connect results adaptor</returns>
        IConnectResultsAdaptor CreateAdaptorByResponseType(Type responseType);
    }
}