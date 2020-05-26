using System;
using System.ComponentModel.DataAnnotations;

namespace ModernPlayerManagementAPI.Models.DTOs
{
    public class UpsertEventDTO
    {
        [Required] public string Name { get; set; }
        [Required] public string Description { get; set; }
        [Required] public DateTime Start { get; set; }
        [Required] public DateTime End { get; set; }
        [Required] public Event.EventType Type { get; set; }
    }
}