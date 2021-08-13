namespace Web.Template.Application.Prebook.Adaptor
{
    using System.Collections.Generic;
    using System.Linq;

    using iVectorConnectInterface.Extra;
    using iVectorConnectInterface.Support;

    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Basket.Models.Components.SubComponent;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Prebook;
    public class ExtraPrebookAdaptor : IPrebookRequestAdaptor
    {
        /// <summary>
        /// Gets or sets the type of the component.
        /// </summary>
        /// <value>The type of the component.</value>
        public ComponentType ComponentType => ComponentType.Extra;

        /// <summary>
        /// Creates the specified basket component.
        /// </summary>
        /// <param name="basketComponent">The basket component.</param>
        /// <param name="connectRequestBody">The connect request body.</param>
        public void Create(IBasketComponent basketComponent, iVectorConnectInterface.Basket.PreBookRequest connectRequestBody)
        {
            var extra = (Extra)basketComponent;
            var extraRequest = new PreBookRequest();

            if (extra.IncludeOptions)
            {
                extraRequest.ExtraOptions = new List<PreBookRequest.ExtraOption>();
                foreach (ISubComponent subComponent in extra.SubComponents)
                {
                    var extraSubComponent = (ExtraOption)subComponent;
                    var basketExtraOption = new PreBookRequest.ExtraOption
                    {
                        BookingToken = extraSubComponent.BookingToken,
                        GuestConfiguration = new GuestConfiguration
                        {
                            Adults = extraSubComponent.Adults,
                            Children = extraSubComponent.Children,
                            Infants = extraSubComponent.Infants,
                            AdultAges = extraSubComponent.AdultAges,
                            ChildAges = extraSubComponent.ChildAges
                        }
                    };

                    extraRequest.ExtraOptions.Add(basketExtraOption);
                }
            }
            else
            {
                var extraOption = (ExtraOption)extra.SubComponents.FirstOrDefault();
                extraRequest.BookingToken = extraOption?.BookingToken;
                extraRequest.DepartureDate = extraOption?.StartDate ?? extra.ArrivalDate;
                extraRequest.ReturnDate = extraOption?.EndDate ?? extra.ArrivalDate.AddDays(extra.Duration);

                extraRequest.GuestConfiguration = new GuestConfiguration
                {
                    Adults = extra.Adults,
                    Children = extra.Children,
                    Infants = extra.Infants,
                    AdultAges = extra.AdultAges,
                    ChildAges = extra.ChildAges
                };
            }

            connectRequestBody.ExtraBookings.Add(extraRequest);

        }
    }
}
