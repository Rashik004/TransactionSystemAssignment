using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionSystem.Contract;
using TransactionSystem.Models;

namespace TransactionSystem.Transactions
{
    public abstract class TransactionBase : ITransaction
    {
        protected Account PrimaryAccount { get; }
        protected decimal Amount { get; }

        protected TransactionBase(Account primaryAccount, decimal amount)
        {
            this.PrimaryAccount = primaryAccount;
            this.Amount = amount;
            CreateDate = DateTime.UtcNow;
            TransactionGuid = Guid.NewGuid();
        }

        public bool IsExecuted { get; set; }
        public Guid TransactionGuid { get; }
        public DateTime CreateDate { get; }
        public abstract bool Process();
        public abstract ITransaction GetRollbackTransaction();
    }
}
