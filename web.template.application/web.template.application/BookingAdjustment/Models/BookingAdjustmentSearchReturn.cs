namespace Web.Template.Application.BookingAdjustment.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Web.Template.Application.Interfaces.BookingAdjustment;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Prebook;

    /// <summary>
    /// Class BookingAdjustmentSearchReturn.
    /// </summary>
    public class BookingAdjustmentSearchReturn : IBookingAdjustmentSearchReturn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BookingAdjustmentSearchReturn"/> class.
        /// </summary>
        public BookingAdjustmentSearchReturn()
        {
            this.BookingAdjustments = new List<IAdjustment>();
            this.ResultToken = this.SetupToken();
        }

        /// <summary>
        /// Gets or sets the booking adjustments.
        /// </summary>
        /// <value>The booking adjustments.</value>
        public List<IAdjustment> BookingAdjustments { get; set; }

        /// <summary>
        /// Gets or sets the result token.
        /// </summary>
        /// <value>The result token.</value>
        public string ResultToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IPrebookReturn" /> is success.
        /// </summary>
        /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>The warnings.</value>
        public List<string> Warnings { get; set; }

        /// <summary>
        /// Setups the token.
        /// </summary>
        /// <returns>A token unique to the search</returns>
        private string SetupToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            string token = Convert.ToBase64String(time.Concat(key).ToArray());
            return token;
        }
    }
}
