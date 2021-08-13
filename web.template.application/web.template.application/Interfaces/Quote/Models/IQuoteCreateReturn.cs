
namespace Web.Template.Application.Interfaces.Quote.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Defines the quote create return class
    /// </summary>
    public interface IQuoteCreateReturn
    {
        /// <summary>
        /// Gets or sets the basket.
        /// </summary>
        /// <value>The basket.</value>
        IBasket Basket { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IQuoteCreateReturn"/> is success.
        /// </summary>
        /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
        bool Success { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>The warnings.</value>
        List<string> Warnings { get; set; }
    }
}
