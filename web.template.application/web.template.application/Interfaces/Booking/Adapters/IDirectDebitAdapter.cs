namespace Web.Template.Application.Interfaces.Booking.Adapters
{
    using System.Collections.Generic;

    using iVectorConnectInterface.Modules;

    using Newtonsoft.Json.Linq;

    using Web.Template.Application.Booking.Models;
    using Web.Template.Application.Interfaces.Booking.Models;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// An interface defining a class that converts into a booking
    /// </summary>
    public interface IDirectDebitAdapter
    {
        /// <summary>
        /// Creates the booking from get booking details response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>
        /// A booking
        /// </returns>
        IDirectDebitRetrieveReturn CreateBookingLineFromDirectDebitRetrieveResponse(ivci.Modules.ModuleResponse response);
    }

    /// <summary>
    /// Adapts the direct debit retrieve response
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Booking.Adapters.IDirectDebitAdapter" />
    public class DirectDebitAdapter : IDirectDebitAdapter
    {
        /// <summary>
        /// Creates the booking from get booking details response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>
        /// A booking
        /// </returns>
        public IDirectDebitRetrieveReturn CreateBookingLineFromDirectDebitRetrieveResponse(ModuleResponse response)
        {
            dynamic content = JObject.Parse(response.Response.ToString());

            List<IBookingLineItem> bookingLine = new List<IBookingLineItem>();
            foreach (var item in content.BookingLine)
            {
                BookingLineItem lineItem = new BookingLineItem()
                                               {
                                                   Id = item.ID,
                                                   DirectDebitListId = item.DirectDebitListID,
                                                   BookingId = item.BookingID,
                                                   BookingReference = item.BookingReference,
                                                   BrandId = item.BrandID,
                                                   TradeHeadOfficeName = item.TradeHeadOfficeName,
                                                   TradeHeadOfficeAbtaatol = item.TradeHeadOfficeABTAATOL,
                                                   TradeName = item.TradeName,
                                                   TradeAbtaatol = item.TradeABTAATOL,
                                                   TradeId = item.TradeID,
                                                   HeadOfficeTradeId = item.HeadOfficeTradeID,
                                                   LeadGuestFirstName = item.LeadGuestFirstName,
                                                   LeadGuestLastName = item.LeadGuestLastName,
                                                   DepartureDate = item.DepartureDate,
                                                   AmountDue = item.AmountDue,
                                                   Pay = item.Pay,
                };
                bookingLine.Add(lineItem);
            }

            DirectDebitRetrieveReturn directDebitRetrieveReturn
                = new DirectDebitRetrieveReturn()
                    {
                        BookingLine = bookingLine,
                        Id = content.ID,
                        ModuleUserId = content.ModuleUserID,
                        AllowAgentReadOnlyAccess = content.AllowAgentReadOnlyAccess,
                        PaymentDueDaysAhead = content.PaymentDueDaysAhead,
                        PaymentDueBeforeDate = content.PaymentDueBeforeDate,
                        IgnoreOverPayments = content.IgnoreOverPayments,
                        Archived = content.Archived,
                };

            return directDebitRetrieveReturn;
        }
    }
}