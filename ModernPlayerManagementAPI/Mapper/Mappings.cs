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
        }
    }
}