namespace Web.Template.Application.Basket.BasketModels.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Basket.Models.Components.SubComponent;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// A class representing a flight
    /// </summary>
    /// <seealso cref="BasketCompontentBase" />
    public class UpdateFlightModel
    {
        /// <summary>
        /// Gets or sets the basket component.
        /// </summary>
        /// <value>
        /// The basket component.
        /// </value>
        public string BasketToken { get; set; }

        /// <summary>
        /// Gets or sets the component token.
        /// </summary>
        /// <value>
        /// The component token.
        /// </value>
        public int ComponentToken { get; set; }

        /// <summary>
        /// Gets or sets the sub components.
        /// </summary>
        /// <value>
        /// The sub components.
        /// </value>
        public List<FlightExtra> SubComponents { get; set; }
    }
}