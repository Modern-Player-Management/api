using System.Collections.Generic;
using ModernPlayerManagementAPI.Models.DTOs;

namespace ModernPlayerManagementAPI.Models.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        User GetUserByUsername(string username);
        ICollection<User> findUsersByUsernameContains(string search);
    }
}