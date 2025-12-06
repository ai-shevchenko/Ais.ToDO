using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace Ais.Commons.Tests;

public abstract class BaseTests<TWebApplicationFactory, TEntryPoint> : IAsyncLifetime, IDisposable
    where TWebApplicationFactory : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    protected readonly TWebApplicationFactory Factory;
    
    protected BaseTests(TWebApplicationFactory factory)
    {
        Factory = factory;
    }

    protected virtual HttpClient Client
    {
        get
        {
            field ??= Factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            return field;
        }
    }
    
    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected TService GetService<TService>() 
        where TService : notnull
    {
        return Factory.Services.GetRequiredService<TService>();
    }
    
    protected TService GetService<TService>(object? key)
        where TService : notnull
    {
        return Factory.Services.GetRequiredKeyedService<TService>(key);
    }
    
    protected async Task ExecuteAsync(Func<IServiceProvider, Task> func)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        await func(scope.ServiceProvider);
    }

    protected async Task<TResult> ExecuteAsync<TResult>(Func<IServiceProvider, Task<TResult>> func)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        return await func(scope.ServiceProvider);
    }
    
    protected async Task ExecuteAsync<TService>(Func<TService, Task> func)
        where TService : class
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<TService>();
        await func(service);
    }
    
    protected async Task<TResult> ExecuteAsync<TService, TResult>(Func<TService, Task<TResult>> func)
        where TService : class
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<TService>();
        return await func(service);
    }
    
    public void Dispose()
    {
        Factory.Dispose();
    }
}