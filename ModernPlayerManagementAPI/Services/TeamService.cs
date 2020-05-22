using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Models.DTOs;
using ModernPlayerManagementAPI.Models.Repository;

namespace ModernPlayerManagementAPI.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository teamRepository;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly IFilesService _filesService;

        public TeamService(ITeamRepository teamRepository, IUserRepository userRepository, IMapper mapper,IFilesService filesService)
        {
            this.teamRepository = teamRepository;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this._filesService = filesService;
        }

        public bool IsUserTeamManager(Guid teamId, Guid userId)
        {
            var team = this.teamRepository.getTeam(teamId);

            return team.ManagerId == userId;
        }
        
        public TeamDTO createTeam(UpsertTeamDTO teamDto, Guid currentUserId)
        {
            var team = new Team()
            {
                Name = teamDto.Name,
                ManagerId = currentUserId,
                Description = teamDto.Description,
                Image = teamDto.Image
            };
            this.teamRepository.Insert(team);

            team = this.teamRepository.getTeam(team.Id);

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

        public TeamDTO getTeamById(Guid id)
        {
            return this.mapper.Map<TeamDTO>(this.teamRepository.getTeam(id));
        }

        public ICollection<TeamDTO> getTeams(Guid userId)
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

        public void addPlayer(Guid teamId, UserDTO playerDto)
        {
            var team = this.teamRepository.getTeam(teamId);

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

        public void removePlayer(Guid teamId, UserDTO dto)
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
            
            var team = this.teamRepository.getTeam(teamId);
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
        
        public void UpdateTeam(Guid teamId, UpsertTeamDTO teamDto)
        {

            
            var team = this.teamRepository.getTeam(teamId);

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
            var team = this.teamRepository.getTeam(teamId);
            if (team.Image != null)
            {
                this._filesService.Delete(Guid.Parse(team.Image.Split("/").Last()));
            }

            this.teamRepository.Delete(teamId);
        }


    }
}