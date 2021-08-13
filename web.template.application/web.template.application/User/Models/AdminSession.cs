namespace Web.Template.Application.User.Models
{
    /// <summary>
    /// Class AdminSession.
    /// </summary>
    public class AdminSession : IAdminSession
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is logged in.
        /// </summary>
        /// <value><c>true</c> if this instance is logged in; otherwise, <c>false</c>.</value>
        public bool LoggedIn { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName { get; set; }
    }
}
