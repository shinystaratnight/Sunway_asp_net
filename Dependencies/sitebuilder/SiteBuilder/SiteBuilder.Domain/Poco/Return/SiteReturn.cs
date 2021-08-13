namespace SiteBuilder.Domain.Poco.Return
{
    using System.Collections.Generic;

    public class SiteReturn
    {
        private List<string> sites = new List<string>();

        public List<string> Sites
        {
            get { return this.sites; }
            set { this.sites = value; }
        }
    }
}