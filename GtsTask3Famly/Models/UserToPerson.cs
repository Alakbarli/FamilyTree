using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GtsTask3Famly.Models
{
    public class UserToPerson
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
        public virtual User User { get; set; }
    }
}
