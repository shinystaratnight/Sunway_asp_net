namespace Web.Template
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
            this.CreateMap<PropertyResult, Hotel>();
            this.CreateMap<FlightResult, Flight>();
            this.CreateMap<TransferResult, Transfer>();
            this.CreateMap<Application.Results.ResultModels.FlightSector, FlightSector>();
            this.CreateMap<RoomOption, Room>();
            this.CreateMap<ISubResult, Room>();

            this.CreateMap<IResult, IBasketComponent>().ConstructUsing(
                (IResult result) =>
                    {
                        if (result is PropertyResult)
                        {
                            return Mapper.Map<Hotel>(result);
                        }

                        if (result is FlightResult)
                        {
                            return Mapper.Map<Flight>(result);
                        }

                        if (result is TransferResult)
                        {
                            return Mapper.Map<Transfer>(result);
                        }

                        return null;
                    }).ForMember(c => c.SubComponents, opt => opt.MapFrom(c => c.SubResults));

            this.CreateMap<ISubResult, ISubComponent>().ConstructUsing(
                (ISubResult result) =>
                    {
                        if (result is RoomOption)
                        {
                            return Mapper.Map<Room>(result);
                        }

                        return null;
                    }).MaxDepth(5);

            this.CreateMap<IResult, Hotel>();
            this.CreateMap<IResult, Flight>();
            this.CreateMap<IResult, Transfer>();
        }
    }
}