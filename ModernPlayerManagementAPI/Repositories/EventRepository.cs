using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ModernPlayerManagementAPI.Database;
using ModernPlayerManagementAPI.Models;

namespace ModernPlayerManagementAPI.Repositories
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        private readonly ITeamRepository _teamRepository;

        public EventRepository(ApplicationDbContext context, ITeamRepository teamRepository) : base(context)
        {
            _teamRepository = teamRepository;
        }

        public override Event GetById(Guid id)
        {
            return (from evt in this._context.Events.Include(e => e.Participations).Include(e => e.Discrepancies)
                select evt).First(e => e.Id == id);
        }

        public ICollection<Event> GetUserFutureEvents(Guid userId)
        {
            var teams = this._teamRepository.getUserTeams(userId);

            return (
                from evt in this._context.Events.Include(e => e.Participations).ThenInclude(p => p.User)
                where teams.Select(team => team.Id).Contains(evt.TeamId)
                where evt.Start >= DateTime.Now
                select evt
            ).ToList();
        }
    }
}