using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Csla;
using Csla.Serialization.Mobile;
using Csla.Core;
using Autofac;
using System.Linq.Expressions;
using System.Reflection;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using System.Threading;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace CslaAsyncAwait.Lib.Server
{
    [Serializable]
    public class ObjectPortalScopeException : Exception
    {
        private Type _genericType = null;

        public ObjectPortalScopeException(Type portalGenericType)
            : base(string.Format("The ObjectPortal<{0}> does not have a registration in the container for {0}", portalGenericType))
        {
            _genericType = portalGenericType;
        }

        public ObjectPortalScopeException(System.Runtime.Serialization.SerializationInfo info, StreamingContext context)
            : base(info, context) 
        { }

        public Type RequestedType
        {
            get { return _genericType; }
        }
    }

    ////disable .NET compiler warning #67 because we are sealing a generic
#pragma warning disable 67

    [Serializable]
    public sealed class ObjectPortal<T>
        : IObjectPortal<T>
        where T : class, IMobileObject
    {
        #region Fields

        private readonly Type _concreteType;

        #endregion

        #region ctor

        public ObjectPortal(ILifetimeScope scope)
        {
            //first check T and see if we are getting and abstract or interface type - if we are - we can use the scope to resolve T
            //if T is not an interface or abstract - then we don't even need to check the container for it - just use the type directly
            Type genericType = typeof(T);

            if (genericType.IsInterface || genericType.IsAbstract)
            {
                IComponentRegistration registration = scope.ComponentRegistry.RegistrationsFor(new TypedService(genericType)).FirstOrDefault();
                if (registration != null)
                {
                    IInstanceActivator activator = registration.Activator as IInstanceActivator;
                    
                    if (activator != null)
                    {
                        _concreteType = activator.LimitType;
                    }
                }

                if (_concreteType == null)
                {
                    throw new ObjectPortalScopeException(genericType);
                }
            }
            else
            {
                //if we were given a non-abstract type already - then just use it
                _concreteType = typeof(T);
            }
        }

        #endregion

        public T Execute(T command)
        {
            T retObj = default(T);

            Type type = typeof(T);
            if (type.IsInterface || type.IsAbstract)
            {
                retObj = (T)typeof(DataPortal).GetMethod("Execute")
                                              .MakeGenericMethod(_concreteType)
                                              .Invoke(null, new object[] { command });
            }
            else
            {
                retObj = DataPortal.Execute<T>(command);
            }

            return retObj;
        }

        public async Task<T> ExecuteAsync(T command)
        {
            Type type = typeof(T);
            T retObj = default(T);

            if (type.IsInterface || type.IsAbstract)
            {
                Task asyncTsk = typeof(DataPortal).GetMethod("ExecuteAsync")
                                                  .MakeGenericMethod(_concreteType)
                                                  .Invoke(null, new object[] { command }) as Task;

                await asyncTsk;

                retObj = VerifyAsyncResultFromTask(asyncTsk);
            }
            else
            {
                retObj = await DataPortal.ExecuteAsync<T>(command);
            }

            return retObj;
        }

        public T Create()
        {
            Type type = typeof(T);
            T retObj = default(T);

            if (type.IsInterface || type.IsAbstract)
            {
                retObj = (T)typeof(DataPortal).GetMethod("Create", Type.EmptyTypes)
                                              .MakeGenericMethod(_concreteType)
                                              .Invoke(null, null);
            }
            else
            {
                retObj = DataPortal.Create<T>();
            }

            return retObj;
        }

        public T Create(object criteria)
        {
            Type type = typeof(T);
            T retObj = default(T);

            if (type.IsInterface || type.IsAbstract)
            {
                retObj = (T)typeof(DataPortal).GetMethod("Create", new Type[] { typeof(object) })
                                              .MakeGenericMethod(_concreteType)
                                              .Invoke(null, new object[]{ criteria });
            }
            else
            {
                retObj = DataPortal.Create<T>(criteria);
            }

            return retObj;
        }

        public async Task<T> CreateAsync(object criteria)
        {
            Type type = typeof(T);
            T retObj = default(T);

            if (type.IsInterface || type.IsAbstract)
            {
                Task result = typeof(DataPortal).GetMethod("CreateAsync", new Type[] { typeof(object) })
                                                .MakeGenericMethod(_concreteType)
                                                .Invoke(null, new object[] { criteria }) as Task;
                await result;

                retObj = VerifyAsyncResultFromTask(result);
            }
            else
            {
                retObj = await DataPortal.CreateAsync<T>(criteria);
            }

            return retObj;
        }

        public async Task<T> CreateAsync()
        {
            Type type = typeof(T);
            T retObj = default(T);

            if (type.IsInterface || type.IsAbstract)
            {
                Task result = typeof(DataPortal).GetMethod("CreateAsync", Type.EmptyTypes)
                                                .MakeGenericMethod(_concreteType)
                                                .Invoke(null, null) as Task;

                await result;

                retObj = VerifyAsyncResultFromTask(result);
            }
            else
            {
                retObj = await DataPortal.CreateAsync<T>();
            }

            return retObj;
        }

        public void Delete(object criteria)
        {
            Type type = typeof(T);
            
            if (type.IsInterface || type.IsAbstract)
            {
                typeof(DataPortal).GetMethod("Delete", new Type[] { typeof(object) })
                                  .MakeGenericMethod(_concreteType)
                                  .Invoke(null, new object[] { criteria });
            }
            else
            {
                DataPortal.Delete<T>(criteria);
            }
        }

        public async Task DeleteAsync(object criteria)
        {
            Type type = typeof(T);
            
            if (type.IsInterface || type.IsAbstract)
            {
                Task asyncTask = (Task)typeof(DataPortal).GetMethod("DeleteAsync", new Type[] { typeof(object) })
                                                         .MakeGenericMethod(_concreteType)
                                                         .Invoke(null, new object[] { criteria });
                await asyncTask;

                if (asyncTask.Exception != null)
                {
                    throw asyncTask.Exception;
                }
            }
            else
            {
                await DataPortal.DeleteAsync<T>(criteria);
            }
        }
            
        public T Fetch()
        {
            Type type = typeof(T);
            T retObj = default(T);

            if (type.IsInterface || type.IsAbstract)
            {
                retObj = (T)typeof(DataPortal).GetMethod("Fetch", new Type[]{})
                                              .MakeGenericMethod(_concreteType)
                                              .Invoke(null, null);
            }
            else
            {
                retObj = DataPortal.Fetch<T>();
            }

            return retObj;
        }

        public T Fetch(object criteria)
        {
            Type type = typeof(T);
            T retObj = default(T);

            if (type.IsInterface || type.IsAbstract)
            {
                retObj = (T)typeof(DataPortal).GetMethod("Fetch", new Type[] { typeof(object) })
                                              .MakeGenericMethod(_concreteType)
                                              .Invoke(null, new object[] { criteria });
            }
            else
            {
                retObj = DataPortal.Fetch<T>(criteria);
            }

            return retObj;
        }

        public async Task<T> FetchAsync(object criteria)
        {
            Type type = typeof(T);
            T retObj = default(T);

            if (type.IsInterface || type.IsAbstract)
            {
                Task asyncTask = typeof(DataPortal).GetMethod("FetchAsync", new Type[] { typeof(object) })
                                                   .MakeGenericMethod(_concreteType)
                                                   .Invoke(null, new object[] { criteria }) as Task;

                await asyncTask;

                retObj = VerifyAsyncResultFromTask(asyncTask);
            }
            else
            {
                retObj = await DataPortal.FetchAsync<T>(criteria);
            }

            return retObj;
        }

        public async Task<T> FetchAsync()
        {
            Type type = typeof(T);
            T retObj = default(T);

            if (type.IsInterface || type.IsAbstract)
            {
                Task asyncTsk = typeof(DataPortal).GetMethod("FetchAsync", Type.EmptyTypes)
                                                  .MakeGenericMethod(_concreteType)
                                                  .Invoke(null, null) as Task;

                await asyncTsk;

                retObj = VerifyAsyncResultFromTask(asyncTsk);
            }
            else
            {
                retObj = await DataPortal.FetchAsync<T>();
            }

            return retObj;
        }

        public T Update(T busObjInstance)
        {
            Type type = typeof(T);
            T retObj = default(T);

            if (type.IsInterface || type.IsAbstract)
            {
                retObj = (T)typeof(DataPortal).GetMethod("Update")
                                              .MakeGenericMethod(_concreteType)
                                              .Invoke(null, new object[] { busObjInstance });
            }
            else
            {
                retObj = DataPortal.Update<T>(busObjInstance);
            }

            return retObj;
        }
         
        public async Task<T> UpdateAsync(T busObjInstance)
        {
            Type type = typeof(T);
            T retObj = default(T);

            if (type.IsInterface || type.IsAbstract)
            {
                Task asyncTsk = typeof(DataPortal).GetMethod("UpdateAsync")
                                                  .MakeGenericMethod(_concreteType)
                                                  .Invoke(null, new object[] { busObjInstance }) as Task;

                await asyncTsk;

                retObj = VerifyAsyncResultFromTask(asyncTsk);
            }
            else
            {
                retObj = await DataPortal.UpdateAsync<T>(busObjInstance);
            }

            return retObj;
        }

        //** NOTE: we do not need to do the concrete type resolution on the 
        //Child DataPortal_XYZ methods because DataPortal_CreateChild - etc.. do not use the attributes for runlocal etc...
        public T CreateChild()
        {
            return DataPortal.CreateChild<T>();
        }

        //** NOTE: we do not need to do the concrete type resolution on the 
        //Child DataPortal_XYZ methods because DataPortal_CreateChild - etc.. do not use the attributes for runlocal etc...
        public T CreateChild(params object[] parameters)
        {
            return DataPortal.CreateChild<T>(parameters);
        }

        //** NOTE: we do not need to do the concrete type resolution on the 
        //Child DataPortal_XYZ methods because DataPortal_UpdateChild - etc.. do not use the attributes for runlocal etc...
        public void UpdateChild(T childObj, params object[] parameters)
        { 
            DataPortal.UpdateChild(childObj, parameters);
        }

        //** NOTE: we do not need to do the concrete type resolution on the 
        //Child DataPortal_XYZ methods because DataPortal_UpdateChild - etc.. do not use the attributes for runlocal etc...
        public void UpdateChild(T childObj)
        {
            DataPortal.UpdateChild(childObj);
        }

        //** NOTE: we do not need to do the concrete type resolution on the 
        //Child DataPortal_XYZ methods because DataPortal_CreateChild - etc.. do not use the attributes for runlocal etc...
        public T FetchChild()
        {
            return DataPortal.FetchChild<T>();
        }

        //** NOTE: we do not need to do the concrete type resolution on the 
        //Child DataPortal_XYZ methods because DataPortal_CreateChild - etc.. do not use the attributes for runlocal etc...
        public T FetchChild(params object[] parameters)
        {
            return DataPortal.FetchChild<T>(parameters);
        }

        public ContextDictionary GlobalContext
        {
            get { return ApplicationContext.GlobalContext; }
        }

        #region Helper Methods

        //private Lazy<PropertyInfo> _taskResultProperty = new Lazy<PropertyInfo>(()=>
        //{
        //    PropertyInfo inf = typeof(Task<>).MakeGenericType(typeof(T)).GetProperty("Result");
        //    return inf;
        //});
        
        //private PropertyInfo AsyncResult { get { return _taskResultProperty.Value; } }
        private T VerifyAsyncResultFromTask(Task dpMethodAsyncTask)
        {
            if (dpMethodAsyncTask.Exception != null)
            {
                throw dpMethodAsyncTask.Exception;
            }
            else
            {
                return dpMethodAsyncTask.GetType().GetProperty("Result").GetValue(dpMethodAsyncTask) as T;
            }
        }

        #endregion

        #region Deprecated "Begin*" methods and events (for old style async calls)

        //** These are excluded from code coverage because they are flagged as obsolete and use the argument to generate a compiler error if you try to use them at all 
        //** (even to test for exceptions being thrown)
        //** The reason we throw exceptions in spite of the fact that calling the method is not allowed by the C# Compiler 
        //** is to prevent someone from calling the methods using Reflection (which would not trigger the compiler error if done properly)
        //** FWIW: we could not find a pragma that would turn off the compiler error so we could call it in a unit test - so we are not testing them

        [Obsolete("BeginCreate(object,object) is deprecated and should never be used, use CreateAsync(object, object) instead.", true)]
        [ExcludeFromCodeCoverage]
        public void BeginCreate(object criteria, object userState)
        {
            throw new NotSupportedException("BeginCreate(object,object) is deprecated and should never be used, use CreateAsync(object, object) instead.");
        }

        [Obsolete("BeginCreate(object) is deprecated and should never be used, use CreateAsync(object) instead.", true)]
        [ExcludeFromCodeCoverage]
        public void BeginCreate(object criteria)
        {
            throw new NotSupportedException("BeginCreate(object) is deprecated and should never be used, use CreateAsync(object) instead.");
        }

        [Obsolete("BeginCreate() is deprecated and should never be used, use CreateAsync() instead.", true)]
        [ExcludeFromCodeCoverage]
        public void BeginCreate()
        {
            throw new NotSupportedException("BeginCreate() is deprecated and should never be used, use CreateAsync() instead.");
        }

        [Obsolete("BeginDelete(object,object) is deprecated and should never be used, use DeleteAsync(object,object) instead.", true)]
        [ExcludeFromCodeCoverage]
        public void BeginDelete(object criteria, object userState)
        {
            throw new NotSupportedException("BeginDelete(object,object) is deprecated and should never be used, use DeleteAsync(object,object) instead.");
        }

        [Obsolete("BeginDelete(object) is deprecated and should never be used, use DeleteAsync(object) instead.", true)]
        [ExcludeFromCodeCoverage]
        public void BeginDelete(object criteria)
        {
            throw new NotSupportedException("BeginDelete(object) is deprecated and should never be used, use DeleteAsync(object) instead.");
        }

        [Obsolete("BeginExecute(T,object) is deprecated and should never be used", true)]
        [ExcludeFromCodeCoverage]
        public void BeginExecute(T command, object userState)
        {
            throw new NotSupportedException("BeginExecute(T,object) is deprecated and should never be used, use ExecuteAsync(T, object) instead.");
        }

        [Obsolete("BeginExecute(T) is deprecated and should never be used, use ExecuteAsync(T) instead.", true)]
        [ExcludeFromCodeCoverage]
        public void BeginExecute(T command)
        {
            throw new NotSupportedException("BeginExecute(T) is deprecated and should never be used, use ExecuteAsync(T) instead.");
        }

        [Obsolete("BeginFetch(object,object) is deprecated and should never be used, use FetchAsync(object,object) instead.", true)]
        [ExcludeFromCodeCoverage]
        public void BeginFetch(object criteria, object userState)
        {
            throw new NotSupportedException("BeginFetch(object,object) is deprecated and should never be used, use FetchAsync(object,object) instead.");
        }

        [Obsolete("BeginFetch(object) is deprecated and should never be used, use FetchAsync(object) instead.", true)]
        [ExcludeFromCodeCoverage]
        public void BeginFetch(object criteria)
        {
            throw new NotSupportedException("BeginFetch(object) is deprecated and should never be used, use FetchAsync(object) instead.");
        }

        [Obsolete("BeginFetch() is deprecated and should never be used, use FetchAsync() instead.", true)]
        [ExcludeFromCodeCoverage]
        public void BeginFetch()
        {
            throw new NotSupportedException("BeginFetch() is deprecated and should never be used, use FetchAsync() instead.");
        }

        [Obsolete("BeginUpdate(T, object) is deprecated and should never be used, use UpdateAsync(T,object) instead.", true)]
        [ExcludeFromCodeCoverage]
        public void BeginUpdate(T obj, object userState)
        {
            throw new NotSupportedException("BeginUpdate(T, object) is deprecated and should never be used, use UpdateAsync(T,object) instead.");
        }

        [Obsolete("BeginUpdate(T) is deprecated and should never be used, use UpdateAsync(T) instead.", true)]
        [ExcludeFromCodeCoverage]
        public void BeginUpdate(T obj)
        {
            throw new NotSupportedException("BeginUpdate(T) is deprecated and should never be used, use UpdateAsync(T) instead.");
        }

        [Obsolete("The CreateCompleted Event is deprecated and should never be used", true)]
        [ExcludeFromCodeCoverage]
        public event EventHandler<DataPortalResult<T>> CreateCompleted;

        [Obsolete("The DeleteCompleted Event is deprecated and should never be used", true)]
        [ExcludeFromCodeCoverage]
        public event EventHandler<DataPortalResult<T>> DeleteCompleted;

        [Obsolete("The ExecuteCompleted Event is deprecated and should never be used", true)]
        [ExcludeFromCodeCoverage]
        public event EventHandler<DataPortalResult<T>> ExecuteCompleted;

        [Obsolete("The FetchCompleted Event is deprecated and should never be used", true)]
        [ExcludeFromCodeCoverage]
        public event EventHandler<DataPortalResult<T>> FetchCompleted;

        [Obsolete("The UpdateCompleted Event is deprecated and should never be used", true)]
        [ExcludeFromCodeCoverage]
        public event EventHandler<DataPortalResult<T>> UpdateCompleted;

        #endregion
    }
    
    // restore .NET compiler warning #67 which we disabled for ObjectFactory<T> definition (sealing a generic class warning)
#pragma warning restore

}
