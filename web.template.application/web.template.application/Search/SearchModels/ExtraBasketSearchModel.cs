namespace Web.Template.Application.Search.SearchModels
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Class ExtraBasketSearchModel.
    /// </summary>
    public class ExtraBasketSearchModel : IExtraBasketSearchModel
    {
        /// <summary>
        /// Gets or sets the basket token.
        /// </summary>
        /// <value>The basket token.</value>
        public string BasketToken { get; set; }

        /// <summary>
        /// Gets or sets the extra identifier.
        /// </summary>
        /// <value>The extra identifier.</value>
        public int ExtraId { get; set; }

        /// <summary>
        /// Gets or sets the extra group identifier.
        /// </summary>
        /// <value>The extra group identifier.</value>
        public int ExtraGroupId { get; set; }

        /// <summary>
        /// Gets or sets the extra types.
        /// </summary>
        /// <value>The extra types.</value>
        public List<int> ExtraTypes { get; set; }
    }
}
