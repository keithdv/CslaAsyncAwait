using Csla;
using Csla.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CslaAsyncAwait.Lib
{
    [Serializable]
    public class Principal : CslaPrincipal, ICslaPrincipal
    {

        private Principal(ICslaIdentity identity)
            : base(identity)
        {

        }

        public static Principal IdealFetch()
        {
            return new Principal(DataPortal.Fetch<Identity>());
        }

        public static void IdealFetchSetContext()
        {
            Csla.ApplicationContext.User = new Principal(DataPortal.Fetch<Identity>());
        }

        public static async Task<Principal> IdealFetchAsync()
        {
            return new Principal(await DataPortal.FetchAsync<IdentityAsync>());
        }

        public static async Task IdealFetchSetContextAsync()
        {
            Csla.ApplicationContext.User = new Principal(await DataPortal.FetchAsync<IdentityAsync>());
        }

        public static Principal SyncAsyncFetch()
        {
            return new Principal(DataPortal.Fetch<IdentityAsync>());
        }

        public static void SyncAsyncFetchSetContext()
        {
            Csla.ApplicationContext.User = new Principal(DataPortal.Fetch<IdentityAsync>());
        }

        public static async Task<Principal> AsyncSyncFetch()
        {
            return new Principal(await DataPortal.FetchAsync<Identity>());
        }

        public static async Task AsyncSyncFetchSetContext()
        {
            Csla.ApplicationContext.User = new Principal(await DataPortal.FetchAsync<Identity>());
        }

    }
}
