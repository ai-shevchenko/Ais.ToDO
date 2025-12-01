namespace Ais.Commons.Core;

public class BaseEntity<TId> : IEntity<TId>, IEquatable<BaseEntity<TId>>
    where TId : IEquatable<TId>
{
    protected BaseEntity()
    {
        Id = default!;
    }
    
    protected BaseEntity(TId id)
    {
        Id = id;
    }
    
    public TId Id { get; }

    public bool IsNew => Id.Equals(default);
    
    public bool Equals(BaseEntity<TId>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((BaseEntity<TId>)obj);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<TId>.Default.GetHashCode(Id);
    }
}