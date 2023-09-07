using System.Net;
using Application.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters;

[AttributeUsage(AttributeTargets.Method)]
public class ValidateUserFilter : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

        var actionArgs = context.ActionArguments;
        // if (actionArgs.TryGetValue("userId", out var userId))
        // {
        //     var guidUserId = new Guid(userId!.ToString()!);
        //     var userExists = await userRepository.ExistsAsync(guidUserId, out var user);
        //
        //     if (!userExists)
        //     {
        //         var problemDetails = GetProblemDetails(context.HttpContext.Request.Path);
        //         context.Result = new BadRequestObjectResult(problemDetails);
        //         return;
        //     }
        //
        //     context.HttpContext.Items["user"] = user;
        // }
        
        if (actionArgs.TryGetValue("userId", out var userId))
        {
            var guidUserId = new Guid(userId!.ToString()!);
            var user = await userRepository.ExistsAsync(guidUserId);

            if (user is null)
            {
                var problemDetails = GetProblemDetails(context.HttpContext.Request.Path);
                context.Result = new BadRequestObjectResult(problemDetails);
                return;
            }

            context.HttpContext.Items["user"] = user;
        }

        
        await next();
    }

    private static ProblemDetails GetProblemDetails(PathString path)
    {
        var problemDetails = new ValidationProblemDetails
        {
            Title = "Validation failed",
            Status = (int)HttpStatusCode.BadRequest,
            Instance = (string)path
        };
        
        problemDetails.Errors.Add("userId", new[] { "user with the provided id does not exist" });

        return problemDetails;
    }
}