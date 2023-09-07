using Application.Models;

namespace Application.Repositories;

public class PostRepository : IPostRepository
{
    private readonly List<Post> _posts = new();


    public Task<bool> CreateAsync(Post post)
    {
        _posts.Add(post);
        return Task.FromResult(true);
    }
}