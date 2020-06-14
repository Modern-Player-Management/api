using System;
using System.Collections.Generic;

namespace ModernPlayerManagementAPI.Models.DTOs
{
    public class GameDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public Game.GameResult Win { get; set; }
        public ICollection<PlayerStats> PlayersStats { get; set; }
    }
}