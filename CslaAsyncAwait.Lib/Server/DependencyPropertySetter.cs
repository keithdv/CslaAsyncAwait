using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Csla.Core;
using Csla.Serialization.Mobile;
using Csla.Rules;
using Autofac.Core;

namespace CslaAsyncAwait.Lib.Server
{
    /// <summary>
    /// utility class that handles setting all the dependency properties on a given business object or command
    /// </summary>
    public static class DependencyPropertySetter
    {
        /// <summary>
        /// used to access private and public nonstatic properties on business objects that have Dependency Attributes applied
        /// </summary>
        private const BindingFlags DPABindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        public static void InitializeDependencies(this IMobileObject obj, ILifetimeScope scope)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            if (scope == null)
            {
                throw new ArgumentNullException("scope");
            }

            obj.GetType().GetProperties(DPABindingFlags)
                .Where(pi => Attribute.IsDefined(pi, typeof(DependencyAttribute)))
                .ToList<PropertyInfo>()
                .ForEach(pi =>
                {
                    DependencyAttribute atb = pi.GetCustomAttribute<DependencyAttribute>();

                    //if the dependency is registered in the scope - resolve it - if not - leave it null
                    if (scope.IsRegistered(pi.PropertyType))
                    {
                        //get the instance now - we need to make sure the underlying type for the requested service may implement IDisposable
                        //whereas the Interface for the requested service may not - we need to be sure in both cases!
                        object depPropInstance = scope.Resolve(pi.PropertyType);

                        //if the dependency is client side and implements IDisposable - 
                        //make sure the registration with Autofac is ExternallyOwned - otherwise throw an exception
                        //this should not be too expensive from a performance standpoint because we only do it if both conditions of
                        //1: the object is going to come back to the client side - AND - 
                        //2: the object implements the IDisposable interface 
                        //(which will not happen very often except in the case of bringing back IGridsObjectPortal<T> types)
                        //if ((atb.DependencyScope == ResolutionScope.Client || atb.DependencyScope == ResolutionScope.ClientAndServer)
                        //    && typeof(IDisposable).IsAssignableFrom(depPropInstance.GetType()))
                        //{
                        //    IComponentRegistration reg = scope
                        //                                             .ComponentRegistry
                        //                                             .RegistrationsFor(new TypedService(pi.PropertyType))
                        //                                             .Where(e => e.Ownership == InstanceOwnership.ExternallyOwned).FirstOrDefault();
                        //    if (reg == null)
                        //    {
                        //        throw new ResolutionOwnershipException(string.Format(ExceptionMessages.DepPropertySetter_InvalidContainerOwnership, pi.Name, pi.PropertyType), obj.GetType(), pi, atb.DependencyScope, Csla.ApplicationContext.LogicalExecutionLocation.ToString());
                        //    }
                        //}

                        pi.SetValue(obj, depPropInstance);
                    }
                });

        }

        //disposes the scoped objects ILifetimeScope and sets all dependency properties to null
        public static void FinalizeDependencies(this IMobileObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            obj.GetType().GetProperties(DPABindingFlags)
                .Where(pi => Attribute.IsDefined(pi, typeof(DependencyAttribute)))
                .ToList<PropertyInfo>()
                .ForEach(pi =>
                {
                    DependencyAttribute atb = pi.GetCustomAttribute<DependencyAttribute>();
                    //we only clear out the scoped object if it is only used on the server side
                    //we checked for IDisposable and made sure that the client side dependency was not being tracked by 
                    //the server side resolution in Autofac when InitializeObject was called so we will be okay to not clear it out
                    //and do not need to worry about it getting disposed here as well
                    pi.SetValue(obj, null);
                });
        }
    }

}
