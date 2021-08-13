namespace Web.Template.Application.Configuration
{
    using System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Configuration;

    /// <summary>
    /// Class SearchModeConfiguration.
    /// </summary>
    public class SearchModeConfiguration : ISearchModeConfiguration
    {
        /// <summary>
        /// Gets or sets the pages.
        /// </summary>
        /// <value>The pages.</value>
        public List<string> Pages { get; set; }

        /// <summary>
        /// Gets or sets the search mode.
        /// </summary>
        /// <value>The search mode.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public SearchMode SearchMode { get; set; }

        /// <summary>
        /// Gets or sets the upsell items.
        /// </summary>
        /// <value>The upsell items.</value>
        public List<UpsellType> UpsellItems { get; set; }
    }
}