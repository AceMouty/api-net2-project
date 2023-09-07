using System.Diagnostics.CodeAnalysis;
using Application.Models;

namespace Application.Repositories;

public interface IUserRepository
{
    Task<bool> CreateAsync(User user);
    Task<User?> ExistsAsync(Guid userId);
    // Stretch
    Task<bool> ExistsAsync(Guid userId, [MaybeNullWhen(false)] out User? user);
}