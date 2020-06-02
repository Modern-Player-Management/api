using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ModernPlayerManagementAPI.Database;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Repositories;
using Moq;
using Xunit;

namespace ModernPlayerManagementAPITests
{
    public class EventRepositoryTest
    {
        [Fact]
        void GetById_Test()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"ApplicationDatabase{Guid.NewGuid()}")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                // Given
                var evt = new Event()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Event",
                    Description = "Test Event Description"
                };
                context.Events.Add(evt);

                context.SaveChanges();

                // When
                var repo = new EventRepository(context, new TeamRepository(context));
                var e = repo.GetById(evt.Id);

                // Then
                Assert.Equal("Test Event", e.Name);
                Assert.Equal("Test Event Description", e.Description);
            }
        }

        [Fact]
        void GetUserFuturEvents_Test()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"ApplicationDatabase{Guid.NewGuid()}")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                // Given
                var manager = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};
                var manager2 = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};
                context.Users.Add(manager);
                context.Users.Add(manager2);

                var team1 = new Team
                    {Id = Guid.NewGuid(), Created = DateTime.Now, ManagerId = manager.Id, Name = "Test Team 1"};
                context.Teams.Add(team1);
                var team2 = new Team
                    {Id = Guid.NewGuid(), Created = DateTime.Now, ManagerId = manager.Id, Name = "Test Team 2"};
                context.Teams.Add(team2);
                var team3 = new Team
                    {Id = Guid.NewGuid(), Created = DateTime.Now, ManagerId = manager2.Id, Name = "Test Team 2"};
                context.Teams.Add(team3);


                var event1 = new Event()
                {
                    Created = DateTime.Now, Description = "Test Event Description",
                    Discrepancies = new List<Discrepancy>(),
                    End = DateTime.Now.AddDays(7), Id = Guid.NewGuid(), Name = "Test Event Found",
                    Participations = new List<Participation>(), Start = DateTime.Now.AddDays(2),TeamId = team1.Id,
                    Type = Event.EventType.Coaching
                };
                
                var event2 = new Event()
                {
                    Created = DateTime.Now, Description = "Test Event Description",
                    Discrepancies = new List<Discrepancy>(),
                    End = DateTime.Now.AddDays(7), Id = Guid.NewGuid(), Name = "Test Event",
                    Participations = new List<Participation>(), Start = DateTime.Now.AddDays(2),TeamId = team3.Id,
                    Type = Event.EventType.Coaching
                };
                
                var event3 = new Event()
                {
                    Created = DateTime.Now, Description = "Test Event Description",
                    Discrepancies = new List<Discrepancy>(),
                    End = DateTime.Now.AddDays(7), Id = Guid.NewGuid(), Name = "Test Event",
                    Participations = new List<Participation>(), Start = DateTime.Now.AddDays(-2),TeamId = team3.Id,
                    Type = Event.EventType.Coaching
                };

                context.Add(event1);
                context.Add(event2);
                context.Add(event3);
                
                context.SaveChanges();
                
                // When
                var repo = new EventRepository(context, new TeamRepository(context));
                var e = repo.GetUserFutureEvents(manager.Id);

                // Then
                Assert.Single(e);
                Assert.Equal("Test Event Found", e.First().Name);
            }
        }
    }
}