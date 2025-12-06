using Xunit;

namespace Ais.Commons.Tests.Containers;

public abstract class BaseContainerTests : IAsyncLifetime
{
    public abstract string ConnectionString { get; }
    public abstract Task InitializeAsync();
    public abstract Task DisposeAsync();
}