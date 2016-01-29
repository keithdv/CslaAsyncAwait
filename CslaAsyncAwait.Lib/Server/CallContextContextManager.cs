using Csla.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CslaAsyncAwait.Lib.Server
{
    public class CallContextContextManager : IContextManager
    {
        private static Dictionary<Guid, HybridDictionary> _myContext = new Dictionary<Guid, HybridDictionary>();

        // This doesn't work
        // If I do this then every async call uses the same ContextDictionary
        //public CallContextContextManager()
        //{
        //    var id = UniqueIdentifier; // Create a seed id
        //}

        private const string UniqueIdentifierKey = "Csla.TestContext.UniqueIdentifierKey";
        public static Guid UniqueIdentifier
        {
            get
            {
                Guid? id = (Guid?)CallContext.LogicalGetData(UniqueIdentifierKey);

                if (!id.HasValue)
                {
                    Debug.WriteLine("************ NewID Created");
                    id = Guid.NewGuid();
                    CallContext.LogicalSetData(UniqueIdentifierKey, id);
                    _myContext[id.Value] = new HybridDictionary();
                }

                return id.Value;
            }
        }

        public static void ClearCallContext()
        {
            // I don't think you would do this in a production senario.
            // Trying to get it working!

            CallContext.LogicalSetData(UniqueIdentifierKey, null);

        }

        private const string _localContextName = "Csla.ClientContext";
        private const string _clientContextName = "Csla.ClientContext";
        private const string _globalContextName = "Csla.GlobalContext";
        private const string _principalContextName = "Csla.PrincipalContext";

        public bool IsValid
        {
            get { return true; }
        }

        private static IPrincipal _principal;

        public IPrincipal GetUser()
        {
            return _principal;
        }

        public void SetUser(IPrincipal principal)
        {
            //Thread.CurrentPrincipal = principal;
            _principal = principal;
        }

        public ContextDictionary GetLocalContext()
        {
            if (_myContext[UniqueIdentifier][_localContextName] == null)
                SetLocalContext(new ContextDictionary());
            return (ContextDictionary)_myContext[UniqueIdentifier][_localContextName];
        }

        public void SetLocalContext(ContextDictionary localContext)
        {
            _myContext[UniqueIdentifier][_localContextName] = localContext;
        }

        public ContextDictionary GetClientContext()
        {
            if (_myContext[UniqueIdentifier][_clientContextName] == null)
                SetClientContext(new ContextDictionary());
            return (ContextDictionary)_myContext[UniqueIdentifier][_clientContextName];
        }

        public void SetClientContext(ContextDictionary clientContext)
        {
            _myContext[UniqueIdentifier][_clientContextName] = clientContext;
        }

        public ContextDictionary GetGlobalContext()
        {
            if (_myContext[UniqueIdentifier][_globalContextName] == null)
                SetGlobalContext(new ContextDictionary());
            return (ContextDictionary)_myContext[UniqueIdentifier][_globalContextName];
        }

        public void SetGlobalContext(ContextDictionary globalContext)
        {
            _myContext[UniqueIdentifier][_globalContextName] = globalContext;
        }

        public System.Collections.IDictionary GlobalApplicationContext
        {
            get { return GetGlobalContext(); }
        }

        public System.Collections.IDictionary LocalApplicationContext
        {
            get { return GetLocalContext(); }
        }

        public IPrincipal User
        {
            get
            {
                return GetUser();
            }
            set
            {
                SetUser(value);
            }
        }
    }
}
