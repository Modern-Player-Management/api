using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernPlayerManagementAPI.Models.DTOs
{
    public class DiscrepancyDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public Discrepancy.DiscrepancyType Type { get; set; }
        public string Reason { get; set; }
        public enum DiscrepancyType { Absence, Delay }
        public int DelayLength { get; set; }
    }
}