using AutoMapper;
using SupportChatSystem.Application.DTOs;
using SupportChatSystem.Domain.Entities;

namespace SupportChatSystem.Application.MappingProfiles;
public class ChatSessionMappingProfile : Profile
{
    public ChatSessionMappingProfile()
    {
        CreateMap<ChatSessionDto, ChatSession>().ReverseMap();
    }
}
