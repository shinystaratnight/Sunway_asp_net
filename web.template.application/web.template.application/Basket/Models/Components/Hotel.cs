namespace Web.Template.Application.Basket.Models.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Web.Template.Application.Basket.Models.Components.SubComponent;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    ///     Hotel Basket component
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Models.IBasketComponent" />
    public class Hotel : BasketCompontentBase
    {
        /// <summary>
        /// Gets or sets the type of the component.
        /// </summary>
        /// <value>
        /// The type of the component.
        /// </value>
        public override ComponentType ComponentType => ComponentType.Hotel;

        /// <summary>
        ///     Gets or sets  The Country
        /// </summary>
        /// <value>
        ///     The Country.
        /// </value>
        public int GeographyLevel1Id { get; set; }

        /// <summary>
        ///     Gets or sets  The Region
        /// </summary>
        /// <value>
        ///     The Region.
        /// </value>
        public int GeographyLevel2Id { get; set; }

        /// <summary>
        ///     Gets or sets  The Resort
        /// </summary>
        /// <value>
        ///     The Resort.
        /// </value>
        public int GeographyLevel3Id { get; set; }

        /// <summary>
        ///     Gets or sets  The name
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the property reference identifier.
        /// </summary>
        /// <value>
        /// The property reference identifier.
        /// </value>
        public int PropertyReferenceId { get; set; }

        /// <summary>
        ///     Gets or sets the rating.
        /// </summary>
        /// <value>
        ///     The rating.
        /// </value>
        public string Rating { get; set; }

        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        /// <value>
        /// The request.
        /// </value>
        public string Request { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        /// <value>
        /// The price.
        /// </value>
        public override decimal TotalPrice { get; set; }

        /// <summary>
        /// Gets or sets the setup component search details.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        /// <value>
        /// The setup component search details.
        /// </value>
        public override void SetupComponentSearchDetails(ISearchModel searchModel)
        {
            foreach (ISubComponent subComponent in this.SubComponents)
            {
                Room room = (Room)subComponent;
                room.SetupSubcomponentSearchDetails(searchModel.Rooms[room.Sequence - 1]);
            }

            this.TotalPrice = this.SubComponents.Sum(s => s.TotalPrice);
        }

        /// <summary>
        /// Setups the meta data.
        /// </summary>
        /// <param name="metaData">The meta data.</param>
        public override void SetupMetaData(Dictionary<string, string> metaData)
        {
            if (metaData != null)
            {
                this.ArrivalDate = DateTime.Parse(metaData["ArrivalDate"]);
                this.Duration = Int32.Parse(metaData["Duration"]);
            }
        }
    }
}