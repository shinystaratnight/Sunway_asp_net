namespace Web.Template.Data.Lookup.Repositories.ConnectLookups.Payment
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Linq;

    using Intuitive;

    using Web.Template.Data.Connect;
    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Payment;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Payment;

    /// <summary>
    ///     Booking documentation Repository that is responsible for managing access to airports
    /// </summary>
    public class ConnectCurrencyRepository : ConnectLookupBase<Currency>, ICurrencyRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectCurrencyRepository"/> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public ConnectCurrencyRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of Booking documentation</returns>
        protected override List<Currency> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("Currency");
            XDocument xDoc = xml.ToXDocument();
            var currencies = new List<Currency>();

            foreach (XElement xElement in xDoc.Element("Lookups")?.Element("Currencys").Elements("Currency"))
            {
                var currency = new Currency()
                                   {
                                       Id = (int)xElement.Element("CurrencyID"), 
                                       SellingCurrencyId = (int)xElement.Element("SellingCurrencyID"), 
                                       Name = (string)xElement.Element("Currency"), 
                                       CurrencyCode = (string)xElement.Element("CurrencyCode"), 
                                       Symbol = (string)xElement.Element("Symbol"), 
                                       CustomerSymbolOverride =
                                           (string)xElement.Element("CustomerSymbolOverride"), 
                                       SymbolPosition = (string)xElement.Element("SymbolPosition")
                                   };
                currencies.Add(currency);
            }

            return currencies;
        }
    }
}