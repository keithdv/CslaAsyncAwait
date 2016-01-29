using CslaAsyncAwait.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
