namespace Web.Template.Application.Interfaces.Configuration
{
    using System.Collections.Generic;

    using Web.Template.Application.Enum;

    /// <summary>
    /// an interface responsible for defining how pricing is done on the site.
    /// </summary>
    public interface IPricingConfiguration
    {
        /// <summary>
        /// Gets or sets the flight booking adjustment types.
        /// </summary>
        /// <value>
        /// The flight booking adjustment types.
        /// </value>
        List<string> FlightBookingAdjustmentTypes { get; set; }

        /// <summary>
        /// Gets or sets the hotel booking adjustment types.
        /// </summary>
        /// <value>
        /// The hotel booking adjustment types.
        /// </value>
        List<string> HotelBookingAdjustmentTypes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [package price].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [package price]; otherwise, <c>false</c>.
        /// </value>
        bool PackagePrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [per person pricing].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [per person pricing]; otherwise, <c>false</c>.
        /// </value>
        bool PerPersonPricing { get; set; }

        /// <summary>
        /// Gets or sets the price format.
        /// </summary>
        /// <value>
        /// The price format.
        /// </value>
        PriceFormat PriceFormat { get; set; }

        /// <summary>
        /// Gets or sets the price format.
        /// </summary>
        /// <value>
        /// The price format.
        /// </value>
        PriceFormat PerPersonPriceFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show group separator].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show group separator]; otherwise, <c>false</c>.
        /// </value>
        bool ShowGroupSeparator { get; set; }
    }
}