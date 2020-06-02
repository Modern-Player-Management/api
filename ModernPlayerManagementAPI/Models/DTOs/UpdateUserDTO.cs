using System;

namespace ModernPlayerManagementAPI.Models.DTOs
{
    public class UpdateUserDTO
    {
        public string Password { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
    }
}