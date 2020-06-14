using System;
using System.Collections.Generic;

namespace ModernPlayerManagementAPI.Models
{
    public class Game : BaseEntity
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public GameResult Win { get; set; }
        public ICollection<PlayerStats> PlayersStats { get; set; }
        
        public enum GameResult {Win,Loss,Draw}
    }
}