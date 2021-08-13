namespace Web.Template.Application.Basket.Models.Components.SubComponent
{
    using System;
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Class ExtraOption.
    /// </summary>
    public class ExtraOption : ISubComponent
    {
        /// <summary>
        /// Gets or sets the adults.
        /// </summary>
        /// <value>The adults.</value>
        public int Adults { get; set; }

        /// <summary>
        /// Gets or sets the adult ages.
        /// </summary>
        /// <value>The adult ages.</value>
        public List<int> AdultAges { get; set; }

        /// <summary>
        /// Gets or sets the booking token.
        /// </summary>
        /// <value>The booking token.</value>
        public string BookingToken { get; set; }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>The children.</value>
        public int Children { get; set; }

        /// <summary>
        /// Gets or sets the child ages.
        /// </summary>
        /// <value>The child ages.</value>
        public List<int> ChildAges { get; set; }

        /// <summary>
        /// Gets or sets the component token.
        /// </summary>
        /// <value>The component token.</value>
        public int ComponentToken { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>The end time.</value>
        public string EndTime { get; set; }

        /// <summary>
        /// Gets or sets the extra category.
        /// </summary>
        /// <value>The extra category.</value>
        public string ExtraCategory { get; set; }

        /// <summary>
        /// Gets or sets the extra category group.
        /// </summary>
        /// <value>The extra category group.</value>
        public string ExtraCategoryGroup { get; set; }

        /// <summary>
        /// Gets or sets the extra category identifier.
        /// </summary>
        /// <value>The extra category identifier.</value>
        public int ExtraCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the infants.
        /// </summary>
        /// <value>The infants.</value>
        public int Infants { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>The start time.</value>
        public string StartTime { get; set; }

        /// <summary>
        /// Gets or sets the total price.
        /// </summary>
        /// <value>The total price.</value>
        public decimal TotalPrice { get; set; }
    }
}
