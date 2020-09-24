using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ModernPlayerManagementAPI.Models
{
    public class Team : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public Guid ManagerId { get; set; }

        [ForeignKey("ManagerId")] public User Manager { get; set; }

        public ICollection<Event> Events { get; set; }

        public ICollection<Membership> Players { get; set; }
        public ICollection<Game> Games { get; set; }

        public void AddEvent(Event evt)
        {
            Events.Add(evt);
        }

        public void AddPlayer(User player)
        {
            if (Players.Select(member => member.UserId).Contains(player.Id))
            {
                return;
            }

            Players.Add(new Membership() {TeamId = Id, UserId = player.Id});

            foreach (var evt in Events)
            {
                evt.Participations.Add(new Participation()
                {
                    Confirmed = false,
                    Created = DateTime.Now,
                    EventId = evt.Id,
                    UserId = player.Id
                });
            }
        }

        public void RemovePlayer(User player)
        {
            if (Players.Select(member => member.UserId).Contains(player.Id))
            {
                Players.Remove(Players.First(membership => membership.UserId == player.Id));
            }
            else
            {
                throw new ArgumentException("Player is not in the team");
            }
        }
    }
}