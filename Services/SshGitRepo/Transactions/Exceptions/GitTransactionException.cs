using System;
namespace EW.Navigator.SCM.GitRepo.Sync.Transactions.Exceptions
{
    [Serializable]
    public class GitTransactionException : Exception
    {
        public GitTransactionException(string message, Exception innerException) : base(message, innerException)
        {

        }
        public GitTransactionException(string message) : base(message)
        {

        }
    }
}
