namespace Web.Template.Application.PromoCode
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Promocode;

    /// <summary>
    /// Model returned from adding a promotional code
    /// </summary>
    public class PromoCodeReturn : IPromoCodeReturn
    {
        /// <summary>
        /// Gets or sets the basket.
        /// </summary>
        /// <value>
        /// The basket.
        /// </value>
        public IBasket Basket { get; set; }

        /// <summary>
        /// Gets or sets the discount.
        /// </summary>
        /// <value>
        /// The discount.
        /// </value>
        public decimal Discount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PromoCodeReturn"/> is successful.
        /// </summary>
        /// <value>
        ///   <c>true</c> if successful; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; } = true;

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        public List<string> Warnings { get; set; } = new List<string>();
    }
}