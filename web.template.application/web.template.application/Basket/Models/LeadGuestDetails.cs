namespace Web.Template.Application.Basket.Models
{
    using System;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Lead guest details class
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Models.ILeadGuest" />
    public class LeadGuestDetails : ILeadGuest
    {
        /// <summary>
        /// Gets or sets the address line1.
        /// </summary>
        /// <value>
        /// The address line1.
        /// </value>
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Gets or sets the address line2.
        /// </summary>
        /// <value>
        /// The address line2.
        /// </value>
        public string AddressLine2 { get; set; }

        /// <summary>
        /// Gets or sets the booking country identifier.
        /// </summary>
        /// <value>
        /// The booking country identifier.
        /// </value>
        public string BookingCountry { get; set; }

        /// <summary>
        /// Gets or sets the booking country identifier.
        /// </summary>
        /// <value>
        /// The booking country identifier.
        /// </value>
        public int BookingCountryID { get; set; }

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        /// <value>
        /// The phone.
        /// </value>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the postcode.
        /// </summary>
        /// <value>
        /// The postcode.
        /// </value>
        public string Postcode { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the town city.
        /// </summary>
        /// <value>
        /// The town city.
        /// </value>
        public string TownCity { get; set; }
    }
}