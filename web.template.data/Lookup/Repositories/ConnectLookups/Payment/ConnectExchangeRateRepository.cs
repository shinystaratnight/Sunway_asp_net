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
    public class ConnectExchangeRateRepository : ConnectLookupBase<ExchangeRate>, IExchangeRateRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectExchangeRateRepository"/> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public ConnectExchangeRateRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of Booking documentation</returns>
        protected override List<ExchangeRate> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("ExchangeRates");
            XDocument xDoc = xml.ToXDocument();
            var rates = new List<ExchangeRate>();

            foreach (XElement xElement in xDoc.Element("Lookups")?.Element("ExchangeRates").Elements("ExchangeRate"))
            {
                var rate = new ExchangeRate()
                               {
                                   CurrencyID = (int)xElement.Element("CurrencyID"), 
                                   Rate = (decimal)xElement.Element("ExchangeRate")
                               };
                rates.Add(rate);
            }

            return rates;
        }
    }
}