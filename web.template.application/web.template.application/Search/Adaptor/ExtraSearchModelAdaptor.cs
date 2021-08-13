
namespace Web.Template.Application.Search.Adaptor
{
    using System.Collections.Generic;
    using System.Linq;

    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Search.SearchModels;

    /// <summary>
    /// Class ExtraSearchModelAdaptor.
    /// </summary>
    public class ExtraSearchModelAdaptor : IExtraSearchModelAdaptor
    {
        /// <summary>
        /// Gets or sets the basket.
        /// </summary>
        /// <value>The basket.</value>
        private IBasket Basket { get; set; }

        /// <summary>
        /// Gets or sets the extra search model.
        /// </summary>
        /// <value>The extra search model.</value>
        private ExtraSearchModel ExtraSearchModel { get; set; }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <param name="extraBasketSearchModel">The extra basket search model.</param>
        /// <returns>The ExtraSearchModel.</returns>
        public ExtraSearchModel Create(IBasket basket, IExtraBasketSearchModel extraBasketSearchModel)
        {
            this.Basket = basket;
            this.ExtraSearchModel = new ExtraSearchModel()
            {
                BookingType = basket.SearchDetails?.SearchMode.ToString(),
                BookingPrice = basket.TotalPrice,
                ExtraId = extraBasketSearchModel.ExtraId,
                ExtraGroupId = extraBasketSearchModel.ExtraGroupId,
                ExtraTypes = extraBasketSearchModel.ExtraTypes
            };

            if (basket.SearchDetails != null)
            {
                this.SetupGuests();
            }

            if (basket.Components != null)
            {
                this.SetupComponentDetails();
            }

            return this.ExtraSearchModel;
        }

        /// <summary>
        /// Setups the guests.
        /// </summary>
        private void SetupGuests()
        {
            this.ExtraSearchModel.Adults = this.Basket.SearchDetails.Rooms.Sum(room => room.Adults);
            this.ExtraSearchModel.Children = this.Basket.SearchDetails.Rooms.Sum(room => room.Children);
            this.ExtraSearchModel.Infants = this.Basket.SearchDetails.Rooms.Sum(room => room.Infants);
            this.ExtraSearchModel.AdultAges = new List<int>();
            this.ExtraSearchModel.ChildAges = new List<int>();

            foreach (Room room in this.Basket.SearchDetails.Rooms)
            {
                this.ExtraSearchModel.ChildAges.AddRange(room.ChildAges);
            }
        }

        /// <summary>
        /// Setups the component details.
        /// </summary>
        private void SetupComponentDetails()
        {
            var basketFlight = (Flight)this.Basket.Components.FirstOrDefault(component => component.ComponentType == ComponentType.Flight);
            var basketHotel = (Hotel)this.Basket.Components.FirstOrDefault(component => component.ComponentType == ComponentType.Hotel);

            if (basketFlight != null)
            {
                this.ExtraSearchModel.DepartureAirportId = basketFlight.DepartureAirportId;
                this.ExtraSearchModel.ArrivalAirportId = basketFlight.ArrivalAirportId;

                this.ExtraSearchModel.DepartureDate = basketFlight.OutboundFlightDetails.DepartureDate;
                this.ExtraSearchModel.DepartureTime = basketFlight.OutboundFlightDetails.DepartureTime;

                this.ExtraSearchModel.ReturnDate = basketFlight.ReturnFlightDetails.ArrivalDate;
                this.ExtraSearchModel.ReturnTime = basketFlight.ReturnFlightDetails.ArrivalTime;
            }

            if (basketHotel != null)
            {
                this.ExtraSearchModel.GeographyLevel1Id = basketHotel.GeographyLevel1Id;
                this.ExtraSearchModel.GeographyLevel2Id = basketHotel.GeographyLevel2Id;
                this.ExtraSearchModel.GeographyLevel3Id = basketHotel.GeographyLevel3Id;
                this.ExtraSearchModel.PropertyReferenceId = basketHotel.PropertyReferenceId;

                if (basketFlight == null)
                {
                    this.ExtraSearchModel.DepartureDate = basketHotel.ArrivalDate;
                    this.ExtraSearchModel.ReturnDate = basketHotel.ArrivalDate.AddDays(basketHotel.Duration);
                }
            }
        }
    }
}
