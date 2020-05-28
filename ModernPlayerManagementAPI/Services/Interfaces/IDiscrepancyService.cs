using System;
using ModernPlayerManagementAPI.Models.DTOs;

namespace ModernPlayerManagementAPI.Services.Interfaces
{
    public interface IDiscrepancyService
    {
        bool IsUserDiscrepancyIssuer(Guid userId, Guid discrepancyId);
        void DeleteDiscrepancy(Guid discrepancyId);
        void UpdateDiscrepancy(Guid discrepancyId, UpsertDiscrepancyDTO dto);
    }
}