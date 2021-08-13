namespace Web.Template.Application.Email.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Email.Models;

    /// <summary>
    /// Email Model
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Email.Models.IEmailModel" />
    public class EmailModel : IEmailModel
    {
        /// <summary>
        /// Gets or sets the email body.
        /// </summary>
        /// <value>
        /// The email body.
        /// </value>
        public Dictionary<string, string> EmailBody { get; set; }

        /// <summary>
        /// Gets or sets the email footer.
        /// </summary>
        /// <value>
        /// The email footer.
        /// </value>
        public Dictionary<string, string> EmailFooter { get; set; }

        /// <summary>
        /// Gets or sets the email subject.
        /// </summary>
        /// <value>
        /// The email subject.
        /// </value>
        public string EmailSubject { get; set; }

        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>
        /// From.
        /// </value>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets from email.
        /// </summary>
        /// <value>
        /// From email.
        /// </value>
        public string FromEmail { get; set; }

        /// <summary>
        /// Gets or sets to email.
        /// </summary>
        /// <value>
        /// To email.
        /// </value>
        public string ToEmail { get; set; }
    }
}