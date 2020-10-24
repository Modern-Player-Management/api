using System;
using System.Collections.Generic;
using System.Linq;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Models.DTOs;
using ModernPlayerManagementAPI.Repositories;
using ModernPlayerManagementAPI.Services;
using ModernPlayerManagementAPI.Services.Interfaces;
using Moq;
using Xunit;

namespace ModernPlayerManagementAPITests
{
    public class EventServiceTest
    {
        private List<Event> events;
        private List<Team> teams;
        private List<User> users;
        private EventService _eventService;

        public EventServiceTest()
        {
            events = new List<Event>();
            teams = new List<Team>();
            users = new List<User>();

            var eventRepoMock = new Mock<IEventRepository>();
            var teamServiceMock = new Mock<ITeamService>();
            var mailServiceMock = new Mock<IMailService>();
            var teamRepository = new Mock<ITeamRepository>();
            var userRepository = new Mock<IUserRepository>();

            teamRepository.Setup(mock => mock.GetById(It.IsAny<Guid>()))
                .Returns<Guid>(teamId =>
                {
                    var team = this.teams.Find(team => team.Id == teamId);
                    if (team.ManagerId != null)
                    {
                        team.Manager = this.users.Find(u => u.Id == team.ManagerId);
                    }

                    team.Players ??= new List<Membership>();

                    foreach (var membership in team.Players)
                    {
                        membership.User = this.users.First(user => user.Id == membership.UserId);
                        membership.Team = this.teams.First(team => team.Id == membership.TeamId);
                    }
                    
                    team.Games ??= new List<Game>();
                    team.Events ??= new List<Event>();

                    return team;
                });
            
            teamServiceMock.Setup(mock => mock.IsUserTeamManager(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns<Guid,Guid>((teamId,
                userId) =>
            {
                var team = teamRepository.Object.GetById(teamId);
                return team.ManagerId == userId;
            });

            
                
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

            teamRepository.Setup(mock => mock.GetById(It.IsAny<Guid>()))
                .Returns(() =>
                {
                    var user1 = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};
                    var user2 = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.Empty};
                    var team = new Team
                    {
                        Id = Guid.NewGuid(), Created = DateTime.Now, Name = "Test Team",
                        Players = new List<Membership>() { }, Events = new List<Event>(),
                        ManagerId = user1.Id,
                        Manager = user1
                    };

                    team.Players.Add(new Membership() {TeamId = team.Id, UserId = user2.Id, User = user2});

                    return team;
                });

            userRepository = new Mock<IUserRepository>();
            
            this._eventService = new EventService(eventRepoMock.Object, teamServiceMock.Object, mailServiceMock.Object,
                teamRepository.Object, userRepository.Object);
        }

        [Fact]
        void ConfirmEvent_Test()
        {
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
            this._eventService.SetPresence(evt.Id, user.Id, new EventPresenceDTO(){Present = true});

            // Then
            Assert.True(this.events.First().Participations.First().Confirmed);
        }

        [Fact]
        void AddDiscrepancy_Test()
        {
            // Given
            var user = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.Empty};
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
            };

            // When
            this._eventService.AddDiscrepancy(evt.Id, dto, user.Id);

            // Then
            Assert.Equal(1, evt.Discrepancies.Count);
            Assert.Equal("Test reason", evt.Discrepancies.First().Reason);
        }

        [Fact]
        void UpdateEvent_Test()
        {
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

        [Fact]
        void DeleteEvent_Test()
        {
            // Given
            var user = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.Empty};
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
            
            // When
            this._eventService.DeleteEvent(evt.Id);
            
            // Then
            Assert.Empty(this.events);
        }

        [Fact(Skip = "TODO")]
        void IsUserTeamManager_Test()
        {
            // Given
            var user = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};
            this.users.Add(user);
            
            var team = new Team
            {
                Id = Guid.NewGuid(), Created = DateTime.Now, Name = "Test Team",
                Players = new List<Membership>() { }, Events = new List<Event>(),
                ManagerId = user.Id,
                Manager = user
            };
            this.teams.Add(team);
            
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
                TeamId = team.Id,
                Type = Event.EventType.Coaching
            };
            this.events.Add(evt);
            
            // When
            var result = this._eventService.IsUserTeamManager(evt.Id, user.Id);
            
            // Then
            Assert.True(result);
        }
    }
}