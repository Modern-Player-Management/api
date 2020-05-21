using System.Linq;
using Microsoft.EntityFrameworkCore;
using ModernPlayerManagementAPI.Database;

namespace ModernPlayerManagementAPI.Models.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public User GetUserByUsername(string username)
        {
            return (from user in this._context.Users.Include(user => user.Memberships).ThenInclude(membership => membership.Team) where user.Username == username select user).First();
        }
    }
}