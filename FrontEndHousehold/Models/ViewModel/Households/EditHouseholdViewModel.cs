using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FrontEndHousehold.Models.ViewModel.Households
{
    public class EditHouseholdViewModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}