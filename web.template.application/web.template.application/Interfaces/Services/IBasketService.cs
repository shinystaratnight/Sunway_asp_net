namespace Web.Template.Application.Interfaces.Services
{
    using System;
    using System.Collections.Generic;

    using Web.Template.Application.Basket.Models;
    using Web.Template.Application.Interfaces.Basket;
    using Web.Template.Application.Interfaces.Book;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Prebook;
    using Web.Template.Application.Interfaces.Promocode;

    /// <summary>
    /// Interface representing the basket service
    /// </summary>
    public interface IBasketService
    {
        /// <summary>
        /// Adds the basket component.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="basketComponent">The basket component.</param>
        /// <returns>The updated basket.</returns>
        IBasket AddBasketComponent(string basketToken, IBasketComponent basketComponent);

        /// <summary>
        /// Adds the component.
        /// </summary>
        /// <param name="componentModel">The component model.</param>
        /// <returns>
        /// A basket
        /// </returns>
        IBasket AddComponent(BasketComponentModel componentModel);

        /// <summary>
        /// Applies the promo code.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="promoCode">The promotional code to be applied.</param>
        /// <returns>A promotional code return</returns>
        IPromoCodeReturn ApplyPromoCode(string basketToken, string promoCode);

        /// <summary>
        ///     Books the basket.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <returns>A basket</returns>
        IBookReturn BookBasket(string basketToken);

        /// <summary>
        ///     Changes the guests.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="guestDetails">The guest details.</param>
        /// <returns>A basket</returns>
        IBasket ChangeGuests(string basketToken, List<BasketRoom> guestDetails);

        /// <summary>
        ///     Changes the lead guest.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="leadGuestDetails">The lead guest details.</param>
        /// <returns>A basket</returns>
        IBasket ChangeLeadGuest(string basketToken, LeadGuestDetails leadGuestDetails);

        /// <summary>
        ///     Changes the payment.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="paymentDetails">The payment details.</param>
        /// <returns>A basket</returns>
        IBasket ChangePayment(string basketToken, PaymentDetails paymentDetails);

        /// <summary>
        /// Changes the payment.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="tradeReference">The trade reference.</param>
        /// <returns>
        /// A basket
        /// </returns>
        IBasket ChangeTradeReference(string basketToken, string tradeReference);

        /// <summary>
        /// Creates the basket.
        /// </summary>
        /// <returns>The basket token</returns>
        Guid CreateBasket();

        /// <summary>
        /// Gets the basket.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <returns>The basket</returns>
        IBasket GetBasket(string basketToken);

        /// <summary>
        ///     Pre-book basket.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <returns>A basket</returns>
        IPrebookReturn PreBookBasket(string basketToken);

        /// <summary>
        ///     Pre-book component.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="componentToken">The component token.</param>
        /// <returns>A basket</returns>
        IPrebookReturn PreBookComponent(string basketToken, int componentToken);

        /// <summary>
        /// Gets the cancellation charges.
        /// </summary>
        /// <param name="componentModels">The component models.</param>
        /// <returns></returns>
        IComponentPaymentCancellationReturn GetCancellationCharges(List<BasketComponentModel> componentModels);

        /// <summary>
        /// Releases the flight seat lock.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <returns>The flight seat lock return status.</returns>
        string ReleaseFlightSeatLock(string basketToken);

        /// <summary>
        ///     Removes the component.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="componentToken">The component token.</param>
        /// <param name="subComponentTokens">Token representing sub components</param>
        /// <returns>A basket</returns>
        IBasket RemoveComponent(string basketToken, int componentToken, List<int> subComponentTokens = null);

        /// <summary>
        /// Applies the promo code.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <returns>The basket</returns>
        IBasket RemovePromoCode(string basketToken);

        /// <summary>
        /// Retrieves the stored basket.
        /// </summary>
        /// <param name="basketStoreId">The basket store identifier.</param>
        /// <returns>The Basket.</returns>
        IBasket RetrieveStoredBasket(int basketStoreId);

        /// <summary>
        /// Stores the basket.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="basketStoreId">The basket store identifier.</param>
        /// <returns>The basket store id</returns>
        int StoreBasket(string basketToken, int basketStoreId);

        /// <summary>
        /// Updates the basket.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="basket">The basket.</param>
        /// <returns>The Basket.</returns>
        IBasket UpdateBasket(string basketToken, IBasket basket);

        /// <summary>
        /// Updates the component.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="basketComponent">The basket component</param>
        /// <returns> A basket</returns>
        IBasket UpdateBasketComponent(string basketToken, IBasketComponent basketComponent);

        /// <summary>
        /// Changes the property requests.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <param name="hotelRequest">The hotel request.</param>
        /// <returns>A basket</returns>
        IBasket ChangePropertyRequests(string basketToken, string hotelRequest);
    }
}