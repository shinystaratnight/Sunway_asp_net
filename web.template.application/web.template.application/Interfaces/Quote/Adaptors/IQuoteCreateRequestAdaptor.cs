namespace Web.Template.Application.Interfaces.Quote.Adaptors
{
    using iVectorConnectInterface.Basket;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Defines a class responsible for adapting a basket model to a quote create request
    /// </summary>
    public interface IQuoteCreateRequestAdaptor
    {
        /// <summary>
        /// Gets the type of the component.
        /// </summary>
        /// <value>The type of the component.</value>
        ComponentType ComponentType { get; }

        /// <summary>
        /// Creates the specified basket component.
        /// </summary>
        /// <param name="basketComponent">The basket component.</param>
        /// <param name="connectRequestBody">The connect request body.</param>
        void Create(IBasketComponent basketComponent, QuoteRequest connectRequestBody);
    }
}
