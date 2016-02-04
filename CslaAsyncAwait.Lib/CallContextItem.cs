using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace CslaAsyncAwait.Lib
{

    [Serializable]
    public class CallContextItem : MarshalByRefObject
    {
        [NonSerialized]
        private static Dictionary<Guid, WeakReference> itemStore = new Dictionary<Guid, WeakReference>();

        public CallContextItem(object item)
        {
            itemStore.Add(UniqueIdentifier, new WeakReference(item));
        }


        public object Item
        {
            get {

                return itemStore[UniqueIdentifier].Target;

            }
        }

        private readonly Guid _uniqueIdentifier = Guid.NewGuid();

        public Guid UniqueIdentifier
        {
            get { return _uniqueIdentifier; }
        }

    }
}
