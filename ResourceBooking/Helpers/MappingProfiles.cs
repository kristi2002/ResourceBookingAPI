using AutoMapper;
using ResourceBooking.Dtos;
using ResourceBooking.Models;

namespace ResourceBooking.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserForCreationDto, User>();
            CreateMap<Resource, ResourceDto>()
                .ForMember(dest => dest.ResourceTypeName, opt => opt.MapFrom(src => src.ResourceType.TypeName));
            CreateMap<ResourceForCreationDto, Resource>();
            CreateMap<Booking, BookingDto>();
            CreateMap<BookingForCreationDto, Booking>();
            CreateMap<ResourceType, ResourceTypeDto>();
            CreateMap<ResourceTypeForCreationDto, ResourceType>();
        }
    }
}
