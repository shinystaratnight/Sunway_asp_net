namespace Web.Template.Application.Interfaces.Promocode
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Model returned from adding a promotional code
    /// </summary>
    public interface IPromoCodeReturn
    {
        /// <summary>
        /// Gets or sets the basket.
        /// </summary>
        /// <value>
        /// The basket.
        /// </value>
        IBasket Basket { get; set; }

        /// <summary>
        /// Gets or sets the discount.
        /// </summary>
        /// <value>
        /// The discount.
        /// </value>
        decimal Discount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IPromoCodeReturn"/> is successful.
        /// </summary>
        /// <value>
        ///   <c>true</c> if successful; otherwise, <c>false</c>.
        /// </value>
        bool Success { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        List<string> Warnings { get; set; }
    }
}