using System;
using System.Collections.Generic;

namespace ModernPlayerManagementAPI.Models.DTOs
{
    public class EventDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Event.EventType Type { get; set; }
        public ICollection<ParticipationDTO> Participations { get; set; }
        public ICollection<DiscrepancyDTO> Discrepancies { get; set; }

        public Boolean CurrentHasConfirmed { get; set; }
    }
}