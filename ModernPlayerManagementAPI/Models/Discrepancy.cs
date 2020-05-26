using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernPlayerManagementAPI.Models
{
    public class Discrepancy : BaseEntity
    {
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public Discrepancy.DiscrepancyType Type { get; set; }
        public string Reason { get; set; }
        public enum DiscrepancyType { Absence, Delay }
        public int DelayLength { get; set; }
    }
}