namespace Web.Template.Application.Booking.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// The direct debit retrieve return object
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Booking.Models.IDirectDebitRetrieveReturn" />
    public class DirectDebitRetrieveReturn : IDirectDebitRetrieveReturn
    {
        /// <summary>
        /// Gets or sets the booking line.
        /// </summary>
        /// <value>
        /// The booking line.
        /// </value>
        public List<IBookingLineItem> BookingLine { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the module user identifier.
        /// </summary>
        /// <value>
        /// The module user identifier.
        /// </value>
        public int ModuleUserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow agent read only access].
        /// </summary>
        /// <value>
        /// <c>true</c> if [allow agent read only access]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowAgentReadOnlyAccess { get; set; }

        /// <summary>
        /// Gets or sets the payment due days ahead.
        /// </summary>
        /// <value>
        /// The payment due days ahead.
        /// </value>
        public int PaymentDueDaysAhead { get; set; }

        /// <summary>
        /// Gets or sets the payment due before date.
        /// </summary>
        /// <value>
        /// The payment due before date.
        /// </value>
        public string PaymentDueBeforeDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [ignore over payments].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [ignore over payments]; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreOverPayments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DirectDebitRetrieveReturn"/> is archived.
        /// </summary>
        /// <value>
        ///   <c>true</c> if archived; otherwise, <c>false</c>.
        /// </value>
        public bool Archived { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [retrieve successful].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [retrieve successful]; otherwise, <c>false</c>.
        /// </value>
        public bool RetrieveSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        public List<string> Warnings { get; set; }
    }
}