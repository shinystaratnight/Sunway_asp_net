namespace Web.Template.Application.Results.ResultModels
{
    using System.Collections.Generic;

    using AutoMapper;

    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Class ExtraResult.
    /// </summary>
    public class ExtraResult : IResult
    {
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtraResult"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        public ExtraResult(IMapper mapper)
        {
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets or sets the booking token.
        /// </summary>
        /// <value>The booking token, the unique identifier connect uses for the result</value>
        public string BookingToken { get; set; }

        /// <summary>
        /// Gets or sets the component token.
        /// </summary>
        /// <value>The component token.</value>
        public int ComponentToken { get; set; }

        /// <summary>
        /// Gets or sets the extra identifier.
        /// </summary>
        /// <value>The extra identifier.</value>
        public int ExtraId { get; set; }

        /// <summary>
        /// Gets or sets the name of the extra.
        /// </summary>
        /// <value>The name of the extra.</value>
        public string ExtraName { get; set; }

        /// <summary>
        /// Gets or sets the type of the extra.
        /// </summary>
        /// <value>The type of the extra.</value>
        public string ExtraType { get; set; }

        /// <summary>
        /// Gets or sets the extra type identifier.
        /// </summary>
        /// <value>The extra type identifier.</value>
        public int ExtraTypeId { get; set; }

        /// <summary>
        /// Gets or sets the search mode.
        /// </summary>
        /// <value>The search mode.</value>
        public SearchMode SearchMode { get; set; }

        /// <summary>
        /// Gets or sets the sub results.
        /// </summary>
        /// <value>The sub results.</value>
        public List<ISubResult> SubResults { get; set; }

        /// <summary>
        /// Gets or sets the total price.
        /// </summary>
        /// <value>The total price.</value>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Creates the basket component.
        /// </summary>
        /// <returns>The Basket Component.</returns>
        public IBasketComponent CreateBasketComponent()
        {
            IBasketComponent basketComponent = this.mapper.Map<IResult, Extra>(this);
            basketComponent.SubComponents = new List<ISubComponent>();
            foreach (var subResult in this.SubResults)
            {
                var subComponent = this.mapper.Map<ISubResult, Basket.Models.Components.SubComponent.ExtraOption>(subResult);
                basketComponent.SubComponents.Add(subComponent);

                basketComponent.ArrivalDate = subComponent.StartDate;
                basketComponent.Duration = (subComponent.EndDate - subComponent.StartDate).Days;
            }
            return basketComponent;
        }
    }
}