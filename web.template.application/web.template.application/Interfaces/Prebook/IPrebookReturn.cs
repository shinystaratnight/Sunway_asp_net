namespace Web.Template.Application.Interfaces.Prebook
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Prebook.Models;

    /// <summary>
    /// Interface defining the prebook return model
    /// </summary>
    public interface IPrebookReturn
    {
        /// <summary>
        /// Gets or sets the basket.
        /// </summary>
        /// <value>
        /// The basket.
        /// </value>
        IBasket Basket { get; set; }

        /// <summary>
        /// Gets or sets the errata.
        /// </summary>
        /// <value>
        /// The errata.
        /// </value>
        List<Erratum> Errata { get; set; }

        /// <summary>
        /// Gets or sets the price change.
        /// </summary>
        /// <value>
        /// The price change.
        /// </value>
        decimal PriceChange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IPrebookReturn"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
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