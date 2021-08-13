namespace Web.Template.Application.BookingAdjustment.Models
{
    using System;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.BookingAdjustment;

    /// <summary>
    /// Class BookingAdjustmentSearchModel.
    /// </summary>
    public class BookingAdjustmentSearchModel : IBookingAdjustmentSearchModel
    {
        /// <summary>
        /// Gets or sets the booking date.
        /// </summary>
        /// <value>The booking date.</value>
        public DateTime BookingDate { get; set; }

        /// <summary>
        /// Gets or sets the brand identifier.
        /// </summary>
        /// <value>The brand identifier.</value>
        public int BrandId { get; set; }

        /// <summary>
        /// Gets or sets the customer currency identifier.
        /// </summary>
        /// <value>The customer currency identifier.</value>
        public int CustomerCurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the first departure date.
        /// </summary>
        /// <value>The first departure date.</value>
        public DateTime FirstDepartureDate { get; set; }

        /// <summary>
        /// Gets or sets the flight carrier identifier.
        /// </summary>
        /// <value>The flight carrier identifier.</value>
        public int FlightCarrierId { get; set; }

        /// <summary>
        /// Gets or sets the type of the flight carrier.
        /// </summary>
        /// <value>The type of the flight carrier.</value>
        public string FlightCarrierType { get; set; }

        /// <summary>
        /// Gets or sets the flight price.
        /// </summary>
        /// <value>
        /// The flight price.
        /// </value>
        public decimal FlightPrice { get; set; }

        /// <summary>
        /// Gets or sets the flight supplier identifier.
        /// </summary>
        /// <value>The flight supplier identifier.</value>
        public int FlightSupplierId { get; set; }

        /// <summary>
        /// Gets or sets the geography level3 identifier.
        /// </summary>
        /// <value>The geography level3 identifier.</value>
        public int GeographyLevel3Id { get; set; }

        /// <summary>
        /// Gets or sets the property price.
        /// </summary>
        /// <value>
        /// The property price.
        /// </value>
        public decimal PropertyPrice { get; set; }

        /// <summary>
        /// Gets or sets the sales channel identifier.
        /// </summary>
        /// <value>The sales channel identifier.</value>
        public int SalesChannelId { get; set; }

        /// <summary>
        /// Gets or sets the search mode.
        /// </summary>
        /// <value>
        /// The search mode.
        /// </value>
        public SearchMode SearchMode { get; set; }

        /// <summary>
        /// Gets or sets the selling country identifier.
        /// </summary>
        /// <value>The selling country identifier.</value>
        public int SellingCountryId { get; set; }

        /// <summary>
        /// Gets or sets the selling exchange rate.
        /// </summary>
        /// <value>The selling exchange rate.</value>
        public decimal SellingExchangeRate { get; set; }

        /// <summary>
        /// Gets or sets the total passengers.
        /// </summary>
        /// <value>The total passengers.</value>
        public int TotalPassengers { get; set; }
    }
}
