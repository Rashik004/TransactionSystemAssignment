using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransactionSystem;
using TransactionSystem.Models;
using TransactionSystem.Transactions;

namespace TransactionSystemTests
{
    [TestClass]
    public class TransactionTests
    {
        private TransactionManager _transactionManager = new TransactionManager();

        [TestMethod]
        public void DepositAccountCheckBalanceAndThenWithdraw_AllTransactionsSuccessful()
        {
            // Create Account with Balance 0
            var initialAccount1 = new Account() {Balance = 0, AccountGuid = Guid.NewGuid() };

            // Add a deposit reqest of 100 to that account
            _transactionManager.AddTransaction(new Deposit(initialAccount1, 100));

            // Pre-check : transactionManager Should have pending transactions at this point
            Assert.IsTrue(_transactionManager.HasPendingTransactions());

            // Account Balance Should be 0 at this point
            Assert.AreEqual(initialAccount1.Balance, 0);

            // Run ProcessPendingTransactions() to process Pending TransactionRequests
            _transactionManager.ProcessPendingTransactions();

            // Check : there should not be any pending Transactions now
            Assert.IsFalse(_transactionManager.HasPendingTransactions());

            // Check balance of the account which should be 100 now
            Assert.AreEqual(initialAccount1.Balance, 100);

            // Create a Withdraw of 50 on that account
            _transactionManager.AddTransaction(new Withdraw(initialAccount1, 50));

            // Perform a ProcessPendingTransactions() to process this
            _transactionManager.ProcessPendingTransactions();

            // check : No pending Transactions at this point
            Assert.IsFalse(_transactionManager.HasPendingTransactions());

            // Check balance: should be 100-50 =50
            Assert.AreEqual(initialAccount1.Balance, 50);
        }

        [TestMethod]
        public void Test_WithDrawRequestForAmountGreaterThanAvailableBalance_TransactionExecutedWhenBalanceConstrainMet()
        {
            // Create account with 75 as initial Balance
            var initialAccount1 = new Account() {Balance = 75, AccountGuid = Guid.NewGuid() };

            // Add a withdraw request of 100 (exceeding the available balance)
            _transactionManager.AddTransaction(new Withdraw(initialAccount1, 100));

            // Check Balance: should be 75 
            Assert.AreEqual(initialAccount1.Balance, 75);

            // Run ProcessPendingTransactions() to process Pending TransactionRequests
            _transactionManager.ProcessPendingTransactions();

            // Add a deposit request to that account of 75
            _transactionManager.AddTransaction(new Deposit(initialAccount1, 75));

            // Run ProcessPendingTransactions() to process Pending TransactionRequests
            _transactionManager.ProcessPendingTransactions();

            // check : there should be pending transactions at this point
            Assert.IsTrue(_transactionManager.HasPendingTransactions());

            // check Balance: should be 150
            Assert.AreEqual(initialAccount1.Balance, 150);

            // Run ProcessPendingTransactions() to process Pending TransactionRequests
            _transactionManager.ProcessPendingTransactions();

            // Check: there should be no pending transactions at this point
            Assert.IsFalse(_transactionManager.HasPendingTransactions());

            // Balance Check : should be 50 (withdraw request of 100 should be successfull at this point)
            Assert.AreEqual(initialAccount1.Balance, 50);

        }

        [TestMethod]
        public void Test_TransferRequestForAmountGreaterThanAvailableBalance_TransactionExecutedWhenBalanceConstrainMet()
        {
            // Create firstAccount with Initial Balance 100
            var initialAccount1 = new Account() { Balance = 100, AccountGuid = Guid.NewGuid() };

            // Create secondAccount with Initial Balance 2000
            var initialAccount2 = new Account() { Balance = 2000, AccountGuid = Guid.NewGuid() };

            // Create a transfer request of 700 from firstAccount to secondAccount
            _transactionManager.AddTransaction(new Transfer(initialAccount1, initialAccount2, 700));

            // Run ProcessPendingTransactions() to process Pending TransactionRequests
            _transactionManager.ProcessPendingTransactions();

            // Balance Check : FirstAccount -> 100
            Assert.AreEqual(initialAccount1.Balance, 100);

            // Balance Check : SecondAccount -> 2000
            Assert.AreEqual(initialAccount2.Balance, 2000);

            // Add a Deposit request of 900 to FirstAccount
            _transactionManager.AddTransaction(new Deposit(initialAccount1, 900));

            // Run ProcessPendingTransactions() to process Pending TransactionRequests
            _transactionManager.ProcessPendingTransactions();

            // Balance Check : FirstAccount: 1000
            Assert.AreEqual(initialAccount1.Balance, 1000);

            // Run ProcessPendingTransactions() to process Pending TransactionRequests
            _transactionManager.ProcessPendingTransactions();

            // Check: there should be no pending transactions at this point
            Assert.IsFalse(_transactionManager.HasPendingTransactions());

            // Balance Check : FirstAccount -> 300
            Assert.AreEqual(initialAccount1.Balance, 300);

            // Balance Check : SecondAccount -> 2700
            Assert.AreEqual(initialAccount2.Balance, 2700);

        }

        [TestMethod]
        public void Test_Transfer_ThenRollback_AccountStatusRegainedItsInitialState()
        {
            // Create firstAccount with Initial Balance 2000
            var initialAccount1 = new Account() { Balance = 2000, AccountGuid = Guid.NewGuid() };

            // Create secondAccount with Initial Balance 100
            var initialAccount2 = new Account() { Balance = 100, AccountGuid = Guid.NewGuid() };

            // Create a transfer request of 700 from firstAccount to secondAccount
            var transfer = new Transfer(initialAccount1, initialAccount2, 700);
            _transactionManager.AddTransaction(transfer);

            // Run ProcessPendingTransactions() to process Pending TransactionRequests
            _transactionManager.ProcessPendingTransactions();

            // Balance Check : FirstAccount -> 1300
            Assert.AreEqual(initialAccount1.Balance, 1300);

            // Balance Check : SecondAccount -> 800
            Assert.AreEqual(initialAccount2.Balance, 800);

            // Perform a Rollback with the transaction Id of the transfer which is made
            _transactionManager.RollbackTransaction(transfer.TransactionGuid);

            // Check: there should be no pending transactions at this point
            Assert.IsFalse(_transactionManager.HasPendingTransactions());

            // Balance Check : FirstAccount -> 2000
            Assert.AreEqual(initialAccount1.Balance, 2000);

            // Balance Check : SecondAccount -> 100
            Assert.AreEqual(initialAccount2.Balance, 100);

        }

        [TestMethod]
        public void Test_Transfer_ThenWithdrawFromTheSecondAccount_ThenRollback()
        {
            // Create firstAccount with Initial Balance 2000
            var initialAccount1 = new Account() { Balance = 2000, AccountGuid = Guid.NewGuid() };

            // Create secondAccount with Initial Balance 100
            var initialAccount2 = new Account() { Balance = 100, AccountGuid = Guid.NewGuid() };

            // Create a transfer request of 700 from firstAccount to secondAccount
            var transfer=new Transfer(initialAccount1, initialAccount2, 700);
            _transactionManager.AddTransaction(transfer);

            // Run ProcessPendingTransactions() to process Pending TransactionRequests
            _transactionManager.ProcessPendingTransactions();

            // Balance Check : FirstAccount -> 1300
            Assert.AreEqual(initialAccount1.Balance, 1300);

            // Balance Check : SecondAccount -> 800
            Assert.AreEqual(initialAccount2.Balance, 800);

            // Create a withdraw request of 600 from the secondAccount
            var withdraw = new Withdraw(initialAccount2, 600);
            _transactionManager.AddTransaction(withdraw);

            // Run ProcessPendingTransactions() to process Pending TransactionRequests
            _transactionManager.ProcessPendingTransactions();

            // Perform a Rollback with the transaction Id of the transfer which is made (Rollback should not be executed because of balance constraint)
            _transactionManager.RollbackTransaction(transfer.TransactionGuid);

            // Perform a Rollback with the transaction Id of the withdraw which was made
            _transactionManager.RollbackTransaction(withdraw.TransactionGuid);

            // Run ProcessPendingTransactions() to process Pending TransactionRequests
            _transactionManager.ProcessPendingTransactions();

            // Balance Check : FirstAccount -> 2000
            Assert.AreEqual(initialAccount1.Balance, 2000);

            // Balance Check : SecondAccount -> 100
            Assert.AreEqual(initialAccount2.Balance, 100);
        }

    }
}
