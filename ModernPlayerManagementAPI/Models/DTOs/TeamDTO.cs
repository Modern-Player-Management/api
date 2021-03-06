﻿using System;
using System.Collections.Generic;

namespace ModernPlayerManagementAPI.Models.DTOs
{
    public class TeamDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public UserDTO Manager { get; set; }
        public bool isCurrentUserManager { get; set; }
        public ICollection<UserDTO> Players { get; set; }
        public DateTime Created { get; set; }
    }
}