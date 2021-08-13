namespace Web.Template.Application.Prebook
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using iVectorConnectInterface.Property;

    using Web.Template.Application.Basket.Models;
    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Basket.Models.Components.SubComponent;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Prebook;
    using Web.Template.Application.Prebook.Models;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Updates basket with values from Prebook response, could potentially make a basket visitor instead.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Prebook.IPrebookResponseProcessor" />
    public class PrebookResponseProcessor : IPrebookResponseProcessor
    {
        /// <summary>
        /// Processes the specified pre book response.
        /// </summary>
        /// <param name="preBookResponse">The pre book response.</param>
        /// <param name="basket">The basket.</param>
        /// <param name="basketComponent">The basket component.</param>
        public void Process(iVectorConnectInterface.Basket.PreBookResponse preBookResponse, IBasket basket, IBasketComponent basketComponent)
        {
            if (basketComponent != null)
            {
                this.UpdateBasketWithResponseValues(preBookResponse, basket, basketComponent);
            }
            else
            {
                this.UpdateBasketWithResponseValues(preBookResponse, basket);
            }
        }

        /// <summary>
        /// Baskets the setup payments.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <param name="paymentDue">The payment due.</param>
        private void BasketSetupPayments(IBasket basket, List<ivci.Support.PaymentDue> paymentDue)
        {
            basket.Payments = new List<IPayment>();

            foreach (ivci.Support.PaymentDue payment in paymentDue.OrderBy(p => p.DateDue))
            {
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
        }

        /// <summary>
        /// Processes the booking adjustments.
        /// </summary>
        /// <param name="connectBookingAdjustments">The connect booking adjustments.</param>
        /// <returns>A list of booking adjustment.</returns>
        private List<IAdjustment> ProcessBookingAdjustments(List<iVectorConnectInterface.Basket.PreBookResponse.BookingAdjustment> connectBookingAdjustments)
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
        /// Updates the basket flight from prebook.
        /// </summary>
        /// <param name="basketFlight">The basket flight.</param>
        /// <param name="flight">The flight.</param>
        /// <param name="isMultiCarrierReturn"></param>
        private void UpdateBasketFlightFromPrebook(IBasketComponent basketFlight, iVectorConnectInterface.Flight.PreBookResponse flight)
        {
            basketFlight.BookingToken = flight.BookingToken;
            basketFlight.Price = flight.TotalPrice;
            basketFlight.TotalCommission = flight.TotalCommission;
            basketFlight.VATOnCommission = flight.VATOnCommission;
            basketFlight.ComponentPreBooked = true;
            if(basketFlight is Flight)
            {
                var castedFlight = (Flight)basketFlight;
                castedFlight.IncludesSupplierBaggage = flight.IncludesSupplierBaggage;
                castedFlight.IncludedBaggageText = flight.IncludedBaggageText;
            }

            foreach (iVectorConnectInterface.Flight.PreBookResponse.Extra extra in flight.Extras)
            {
                var bookingToken = extra.ExtraBookingToken;

                FlightExtra existingExtra = null;
                if (basketFlight.SubComponents.Exists(sc => sc.BookingToken == bookingToken))
                {
                    existingExtra = (FlightExtra)basketFlight.SubComponents.FirstOrDefault(sc => sc.BookingToken == bookingToken);
                }

                var totalPax = basketFlight.Adults + basketFlight.Children + basketFlight.Infants;

                if (existingExtra == null)
                {
                    var flightExtra = new FlightExtra();
                    flightExtra.UpdateDetailsFromConnect(extra, totalPax);
                    basketFlight.SubComponents.Add(flightExtra);
                }
                else
                {
                    existingExtra.UpdateDetailsFromConnect(extra, totalPax);
                }

                basketFlight.Price -= extra.Price * extra.QuantitySelected;
            }

            basketFlight.TermsAndConditions = flight.TermsAndConditions;
            basketFlight.TermsAndConditionsUrl = flight.TermsAndConditionsURL;

            basketFlight.CancellationCharges = new List<ICancellationCharge>();
            foreach (var cancellation in flight.Cancellations)
            {
                basketFlight.CancellationCharges.Add(new CancellationCharge() { Amount = cancellation.Amount, EndDate = cancellation.EndDate, StartDate = cancellation.StartDate });
            }

            basketFlight.Payments = new List<IPayment>();
            foreach (var paymentdue in flight.PaymentsDue)
            {
                basketFlight.Payments.Add(new Payment() { Amount = paymentdue.Amount, DateDue = paymentdue.DateDue });
            }
        }

        private void UpdateBasketFlightFromMultiCarrierPrebook(IBasketComponent basketFlightComponent, iVectorConnectInterface.Flight.PreBookResponse flight)
        {
            var basketFlight = (Flight)basketFlightComponent;

            if (string.IsNullOrEmpty(basketFlight.ReturnMultiCarrierDetails.SearchBookingToken))
            {
                basketFlight.ReturnMultiCarrierDetails.SearchBookingToken = basketFlight.ReturnMultiCarrierDetails.BookingToken;
            }
            basketFlight.ReturnMultiCarrierDetails.BookingToken = flight.BookingToken;

            basketFlight.Price += flight.TotalPrice;
            basketFlight.TotalCommission += flight.TotalCommission;
            basketFlight.VATOnCommission += flight.VATOnCommission;

            basketFlight.ReturnMultiCarrierDetails.Price = flight.TotalPrice;
            basketFlight.ReturnMultiCarrierDetails.TotalCommission = flight.TotalCommission;

            basketFlight.ReturnMultiCarrierDetails.TermsAndConditions = flight.TermsAndConditions;
            basketFlight.ReturnMultiCarrierDetails.TermsAndConditionsUrl = flight.TermsAndConditionsURL;

            foreach (iVectorConnectInterface.Flight.PreBookResponse.Extra extra in flight.Extras)
            {
                var bookingToken = extra.ExtraBookingToken;

                FlightExtra existingExtra = null;
                if (basketFlight.SubComponents.Exists(sc => sc.BookingToken == bookingToken))
                {
                    existingExtra = (FlightExtra)basketFlight.SubComponents.FirstOrDefault(
                        sc =>
                            {
                                var fe = (FlightExtra)sc;
                                return sc.BookingToken == bookingToken && fe.ReturnMultiCarrierExtra;
                            });
                }

                var totalPax = basketFlight.Adults + basketFlight.Children + basketFlight.Infants;

                if (existingExtra == null)
                {
                    var flightExtra = new FlightExtra();
                    flightExtra.UpdateDetailsFromConnect(extra, totalPax);
                    flightExtra.ReturnMultiCarrierExtra = true;
                    basketFlight.SubComponents.Add(flightExtra);
                }
                else
                {
                    existingExtra.UpdateDetailsFromConnect(extra, totalPax);
                }

                basketFlight.Price -= extra.Price * extra.QuantitySelected;
            }

            foreach (var cancellation in flight.Cancellations)
            {
                basketFlight.CancellationCharges.Add(new CancellationCharge() { Amount = cancellation.Amount, EndDate = cancellation.EndDate, StartDate = cancellation.StartDate });
            }

            foreach (var paymentdue in flight.PaymentsDue)
            {
                basketFlight.Payments.Add(new Payment() { Amount = paymentdue.Amount, DateDue = paymentdue.DateDue });
            }
        }

        /// <summary>
        /// Updates the basket property from prebook.
        /// </summary>
        /// <param name="basketProperty">The basket property.</param>
        /// <param name="property">The property.</param>
        private void UpdateBasketPropertyFromPrebook(IBasketComponent basketProperty, PreBookResponse property)
        {
            var priceChange = property.TotalPrice - basketProperty.TotalPrice;
            var roomCount = basketProperty.SubComponents.Count;
            basketProperty.BookingToken = property.BookingToken;
            basketProperty.ComponentPreBooked = true;

            foreach (ISubComponent subComponent in basketProperty.SubComponents)
            {
                subComponent.TotalPrice = property.TotalPrice / roomCount;
            }

            basketProperty.CancellationCharges = new List<ICancellationCharge>();
            foreach (var cancellation in property.Cancellations)
            {
                basketProperty.CancellationCharges.Add(new CancellationCharge() { Amount = cancellation.Amount, EndDate = cancellation.EndDate, StartDate = cancellation.StartDate });
            }

            basketProperty.Payments = new List<IPayment>();
            foreach (var paymentdue in property.PaymentsDue)
            {
                basketProperty.Payments.Add(new Payment() { Amount = paymentdue.Amount, DateDue = paymentdue.DateDue });
            }

            basketProperty.TotalCommission = property.TotalCommission;
            basketProperty.VATOnCommission = property.VATOnCommission;
            basketProperty.TotalPrice = property.TotalPrice;
            basketProperty.TermsAndConditions = property.TermsAndConditions;
            basketProperty.TermsAndConditionsUrl = property.TermsAndConditionsURL;
        }

        /// <summary>
        /// Updates the basket transfer from prebook.
        /// </summary>
        /// <param name="basketTransfer">The basket transfer.</param>
        /// <param name="transfer">The transfer.</param>
        private void UpdateBasketTransferFromPrebook(IBasketComponent basketTransfer, iVectorConnectInterface.Transfer.PreBookResponse transfer)
        {
            basketTransfer.BookingToken = transfer.BookingToken;
            basketTransfer.TotalPrice = transfer.TotalPrice;
            basketTransfer.TotalCommission = transfer.TotalCommission;
            basketTransfer.VATOnCommission = transfer.VATOnCommission;
            basketTransfer.ComponentPreBooked = true;

            basketTransfer.CancellationCharges = new List<ICancellationCharge>();
            foreach (var cancellation in transfer.Cancellations)
            {
                basketTransfer.CancellationCharges.Add(new CancellationCharge() { Amount = cancellation.Amount, EndDate = cancellation.EndDate, StartDate = cancellation.StartDate });
            }

            basketTransfer.Payments = new List<IPayment>();
            foreach (var paymentdue in transfer.PaymentsDue)
            {
                basketTransfer.Payments.Add(new Payment() { Amount = paymentdue.Amount, DateDue = paymentdue.DateDue });
            }
        }

        /// <summary>
        /// Updates the basket extra from prebook.
        /// </summary>
        /// <param name="basketExtra">The basket extra.</param>
        /// <param name="extra">The extra.</param>
        private void UpdateBasketExtraFromPrebook(IBasketComponent basketExtra, iVectorConnectInterface.Extra.PreBookResponse extra)
        {
            basketExtra.BookingToken = extra.BookingToken;
            basketExtra.TotalPrice = extra.TotalPrice;
            basketExtra.TotalCommission = extra.TotalCommission;
            basketExtra.VATOnCommission = extra.VATOnCommission;
            basketExtra.ComponentPreBooked = true;

            basketExtra.CancellationCharges = new List<ICancellationCharge>();
            foreach (var cancellation in extra.Cancellations)
            {
                basketExtra.CancellationCharges.Add(new CancellationCharge { Amount = cancellation.Amount, EndDate = cancellation.EndDate, StartDate = cancellation.StartDate });
            }

            basketExtra.Payments = new List<IPayment>();
            foreach (var paymentDue in extra.PaymentsDue)
            {
                basketExtra.Payments.Add(new Payment { Amount = paymentDue.Amount, DateDue = paymentDue.DateDue });
            }
        }

        /// <summary>
        /// Updates the basket with response values.
        /// </summary>
        /// <param name="preBookResponse">The pre book response.</param>
        /// <param name="basket">The basket.</param>
        private void UpdateBasketWithResponseValues(iVectorConnectInterface.Basket.PreBookResponse preBookResponse, IBasket basket)
        {
            basket.AllComponentsPreBooked = true;
            basket.PrebookTotalPrice = preBookResponse.TotalPrice;
            basket.FlightSupplierPaymentAmount = preBookResponse.PaymentAmountDetail.FlightSupplierPaymentAmount;
            basket.CommissionPercentage = preBookResponse.CommissionPercentage;
            basket.VATOnCommissionPercentage = preBookResponse.VATOnCommissionPercentage;
            basket.IsTrade = preBookResponse.IsTrade;

            // This is where we get valid creditCardTypes
            var paymentDue = new List<ivci.Support.PaymentDue>();
 
            var componentCount = 0;
            if (basket.Components != null)
            {
                basket.Errata = new List<IErratum>();
                foreach (PreBookResponse property in preBookResponse.PropertyBookings)
                {
                    var basketProperty = basket.Components.Where(c => c.ComponentType == ComponentType.Hotel).ToList()[componentCount];
                    this.UpdateBasketPropertyFromPrebook(basketProperty, property);
                    paymentDue.AddRange(property.PaymentsDue);
                    componentCount += 1;

                    List<Erratum> errata = property.Errata.Select(x => new Erratum()
                    {
                        ComponentType = ComponentType.Hotel,
                        Description = x.ErratumDescription,
                        Subject = x.ErratumSubject
                    }).ToList();
                    basket.Errata.AddRange(errata);
                }

                componentCount = 0;
                var keepSameComponent = false;
                foreach (iVectorConnectInterface.Flight.PreBookResponse flight in preBookResponse.FlightBookings.OrderBy(fb => fb.MultiCarrierReturn))
                {
                    var basketFlight = (Flight)basket.Components.Where(c => c.ComponentType == ComponentType.Flight).ToList()[componentCount];

                    if (flight.MultiCarrierReturn)
                    {
                        this.UpdateBasketFlightFromMultiCarrierPrebook(basketFlight, flight);
                    }
                    else
                    {
                        this.UpdateBasketFlightFromPrebook(basketFlight, flight);
                    }

                    paymentDue.AddRange(flight.PaymentsDue);

                    if (flight.ShowDateOfBirth)
                    {
                        basket.GuestsRequireDoB = true;
                    }

                    // Mix and match flights, have two prebooks for the same component, so we only want to increment on the second loop
                    keepSameComponent = flight.MultiCarrier && !keepSameComponent;

                    if (!keepSameComponent)
                    {
                        componentCount += 1;
                    }

                    List<Erratum> errata = flight.FlightErrata.Select(x => new Erratum()
                   {
                       ComponentType = ComponentType.Flight,
                       Description = x.ErratumDescription,
                       Subject = x.ErratumSubject
                   }).ToList();
                    basket.Errata.AddRange(errata);
                }

                componentCount = 0;
                foreach (iVectorConnectInterface.Transfer.PreBookResponse transfer in preBookResponse.TransferBookings)
                {
                    var basketTransfer = basket.Components.Where(c => c.ComponentType == ComponentType.Transfer).ToList()[componentCount];
                    this.UpdateBasketTransferFromPrebook(basketTransfer, transfer);
                    paymentDue.AddRange(transfer.PaymentsDue);
                    componentCount += 1;
                }

                componentCount = 0;
                foreach (iVectorConnectInterface.Extra.PreBookResponse extra in preBookResponse.ExtraBookings)
                {
                    var basketExtra = basket.Components.Where(c => c.ComponentType == ComponentType.Extra).ToList()[componentCount];
                    this.UpdateBasketExtraFromPrebook(basketExtra, extra);
                    paymentDue.AddRange(extra.PaymentsDue);
                    componentCount += 1;
                }
            }

            basket.Adjustments = this.ProcessBookingAdjustments(preBookResponse.BookingAdjustments);

            basket.AmountDueToday = preBookResponse.PaymentAmountDetail.DueNowPaymentAmount;
            basket.OutstandingAmount = preBookResponse.PaymentAmountDetail.TotalPaymentAmount
                                       - preBookResponse.PaymentAmountDetail.DueNowPaymentAmount;

            this.BasketSetupPayments(basket, paymentDue);
        }

        /// <summary>
        /// Updates the basket with response values.
        /// </summary>
        /// <param name="preBookResponse">The pre book response.</param>
        /// <param name="basket">The basket.</param>
        /// <param name="basketComponent">A basket component.</param>
        private void UpdateBasketWithResponseValues(iVectorConnectInterface.Basket.PreBookResponse preBookResponse, IBasket basket, IBasketComponent basketComponent)
        {
            switch (basketComponent.ComponentType)
            {
                case ComponentType.Flight:
                    var basketFlight = (Flight)basketComponent;
                    if (!string.IsNullOrEmpty(basketFlight.ReturnMultiCarrierDetails?.BookingToken))
                    {
                        this.UpdateBasketFlightFromPrebook(
                            basketComponent,
                            preBookResponse.FlightBookings.FirstOrDefault(f => f.MultiCarrierOutbound));
                        this.UpdateBasketFlightFromMultiCarrierPrebook(
                            basketComponent,
                            preBookResponse.FlightBookings.FirstOrDefault(f => f.MultiCarrierReturn));
                    }
                    else
                    {
                        this.UpdateBasketFlightFromPrebook(basketComponent, preBookResponse.FlightBookings[0]);
                    }

                    break;
                case ComponentType.Hotel:
                    var basketProperty = (Hotel)basketComponent;
                    this.UpdateBasketPropertyFromPrebook(basketProperty, preBookResponse.PropertyBookings[0]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}