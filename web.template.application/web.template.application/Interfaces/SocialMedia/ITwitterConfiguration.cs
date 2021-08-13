/// <summary>
/// 
/// </summary>
namespace Web.Template.Application.Interfaces.SocialMedia

    

{using Newtonsoft.Json;
    /// <summary>
    /// Interface ITwitterConfiguration
    /// </summary>
    public interface ITwitterConfiguration
    {
        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>The access token.</value>
        [JsonIgnore]
        string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the access token secret.
        /// </summary>
        /// <value>The access token secret.</value>
        [JsonIgnore]
        string AccessTokenSecret { get; set; }

        /// <summary>
        /// Gets or sets the consumer key.
        /// </summary>
        /// <value>The consumer key.</value>
        [JsonIgnore]
        string ConsumerKey { get; set; }

        /// <summary>
        /// Gets or sets the consumer secret.
        /// </summary>
        /// <value>The consumer secret.</value>
        [JsonIgnore]
        string ConsumerSecret { get; set; }

        /// <summary>
        /// Gets or sets the twitter handle.
        /// </summary>
        /// <value>The twitter handle.</value>
        string TwitterHandle { get; set; }
    }
}