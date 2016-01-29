
using Csla;
using CslaAsyncAwait.Lib.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CslaAsyncAwait.Lib
{

    public interface IUniqueValue { Guid UniqueValue { get; } }
    public class UniqueValueClass : IUniqueValue
    {
        public UniqueValueClass()
        {
            this.UniqueValue = Guid.NewGuid();
        }
        public Guid UniqueValue { get; private set; }
    }

    public interface IBO_Parent : Csla.IBusinessBase
    {
        Guid UniqueValue { get; }
        IBO_ChildA BO_ChildA { get; }
        IBO_ChildA BO_ChildA_1 { get; }
    }

    [Serializable]
    public class BO_Parent : BusinessBase<BO_Parent>, IBO_Parent, IDisposable
    {
        public static readonly PropertyInfo<IBO_ChildA> BO_ChildAProperty = RegisterProperty<IBO_ChildA>(c => c.BO_ChildA);
        public IBO_ChildA BO_ChildA
        {
            get { return GetProperty(BO_ChildAProperty); }
            set { SetProperty(BO_ChildAProperty, value); }
        }

        public static readonly PropertyInfo<IBO_ChildA> BO_ChildA_1Property = RegisterProperty<IBO_ChildA>(c => c.BO_ChildA_1);
        public IBO_ChildA BO_ChildA_1
        {
            get { return GetProperty(BO_ChildA_1Property); }
            set { SetProperty(BO_ChildA_1Property, value); }
        }

        private Guid uniqueValue;

        public Guid UniqueValue
        {
            get { return uniqueValue; }
            private set { uniqueValue = value; }
        }

        public bool IsDisposed = false;

        [NonSerialized]
        private IUniqueValue _servDep = null;
        [Dependency()]
        private IUniqueValue ServerDependency
        {
            get { return _servDep; }
            set { _servDep = value; }
        }

        [NonSerialized]
        private IObjectPortal<IBO_ChildA> portal;
        [Dependency()]
        public IObjectPortal<IBO_ChildA> Portal
        {
            get { return portal; }
            set { portal = value; }
        }


        protected void DataPortal_Create()
        {
            BO_ChildA = Portal.CreateChild();
            BO_ChildA_1 = Portal.CreateChild();
        }

        protected async Task DataPortal_Fetch()
        {
            using (BypassPropertyChecks)
            {
                this.UniqueValue = ServerDependency.UniqueValue;
            }

            //this.BO_ChildA = await Portal.FetchAsync();
            //this.BO_ChildB = await PortalB.FetchAsync();
            //this.BO_ChildB_1 = await PortalB.FetchAsync();

            List<Task> tasks = new List<Task>();

            tasks.Add(Portal.FetchAsync());
            tasks.Add(Portal.FetchAsync());

            await Task.WhenAll(tasks);

            this.BO_ChildA = ((Task<IBO_ChildA>)tasks[0]).Result;
            this.BO_ChildA_1 = ((Task<IBO_ChildA>)tasks[1]).Result;

        }

        protected void DataPortal_Insert()
        {
            using (BypassPropertyChecks)
            {
                this.UniqueValue = ServerDependency.UniqueValue;
            }

            FieldManager.UpdateChildren();

        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
    public interface IBO_ChildA : IBusinessListBase<IBO_GrandChildA> { Guid UniqueValue { get; } }

    [Serializable]
    public class BO_ChildA : BusinessListBase<BO_ChildA, IBO_GrandChildA>, IBO_ChildA
    {

        public BO_ChildA() { MarkAsChild(); }

        private Guid uniqueValue;
        public Guid UniqueValue
        {
            get { return uniqueValue; }
            private set { uniqueValue = value; }
        }

        [NonSerialized]
        private IUniqueValue _servDep = null;
        [Dependency()]
        private IUniqueValue ServerDependency
        {
            get { return _servDep; }
            set { _servDep = value; }
        }


        [Csla.NotUndoable]
        [NonSerialized]
        private IObjectPortal<IBO_GrandChildA> _portal;

        [Dependency]
        internal IObjectPortal<IBO_GrandChildA> portal
        {
            private get { return _portal; }
            set { _portal = value; }
        }

        protected void Child_Create()
        {
            Add(portal.CreateChild());
            Add(portal.CreateChild());
        }

        private async Task DataPortal_Fetch()
        {
            await Task.Delay(500);

            this.UniqueValue = ServerDependency.UniqueValue;

            var tasks = new List<Task>();

            tasks.Add(portal.FetchAsync());
            tasks.Add(portal.FetchAsync());

            await Task.WhenAll(tasks);

            Add(((Task<IBO_GrandChildA>)tasks[0]).Result);
            Add(((Task<IBO_GrandChildA>)tasks[1]).Result);
        }

    }

    public interface IBO_GrandChildA : IBusinessBase { Guid UniqueValue { get; } IBO_GrandChildA_Branch Branch { get; } }

    [Serializable]
    public class BO_GrandChildA : BusinessBase<BO_GrandChildA>, IBO_GrandChildA
    {

        public BO_GrandChildA() { MarkAsChild(); }

        private Guid uniqueValue;

        public Guid UniqueValue
        {
            get { return uniqueValue; }
            private set { uniqueValue = value; }
        }

        public static readonly PropertyInfo<IBO_GrandChildA_Branch> BranchProperty = RegisterProperty<IBO_GrandChildA_Branch>(c => c.Branch);
        public IBO_GrandChildA_Branch Branch
        {
            get { return GetProperty(BranchProperty); }
            set { SetProperty(BranchProperty, value); }
        }

        [NonSerialized]
        private IUniqueValue _servDep = null;
        [Dependency()]
        private IUniqueValue ServerDependency
        {
            get { return _servDep; }
            set { _servDep = value; }
        }


        [Csla.NotUndoable]
        [NonSerialized]
        private IObjectPortal<IBO_GrandChildA_Branch> _portal;

        [Dependency]
        internal IObjectPortal<IBO_GrandChildA_Branch> portal
        {
            private get { return _portal; }
            set { _portal = value; }
        }

        private void Child_Create()
        {
            using (BypassPropertyChecks)
            {
                this.UniqueValue = ServerDependency.UniqueValue;
                this.Branch = portal.CreateChild(); ;
            }
        }

        private async Task DataPortal_Fetch()
        {
            using (BypassPropertyChecks)
            {
                this.UniqueValue = ServerDependency.UniqueValue;
                this.Branch = await portal.FetchAsync(); ;
            }
        }

        protected void Child_Insert()
        {
            using (BypassPropertyChecks)
            {
                this.UniqueValue = ServerDependency.UniqueValue;
            }

            FieldManager.UpdateChildren();

        }

    }

    public interface IBO_GrandChildA_Branch : IBusinessBase { Guid UniqueValue { get; } }

    [Serializable]
    public class BO_GrandChildA_Branch : BusinessBase<BO_GrandChildA_Branch>, IBO_GrandChildA_Branch
    {
        public BO_GrandChildA_Branch() { MarkAsChild(); }

        private Guid uniqueValue;

        public Guid UniqueValue
        {
            get { return uniqueValue; }
            private set { uniqueValue = value; }
        }

        [NonSerialized]
        private IUniqueValue _servDep = null;
        [Dependency()]
        private IUniqueValue ServerDependency
        {
            get { return _servDep; }
            set { _servDep = value; }
        }

        private async Task DataPortal_Fetch()
        {
            using (BypassPropertyChecks)
            {
                this.UniqueValue = await Task.FromResult(ServerDependency.UniqueValue);
            }
        }

        private void Child_Insert()
        {
            using (BypassPropertyChecks)
            {
                this.UniqueValue = ServerDependency.UniqueValue;
            }

        }
    }

}
