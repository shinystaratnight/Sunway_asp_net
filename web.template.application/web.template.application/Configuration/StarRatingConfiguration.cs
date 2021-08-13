namespace Web.Template.Application.Configuration
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Configuration;

    /// <summary>
    /// Class StarRatingConfiguration.
    /// </summary>
    public class StarRatingConfiguration : IStarRatingConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether [display half ratings].
        /// </summary>
        /// <value><c>true</c> if [display half ratings]; otherwise, <c>false</c>.</value>
        public bool DisplayHalfRatings { get; set; }

        /// <summary>
        /// Gets or sets the append text.
        /// </summary>
        /// <value>The append text.</value>
        public List<AppendTextItem> AppendText { get; set; }

        /// <summary>
        /// Class AppendTextItem.
        /// </summary>
        public class AppendTextItem
        {
            /// <summary>
            /// Gets or sets the rating.
            /// </summary>
            /// <value>The rating.</value>
            public decimal Rating { get; set; }

            /// <summary>
            /// Gets or sets the text.
            /// </summary>
            /// <value>The text.</value>
            public string Text { get; set; }
        }
    }
}
