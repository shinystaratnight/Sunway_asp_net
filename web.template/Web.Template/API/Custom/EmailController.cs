namespace Web.Template.API.Custom
{
    using System.Collections.Generic;
    using System.Text;
    using System.Web.Http;

    using Web.Template.API.Content;
    using Web.Template.Models.Application;

    /// <summary>
    ///     API to send simple emails
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class EmailController : ApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailController"/> class.
        /// </summary>
        public EmailController()
        {
        }

        /// <summary>
        /// Gets the page by URL.
        /// </summary>
        /// <param name="emailModel">The email model.</param>
        [Route("api/email/plaintext/Send")]
        [HttpPost]
        public void GetPageByURL([FromBody] EmailModel emailModel)
        {
            var sb = new StringBuilder();
            sb.Append("<html><body>");
            sb.AppendLine();
            sb.Append("<table>");
            sb.AppendLine();
            AppendKeyValueItems(emailModel, sb);
            sb.Append("</table>");
            sb.AppendLine();
            sb.AppendLine();
            sb.Append("</body></html>");
            sb.AppendLine();

            var email = new Intuitive.Email(true) { EmailTo = emailModel.ToEmail, FromEmail = "admin@intuitivesystems.co.uk", From = "admin", Subject = emailModel.EmailSubject, Body = sb.ToString() };

            email.SendEmail(true);
        }

        /// <summary>
        /// Appends the key value items.
        /// </summary>
        /// <param name="emailModel">The email model.</param>
        /// <param name="sb">The string builder.</param>
        private static void AppendKeyValueItems(EmailModel emailModel, StringBuilder sb)
        {
            foreach (KeyValuePair<string, string> keyValuePair in emailModel.EmailBody)
            {
                sb.Append("<tr>");
                sb.AppendLine();
                sb.Append("<td>");
                sb.AppendLine();
                sb.AppendLine($"{keyValuePair.Key} : {keyValuePair.Value}");
                sb.AppendLine();
                sb.Append("</td>");
                sb.AppendLine();
                sb.Append("</tr>");
                sb.AppendLine();
            }
        }
    }
}