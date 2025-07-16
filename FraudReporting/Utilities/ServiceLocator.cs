using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace FraudReporting.Utilities
{
    /// <summary>
    /// Service Locator utility class. Should only be used as a last resort instead of properly injecting dependancies. 
    /// 
    /// Note that this is a refactor of the method found here: https://dotnetcoretutorials.com/2018/05/06/servicelocator-shim-for-net-core/
    /// Instead of as above where there was as single service locator 
    /// 
    /// </summary>
    public class ServiceLocator
    {
        private static IServiceCollection _serviceCollection;
        private ServiceProvider serviceProvider;

        public ServiceLocator()
        {
            if (_serviceCollection == null)
                throw new Exception("No static service collection was definied. This must be set in Startup.ConfigureServices() in order to create service locator instances.");
            this.serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        public ServiceLocator(ServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public static ServiceLocator Current
        {
            get
            {
                //NOTE: At the moment this'll just always return a brand-new ServiceLocator instance every time to ensure there's no singleton issues. Could
                //potentially refactor this to reset to one per request to ensure the AddScoped() dependencies work properly.
                return new ServiceLocator();
            }
        }

        public static void SetServiceCollection(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public object GetInstance(Type serviceType)
        {
            return serviceProvider.GetService(serviceType);
        }

        public TService GetInstance<TService>()
        {
            return serviceProvider.GetService<TService>();
        }
    }
}
