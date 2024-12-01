using System;

namespace MyApp.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }  // "Income" or "Expense"
        public string Description { get; set; }
        public int UsersId { get; set; }
    }
}