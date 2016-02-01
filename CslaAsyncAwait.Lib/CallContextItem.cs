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
    public class CallContextItem<T>
    {
        [NonSerialized]
        private static Dictionary<Guid, T> itemStore = new Dictionary<Guid, T>();

        public CallContextItem(T item)
        {
            itemStore.Add(UniqueIdentifier, item);
        }

        ~CallContextItem()
        {
            if (itemStore.ContainsKey(UniqueIdentifier))
            {
                var dispose = itemStore[UniqueIdentifier] as IDisposable;

                if(dispose != null)
                {
                    dispose.Dispose();
                }

                itemStore.Remove(UniqueIdentifier);
            }
        }

        private Guid _uniqueIdentifier = Guid.NewGuid();

        public Guid UniqueIdentifier
        {
            get { return _uniqueIdentifier; }
            private set { _uniqueIdentifier = value; }
        }

    }
}
