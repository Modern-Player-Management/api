using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernPlayerManagementAPI.Models
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public ICollection<Membership> Memberships { get; set; }
        [NotMapped]
        public string Token { get; set; }
    }
}