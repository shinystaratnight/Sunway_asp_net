namespace Web.Template.Application.Interfaces.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Enum;

    /// <summary>
    /// Result interface
    /// </summary>
    public interface IResult
    {
        /// <summary>
        ///     Gets or sets the booking token.
        /// </summary>
        /// <value>
        ///     The booking token, the unique identifier connect uses for the result
        /// </value>
        string BookingToken { get; set; }

        /// <summary>
        /// Gets or sets the component token.
        /// </summary>
        /// <value>
        /// The component token.
        /// </value>
        int ComponentToken { get; set; }

        /// <summary>
        /// Gets or sets the search mode.
        /// </summary>
        /// <value>
        /// The search mode.
        /// </value>
        SearchMode SearchMode { get; set; }

        /// <summary>
        /// Gets or sets the sub results.
        /// </summary>
        /// <value>
        /// The sub results.
        /// </value>
        List<ISubResult> SubResults { get; set; }

        /// <summary>
        /// Gets or sets the total price.
        /// </summary>
        /// <value>
        /// The total price.
        /// </value>
        decimal TotalPrice { get; set; }

        /// <summary>
        /// Creates the basket component.
        /// </summary>
        /// <returns>The Basket Component.</returns>
        IBasketComponent CreateBasketComponent();
    }
}