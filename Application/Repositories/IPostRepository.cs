using Application.Models;

namespace Application.Repositories;

public interface IPostRepository
{
    Task<bool> CreateAsync(Post post);
}