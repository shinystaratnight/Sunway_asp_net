namespace Web.Template.Application.Configuration
{
    using System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Configuration;

    /// <summary>
    /// Class responsible for configuring searching.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Configuration.ISearchConfiguration" />
    public class SearchConfiguration : ISearchConfiguration
    {
        /// <summary>
        /// Gets or sets the failed search URL.
        /// </summary>
        /// <value>The failed search URL.</value>
        public string FailedSearchUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [package search].
        /// </summary>
        /// <value><c>true</c> if [package search]; otherwise, <c>false</c>.</value>
        public bool PackageSearch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to [priority property] search.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [priority property]; otherwise, <c>false</c>.
        /// </value>
        public bool PriorityProperty { get; set; }

        /// <summary>
        /// Gets or sets the search book ahead days.
        /// </summary>
        /// <value>
        /// The search book ahead days.
        /// </value>
        public int SearchBookAheadDays { get; set; }

        /// <summary>
        /// Gets or sets the search expiry.
        /// </summary>
        /// <value>
        /// The search expiry.
        /// </value>
        public int SearchExpiry { get; set; }

        /// <summary>
        /// Gets or sets the search modes.
        /// </summary>
        /// <value>
        /// The search modes.
        /// </value>
        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public List<SearchMode> SearchModes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [search booking adjustments].
        /// </summary>
        /// <value><c>true</c> if [search booking adjustments]; otherwise, <c>false</c>.</value>
        public bool SearchBookingAdjustments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use the deal finder flight cache.
        /// </summary>
        /// <value><c>true</c> if using the deal finder flight cache; otherwise, <c>false</c>.</value>
        public bool UseDealFinder { get; set; }
    }
}