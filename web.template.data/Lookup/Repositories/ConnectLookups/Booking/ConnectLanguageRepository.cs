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
    public class ConnectLanguageRepository : ConnectLookupBase<Language>, ILanguageRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectLanguageRepository" /> class.
        /// </summary>
        /// <param name="asyncLookup">A class using intuitive async cache to get lookup xml from connect.</param>
        public ConnectLanguageRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of Booking documentation</returns>
        protected override List<Language> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("Language");
            XDocument xDoc = xml.ToXDocument();
            var languages = new List<Language>();

            foreach (XElement xElement in xDoc.Element("Lookups")?.Element("Languages").Elements("Language"))
            {
                var language = new Language()
                                   {
                                       Id = (int)xElement.Element("LanguageID"), 
                                       CultureCode = (string)xElement.Element("CultureCode"), 
                                       CustomerLanguage = (bool)xElement.Element("CustomerLanguage"), 
                                       DefaultLanguage = (bool)xElement.Element("DefaultLanguage"), 
                                       LanguageCode = (string)xElement.Element("LanguageCode"), 
                                       Name = (string)xElement.Element("Language"), 
                                       SystemLanguage = (bool)xElement.Element("SystemLanguage")
                                   };
                languages.Add(language);
            }

            return languages;
        }
    }
}