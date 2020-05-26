using System;
using System.Collections.Generic;
using System.Linq;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Models.DTOs;
using ModernPlayerManagementAPI.Repositories;
using ModernPlayerManagementAPI.Services;
using Moq;
using Xunit;

namespace ModernPlayerManagementAPITests
{
    public class EventServiceTest
    {
        private List<Event> events;
        private EventService _eventService;

        void setup()
        {
            events = new List<Event>();

            var eventRepoMock = new Mock<IEventRepository>();
            var teamServiceMock = new Mock<ITeamService>();

            eventRepoMock.Setup(mock => mock.GetById(It.IsAny<Guid>()))
                .Returns<Guid>(eventId => this.events.Find(evt => evt.Id == eventId));

            eventRepoMock.Setup(mock => mock.Update(It.IsAny<Event>()))
                .Callback<Event>(evt =>
                {
                    this.events.Remove(this.events.Find(e => e.Id == evt.Id));
                    this.events.Add(evt);
                });

            eventRepoMock.Setup(mock => mock.Delete(It.IsAny<Guid>()))
                .Callback<Guid>(eventId => this.events.Remove(this.events.Find(e => e.Id == eventId)));

            this._eventService = new EventService(eventRepoMock.Object, teamServiceMock.Object);
        }

        [Fact]
        void ConfirmEvent_Test()
        {
            this.setup();
            
            // Given
            var user = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};
            var evt = new Event()
            {
                Id = Guid.NewGuid(),
                Name = "Test Event",
                Description = "Test Event Description",
                Start = DateTime.Now,
                End = DateTime.Now,
                Discrepancies = new List<Discrepancy>()
                    {new Discrepancy() {Created = DateTime.Now, Reason = "Test", User = user}},
                Participations = new List<Participation>()
                    {new Participation() {Created = DateTime.Now, User = user, UserId = user.Id}},
                Created = DateTime.Now,
                Type = Event.EventType.Coaching
            };
            
            events.Add(evt);
            
            // When
            this._eventService.ConfirmEvent(evt.Id,user.Id);
            
            // Then
            Assert.Equal(true, this.events.First().Participations.First().Confirmed);
        }

        [Fact]
        void AddDiscrepancy_Test()
        {
            this.setup();
            
            // Given
            var user = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};
            var evt = new Event()
            {
                Id = Guid.NewGuid(),
                Name = "Test Event",
                Description = "Test Event Description",
                Start = DateTime.Now,
                End = DateTime.Now,
                Discrepancies = new List<Discrepancy>(),
                Participations = new List<Participation>()
                    {new Participation() {Created = DateTime.Now, User = user, UserId = user.Id}},
                Created = DateTime.Now,
                Type = Event.EventType.Coaching
            };
            
            events.Add(evt);
            
            var dto = new UpsertDiscrepancyDTO()
            {
                Reason = "Test reason",
                Type = Discrepancy.DiscrepancyType.Delay,
                DelayLength = 15,
                UserId = user.Id
            };
            
            // When
            this._eventService.AddDiscrepancy(evt.Id,dto, user.Id);

            // Then
            Assert.Equal(1,evt.Discrepancies.Count);
            Assert.Equal( "Test reason",evt.Discrepancies.First().Reason);
            
        }

        [Fact]
        void UpdateEvent_Test()
        {
            this.setup();
            
            // Given
            var evt = new Event()
            {
                Id = Guid.NewGuid(),
                Name = "Test Event",
                Description = "Test Event Description",
                Start = DateTime.Now,
                End = DateTime.Now,
                Discrepancies = new List<Discrepancy>(),
                Participations = new List<Participation>()
                    {new Participation() {Created = DateTime.Now}},
                Created = DateTime.Now,
                Type = Event.EventType.Coaching
            };
            this.events.Add(evt);
            
            var dto = new UpsertEventDTO()
            {
                Name = "Test Event",
                Description = "Test Event Description Updated",
                Start = DateTime.Now,
                End = DateTime.Now,
                Type = Event.EventType.Coaching
            };
            
            // When
            this._eventService.UpdateEvent(dto, evt.Id);

            // Then
            Assert.Equal("Test Event Description Updated", this.events.First().Description);
        }
    }
}