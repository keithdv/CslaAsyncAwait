using Csla;
using Csla.Data;
using Csla.Security;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CslaAsyncAwait.Lib
{
    [Serializable]
    public class Identity : Csla.Security.CslaIdentityBase<Identity>
    {


        public static readonly PropertyInfo<int> EmployeeIDProperty = RegisterProperty<int>(c => c.EmployeeID);
        public int EmployeeID
        {
            get { return GetProperty(EmployeeIDProperty); }
            private set { LoadProperty(EmployeeIDProperty, value); }
        }


        private void DataPortal_Fetch()
        {

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null))
            {
                using (var stream = isoStore.OpenFile(string.Format("{0}.txt", Guid.NewGuid().ToString("N")), System.IO.FileMode.OpenOrCreate))
                {

                    stream.Write(new byte[] { 1 }, 0, 1);

                    stream.Close();
                }

                isoStore.Close();
            }

        }

    }

    [Serializable]
    public class IdentityAsync : Csla.Security.CslaIdentityBase<IdentityAsync>
    {


        public static readonly PropertyInfo<int> EmployeeIDProperty = RegisterProperty<int>(c => c.EmployeeID);
        public int EmployeeID
        {
            get { return GetProperty(EmployeeIDProperty); }
            private set { LoadProperty(EmployeeIDProperty, value); }
        }


        private async Task DataPortal_Fetch()
        {

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null))
            {
                using (var stream = isoStore.OpenFile(string.Format("{0}.txt", Guid.NewGuid().ToString("N")), System.IO.FileMode.OpenOrCreate))
                {

                    await stream.WriteAsync(new byte[] { 1 }, 0, 1);

                    stream.Close();
                }

                isoStore.Close();
            }

        }

    }
}
