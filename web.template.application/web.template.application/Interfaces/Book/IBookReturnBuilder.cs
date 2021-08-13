namespace Web.Template.Application.Interfaces.Book
{
    using System.Collections.Generic;

    using iVectorConnectInterface.Basket;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Interface defining the builder class for prebook returns
    /// </summary>
    public interface IBookReturnBuilder
    {
        /// <summary>
        /// Gets a value indicating whether [currently successful].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [currently successful]; otherwise, <c>false</c>.
        /// </value>
        bool CurrentlySuccessful { get; }

        /// <summary>
        /// Adds the response.
        /// </summary>
        /// <param name="preBookResponse">The pre book response.</param>
        void AddResponse(BookResponse preBookResponse);

        /// <summary>
        /// Adds the warning.
        /// </summary>
        /// <param name="warning">The warning.</param>
        void AddWarning(string warning);

        /// <summary>
        /// Adds the warnings.
        /// </summary>
        /// <param name="warnings">The warnings.</param>
        void AddWarnings(List<string> warnings);

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>A prebook return</returns>
        IBookReturn Build();

        /// <summary>
        /// Sets the basket.
        /// </summary>
        /// <param name="basket">The basket.</param>
        void SetBasket(IBasket basket);

        /// <summary>
        /// Sets to failure.
        /// </summary>
        void SetToFailure();
    }
}