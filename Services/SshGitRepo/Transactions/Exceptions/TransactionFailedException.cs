using System;

namespace EW.Navigator.SCM.GitRepo.Sync.Transactions.Exceptions
{
    [Serializable]
    public class TransactionFailedException : GitTransactionException
    {
        public TransactionFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public TransactionFailedException(string message) : base(message)
        {
        }
    }
}

