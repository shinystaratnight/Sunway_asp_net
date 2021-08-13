namespace Web.Template.Domain.Entities.SocialMedia
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Tweet entity representing a tweet from twitter
    /// </summary>
    public class Tweet
    {
        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>The created at.</value>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the images.
        /// </summary>
        /// <value>The images.</value>
        public List<string> Images { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url { get; set; }
    }
}