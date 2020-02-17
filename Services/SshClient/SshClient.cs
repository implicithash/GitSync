using System;
using System.Threading;
using System.Threading.Tasks;
using EW.Navigator.SCM.SshClient.Interfaces;
using Renci.SshNet;

namespace EW.Navigator.SCM.SshClient
{
    public class SshCredentials : ISshClientCredentials
    {
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; } = 22;
        public string PrivateKey { get; set; }
        public string PassPhrase { get; set; }
    }

    public class SshClient : ISshClient
    {
        private readonly ISshClientCredentials _sshCredentials;
        private readonly ConnectionInfo _connectionInfo;

        public SshClient(ISshClientCredentials sshCredentials)
        {
            _sshCredentials = sshCredentials;
            if (_sshCredentials != null)
                _connectionInfo = SetupCredentials();
        }

        private ConnectionInfo SetupCredentials()
        {
            var connectionInfo = new ConnectionInfo(_sshCredentials.Host, _sshCredentials.Port, _sshCredentials.UserName,
                new AuthenticationMethod[]{
                    new PasswordAuthenticationMethod(_sshCredentials.UserName, _sshCredentials.Password)/*,

                    // key based authentication (using keys in OpenSSH Format)
                    new PrivateKeyAuthenticationMethod(_sshCredentials.UserName, new PrivateKeyFile[]{
                        new PrivateKeyFile(@"openssh.key","passphrase")
                    }),*/
                }
            );
            return connectionInfo;
        }

        public async Task<bool> ExecuteShellCommand(string command)
        {
            using (var sshClient = new Renci.SshNet.SshClient(_connectionInfo))
            {
                //if(!sshClient.IsConnected) return false;
                sshClient.Connect();

                await ExecuteCommandAsync(sshClient, command);

                //var result = sshClient.CreateCommand(command).Execute();
                sshClient.Disconnect();

                return true;
            }
        }

        private async Task ExecuteCommandAsync(Renci.SshNet.SshClient sshClient, string command)
        {
            var progress = new Progress<ScriptOutputLine>();
            progress.ProgressChanged += delegate (object sender, ScriptOutputLine line)
            {
                Console.WriteLine(line.Line);
            };
            await sshClient.CreateCommand(command).ExecuteAsync(progress, new CancellationToken());
        }
    }
}