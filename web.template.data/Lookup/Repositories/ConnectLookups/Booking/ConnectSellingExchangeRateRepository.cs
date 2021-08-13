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
    /// Class ConnectSellingExchangeRateRepository.
    /// </summary>
    public class ConnectSellingExchangeRateRepository : ConnectLookupBase<SellingExchangeRate>, 
                                                        ISellingExchangeRateRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectSellingExchangeRateRepository" /> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public ConnectSellingExchangeRateRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of the entity</returns>
        protected override List<SellingExchangeRate> Setup()
        {
            XmlDocument lookupXml = this.GetLookupsXml("SellingExchangeRates");
            XDocument lookupXDoc = lookupXml.ToXDocument();

            var sellingExchangeRates = new List<SellingExchangeRate>();

            XElement element = lookupXDoc.Element("Lookups")?.Element("SellingExchangeRates");

            if (element != null)
            {
                foreach (XElement xElement in element.Elements("SellingExchangeRate"))
                {
                    var sellingExchangeRate = new SellingExchangeRate()
                                                  {
                                                      Id =
                                                          (int)
                                                          xElement.Element("SellingCurrencyID"), 
                                                      CurrencyId =
                                                          (int)xElement.Element("CurrencyID"), 
                                                      Rate =
                                                          (decimal)
                                                          xElement.Element("ExchangeRate")
                                                  };
                    sellingExchangeRates.Add(sellingExchangeRate);
                }
            }

            return sellingExchangeRates;
        }
    }
}