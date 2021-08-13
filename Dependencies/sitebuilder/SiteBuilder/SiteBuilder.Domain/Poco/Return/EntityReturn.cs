namespace SiteBuilder.Domain.Poco.Return
{
    using System.Collections.Generic;

    public class EntityReturn
    {
        private List<string> languages = new List<string>();

        public List<string> Languages
        {
            get { return this.languages; }
            set { this.languages = value; }
        }

        public string Name { get; set; }

        public string JsonSchema { get; set; }

        public string Type { get; set; }
    }
}
