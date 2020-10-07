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
        private readonly DbSet<Event> events;
        public TeamRepository(ApplicationDbContext context) : base(context)
        {
            events = context.Set<Event>();
        }

        public override ICollection<Team> GetAll()
        {
            return GetTeamsEager().ToList();
        }

        public override void Update(Team entity)
        {
            _entities.Update(entity);
            foreach (var evt in entity.Events)
            {
                events.Update(evt);
            }
            _context.SaveChanges();
        }

        public override Team GetById(Guid id)
        {
            return (from team in this.GetTeamsLazy()
                where team.Id == id
                select team).First();
        }
        
        public Team GetByIdDetailed(Guid id)
        {
            return (from team in this.GetTeamsEager()
                where team.Id == id
                select team).First();
        }

        public ICollection<Team> getUserTeams(Guid userId)
        {
            return (from team in this.GetTeamsLazy()
                where team.ManagerId == userId || team.Players.Select(member => member.UserId).Contains(userId)
                orderby team.Created
                select team).ToList();
        }

        public ICollection<PlayerStatsAvgDTO> GetAverageStats(Guid teamId)
        {
            return this.GetTeamsEager()
                .Where(team => team.Id == teamId)
                .SelectMany(team => team.Games)
                .SelectMany(game => game.PlayersStats)
                .GroupBy(stat => stat.Player)
                .Select(group => new PlayerStatsAvgDTO()
                {
                    Player = group.Key,
                    Score = group.Average(stat => stat.Score),
                    Assists = group.Average(stat => stat.Assists),
                    Goals = group.Average(stat => stat.Goals),
                    Saves = group.Average(stat => stat.Saves),
                    Shots = group.Average(stat => stat.Shots)
                }).ToList();
        }

        private IIncludableQueryable<Team, User> GetTeamsLazy()
        {
            return this._context.Teams
                .Include(team => team.Manager)
                .Include(team => team.Players)
                .ThenInclude(membership => membership.User);
        }

        private IIncludableQueryable<Team, ICollection<PlayerStats>> GetTeamsEager()
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
                .ThenInclude(part => part.User)
                .Include(team => team.Games)
                .ThenInclude(game => game.PlayersStats);
        }
    }
}