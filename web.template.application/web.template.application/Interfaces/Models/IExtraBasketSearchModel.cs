namespace Web.Template.Application.Interfaces.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface IExtraBasketSearchModel
    /// </summary>
    public interface IExtraBasketSearchModel
    {
        /// <summary>
        /// Gets or sets the basket token.
        /// </summary>
        /// <value>The basket token.</value>
        string BasketToken { get; set; }

        /// <summary>
        /// Gets or sets the extra identifier.
        /// </summary>
        /// <value>The extra identifier.</value>
        int ExtraId { get; set; }

        /// <summary>
        /// Gets or sets the extra group identifier.
        /// </summary>
        /// <value>The extra group identifier.</value>
        int ExtraGroupId { get; set; }

        /// <summary>
        /// Gets or sets the extra types.
        /// </summary>
        /// <value>The extra types.</value>
        List<int> ExtraTypes { get; set; }
    }
}