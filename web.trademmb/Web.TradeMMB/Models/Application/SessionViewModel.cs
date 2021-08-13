namespace Web.TradeMMB.Models.Application
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.User;

    /// <summary>
    /// The session view model
    /// </summary>
    public class SessionViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SessionViewModel"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the user session.
        /// </summary>
        /// <value>
        /// The user session.
        /// </value>
        public IUserSession UserSession { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// List of warnings.
        /// </value>
        public List<string> Warnings { get; set; }
    }
}