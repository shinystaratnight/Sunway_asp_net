namespace Web.Template.Application.Interfaces.Payment
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface IProcessThreeDSecureReturn
    /// </summary>
    public interface IProcessThreeDSecureReturn
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IProcessThreeDSecureReturn"/> is success.
        /// </summary>
        /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
        bool Success { get; set; }

        /// <summary>
        /// Gets or sets the payment token.
        /// </summary>
        /// <value>The payment token.</value>
        string PaymentToken { get; set; }

        /// <summary>
        /// Gets or sets the three d secure code.
        /// </summary>
        /// <value>The three d secure code.</value>
        string ThreeDSecureCode { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>The warnings.</value>
        List<string> Warnings { get; set; }
    }
}