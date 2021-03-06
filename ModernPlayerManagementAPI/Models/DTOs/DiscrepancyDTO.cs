﻿using System;

namespace ModernPlayerManagementAPI.Models.DTOs
{
    public class DiscrepancyDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public Discrepancy.DiscrepancyType Type { get; set; }
        public string Reason { get; set; }
        public int DelayLength { get; set; }
    }
}