using System;

namespace EW.Navigator.SCM.GitRepo.Sync.Transactions.Exceptions
{
    public class TransactionCloneException : GitTransactionException
    {
        public TransactionCloneException(string message, Exception innerException) : base(message, innerException)
        {

        }
        public TransactionCloneException(string message) : base(message)
        {

        }
    }
}

