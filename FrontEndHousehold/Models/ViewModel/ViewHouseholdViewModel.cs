using FrontEndHousehold.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FrontEndHousehold.Models.ViewModel
{
    public class ViewHouseholdViewModel
    {
        public string Name { get; set; }        
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public ViewHouseholdViewModel()
        {

        }

        public ViewHouseholdViewModel(Household household)
        {
            Name = household.Name;
            Description = household.Description;
            DateCreated = household.DateCreated;
            DateUpdated = household.DateUpdated;
        }
    }
}