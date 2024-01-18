namespace Web.Api.Entities;

public abstract class Entity<T> : Entity
{
    public T Id { get; set; }
}

public abstract class Entity
{
    public virtual DateTime CreatedTime { get; set; }

    public virtual DateTime UpdatedTime { get; set; }
}