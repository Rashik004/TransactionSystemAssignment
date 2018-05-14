using System;
using TransactionSystem.Models;

namespace TransactionSystem.Contract
{
    public interface ITransaction
    {
        bool IsExecuted { get; }

        Guid TransactionGuid { get; }

        DateTime CreateDate { get; }

        bool Process();

        ITransaction GetRollbackTransaction();
    }
}
