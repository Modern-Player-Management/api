using System;
using System.Collections.Generic;
using ModernPlayerManagementAPI.Models;

namespace ModernPlayerManagementAPI.Repositories
{
    public interface ITeamRepository : IRepository<Team>
    {
        ICollection<Team> getUserTeams(Guid userId);
    }
}