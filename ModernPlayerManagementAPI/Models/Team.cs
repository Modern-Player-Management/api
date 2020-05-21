using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernPlayerManagementAPI.Models
{
    public class Team : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public Guid ManagerId { get; set; }

        [ForeignKey("ManagerId")] 
        public User Manager { get; set; }

        public ICollection<Membership> Memberships { get; set; }
    }
}