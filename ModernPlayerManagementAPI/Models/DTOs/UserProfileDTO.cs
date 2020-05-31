using System;

namespace ModernPlayerManagementAPI.Models.DTOs
{
    public class UserProfileDTO
    {
        public string Username { get; set; }
        public string Image { get; set; }
        public string Email { get; set; }
        public Guid CalendarSecret { get; set; }
    }
}