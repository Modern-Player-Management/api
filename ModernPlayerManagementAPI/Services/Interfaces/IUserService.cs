using System.Collections;
using System.Collections.Generic;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Models.DTOs;

namespace ModernPlayerManagementAPI.Services
{
    public interface IUserService
    {
        bool IsUniqueUser(string username);
        User Authenticate(string username, string password);
        User Register(string username, string email, string password);
        void Update(UpdateUserDTO dto);
        ICollection<UserDTO> SearchUser(string search);
        UserDTO GetFromUsername(string username);
    }
}