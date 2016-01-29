using Autofac;
using Csla.Server.Hosts;
using Csla.Server.Hosts.WcfChannel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace CslaAsyncAwait.Lib.Server
{

    /// <summary>
    /// Keeps the Autofac scope open per call
    /// </summary>

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AutofacWcfPortal : IWcfPortal
    {

        private ILifetimeScope _scope;
        private IDataPortalActivatorServer _dataPortalActivator;

        public AutofacWcfPortal(ILifetimeScope scope)
        {
            this._scope = scope;
            // Since IDataPortalActivatorServer is internal it can't be on a public constructor
            // AutofacWcfPortal can't be internal because it needs to be in the Web.Config of the IIS WCF Application
            this._dataPortalActivator = scope.Resolve<IDataPortalActivatorServer>();
            this._dataPortalActivator.Container = scope; // This property is in Localcontext so it needs to be set percall
        }

        private WcfPortal portal = new WcfPortal();

        /// <summary>
        /// Create a new business object.
        /// </summary>
        /// <param name="request">The request parameter object.</param>
        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public async Task<WcfResponse> Create(CreateRequest request)
        {
            Csla.Server.DataPortal portal = new Csla.Server.DataPortal();

            object result;

            try
            {
                result = await portal.Create(request.ObjectType, request.Criteria, request.Context, true);
            }
            catch (Exception ex)
            {
                result = ex;
            }

            return new WcfResponse(result);
        }

        /// <summary>
        /// Get an existing business object.
        /// </summary>
        /// <param name="request">The request parameter object.</param>
        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public async Task<WcfResponse> Fetch(FetchRequest request)
        {
            Csla.Server.DataPortal portal = new Csla.Server.DataPortal();

            object result;

            try
            {
                result = await portal.Fetch(request.ObjectType, request.Criteria, request.Context, true);
            }
            catch (Exception ex)
            {
                result = ex;
            }

            return new WcfResponse(result);

        }

        /// <summary>
        /// Update a business object.
        /// </summary>
        /// <param name="request">The request parameter object.</param>
        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public async Task<WcfResponse> Update(UpdateRequest request)
        {
            Csla.Server.DataPortal portal = new Csla.Server.DataPortal();

            object result;

            try
            {
                result = await portal.Update(request.Object, request.Context, true);
            }
            catch (Exception ex)
            {
                result = ex;
            }

            return new WcfResponse(result);

        }

        /// <summary>
        /// Delete a business object.
        /// </summary>
        /// <param name="request">The request parameter object.</param>
        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public async Task<WcfResponse> Delete(DeleteRequest request)
        {
            Csla.Server.DataPortal portal = new Csla.Server.DataPortal();

            object result;

            try
            {
                result = await portal.Delete(request.ObjectType, request.Criteria, request.Context, true);
            }
            catch (Exception ex)
            {
                result = ex;
            }

            return new WcfResponse(result);

        }
    }
}
