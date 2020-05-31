namespace ModernPlayerManagementAPI.Services.Interfaces
{
    public interface IMailService
    {
        void SendMail(string username,string email, string subject, string body);
    }
}