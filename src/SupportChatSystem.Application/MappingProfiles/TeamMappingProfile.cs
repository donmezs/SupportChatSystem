using AutoMapper;
using SupportChatSystem.Application.DTOs;
using SupportChatSystem.Domain.Entities;

namespace SupportChatSystem.Application.MappingProfiles;
public class TeamMappingProfile : Profile
{
    public TeamMappingProfile()
    {
        CreateMap<TeamDto, Team>().ReverseMap();
    }
}
