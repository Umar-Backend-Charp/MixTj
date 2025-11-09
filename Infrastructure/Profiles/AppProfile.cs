using AutoMapper;
using Domain.DTO.Auth;
using Domain.Entities;

namespace Infrastructure.Profiles;

public class AppProfile : Profile
{
    public AppProfile()
    {
        CreateMap<Register, AppUser>();
    }
}