namespace Web.Template.Application.Interfaces.Booking.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a class that is returned as a result of a direct debit search
    /// </summary>
    public interface IDirectDebitRetrieveReturn
    {
        /// <summary>
        /// Gets or sets the booking line.
        /// </summary>
        /// <value>
        /// The booking line.
        /// </value>
        List<IBookingLineItem> BookingLine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [retrieve successful].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [retrieve successful]; otherwise, <c>false</c>.
        /// </value>
        bool RetrieveSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        List<string> Warnings { get; set; }
    }
}