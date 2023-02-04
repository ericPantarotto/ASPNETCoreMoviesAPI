using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MoviesAPI.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection InstallServices(
            this IServiceCollection services,
            IConfiguration configuration,
            params Assembly[] assemblies)
        {
            var serviceInstallers = assemblies
                .SelectMany(a => a.DefinedTypes)
                .Where(IsAssignableToType<IServiceInstaller>)
                .Select(Activator.CreateInstance)
                .Cast<IServiceInstaller>();

            serviceInstallers.ToList().ForEach(serviceInstaller => 
                serviceInstaller.Install(services, configuration));

            return services;
        }

        private static bool IsAssignableToType<T>(TypeInfo typeInfo) =>
            typeof(T).IsAssignableFrom(typeInfo) &&
            !typeInfo.IsInterface &&
            !typeInfo.IsAbstract;
    }
    
}
