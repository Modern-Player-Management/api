using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using RocketLeagueReplayParser;

namespace ModernPlayerManagementAPI.Models
{
    public class Game : BaseEntity
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public GameResult Win { get; set; }
        public ICollection<PlayerStats> PlayersStats { get; set; }

        [NotMapped]
        private Replay replay;

        public enum GameResult
        {
            Win,
            Loss,
            Draw
        }

        public Game()
        {
        }

        public Game(Replay replay)
        {
            this.replay = replay;
            Created = DateTime.Now;
            Date = ExtractDate();
            Name = replay.Properties.GetValueOrDefault("ReplayName").Value.ToString();
            Win = IsWin();
            PlayersStats = ExtractPlayerStats();
        }

        private GameResult IsWin()
        {
            int playerTeam = int.Parse(replay.Properties.GetValueOrDefault("PrimaryPlayerTeam").Value.ToString());
            int playerTeamScore = ((List<PropertyDictionary>) replay.Properties.GetValueOrDefault("PlayerStats").Value)
                .Where(v => int.Parse(v.GetValueOrDefault("Team").Value.ToString()) == playerTeam)
                .Select(v => int.Parse(v.GetValueOrDefault("Goals").Value.ToString()))
                .Sum();
            int ennemyTeamScore = ((List<PropertyDictionary>) replay.Properties.GetValueOrDefault("PlayerStats").Value)
                .Where(v => int.Parse(v.GetValueOrDefault("Team").Value.ToString()) != playerTeam)
                .Select(v => int.Parse(v.GetValueOrDefault("Goals").Value.ToString()))
                .Sum();
            
            return (playerTeamScore == ennemyTeamScore)
                ? Game.GameResult.Draw
                : (playerTeamScore > ennemyTeamScore ? Game.GameResult.Win : Game.GameResult.Loss);
        }

        private DateTime ExtractDate()
        {
            return DateTime.ParseExact(replay.Properties.GetValueOrDefault("Date").Value.ToString(),
                "yyyy-MM-dd HH-mm-ss", CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        private List<PlayerStats> ExtractPlayerStats()
        {
            int playerTeam = int.Parse(replay.Properties.GetValueOrDefault("PrimaryPlayerTeam").Value.ToString());
            return ((List<PropertyDictionary>) replay.Properties.GetValueOrDefault("PlayerStats").Value)
                .Where(v => int.Parse(v.GetValueOrDefault("Team").Value.ToString()) == playerTeam)
                .Select(v => new PlayerStats()
                {
                    Player = v.GetValueOrDefault("Name").Value.ToString(),
                    Assists = int.Parse(v.GetValueOrDefault("Assists").Value.ToString()),
                    Goals = int.Parse(v.GetValueOrDefault("Goals").Value.ToString()),
                    Saves = int.Parse(v.GetValueOrDefault("Saves").Value.ToString()),
                    Shots = int.Parse(v.GetValueOrDefault("Shots").Value.ToString()),
                    Score = int.Parse(v.GetValueOrDefault("Score").Value.ToString()),
                    Created = DateTime.Now
                }).ToList();
        }
    }
}