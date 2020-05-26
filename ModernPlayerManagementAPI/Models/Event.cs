using System;
using System.Collections.Generic;

namespace ModernPlayerManagementAPI.Models
{
    public class Event : BaseEntity
    {
        public Guid TeamId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Event.EventType Type { get; set; }
        public ICollection<Participation> Participations { get; set; }
        public ICollection<Discrepancy> Discrepancies { get; set; }
        public enum EventType { Scrim, Meeting, Tournament, Coaching}
    }
}