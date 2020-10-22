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
            Win = IsWin(ExtractScore("Team0Score"), ExtractScore("Team1Score"));
            PlayersStats = ExtractPlayerStats();
        }

        private int ExtractScore(string team)
        {
            return int.Parse(replay.Properties.GetValueOrDefault(team).Value.ToString());
        }

        private static GameResult IsWin(int team0Score, int team1Score)
        {
            return (team0Score == team1Score)
                ? Game.GameResult.Draw
                : (team0Score > team1Score ? Game.GameResult.Win : Game.GameResult.Loss);
        }

        private DateTime ExtractDate()
        {
            return DateTime.ParseExact(replay.Properties.GetValueOrDefault("Date").Value.ToString(),
                "yyyy-MM-dd HH-mm-ss", CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        private List<PlayerStats> ExtractPlayerStats()
        {
            return ((List<PropertyDictionary>) replay.Properties.GetValueOrDefault("PlayerStats").Value)
                .Where(v => int.Parse(v.GetValueOrDefault("Team").Value.ToString()) == 0)
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