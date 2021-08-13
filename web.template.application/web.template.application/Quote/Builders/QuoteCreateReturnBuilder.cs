namespace Web.Template.Application.Quote.Builders
{
    using System.Collections.Generic;

    using iVectorConnectInterface.Basket;

    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Quote;
    using Web.Template.Application.Interfaces.Quote.Builders;
    using Web.Template.Application.Interfaces.Quote.Models;

    /// <summary>
    /// Class QuoteCreateReturnBuilder.
    /// </summary>
    public class QuoteCreateReturnBuilder : IQuoteCreateReturnBuilder
    {
        /// <summary>
        /// The quote create return
        /// </summary>
        private readonly IQuoteCreateReturn quoteCreateReturn;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteCreateReturnBuilder"/> class.
        /// </summary>
        /// <param name="quoteCreateReturn">The quote create return.</param>
        public QuoteCreateReturnBuilder(IQuoteCreateReturn quoteCreateReturn)
        {
            this.quoteCreateReturn = quoteCreateReturn;
            this.quoteCreateReturn.Warnings = new List<string>();
            this.quoteCreateReturn.Success = true;
        }

        /// <summary>
        /// Gets a value indicating whether [currently successful].
        /// </summary>
        /// <value><c>true</c> if [currently successful]; otherwise, <c>false</c>.</value>
        public bool CurrentlySuccessful => this.quoteCreateReturn.Success;

        /// <summary>
        /// Adds the warning.
        /// </summary>
        /// <param name="warning">The warning.</param>
        public void AddWarning(string warning)
        {
            this.quoteCreateReturn.Warnings.Add(warning);
            this.quoteCreateReturn.Success = false;
        }

        /// <summary>
        /// Adds the warnings.
        /// </summary>
        /// <param name="warnings">The warnings.</param>
        public void AddWarnings(List<string> warnings)
        {
            this.quoteCreateReturn.Warnings.AddRange(warnings);
            this.quoteCreateReturn.Success = this.quoteCreateReturn.Warnings.Count == 0;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>The quote create return.</returns>
        public IQuoteCreateReturn Build()
        {
            return this.quoteCreateReturn;
        }

        /// <summary>
        /// Sets the basket.
        /// </summary>
        /// <param name="basket">The basket.</param>
        public void SetBasket(IBasket basket)
        {
            this.quoteCreateReturn.Basket = basket;
        }

        /// <summary>
        /// Sets to failure.
        /// </summary>
        public void SetToFailure()
        {
            this.quoteCreateReturn.Success = false;
        }
    }
}
