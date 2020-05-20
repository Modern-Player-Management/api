using System.Linq;
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
            return (from user in this._context.Users where user.Username == username select user).First();
        }
    }
}