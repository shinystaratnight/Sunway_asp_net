namespace Web.Template.Application.Interfaces.Email.Services
{
    using Web.Template.Application.Interfaces.Email.Models;

    /// <summary>
    /// Email Service Interface
    /// </summary>
    public interface IEmailService
    {
        bool SendEmail(IEmailModel emailModel);
    }
}