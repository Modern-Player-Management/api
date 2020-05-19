using System;
using System.Collections.Generic;

namespace ModernPlayerManagementAPI.Models.DTOs
{
    public class TeamDTO
    {
        public Guid Id;
        public string Name { get; set; }
        public UserDTO Manager { get; set; }
        public bool isCurrentUserManager { get; set; }
        public ICollection<UserDTO> Members { get; set; }
    }
}