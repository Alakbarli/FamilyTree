using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GtsTask3Famly.ViewModel
{
    public class RelationshipJsonVM
    {
        public int MainUserId { get; set; }
        public int? PersonId { get; set; }
        public string MainUserName { get; set; }
        
        public int MainUserGenderId { get; set; }
        public string RoleName { get; set; }
        public string MainUserPhoto { get; set; }
        
        public int FamilyId { get; set; }
        
    }
}
