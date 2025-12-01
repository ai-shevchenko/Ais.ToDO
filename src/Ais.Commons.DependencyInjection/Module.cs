using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.Commons.DependencyInjection;

public abstract class Module
{
    public abstract void Configure(IServiceCollection services, IConfiguration configuration);
}