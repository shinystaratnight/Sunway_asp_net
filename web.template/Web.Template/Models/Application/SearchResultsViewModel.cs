namespace Web.Template.Models.Application
{
    using System.Collections.Generic;

    /// <summary>
    /// view model for the search results.
    /// </summary>
    public class SearchResultsViewModel
    {
        /// <summary>
        /// Gets or sets the search results.
        /// </summary>
        /// <value>The search results.</value>
        public List<SearchResult> SearchResults { get; set; }
    }
}