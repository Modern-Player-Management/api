namespace ModernPlayerManagementAPI.Services
{
    public interface IEmailValidator
    {
        bool IsValidEmail(string email);
    }
}