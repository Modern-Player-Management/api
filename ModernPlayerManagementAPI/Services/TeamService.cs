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
        private readonly IFilesService filesService;
        private readonly IEventRepository eventRepository;
        private readonly IRepository<Game> gameRepository;

        public TeamService(ITeamRepository teamRepository, IUserRepository userRepository, IMapper mapper,
            IFilesService filesService, IEventRepository eventRepository, IRepository<Game> gameRepository)
        {
            this.teamRepository = teamRepository;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.filesService = filesService;
            this.eventRepository = eventRepository;
            this.gameRepository = gameRepository;
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

            evt.Participations.Add(new Participation()
                {Created = DateTime.Now, Confirmed = false, UserId = team.ManagerId});

            team.AddEvent(evt);
            this.teamRepository.Update(team);

            evt = this.eventRepository.GetById(evt.Id);

            var responseDto = new EventDTO()
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

            return responseDto;
        }

        public GameDTO AddGame(Replay replay, Guid teamId)
        {
            var team = this.teamRepository.GetById(teamId);
            var game = new Game(replay);
            team.Games.Add(game);

            this.teamRepository.Update(team);

            return this.mapper.Map<GameDTO>(this.gameRepository.GetById(game.Id));
        }

        public List<PlayerStatsAvgDTO> GetStats(Guid teamId)
        {
            return this.teamRepository.GetAverageStats(teamId).ToList();
        }

        public TeamDTO CreateTeam(InsertTeamDTO teamDto, Guid currentUserId)
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

        public TeamDTODetailed GetTeam(Guid id, Guid userId)
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
                    }).ToList(),
                    CurrentHasConfirmed =
                        evt.Participations.First(p => p.UserId == userId).Confirmed
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

            var player = GetUserFromDto(playerDto);

            team.AddPlayer(player);
            this.teamRepository.Update(team);
        }

        private User GetUserFromDto(UserDTO playerDto)
        {
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

            return player;
        }

        public void RemovePlayer(Guid teamId, UserDTO dto)
        {
            var player = GetUserFromDto(dto);
            var team = this.teamRepository.GetById(teamId);
            team.RemovePlayer(player);
            this.teamRepository.Update(team);
        }

        public void UpdateTeam(Guid teamId, UpdateTeamDTO teamDto)
        {
            var team = this.teamRepository.GetById(teamId);

            if (teamDto.Image != team.Image && teamDto.Image != null)
            {
                if (team.Image != null)
                {
                    this.filesService.Delete(Guid.Parse(team.Image.Split("/").Last()));
                }

                team.Image = teamDto.Image;
            }
            
            team.Name = teamDto.Name ?? team.Name;
            team.Description = teamDto.Description ?? team.Description;

            this.teamRepository.Update(team);
        }

        public void DeleteTeam(Guid teamId)
        {
            var team = this.teamRepository.GetById(teamId);
            if (team.Image != null)
            {
                this.filesService.Delete(Guid.Parse(team.Image.Split("/").Last()));
            }

            this.teamRepository.Delete(teamId);
        }
    }
}