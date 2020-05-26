using System.Collections.Generic;
using ModernPlayerManagementAPI.Models;

namespace ModernPlayerManagementAPI.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        User GetUserByUsername(string username);
        ICollection<User> findUsersByUsernameContains(string search);
    }
}