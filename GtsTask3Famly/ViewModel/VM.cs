using GtsTask3Famly.Models;
using System.Collections.Generic;

namespace GtsTask3Famly.ViewModel
{
    public class VM
    {
        public Person Person { get; set; }
        public IEnumerable<RelRole> RelRoles { get; set; }
        public Relationship Mother { get; set; }
        public Relationship Father { get; set; }
        public ICollection<Relationship> Grandpa { get; set; }
        public ICollection<Relationship> Grandma { get; set; }
        public IEnumerable<Person> People { get; set; }
        public IEnumerable<Relationship> Relationships { get; set; }
        public Relationship Relationship { get; set; }
        public Family Family { get; set; }
        public IEnumerable<Family> Families { get; set; }
        public User User { get; set; }
        public IEnumerable<FamilyToUser> UsersToFamily { get; set; }

    }
}
