using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagicWords.Core
{
    /// <summary>
    /// Gestiona las dependencias principales del juego permitiendo un acoplamiento débil entre sistemas
    /// </summary>
    public class ServiceLocator
    {
        private static ServiceLocator instance;
        private readonly Dictionary<Type, object> services;

        public static ServiceLocator Instance
        {
            get
            {
                instance ??= new ServiceLocator();
                return instance;
            }
        }

        private ServiceLocator()
        {
            services = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Registra un servicio en el locator
        /// </summary>
        public void Register<T>(T service) where T : class
        {
            if (service == null)
            {
                Debug.LogError($"Can't register null service for type {typeof(T)}");
                return;
            }

            var type = typeof(T);
            if (services.ContainsKey(type))
            {
                Debug.LogWarning($"Service of type {type} already registered. Updating reference.");
            }

            services[type] = service;
            Debug.Log($"Service of type {type} registered successfully");
        }

        /// <summary>
        /// Obtiene un servicio registrado
        /// </summary>
        public T Get<T>() where T : class
        {
            var type = typeof(T);
            if (!services.ContainsKey(type))
            {
                Debug.LogError($"Service of type {type} not registered");
                return null;
            }

            return services[type] as T;
        }

        /// <summary>
        /// Elimina un servicio registrado
        /// </summary>
        public void Unregister<T>() where T : class
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
            {
                services.Remove(type);
                Debug.Log($"Service of type {type} unregistered");
            }
        }

        /// <summary>
        /// Limpia todos los servicios registrados
        /// </summary>
        public void Clear()
        {
            services.Clear();
            Debug.Log("All services cleared");
        }
    }
}