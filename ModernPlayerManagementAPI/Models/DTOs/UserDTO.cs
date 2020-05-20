﻿using System;
using System.ComponentModel.DataAnnotations;

namespace ModernPlayerManagementAPI.Models.DTOs
{
    public class UserDTO
    {
        [Required]
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
    }
}