namespace Ais.ToDo.Infrastructure.Contracts;

public interface ICurrenUserProvider
{
    CurrentUserDto<T> GetCurrentUser<T>()
        where T : IParsable<T>;
}

public sealed record CurrentUserDto<TId>
{
    public required TId UserId { get; init; }
    public required string UserName { get; init; }
    public required string Description { get; init; }
}

