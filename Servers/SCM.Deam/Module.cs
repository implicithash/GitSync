using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceModel;
using EW.Navigator.SCM.Contracts;
using EW.Navigator.SCM.Services;
using EW.Navigator.Servers.Application.Interface;
using EW.Navigator.Servers.Common;
using EW.Navigator.Servers.DEAM.Contracts;
using EW.Navigator.Servers.DEAM.Interfaces;
using Microsoft.Practices.Unity;
using EW.Navigator.Utilities;

namespace EW.EProcessing.SCM.Deam
{
    public class Module : IApplicationServerModule, IDisposable
    {
        private static readonly TraceSource Ts = new TraceSource("SCMModule");
        private bool _disposed;
        private readonly IUnityContainer _container;

        /// <summary>
        /// Apply the configured timeout after which the service is due to be closed
        /// </summary>
        private readonly TimeSpan _closeTimeout = TimeSpan.FromSeconds(3);
        //private UnityServiceHost<AppConfigurationManager> _appManagerServiceHost;

        public Module(IUnityContainer container)
        {
            _container = container.CreateChildContainer();

            _container.RegisterType<IConfigurationManager, AppConfigurationManager>(new ContainerControlledLifetimeManager());
        }

        public void Initialize(Configuration settings)
        {

        }

        public void Start()
        {
            //var tcpBaseAddress = _container.Resolve<Uri>(AddressNames.TcpBaseAddressName);
            //var httpBaseAddress = _container.Resolve<Uri>(AddressNames.HttpBaseAddressName);

            //_appManagerServiceHost = new UnityServiceHost<AppConfigurationManager>(_container, tcpBaseAddress, httpBaseAddress);
            //_appManagerServiceHost.Open();

            var configurationManager = _container.Resolve<IConfigurationManager>();
        }

        public void Stop()
        {
            //CloseHosts();
        }

        public void Pause()
        {
        }

        public void Continue()
        {

        }

        public void Dispose()
        {
            if (_disposed) return;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void CloseHost(ServiceHost host)
        {
            try
            {
                if (host != null &&
                    (host.State == CommunicationState.Opened
                     || host.State == CommunicationState.Opening)) host.Close(_closeTimeout);
            }
            catch (Exception ex)
            {
                Ts.TraceEvent(TraceEventType.Error, () => $"Error at SCM.Deam.Module, host - {host}, error:\r\n{ex}");
                host?.Abort();
            }
        }

        private void StopAction(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Ts.TraceEvent(TraceEventType.Error, 0, "Ошибка в DEAM.Application.WCFApplication.StopAction, action-{0}, ex:\r\n{1}", action, ex);
            }
        }

        private void CloseHosts()
        {
            //CloseHost(_appManagerServiceHost);
        }

        public virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            CloseHosts();
            _disposed = true;
        }
        ~Module()
        {
            Dispose(false);
        }
    }
}
