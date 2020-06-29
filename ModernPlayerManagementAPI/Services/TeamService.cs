using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Models.DTOs;
using ModernPlayerManagementAPI.Repositories;
using RocketLeagueReplayParser;

namespace ModernPlayerManagementAPI.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository teamRepository;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly IFilesService _filesService;
        private readonly IEventRepository eventRepository;

        public TeamService(ITeamRepository teamRepository, IUserRepository userRepository, IMapper mapper,
            IFilesService filesService, IEventRepository eventRepository)
        {
            this.teamRepository = teamRepository;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this._filesService = filesService;
            this.eventRepository = eventRepository;
        }

        public bool IsUserTeamManager(Guid teamId, Guid userId)
        {
            var team = this.teamRepository.GetById(teamId);

            return team.ManagerId == userId;
        }

        public EventDTO AddEvent(Guid teamId, UpsertEventDTO dto)
        {
            var team = this.teamRepository.GetById(teamId);
            var evt = new Event()
            {
                Name = dto.Name,
                Description = dto.Description,
                Start = dto.Start,
                End = dto.End,
                Created = DateTime.Now,
                Discrepancies = new List<Discrepancy>(),
                Participations = team.Players.Select(membership => new Participation()
                    {Created = DateTime.Now, Confirmed = false, UserId = membership.UserId}).ToList(),
                Type = dto.Type
            };
            team.Events ??= new List<Event>();
            team.Events.Add(evt);
            this.teamRepository.Update(team);

            evt = this.eventRepository.GetById(evt.Id);

            var responseDTO = new EventDTO()
            {
                Id = evt.Id,
                Name = evt.Name,
                Description = evt.Description,
                Start = evt.Start,
                End = evt.End,
                Type = evt.Type,
                Participations = evt.Participations.Select(e => new ParticipationDTO()
                    {Confirmed = e.Confirmed, UserId = e.UserId, Username = e.User.Username}).ToList(),
                Discrepancies = new List<DiscrepancyDTO>()
            };

            return responseDTO;
        }

        public GameDTO AddGame(Replay replay, Guid teamId)
        {
            var playerStats = (List<PropertyDictionary>) replay.Properties.GetValueOrDefault("PlayerStats").Value;
            var stats = playerStats
                .Where(v => Int32.Parse(v.GetValueOrDefault("Team").Value.ToString()) == 0)
                .Select(v => new PlayerStats()
                {
                    Player = v.GetValueOrDefault("Name").Value.ToString(),
                    Assists = int.Parse(v.GetValueOrDefault("Assists").Value.ToString()),
                    Goals = int.Parse(v.GetValueOrDefault("Goals").Value.ToString()),
                    Saves = int.Parse(v.GetValueOrDefault("Saves").Value.ToString()),
                    Shots = int.Parse(v.GetValueOrDefault("Shots").Value.ToString()),
                    Score = int.Parse(v.GetValueOrDefault("Score").Value.ToString()),
                    Created = DateTime.Now
                }).ToList();

            CultureInfo provider = CultureInfo.InvariantCulture;
            var date = DateTime.ParseExact(replay.Properties.GetValueOrDefault("Date").Value.ToString(),
                "yyyy-MM-dd HH-mm-ss", provider, DateTimeStyles.None);

            var team0Score = int.Parse(replay.Properties.GetValueOrDefault("Team0Score").Value.ToString());
            var team1Score = int.Parse(replay.Properties.GetValueOrDefault("Team1Score").Value.ToString());
            var name = replay.Properties.GetValueOrDefault("ReplayName").Value.ToString();
            var game = new Game()
            {
                Created = DateTime.Now,
                Date = date,
                Name = name,
                Win = (team0Score == team1Score)
                    ? Game.GameResult.Draw
                    : (team0Score > team1Score ? Game.GameResult.Win : Game.GameResult.Loss),
                PlayersStats = stats
            };

            var team = this.teamRepository.GetById(teamId);
            team.Games.Add(game);

            this.teamRepository.Update(team);
            team = this.teamRepository.GetById(teamId);
            return this.mapper.Map<GameDTO>(team.Games.First(g => g.Name == name));
        }


        public TeamDTO createTeam(InsertTeamDTO teamDto, Guid currentUserId)
        {
            var team = new Team()
            {
                Name = teamDto.Name,
                ManagerId = currentUserId,
                Description = teamDto.Description,
                Image = teamDto.Image
            };
            this.teamRepository.Insert(team);

            team = this.teamRepository.GetById(team.Id);

            var teamDTO = new TeamDTO()
            {
                Id = team.Id,
                Name = team.Name,
                isCurrentUserManager = true,
                Manager = mapper.Map<UserDTO>(team.Manager),
                Players = new List<UserDTO>(),
                Created = team.Created,
                Description = team.Description,
                Image = team.Image
            };

            return teamDTO;
        }

        public TeamDTODetailed GetTeam(Guid id,Guid userId)
        {
            var team = this.teamRepository.GetByIdDetailed(id);

            var dto = new TeamDTODetailed()
                {
                    Name = team.Name,
                    Created = team.Created,
                    Id = team.Id,
                    isCurrentUserManager = team.Manager.Id == userId,
                    Manager = mapper.Map<UserDTO>(team.Manager),
                    Players = team.Players.Select(membership => new UserDTO()
                    {
                        Id = membership.UserId,
                        Username = membership.User.Username,
                        Image = membership.User.Image
                    }).ToList(),
                    Description = team.Description,
                    Image = team.Image,
                    Events = team.Events.Select(evt => new EventDTO()
                    {
                        Id = evt.Id,
                        Name = evt.Name,
                        Description = evt.Description,
                        Start = evt.Start,
                        End = evt.End,
                        Type = evt.Type,
                        Participations = evt.Participations.Select(e => new ParticipationDTO()
                            {Confirmed = e.Confirmed, UserId = e.UserId, Username = e.User.Username}).ToList(),
                        Discrepancies = evt.Discrepancies.Select(e => new DiscrepancyDTO()
                        {
                            Id = e.Id,
                            DelayLength = e.DelayLength,
                            Reason = e.Reason,
                            Type = e.Type,
                            UserId = e.UserId,
                            Username = e.User.Username
                        }).ToList()
                    }).ToList(),
                    Games = team.Games.Select(game => this.mapper.Map<GameDTO>(game)).ToList()
                };
                return dto;
        }

        public ICollection<TeamDTO> GetTeams(Guid userId)
        {
            var teams = this.teamRepository.getUserTeams(userId);
            return teams.Select(team =>
            {
                var dto = new TeamDTO()
                {
                    Name = team.Name,
                    Created = team.Created,
                    Id = team.Id,
                    isCurrentUserManager = team.Manager.Id == userId,
                    Manager = mapper.Map<UserDTO>(team.Manager),
                    Players = team.Players.Select(membership => new UserDTO()
                    {
                        Id = membership.UserId,
                        Username = membership.User.Username,
                        Image = membership.User.Image
                    }).ToList(),
                    Description = team.Description,
                    Image = team.Image
                };
                return dto;
            }).ToList();
        }

        public void AddPlayer(Guid teamId, UserDTO playerDto)
        {
            var team = this.teamRepository.GetById(teamId);

            User player;
            if (playerDto.Id != Guid.Empty)
            {
                player = this.userRepository.GetById(playerDto.Id);
            }
            else if (playerDto.Username != null)
            {
                player = this.userRepository.GetUserByUsername(playerDto.Username);
            }
            else
            {
                throw new ArgumentException("You should provide either the username or the Id of the user");
            }

            if (!team.Players.Select(member => member.UserId).Contains(player.Id))
            {
                team.Players.Add(new Membership() {TeamId = teamId, UserId = player.Id});
                this.teamRepository.Update(team);
            }
        }

        public void RemovePlayer(Guid teamId, UserDTO dto)
        {
            User player;
            if (dto.Id != Guid.Empty)
            {
                player = this.userRepository.GetById(dto.Id);
            }
            else if (dto.Username != null)
            {
                player = this.userRepository.GetUserByUsername(dto.Username);
            }
            else
            {
                throw new ArgumentException("You should provide either the username or the Id of the user");
            }

            var team = this.teamRepository.GetById(teamId);
            if (team.Players.Select(member => member.UserId).Contains(player.Id))
            {
                team.Players.Remove(team.Players.First(membership => membership.UserId == player.Id));
                this.teamRepository.Update(team);
            }
            else
            {
                throw new ArgumentException("Player is not in the team");
            }
        }

        public void UpdateTeam(Guid teamId, UpdateTeamDTO teamDto)
        {
            var team = this.teamRepository.GetById(teamId);

            if (teamDto.Image != team.Image)
            {
                if (team.Image != null)
                {
                    this._filesService.Delete(Guid.Parse(team.Image.Split("/").Last()));
                }

                team.Image = teamDto.Image;
            }

            team.Name = teamDto?.Name;
            team.Description = teamDto?.Description;
            this.teamRepository.Update(team);
        }

        public void DeleteTeam(Guid teamId)
        {
            var team = this.teamRepository.GetById(teamId);
            if (team.Image != null)
            {
                this._filesService.Delete(Guid.Parse(team.Image.Split("/").Last()));
            }

            this.teamRepository.Delete(teamId);
        }
    }
}