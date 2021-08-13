namespace Web.Template.Application.Payment.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Payment;

    /// <summary>
    /// Class ProcessThreeDSecureReturn.
    /// </summary>
    /// <seealso cref="IProcessThreeDSecureReturn" />
    public class ProcessThreeDSecureReturn : IProcessThreeDSecureReturn
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ProcessThreeDSecureReturn"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the payment token.
        /// </summary>
        /// <value>
        /// The payment token.
        /// </value>
        public string PaymentToken { get; set; }

        /// <summary>
        /// Gets or sets the three d secure code.
        /// </summary>
        /// <value>The three d secure code.</value>
        public string ThreeDSecureCode { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        public List<string> Warnings { get; set; }
    }
}
