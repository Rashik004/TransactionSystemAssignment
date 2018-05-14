using System;
using System.Collections.Generic;
using System.Linq;
using TransactionSystem.Contract;

namespace TransactionSystem
{
    public class TransactionManager
    {
        //TODO:  Maintain a List of Transactions (Deposit/Withdraw/ Transfer )

        //TODO:  Add a Method for Adding Transaction
        private IList<ITransaction> transactions = new List<ITransaction>();
        
        
        
        public bool HasPendingTransactions()
        {
            // This should track if there is any pending transaction request (Deposit/Withdraw/ Transfer)
            return transactions.Any(t => !t.IsExecuted);
        }

        public void ProcessPendingTransactions()
        {
            // The logic for processing pending transaction sequentially goes here
            // It should track which are already processed and which are pending transactions

            var transactionsToBeExecuted =
                transactions.Where(t => !t.IsExecuted).OrderBy(t => t.CreateDate);
            foreach (var transaction in transactionsToBeExecuted)
            {
                transaction.Process();
            }
        }

        public void RollbackTransaction(Guid transactionId)
        {
            var transactionToBeRolledBack = transactions.FirstOrDefault(t => t.TransactionGuid == transactionId);
            if (transactionToBeRolledBack != null)
            {
                var rollbackTransaction = transactionToBeRolledBack.GetRollbackTransaction();
                rollbackTransaction.Process();
                this.AddTransaction(rollbackTransaction);
            }
        }

        public void AddTransaction(ITransaction transaction)
        {
            transactions.Add(transaction);
        }
    }
}
