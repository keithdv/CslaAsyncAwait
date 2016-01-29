using Autofac;
using Csla.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CslaAsyncAwait.Lib.Server
{
    public interface IDataPortalActivator : Csla.Server.IDataPortalActivator
    {


    }

    // Make it internal
    // We don't want this to provide access to the container to the application
    // If used it would be a service reference
    // Only AutofacWcfPortal, etc should access this
    internal interface IDataPortalActivatorServer : IDataPortalActivator
    {
        ILifetimeScope Container { get; set; }

    }
}
