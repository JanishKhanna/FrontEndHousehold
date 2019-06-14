using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FrontEndHousehold.Models.ViewModel.Transactions
{
    public class EditTransactionViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateOfTransaction { get; set; }
        public decimal Amount { get; set; }
        //public int CategoryId { get; set; }
        //public SelectList Categories { get; set; } 
    }
}