namespace Web.Template.Application.Interfaces.Models
{
    using System;
    using System.Collections.Generic;

    using Web.Template.Application.Enum;

    /// <summary>
    ///     Interface representing the key functionality of a basket component
    /// </summary>
    public interface IBasketComponent
    {
        /// <summary>
        /// Gets or sets the adults.
        /// </summary>
        /// <value>
        /// The adults.
        /// </value>
        int Adults { get; set; }

        /// <summary>
        ///     Gets or sets the arrival date.
        /// </summary>
        /// <value>
        ///     The arrival date.
        /// </value>
        DateTime ArrivalDate { get; set; }
  
        /// <summary>
        /// Gets or sets the basket token.
        /// </summary>
        /// <value>The basket token.</value>
        string BasketToken { get; set; }

        /// <summary>
        ///     Gets or sets the booking token.
        /// </summary>
        /// <value>
        ///     The booking token.
        /// </value>
        string BookingToken { get; set; }

        /// <summary>
        /// Gets or sets the cancellation.
        /// </summary>
        /// <value>
        /// The cancellation.
        /// </value>
        List<ICancellationCharge> CancellationCharges { get; set; }

        /// <summary>
        /// Gets or sets the child ages.
        /// </summary>
        /// <value>
        /// The child ages.
        /// </value>
        List<int> ChildAges { get; set; }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        int Children { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [component booked].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [component booked]; otherwise, <c>false</c>.
        /// </value>
        bool ComponentBooked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [component pre booked].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [component pre booked]; otherwise, <c>false</c>.
        /// </value>
        bool ComponentPreBooked { get; set; }

        /// <summary>
        /// Gets or sets the component token.
        /// </summary>
        /// <value>
        /// The component token.
        /// </value>
        int ComponentToken { get; set; }

        /// <summary>
        /// Gets the type of the component.
        /// </summary>
        /// <value>
        /// The type of the component.
        /// </value>
        ComponentType ComponentType { get; }

        /// <summary>
        ///     Gets or sets the display name.
        /// </summary>
        /// <value>
        ///     The display name.
        /// </value>
        string DisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the duration.
        /// </summary>
        /// <value>
        ///     The duration.
        /// </value>
        int Duration { get; set; }

        /// <summary>
        /// Gets or sets the guest i ds.
        /// </summary>
        /// <value>
        /// The guest i ds.
        /// </value>
        List<int> GuestIDs { get; set; }

        /// <summary>
        /// Gets or sets the infants.
        /// </summary>
        /// <value>
        /// The infants.
        /// </value>
        int Infants { get; set; }

        /// <summary>
        /// Gets or sets the payments.
        /// </summary>
        /// <value>
        /// The payments.
        /// </value>
        List<IPayment> Payments { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        /// <value>The price.</value>
        decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the search mode.
        /// </summary>
        /// <value>
        /// The search mode.
        /// </value>
        SearchMode SearchMode { get; set; }

        /// <summary>
        /// Gets or sets the search token.
        /// </summary>
        /// <value>The search token.</value>
        string SearchToken { get; set; }

        /// <summary>
        /// Gets or sets the sub components.
        /// </summary>
        /// <value>
        /// The sub components.
        /// </value>
        List<ISubComponent> SubComponents { get; set; }

        /// <summary>
        /// Gets or sets the terms and conditions.
        /// </summary>
        /// <value>
        /// The terms and conditions.
        /// </value>
        string TermsAndConditions { get; set; }

        /// <summary>
        /// Gets or sets the terms and conditions URL.
        /// </summary>
        /// <value>
        /// The terms and conditions URL.
        /// </value>
        string TermsAndConditionsUrl { get; set; }

        /// <summary>
        /// Gets or sets the total commission.
        /// </summary>
        /// <value>
        /// The total commission.
        /// </value>
        decimal TotalCommission { get; set; }

        /// <summary>
        ///     Gets or sets the price.
        /// </summary>
        /// <value>
        ///     The price.
        /// </value>
        decimal TotalPrice { get; set; }

        /// <summary>
        /// Gets or sets the vat on commission.
        /// </summary>
        /// <value>The vat on commission.</value>
        decimal VATOnCommission { get; set; }

        /// <summary>
        /// Gets or sets the setup component search details.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        /// <value>
        /// The setup component search details.
        /// </value>
        void SetupComponentSearchDetails(ISearchModel searchModel);

        /// <summary>
        /// Setups the component extra search details.
        /// </summary>
        /// <param name="extraSearchModel">The extra search model.</param>
        void SetupComponentExtraSearchDetails(IExtraSearchModel extraSearchModel);

        /// <summary>
        /// Setups the meta data.
        /// </summary>
        /// <param name="metaData">The meta data.</param>
        void SetupMetaData(Dictionary<string, string> metaData);
    }
}