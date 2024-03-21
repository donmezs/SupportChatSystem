using AutoMapper;
using SupportChatSystem.Application.DTOs;
using SupportChatSystem.Domain.Entities;

namespace SupportChatSystem.Application.MappingProfiles;
public class AgentMappingProfile : Profile
{
    public AgentMappingProfile()
    {
        CreateMap<AgentDto, Agent>().ReverseMap();
    }
}
