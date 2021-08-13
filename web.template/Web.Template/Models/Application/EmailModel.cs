namespace Web.Template.Models.Application
{
    using System.Collections.Generic;

    /// <summary>
    /// Class used to pass to email web controller to send emails
    /// </summary>
    public class EmailModel
    {
        /// <summary>
        /// Gets or sets to email.
        /// </summary>
        /// <value>
        /// To email.
        /// </value>
        public string ToEmail { get; set; }

        /// <summary>
        /// Gets or sets the email subject.
        /// </summary>
        /// <value>
        /// The email subject.
        /// </value>
        public string EmailSubject { get; set; }

        /// <summary>
        /// Gets or sets the email body.
        /// </summary>
        /// <value>
        /// The email body.
        /// </value>
        public Dictionary<string, string> EmailBody { get; set; }
    }
}