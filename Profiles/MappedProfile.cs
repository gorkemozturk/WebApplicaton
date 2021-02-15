using AutoMapper;
using WebApplication.Models;

namespace WebApplication.Profiles
{
    public class MappedProfile : Profile
    {
        public MappedProfile()
        {
            CreateMap<RoomEntity, Room>()
                .ForMember(d => d.Rate, o => o.MapFrom(src => src.Rate / 100.0m))
                .ForMember(d => d.Self, o => o.MapFrom(src => 
                    Link.To(nameof(Controllers.RoomsController.GetRoom), new { id = src.Id })
                ));
        }
    }
}
