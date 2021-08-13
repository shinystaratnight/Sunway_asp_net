namespace Web.Template.Application.Interfaces.Site.ivcRequests
{
    using iVectorConnectInterface.Interfaces;

    /// <summary>
    ///  Builds a connect request for the Add URL Redirect Request
    /// </summary>
    public interface IAddUrlRedirectRequestFactory
    {
        /// <summary>
        /// Creates the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <returns>a connect request</returns>
        iVectorConnectRequest Create(string url, string redirectUrl);
    }
}