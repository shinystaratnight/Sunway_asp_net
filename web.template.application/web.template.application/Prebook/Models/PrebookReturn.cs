namespace Web.Template.Application.Prebook.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Prebook;

    /// <summary>
    /// Model returned from the prebook to the UI.
    /// </summary>
    public class PrebookReturn : IPrebookReturn
    {
        /// <summary>
        /// Gets or sets the basket.
        /// </summary>
        /// <value>
        /// The basket.
        /// </value>
        public IBasket Basket { get; set; }

        /// <summary>
        /// Gets or sets the Errata.
        /// </summary>
        /// <value>
        /// The Errata.
        /// </value>
        public List<Erratum> Errata { get; set; }

        /// <summary>
        /// Gets or sets the price change.
        /// </summary>
        /// <value>
        /// The price change.
        /// </value>
        public decimal PriceChange { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PrebookReturn"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        public List<string> Warnings { get; set; }
    }
}