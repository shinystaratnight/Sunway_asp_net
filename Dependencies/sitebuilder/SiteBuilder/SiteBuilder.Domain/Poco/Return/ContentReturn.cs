namespace SiteBuilder.Domain.Poco.Return
{
    using System.Collections.Generic;

    public class ContentReturn
    {
        private List<ContentReturnContext> contexts = new List<ContentReturnContext>();

        public List<ContentReturnContext> Contexts
        {
            get { return this.contexts; }
            set { this.contexts = value; }
        }
    }
}
