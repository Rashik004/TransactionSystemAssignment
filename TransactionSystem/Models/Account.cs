using System;

namespace TransactionSystem.Models
{
    public class Account
    {
        public Guid AccountGuid { get; set; }

        public decimal Balance { get; set; }
    }
}
