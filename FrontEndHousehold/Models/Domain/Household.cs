using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FrontEndHousehold.Models.Domain
{
    public class Household
    {
        public int HouseholdId { get; set; }
        public string Name { get; set; }        
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int NumberOfUsers { get; set; }
        public bool IsOwner { get; set; }

        public Household()
        {
            DateCreated = DateTime.Now;
        }
    }
}