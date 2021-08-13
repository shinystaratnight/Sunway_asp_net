namespace Web.TradeMMB.API.Content
{
    using System.Web.Http;

    using Web.Template.Application.Email.Models;
    using Web.Template.Application.Interfaces.Email.Services;

    /// <summary>
    /// The API to Email services
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class EmailController : ApiController
    {
        /// <summary>
        /// The email service
        /// </summary>
        private readonly IEmailService emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailController"/> class.
        /// </summary>
        /// <param name="emailService">The email service.</param>
        public EmailController(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        [Route("api/email/send")]
        [HttpPost]
        public bool SendEmail([FromBody] EmailModel email)
        {
            if (email != null)
            {
                return this.emailService.SendEmail(email);
            }

            return false;
        }
    }
}