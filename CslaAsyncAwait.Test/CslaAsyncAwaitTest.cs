using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla;
using CslaAsyncAwait.Lib;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CslaAsyncAwait.Test
{
    [TestClass]
    public class CslaAsyncAwaitTest
    {

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext c)
        {
            Csla.ApplicationContext.ContextManager = new Csla.Test.AppContext.TestContext();
        }


        [TestMethod]
        public void Fetch()
        {
            var princ = Principal.IdealFetch();
            Assert.IsInstanceOfType(princ, typeof(Principal));


        }

        [TestMethod]
        public void FetchSetContext()
        {
            Principal.IdealFetchSetContext();
            Assert.IsInstanceOfType(Csla.ApplicationContext.User, typeof(Principal));

        }


        [TestMethod]
        public async Task FetchAsync()
        {
            var princ = await Principal.IdealFetchAsync();
            Assert.IsInstanceOfType(princ, typeof(Principal));

        }

        [TestMethod]
        public async Task FetchSetContextAsync()
        {
            await Principal.IdealFetchSetContextAsync();
            // In WPF we would still have the correct CslaAsyncAwait.Lib.Principal in Csla.ApplicationContext.User
            Assert.IsInstanceOfType(Csla.ApplicationContext.User, typeof(Principal));

        }

        public async Task IdealFetchSetContextAsync()
        {
            await Principal.IdealFetchSetContextAsync();
            // In WPF we would still have the correct CslaAsyncAwait.Lib.Principal in Csla.ApplicationContext.User
            Assert.IsInstanceOfType(Csla.ApplicationContext.User, typeof(Principal));

        }

        [TestMethod]
        public async Task FetchSetContextAsync_Nito()
        {
            await Nito.AsyncEx.AsyncContext.Run<Task>((Func<Task>)IdealFetchSetContextAsync);
        }

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

        [TestMethod]
        public void FetchSetContextAsync_AsyncPump_RunTask()
        {
            AsyncPump.Run((Func<Task>)IdealFetchSetContextAsync);
        }

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
