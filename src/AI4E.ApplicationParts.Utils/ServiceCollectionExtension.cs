using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AI4E.ApplicationParts.Utils
{
    public static class ServiceCollectionExtension
    {
        public static ApplicationPartManager GetApplicationPartManager(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var manager = services.GetService<ApplicationPartManager>();
            if (manager == null)
            {
                manager = new ApplicationPartManager();

                var entryAssembly = Assembly.GetEntryAssembly();

                // Blazor cannot access the entry assembly apparently.
                if (entryAssembly != null)
                {
                    manager.ApplicationParts.Add(new AssemblyPart(entryAssembly));
                }
            }

            return manager;
        }

        public static void ConfigureApplicationParts(this IServiceCollection services, Action<ApplicationPartManager> configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var partManager = services.GetApplicationPartManager();
            configuration(partManager);
            services.TryAddSingleton(partManager);
        }

        private static T GetService<T>(this IServiceCollection services)
        {
            var serviceDescriptor = services.LastOrDefault(d => d.ServiceType == typeof(T));

            return (T)serviceDescriptor?.ImplementationInstance;
        }
    }
}
