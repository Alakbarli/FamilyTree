using System.Collections.Generic;

namespace GtsTask3Famly.Models
{
    public class Gender
    {
        public Gender()
        {
            People = new HashSet<Person>();
            Roles = new HashSet<RelRole>();
            Users = new HashSet<User>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Person> People { get; set; }
        public ICollection<RelRole> Roles { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
