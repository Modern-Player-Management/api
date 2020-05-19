using System;
using System.Collections.Generic;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Models.Repository;

namespace ModernPlayerManagementAPI.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository teamRepository;

        public TeamService(ITeamRepository teamRepository)
        {
            this.teamRepository = teamRepository;
        }

        public Team createTeam(Team team, Guid managerId)
        {
            team.ManagerId = managerId;
            this.teamRepository.Insert(team);
            return team;
        }

        public Team getTeamById(Guid id)
        {
            return this.teamRepository.getTeam(id);
        }

        public ICollection<Team> getTeams(Guid userId)
        {
            return this.teamRepository.getUserTeams(userId);
        }

        public void addPlayer(Guid teamId, User player)
        {
            var team = this.teamRepository.getTeam(teamId);
            if (!team.Members.Contains(player))
            {
                team.Members.Add(player);
                this.teamRepository.Update(team);
            }
        }

        public void removePlayer(Guid teamId, User player)
        {
            var team = this.teamRepository.getTeam(teamId);
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

        public void UpdateTeam(Team team)
        {
            this.teamRepository.Update(team);
        }

        public void DeleteTeam(Guid teamId)
        {
            this.teamRepository.Delete(teamId);
        }
    }
}