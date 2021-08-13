namespace Web.Template.Application.Prebook.Adaptor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using iVectorConnectInterface.Basket;

    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Interfaces.Prebook;
    using Web.Template.Application.IVectorConnect.Requests;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Class that builds Property search requests
    /// </summary>
    /// <seealso cref="IBasketPrebookService" />
    /// <seealso cref="IBasketPrebookService" />
    /// <seealso cref="IBasketPrebookService" />
    public class ConnectBasketPrebookService : IBasketPrebookService
    {
        /// <summary>
        /// The connect login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory connectLoginDetailsFactory;

        /// <summary>
        /// The i vector connect request factory
        /// </summary>
        private readonly IIVectorConnectRequestFactory connectRequestFactory;

        /// <summary>
        /// The prebook request adaptor factory
        /// </summary>
        private readonly IPrebookRequestAdaptorFactory prebookRequestAdaptorFactory;

        /// <summary>
        /// The prebook response processor
        /// </summary>
        private readonly IPrebookResponseProcessor prebookResponseProcessor;

        /// <summary>
        /// The prebook return factory
        /// </summary>
        private readonly IPrebookReturnBuilder prebookReturnBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectBasketPrebookService" /> class.
        /// </summary>
        /// <param name="connectRequestFactory">The i vector connect request factory.</param>
        /// <param name="prebookRequestAdaptorFactory">The prebook request adaptor factory.</param>
        /// <param name="prebookReturnBuilder">The prebook return builder.</param>
        /// <param name="prebookResponseProcessor">The prebook response processor.</param>
        /// <param name="connectLoginDetailsFactory">The connect login details factory.</param>
        public ConnectBasketPrebookService(
            IIVectorConnectRequestFactory connectRequestFactory, 
            IPrebookRequestAdaptorFactory prebookRequestAdaptorFactory, 
            IPrebookReturnBuilder prebookReturnBuilder, 
            IPrebookResponseProcessor prebookResponseProcessor, 
            IConnectLoginDetailsFactory connectLoginDetailsFactory)
        {
            this.connectRequestFactory = connectRequestFactory;
            this.prebookRequestAdaptorFactory = prebookRequestAdaptorFactory;
            this.prebookReturnBuilder = prebookReturnBuilder;
            this.prebookResponseProcessor = prebookResponseProcessor;
            this.connectLoginDetailsFactory = connectLoginDetailsFactory;
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="connectRequestBody">The connect request body.</param>
        /// <returns>A prebook response</returns>
        public PreBookResponse GetResponse(PreBookRequest connectRequestBody)
        {
            var connectRequest = this.connectRequestFactory.Create(connectRequestBody, HttpContext.Current);
            var preBookResponse = connectRequest.Go<PreBookResponse>();

            return preBookResponse;
        }

        /// <summary>
        /// Creates a search connect search request using a WebTemplate search model.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <param name="componentToken">Token used to retrieve a component</param>
        /// <returns>
        /// A Connect Property search request
        /// </returns>
        public IPrebookReturn Prebook(IBasket basket, int componentToken = 0)
        {
            this.prebookReturnBuilder.AddWarnings(this.ValidateBasket(basket));

            try
            {
                if (this.prebookReturnBuilder.CurrentlySuccessful)
                {
                    IBasketComponent basketComponent = null;
                    if (componentToken > 0)
                    {
                        basketComponent = basket.Components.FirstOrDefault(c => c.ComponentToken == componentToken);
                    }

                    PreBookRequest connectRequestBody = this.BuildPrebookRequest(basket, basketComponent);
                    int totalBookings = connectRequestBody.TransferBookings.Count + connectRequestBody.CarHireBookings.Count + connectRequestBody.ExtraBookings.Count + connectRequestBody.FlightBookings.Count
                                        + connectRequestBody.PropertyBookings.Count;
                    if (totalBookings > 0)
                    {
                        var preBookResponse = this.GetResponse(connectRequestBody);
                        this.ProcessResponse(preBookResponse, basket, basketComponent);
                    }
                }
            }
            catch (Exception exception)
            {
                this.prebookReturnBuilder.SetToFailure();
                this.prebookReturnBuilder.AddWarning(exception.ToString());
            }

            IPrebookReturn prebookReturn = this.prebookReturnBuilder.Build();

            return prebookReturn;
        }

        /// <summary>
        /// Builds the prebook request.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <param name="basketComponent">The basket component.</param>
        /// <returns>A prebook request</returns>
        public PreBookRequest BuildPrebookRequest(IBasket basket, IBasketComponent basketComponent)
        {
            PreBookRequest connectRequestBody = this.SetupConnectRequest();

            if (basketComponent != null)
            {
                this.BuildSingleComponentPrebookRequest(basket, basketComponent, connectRequestBody);
            }
            else
            {
                this.BuildPrebookRequestForAllComponents(basket, connectRequestBody);
            }

            return connectRequestBody;
        }

        /// <summary>
        /// Builds the prebook request for all components.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <param name="connectRequestBody">The connect request body.</param>
        private void BuildPrebookRequestForAllComponents(IBasket basket, PreBookRequest connectRequestBody)
        {
            foreach (IBasketComponent component in basket.Components)
            {
                var requestAdaptor = this.prebookRequestAdaptorFactory.CreateAdaptorByComponentType(component.ComponentType);
                requestAdaptor.Create(component, connectRequestBody);
            }
        }

        /// <summary>
        /// Builds the single component prebook request.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <param name="basketComponent">The basket component.</param>
        /// <param name="connectRequestBody">The connect request body.</param>
        private void BuildSingleComponentPrebookRequest(IBasket basket, IBasketComponent basketComponent, PreBookRequest connectRequestBody)
        {
            if (basketComponent != null)
            {
                var requestAdaptor = this.prebookRequestAdaptorFactory.CreateAdaptorByComponentType(basketComponent.ComponentType);
                requestAdaptor.Create(basketComponent, connectRequestBody);
            }
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="preBookResponse">The pre book response.</param>
        /// <param name="basket">The basket.</param>
        /// <param name="basketComponent">The basket component.</param>
        private void ProcessResponse(PreBookResponse preBookResponse, IBasket basket, IBasketComponent basketComponent = null)
        {
            this.prebookReturnBuilder.AddResponse(preBookResponse);
            this.prebookReturnBuilder.AddWarnings(preBookResponse.ReturnStatus.Exceptions);

            if (this.prebookReturnBuilder.CurrentlySuccessful)
            {
                this.prebookReturnBuilder.SetPrePreBookPrice(basket.TotalPrice);

                this.prebookResponseProcessor.Process(preBookResponse, basket, basketComponent);

                this.prebookReturnBuilder.SetPostPreBookPrice(basket.TotalPrice);
            }

            this.prebookReturnBuilder.SetBasket(basket);
        }

        /// <summary>
        /// Setups the connect request.
        /// </summary>
        /// <returns>
        /// A bare bones connect request
        /// </returns>
        private PreBookRequest SetupConnectRequest()
        {
            var connectRequest = new PreBookRequest() { LoginDetails = this.connectLoginDetailsFactory.Create(HttpContext.Current), };
            return connectRequest;
        }

        /// <summary>
        /// Validates the basket.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <returns>A list of warnings</returns>
        private List<string> ValidateBasket(IBasket basket)
        {
            var warnings = new List<string>();

            if (basket == null)
            {
                warnings.Add("You can not prebook a basket that does not exist.");
            }

            if (basket?.Components == null || basket?.Components?.Count == 0)
            {
                warnings.Add("You need atleast one component in your basket to prebook");
            }

            return warnings;
        }
    }
}