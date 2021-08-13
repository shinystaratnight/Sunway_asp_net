namespace Web.Template.Models.Application
{
    using System;
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    ///     ResultsCollection returned from search
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        ///     Gets or sets the arrival date.
        /// </summary>
        /// <value>
        ///     The arrival date.
        /// </value>
        public DateTime ArrivalDate { get; set; }

        /// <summary>
        /// Gets or sets the component token.
        /// </summary>
        /// <value>The component token.</value>
        public int ComponentToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SearchResult"/> is display.
        /// </summary>
        /// <value><c>true</c> if display; otherwise, <c>false</c>.</value>
        public bool Display { get; set; }

        /// <summary>
        ///     Gets or sets the display name.
        /// </summary>
        /// <value>
        ///     The display name.
        /// </value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the display information.
        /// </summary>
        /// <value>The display information.</value>
        public Dictionary<string, object> MetaData { get; set; }

        /// <summary>
        ///     Gets or sets the price.
        /// </summary>
        /// <value>
        ///     The price.
        /// </value>
        public decimal Price { get; set; }

        /// <summary>
        ///     Gets or sets the return date.
        /// </summary>
        /// <value>
        ///     The return date.
        /// </value>
        public DateTime ReturnDate { get; set; }

        /// <summary>
        /// Gets or sets the sub results.
        /// </summary>
        /// <value>
        /// The sub results.
        /// </value>
        public List<ISubResult> SubResults { get; set; }
    }
}