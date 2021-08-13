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
    /// Class ConnectCreditCardSurchargeRepository.
    /// </summary>
    public class ConnectCreditCardSurchargeRepository : ConnectLookupBase<CreditCardSurcharge>, 
                                                        ICreditCardSurchargeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectCreditCardSurchargeRepository" /> class.
        /// </summary>
        /// <param name="asyncLookup">A class using intuitive async cache to get lookup xml from connect.</param>
        public ConnectCreditCardSurchargeRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of the entity</returns>
        protected override List<CreditCardSurcharge> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("CreditCardType");
            XDocument xdoc = xml.ToXDocument();

            var creditCardSurcharges = new List<CreditCardSurcharge>();

            XElement element = xdoc.Element("Lookups")?.Element("CreditCardTypes");
            if (element != null)
            {
                foreach (XElement xElement in element.Elements("CreditCardType"))
                {
                    var creditCardSurcharge = new CreditCardSurcharge()
                                                  {
                                                      CreditCardTypeId =
                                                          (int)
                                                          xElement.Element("CreditCardTypeID"), 
                                                      Id =
                                                          (int)
                                                          xElement.Element(
                                                              "CreditCardSurchargeID"), 
                                                      SellingGeographyLevel1Id =
                                                          (int)
                                                          xElement.Element(
                                                              "SellingGeographyLevel1ID"), 
                                                      SurchargePercentage =
                                                          (decimal)
                                                          xElement.Element("SurchargePercentage"), 
                                                      SurchargeType =
                                                          (string)
                                                          xElement.Element("SurchargeType"), 
                                                      UseCreditCard =
                                                          (bool)xElement.Element("UseCreditCard"), 
                                                  };
                    creditCardSurcharges.Add(creditCardSurcharge);
                }
            }

            return creditCardSurcharges;
        }
    }
}