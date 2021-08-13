namespace Web.Template.Application.Results.ResultModels
{
    using System.Collections.Generic;

    using AutoMapper;

    using Web.Template.Application.Basket.Models;
    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Class representing a single transfer result
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Models.IResult" />
    public class TransferResult : IResult
    {
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferResult"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        public TransferResult(IMapper mapper)
        {
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets or sets the arrival parent identifier.
        /// </summary>
        /// <value>
        /// The arrival parent identifier.
        /// </value>
        public int ArrivalParentId { get; set; }

        /// <summary>
        /// Gets or sets the type of the arrival parent.
        /// </summary>
        /// <value>
        /// The type of the arrival parent.
        /// </value>
        public string ArrivalParentType { get; set; }

        /// <summary>
        /// Gets or sets the booking token.
        /// </summary>
        /// <value>
        /// The booking token, the unique identifier connect users for the result
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
        /// Gets or sets the departure identifier.
        /// </summary>
        /// <value>
        /// The departure identifier.
        /// </value>
        public int DepartureId { get; set; }

        /// <summary>
        /// Gets or sets the type of the departure parent.
        /// </summary>
        /// <value>
        /// The type of the departure parent.
        /// </value>
        public string DepartureParentType { get; set; }

        /// <summary>
        /// Gets or sets the maximum capacity.
        /// </summary>
        /// <value>
        /// The maximum capacity.
        /// </value>
        public int MaximumCapacity { get; set; }

        /// <summary>
        /// Gets or sets the minimum capacity.
        /// </summary>
        /// <value>
        /// The minimum capacity.
        /// </value>
        public int MinimumCapacity { get; set; }

        /// <summary>
        /// Gets or sets the outbound journey time.
        /// </summary>
        /// <value>
        /// The outbound journey time.
        /// </value>
        public string OutboundJourneyTime { get; set; }

        /// <summary>
        /// Gets or sets the return journey time.
        /// </summary>
        /// <value>
        /// The return journey time.
        /// </value>
        public string ReturnJourneyTime { get; set; }

        /// <summary>
        /// Gets or sets the search mode.
        /// </summary>
        /// <value>
        /// The search mode.
        /// </value>
        public SearchMode SearchMode { get; set; }

        /// <summary>
        /// Gets or sets the sub results.
        /// </summary>
        /// <value>
        /// The sub results.
        /// </value>
        public List<ISubResult> SubResults { get; set; }

        /// <summary>
        /// Gets or sets the supplier identifier.
        /// </summary>
        /// <value>
        /// The supplier identifier.
        /// </value>
        public int SupplierId { get; set; }

        /// <summary>
        /// Gets or sets the total price.
        /// </summary>
        /// <value>
        /// The total price.
        /// </value>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Gets or sets the vehicle.
        /// </summary>
        /// <value>
        /// The vehicle.
        /// </value>
        public string Vehicle { get; set; }

        /// <summary>
        /// Gets or sets the vehicle quantity.
        /// </summary>
        /// <value>
        /// The vehicle quantity.
        /// </value>
        public int VehicleQuantity { get; set; }

        /// <summary>
        /// Creates the basket component.
        /// </summary>
        /// <returns>The Basket component.</returns>
        public IBasketComponent CreateBasketComponent()
        {
            var basketComponent = this.mapper.Map<IResult, Transfer>(this);
            basketComponent.OutboundJourneyDetails = new TransferJourneyDetails() { JourneyTime = this.OutboundJourneyTime };
            basketComponent.ReturnJourneyDetails = new TransferJourneyDetails() { JourneyTime = this.ReturnJourneyTime };
            return basketComponent;
        }
    }
}