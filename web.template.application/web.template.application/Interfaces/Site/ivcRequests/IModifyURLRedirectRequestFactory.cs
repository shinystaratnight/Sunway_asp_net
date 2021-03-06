namespace Web.Template.Application.Interfaces.Site.ivcRequests
{
    using iVectorConnectInterface.Interfaces;

    /// <summary>
    ///  Builds a connect request for the Add URL Redirect Request
    /// </summary>
    public interface IModifyUrlRedirectRequestFactory
    {
        /// <summary>
        /// Creates the specified URL.
        /// </summary>
        /// <returns>a connect request</returns>
        iVectorConnectRequest Create(string url, string redirectUrl, int redirectId);
    }
}