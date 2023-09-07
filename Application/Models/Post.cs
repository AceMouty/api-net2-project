namespace Application.Models;

public class Post
{
    public required Guid Id { get; init; }
    public required string Text { get; init; }
    public required Guid UserId { get; init; }
}