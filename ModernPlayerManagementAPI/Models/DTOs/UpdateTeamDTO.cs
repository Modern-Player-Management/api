using System.ComponentModel.DataAnnotations;

namespace ModernPlayerManagementAPI.Models.DTOs
{
    public class UpdateTeamDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}