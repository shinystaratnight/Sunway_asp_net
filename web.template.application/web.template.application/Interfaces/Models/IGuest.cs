namespace Web.Template.Application.Interfaces.Models
{
    using System;

    /// <summary>
    /// Guest interfaces
    /// </summary>
    public interface IGuest
    {
        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        /// <value>
        /// The age.
        /// </value>
        int Age { get; set; }

        /// <summary>
        /// Gets or sets the booking passenger identifier.
        /// </summary>
        /// <value>
        /// The booking passenger identifier.
        /// </value>
        int BookingPassengerID { get; set; }

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        string Gender { get; set; }

        /// <summary>
        /// Gets or sets the guest identifier.
        /// </summary>
        /// <value>
        /// The guest identifier.
        /// </value>
        int GuestID { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        string LastName { get; set; }

        /// <summary>
        /// Gets or sets the name of the middle.
        /// </summary>
        /// <value>
        /// The name of the middle.
        /// </value>
        string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the nationality identifier.
        /// </summary>
        /// <value>
        /// The nationality identifier.
        /// </value>
        int NationalityId { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        string Type { get; set; }
    }
}