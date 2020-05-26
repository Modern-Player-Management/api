using System;

namespace ModernPlayerManagementAPI.Models.DTOs
{
    public class ParticipationDTO
    {
        public bool Confirmed { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
    }
}