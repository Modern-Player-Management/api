using System;
using System.Collections.Generic;
using ModernPlayerManagementAPI.Models;

namespace ModernPlayerManagementAPI.Repositories
{
    public interface IEventRepository : IRepository<Event>
    {
        ICollection<Event> GetUserFutureEvents(Guid userId);
    }
}