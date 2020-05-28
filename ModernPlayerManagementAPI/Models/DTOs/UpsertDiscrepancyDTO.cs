using System;

namespace ModernPlayerManagementAPI.Models.DTOs
{
    public class UpsertDiscrepancyDTO
    {
        public Discrepancy.DiscrepancyType Type { get; set; }
        public string Reason { get; set; }
        public int DelayLength { get; set; }
    }
}