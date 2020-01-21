using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GtsTask3Famly.ViewModel
{
    public class InvateEmail
    {
        [Required]
        [StringLength(maximumLength: 50)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public int PersonId { get; set; }
        [Required]
        public int FamilyId { get; set; }
    }
}
