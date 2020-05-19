using System;

namespace ModernPlayerManagementAPI.Models.DTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}