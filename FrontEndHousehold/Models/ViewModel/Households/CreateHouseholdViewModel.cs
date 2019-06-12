using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FrontEndHousehold.Models.ViewModel.Households
{
    public class CreateHouseholdViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }        
    }
}