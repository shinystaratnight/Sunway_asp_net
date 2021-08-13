namespace Web.Template.Application.Interfaces.Book
{
    using iVectorConnectInterface.Basket;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    ///     Interface For classes that want to take a search model and produce a Connect property search request
    /// </summary>
    public interface IBookRequestAdaptor
    {
        /// <summary>
        ///     Gets the type of the request.
        /// </summary>
        /// <value>
        ///     The type of the request.
        /// </value>
        ComponentType ComponentType { get; }

        /// <summary>
        /// Creates a search connect search request using a WebTemplate search model.
        /// </summary>
        /// <param name="basketComponent">The basket component.</param>
        /// <param name="connectRequestBody">The connect request body.</param>
        void Create(IBasketComponent basketComponent, BookRequest connectRequestBody);
    }
}