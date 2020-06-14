using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ModernPlayerManagementAPI.Database;
using ModernPlayerManagementAPI.Models;

namespace ModernPlayerManagementAPI.Repositories
{
    public class TeamRepository : Repository<Team>, ITeamRepository
    {
        public TeamRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override ICollection<Team> GetAll()
        {
            return GetTeamsEager().ToList();
        }

        public override Team GetById(Guid id)
        {
            return (from team in this.GetTeamsEager()
                where team.Id == id
                select team).First();
        }

        public ICollection<Team> getUserTeams(Guid userId)
        {
            return (from team in this.GetTeamsEager()
                where team.ManagerId == userId || team.Players.Select(member => member.UserId).Contains(userId)
                orderby team.Created
                select team).ToList();
        }

        private IIncludableQueryable<Team, User> GetTeamsEager()
        {
            return this._context.Teams
                .Include(team => team.Manager)
                .Include(team => team.Players)
                .ThenInclude(membership => membership.User)
                .Include(team => team.Events)
                .ThenInclude(evt => evt.Discrepancies)
                .ThenInclude(evt => evt.User)
                .Include(team => team.Events)
                .ThenInclude(evt => evt.Participations)
                .ThenInclude(part => part.User);
        }
    }
}