namespace Web.Template.Application.Results.Adaptors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Xml;
    using System.Xml.Linq;

    using AutoMapper;
    using Enum;
    using Intuitive;

    using iVectorConnectInterface.Property;

    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Results;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Results.ResultModels;

    /// <summary>
    /// Class used to adapt from the connect result into our domain property class
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Results.IConnectResultComponentAdaptor" />
    public class ConnectPropertyAdaptor : IConnectResultComponentAdaptor
    {
        /// <summary>
        /// The facility flag x path
        /// </summary>
        private const string FacilityFlagXPath = "Property/FacilityFlag";

		/// <summary>
		/// The video code x path
		/// </summary>
		private const string LatitudeXPath = "Property/Latitude";

		/// <summary>
		/// The video code x path
		/// </summary>
		private const string LongitudeXPath = "Property/Longitude";

		/// <summary>
		/// The main image x path
		/// </summary>
		private const string MainImageXPath = "Property/MainImage";

        /// <summary>
        /// The name x path
        /// </summary>
        private const string NameXPath = "Property/PropertyName";

        /// <summary>
        /// The osreference x path
        /// </summary>
        private const string OSReferencePath = "Property/OSReference";

        /// <summary>
        /// The rating x path
        /// </summary>
        private const string RatingXPath = "Property/Rating";

        /// <summary>
        /// The rating x path
        /// </summary>
        private const string ReviewXPath = "Property/ReviewAverageScore";

        /// <summary>
        /// The summary x path
        /// </summary>
        private const string SummaryXPath = "Property/Summary";

        /// <summary>
        /// The urlx path
        /// </summary>
        private const string URLXPath = "Property/URL";

        /// <summary>
        /// The video code x path
        /// </summary>
        private const string VideoCodeXPath = "Property/VideoCode";

        /// <summary>
        /// The arrival date
        /// </summary>
        private DateTime arrivalDate;

        /// <summary>
        /// The duration
        /// </summary>
        private int duration;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// The site service
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectPropertyAdaptor" /> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="siteService">The site service.</param>
        public ConnectPropertyAdaptor(IMapper mapper, ISiteService siteService)
        {
            this.mapper = mapper;
            this.siteService = siteService;
        }

        /// <summary>
        /// Gets the type of the component.
        /// </summary>
        /// <value>
        /// The type of the component.
        /// </value>
        public Type ComponentType => typeof(SearchResponse.PropertyResult);

        /// <summary>
        /// Creates the specified property result.
        /// </summary>
        /// <param name="connectResult">The connect result.</param>
        /// <param name="searchMode">The search mode.</param>
        /// <param name="context">The context.</param>
        /// <returns>A single result</returns>
        public IResult Create(object connectResult, SearchMode searchMode, HttpContext context)
        {
            ISite site = this.siteService.GetSite(context);

            var ivcPropertyResult = (SearchResponse.PropertyResult)connectResult;
            var rating = XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, RatingXPath);

            var propertyResult = new PropertyResult(this.mapper)
            {
                BookingToken = ivcPropertyResult.BookingToken,
                ArrivalDate = this.arrivalDate,
                Duration = this.duration,
                SearchMode = searchMode,
                FacilityFlag = Functions.SafeInt(XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, FacilityFlagXPath)),
                GeographyLevel1Id = ivcPropertyResult.GeographyLevel1ID,
                GeographyLevel2Id = ivcPropertyResult.GeographyLevel2ID,
                GeographyLevel3Id = ivcPropertyResult.GeographyLevel3ID,
                Name = XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, NameXPath),
                OSReference = XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, OSReferencePath),
                PropertyReferenceId = ivcPropertyResult.PropertyReferenceID,
                Rating = string.IsNullOrEmpty(rating) ? "0.0" : rating,
                MainImage = XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, MainImageXPath),
                Images = new List<Image>(),
                ReviewAverageScore = XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, ReviewXPath),
                Summary = XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, SummaryXPath),
                URL = XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, URLXPath),
                VideoCode = XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, VideoCodeXPath),
                SubResults = new List<ISubResult>(),
                ProductAttributes = new List<string>(),
				Latitude = Functions.SafeNumeric(XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, LatitudeXPath)),
				Longitude = Functions.SafeNumeric(XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, LongitudeXPath))
			};

            try
            {
                XDocument xDoc = ivcPropertyResult.SearchResponseXML.ToXDocument();
                XElement element = xDoc.Element("Property")?.Element("Images");

                if (element != null)
                {
                    foreach (XElement xElement in element.Elements("Image"))
                    {
                        var image = new Image()
                        {
                            Title = (string)xElement.Element("Title"),
                            Url = (string)xElement.Element("URL")
                        };
                        propertyResult.Images.Add(image);
                    }
                }
                
                XElement productAttributeElement = xDoc.Element("Property")?.Element("ProductAttributes");
                if (productAttributeElement != null)
                { 
                    foreach (XElement xElement in productAttributeElement?.Elements("ProductAttribute"))
                    {
                        var attribute = (string)xElement.Element("ProductAttribute");
                        propertyResult.ProductAttributes.Add(attribute);
                    }
                }
            }
            catch (Exception ex)
            {
                Intuitive.FileFunctions.AddLogEntry("PropertyAdaptor", "No Response XML", ex.ToString());
            }


            foreach (SearchResponse.RoomType roomType in ivcPropertyResult.RoomTypes.OrderBy(rt => rt.Seq))
            {
                if (site.SiteConfiguration.BookingJourneyConfiguration.OnRequestDisplay != OnRequestDisplay.None
                    || !roomType.OnRequest)
                {
                    var roomOption = new RoomOption
                    {
                        BookingToken = roomType.RoomBookingToken,
                        InvalidFlightResultIds = roomType.InvalidFlightResults,
                        MealBasisId = roomType.MealBasisID,
                        OnRequest = roomType.OnRequest,
                        RoomType = roomType.RoomType,
                        RoomView = roomType.RoomView,
                        Sequence = roomType.Seq,
                        Source = roomType.Source,
                        TotalPrice = roomType.Total,
                        SupplierId = roomType.SupplierDetails.SupplierID
                    };

                    roomOption.ComponentToken = roomOption.GetHashCode();

                    propertyResult.SubResults.Add(roomOption);
                }
            }

            propertyResult.ComponentToken = propertyResult.GetHashCode();

            return propertyResult;
        }

        /// <summary>
        /// Sets the arrival date.
        /// </summary>
        /// <param name="date">The date.</param>
        public void SetArrivalDate(DateTime date)
        {
            this.arrivalDate = date;
        }

        /// <summary>
        /// Sets the duration.
        /// </summary>
        /// <param name="newDuration">The new duration.</param>
        public void SetDuration(int newDuration)
        {
            this.duration = newDuration;
        }
    }
}