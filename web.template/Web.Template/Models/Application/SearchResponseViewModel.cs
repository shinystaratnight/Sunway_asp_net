namespace Web.Template.Models.Application
{
    using System.Collections.Generic;

    /// <summary>
    ///     The search results model
    /// </summary>
    public class SearchResponseViewModel
    {
        /// <summary>
        /// Gets or sets the result count.
        /// </summary>
        /// <value>The result count.</value>
        public Dictionary<string, int> ResultCounts { get; set; }

        /// <summary>
        /// Gets or sets the result token.
        /// </summary>
        /// <value>The result token.</value>
        public Dictionary<string, string> ResultTokens { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SearchResponseViewModel"/> is success.
        /// </summary>
        /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>The warnings.</value>
        public List<string> Warnings { get; set; }
    }
}