using Sirius.LogbookViewer.Product;
using Sirius.LogbookViewer.UI.Service;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;

namespace Sirius.LogbookViewer.App.Service
{
    /// <summary>
    /// A lightweight IoC container implementation.
    /// </summary>
    public class ServiceContainer : IServiceContainer
    {
        private static readonly Lazy<IServiceContainer> lazy = new Lazy<IServiceContainer>(() => new ServiceContainer());
        private ConcurrentDictionary<Type, Type> _registeredTypes;
        private ConcurrentDictionary<Type, object> _registeredInstances;

        [Import(AllowDefault = true)]
        private IResourceManager _productSpecificResourceManager;

        [Import]
        private IParser _fileParser;

        public static IServiceContainer Instance { get { return lazy.Value; } }

        private ServiceContainer()
        {
            BuildContainer();
        }

        private void BuildContainer()
        {
            _registeredTypes = new ConcurrentDictionary<Type, Type>();
            _registeredInstances = new ConcurrentDictionary<Type, object>();
            //Register<IFilePicker, StandaloneFileDialog>();
            Register<IFilePicker, FilePicker>();
            Register<IWaitIndicator, LoadingDialog>();

            var catalog = new DirectoryCatalog(".");
            using (var container = new CompositionContainer(catalog))
            {
                container.ComposeParts(this);
            }

            Register<IResourceManager>(_productSpecificResourceManager);
            Register<UIResourceManager>(new UIResourceManager());
            Register<IParser>(_fileParser);
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
            if (_registeredInstances.ContainsKey(typeof(T)))
            {
                return (T)_registeredInstances[typeof(T)];
            }

            return (T)CreateInstance(typeof(T));
        }

        /// <summary>
        ///  Instantiates the requested object type along with all of its dependent types.
        /// </summary>
        private object CreateInstance(Type type)
        {
            if (!_registeredTypes.ContainsKey(type)) // this type is not registered
            {
                return null;
            }

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
