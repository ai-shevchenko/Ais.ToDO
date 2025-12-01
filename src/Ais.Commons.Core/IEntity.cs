namespace Ais.Commons.Core;

public interface IEntity<out TId>
    where TId : IEquatable<TId>
{
    TId Id { get; }
    
    bool IsNew { get; }
}