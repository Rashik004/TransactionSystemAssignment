using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionSystem.Contract;
using TransactionSystem.Models;

namespace TransactionSystem.Transactions
{
    public class Deposit: TransactionBase
    {
        public Deposit(Account primaryAccount, decimal amount) : base(primaryAccount, amount)
        {
        }

        public override bool Process()
        {
            PrimaryAccount.Balance += Amount;
            return IsExecuted = true;
        }

        public override ITransaction GetRollbackTransaction()
        {
            return new Withdraw(PrimaryAccount, Amount);
        }
    }
}
