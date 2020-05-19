using System;
using System.Collections;
using System.Collections.Generic;

namespace ModernPlayerManagementAPI.Models.Repository
{
    public interface ITeamRepository : IRepository<Team>
    {
        ICollection<Team> getTeams();
        Team getTeam(Guid id);
        ICollection<Team> getUserTeams(Guid userId);

    }
}