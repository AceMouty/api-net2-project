using Application.Models;

namespace Application.Repositories;

public class UserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    
    public Task<bool> CreateAsync(User user)
    {
        _users.Add(user);
        return Task.FromResult(true);
    }

    public Task<User?> ExistsAsync(Guid userId)
    {
        return Task.FromResult(_users.FirstOrDefault(u => u.Id == userId));
    }

    public Task<bool> ExistsAsync(Guid userId, out User? user)
    {
        var userExist = _users.Any(u => u.Id == userId);

        user = userExist
            ? _users.First(u => u.Id == userId)
            : null;
        
        return Task.FromResult(userExist);
    }
}