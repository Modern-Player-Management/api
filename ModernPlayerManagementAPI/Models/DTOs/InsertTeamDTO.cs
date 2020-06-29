using System.ComponentModel.DataAnnotations;

namespace ModernPlayerManagementAPI.Models.DTOs
{
    public class InsertTeamDTO
    {
        [Required] public string Name { get; set; }
        [Required] public string Description { get; set; }
        public string Image { get; set; }
    }
}