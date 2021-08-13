namespace Web.Template.Application.Email.Services
{
    using System.Collections.Generic;
    using System.Text;

    using Intuitive.Net;

    using Tweetinvi.Streaming.Parameters;

    using Web.Template.Application.Email.Models;
    using Web.Template.Application.Interfaces.Email.Models;
    using Web.Template.Application.Interfaces.Email.Services;

    /// <summary>
    /// Service that process email sending
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Email.Services.IEmailService" />
    public class EmailService : IEmailService
    {
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="emailModel">The email model.</param>
        /// <returns>Returns true if the email is sent successfully</returns>
        public bool SendEmail(IEmailModel emailModel)
        {
            Intuitive.Email email = this.ProcessEmail(emailModel);
            bool success = email.SendEmail(true);
            return success;
        }

        /// <summary>
        /// Processes the email.
        /// </summary>
        /// <param name="emailModel">The email model.</param>
        /// <returns>Returns the processed email</returns>
        public Intuitive.Email ProcessEmail(IEmailModel emailModel)
        {
            StringBuilder body = this.ProcessBody(emailModel);

            var email = new Intuitive.Email(true)
            {
                EmailTo = emailModel.ToEmail,
                FromEmail = emailModel.FromEmail,
                From = emailModel.From,
                Subject = emailModel.EmailSubject,
                Body = body.ToString()
            };
            return email;
        }

        /// <summary>
        /// Processes the body.
        /// </summary>
        /// <param name="emailModel">The email model.</param>
        /// <returns>Returns the processed email body</returns>
        private StringBuilder ProcessBody(IEmailModel emailModel)
        {
            var body = new StringBuilder();

            body.AppendLine("<html><body>");
            body.AppendLine("<table>");
            foreach (KeyValuePair<string, string> pair in emailModel.EmailBody)
            {
                body.AppendLine("<tr>");
                if (!string.IsNullOrEmpty(pair.Key))
                {
                    body.AppendLine($"<td>{pair.Key} :</td><td>{pair.Value}</td>");
                }
                else
                {
                    body.AppendLine($"<td colspan=\"2\">{pair.Value}</td>");
                }
                body.AppendLine("</tr>");
            }
            body.AppendLine("</table>");

            body.AppendLine("<table>");
            foreach (KeyValuePair<string, string> pair in emailModel.EmailFooter)
            {
                body.AppendLine("<tr>");
                if (!string.IsNullOrEmpty(pair.Key))
                {
                    body.AppendLine($"<td>{pair.Key} :</td><td>{pair.Value}</td>");
                }
                else
                {
                    body.AppendLine($"<td colspan=\"2\">{pair.Value}</td>");
                }
                body.AppendLine("</tr>");
            }
            body.AppendLine("</table>");
            body.AppendLine("</body></html>");
            return body;
        }
    }
}