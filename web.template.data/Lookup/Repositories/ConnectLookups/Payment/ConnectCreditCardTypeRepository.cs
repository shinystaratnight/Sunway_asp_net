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
    public class ConnectCreditCardTypeRepository : ConnectLookupBase<CreditCardType>, ICreditCardTypeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectCreditCardTypeRepository"/> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public ConnectCreditCardTypeRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of Booking documentation</returns>
        protected override List<CreditCardType> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("CreditCardType");
            XDocument xDoc = xml.ToXDocument();
            var cardTypes = new List<CreditCardType>();

            XElement element = xDoc.Element("Lookups")?.Element("CreditCardTypes");
            if (element != null)
            {
                foreach (XElement xElement in element?.Elements("CreditCardType"))
                {
                    var cardType = new CreditCardType()
                                       {
                                           Id = (int)xElement.Element("CreditCardTypeID"), 
                                           Name = (string)xElement.Element("CreditCardType"), 
                                           SurchargePercentage =
                                               (decimal)xElement.Element("SurchargePercentage"), 
                                           SellingGeographyLevel1ID =
                                               (int)xElement.Element("SellingGeographyLevel1ID")
                                       };
                    cardTypes.Add(cardType);
                }
            }

            return cardTypes;
        }
    }
}