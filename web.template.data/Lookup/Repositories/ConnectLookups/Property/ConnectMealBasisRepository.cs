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
    ///     Airport Repository that is responsible for managing access to airports
    /// </summary>
    public class ConnectMealBasisRepository : ConnectLookupBase<MealBasis>, IMealBasisRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectMealBasisRepository"/> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public ConnectMealBasisRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of MealBasis</returns>
        protected override List<MealBasis> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("MealBasis");
            XDocument xDoc = xml.ToXDocument();
            var mealBases = new List<MealBasis>();

            foreach (XElement xElement in xDoc.Element("Lookups")?.Element("MealBases").Elements("MealBasis"))
            {
                var mealBasis = new MealBasis()
                                    {
                                        Id = (int)xElement.Element("MealBasisID"), 
                                        MealBasisCode = (string)xElement.Element("MealBasisCode"), 
                                        Name = (string)xElement.Element("MealBasis")
                                    };
                mealBases.Add(mealBasis);
            }

            return mealBases;
        }
    }
}