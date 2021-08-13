namespace SiteBuilder.Domain.Poco.Return
{
    using System.Collections.Generic;

    public class InstancesReturn
    {
        private List<string> instances = new List<string>();

        public List<string> Instances
        {
            get { return this.instances; }
            set { this.instances = value; }
        }
    }
}
