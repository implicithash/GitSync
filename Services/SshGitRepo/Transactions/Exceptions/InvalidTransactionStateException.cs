using EW.Navigator.SCM.SshGit.Entities;
using System;

namespace EW.Navigator.SCM.GitRepo.Sync.Transactions.Exceptions
{
    [Serializable]
    public class InvalidTransactionStateException : GitTransactionException
    {
        public InvalidTransactionStateException(TransactionState expectedState, TransactionState actualState) : base($"The transaction is in state '{actualState}' but was expected to be in state '{expectedState}'")
        {
        }
    }
}
