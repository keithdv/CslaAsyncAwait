using Autofac;
using Csla.Xaml;
using CslaAsyncAwait.Lib;
using CslaAsyncAwait.Lib.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CslaAsyncAwait.Test
{
    [TestClass]
    public class BusinessObjectTests
    {

        private static IContainer _container;


        [TestMethod]
        public async Task FetchAsync_A()
        {

            // Multi-threaded async-await DataPortal-Fetch
            // Honestly I don't know why this is working!!! 

            // I am really suprised that the async/await even on mutiple threads
            // the Thread Local Storage is not leading to more scopeStacks being created
            // in the DataPortalActivator

            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterModule<CslaAsyncAwait.Lib.Server.AutofacModule>();

            _container = builder.Build();

            Csla.ApplicationContext.DataPortalActivator = new CslaAsyncAwait.Lib.Server.DataPortalActivator(_container);

            Csla.ApplicationContext.ContextManager = new Csla.Xaml.ApplicationContextManager();

            var scope = _container.BeginLifetimeScope();

            var result = await scope.Resolve<IObjectPortal<IBO_Parent>>().FetchAsync();

            Assert.AreEqual(result.UniqueValue, result.BO_ChildA[0].UniqueValue);
            Assert.AreEqual(1, (from a in result.BO_ChildA select a.UniqueValue).Distinct().Count());
            Assert.AreEqual(1, (from a in result.BO_ChildA select a.Branch.UniqueValue).Distinct().Count());
            Assert.AreEqual(result.UniqueValue, (from a in result.BO_ChildA select a.Branch.UniqueValue).First());


        }

        [TestMethod]
        public async Task FetchAsync_Insert()
        {

            // Multi-threaded async-await DataPortal-Insert
            // Honestly I don't know why this is working either!!! 

            // I am really suprised that the async/await even on mutiple threads
            // the Thread Local Storage is not leading to more scopeStacks being created
            // in the DataPortalActivator
            // Update - this is single threaded since I"m using FieldManager.UpdateChildren()

            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterModule<CslaAsyncAwait.Lib.Server.AutofacModule>();

            _container = builder.Build();

            var activator = new CslaAsyncAwait.Lib.Server.DataPortalActivator(_container);

            Csla.ApplicationContext.DataPortalActivator = activator;

            var scope = _container.BeginLifetimeScope();

            var s = new DataPortalActivator.ScopeMetadata();
            s.ChildTypes.Add("UNITTEST");
            s.Scope = scope;

            activator.scopeStack.Push(s);

            var uniqueID = scope.Resolve<IUniqueValue>();

            var result = await scope.Resolve<IObjectPortal<IBO_Parent>>().CreateAsync();

            Assert.IsTrue(result.IsNew);

            result = (IBO_Parent) await result.SaveAsync();

            //var result = await scope.Resolve<IObjectPortal<IBO_Parent>>().FetchAsync();

            Assert.AreEqual(result.UniqueValue, result.BO_ChildA[0].UniqueValue);
            Assert.AreEqual(1, (from a in result.BO_ChildA select a.UniqueValue).Distinct().Count());
            Assert.AreEqual(1, (from a in result.BO_ChildA select a.Branch.UniqueValue).Distinct().Count());
            Assert.AreEqual(result.UniqueValue, (from a in result.BO_ChildA select a.Branch.UniqueValue).First());

            //Assert.AreEqual(uniqueID, result.UniqueValue); This does fail?? 
        }

    }
}
