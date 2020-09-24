using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernPlayerManagementAPI.Models
{
    public class Discrepancy : BaseEntity
    {
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey("UserId")] public User User { get; set; }
        public DiscrepancyType Type { get; set; }
        public string Reason { get; set; }

        public enum DiscrepancyType
        {
            Absence,
            Delay
        }
        
        public bool IsUserDiscrepancyIssuer(Guid userId)
        {
            return UserId == userId;
        }

        public int DelayLength { get; set; }
    }
}