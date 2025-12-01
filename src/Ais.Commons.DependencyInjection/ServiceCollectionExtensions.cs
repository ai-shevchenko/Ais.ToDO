using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.Commons.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddModule<T>(this IServiceCollection services,  IConfiguration configuration)
        where T : Module
    {
        var module = Activator.CreateInstance<T>();
        module.Configure(services, configuration);
        return services;
    }
}