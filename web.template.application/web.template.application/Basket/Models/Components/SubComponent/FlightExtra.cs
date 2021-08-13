namespace Web.Template.Application.Basket.Models.Components.SubComponent
{
    using iVectorConnectInterface.Flight;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Class representing a flight extra e.g. a bag.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Models.ISubComponent" />
    public class FlightExtra : ISubComponent
    {
        /// <summary>
        /// Gets or sets the booking token.
        /// </summary>
        /// <value>
        /// The booking token.
        /// </value>
        public string BookingToken { get; set; }

        /// <summary>
        /// Gets or sets the component token.
        /// </summary>
        /// <value>
        /// The component token.
        /// </value>
        public int ComponentToken { get; set; }

        /// <summary>
        /// Gets or sets the costing basis.
        /// </summary>
        /// <value>
        /// The costing basis.
        /// </value>
        public string CostingBasis { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [default baggage].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [default baggage]; otherwise, <c>false</c>.
        /// </value>
        public bool DefaultBaggage { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the type of the extra.
        /// </summary>
        /// <value>
        /// The type of the extra.
        /// </value>
        public string ExtraType { get; set; }

        /// <summary>
        /// Gets or sets the guest identifier.
        /// </summary>
        /// <value>
        /// The guest identifier.
        /// </value>
        public int GuestID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FlightExtra"/> is mandatory.
        /// </summary>
        /// <value><c>true</c> if mandatory; otherwise, <c>false</c>.</value>
        public bool Mandatory { get; set; }

        /// <summary>
        /// Gets or sets the quantity available.
        /// </summary>
        /// <value>
        /// The quantity available.
        /// </value>
        public int QuantityAvailable { get; set; }

        /// <summary>
        /// Gets or sets the quantity selected.
        /// </summary>
        /// <value>
        /// The quantity selected.
        /// </value>
        public int QuantitySelected { get; set; }

        /// <summary>
        /// Gets or sets the total price.
        /// </summary>
        /// <value>
        /// The total price.
        /// </value>
        public decimal TotalPrice { get; set; }

        public bool ReturnMultiCarrierExtra { get; set; }

        /// <summary>
        /// Updates the details from connect.
        /// </summary>
        /// <param name="extra">The extra.</param>
        public void UpdateDetailsFromConnect(PreBookResponse.Extra extra, int totalPax)
        {
            this.BookingToken = extra.ExtraBookingToken;
            this.ComponentToken = extra.GetHashCode();
            this.TotalPrice = extra.Price;
            this.ExtraType = extra.ExtraType;
            this.Description = extra.Description;
            this.DefaultBaggage = extra.DefaultBaggage;
            if (extra.Description == "Automatic seat selection for entire party")
            {
                this.QuantityAvailable = totalPax*totalPax;
                this.QuantitySelected = totalPax;
                this.CostingBasis = "Per Passenger";
                this.TotalPrice = extra.Price / totalPax;
            }
            else
            {
                this.QuantityAvailable = extra.QuantityAvailable;
                this.QuantitySelected = extra.QuantitySelected;
                this.CostingBasis = extra.CostingBasis;
            }

            this.GuestID = extra.GuestID;
            this.Mandatory = extra.Mandatory;
        }
    }
}