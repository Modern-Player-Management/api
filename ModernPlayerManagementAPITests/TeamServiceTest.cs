using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ModernPlayerManagementAPI.Mapper;
using ModernPlayerManagementAPI.Migrations;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Models.DTOs;
using ModernPlayerManagementAPI.Models.Repository;
using ModernPlayerManagementAPI.Services;
using Moq;
using Xunit;

namespace ModernPlayerManagementAPITests
{
    public class TeamServiceTest
    {
        private List<Team> teams;
        private List<User> users;
        private TeamService teamService;

        private void setup()
        {
            teams = new List<Team>();
            this.users = new List<User>();

            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(mock => mock.Insert(It.IsAny<Team>())).Callback<Team>(team =>
            {
                team.Members ??= new List<User>();
                teams.Add(team);
            });
            teamRepository.Setup(mock => mock.getTeam(It.IsAny<Guid>()))
                .Returns<Guid>(teamId =>
                {
                    var team = this.teams.Find(team => team.Id == teamId);
                    if (team.ManagerId != null)
                    {
                        team.Manager = this.users.Find(u => u.Id == team.ManagerId);
                    }

                    return team;
                });
            teamRepository.Setup(mock => mock.getUserTeams(It.IsAny<Guid>()))
                .Returns<Guid>(userId => teams.FindAll(team =>
                    team.ManagerId == userId || team.Members.Select(member => member.Id).Contains(userId)).Select(
                    team =>
                    {
                        if (team.ManagerId != null)
                        {
                            team.Manager = this.users.Find(u => u.Id == team.ManagerId);
                        }

                        return team;
                    }).ToList());
            teamRepository.Setup(mock => mock.Update(It.IsAny<Team>()))
                .Callback<Team>(team =>
                {
                    this.teams.Remove(this.teams.Find(t => t.Id == team.Id));
                    this.teams.Add(team);
                });

            teamRepository.Setup(mock => mock.Delete(It.IsAny<Guid>())).Callback<Guid>(teamId =>
            {
                this.teams.Remove(this.teams.Find(team => team.Id == teamId));
            });


            var userRepository = new Mock<IRepository<User>>();
            userRepository.Setup(mock => mock.GetById(It.IsAny<Guid>()))
                .Returns<Guid>(userId => this.users.Find(user => user.Id == userId));

            var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new Mappings()); });
            var mapper = mockMapper.CreateMapper();
            teamService = new TeamService(teamRepository.Object, userRepository.Object, mapper);
        }

        [Fact]
        public void Create_Team_Test()
        {
            this.setup();
            // Given
            var manager = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};
            var team = new UpsertTeamDTO()
                {Name = "Test Team"};

            // When
            teamService.createTeam(team, manager.Id);

            // Then
            Assert.Equal(this.teams[0].ManagerId, manager.Id);
            Assert.Equal(1, teams.Count);
        }

        [Fact]
        public void Get_Team_By_Id()
        {
            this.setup();
            // Given
            var teamId = Guid.NewGuid();
            var team = new Team
                {Id = teamId, Created = DateTime.Now, Name = "Test Team"};
            this.teams.Add(team);

            // When
            var fetchedTeam = this.teamService.getTeamById(teamId);

            // Then
            Assert.Equal("Test Team", fetchedTeam.Name);
        }

        [Fact]
        public void GetTeams_Test()
        {
            this.setup();
            // Given
            var manager1 = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};
            var manager2 = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};

            var team1 = new Team
                {Id = Guid.NewGuid(), Created = DateTime.Now, Name = "Test Team", ManagerId = manager1.Id};

            var team2 = new Team
                {Id = Guid.NewGuid(), Created = DateTime.Now, Name = "Test Team",ManagerId = manager2.Id, Members = new List<User>() {manager1}};
            this.teams.Add(team1);
            this.teams.Add(team2);
            this.users.Add(manager1);
            this.users.Add(manager2);

            // When
            List<TeamDTO> getTeams = this.teamService.getTeams(manager1.Id).ToList();

            //Then
            Assert.Equal(2, getTeams.Count);
            Assert.Equal("Ombrelin", getTeams[0].Manager.Username);
        }

        [Fact]
        public void Add_Player_To_Team_Test()
        {
            this.setup();
            // Given
            var user = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};

            var team = new Team
                {Id = Guid.NewGuid(), Created = DateTime.Now, Name = "Test Team", Members = new List<User>()};
            this.users.Add(user);
            this.teams.Add(team);

            var userDTO = new UserDTO()
            {
                Id = user.Id
            };

            // When
            this.teamService.addPlayer(team.Id, userDTO);

            // Then
            Assert.Equal(1, this.teams[0].Members.Count);
        }

        [Fact]
        public void Remove_Player_To_Team_Test()
        {
            this.setup();
            // Given
            var user = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};

            var team = new Team
                {Id = Guid.NewGuid(), Created = DateTime.Now, Name = "Test Team", Members = new List<User>() {user}};
            this.users.Add(user);
            this.teams.Add(team);

            // When
            this.teamService.removePlayer(team.Id, user.Id);

            // Then
            Assert.Equal(0, this.teams[0].Members.Count);
        }

        [Fact]
        public void Update_Team_Test()
        {
            this.setup();
            // Given
            var team = new Team
                {Id = Guid.NewGuid(), Created = DateTime.Now, Name = "Test Team", Members = new List<User>() { }};
            this.teams.Add(team);

            // When 
            this.teamService.UpdateTeam(team.Id, new UpsertTeamDTO() {Name = "Test Team Updated"});

            // Then
            Assert.Equal("Test Team Updated", this.teams[0].Name);
        }
        
        
        [Fact]
        public void Delete_Team_Test()
        {
            this.setup();
            // Given
            var team = new Team
                {Id = Guid.NewGuid(), Created = DateTime.Now, Name = "Test Team", Members = new List<User>() { }};
            this.teams.Add(team);

            // When 
            this.teamService.DeleteTeam(team.Id);

            // Then
            Assert.Equal(0, this.teams.Count);
        }
        
    }
}