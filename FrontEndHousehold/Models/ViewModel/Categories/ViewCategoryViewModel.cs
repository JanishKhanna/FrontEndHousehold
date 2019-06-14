using FrontEndHousehold.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FrontEndHousehold.Models.ViewModel.Categories
{
    public class ViewCategoryViewModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int HouseholdId { get; set; }
        public bool IsOwner { get; set; }

        public ViewCategoryViewModel()
        {

        }

        public ViewCategoryViewModel(Category category)
        {
            HouseholdId = category.HouseholdId;
            CategoryId = category.CategoryId;
            Name = category.Name;
            Description = category.Description;
            DateCreated = category.DateCreated;
            DateUpdated = category.DateUpdated;
        }
    }
}