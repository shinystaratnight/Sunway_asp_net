namespace Web.Template.Application.Interfaces.Configuration
{
    using System.Collections.Generic;

    using Web.Template.Application.Enum;

    /// <summary>
    /// Interface ISearchModeConfiguration
    /// </summary>
    public interface ISearchModeConfiguration
    {
        /// <summary>
        /// Gets or sets the pages.
        /// </summary>
        /// <value>The pages.</value>
        List<string> Pages { get; set; }

        /// <summary>
        /// Gets or sets the search mode.
        /// </summary>
        /// <value>The search mode.</value>
        SearchMode SearchMode { get; set; }

        /// <summary>
        /// Gets or sets the upsell items.
        /// </summary>
        /// <value>The upsell items.</value>
        List<UpsellType> UpsellItems { get; set; }
    }
}