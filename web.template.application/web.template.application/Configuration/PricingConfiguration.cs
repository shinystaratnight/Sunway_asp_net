namespace Web.Template.Application.Configuration
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Configuration;

    /// <summary>
    /// A class responsible for defining how pricing is done on the site.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Configuration.IPricingConfiguration" />
    public class PricingConfiguration : IPricingConfiguration
    {
        /// <summary>
        /// Gets or sets the flight booking adjustment types.
        /// </summary>
        /// <value>
        /// The flight booking adjustment types.
        /// </value>
        public List<string> FlightBookingAdjustmentTypes { get; set; }

        /// <summary>
        /// Gets or sets the hotel booking adjustment types.
        /// </summary>
        /// <value>
        /// The hotel booking adjustment types.
        /// </value>
        public List<string> HotelBookingAdjustmentTypes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [package price]. e.g. do we need opacity over the flight and hotel breakdown.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [package price]; otherwise, <c>false</c>.
        /// </value>
        public bool PackagePrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [per person pricing], 
        /// whether to display prices for two adults as 200 per person or 400.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [per person pricing]; otherwise, <c>false</c>.
        /// </value>
        public bool PerPersonPricing { get; set; }

        /// <summary>
        /// Gets or sets the price format. E.g. Whether to show 120 or 119.99
        /// </summary>
        /// <value>
        /// The price format.
        /// </value>
        [JsonIgnore]
        public PriceFormat PerPersonPriceFormat { get; set; }

        /// <summary>
        /// Gets the price format display.
        /// </summary>
        /// <value>
        /// The price format display.
        /// </value>
        public string PerPersonPriceFormatDisplay => this.PerPersonPriceFormat.ToString();

        /// <summary>
        /// Gets or sets the price format. E.g. Whether to show 120 or 119.99
        /// </summary>
        /// <value>
        /// The price format.
        /// </value>
        [JsonIgnore]
        public PriceFormat PriceFormat { get; set; }

        /// <summary>
        /// Gets the price format display.
        /// </summary>
        /// <value>
        /// The price format display.
        /// </value>
        public string PriceFormatDisplay => this.PriceFormat.ToString();

        /// <summary>
        /// Gets or sets a value indicating whether [show group separator].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show group separator]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowGroupSeparator { get; set; }
    }
}