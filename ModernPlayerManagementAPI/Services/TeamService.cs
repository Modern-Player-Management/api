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

        public TeamService(ITeamRepository teamRepository, IUserRepository userRepository, IMapper mapper)
        {
            this.teamRepository = teamRepository;
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public TeamDTO createTeam(UpsertTeamDTO teamDto, Guid currentUserId)
        {
            var team = new Team()
            {
                Name = teamDto.Name,
                ManagerId = currentUserId
            };
            this.teamRepository.Insert(team);

            team = this.teamRepository.getTeam(team.Id);
            
            var teamDTO = new TeamDTO()
            {
                Id = team.Id,
                Name = team.Name,
                isCurrentUserManager = true,
                Manager = mapper.Map<UserDTO>(team.Manager),
                Members = team.Members.Select(member => mapper.Map<UserDTO>(member)).ToList(),
                Created = team.Created
            };
            
            return teamDTO;
        }

        public TeamDTO getTeamById(Guid id)
        {
            return this.mapper.Map<TeamDTO>(this.teamRepository.getTeam(id));
        }

        public ICollection<TeamDTO> getTeams(Guid userId)
        {
            return this.teamRepository.getUserTeams(userId).Select(team =>
            {
                var dto = mapper.Map<TeamDTO>(team);
                dto.isCurrentUserManager = dto.Manager.Id == userId;
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
            
            if (!team.Members.Contains(player))
            {
                team.Members.Add(player);
                this.teamRepository.Update(team);
            }
        }

        public void removePlayer(Guid teamId, Guid playerId)
        {
            var team = this.teamRepository.getTeam(teamId);
            var player = this.userRepository.GetById(playerId);
            if (team.Members.Contains(player))
            {
                team.Members.Remove(player);
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
            team.Name = teamDto.Name;
            this.teamRepository.Update(team);
        }

        public void DeleteTeam(Guid teamId)
        {
            this.teamRepository.Delete(teamId);
        }
    }
}