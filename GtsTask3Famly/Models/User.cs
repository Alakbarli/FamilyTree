using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GtsTask3Famly.Models
{
    public class User:IdentityUser
    {
        [Required,StringLength(50)]
        public string Firstname { get; set; }
        [Required, StringLength(50)]
        public string Lastname { get; set; }
        public DateTime BirthDate { get; set; }
        public int GenderId { get; set; }
        public virtual  Gender Gender { get; set; }
        public virtual UserToPerson UserToPerson { get; set; }
        public virtual FamilyToUser FamilyToUser { get; set; }
        [NotMapped]
        public IFormFile ProfilePhoto { get; set; }
        public string Avatar { get; set; }
    }
}
