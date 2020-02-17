using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using EW.Navigator.SCM.GitRepo.Sync.Extensions;
using EW.Navigator.SCM.GitRepo.Sync.Transactions.Exceptions;
using EW.Navigator.SCM.GitRepo.Sync.Utilities;
using EW.Navigator.SCM.SshGit.Entities;
using EW.Navigator.SCM.SshGitRepo.Interfaces;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace EW.Navigator.SCM.GitRepo.Sync.Transactions
{
    public abstract class BaseGitTransaction : IGitTransaction
    {
        protected readonly string Origin = "origin";

        public TransactionState State { get; protected set; } = TransactionState.Created;

        public string RemotePath { get; }

        public string LocalPath { get; }

        public SshUserKeyCredentials SshCredentials { get; }

        protected BaseGitTransaction(string remotePath, string localPath, SshUserKeyCredentials credentials)
        {
            RemotePath = remotePath ?? throw new ArgumentNullException(nameof(remotePath));
            LocalPath = localPath ?? throw new ArgumentNullException(nameof(localPath));
            SshCredentials = credentials ?? throw new ArgumentNullException(Properties.Resources.SshCredentials_Empty);
        }


        /// <summary>
        /// Begin a transaction
        /// </summary>
        public virtual void Begin()
        {
            EnsureIsInState(TransactionState.Created);

            if (!EnsureCanCloneIntoLocalDirectory())
            {
                Reset();
                State = TransactionState.Active;
                return;
            }

            try
            {
                var cloneOptions = SetOptions<CloneOptions>();
                Repository.Clone(RemotePath, LocalPath, cloneOptions);
            }
            catch (LibGit2SharpException)
            {
                Reset();
            }
            State = TransactionState.Active;
        }

        /// <summary>
        /// Set clone/fetch options
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T SetOptions<T>() where T : FetchOptionsBase, new()
        {
            var options = new T
            {
                CredentialsProvider = new CredentialsHandler((url, usernameFromUrl, types) => SshCredentials)
            };
            return options;
        }

        /// <summary>
        /// Set push options
        /// </summary>
        /// <returns></returns>
        public virtual PushOptions SetPushOptions()
        {
            var options = new PushOptions
            {
                CredentialsProvider = new CredentialsHandler((url, usernameFromUrl, types) => SshCredentials)
            };
            return options;
        }

        /// <summary>
        /// Hard reset of the local git repository
        /// </summary>
        public virtual void Reset()
        {
            using (var localRepository = new Repository(LocalPath))
            {
                localRepository.Reset(ResetMode.Hard);
                localRepository.RemoveUntrackedFiles();
            }
        }

        /// <summary>
        /// Index all the files which have been changed
        /// </summary>
        public virtual void IndexAll()
        {
            using (var localRepository = new Repository(LocalPath))
            {
                Commands.Stage(localRepository, "*");
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Commit the changes
        /// </summary>
        /// <param name="defaultMessage"></param>
        public virtual void Commit(string defaultMessage)
        {
            EnsureIsInState(TransactionState.Active);

            /*var canCommit = CanCommit();
            if (!canCommit)
            {
                State = TransactionState.Failed;
                OnTransactionAborted();
                throw new TransactionFailedException(Properties.Resources.TransactionFailedMessage);
            }*/

            try
            {
                using (var localRepository = new Repository(LocalPath))
                {
                    // set user credentials for push options
                    var pushOptions = SetPushOptions();

                    // index all the files into the repo folder
                    IndexAll();

                    var signature = localRepository.Config.BuildSignature(DateTimeOffset.Now) ??
                                    new Signature(RepoSettings.Default.DefaultUserName, RepoSettings.Default.DefaultEmail, DateTimeOffset.Now);

                    localRepository.Commit(defaultMessage, signature, signature);

                    // push all branches with local changes and all tags
                    var branchesToPush = GetBranchesToPush(localRepository).ToList();

                    var refSpecs = branchesToPush.ToRefSpecs().Union(localRepository.Tags.ToRefSpecs());
                    localRepository.Network.Push(localRepository.Network.Remotes[Origin], refSpecs, pushOptions);
                }

                State = TransactionState.Completed;
                OnTransactionCompleted();
            }
            catch (NonFastForwardException ex)
            {
                State = TransactionState.Failed;
                OnTransactionAborted();
                throw new TransactionFailedException(Properties.Resources.TransactionFailedMessage, ex);
            }
            catch (EmptyCommitException ex)
            {
                throw new EmptyCommitException(ex.Message);
            }
        }

        private bool EnsureCanCloneIntoLocalDirectory()
        {
            if (Directory.Exists(LocalPath))
            {
                if (Directory.EnumerateFiles(LocalPath).Any())
                {
                    return false;
                }
            }
            else
            {
                Directory.CreateDirectory(LocalPath);
            }

            return true;
        }

        protected void EnsureIsInState(TransactionState expectedState)
        {
            if (State != expectedState)
            {
                throw new InvalidTransactionStateException(expectedState, State);
            }
        }

        private bool CanCommit()
        {
            using (var localRepository = new Repository(LocalPath))
            {
                // fetch changes from the remote repository
                var options = SetOptions<FetchOptions>();
                localRepository.FetchOrigin(options);

                var localBranches = localRepository.Branches.GetLocalBranches().ToList();

                if ((from branch in localBranches.Where(b => b.IsTracking)
                     let hasRemoteChanges = branch.TrackingDetails.BehindBy > 0
                     let hasLocalChanges = branch.TrackingDetails.AheadBy > 0
                     where hasRemoteChanges && hasLocalChanges
                     select hasRemoteChanges).Any())
                {
                    return false;
                }
            }
            return true;
        }

        private IEnumerable<Branch> GetBranchesToPush(IRepository localRepository)
        {
            var localBranches = localRepository.Branches.GetLocalBranches().ToList();

            foreach (var branch in localBranches)
            {
                if (branch.IsTracking)
                {
                    // check if there are changes to the branch locally or in the remote branch
                    var hasLocalChanges = branch.TrackingDetails.AheadBy > 0;

                    // if the branch has local changes, add it to the list of branches we need to push         
                    if (hasLocalChanges)
                    {
                        yield return branch;
                    }
                }
                else
                {
                    // set up the local branch to track the corresponding remote branch
                    localRepository.Branches.Update(
                        branch,
                        b => b.Remote = Origin,
                        b => b.UpstreamBranch = branch.CanonicalName);

                    yield return branch;
                }
            }

        }
        protected virtual void OnTransactionAborted() => DirectoryHelper.DeleteRecursively(LocalPath);

        protected abstract void OnTransactionCompleted();
    }
}
