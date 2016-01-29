using Csla.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CslaAsyncAwait.Lib.Server
{
    public interface IContextAdapter
    {
        ContextDictionary LocalContext { get; }
        ContextDictionary GlobalContext { get; }
    }

    public class ContextAdapter : IContextAdapter
    {
        public ContextDictionary GlobalContext
        {
            get
            {
                return Csla.ApplicationContext.LocalContext;
            }
        }

        public ContextDictionary LocalContext
        {
            get
            {
                return Csla.ApplicationContext.GlobalContext;
            }
        }
    }
}
