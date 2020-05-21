using System;

namespace ModernPlayerManagementAPI.Models.DTOs
{
    public class LoggedUserDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string Image { get; set; }
    }
}