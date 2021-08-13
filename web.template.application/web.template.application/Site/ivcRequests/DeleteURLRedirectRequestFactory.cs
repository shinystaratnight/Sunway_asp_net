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
    public class DeleteUrlRedirectRequestFactory : IDeleteUrlRedirectRequestFactory
    {
        /// <summary>
        ///     The connect login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory connectLoginDetailsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteUrlRedirectRequestFactory"/> class.
        /// </summary>
        /// <param name="connectLoginDetailsFactory">The connect login details factory.</param>
        public DeleteUrlRedirectRequestFactory(IConnectLoginDetailsFactory connectLoginDetailsFactory)
        {
            this.connectLoginDetailsFactory = connectLoginDetailsFactory;
        }

        /// <summary>
        /// Creates a connect request.
        /// </summary>
        /// <param name="redirectId">The id stored against the redirect</param>
        /// <returns>a connect request</returns>
        public iVectorConnectRequest Create(int redirectId)
        {
            {
                var request = new ivci.DeleteURLRedirectRequest()
                {
                    LoginDetails = connectLoginDetailsFactory.Create(HttpContext.Current, true),
                    RedirectID = redirectId
                };
                return request;
            }
        }
    }
}