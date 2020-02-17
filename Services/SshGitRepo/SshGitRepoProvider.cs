using EW.Navigator.SCM.Contracts;
using EW.Navigator.SCM.SshGitRepo.Interfaces;
using System;
using System.Threading.Tasks;

namespace EW.Navigator.SCM.GitRepo.Sync
{
    public abstract class SshGitRepoProvider<TReq, TResult> where TReq : IRepoRequest
        where TResult : IRepoResult
    {
        public virtual Task<TResult> GitRepoSync(TReq request)
        {
            throw new NotImplementedException();
        }
    }

    public class SshGitAnsibleRepoProvider : SshGitRepoProvider<IRepoApplicationRequest, IRepoResult>
    {
        public override async Task<IRepoResult> GitRepoSync(IRepoApplicationRequest request)
        {
            var gitRepository = new GitRepository(request.SshCredentials);
            var result = await gitRepository.RemoteRepoSync(request);
            return result;
        }
    }
}
