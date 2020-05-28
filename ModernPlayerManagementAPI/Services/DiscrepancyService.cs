using System;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Models.DTOs;
using ModernPlayerManagementAPI.Repositories;
using ModernPlayerManagementAPI.Services.Interfaces;

namespace ModernPlayerManagementAPI.Services
{
    public class DiscrepancyService : IDiscrepancyService
    {
        private readonly IRepository<Discrepancy> discrepancyRepository;

        public DiscrepancyService(IRepository<Discrepancy> discrepancyRepository)
        {
            this.discrepancyRepository = discrepancyRepository;
        }

        public bool IsUserDiscrepancyIssuer(Guid userId, Guid discrepancyId)
        {
            var discrepancy = this.discrepancyRepository.GetById(discrepancyId);
            return discrepancy.UserId == userId;
        }

        public void DeleteDiscrepancy(Guid discrepancyId)
        {
            this.discrepancyRepository.Delete(discrepancyId);
        }

        public void UpdateDiscrepancy(Guid discrepancyId, UpsertDiscrepancyDTO dto)
        {
            var discrepancy = this.discrepancyRepository.GetById(discrepancyId);
            discrepancy.Reason = dto.Reason;
            discrepancy.Type = dto.Type;
            discrepancy.DelayLength = dto.DelayLength;
            
            this.discrepancyRepository.Update(discrepancy);
        }
    }
}