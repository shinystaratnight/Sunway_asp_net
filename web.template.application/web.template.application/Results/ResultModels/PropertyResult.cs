namespace Web.Template.Application.Results.ResultModels
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using AutoMapper;

    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Basket.Models.Components.SubComponent;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    ///     Hotel ResultsCollection
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Models.IResult" />
    public class PropertyResult : IResult
    {
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyResult"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        public PropertyResult(IMapper mapper)
        {
            this.mapper = mapper;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyResult"/> class.
        /// </summary>
        public PropertyResult()
        {
        }

        /// <summary>
        /// Gets or sets the arrival date.
        /// </summary>
        /// <value>The arrival date.</value>
        public DateTime ArrivalDate { get; set; }

        /// <summary>
        ///     Gets or sets the booking token.
        /// </summary>
        /// <value>
        ///     The booking token, the unique identifier connect users for the result
        /// </value>
        public string BookingToken { get; set; }

        /// <summary>
        /// Gets or sets the component token.
        /// </summary>
        /// <value>
        /// The component token.
        /// </value>
        public int ComponentToken { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the facility flag.
        /// </summary>
        /// <value>The facility flag.</value>
        public int FacilityFlag { get; set; }

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
        /// Gets or sets the images.
        /// </summary>
        /// <value>The images.</value>
        public List<Image> Images { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        /// <value>
        /// The latitude.
        /// </value>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        /// <value>
        /// The longitude.
        /// </value>
        public double Longitude { get; set; }

        /// <summary> 
        /// Gets or sets the main image.
        /// </summary>
        /// <value>The main image.</value>
        public string MainImage { get; set; }

        /// <summary>
        ///     Gets or sets  The name
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the product attributes.
        /// </summary>
        /// <value>
        /// The product attributes.
        /// </value>
        public List<string> ProductAttributes { get; set; }

        /// <summary>
        /// Gets or sets the property reference identifier.
        /// </summary>
        /// <value>The property reference identifier.</value>
        public int PropertyReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the property osreference.
        /// </summary>
        /// <value>The property osreference.</value>
        public string OSReference { get; set; }

        /// <summary>
        ///     Gets or sets the rating.
        /// </summary>
        /// <value>
        ///     The rating.
        /// </value>
        public string Rating { get; set; }

        /// <summary>
        /// Gets or sets the review average score.
        /// </summary>
        /// <value>
        /// The review average score.
        /// </value>
        public string ReviewAverageScore { get; set; }

        /// <summary>
        /// Gets or sets the search mode.
        /// </summary>
        /// <value>
        /// The search mode.
        /// </value>
        public SearchMode SearchMode { get; set; }

        /// <summary>
        /// Gets or sets the sub results.
        /// </summary>
        /// <value>
        /// The sub results.
        /// </value>
        [XmlIgnore]
        public List<ISubResult> SubResults { get; set; }

        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        /// <value>The summary.</value>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the total price.
        /// </summary>
        /// <value>
        /// The total price.
        /// </value>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string URL { get; set; }

        /// <summary>
        /// Gets or sets the Video Code for the properties video.
        /// </summary>
        /// <value>The Code for the property video.</value>
        public string VideoCode { get; set; }

        /// <summary>
        /// Creates the basket component.
        /// </summary>
        /// <returns>The Basket Component.</returns>
        public IBasketComponent CreateBasketComponent()
        {
            IBasketComponent basketComponent = this.mapper.Map<IResult, Hotel>(this);
            basketComponent.SubComponents = new List<ISubComponent>();
            foreach (var subResult in this.SubResults)
            {
                var subComponent = this.mapper.Map<ISubResult, Room>(subResult);
                basketComponent.SubComponents.Add(subComponent);
            }

            return basketComponent;
        }
    }
}