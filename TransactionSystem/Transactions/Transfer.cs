using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionSystem.Contract;
using TransactionSystem.Models;

namespace TransactionSystem.Transactions
{
    public class Transfer : TransactionBase
    {
        private readonly Account secondaryAccount;
        private readonly Deposit deposit;
        private readonly Withdraw withdraw;
        public Transfer(Account primaryAccount,Account secondaryAccount, decimal amount) : base(primaryAccount, amount)
        {
            this.secondaryAccount = secondaryAccount;
            deposit = new Deposit(secondaryAccount, amount);
            withdraw = new Withdraw(primaryAccount, amount);
        }

        public override bool Process()
        {
            return IsExecuted = withdraw.Process() && deposit.Process();
        }

        public override ITransaction GetRollbackTransaction()
        {
            return new Transfer(this.secondaryAccount, this.PrimaryAccount, this.Amount);
        }
    }
}
