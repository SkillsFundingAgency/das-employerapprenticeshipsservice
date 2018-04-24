using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.ServiceLocation;
using NLog;
using StructureMap;

namespace SFA.DAS.EAS.Web.DependencyResolution
{
    public class StructureMapDependencyScope : ServiceLocatorImplBase
    {
        public IContainer Container { get; set; }

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private const string NestedContainerKey = "Nested.Container.Key";

        public IContainer CurrentNestedContainer {
            get
            {
                return (IContainer)HttpContext.Items[NestedContainerKey];
            }

            set
            {
                HttpContext.Items[NestedContainerKey] = value;
            }
        }

        private HttpContextBase HttpContext
        {
            get
            {
                var ctx = Container.TryGetInstance<HttpContextBase>();
                return ctx ?? new HttpContextWrapper(System.Web.HttpContext.Current);
            }
        }

        public StructureMapDependencyScope(IContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            Container = container;
        }

        public void CreateNestedContainer()
        {
            if (CurrentNestedContainer != null)
            {
                return;
            }

            CurrentNestedContainer = Container.GetNestedContainer();
        }

        public void Dispose()
        {
            DisposeNestedContainer();
            Container.Dispose();
        }

        public void DisposeNestedContainer()
        {
            if (CurrentNestedContainer != null)
            {
                CurrentNestedContainer.Dispose();
				CurrentNestedContainer = null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return DoGetAllInstances(serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            Logger.Trace($"Getting all instances of type '{serviceType.Name}' from container");

            return (CurrentNestedContainer ?? Container).GetAllInstances(serviceType).Cast<object>();
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            Logger.Trace($"Getting instance of type '{serviceType.Name}' from container");

            try
            {
                IContainer container = (CurrentNestedContainer ?? Container);

                if (string.IsNullOrEmpty(key))
                {
                    return serviceType.IsAbstract || serviceType.IsInterface
                        ? container.TryGetInstance(serviceType)
                        : container.GetInstance(serviceType);
                }

                return container.GetInstance(serviceType, key);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                throw;
            }
        }
    }
}