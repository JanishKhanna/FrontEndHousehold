using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FrontEndHousehold.Models.ViewModel
{
    public class InviteUserViewModel
    {
        public int HouseholdId { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}