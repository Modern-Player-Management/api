using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernPlayerManagementAPI.Models
{
    public class Membership : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid TeamId { get; set; }
        [ForeignKey("UserId")] 
        public User User { get; set; }
        [ForeignKey("TeamId")] 
        public Team Team { get; set; }
    }
}