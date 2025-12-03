using AsadorMoron.Interfaces;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Administrador;
using AsadorMoron.ViewModels.Clientes;
using AsadorMoron.ViewModels.Contabilidad;
using AsadorMoron.ViewModels.Establecimientos;
using AsadorMoron.ViewModels.Informes;
using AsadorMoron.ViewModels.Repartidores;
using System;
using System.Collections.Generic;

namespace AsadorMoron.ViewModels.Base
{
    // Simple service locator - TODO: Migrate to MAUI DI with IServiceProvider
    internal class ViewModelLocator
    {
        private readonly Dictionary<Type, Type> _typeRegistrations = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
        private static readonly ViewModelLocator _instance = new ViewModelLocator();

        public static ViewModelLocator Instance => _instance;

        public ViewModelLocator()
        {
            // Services
            _typeRegistrations[typeof(INavigationService)] = typeof(NavigationService);
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type type)
        {
            // Check singletons first
            if (_singletons.TryGetValue(type, out var singleton))
                return singleton;

            // Check if we have a type registration (interface -> implementation)
            if (_typeRegistrations.TryGetValue(type, out var implementationType))
                return Activator.CreateInstance(implementationType);

            // Otherwise just create the type directly
            return Activator.CreateInstance(type);
        }

        public void Register<T>(T instance)
        {
            _singletons[typeof(T)] = instance;
        }

        public void Register<TInterface, T>() where T : TInterface
        {
            _typeRegistrations[typeof(TInterface)] = typeof(T);
        }

        public void RegisterSingleton<TInterface, T>() where T : TInterface
        {
            _typeRegistrations[typeof(TInterface)] = typeof(T);
            _singletons[typeof(TInterface)] = Activator.CreateInstance(typeof(T));
        }
    }
}
