using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionSystem.Contract;
using TransactionSystem.Models;

namespace TransactionSystem.Transactions
{
    public class Withdraw: TransactionBase
    {
        public Withdraw(Account primaryAccount, decimal amount) : base(primaryAccount, amount)
        {
        }

        public override bool Process()
        {
            if (PrimaryAccount.Balance < Amount)
            {
                return IsExecuted = false;
            }
            PrimaryAccount.Balance -= Amount;
            return IsExecuted = true;
        }

        public override ITransaction GetRollbackTransaction()
        {
            return new Deposit(PrimaryAccount, Amount);
        }
    }
}
