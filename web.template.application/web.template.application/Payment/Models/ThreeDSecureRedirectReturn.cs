namespace Web.Template.Application.Payment.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Payment;

    /// <summary>
    /// Class ThreeDSecureRedirectReturn.
    /// </summary>
    public class ThreeDSecureRedirectReturn : IThreeDSecureRedirectReturn
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ThreeDSecureRedirectReturn"/> is success.
        /// </summary>
        /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the HTML.
        /// </summary>
        /// <value>The HTML.</value>
        public string Html { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>The warnings.</value>
        public List<string> Warnings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ThreeDSecureRedirectReturn"/> is enrollment.
        /// </summary>
        /// <value><c>true</c> if enrollment; otherwise, <c>false</c>.</value>
        public bool Enrollment { get; set; }

        /// <summary>
        /// Gets or sets the payment token.
        /// </summary>
        /// <value>The payment token.</value>
        public string PaymentToken { get; set; }
    }
}
