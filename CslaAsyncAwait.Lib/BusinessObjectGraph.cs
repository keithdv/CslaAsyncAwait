
using Csla;
using CslaAsyncAwait.Lib.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CslaAsyncAwait.Lib
{

    public interface IUniqueValue : IDisposable { Guid UniqueValue { get; } void Open(); }
    public class UniqueValueClass : IUniqueValue, IDisposable
    {
        public UniqueValueClass()
        {
            this.UniqueValue = Guid.NewGuid();
            IsOpen = false;
        }
        public Guid UniqueValue { get; private set; }

        public bool IsOpen { get; private set; }

        public void Open()
        {
            if (IsOpen)
            {
                throw new Exception("Already open.");
            }

            IsOpen = true;

        }

        public void Dispose()
        {
            IsOpen = false;
        }
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
            ServerDependency.Open();
            BO_ChildA = Portal.CreateChild();
            BO_ChildA_1 = Portal.CreateChild();

        }

        protected void DataPortal_Fetch()
        {
            using (BypassPropertyChecks)
            {
                this.UniqueValue = ServerDependency.UniqueValue;
            }

            this.BO_ChildA = Portal.Fetch();
            this.BO_ChildA_1 = Portal.Fetch();

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

        private void DataPortal_Fetch()
        {

            this.UniqueValue = ServerDependency.UniqueValue;

            Add(portal.Fetch());
            Add(portal.Fetch());

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

        private void DataPortal_Fetch()
        {
            using (BypassPropertyChecks)
            {
                this.UniqueValue = ServerDependency.UniqueValue;
                this.Branch = portal.Fetch(); ;
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

        private void DataPortal_Fetch()
        {
            using (BypassPropertyChecks)
            {
                this.UniqueValue = ServerDependency.UniqueValue;
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
