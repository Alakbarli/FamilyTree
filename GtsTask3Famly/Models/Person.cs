using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GtsTask3Famly.Models
{
    public class Person
    {
        public Person()
        {
            Relationships = new HashSet<Relationship>();
        }
        public int Id { get; set; }
        public string Photo { get; set; }
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime Birthdate { get; set; }
        public int Age { get; set; }
        [Required]
        public int GenderId { get; set; }
        public int FamilyId { get; set; }
        public virtual Family Family { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual ICollection<Relationship> Relationships { get; set; }
        [NotMapped]
        public IFormFile PhotoFile { get; set; }
        public virtual UserToPerson UserToPerson { get; set; }

    }
}
