using System;
using System.Linq;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Models.DTOs;
using ModernPlayerManagementAPI.Repositories;

namespace ModernPlayerManagementAPI.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ITeamService _teamService;

        public EventService(IEventRepository eventRepository, ITeamService teamService)
        {
            _eventRepository = eventRepository;
            _teamService = teamService;
        }

        public bool IsUserTeamManager(Guid eventId, Guid userId)
        {
            var evt = this._eventRepository.GetById(eventId);
            return this._teamService.IsUserTeamManager(evt.TeamId, userId);
        }

        public void ConfirmEvent(Guid eventId, Guid userId)
        {
            var evt = this._eventRepository.GetById(eventId);
            var participation = evt.Participations.First(part => part.UserId == userId);

            participation.Confirmed = true;

            this._eventRepository.Update(evt);
        }

        public void AddDiscrepancy(Guid eventId, UpsertDiscrepancyDTO dto, Guid userId)
        {
            var evt = this._eventRepository.GetById(eventId);
            var discrepancy = new Discrepancy()
            {
                Created = DateTime.Now,
                Reason = dto.Reason,
                Type = dto.Type,
                DelayLength = dto.DelayLength,
                EventId = eventId,
                UserId = userId
            };

            evt.Discrepancies.Add(discrepancy);

            this._eventRepository.Update(evt);
        }

        public void UpdateEvent(UpsertEventDTO dto, Guid eventId)
        {
            var evt = _eventRepository.GetById(eventId);

            evt.Name = dto.Name;
            evt.Description = dto.Description;
            evt.Start = dto.Start;
            evt.End = dto.End;
            evt.Type = dto.Type;

            this._eventRepository.Update(evt);
        }

        public void DeleteEvent(Guid eventId)
        {
            this._eventRepository.Delete(eventId);
        }
    }
}