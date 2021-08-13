namespace Web.Template.Application.User.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.User;

    /// <summary>
    /// Class Session.
    /// </summary>
    public class Session : ISession
    {
        /// <summary>
        /// Gets or sets the user session.
        /// </summary>
        /// <value>The user session.</value>
        public IUserSession UserSession { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>The warnings.</value>
        public List<string> Warnings { get; set; }
    }
}