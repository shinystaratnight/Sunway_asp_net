namespace Web.Template.Application.SocialMedia
{
    using Web.Template.Application.Interfaces.SocialMedia;
    using Newtonsoft.Json;

    /// <summary>
    /// Class TwitterConfiguration.
    /// </summary>
    public class TwitterConfiguration : ITwitterConfiguration
    {
        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>The access token.</value>
        [JsonIgnore]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the access token secret.
        /// </summary>
        /// <value>The access token secret.</value>
        [JsonIgnore]
        public string AccessTokenSecret { get; set; }

        /// <summary>
        /// Gets or sets the consumer key.
        /// </summary>
        /// <value>The consumer key.</value>
        [JsonIgnore]
        public string ConsumerKey { get; set; }

        /// <summary>
        /// Gets or sets the consumer secret.
        /// </summary>
        /// <value>The consumer secret.</value>
        [JsonIgnore]
        public string ConsumerSecret { get; set; }

        /// <summary>
        /// Gets or sets the user handle.
        /// </summary>
        /// <value>The user handle.</value>
        public string TwitterHandle { get; set; }
    }
}