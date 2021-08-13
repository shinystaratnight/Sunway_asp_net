namespace Web.Template.Data.Lookup.Repositories.Flight
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Linq;

    using Intuitive;

    using Web.Template.Data.Connect;
    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Property;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Property;

    /// <summary>
    ///     Filter facility Repository that is responsible for managing access to airports
    /// </summary>
    public class ConnectFilterFacilityRepository : ConnectLookupBase<FilterFacility>, IFilterFacilityRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectFilterFacilityRepository"/> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public ConnectFilterFacilityRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of Filter Facility</returns>
        protected override List<FilterFacility> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("FilterFacility");
            XDocument xDoc = xml.ToXDocument();
            var facilities = new List<FilterFacility>();

            foreach (
                XElement xElement in xDoc.Element("Lookups")?.Element("FilterFacilities").Elements("FilterFacility"))
            {
                var facility = new FilterFacility()
                                   {
                                       Id = (int)xElement.Element("FilterFacilityID"), 
                                       Name = (string)xElement.Element("FilterFacility"), 
                                       Priority = (int)xElement.Element("Priority")
                                   };
                facilities.Add(facility);
            }

            return facilities;
        }
    }
}