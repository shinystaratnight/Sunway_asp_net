namespace Web.Template.Application.Interfaces.BookingAdjustment
{
    using System;

    using Web.Template.Application.Enum;

    /// <summary>
    /// Interface IBookingAdjustmentSearchModel
    /// </summary>
    public interface IBookingAdjustmentSearchModel
    {
        /// <summary>
        /// Gets or sets the booking date.
        /// </summary>
        /// <value>The booking date.</value>
        DateTime BookingDate { get; set; }

        /// <summary>
        /// Gets or sets the brand identifier.
        /// </summary>
        /// <value>The brand identifier.</value>
        int BrandId { get; set; }

        /// <summary>
        /// Gets or sets the customer currency identifier.
        /// </summary>
        /// <value>The customer currency identifier.</value>
        int CustomerCurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the first departure date.
        /// </summary>
        /// <value>The first departure date.</value>
        DateTime FirstDepartureDate { get; set; }

        /// <summary>
        /// Gets or sets the flight carrier identifier.
        /// </summary>                                                     
        /// <value>The flight carrier identifier.</value>
        int FlightCarrierId { get; set; }

        /// <summary>
        /// Gets or sets the type of the flight carrier.
        /// </summary>
        /// <value>The type of the flight carrier.</value>
        string FlightCarrierType { get; set; }

        /// <summary>
        /// Gets or sets the flight price.
        /// </summary>
        /// <value>
        /// The flight price.
        /// </value>
        decimal FlightPrice { get; set; }

        /// <summary>
        /// Gets or sets the flight supplier identifier.
        /// </summary>
        /// <value>The flight supplier identifier.</value>
        int FlightSupplierId { get; set; }

        /// <summary>
        /// Gets or sets the geography level3 identifier.
        /// </summary>
        /// <value>The geography level3 identifier.</value>
        int GeographyLevel3Id { get; set; }

        /// <summary>
        /// Gets or sets the property price.
        /// </summary>
        /// <value>
        /// The property price.
        /// </value>
        decimal PropertyPrice { get; set; }

        /// <summary>
        /// Gets or sets the sales channel identifier.
        /// </summary>
        /// <value>The sales channel identifier.</value>
        int SalesChannelId { get; set; }

        /// <summary>
        /// Gets or sets the selling country identifier.
        /// </summary>
        /// <value>The selling country identifier.</value>
        int SellingCountryId { get; set; }

        /// <summary>
        /// Gets or sets the search mode.
        /// </summary>
        /// <value>
        /// The search mode.
        /// </value>
        SearchMode SearchMode { get; set; }

        /// <summary>
        /// Gets or sets the selling exchange rate.
        /// </summary>
        /// <value>The selling exchange rate.</value>
        decimal SellingExchangeRate { get; set; }

        /// <summary>
        /// Gets or sets the total passengers.
        /// </summary>
        /// <value>The total passengers.</value>
        int TotalPassengers { get; set; }
    }
}
