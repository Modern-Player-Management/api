using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ModernPlayerManagementAPI.Database;
using ModernPlayerManagementAPI.Migrations;
using ModernPlayerManagementAPI.Services;

namespace ModernPlayerManagementAPI.Models.Repository
{
    public class TeamRepository : Repository<Team>, ITeamRepository
    {
        public TeamRepository(ApplicationDbContext context) : base(context)
        {
        }

        public ICollection<Team> getTeams()
        {
            return GetTeamsEager().ToList();
        }


        public Team getTeam(Guid id)
        {
            return (from team in this._context.Teams.Include(team => team.Manager).Include(team => team.Memberships)
                where team.Id == id
                select team).First();
        }

        public ICollection<Team> getUserTeams(Guid userId)
        {
            return (from team in this.GetTeamsEager()
                where team.ManagerId == userId || team.Memberships.Select(member => member.UserId).Contains(userId)
                orderby team.Created
                select team).ToList();
        }

        private IIncludableQueryable<Team, User> GetTeamsEager()
        {
            return this._context.Teams
                .Include(team => team.Manager)
                .Include(team => team.Memberships)
                .ThenInclude(membership => membership.User);
        }
    }
}