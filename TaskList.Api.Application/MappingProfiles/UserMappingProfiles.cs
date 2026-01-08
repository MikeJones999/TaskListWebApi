using AutoMapper;
using TaskList.Api.Domain.Users.DTOs.AuthModels;
using TaskList.Api.Domain.Users.DTOs.UserProfileModels;
using TaskList.Api.Domain.Users.Models.AuthenticationModels;

namespace TaskList.Api.Application.MappingProfiles
{
    public class UserMappingProfiles : Profile
    {
        public UserMappingProfiles()
        {
            CreateMap<ApplicationUser, UserProfileResponse>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));


            CreateMap<UserRegister, ApplicationUser>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.DisplayName) ? src.UserName : src.DisplayName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName));
        }
    }
}
