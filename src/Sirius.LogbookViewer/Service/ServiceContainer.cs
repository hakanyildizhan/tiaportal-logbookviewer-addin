using Sirius.LogbookViewer.UI.Service;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sirius.LogbookViewer.Service
{
    /// <summary>
    /// A lightweight IoC container implementation.
    /// </summary>
    public class ServiceContainer : IServiceContainer
    {
        private static readonly Lazy<IServiceContainer> lazy = new Lazy<IServiceContainer>(() => new ServiceContainer());
        private ConcurrentDictionary<Type, Type> _registeredTypes;
        private ConcurrentDictionary<Type, object> _registeredInstances;

        public static IServiceContainer Instance { get { return lazy.Value; } }

        private ServiceContainer()
        {
            BuildContainer();
        }

        private void BuildContainer()
        {
            _registeredTypes = new ConcurrentDictionary<Type, Type>();
            _registeredInstances = new ConcurrentDictionary<Type, object>();
            Register<IFilePicker, FilePicker>();
            //Register<IWaitIndicator, TIALoadingDialog>();
            Register<IWaitIndicator, LoadingDialog>();
        }

        /// <summary>
        /// Registers a service with its concrete type.
        /// </summary>
        public void Register<S, T>() where T : S
        {
            _registeredTypes[typeof(S)] = typeof(T);
        }

        /// <summary>
        /// Registers a concrete service by itself.
        /// </summary>
        public void Register<S>()
        {
            _registeredTypes[typeof(S)] = typeof(S);
        }

        /// <summary>
        /// Registers a concrete service with its instance.
        /// </summary>
        public void Register<S>(object instance)
        {
            _registeredInstances[typeof(S)] = instance;
        }

        public T Resolve<T>()
        {
            return (T)CreateInstance(typeof(T));
        }

        /// <summary>
        ///  Instantiates the requested object type along with all of its dependent types.
        /// </summary>
        private object CreateInstance(Type type)
        {
            if (_registeredInstances.ContainsKey(type)) // if an instance is registered, return it
            {
                return _registeredInstances[type];
            }

            if (!_registeredTypes.ContainsKey(type)) // this type is also not registered by type
            {
                return null;
            }

            // type was not registered with an instance
            // look for type registration and instantiate the dependency tree
            var registeredType = _registeredTypes[type];
            var constructors = registeredType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            var injectedTypes = constructors[0].GetParameters().Select(a => a.ParameterType);

            if (!injectedTypes.Any())
            {
                return Activator.CreateInstance(registeredType);
            }

            var instances = new List<object>();
            foreach (var injectedType in injectedTypes)
            {
                instances.Add(CreateInstance(injectedType));
            }

            return constructors[0].Invoke(instances.ToArray());
        }
    }
}
