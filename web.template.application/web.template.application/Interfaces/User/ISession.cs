namespace Web.Template.Application.Interfaces.User
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface ISession
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// Gets or sets the user session.
        /// </summary>
        /// <value>The user session.</value>
        IUserSession UserSession { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>The warnings.</value>
        List<string> Warnings { get; set; }
    }
}