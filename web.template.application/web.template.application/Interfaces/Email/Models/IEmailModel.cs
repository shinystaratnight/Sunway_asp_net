namespace Web.Template.Application.Interfaces.Email.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Email Model Interface
    /// </summary>
    public interface IEmailModel
    {
        /// <summary>
        /// Gets or sets the email body.
        /// </summary>
        /// <value>
        /// The email body.
        /// </value>
        Dictionary<string, string> EmailBody { get; set; }

        /// <summary>
        /// Gets or sets the email footer.
        /// </summary>
        /// <value>
        /// The email footer.
        /// </value>
        Dictionary<string, string> EmailFooter { get; set; }

        /// <summary>
        /// Gets or sets the email subject.
        /// </summary>
        /// <value>
        /// The email subject.
        /// </value>
        string EmailSubject { get; set; }

        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>
        /// From.
        /// </value>
        string From { get; set; }

        /// <summary>
        /// Gets or sets from email.
        /// </summary>
        /// <value>
        /// From email.
        /// </value>
        string FromEmail { get; set; }

        /// <summary>
        /// Gets or sets to email.
        /// </summary>
        /// <value>
        /// To email.
        /// </value>
        string ToEmail { get; set; }
    }
}