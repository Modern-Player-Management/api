using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using ModernPlayerManagementAPI.Mapper;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Models.DTOs;
using ModernPlayerManagementAPI.Repositories;
using ModernPlayerManagementAPI.Services;
using Moq;
using RocketLeagueReplayParser;
using Xunit;

namespace ModernPlayerManagementAPITests
{
    public class TeamServiceTest
    {
        private readonly List<Team> teams;
        private readonly List<User> users;
        private readonly TeamService teamService;
        private List<Event> events;

        public TeamServiceTest()
        {
            teams = new List<Team>();
            users = new List<User>();
            events = new List<Event>();

            var teamRepository = new Mock<ITeamRepository>();
            teamRepository.Setup(mock => mock.Insert(It.IsAny<Team>())).Callback<Team>(team =>
            {
                team.Players ??= new List<Membership>();
                teams.Add(team);
            });
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
            teamRepository.Setup(mock => mock.GetByIdDetailed(It.IsAny<Guid>()))
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
            teamRepository.Setup(mock => mock.getUserTeams(It.IsAny<Guid>()))
                .Returns<Guid>(userId => teams.FindAll(team =>
                        team.ManagerId == userId || team.Players.Select(member => member.UserId).Contains(userId))
                    .Select(
                        team =>
                        {
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

                            team.Events ??= new List<Event>();
                            foreach (var evt in events.Where(evt => evt.TeamId == team.Id))
                            {
                                team.Events.Add(evt);
                            }

                            team.Games ??= new List<Game>();

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


            var userRepository = new Mock<IUserRepository>();
            var fileService = new Mock<IFilesService>();
            userRepository.Setup(mock => mock.GetById(It.IsAny<Guid>()))
                .Returns<Guid>(userId => this.users.Find(user => user.Id == userId));

            var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new Mappings()); });
            var mapper = mockMapper.CreateMapper();

            var eventRespository = new Mock<IEventRepository>();
            eventRespository.Setup(mock => mock.GetById(It.IsAny<Guid>())).Returns<Guid>(eventId =>
            {
                return this.teams.Select(team => team.Events).SelectMany(events => events)
                    .First(evt => evt.Id == eventId);
            });

            teamService = new TeamService(teamRepository.Object, userRepository.Object, mapper, fileService.Object,
                eventRespository.Object);
        }

        [Fact]
        public void Create_Team_Test()
        {
            // Given
            var manager = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};
            var team = new UpsertTeamDTO()
                {Name = "Test Team"};

            // When
            teamService.createTeam(team, manager.Id);

            // Then
            Assert.Equal(this.teams[0].ManagerId, manager.Id);
            Assert.Single(teams);
        }

        [Fact]
        public void Get_Team_By_Id()
        {
            // Given
            var manager = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};
            var teamId = Guid.NewGuid();
            var team = new Team
                {Id = teamId, Created = DateTime.Now,ManagerId = manager.Id,Name = "Test Team"};
            this.teams.Add(team);
            this.users.Add(manager);
            
            // When
            var fetchedTeam = this.teamService.GetTeam(teamId,manager.Id);

            // Then
            Assert.Equal("Test Team", fetchedTeam.Name);
        }

        [Fact]
        public void GetTeams_Test()
        {
            // Given
            var manager1 = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};
            var manager2 = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};
            var user = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};

            var team1 = new Team
            {
                Id = Guid.NewGuid(), Created = DateTime.Now, Name = "Test Team 1", ManagerId = manager1.Id,
                Events = new List<Event>()
                {
                    new Event()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Test Event",
                        Description = "Test Event Description",
                        Start = DateTime.Now,
                        End = DateTime.Now,
                        Discrepancies = new List<Discrepancy>()
                            {new Discrepancy() {Created = DateTime.Now, Reason = "Test", User = user}},
                        Participations = new List<Participation>()
                            {new Participation() {Created = DateTime.Now, User = user}},
                        Created = DateTime.Now,
                        Type = Event.EventType.Coaching
                    }
                }
            };

            var team2 = new Team
            {
                Id = Guid.NewGuid(), Created = DateTime.Now, Name = "Test Team 2", ManagerId = manager2.Id,
                Players = new List<Membership>() { }
            };
            team2.Players.Add(new Membership() {UserId = manager1.Id, TeamId = team2.Id});
            this.teams.Add(team1);
            this.teams.Add(team2);
            this.users.Add(manager1);
            this.users.Add(manager2);

            // When
            List<TeamDTO> getTeams = this.teamService.GetTeams(manager1.Id).ToList();

            //Then
            Assert.Equal(2, getTeams.Count);
            Assert.Equal("Ombrelin", getTeams[0].Manager.Username);
        }

        [Fact]
        public void Add_Player_To_Team_Test()
        {
            // Given
            var user = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};

            var team = new Team
                {Id = Guid.NewGuid(), Created = DateTime.Now, Name = "Test Team", Players = new List<Membership>()};
            this.users.Add(user);
            this.teams.Add(team);

            var userDTO = new UserDTO()
            {
                Id = user.Id
            };

            // When
            this.teamService.AddPlayer(team.Id, userDTO);

            // Then
            Assert.Equal(1, this.teams[0].Players.Count);
        }

        [Fact]
        public void Remove_Player_To_Team_Test()
        {
            // Given
            var user = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};

            var team = new Team
                {Id = Guid.NewGuid(), Created = DateTime.Now, Name = "Test Team", Players = new List<Membership>()};
            team.Players.Add(new Membership() {UserId = user.Id, TeamId = team.Id});

            this.users.Add(user);
            this.teams.Add(team);

            // When
            this.teamService.RemovePlayer(team.Id, new UserDTO() {Id = user.Id});

            // Then
            Assert.Equal(0, this.teams[0].Players.Count);
        }

        [Fact]
        public void Update_Team_Test()
        {
            // Given
            var team = new Team
            {
                Id = Guid.NewGuid(), Created = DateTime.Now, Name = "Test Team",
                Players = new List<Membership>() { }
            };
            this.teams.Add(team);

            // When 
            this.teamService.UpdateTeam(team.Id, new UpsertTeamDTO() {Name = "Test Team Updated"});

            // Then
            Assert.Equal("Test Team Updated", this.teams[0].Name);
        }


        [Fact]
        public void Delete_Team_Test()
        {
            // Given
            var team = new Team
            {
                Id = Guid.NewGuid(), Created = DateTime.Now, Name = "Test Team",
                Players = new List<Membership>() { }
            };
            this.teams.Add(team);

            // When 
            this.teamService.DeleteTeam(team.Id);

            // Then
            Assert.Empty(this.teams);
        }

        [Fact]
        public void Add_Event_Test()
        {
            // Given
            var team = new Team
            {
                Id = Guid.NewGuid(), Created = DateTime.Now, Name = "Test Team",
                Players = new List<Membership>() { }, Events = new List<Event>()
            };
            this.teams.Add(team);

            var evtDTO = new UpsertEventDTO()
            {
                Name = "Test Event",
                Description = "Test Event Description",
                Start = DateTime.Now,
                End = DateTime.Now,
                Type = Event.EventType.Coaching
            };

            // When
            this.teamService.AddEvent(team.Id, evtDTO);

            // Then
            Assert.Single(team.Events);
            Assert.Equal("Test Event", team.Events.First().Name);
        }

        [Fact]
        void IsUserTeamManager_Test()
        {
            // Given
            var user1 = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};
            var user2 = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};

            var team = new Team
            {
                Id = Guid.NewGuid(), Created = DateTime.Now, Name = "Test Team",
                Players = new List<Membership>() { }, Events = new List<Event>(),
                ManagerId = user1.Id
            };
            this.teams.Add(team);


            // When
            bool result1 = this.teamService.IsUserTeamManager(team.Id, user1.Id);
            bool result2 = this.teamService.IsUserTeamManager(team.Id, user2.Id);

            // Then
            Assert.True(result1);
            Assert.False(result2);
        }

        [Fact]
        void AddGameTest()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            // Given
            var user = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};

            var team = new Team
                {Id = Guid.NewGuid(), Created = DateTime.Now, Name = "Test Team", Players = new List<Membership>()};
            team.Players.Add(new Membership() {UserId = user.Id, TeamId = team.Id});

            this.users.Add(user);
            this.teams.Add(team);
            var replay = Replay.Deserialize("../../../42EC23C0457C5367AA062C825B3011ED.replay");

            // When
            this.teamService.AddGame(replay, team.Id);

            // Then
            team = this.teams.First(t => t.Id == team.Id);
            Assert.Equal(1,team.Games.Count);
            Assert.Equal(3,team.Games.First().PlayersStats.Count);
        }
    }
}