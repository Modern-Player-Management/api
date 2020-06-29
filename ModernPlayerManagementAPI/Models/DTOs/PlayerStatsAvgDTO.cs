namespace ModernPlayerManagementAPI.Models
{
    public class PlayerStatsAvgDTO
    {
        public string Player { get; set; }
        public double Goals { get; set; } = 0;
        public double Saves { get; set; }= 0;
        public double Shots { get; set; }= 0;
        public double Assists { get; set; }= 0;
        public double Score { get; set; }
        
        public double GoalShots => Goals == 0 ? 0 : (this.Shots / this.Goals) * 100;
    }
}