namespace SiteBuilder.Domain.Poco.Return
{
    using System.Collections.Generic;

    public class LookupsReturn
    {
        private List<string> lookups = new List<string>();

        public List<string> Lookups
        {
            get { return this.lookups; }
            set { this.lookups = value; }
        }
    }
}
