namespace Web.Template.Application.Payment.Services
{
    using System.Collections.Generic;
    using System.Web;

    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Interfaces.Payment;
    using Web.Template.Application.Net.IVectorConnect;
    using Web.Template.Application.Payment.Models;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Class ThreeDSecureService.
    /// </summary>
    public class ThreeDSecureService : IThreeDSecureService
    {
        /// <summary>
        /// The get3 d secure redirect factory
        /// </summary>
        private readonly IGet3DSecureRedirectRequestFactory get3DSecureRedirectFactory;

        /// <summary>
        /// The three d secure redirect return factory
        /// </summary>
        private readonly IThreeDSecureRedirectReturnFactory threeDSecureRedirectReturnFactory;

        /// <summary>
        /// The process3 d secure request factory
        /// </summary>
        private readonly IProcess3DSecureReturnRequestFactory process3DSecureRequestFactory;

        /// <summary>
        /// The process three d secure return factory
        /// </summary>
        private readonly IProcess3DSecureReturnFactory process3DSecureReturnFactory;

        /// <summary>
        /// The connect request factory
        /// </summary>
        private readonly IIVectorConnectRequestFactory connectRequestFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreeDSecureService"/> class.
        /// </summary>
        /// <param name="get3DSecureRedirectFactory">The get3 d secure redirect factory.</param>
        /// <param name="connectRequestFactory">The connect request factory.</param>
        /// <param name="threeDSecureRedirectReturnFactory">The three d secure redirect return factory.</param>
        /// <param name="process3DSecureRequestFactory">The process3 d secure request factory.</param>
        /// <param name="process3DSecureReturnFactory">The process three d secure return factory.</param>
        public ThreeDSecureService(IGet3DSecureRedirectRequestFactory get3DSecureRedirectFactory, IIVectorConnectRequestFactory connectRequestFactory, IThreeDSecureRedirectReturnFactory threeDSecureRedirectReturnFactory, IProcess3DSecureReturnRequestFactory process3DSecureRequestFactory, IProcess3DSecureReturnFactory process3DSecureReturnFactory)
        {
            this.get3DSecureRedirectFactory = get3DSecureRedirectFactory;
            this.connectRequestFactory = connectRequestFactory;
            this.threeDSecureRedirectReturnFactory = threeDSecureRedirectReturnFactory;
            this.process3DSecureRequestFactory = process3DSecureRequestFactory;
            this.process3DSecureReturnFactory = process3DSecureReturnFactory;
        }

        /// <summary>
        /// Get3s the d secure redirect.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns> The ThreeDSecureRedirectReturn.</returns>
        public IThreeDSecureRedirectReturn Get3DSecureRedirect(IThreeDSecureRedirectModel model)
        {
            var requestBody = this.get3DSecureRedirectFactory.Create(model);

            IIVectorConnectRequest ivcRequest = this.connectRequestFactory.Create(requestBody, HttpContext.Current);

            IThreeDSecureRedirectReturn redirectReturn = new ThreeDSecureRedirectReturn();

            List<string> warnings = requestBody.Validate();
            if (warnings.Count == 0)
            {
                ivci.Get3DSecureRedirectResponse redirectResponse = ivcRequest.Go<ivci.Get3DSecureRedirectResponse>(true);
                redirectReturn = this.threeDSecureRedirectReturnFactory.Create(redirectResponse);
            }

            return redirectReturn;
        }

        /// <summary>
        /// Process3s the d secure.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The ProcessThreeDSecureReturn.</returns>
        public IProcessThreeDSecureReturn Process3DSecure(IProcessThreeDSecureModel model)
        {
            var requestBody = this.process3DSecureRequestFactory.Create(model);


            IIVectorConnectRequest ivcRequest = this.connectRequestFactory.Create(requestBody, HttpContext.Current);

            ivci.Process3DSecureReturnResponse processResponse = null;

            List<string> warnings = requestBody.Validate();
            if (warnings.Count == 0)
            {
                processResponse = ivcRequest.Go<ivci.Process3DSecureReturnResponse>(true);
            }
            IProcessThreeDSecureReturn redirectReturn = this.process3DSecureReturnFactory.Create(processResponse);

            return redirectReturn;
        }
    }
}
