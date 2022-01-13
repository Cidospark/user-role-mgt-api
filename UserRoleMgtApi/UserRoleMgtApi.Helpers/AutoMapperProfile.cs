using System;
using AutoMapper;
using UserRoleMgtApi.Models;
using UserRoleMgtApi.Models.Dtos;

namespace UserRoleMgtApi.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserToReturnDto>();
            CreateMap<User, RegisterSuccessDto>()
                .ForMember(dest => dest.UserId, x => x.MapFrom(x => x.Id))
                .ForMember(d => d.FullName, x => x.MapFrom(x => $"{x.FirstName} {x.LastName}"));

            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(u => u.Email))
                .ForMember(dest => dest.Address, x => x.MapFrom(s => new Address { Street = s.Street, State = s.State, Country = s.Country }));

            CreateMap<PhotoUploadDto, Photo>();
            CreateMap<Photo, PhotoToReturnDto>();
        }
    }
}
