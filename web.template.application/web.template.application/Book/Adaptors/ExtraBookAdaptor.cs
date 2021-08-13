namespace Web.Template.Application.Book.Adaptors
{
    using System.Collections.Generic;

    using iVectorConnectInterface.Basket;

    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Basket.Models.Components.SubComponent;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Book;
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Class ExtraBookAdaptor.
    /// </summary>
    public class ExtraBookAdaptor : IBookRequestAdaptor
    {
        /// <summary>
        /// Gets or sets the type of the component.
        /// </summary>
        /// <value>The type of the component.</value>
        public ComponentType ComponentType => ComponentType.Extra;

        public void Create(IBasketComponent basketComponent, BookRequest connectRequestBody)
        {
            var extra = (Extra)basketComponent;

            var extraBookRequest = new iVectorConnectInterface.Extra.BookRequest()
                                       {
                                           ExpectedTotal = extra.TotalPrice
                                       };

            if (extra.IncludeOptions)
            {
                extraBookRequest.ExtraOptions = new List<iVectorConnectInterface.Extra.BookRequest.ExtraOption>();
                foreach (ISubComponent subComponent in extra.SubComponents)
                {
                    var extraSubComponent = (ExtraOption)subComponent;
                    var basketExtraOption = new iVectorConnectInterface.Extra.BookRequest.ExtraOption
                                                {
                                                    BookingToken = extraSubComponent.BookingToken,
                                                    GuestIDs = extra.GuestIDs,
                                                };
                    extraBookRequest.ExtraOptions.Add(basketExtraOption);
                }
            }
            else
            {
                extraBookRequest.BookingToken = extra.BookingToken;
                extraBookRequest.GuestIDs = extra.GuestIDs;
            }

            connectRequestBody.ExtraBookings.Add(extraBookRequest);
        }
    }
}
