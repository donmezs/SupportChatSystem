using AutoMapper;
using SupportChatSystem.Application.DTOs;
using SupportChatSystem.Domain.Entities;

namespace SupportChatSystem.Application.MappingProfiles;
public  class ShiftMappingProfile : Profile
{
    public ShiftMappingProfile()
    {
        CreateMap<ShiftDto, Shift>().ReverseMap();
    }
}
