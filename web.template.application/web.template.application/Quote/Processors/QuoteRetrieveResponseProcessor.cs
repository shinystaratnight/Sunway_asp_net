namespace Web.Template.Application.Quote.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using iVectorConnectInterface;

    using Web.Template.Application.Basket.Models;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Quote.Processors;
    using Web.Template.Application.Prebook.Models;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Class QuoteRetrieveResponseProcessor.
    /// </summary>
    public class QuoteRetrieveResponseProcessor : IQuoteRetrieveResponseProcessor
    {
        /// <summary>
        /// Processes the specified quote retrieve response.
        /// </summary>
        /// <param name="quoteRetrieveResponse">The quote retrieve response.</param>
        /// <param name="basket">The basket.</param>
        public void Process(QuoteRetrieveResponse quoteRetrieveResponse, IBasket basket)
        {
            this.UpdateBasketWithResponseValues(quoteRetrieveResponse, basket);
        }

        /// <summary>
        /// Baskets the setup payments.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <param name="paymentDue">The payment due.</param>
        private void BasketSetupPayments(IBasket basket, List<ivci.Support.PaymentDue> paymentDue)
        {
            basket.AmountDueToday = 0;
            basket.Payments = new List<IPayment>();

            foreach (ivci.Support.PaymentDue payment in paymentDue.OrderBy(p => p.DateDue))
            {
                if (payment.DateDue <= DateTime.Now)
                {
                    basket.AmountDueToday += payment.Amount;
                }
                else
                {
                    basket.OutstandingAmount += payment.Amount;
                }

                if (basket.Payments.Count(p => p.DateDue.Date == payment.DateDue.Date) > 0)
                {
                    IPayment firstOrDefault = basket.Payments.FirstOrDefault(p => p.DateDue.Date == payment.DateDue.Date);
                    if (firstOrDefault != null)
                    {
                        firstOrDefault.Amount += payment.Amount;
                    }
                }
                else
                {
                    basket.Payments.Add(new Payment() { Amount = payment.Amount, DateDue = payment.DateDue.Date });
                }
            }

            basket.AmountDueToday += basket.FlightSupplierPaymentAmount;
            var basketAdjustMentTotal = basket.Adjustments?.Sum(a => a.AdjustmentAmount) ?? 0;
            basket.AmountDueToday += basketAdjustMentTotal;
        }


        /// <summary>
        /// Processes the booking adjustments.
        /// </summary>
        /// <param name="connectBookingAdjustments">The connect booking adjustments.</param>
        /// <returns>A list of booking adjustments.</returns>
        private List<IAdjustment> ProcessBookingAdjustments(List<ivci.Basket.PreBookResponse.BookingAdjustment> connectBookingAdjustments)
        {
            var adjustments = new List<IAdjustment>();
            foreach (iVectorConnectInterface.Basket.PreBookResponse.BookingAdjustment bookingAdjustment in connectBookingAdjustments)
            {
                IAdjustment adjustment = new BookingAdjustment()
                {
                    AdjustmentType = bookingAdjustment.AdjustmentType,
                    AdjustmentAmount = bookingAdjustment.AdjustmentAmount,
                    CalculationBasis = bookingAdjustment.CalculationBasis,
                    ParentType = bookingAdjustment.ParentType
                };
                adjustments.Add(adjustment);
            }
            return adjustments;
        }

        /// <summary>
        /// Setups the lead guest details.
        /// </summary>
        /// <param name="quoteRetrieveResponse">The quote retrieve response.</param>
        /// <param name="basket">The basket.</param>
        private void SetupLeadGuestDetails(QuoteRetrieveResponse quoteRetrieveResponse, IBasket basket)
        {
            var leadCustomer = quoteRetrieveResponse.LeadCustomer;
            basket.LeadGuest = new LeadGuestDetails()
            {
                Title = leadCustomer.CustomerTitle,
                FirstName = leadCustomer.CustomerFirstName,
                LastName = leadCustomer.CustomerLastName,
                DateOfBirth = leadCustomer.DateOfBirth,
                AddressLine1 = leadCustomer.CustomerAddress1,
                AddressLine2 = leadCustomer.CustomerAddress2,
                TownCity = leadCustomer.CustomerTownCity,
                Postcode = leadCustomer.CustomerPostcode,
                BookingCountryID = leadCustomer.CustomerBookingCountryID,
                Phone = !string.IsNullOrEmpty(leadCustomer.CustomerPhone) ? leadCustomer.CustomerPhone : leadCustomer.CustomerMobile,
                Email = leadCustomer.CustomerEmail
            };
        }

        /// <summary>
        /// Setups the guests.
        /// </summary>
        /// <param name="quoteRetrieveResponse">The quote retrieve response.</param>
        /// <param name="basket">The basket.</param>
        private void SetupGuests(QuoteRetrieveResponse quoteRetrieveResponse, IBasket basket)
        {
            basket.Rooms = new List<BasketRoom>();

            if (quoteRetrieveResponse.Properties.Any())
            {
                var property = quoteRetrieveResponse.Properties.FirstOrDefault();
                var roomNumber = 1;
                if (property != null)
                {
                    foreach (var room in property.Rooms)
                    {
                        var basketRoom = new BasketRoom()
                        {
                            RoomNumber = roomNumber,
                            Guests = new List<GuestDetail>()
                        };

                        foreach (int guestID in room.GuestIDs)
                        {
                            var guest = quoteRetrieveResponse.GuestDetails.FirstOrDefault(guestDetail => guestDetail.GuestID == guestID);
                            if (guest != null)
                            {
                                GuestDetail guestDetail = this.ProcessGuest(guest);
                                basketRoom.Guests.Add(guestDetail);
                            }
                        }

                        basket.Rooms.Add(basketRoom);
                        roomNumber += 1;
                    }
                }
            }
            else
            {
                var basketRoom = new BasketRoom()
                {
                    RoomNumber = 1,
                    Guests = new List<GuestDetail>()
                };

                foreach (var guest in quoteRetrieveResponse.GuestDetails)
                {
                    GuestDetail guestDetail = this.ProcessGuest(guest);
                    basketRoom.Guests.Add(guestDetail);
                }
                basket.Rooms.Add(basketRoom);
            }
        }

        /// <summary>
        /// Processes the guest.
        /// </summary>
        /// <param name="guest">The guest.</param>
        /// <returns>Web.Template.Application.Basket.Models.GuestDetail.</returns>
        private GuestDetail ProcessGuest(ivci.Support.GuestDetail guest)
        {
            var guestDetail = new GuestDetail()
            {
                GuestID = guest.GuestID,
                BookingPassengerID = guest.BookingPassengerID,
                Type = guest.Type,
                Title = guest.Title,
                FirstName = guest.FirstName,
                LastName = guest.LastName,
                DateOfBirth = guest.DateOfBirth,
                Age = guest.Age,
            };
            return guestDetail;
        }
        /// <summary>
        /// Updates the basket with response values.
        /// </summary>
        /// <param name="quoteRetrieveResponse">The quote retrieve response.</param>
        /// <param name="basket">The basket.</param>
        private void UpdateBasketWithResponseValues(QuoteRetrieveResponse quoteRetrieveResponse, IBasket basket)
        {
            if (!string.IsNullOrEmpty(quoteRetrieveResponse.QuoteReference))
            {
                basket.BookingReference = quoteRetrieveResponse.QuoteReference.Split('/')[0];
            }
            basket.AllComponentsPreBooked = true;
            basket.PrebookTotalPrice = quoteRetrieveResponse.TotalPrice;

            this.SetupLeadGuestDetails(quoteRetrieveResponse, basket);
            this.SetupGuests(quoteRetrieveResponse, basket);

            var paymentDue = new List<ivci.Support.PaymentDue>();

            if (basket.Components != null)
            {
                basket.Errata = new List<IErratum>();

                foreach (QuoteProperty quoteProperty in quoteRetrieveResponse.Properties)
                {
                    paymentDue.AddRange(quoteProperty.PaymentsDue);

                    List<Erratum> errata = quoteProperty.Errata.Select(x => new Erratum()
                    {
                        ComponentType = ComponentType.Hotel,
                        Description = x.ErratumDescription
                    }).ToList();
                    basket.Errata.AddRange(errata);
                }

                foreach (QuoteFlight quoteFlight in quoteRetrieveResponse.Flights)
                {
                    paymentDue.AddRange(quoteFlight.PaymentsDue);
                    basket.GuestsRequireDoB = quoteFlight.ShowDateOfBirth;
                }

                foreach (QuoteTransfer quoteTransfer in quoteRetrieveResponse.Transfers)
                {
                    paymentDue.AddRange(quoteTransfer.PaymentsDue);
                }
            }

            basket.Adjustments = this.ProcessBookingAdjustments(quoteRetrieveResponse.BookingAdjustments);

            this.BasketSetupPayments(basket, paymentDue);
        }
    }
}
