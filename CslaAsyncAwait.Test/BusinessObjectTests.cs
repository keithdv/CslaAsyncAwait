using Autofac;
using CslaAsyncAwait.Lib;
using CslaAsyncAwait.Lib.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace CslaAsyncAwait.Test
{
    [TestClass]
    public class BusinessObjectTests
    {

        private static IContainer container;
        private static DataPortalActivator activator;

        [ClassInitialize]
        public static void ClassInitialize(TestContext tc)
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterModule<CslaAsyncAwait.Lib.Server.AutofacModule>();

            container = builder.Build();

            activator = new CslaAsyncAwait.Lib.Server.DataPortalActivator(container);

            Csla.ApplicationContext.DataPortalActivator = activator;

            // DEFAULT - Static everything

            // WPF
            //Csla.ApplicationContext.ContextManager = new Csla.Xaml.ApplicationContextManager();


            // CallContext
            Csla.ApplicationContext.ContextManager = new CallContextContextManager();

            
        }

        [TestMethod]
        public void Fetch_A()
        {

            // Multi-threaded async-await DataPortal-Fetch
            // Honestly I don't know why this is working!!! 

            // I am really suprised that the async/await even on mutiple threads
            // the Thread Local Storage is not leading to more scopeStacks being created
            // in the DataPortalActivator

            var scope = container.BeginLifetimeScope();

            CallContext.LogicalSetData("CslaAsyncAwait.UnitTest", scope);

            var result = scope.Resolve<IObjectPortal<IBO_Parent>>().Fetch();

            Assert.AreEqual(result.UniqueValue, result.BO_ChildA[0].UniqueValue);
            Assert.AreEqual(1, (from a in result.BO_ChildA select a.UniqueValue).Distinct().Count());
            Assert.AreEqual(1, (from a in result.BO_ChildA select a.Branch.UniqueValue).Distinct().Count());
            Assert.AreEqual(result.UniqueValue, (from a in result.BO_ChildA select a.Branch.UniqueValue).First());


        }

        [TestMethod]
        public void Fetch_B()
        {

            // Multi-threaded async-await DataPortal-Fetch
            // Honestly I don't know why this is working!!! 

            // I am really suprised that the async/await even on mutiple threads
            // the Thread Local Storage is not leading to more scopeStacks being created
            // in the DataPortalActivator

            var scope = container.BeginLifetimeScope();

            var result = scope.Resolve<IObjectPortal<IBO_Parent>>().Fetch();

            Assert.AreEqual(result.UniqueValue, result.BO_ChildA[0].UniqueValue);
            Assert.AreEqual(1, (from a in result.BO_ChildA select a.UniqueValue).Distinct().Count());
            Assert.AreEqual(1, (from a in result.BO_ChildA select a.Branch.UniqueValue).Distinct().Count());
            Assert.AreEqual(result.UniqueValue, (from a in result.BO_ChildA select a.Branch.UniqueValue).First());


        }

        [TestMethod]
        public void Fetch_C()
        {

            // Multi-threaded async-await DataPortal-Fetch
            // Honestly I don't know why this is working!!! 

            // I am really suprised that the async/await even on mutiple threads
            // the Thread Local Storage is not leading to more scopeStacks being created
            // in the DataPortalActivator

            var scope = container.BeginLifetimeScope();

            var result = scope.Resolve<IObjectPortal<IBO_Parent>>().Fetch();

            Assert.AreEqual(result.UniqueValue, result.BO_ChildA[0].UniqueValue);
            Assert.AreEqual(1, (from a in result.BO_ChildA select a.UniqueValue).Distinct().Count());
            Assert.AreEqual(1, (from a in result.BO_ChildA select a.Branch.UniqueValue).Distinct().Count());
            Assert.AreEqual(result.UniqueValue, (from a in result.BO_ChildA select a.Branch.UniqueValue).First());


        }

        [TestMethod]
        public void FetchAsync_Insert()
        {

            // Multi-threaded async-await DataPortal-Insert
            // Honestly I don't know why this is working either!!! 

            // I am really suprised that the async/await even on mutiple threads
            // the Thread Local Storage is not leading to more scopeStacks being created
            // in the DataPortalActivator
            // Update - this is single threaded since I"m using FieldManager.UpdateChildren()

            var scope = container.BeginLifetimeScope();

            var s = new DataPortalActivator.ScopeMetadata();
            s.ChildTypes.Add("UNITTEST");
            s.Scope = scope;

            activator.scopeStack.Push(s);

            var uniqueID = scope.Resolve<IUniqueValue>();

            var result = scope.Resolve<IObjectPortal<IBO_Parent>>().Create();

            Assert.IsTrue(result.IsNew);

            result = (IBO_Parent)result.Save();

            //var result = await scope.Resolve<IObjectPortal<IBO_Parent>>().FetchAsync();

            Assert.AreEqual(result.UniqueValue, result.BO_ChildA[0].UniqueValue);
            Assert.AreEqual(1, (from a in result.BO_ChildA select a.UniqueValue).Distinct().Count());
            Assert.AreEqual(1, (from a in result.BO_ChildA select a.Branch.UniqueValue).Distinct().Count());
            Assert.AreEqual(result.UniqueValue, (from a in result.BO_ChildA select a.Branch.UniqueValue).First());

            Assert.AreEqual(uniqueID, result.UniqueValue); //This does fail?? 
        }


    }
}
