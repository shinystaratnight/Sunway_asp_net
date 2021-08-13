namespace Web.Template.Application.Interfaces.Payment
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface IThreeDSecureRedirectReturn
    /// </summary>
    public interface IThreeDSecureRedirectReturn
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IThreeDSecureRedirectReturn"/> is success.
        /// </summary>
        /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
        bool Success { get; set; }

        /// <summary>
        /// Gets or sets the HTML.
        /// </summary>
        /// <value>The HTML.</value>
        string Html { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>The warnings.</value>
        List<string> Warnings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IThreeDSecureRedirectReturn"/> is enrollment.
        /// </summary>
        /// <value><c>true</c> if enrollment; otherwise, <c>false</c>.</value>
        bool Enrollment { get; set; }

        /// <summary>
        /// Gets or sets the payment token.
        /// </summary>
        /// <value>The payment token.</value>
        string PaymentToken { get; set; }
    }
}