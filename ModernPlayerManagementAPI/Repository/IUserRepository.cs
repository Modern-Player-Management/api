namespace ModernPlayerManagementAPI.Models.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        User GetUserByUsername(string username);
    }
}