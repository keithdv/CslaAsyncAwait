using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CslaAsyncAwait.Lib.Server
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<AutofacWcfPortal>().AsSelf();

            builder.RegisterType<ContextAdapter>().As<IContextAdapter>();

            builder.RegisterType<DataPortalActivator>()
                .As<IDataPortalActivator>()
                .As<IDataPortalActivatorServer>()
                .InstancePerLifetimeScope();
        }

    }
}
