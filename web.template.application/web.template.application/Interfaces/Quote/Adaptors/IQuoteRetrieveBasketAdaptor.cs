namespace Web.Template.Application.Interfaces.Quote.Adaptors
{
    using System.Collections.Generic;

    using iVectorConnectInterface;

    using Web.Template.Application.Basket.Models.Components;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Interface IQuoteRetrieveBasketAdaptor
    /// </summary>
    public interface IQuoteRetrieveBasketAdaptor
    {
        /// <summary>
        /// Creates the property component.
        /// </summary>
        /// <param name="quoteProperty">The quote property.</param>
        /// <param name="guestDetails">The guest details.</param>
        /// <returns>The basket component.</returns>
        Hotel CreatePropertyComponent(QuoteProperty quoteProperty, List<ivci.Support.GuestDetail> guestDetails);

        /// <summary>
        /// Creates the flight component.
        /// </summary>
        /// <param name="quoteFlight">The quote flight.</param>
        /// <param name="guestDetails">The guest details.</param>
        /// <returns>The basket component.</returns>
        Flight CreateFlightComponent(QuoteFlight quoteFlight, List<ivci.Support.GuestDetail> guestDetails);

        /// <summary>
        /// Creates the transfer component.
        /// </summary>
        /// <param name="quoteTransfer">The quote transfer.</param>
        /// <param name="guestDetails">The guest details.</param>
        /// <returns>The transfer component.</returns>
        Transfer CreateTransferComponent(QuoteTransfer quoteTransfer, List<ivci.Support.GuestDetail> guestDetails);
    }
}
