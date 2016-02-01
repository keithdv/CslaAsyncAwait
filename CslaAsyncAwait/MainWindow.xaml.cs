using CslaAsyncAwait.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autofac;
using System.Xml.Schema;
using System.IO;

namespace CslaAsyncAwait
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var princ = Principal.IdealFetch();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Principal.IdealFetchSetContext();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var princ = await Principal.IdealFetchAsync();
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            await Principal.IdealFetchSetContextAsync();

            // This works here but I cannot get this
            // behavior in the unit tests
            if (Csla.ApplicationContext.User.GetType() != typeof(Principal))
            {
                throw new Exception("Wrong type");
            }
        }


        private void SyncAsyncFetch_Click(object sender, RoutedEventArgs e)
        {
            var princ = Principal.SyncAsyncFetch();
        }

        private async void AsyncSyncFetch_Click(object sender, RoutedEventArgs e)
        {
            var princ = await Principal.AsyncSyncFetch();
        }

        private void SyncAsyncFetchSetContext_Click(object sender, RoutedEventArgs e)
        {
            Principal.SyncAsyncFetchSetContext();
        }

        private async void AsyncSyncFetchSetContext_Click(object sender, RoutedEventArgs e)
        {
            await Principal.AsyncSyncFetchSetContext();
        }

        private Guid key = Guid.NewGuid();

        private Guid key1 = Guid.NewGuid();
        private Guid key2 = Guid.NewGuid();

        private async void CallContext_Nested_Async_Calls_Click(object sender, RoutedEventArgs e)
        {
            var asyncList = new List<string>();

            asyncList.Add("CallContext Click");

            CallContext.LogicalSetData(key.ToString(), asyncList);

            await Nested1();

            asyncList = (List<string>)CallContext.LogicalGetData(key.ToString());


            object key1value = (object)CallContext.LogicalGetData(key1.ToString());
            object key2value = (object)CallContext.LogicalGetData(key2.ToString());

        }

        private async Task Nested1()
        {
            var asyncList = (List<string>)CallContext.LogicalGetData(key.ToString());

            asyncList.Add("Nested1");

            var container = Builder().Build();
            var scope = container.BeginLifetimeScope();

            WeakEventManager<ILifetimeScope, Autofac.Core.Lifetime.LifetimeScopeEndingEventArgs>.AddHandler(scope, "CurrentScopeEnding", Scope_CurrentScopeEnding);

            //CallContext.LogicalSetData(key1.ToString(), scope);
            CallContext.LogicalSetData(key1.ToString(), new CallContextItem<ILifetimeScope>(scope));

            await Nested2();


        }

        private void Scope_CurrentScopeEnding(object sender, Autofac.Core.Lifetime.LifetimeScopeEndingEventArgs e)
        {

        }

        private async Task Nested2()
        {
            var asyncList = (List<string>)CallContext.LogicalGetData(key.ToString());

            asyncList.Add("Nested4");

            CallContext.LogicalSetData(key2.ToString(), new CallContextItem<object>(new object()));

            await Nested3();
        }

        private async Task Nested3()
        {
            var asyncList = (List<string>)CallContext.LogicalGetData(key.ToString());

            asyncList.Add("Nested5");

            object key1value = (object)CallContext.LogicalGetData(key1.ToString());
            object key2value = (object)CallContext.LogicalGetData(key2.ToString());


            await Task.Delay(10);

        }

        private void GCCollect_Click(object sender, RoutedEventArgs e)
        {
            GC.Collect();
        }

        private ContainerBuilder Builder()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterModule<CslaAsyncAwait.Lib.Server.AutofacModule>();

            return builder;
        }

        
        public void CreateEntityFrameworkError()
        {

            var scope = Builder().Build().BeginLifetimeScope();

            // This errors in MSTest but doesn't seem to error in WPF

            // THis describes the error and the 'workaround' though it's pretty weak
            // https://msdn.microsoft.com/en-us/library/dn458353(v=vs.110).aspx
            //System.Configuration.ConfigurationManager.GetSection("system.xml/xmlReader"); // Workaround - Fails if this is commented out

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

        private void EntityFrameworkError_Click(object sender, RoutedEventArgs e)
        {
            CreateEntityFrameworkError();
        }
    }
}
