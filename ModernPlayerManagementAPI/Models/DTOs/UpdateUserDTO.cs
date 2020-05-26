using System;

namespace ModernPlayerManagementAPI.Models.DTOs
{
    public class UpdateUserDTO
    {
        public Guid Id { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
    }
}