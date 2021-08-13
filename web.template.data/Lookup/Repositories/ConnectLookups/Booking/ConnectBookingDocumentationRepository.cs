namespace Web.Template.Data.Lookup.Repositories.ConnectLookups.Booking
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Linq;

    using Intuitive;

    using Web.Template.Data.Connect;
    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;

    /// <summary>
    ///     Booking documentation Repository that is responsible for managing access to airports
    /// </summary>
    public class ConnectBookingDocumentationRepository : ConnectLookupBase<BookingDocumentation>, 
                                                         IBookingDocumentationRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectBookingDocumentationRepository"/> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public ConnectBookingDocumentationRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of Booking documentation</returns>
        protected override List<BookingDocumentation> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("BookingDocumentation");
            XDocument xDoc = xml.ToXDocument();
            var documents = new List<BookingDocumentation>();

            foreach (
                XElement xElement in
                    xDoc.Element("Lookups")?.Element("BookingDocumentations").Elements("BookingDocumentation"))
            {
                var document = new BookingDocumentation()
                                   {
                                       Id = (int)xElement.Element("BookingDocumentationID"), 
                                       DocumentName = (string)xElement.Element("DocumentName"), 
                                       Recipient = (string)xElement.Element("Recipient"), 
                                   };
                documents.Add(document);
            }

            return documents;
        }
    }
}