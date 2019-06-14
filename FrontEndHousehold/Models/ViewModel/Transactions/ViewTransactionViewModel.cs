using FrontEndHousehold.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FrontEndHousehold.Models.ViewModel.Transactions
{
    public class ViewTransactionViewModel
    {
        public int TransactionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateOfTransaction { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool VoidTransaction { get; set; }
        public int CategoryId { get; set; }
        public int BankAccountId { get; set; }
        public string OwnerOfTransactionId { get; set; }
        public bool IsOwner { get; set; }

        public ViewTransactionViewModel()
        {

        }

        public ViewTransactionViewModel(Transaction transaction)
        {
            IsOwner = transaction.IsOwner;
            CategoryId = transaction.CategoryId;
            BankAccountId = transaction.BankAccountId;
            OwnerOfTransactionId = transaction.OwnerOfTransactionId;
            TransactionId = transaction.TransactionId;
            Title = transaction.Title;
            Description = transaction.Description;
            DateOfTransaction = transaction.DateOfTransaction;
            Amount = transaction.Amount;
            DateCreated = transaction.DateCreated;
            DateUpdated = transaction.DateUpdated;
            VoidTransaction = transaction.VoidTransaction;
        }
    }
}