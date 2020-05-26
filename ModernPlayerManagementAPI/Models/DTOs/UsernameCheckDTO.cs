using System.ComponentModel.DataAnnotations;

namespace ModernPlayerManagementAPI.Models.DTOs
{
    public class UsernameCheckDTO
    {
        [Required] public string Username { get; set; }
    }
}