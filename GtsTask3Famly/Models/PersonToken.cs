using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GtsTask3Famly.Models
{
    public class PersonToken
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string UserId  { get; set; }
        public string Email { get; set; }
        public int PersonId { get; set; }
        public DateTime Date { get; set; }

    }
}
