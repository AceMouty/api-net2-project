namespace Application.Models;

public class User
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }

    public override string ToString()
    {
        return $"{{ Id: {Id}, Name: {Name} }}";
    }
}