using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GtsTask3Famly.Models
{
    public class RelRole
    {

        public RelRole()
        {
            Relationships = new HashSet<Relationship>();
        }
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int? GenderId { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual ICollection<Relationship> Relationships { get; set; }
    }
}
