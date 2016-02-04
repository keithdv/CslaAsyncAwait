using Autofac;
using CslaAsyncAwait.Lib;
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
    public class EntityFrameworkError
    {

        [TestMethod]
        public void CreateEntityFrameworkError()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterModule<CslaAsyncAwait.Lib.Server.AutofacModule>();

            var container = builder.Build();

            var scope = container.BeginLifetimeScope();

            // THis describes the error and the 'workaround'
            // though it's pretty weak
            // https://msdn.microsoft.com/en-us/library/dn458353(v=vs.110).aspx
            // System.Configuration.ConfigurationManager.GetSection("system.xml/xmlReader"); // Workaround - Fails if this is commented out

            CallContext.LogicalSetData("CslaAsyncAwait.UnitTest", scope);

            AddXmlSchemaToSet(new XmlSchemaSet());

            // If you don't set this to null you get an error 
            // from MSTEST and the test is not marked completed
            CallContext.LogicalSetData("CslaAsyncAwait.UnitTest", null);

        }

        // This is the code that is erroring in Entity Framework Core.Schema object
        private static void AddXmlSchemaToSet(XmlSchemaSet schemaSet)
        {
            // loop through the children to do a depth first load


            var xsdStream = GetResourceStream("System.Data.Resources.ProviderServices.ProviderManifest.xsd");
            var schema = XmlSchema.Read(xsdStream, null);
            schemaSet.Add(schema);
            //schemasAlreadyAdded.Add(schemaResource.NamespaceUri);

        }

        private static Stream GetResourceStream(string resourceName)
        {

            var resourceStream = typeof(System.Data.Entity.Core.EntityKey).Assembly.GetManifestResourceStream(resourceName);


            return resourceStream;
        }

        private ILifetimeScope scope;

        [TestMethod]
        public async Task CreateEntityFrameworkError_Fix()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterModule<CslaAsyncAwait.Lib.Server.AutofacModule>();

            var container = builder.Build();

            scope = container.BeginLifetimeScope();

            // THis describes the error and the 'workaround'
            // though it's pretty weak
            // https://msdn.microsoft.com/en-us/library/dn458353(v=vs.110).aspx
            // System.Configuration.ConfigurationManager.GetSection("system.xml/xmlReader"); // Workaround - Fails if this is commented out

            CallContext.LogicalSetData("CslaAsyncAwait.UnitTest", new CallContextItem(scope));

            AddXmlSchemaToSet(new XmlSchemaSet());

            await CheckCallContext();

        }

        public async Task CheckCallContext()
        {
            var result = (CallContextItem) CallContext.LogicalGetData("CslaAsyncAwait.UnitTest");

            Assert.AreSame(scope, result.Item);

            await Task.Delay(1);
        }

    }
}
