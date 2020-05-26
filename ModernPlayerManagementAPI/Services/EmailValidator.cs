using System.Net.Mail;

namespace ModernPlayerManagementAPI.Services
{
    public class EmailValidator : IEmailValidator
    {
        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}