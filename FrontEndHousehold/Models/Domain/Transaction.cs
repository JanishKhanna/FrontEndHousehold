using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FrontEndHousehold.Models.Domain
{
    public class Transaction
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

        public Transaction()
        {
            DateCreated = DateTime.Now;
        }
    }
}