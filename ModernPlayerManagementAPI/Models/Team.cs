﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernPlayerManagementAPI.Models
{
    public class Team : BaseEntity
    {
        public string Name { get; set; }
        public Guid ManagerId { get; set; }

        [ForeignKey("ManagerId")] 
        public User Manager { get; set; }

        public ICollection<User> Members { get; set; }
    }
}