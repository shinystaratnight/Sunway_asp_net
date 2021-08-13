namespace Web.Booking
{
    using AutoMapper;

    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Basket.Models.Components.SubComponent;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Results.ResultModels;

    using FlightSector = Web.Template.Application.Basket.Models.Components.SubComponent.FlightSector;

    /// <summary>
    ///     Class that manages our automapper bindings used to specify maps that automapper should have available.
    /// </summary>
    /// <seealso cref="AutoMapper.Profile" />
    public class AutoMapperConfig : Profile
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AutoMapperConfig" /> class.
        /// </summary>
        public AutoMapperConfig()
        {
            this.CreateMap<Web.Template.Application.Results.ResultModels.FlightSector, FlightSector>();
            this.CreateMap<RoomOption, Room>();
            this.CreateMap<ISubResult, Room>();

            this.CreateMap<Web.Template.Application.Results.ResultModels.ExtraOption, Web.Template.Application.Basket.Models.Components.SubComponent.ExtraOption>();
            this.CreateMap<ISubResult, Web.Template.Application.Basket.Models.Components.SubComponent.ExtraOption>();

            this.CreateMap<PropertyResult, Hotel>().ForMember(c => c.SubComponents, opt => opt.Ignore());
            this.CreateMap<FlightResult, Flight>().ForMember(c => c.SubComponents, opt => opt.MapFrom(c => c.SubResults));
            this.CreateMap<TransferResult, Transfer>().ConstructUsingServiceLocator();
            this.CreateMap<ExtraResult, Extra>().ForMember(c => c.SubComponents, opt => opt.MapFrom(c => c.SubResults));

            this.CreateMap<IResult, Hotel>().ForMember(c => c.SubComponents, opt => opt.MapFrom(c => c.SubResults));
            this.CreateMap<IResult, Flight>().ForMember(c => c.SubComponents, opt => opt.MapFrom(c => c.SubResults));
            this.CreateMap<IResult, Transfer>().ConstructUsingServiceLocator();
            this.CreateMap<IResult, Extra>().ForMember(c => c.SubComponents, opt => opt.MapFrom(c => c.SubResults));

            this.CreateMap<ISubResult, ISubComponent>();
        }
    }
}