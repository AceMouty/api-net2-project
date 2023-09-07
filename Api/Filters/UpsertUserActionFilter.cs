using System.Net;
using Api.Contracts.Requests;
using Api.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters;

[AttributeUsage(AttributeTargets.Method)]
public class UpsertUserActionFilter :  Attribute, IAsyncActionFilter
{
    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var validator  =
            context.HttpContext.RequestServices.GetRequiredService<UpsertUserValidator>();

        if (context.ActionArguments.TryGetValue("user", out var user))
        {
            var validationResult = validator.Validate((user as UpsertUserRequest)!);
            if (!validationResult.IsValid)
            {
                var problemDetails = GetProblemDetails(validationResult.Errors, context.HttpContext.Request.Path);
                context.Result = new BadRequestObjectResult(problemDetails);
                return Task.CompletedTask;
            }
        }
        else
        {
            context.Result = new BadRequestObjectResult("Invalid request body");
            return Task.CompletedTask;
        }

        next();
        return Task.CompletedTask;
    }

    private static ValidationProblemDetails GetProblemDetails(List<ValidationFailure> errors, PathString path)
    {
        var problemDetails = new ValidationProblemDetails
        {
            Title = "Validation failed",
            Status = (int)HttpStatusCode.BadRequest,
            Instance = (string)path
        };
        
        foreach (var error in errors)
        {
            problemDetails.Errors.Add(error.PropertyName.ToLower(), new[] { error.ErrorMessage });
        }

        return problemDetails;
    }
}