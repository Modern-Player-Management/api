using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernPlayerManagementAPI.Models
{
    public class Participation : BaseEntity
    {
        public bool Confirmed { get; set; }
        [Required]
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey("UserId")] public User User { get; set; }
    }
}