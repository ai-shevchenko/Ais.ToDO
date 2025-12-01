using MediatR;

namespace Ais.Commons.CQRS.Notifications;

public sealed record NotificationWrapper<TModel> : INotification
{
    public required TModel Model { get; init; }
}