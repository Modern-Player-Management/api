using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ModernPlayerManagementAPI.Database;
using ModernPlayerManagementAPI.Migrations;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Models.Repository;
using Moq;
using Xunit;

namespace ModernPlayerManagementAPITests
{
    public class TeamRepositoryTest
    {
        private ApplicationDbContext context;

        void setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ApplicationDatabase")
                .Options;

            this.context = new ApplicationDbContext(options);
        }

        [Fact]
        public void Get_Team_By_Id()
        {
            this.setup();

            // Given
            var manager = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};
            context.Users.Add(manager);

            var team = new Team
                {Id = Guid.NewGuid(), Created = DateTime.Now, ManagerId = manager.Id, Name = "Test Team"};
            context.Teams.Add(team);
            context.SaveChanges();

            // When 
            TeamRepository repo = new TeamRepository(context);
            Team testTea = repo.GetById(team.Id);

            // Then
            Assert.Equal("Test Team", testTea.Name);
            Assert.Equal(team.Id, testTea.Id);
            Assert.Equal(team.ManagerId, manager.Id);
        }

        [Fact]
        public void Get_Teams_Returns_Teams_In_Which_User_Is_Manager_Or_Member()
        {
            this.setup();

            // Given
            var manager = new User {Username = "manager", Email = "manager@manager.fr", Id = Guid.NewGuid()};
            var user = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};
            context.Users.Add(manager);
            context.Users.Add(user);

            var team1 = new Team
                {Id = Guid.NewGuid(), Created = DateTime.Now, ManagerId = user.Id, Name = "Test Team 1"};
            context.Teams.Add(team1);

            var team2 = new Team
            {
                Id = Guid.NewGuid(), Created = DateTime.Now, ManagerId = manager.Id, Name = "Test Team 2",
                Members = new List<User> {user}
            };
            context.Teams.Add(team2);

            context.SaveChanges();

            // When 
            TeamRepository repo = new TeamRepository(context);
            List<Team> result = repo.getUserTeams(user.Id).ToList();

            // Then
            Assert.Equal(2, result.Count);
        }
    }
}