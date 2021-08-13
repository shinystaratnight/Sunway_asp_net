namespace DealFinder.Request
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using DealFinder.Response;
    using MediatR;

    public class LookupsRequest : IRequest<LookupsResponse>
    {
        public List<string> Lookups { get; set; }
    }
}
