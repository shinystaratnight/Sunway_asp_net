namespace Web.Booking.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Mvc;

    using Intuitive;

    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Payment;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Payment.Models;

    /// <summary>
    /// Class ThreeDSecureController.
    /// </summary>
    public class ThreeDSecureController : Controller
    {
        /// <summary>
        /// The basket service
        /// </summary>
        private readonly IBasketService basketService;

        /// <summary>
        /// The site service
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        /// The three d secure service
        /// </summary>
        private readonly IThreeDSecureService threeDSecureService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreeDSecureController" /> class.
        /// </summary>
        /// <param name="basketService">The basket service.</param>
        /// <param name="threeDSecureService">The three d secure service.</param>
        public ThreeDSecureController(IBasketService basketService, IThreeDSecureService threeDSecureService, ISiteService siteService)
        {
            this.basketService = basketService;
            this.threeDSecureService = threeDSecureService;
            this.siteService = siteService;
        }

        /// <summary>
        /// Setups the specified basket token.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <returns>The 3d secure page</returns>
        public ActionResult Submit([FromUri] string basketToken)
        {
            IBasket basket = this.basketService.GetBasket(basketToken);
            return this.View("_Submit", basket);
        }

        /// <summary>
        /// Responses this instance.
        /// </summary>
        /// <param name="basketStoreId">The basket store identifier.</param>
        /// <param name="formCollection">The form collection.</param>
        /// <returns>The ActionResult.</returns>
        [System.Web.Mvc.HttpPost]
        public ActionResult PaymentResponse([FromUri] int basketStoreId, [FromBody] FormCollection formCollection)
        {
            var basketToken = this.basketService.CreateBasket().ToString();
            IBasket basket = this.basketService.RetrieveStoredBasket(basketStoreId);

            ISite site = this.siteService.GetSite(System.Web.HttpContext.Current);

            if (basket == null)
            {
                return new RedirectResult($"{site.SiteConfiguration.OriginUrl}?warning=basketexpired");
            }

            IProcessThreeDSecureModel processThreeDSecureModel = new ProcessThreeDSecureModel()
                                                                     {
                                                                         PaymentDetails = basket.PaymentDetails
                                                                     };
            foreach (var key in formCollection.AllKeys)
            {
                processThreeDSecureModel.FormValues += $"{key}={formCollection[key]}&";
            }
            processThreeDSecureModel.FormValues = processThreeDSecureModel.FormValues.TrimEnd('&');

            IProcessThreeDSecureReturn processThreeDSecureReturn =
                this.threeDSecureService.Process3DSecure(processThreeDSecureModel);


            if (processThreeDSecureReturn.Success)
            {
                basket.PaymentDetails.ThreeDSecureCode = processThreeDSecureReturn.ThreeDSecureCode;
                basket.PaymentDetails.PaymentToken = processThreeDSecureReturn.PaymentToken;
                this.basketService.UpdateBasket(basketToken, basket);
                return new RedirectResult($"/booking/completebooking?t={basketToken}");
            }


            basket.PaymentDetails.ThreeDSecureCode = string.Empty;
            basket.PaymentDetails.PaymentToken = string.Empty;
            this.basketService.UpdateBasket(basketToken, basket);
            return new RedirectResult($"/booking/payment?t={basketToken}&warning=paymentfailed");
        }
    }
}