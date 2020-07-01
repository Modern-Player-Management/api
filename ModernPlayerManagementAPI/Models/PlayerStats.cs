using System.ComponentModel.DataAnnotations.Schema;

namespace ModernPlayerManagementAPI.Models
{
    public class PlayerStats : BaseEntity
    {
        public string Player { get; set; }
        public int Goals { get; set; } = 0;
        public int Saves { get; set; }= 0;
        public int Shots { get; set; }= 0;
        public int Assists { get; set; }= 0;
        public int Score { get; set; }

        [NotMapped]
        public float GoalShots => this.Shots == 0 ? 0 : (this.Goals /  this.Shots) * 100;
    }
}