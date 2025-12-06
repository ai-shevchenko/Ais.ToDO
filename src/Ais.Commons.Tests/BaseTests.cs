using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Ais.ToDo.Tests;

public abstract class BaseTests<TWebApplicationFactory, TEntryPoint> : IAsyncLifetime, IDisposable
    where TWebApplicationFactory : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    protected readonly TWebApplicationFactory Factory;
    protected readonly HttpClient Client;
    
    protected BaseTests(TWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
    }
    
    protected async Task ExecuteAsync(Func<IServiceProvider, Task> func)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        await func(scope.ServiceProvider);
    }

    protected async Task ExecuteAsync<T>(Func<T, Task> func)
        where T : class
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<T>();
        await func(service);
    }

    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Factory.Dispose();
    }
}