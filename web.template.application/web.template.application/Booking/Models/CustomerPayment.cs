namespace Web.Template.Application.Booking.Models
{
    using System;

    /// <summary>
    /// Class CustomerPayment.
    /// </summary>
    public class CustomerPayment
    {
        /// <summary>
        /// Gets or sets the date due.
        /// </summary>
        /// <value>The date due.</value>
        public DateTime DateDue { get; set; }

        /// <summary>
        /// Gets or sets the currency identifier.
        /// </summary>
        /// <value>The currency identifier.</value>
        public int CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the total payment.
        /// </summary>
        /// <value>The total payment.</value>
        public decimal TotalPayment { get; set; }

        /// <summary>
        /// Gets or sets the outstanding.
        /// </summary>
        /// <value>The outstanding.</value>
        public decimal Outstanding { get; set; }
    }
}
