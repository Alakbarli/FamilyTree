using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GtsTask3Famly.Models
{
    public class Family
    {
        public Family()
        {
            Users = new HashSet<Relationship>();
            FamilyToUsers = new HashSet<FamilyToUser>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        [NotMapped]
        public IFormFile LogoFile { get; set; }
        public virtual ICollection<Relationship> Users { get; set; }
        public virtual ICollection<FamilyToUser> FamilyToUsers { get; set; }
        public virtual  ICollection<Person> FamilyMembers { get; set; }
    }
}
