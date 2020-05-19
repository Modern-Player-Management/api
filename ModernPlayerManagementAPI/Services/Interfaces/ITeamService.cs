using System;
using System.Collections.Generic;
using ModernPlayerManagementAPI.Models;

namespace ModernPlayerManagementAPI.Services
{
    public interface ITeamService
    {
        Team createTeam(Team team, Guid managerId);
        Team getTeamById(Guid id);
        ICollection<Team> getTeams(Guid userId);
        void addPlayer(Guid teamId, User player);
        void removePlayer(Guid teamId, User player);
        void UpdateTeam(Team team);
        void DeleteTeam(Guid Id);
    }
}