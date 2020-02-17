using System;
using Unity;
using Unity.Injection;
using EW.Navigator.SCM.RestfulGit.Interfaces;
using EW.Navigator.SCM.Services.Interfaces;

namespace EW.Navigator.SCM.RestfulGit.Sync
{
    public class DefaultQueryFactory : IQueryFactory
    {
        private readonly Func<QueryType, IQuery> _queryFactory;

        public DefaultQueryFactory(Func<QueryType, IQuery> queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public IQuery Create(QueryType request)
        {
            return _queryFactory(request);
        }
    }

    public static class RegisterFactory
    {
        public static IUnityContainer Configure(IHttpRequestConfiguration config)
        {
            var container = new UnityContainer();
            //register the queries using named mappings
            var injectedContext = new InjectionConstructor(new CommitsRequest()
            {
                Url = config.Url,
                RefName = config.RefName,
                StartSha = config.StartSha,
                Limit = config.Limit
            });
            container.RegisterType<IQuery, CommitsQuery>(QueryType.Commits.ToString(), injectedContext);
            injectedContext = new InjectionConstructor(new ShaCommitRequest()
            {
                Url = config.Url,
                Sha = config.Sha
            });
            container.RegisterType<IQuery, ShaCommitQuery>(QueryType.OneCommit.ToString(), injectedContext);
            //create the strategy
            IQuery QueryFactory(QueryType queryType) =>
                container.Resolve<IQuery>(queryType.ToString()) ?? NullQuery.Empty;
            //register factory
            var factory = new DefaultQueryFactory(QueryFactory);
            container.RegisterInstance<IQueryFactory>(factory);

            return container;
        }
    }
}
