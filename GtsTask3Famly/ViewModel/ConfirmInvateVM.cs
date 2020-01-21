using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GtsTask3Famly.ViewModel
{
    public class ConfirmInvateVM
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Token { get; set; }
        public int FamilyId { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password), ErrorMessage = "The verification password does not match your password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
