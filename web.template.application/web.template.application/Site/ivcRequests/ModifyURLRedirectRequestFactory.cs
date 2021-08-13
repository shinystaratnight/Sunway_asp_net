namespace Web.Template.Application.Site.ivcRequests
{
    using System.Web;
    using iVectorConnectInterface.Interfaces;
    using Interfaces.Site.ivcRequests;
    using IVectorConnect.Requests;
    using ivci = iVectorConnectInterface;

    /// <summary>
    ///     Builds a connect request for the Add URL Redirect Request
    /// </summary>
    public class ModifyUrlRedirectRequestFactory : IModifyUrlRedirectRequestFactory
    {
        /// <summary>
        ///     The connect login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory connectLoginDetailsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyUrlRedirectRequestFactory"/> class.
        /// </summary>
        /// <param name="connectLoginDetailsFactory">The connect login details factory.</param>
        public ModifyUrlRedirectRequestFactory(IConnectLoginDetailsFactory connectLoginDetailsFactory)
        {
            this.connectLoginDetailsFactory = connectLoginDetailsFactory;
        }

        /// <summary>
        /// Creates a connect request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <param name="redirectId">The id stored against the redirect</param>
        /// <returns>a connect request</returns>
        public iVectorConnectRequest Create(string url, string redirectUrl, int redirectId)
        {
            {
                var request = new ivci.ModifyURLRedirectRequest()
                {
                    CurrentURL = redirectUrl,
                    LoginDetails = connectLoginDetailsFactory.Create(HttpContext.Current, true),
                    OldURL = url,
                    RedirectID = redirectId
                };
                return request;
            }
        }
    }
}