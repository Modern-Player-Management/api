using System;

namespace ModernPlayerManagementAPI.Models.DTOs
{
    public class DbFileDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}