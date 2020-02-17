using System;
using System.Threading.Tasks;
using EW.Navigator.SCM.SshClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SshClient.Tests
{
    [TestClass]
    public class UnitTestSshClient
    {
        [TestMethod]
        public async Task TestMethodSshClient()
        {
            var sshCredentials = new SshCredentials()
            {
                Host = CredentialConstants.Host,
                UserName = CredentialConstants.UserName,
                Password = CredentialConstants.Password,
                PrivateKey = CredentialConstants.PrivateKey,
                PassPhrase = CredentialConstants.PassPhrase
            };
            var sshClient = new EW.Navigator.SCM.SshClient.SshClient(sshCredentials);
            var command = "ls -a";

            //var result = await sshClient.ExecuteShellCommand(command);
            var result = Task.Run(() => sshClient.ExecuteShellCommand(command));

            await result.ContinueWith(tsk =>
            {
                if (tsk.Status == TaskStatus.RanToCompletion)
                    Assert.AreNotEqual(false, result.Result, "Not connected. Check the ssh credentials");
            });
        }

        /// <summary>
        /// Constants for unit tests
        /// </summary>
        public static class CredentialConstants
        {
            public static string Host { get; set; } = "192.168.3.93";
            public static string UserName { get; set; } = "admin";
            public static string Password { get; set; } = "admin";
            public static string PrivateKey { get; set; } = "";
            public static string PassPhrase { get; set; } = "";
        }
    }
}
