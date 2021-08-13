namespace Web.Template.Application.Interfaces.Models
{
    using System;

    /// <summary>
    /// Lead guest interface
    /// </summary>
    public interface ILeadGuest
    {
        /// <summary>
        /// Gets or sets the address line1.
        /// </summary>
        /// <value>
        /// The address line1.
        /// </value>
        string AddressLine1 { get; set; }

        /// <summary>
        /// Gets or sets the address line2.
        /// </summary>
        /// <value>
        /// The address line2.
        /// </value>
        string AddressLine2 { get; set; }

        /// <summary>
        /// Gets or sets the booking country identifier.
        /// </summary>
        /// <value>
        /// The booking country identifier.
        /// </value>
        string BookingCountry { get; set; }

        /// <summary>
        /// Gets or sets the booking country identifier.
        /// </summary>
        /// <value>
        /// The booking country identifier.
        /// </value>
        int BookingCountryID { get; set; }

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        string Email { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        string LastName { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        /// <value>
        /// The phone.
        /// </value>
        string Phone { get; set; }

        /// <summary>
        /// Gets or sets the postcode.
        /// </summary>
        /// <value>
        /// The postcode.
        /// </value>
        string Postcode { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the town city.
        /// </summary>
        /// <value>
        /// The town city.
        /// </value>
        string TownCity { get; set; }
    }
}