using System.Diagnostics;
using Api.Contracts.Requests;
using Api.Filters;
using Application.Models;
using Application.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class UserController : ControllerBase
{

    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;
    
    public UserController(IUserRepository userRepository, IPostRepository postRepository)
    {
        _userRepository = userRepository;
        _postRepository = postRepository;
    }

    [HttpPost(ApiEndpoints.Users.Register)]
    [UpsertUserActionFilter]
    public async Task<IActionResult> CreateUser([FromBody] UpsertUserRequest user)
    {

        // TODO: create mapping extension for this
        var newUser = new User { Id = Guid.NewGuid(), Name = user.Name! };
        await _userRepository.CreateAsync(newUser);
        
        
        Console.WriteLine(Activity.Current?.Id ?? HttpContext?.TraceIdentifier);
        return Created(ApiEndpoints.Users.Register, newUser);
    }

    [HttpPost(ApiEndpoints.Users.Posts.Create)]
    [ValidateUserFilter]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest post, [FromRoute] Guid userId)
    {
        Console.WriteLine(HttpContext.Items["user"]);
        var newPost = new Post { Id = Guid.NewGuid(), Text = post.Text!, UserId = userId };
        await _postRepository.CreateAsync(newPost);

        return Created(ApiEndpoints.Users.Posts.Create, post);
    }
}