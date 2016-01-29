using Csla;
using Csla.Core;
using Csla.Serialization.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CslaAsyncAwait.Lib.Server
{
    /// <summary>
    /// mockable bridge interface for the CSLA DataPortal - adds additional methods not included with IDataPortal&lt;T&gt; for accessing Child DataPortal Methods
    /// </summary>
    /// <typeparam name="T">IBusinessBase implementation to pass along to the real CSLA dataportal (or mock thereof)</typeparam>
    public interface IObjectPortal<T>
        : IDataPortal<T>
        where T : class, IMobileObject
    {
        // These are missing from IDataPortal
        T CreateChild();
        T CreateChild(params object[] parameters);
        T FetchChild();
        T FetchChild(params object[] parameters);
        void UpdateChild(T childObj, params object[] parameters);
        void UpdateChild(T childObj);
    }
}
