using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace CslaAsyncAwait.Lib
{

    /// <summary>
    /// Wrap objects that get put into the CallContext to prevent Cross-Domain errors
    /// CallContext was designed to permit communication across appdomains
    /// when that happens some not func security stuff happens
    /// Not an issue in WPF or IIS but breaks MSTEST itself
    /// MarshalByRefObject seems to solve all of the issues
    /// </summary>
    public class CallContextItem : MarshalByRefObject
    {
        public CallContextItem(object item)
        {
            this._item = item;
        }

        private object _item;

        public object Item
        {
            get {

                return _item;
            }
        }

    }
}
