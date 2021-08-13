namespace Web.Template.Application.Basket.Models.Components
{
    using System.Collections.Generic;

    using AutoMapper;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Property;

    /// <summary>
    /// A Transfer Basket Component
    /// </summary>
    /// <seealso cref="BasketCompontentBase" />
    public class Transfer : BasketCompontentBase
    {
        /// <summary>
        /// The airport repository
        /// </summary>
        private readonly IAirportRepository airportRepository;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// The property reference repository
        /// </summary>
        private readonly IPropertyReferenceRepository propertyReferenceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="Transfer"/> class.
        /// </summary>
        public Transfer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transfer"/> class.
        /// </summary>
        /// <param name="airportRepository">The airport repository.</param>
        /// <param name="propertyReferenceRepository">The property reference repository.</param>
        /// <param name="mapper">The mapper.</param>
        public Transfer(IAirportRepository airportRepository, IPropertyReferenceRepository propertyReferenceRepository, IMapper mapper)
        {
            this.airportRepository = airportRepository;
            this.propertyReferenceRepository = propertyReferenceRepository;
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets or sets the arrival identifier.
        /// </summary>
        /// <value>
        /// The arrival identifier.
        /// </value>
        public int ArrivalParentId { get; set; }

        /// <summary>
        /// Gets or sets the name of the arrival parent.
        /// </summary>
        /// <value>
        /// The name of the arrival parent.
        /// </value>
        public string ArrivalParentName { get; set; }

        /// <summary>
        /// Gets or sets the type of the arrival parent.
        /// </summary>
        /// <value>
        /// The type of the arrival parent.
        /// </value>
        public string ArrivalParentType { get; set; }

        /// <summary>
        /// Gets or sets the type of the component.
        /// </summary>
        /// <value>
        /// The type of the component.
        /// </value>
        public override ComponentType ComponentType => ComponentType.Transfer;

        /// <summary>
        /// Gets or sets the departure identifier.
        /// </summary>
        /// <value>
        /// The departure identifier.
        /// </value>
        public int DepartureParentId { get; set; }

        /// <summary>
        /// Gets or sets the name of the departure parent.
        /// </summary>
        /// <value>
        /// The name of the departure parent.
        /// </value>
        public string DepartureParentName { get; set; }

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
        /// Gets or sets a value indicating whether [one way].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [one way]; otherwise, <c>false</c>.
        /// </value>
        public bool OneWay { get; set; }

        /// <summary>
        /// Gets or sets the outbound transfer details.
        /// </summary>
        /// <value>The outbound transfer details.</value>
        public TransferJourneyDetails OutboundJourneyDetails { get; set; }

        /// <summary>
        /// Gets or sets the return transfer details.
        /// </summary>
        /// <value>The return transfer details.</value>
        public TransferJourneyDetails ReturnJourneyDetails { get; set; }

        /// <summary>
        /// Gets or sets the supplier identifier.
        /// </summary>
        /// <value>
        /// The supplier identifier.
        /// </value>
        public int SupplierId { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        /// <value>
        /// The price.
        /// </value>
        public override decimal TotalPrice { get; set; }

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
        /// Gets the name of the parent type.
        /// </summary>
        /// <param name="parentType">Type of the parent.</param>
        /// <param name="parentId">The parent identifier.</param>
        /// <returns>the parent type name as a string</returns>
        public string GetParentTypeName(string parentType, int parentId)
        {
            string parentTypeName = string.Empty;

            if (parentType == "Airport")
            {
                parentTypeName = this.airportRepository.GetSingle(parentId).Name;
            }
            else if (parentType == "Property")
            {
                parentTypeName = this.propertyReferenceRepository.GetSingle(parentId).Name;
            }

            return parentTypeName;
        }

        /// <summary>
        /// Gets or sets the setup component search details.
        /// </summary>
        /// <param name="searchModel">The object the represents what we searched for.</param>
        /// <value>
        /// The setup component search details.
        /// </value>
        public override void SetupComponentSearchDetails(ISearchModel searchModel)
        {
            base.SetupComponentSearchDetails(searchModel);

            this.OneWay = searchModel.OneWay;

            this.OutboundJourneyDetails.Date = searchModel.DepartureDate;
            this.OutboundJourneyDetails.Time = searchModel.DepartureTime;

            this.DepartureParentId = searchModel.DepartureID;
            this.DepartureParentName = this.GetParentTypeName(this.DepartureParentType, this.DepartureParentId);

            if (!this.OneWay)
            {
                this.ReturnJourneyDetails.Date = searchModel.DepartureDate.AddDays(this.Duration);
                this.ReturnJourneyDetails.Time = searchModel.ReturnTime;

                this.ArrivalParentId = searchModel.ArrivalID;
                this.ArrivalParentName = this.GetParentTypeName(this.ArrivalParentType, this.ArrivalParentId);
            }
        }

        /// <summary>
        /// Setups the meta data.
        /// </summary>
        /// <param name="metaData">The meta data.</param>
        public override void SetupMetaData(Dictionary<string, string> metaData)
        {
            if (metaData != null)
            {
                this.OutboundJourneyDetails.FlightCode = metaData["OutboundFlightCode"];
                this.ReturnJourneyDetails.FlightCode = metaData["ReturnFlightCode"];
            }
        }
    }
}