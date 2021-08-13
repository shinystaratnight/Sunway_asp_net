namespace Web.Template.Application.Interfaces.Configuration
{
    using System.Collections.Generic;

    using Web.Template.Application.Enum;

    /// <summary>
    ///  Class responsible for configuring searching.
    /// </summary>
    public interface ISearchConfiguration
    {
        /// <summary>
        /// Gets or sets the failed search URL.
        /// </summary>
        /// <value>The failed search URL.</value>
        string FailedSearchUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [package search].
        /// </summary>
        /// <value><c>true</c> if [package search]; otherwise, <c>false</c>.</value>
        bool PackageSearch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [priority property].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [priority property]; otherwise, <c>false</c>.
        /// </value>
        bool PriorityProperty { get; set; }

        /// <summary>
        /// Gets or sets the search book ahead days.
        /// </summary>
        /// <value>
        /// The search book ahead days.
        /// </value>
        int SearchBookAheadDays { get; set; }

        /// <summary>
        /// Gets or sets the search expiry.
        /// </summary>
        /// <value>
        /// The search expiry.
        /// </value>
        int SearchExpiry { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [search booking adjustments].
        /// </summary>
        /// <value><c>true</c> if [search booking adjustments]; otherwise, <c>false</c>.</value>
        bool SearchBookingAdjustments { get; set; }

        /// <summary>
        /// Gets or sets the search modes.
        /// </summary>
        /// <value>
        /// The search modes.
        /// </value>
        List<SearchMode> SearchModes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use the deal finder flight cache.
        /// </summary>
        /// <value><c>true</c> if using the deal finder flight cache; otherwise, <c>false</c>.</value>
        bool UseDealFinder { get; set; }
    }
}