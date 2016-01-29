using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla;
using CslaAsyncAwait.Lib;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CslaAsyncAwait.Lib.Server;

namespace CslaAsyncAwait.Test
{
    [TestClass]
    public class CslaAsyncAwaitTest
    {

        private const string key = "CslaAsyncAwaitTestKey";

        [ClassInitialize]
        public static void ClassInitialize(TestContext c)
        {

            ContextManager = Csla.ApplicationContext.ContextManager;

            // Senario 1 - Leave ContextManager the default
            // Loose Csla.ApplicationContext.User at the end of the await call
            // 3 [Async] Unit Test fail

            // Senario 2 - Put use into a static manager
            // Works but now all of the async calls shard a ContextDictionary
            // which is what we don't want (for things like storing a scope)
            // Fail do to the "Assert.IsNull(Csla.ApplicationContext.LocalContext[key]);" statements
            //Csla.ApplicationContext.ContextManager = new Csla.Test.AppContext.StaticContextManager();

            // Very close to what we want
            // CallContext is working how I would expect
            // The only problem is we need to "seed" UniqueIdenfier in the Async call we want the principal to be "alive" in

            // Update:  added ClearCallContext
            // The async tests fail if Fetch and/or FetchSetContext are ran before them
            // Which is kinda flaky
            // I'm guessing because it's seeding "UniqueIdentifier" first

            Csla.ApplicationContext.ContextManager = new CallContextContextManager();

        }

        private static Csla.Core.IContextManager ContextManager;

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Csla.ApplicationContext.ContextManager = ContextManager;
        }


        [TestMethod]
        public void Fetch()
        {
            var princ = Principal.IdealFetch();
            Assert.IsInstanceOfType(princ, typeof(Principal));


            // The async tests fail if Fetch and/or FetchSetContext are ran before them
            // Which is kinda flaky
            // I'm guessing because it's seeding "UniqueIdentifier" first

            CallContextContextManager.ClearCallContext();

        }

        [TestMethod]
        public void FetchSetContext()
        {
            Principal.IdealFetchSetContext();
            Assert.IsInstanceOfType(Csla.ApplicationContext.User, typeof(Principal));


            // The async tests fail if Fetch and/or FetchSetContext are ran before them
            // Which is kinda flaky
            // I'm guessing because it's seeding "UniqueIdentifier" first

            CallContextContextManager.ClearCallContext();

        }


        [TestMethod]
        [TestCategory("Async")]
        public async Task FetchAsync()
        {

            // This is required so that the CallContext.UniqueIdenfier is set at this top level
            // How do we get rid of this???
            Assert.IsNotNull(CallContextContextManager.UniqueIdentifier);

            var princ = await Principal.IdealFetchAsync();
            Assert.IsInstanceOfType(princ, typeof(Principal));

            // See if we are getting carry-over data from other unit tests into this unit test
            var x = Csla.ApplicationContext.LocalContext[key];
            Assert.IsNull(x, CallContextContextManager.UniqueIdentifier.ToString());
            Csla.ApplicationContext.LocalContext[key] = "FetchAsync";
        }



        [TestMethod]
        [TestCategory("Async")]
        public async Task FetchSetContextAsync()
        {

            // This is required so that the CallContext.UniqueIdenfier is set at this top level
            // How do we get rid of this???
            Assert.IsNotNull(CallContextContextManager.UniqueIdentifier);

            await Principal.IdealFetchSetContextAsync();
            // In WPF we would still have the correct CslaAsyncAwait.Lib.Principal in Csla.ApplicationContext.User
            Assert.IsInstanceOfType(Csla.ApplicationContext.User, typeof(Principal));

            // See if we are getting carry-over data from other unit tests into this unit test
            var x = Csla.ApplicationContext.LocalContext[key];
            Assert.IsNull(x, CallContextContextManager.UniqueIdentifier.ToString());
            Csla.ApplicationContext.LocalContext[key] = "FetchSetContextAsync";

        }

        public async Task IdealFetchSetContextAsync()
        {
            await Principal.IdealFetchSetContextAsync();
            // In WPF we would still have the correct CslaAsyncAwait.Lib.Principal in Csla.ApplicationContext.User
            Assert.IsInstanceOfType(Csla.ApplicationContext.User, typeof(Principal));

            // See if we are getting carry-over data from other unit tests into this unit test
            var x = Csla.ApplicationContext.LocalContext[key];
            Assert.IsNull(x, CallContextContextManager.UniqueIdentifier.ToString());
            Csla.ApplicationContext.LocalContext[key] = new object();

        }

        [TestMethod]
        [TestCategory("Async")]
        public async Task FetchSetContextAsync_1()
        {

            // This is required so that the CallContext.UniqueIdenfier is set at this top level
            // How do we get rid of this???
            Assert.IsNotNull(CallContextContextManager.UniqueIdentifier);

            await Principal.IdealFetchSetContextAsync();
            // In WPF we would still have the correct CslaAsyncAwait.Lib.Principal in Csla.ApplicationContext.User
            Assert.IsInstanceOfType(Csla.ApplicationContext.User, typeof(Principal));

            // See if we are getting carry-over data from other unit tests into this unit test
            var x = Csla.ApplicationContext.LocalContext[key];
            Assert.IsNull(x, CallContextContextManager.UniqueIdentifier.ToString());
            Csla.ApplicationContext.LocalContext[key] = "FetchSetContextAsync_1";

        }

        [TestMethod]
        [TestCategory("Async")]
        public async Task FetchSetContextAsync_2()
        {

            // This is required so that the CallContext.UniqueIdenfier is set at this top level
            // How do we get rid of this???
            Assert.IsNotNull(CallContextContextManager.UniqueIdentifier);

            await Principal.IdealFetchSetContextAsync();
            // In WPF we would still have the correct CslaAsyncAwait.Lib.Principal in Csla.ApplicationContext.User
            Assert.IsInstanceOfType(Csla.ApplicationContext.User, typeof(Principal));

            // See if we are getting carry-over data from other unit tests into this unit test
            var x = Csla.ApplicationContext.LocalContext[key];
            Assert.IsNull(x, CallContextContextManager.UniqueIdentifier.ToString());
            Csla.ApplicationContext.LocalContext[key] = "FetchSetContextAsync_2";

        }

        [Ignore]
        [TestMethod]
        [TestCategory("Async")]
        public async Task FetchSetContextAsync_Nito()
        {
            // This is required so that the CallContext.UniqueIdenfier is set at this top level
            // How do we get rid of this???
            Assert.IsNotNull(CallContextContextManager.UniqueIdentifier);

            await Nito.AsyncEx.AsyncContext.Run<Task>((Func<Task>)IdealFetchSetContextAsync);

            // See if we are getting carry-over data from other unit tests into this unit test
            var x = Csla.ApplicationContext.LocalContext[key];
            Assert.IsNull(x, (x ?? "").ToString());
            Csla.ApplicationContext.LocalContext[key] = new object();

        }

        [Ignore]
        [TestMethod]
        public void FetchSetContextAsync_Dispatcher()
        {
            // Even using the DispatcherSynchronizationContext
            // and referencing Csla.Xaml Csla.ApplicationContext.User
            // reverts back to WindowsPrincipal
            DispatcherRun((Func<Task>)IdealFetchSetContextAsync);

            // In WPF we would still have the correct CslaAsyncAwait.Lib.Principal in Csla.ApplicationContext.User
            Assert.IsInstanceOfType(Csla.ApplicationContext.User, typeof(Principal));

        }

        public static void DispatcherRun(Func<Task> func)
        {
            if (func == null) throw new ArgumentNullException("func");

            var prevCtx = SynchronizationContext.Current;
            try
            {
                var syncCtx = new DispatcherSynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(syncCtx);

                var t = func();
                if (t == null) throw new InvalidOperationException();

                var frame = new DispatcherFrame();

                t.ContinueWith(_ => { frame.Continue = false; },
                    TaskScheduler.Default);
                Dispatcher.PushFrame(frame);

                t.GetAwaiter().GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevCtx);
            }
        }

        [Ignore]
        [TestMethod]
        public void FetchSetContextAsync_AsyncPump_RunTask()
        {
            AsyncPump.Run((Func<Task>)IdealFetchSetContextAsync);
        }

        [Ignore]
        [TestMethod]
        public void FetchSetContextAsync_AsyncPump_RunVoid()
        {
            AsyncPump.Run(new Action(() =>
            {

                Principal.IdealFetchSetContextAsync();
                // In WPF we would still have the correct CslaAsyncAwait.Lib.Principal in Csla.ApplicationContext.User
                Assert.IsInstanceOfType(Csla.ApplicationContext.User, typeof(Principal));
            }));
        }


    }
}
