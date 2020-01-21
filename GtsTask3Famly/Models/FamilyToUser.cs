using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GtsTask3Famly.Models
{
    public class FamilyToUser
    {
        public int Id { get; set; }
        public int FamilyId { get; set; }
        public string UserId { get; set; }
        public virtual Family Family { get; set; }
        public virtual User User { get; set; }
    }
}
