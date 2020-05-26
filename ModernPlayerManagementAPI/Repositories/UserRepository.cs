using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ModernPlayerManagementAPI.Database;
using ModernPlayerManagementAPI.Models;

namespace ModernPlayerManagementAPI.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public User GetUserByUsername(string username)
        {
            return (from user in this._context.Users.Include(user => user.Memberships)
                    .ThenInclude(membership => membership.Team)
                where user.Username == username
                select user).First();
        }

        public ICollection<User> findUsersByUsernameContains(string search)
        {
            return (from user in this._context.Users where user.Username.Contains(search) select user).ToList();
        }
    }
}