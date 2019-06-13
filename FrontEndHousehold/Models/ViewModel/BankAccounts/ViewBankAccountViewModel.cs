using FrontEndHousehold.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FrontEndHousehold.Models.ViewModel.BankAccounts
{
    public class ViewBankAccountViewModel
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public decimal Balance { get; set; }
        public bool IsOwner { get; set; }

        public ViewBankAccountViewModel()
        {

        }

        public ViewBankAccountViewModel(BankAccount account)
        {
            AccountId = account.AccountId;
            Name = account.Name;
            Description = account.Description;
            DateCreated = account.DateCreated;
            DateUpdated = account.DateUpdated;
            Balance = account.Balance;
            IsOwner = account.IsOwner;
        }
    }
}