using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Csla.Serialization.Mobile;
using Csla.Reflection;

namespace CslaAsyncAwait.Lib.Server
{
    public sealed class DataPortalActivator
        : IDataPortalActivator, IDataPortalActivatorServer
    {

        private const string LocalContextKey = "DataPortalActivator.scopeKeys";
        private const string DataPortalActivatorKey = "DataPortalActivator.ThreadScopeKey";


        public class ScopeMetadata
        {
            public ScopeMetadata()
            {
                ChildTypes = new List<string>();
            }

            public ILifetimeScope Scope { get; set; }
            public IList<string> ChildTypes { get; set; }

        }

        private IContextAdapter _contextAdapter;

        /// <summary>
        /// handle to a passed in IoC container
        /// </summary>
        private IContainer _iocContainer = null;


        private ILifetimeScope Container
        {
            get
            {
                return _iocContainer ?? (ILifetimeScope)_contextAdapter.LocalContext[DataPortalActivatorKey];
            }
            set
            {
                // Does not support both the IContainer constructor
                // and setting the property which stores the container in thread context
                // Use one or the other - usually client and server (AutofacWcfPortal)
                _iocContainer = null;
                _contextAdapter.LocalContext[DataPortalActivatorKey] = value;
            }
        }

        ILifetimeScope IDataPortalActivatorServer.Container
        {
            get
            {
                return Container;
            }
            set
            {
                Container = value;
            }
        }

        public Stack<ScopeMetadata> scopeStack
        {

            get
            {
                // Store in LocalContext
                // This is required so this will work 2 and 3 physical tiers
                // on the server it is important to be associated with the thread
                if (!_contextAdapter.LocalContext.Contains(LocalContextKey))
                {
                    _contextAdapter.LocalContext[LocalContextKey] = new Stack<ScopeMetadata>();
                }

                return (Stack<ScopeMetadata>)_contextAdapter.LocalContext[LocalContextKey];
            }
        }

        /// <summary>
        /// constructs a ObjectActivator assuming it is the server
        /// Since the container is not sent in with the constructor it needs to be set
        /// using the container property
        /// </summary>
        /// <param name="iocContainer">The <seealso cref="AutoFac.IContainer"/> to create lifetime scopes with for resolving business object types that implement IBusinessScope</param>
        internal DataPortalActivator()
        {
            _contextAdapter = new ContextAdapter();
            Container = null; // Clear the thread storage
        }

        /// <summary>
        /// constructs a ObjectActivator using the passed in iocContainer
        /// </summary>
        /// <param name="iocContainer">The <seealso cref="AutoFac.IContainer"/> to create lifetime scopes with for resolving business object types that implement IBusinessScope</param>
        public DataPortalActivator(IContainer iocContainer)
            : this()
        {
            _iocContainer = iocContainer;
        }

        /// <summary>
        /// This is meant to be the injected constructor
        /// </summary>
        /// <param name="iocContainer">The <seealso cref="AutoFac.IContainer"/> to create lifetime scopes with for resolving business object types that implement IBusinessScope</param>
        public DataPortalActivator(IContextAdapter contextAdapter)
        {
            this._contextAdapter = contextAdapter;
            Container = null; // Clear the thread storage
        }

        /// <summary>
        /// All possible values
        /// </summary>
        /// <param name="iocContainer">The <seealso cref="AutoFac.IContainer"/> to create lifetime scopes with for resolving business object types that implement IBusinessScope</param>
        public DataPortalActivator(IContainer iocContainer, IContextAdapter contextAdapter)
        {
            this._contextAdapter = contextAdapter;
            this._iocContainer = iocContainer;
        }


        public object CreateInstance(Type requestedType)
        {
            if (requestedType == null)
            {
                throw new ArgumentNullException("requestedType");
            }
            else
            {


                object retInst;

                IComponentRegistration registration = Container.ComponentRegistry.RegistrationsFor(new TypedService(requestedType)).FirstOrDefault();

                if ((requestedType.IsInterface || requestedType.IsAbstract) && (!Container.ComponentRegistry.IsRegistered(new TypedService(requestedType))))
                {
                    throw new Exception(requestedType.FullName);
                }
                else if (registration != null)
                {
                    IInstanceActivator activator = registration.Activator as IInstanceActivator;
                    requestedType = activator.LimitType;
                }

                // Before we created the object with the scope
                // But we are going to dispose the scope in FinalizeDependencies
                // So if the object is IDisposable it will be disposed!!
                // In all cases do the default behavior - MethodCaller.CreateInstance
                retInst = MethodCaller.CreateInstance(requestedType);

                return retInst;
            }
        }

        public void InitializeInstance(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            ScopeMetadata scope = null;

            if (scopeStack.Count == 0)
            {
                scope = new ScopeMetadata();
                scope.Scope = Container.BeginLifetimeScope();
                scope.ChildTypes.Add(obj.GetType().Name);
                scopeStack.Push(scope);
            }
            else
            {
                scope = scopeStack.Peek();
                scope.ChildTypes.Add(obj.GetType().Name);
            }


            ((IMobileObject)obj).InitializeDependencies(scope.Scope);

        }

        public void FinalizeInstance(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            if (obj is IMobileObject)
            {
                ((IMobileObject)obj).FinalizeDependencies();
            }

            var scope = scopeStack.Peek();

            scope.ChildTypes.Remove(obj.GetType().Name);

            if (scope.ChildTypes.Count == 0)
            {
                scopeStack.Pop();
                scope.Scope.Dispose();
            }

        }



    }

    [Serializable]
    public class LegacyBusinessScopeUsageException : Exception
    {
        public LegacyBusinessScopeUsageException() { }
        public LegacyBusinessScopeUsageException(string message) : base(message) { }
        public LegacyBusinessScopeUsageException(string message, Exception inner) : base(message, inner) { }
        protected LegacyBusinessScopeUsageException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        { }
    }

    [Serializable]
    public class ScopeKeyMissingChildException : Exception
    {
        public ScopeKeyMissingChildException() { }
        public ScopeKeyMissingChildException(string message) : base(message) { }
        public ScopeKeyMissingChildException(string message, Exception inner) : base(message, inner) { }
        protected ScopeKeyMissingChildException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        { }
    }

    [Serializable]
    public class UnexpectedContainerException : Exception
    {
        public UnexpectedContainerException() { }
        public UnexpectedContainerException(string message) : base(message) { }
        public UnexpectedContainerException(string message, Exception inner) : base(message, inner) { }
        protected UnexpectedContainerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        { }
    }
}
