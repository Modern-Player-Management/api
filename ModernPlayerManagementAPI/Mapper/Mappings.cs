using AutoMapper;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Models.DTOs;

namespace ModernPlayerManagementAPI.Mapper
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Team, TeamDTO>().ReverseMap();
            CreateMap<Event, EventDTO>().ReverseMap();
            CreateMap<User, UserProfileDTO>().ReverseMap();
            CreateMap<Game, GameDTO>().ReverseMap();
            CreateMap<PlayerStats, PlayerStatsDTO>().ReverseMap();
        }
    }
}