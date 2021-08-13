namespace Web.Template.Application.SiteBuilderService.Models
{
    /// <summary>
    /// Class AuthorizationModel.
    /// </summary>
    public class AuthorizationModel
    {
        /// <summary>
        /// Gets or sets the access_token.
        /// </summary>
        /// <value>The access_token.</value>
        public string access_token { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>The error.</value>
        public string error { get; set; }

        /// <summary>
        /// Gets or sets the error_description.
        /// </summary>
        /// <value>The error_description.</value>
        public string error_description { get; set; }

        /// <summary>
        /// Gets or sets the expires_in.
        /// </summary>
        /// <value>The expires_in.</value>
        public int expires_in { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string firstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string lastName { get; set; }

        /// <summary>
        /// Gets or sets the token_type.
        /// </summary>
        /// <value>The token_type.</value>
        public string token_type { get; set; }
    }
}