using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ModernPlayerManagementAPI.Database;
using ModernPlayerManagementAPI.Models;

namespace ModernPlayerManagementAPI.Repositories
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(ApplicationDbContext context) : base(context)
        {
        }

        public new Event GetById(Guid id)
        {
            return (from evt in this._context.Events.Include(e => e.Participations) select evt).First(e => e.Id == id);
        }
    }
}