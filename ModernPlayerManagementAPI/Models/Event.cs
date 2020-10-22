using System;
using System.Collections.Generic;
using System.Linq;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;

namespace ModernPlayerManagementAPI.Models
{
    public class Event : BaseEntity
    {
        public Guid TeamId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public EventType Type { get; set; }
        public ICollection<Participation> Participations { get; set; }
        public ICollection<Discrepancy> Discrepancies { get; set; }

        public enum EventType
        {
            Scrim,
            Meeting,
            Tournament,
            Coaching
        }

        public CalendarEvent GetAsICal()
        {
            return new CalendarEvent()
            {
                Start = new CalDateTime(Start),
                End = new CalDateTime(End),
                Summary = Name,
                Attendees = Participations.Select(p => new Attendee()
                    {CommonName = p.User.Username, Value = new Uri($"mailto:{p.User.Email}")}).ToList(),
                Description = Description
            };
        }

        public void AddDiscrepancy(Discrepancy d)
        {
            if (Discrepancies.Select(d => d.UserId).Contains(d.UserId))
            {
                throw new ArgumentException("You already have a discrepancy on this event");
            }
            Discrepancies.Add(d);
        }
    }
}