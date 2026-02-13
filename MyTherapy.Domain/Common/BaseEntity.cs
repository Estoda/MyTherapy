namespace MyTherapy.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid(); // Generates a new unique identifier for each entity
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Sets the creation time to the current UTC time
}
