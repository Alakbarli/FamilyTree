using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GtsTask3Famly.Models
{
    public class Relationship
    {
        public int Id { get; set; }
        public int? PersonId { get; set; }
        [Required]
        public int FamilyId { get; set; }
        public bool IsMain { get; set; }
        public int? RelRoleId { get; set; }
        [Required]
        public int RelatedUserId { get; set; }
        [NotMapped]
        public virtual Person RelatedUser { get; set; }
        public virtual RelRole Role { get; set; }
        public virtual Family Family { get; set; }
        public virtual Person Person { get; set; }
    }
}
