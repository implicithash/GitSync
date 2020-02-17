namespace EW.Navigator.SCM.RestfulGit.Interfaces
{
    /// <summary>
    /// Declaration of query factory
    /// </summary>
    public interface IQueryFactory
    {
        /// <summary>
        /// Creating a query for a commit of definite type
        /// </summary>
        /// <param name="queryType">Type of the query</param>
        /// <returns></returns>
        IQuery Create(QueryType queryType);
    }

    /// <summary>
    /// Query type
    /// </summary>
    public enum QueryType
    {
        Commits = 1,
        OneCommit = 2
    }
}

