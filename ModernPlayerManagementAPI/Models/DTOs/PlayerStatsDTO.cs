﻿namespace ModernPlayerManagementAPI.Models.DTOs
{
    public class PlayerStatsDTO
    {
        public string Player { get; set; }
        public int Goals { get; set; } = 0;
        public int Saves { get; set; }= 0;
        public int Score { get; set; }= 0;
        public int Shots { get; set; }= 0;
        public int Assists { get; set; }= 0;
        public float GoalShots => Goals == 0 ? 0 : (this.Shots / this.Goals) * 100;
    }
}