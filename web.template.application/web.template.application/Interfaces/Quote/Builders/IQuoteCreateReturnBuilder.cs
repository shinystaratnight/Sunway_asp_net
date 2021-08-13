namespace Web.Template.Application.Interfaces.Quote.Builders
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Quote.Models;

    /// <summary>
    /// Interface IQuoteCreateReturnBuilder
    /// </summary>
    public interface IQuoteCreateReturnBuilder
    {
        /// <summary>
        /// Gets a value indicating whether [currently successful].
        /// </summary>
        /// <value><c>true</c> if [currently successful]; otherwise, <c>false</c>.</value>
        bool CurrentlySuccessful { get; }

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
        /// <returns>The quote create return.</returns>
        IQuoteCreateReturn Build();

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
