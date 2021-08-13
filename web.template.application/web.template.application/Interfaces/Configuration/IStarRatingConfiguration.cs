namespace Web.Template.Application.Interfaces.Configuration
{
    using System.Collections.Generic;

    using Web.Template.Application.Configuration;

    /// <summary>
    /// Interface IStarRatingConfiguration
    /// </summary>
    public interface IStarRatingConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether [display half ratings].
        /// </summary>
        /// <value><c>true</c> if [display half ratings]; otherwise, <c>false</c>.</value>
        bool DisplayHalfRatings { get; set; }

        /// <summary>
        /// Gets or sets the append text.
        /// </summary>
        /// <value>The append text.</value>
        List<StarRatingConfiguration.AppendTextItem> AppendText { get; set; }
    }
}