using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Csla.DataPortalClient;

namespace CslaAsyncAwait.Lib.Server
{
    public class BasicHttpBindingWcfProxy : WcfProxy
    {

        public BasicHttpBindingWcfProxy() : base()
        {

        }

        protected override ChannelFactory<IWcfPortal> GetChannelFactory()
        {
            //ChannelFactory<IWcfPortal> factory;

            //factory = new ChannelFactory<IWcfPortal>("GRiDSDataPortalEndpoint");

            //return factory;


            ChannelFactory<Csla.DataPortalClient.IWcfPortal> factory = base.GetChannelFactory();

            factory.Credentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation;

            BasicHttpBinding basicBinding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
            basicBinding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            basicBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
            basicBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            basicBinding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            basicBinding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            basicBinding.MaxBufferPoolSize = int.MaxValue;
            basicBinding.MaxReceivedMessageSize = int.MaxValue;
            basicBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;

            factory.Endpoint.Binding = basicBinding;

            return factory;
        }
    }
}
