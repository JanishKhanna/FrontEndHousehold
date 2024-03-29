﻿using FrontEndHousehold.Models.Domain;
using FrontEndHousehold.Models.ViewModel.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FrontEndHousehold.Models.ViewModel.Households
{
    public class ViewHouseholdViewModel
    {
        public int HouseholdId { get; set; }
        public string Name { get; set; }        
        public string Description { get; set; }
        public int NumberOfUsers { get; set; }
        public bool IsOwner { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public ViewHouseholdViewModel()
        {

        }

        public ViewHouseholdViewModel(Household household)
        {
            HouseholdId = household.HouseholdId;
            Name = household.Name;
            Description = household.Description;
            DateCreated = household.DateCreated;
            DateUpdated = household.DateUpdated;
            IsOwner = household.IsOwner;
            NumberOfUsers = household.NumberOfUsers;
        }
    }
}