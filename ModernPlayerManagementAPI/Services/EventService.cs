using System;
using System.Collections.Generic;
using System.Linq;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Models.DTOs;
using ModernPlayerManagementAPI.Repositories;
using ModernPlayerManagementAPI.Services.Interfaces;

namespace ModernPlayerManagementAPI.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ITeamService _teamService;
        private readonly IMailService mailService;
        private readonly ITeamRepository teamRepository;
        private readonly IUserRepository _userRepository;

        public EventService(IEventRepository eventRepository, ITeamService teamService, IMailService mailService,
            ITeamRepository teamRepository, IUserRepository userRepository)
        {
            _eventRepository = eventRepository;
            _teamService = teamService;
            this.mailService = mailService;
            this.teamRepository = teamRepository;
            this._userRepository = userRepository;
        }

        public bool IsUserTeamManager(Guid eventId, Guid userId)
        {
            var evt = this._eventRepository.GetById(eventId);
            return this._teamService.IsUserTeamManager(evt.TeamId, userId);
        }
            
        public Calendar GetUserCalendar(Guid icalSecret)
        {
            var user = this._userRepository.GetByICalSecret(icalSecret);
            var events = this._eventRepository.GetUserFutureEvents(user.Id);
            
             var calendarEvents = events.Select(evt => new CalendarEvent()
             {
                 Start = new CalDateTime(evt.Start),
                 End = new CalDateTime(evt.End),
                 Summary = evt.Name,
                 Attendees = evt.Participations.Select(p => new Attendee(){CommonName = p.User.Username, Value = new Uri($"mailto:{p.User.Email}")}).ToList(),
                 Description = evt.Description
             });
            
             var calendar = new Calendar();
             foreach (var calendarEvent in calendarEvents)
             {
                 calendar.Events.Add(calendarEvent);
             }

             return calendar;
        }

        public void SetPresence(Guid eventId, Guid userId,EventPresenceDTO dto)
        {
            var evt = this._eventRepository.GetById(eventId);
            var participation = evt.Participations.First(part => part.UserId == userId);

            participation.Confirmed = dto.Present;

            this._eventRepository.Update(evt);
        }

        public void AddDiscrepancy(Guid eventId, UpsertDiscrepancyDTO dto, Guid userId)
        {
            var evt = this._eventRepository.GetById(eventId);
            if (evt.Discrepancies.Select(d => d.UserId).Contains(userId))
            {
                throw new ArgumentException("You already have a discrepancy on this event");
            }
            var discrepancy = new Discrepancy()
            {
                Created = DateTime.Now,
                Reason = dto.Reason,
                Type = dto.Type,
                DelayLength = dto.DelayLength,
                EventId = eventId,
                UserId = userId
            };

            evt.Discrepancies ??= new List<Discrepancy>();
            evt.Discrepancies.Add(discrepancy);

            SendNotification(dto, userId, evt);

            this._eventRepository.Update(evt);
        }

        private void SendNotification(UpsertDiscrepancyDTO dto, Guid userId, Event evt)
        {
            var team = this.teamRepository.GetById(evt.TeamId);
            var manager = team.Manager;
            var currentUser = team.Players.First(membership => membership.UserId == userId).User;
            var body =
                $"{currentUser.Username} has issued a {dto.Type} {(dto.Type == Discrepancy.DiscrepancyType.Delay ? $"( {dto.DelayLength} )" : "")} " +
                $"for the event {evt.Name} ({evt.Start} - {evt.End})";

            this.mailService.SendMail(manager.Username, manager.Email, "Discrepancy Notification", body);
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