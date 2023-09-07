# .NET Middleware & Validation

## Introduction

In this challenge you will build an API, write custom middleware and filters that satisfies the requirements 
listed under the `Project Requirements` section.

Completion of this project demonstrates working understanding of 

- .NET Pipeline
- Middleware
- Action Filters
- Http Context
- Request Validation

## Prerequisites
.NET 7 SDK

## Instructions

### 1. Project Setup

- Create a new **empty** `solution` and call it whatever you want eg `MiddlewareAndValidation`. If using .NET CLI Tool
  use `dotnet new sln -n <project_name>`.
- Create two new Projects
  - `Api`:  Web API Project, this is where Controllers, Mapping Extension methods and API Contracts will live.
  - `Application`: Class Library, this is where our Models and Repositories will live along with Extension Methods for project registration.

Create a reference where `Api` project references the `Application` project.

If you need help with project creation and referencing using the `dotnet` cli tool reach out for help or you can also use 
`dotnet --help` and `dotnet new --help` to help figure out the needed commands.

#### 1.a Required NuGet Packages

`Application`
- Microsoft.Extensions.DependencyInjection.Abstractions: required for being able to register the `Application` project properly

#### 1.b API Contracts

In the `Api` project create a folder called `Contracts` and inside of Contracts create two classes

- `CreatePostRequest`: Represents a incoming request to make a new `Post`
- `UserUpsertRequest`: upsert means Update / Insert, this class is used for incoming requests that update or create a user.

These two classes will be used to represent the `request body` for incoming POST / PUT requests to the API for creating or updating
Posts or Users respectively.

#### 1.c Application Registration & In Memory DB Setup

In order to get the `Application` and `In-Memory` database setup correctly we will need to do the following...

- Setup repositories
```csharp
// File: Application/Respositories/UserRepository
public class UserRepository : IUserRepository
{
    // in-memory DB
    private readonly List<User> _users = new();
 
   // your repository methods here...   
}

// File: Application/Repositories/PostRepository
public class PostRepository : IPostRepository
{
    // in-memory DB
    private readonly List<Post> _posts = new();
 
   // your repository methods here...   
}
```

- Setup Registration File
```csharp
// File: Application/ApplicationServiceCollectionExtensions
public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IPostRepository, PostRepository>();
        return services;
    }
}
```

- Register the Application project
```csharp
// File: Api/Program.cs
var builder = WebApplication.CreateBuilder(args);

// Other code...

builder.Services.AddApplication();

var app = builder.Build();
```

**Important Note** If your application is forcing HTTP requests and giving you odd behavior related to HTTPS
you can delete this line here...

```csharp
var app = builder.Build();

// Other code...

app.UseHttpsRedirection(); // <- remove this line and restart server
```


### Middleware and Filter requirements

- `Logger`

  - Create a `logger middleware` that logs to the console the following information about each request in this format: 
    - `[MiddlewareAndValidationLogger] {request_method} | {request_url} | {timestamp}`
  - this middleware runs on every request made to the API

> NOTE: To send a response from a filter use the `Result` property on the `ActionExecutingContext` object
> Red on the [ActionExecutingContext here](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.actionexecutingcontext?view=aspnetcore-7.0) for more

- `Validate UserId Filter`

  - this filter will be used for all user endpoints that include an `id` parameter in the url (ex: `/api/users/:id`) and it should check the database to make sure there is a user with that id.
  - if the `id` parameter is valid, store the user object on `HttpContext` and allow the request to continue
  - if the `id` parameter does not match any user id in the database, respond with status `404` and `{ message: "user not found" }`
  - this filter should only be able to be used on `Controller Actions`. 
  - **Important Note** have your filter inherit from the `Attribute Class` and implement either `IActionFilter` or `IAsyncActionFilter`

- `ValidateUser`

  - Using [Data Annotations]() validate the `body` on a request to create or update a user
  - if the request `body` lacks the required `name` field, the API should respond with a Validation Error

- `Validate Post`

  - Using [Data Annotations]() validate the `body` on a request to create a new post
  - if the request `body` lacks the required `text` field, the API should respond with a Validation Error

### Repositories (Data Access)

IUserRepository

- `CreateAsync`: calling CreateAsync passing it a User will add the user to the database and return true when successfully added.
- `ExistsAsync`: calling ExistsAsync passing in a `userID` will return a User if the user exists, else returns null.

IPostRepository

- `CreateAsync`: calling CreateAsync passing it a `Post` will add the post to the database and return true when successfully added.

**All methods should return a Task, when returning use Task.FromResult**

#### Database Schemas

The _Database Schemas_ for the `users` and `posts` resources are:

##### Users

| field | data type | metadata                                            |
| ----- |-----------| --------------------------------------------------- |
| id    | Guid      | primary key, auto-increments, generated by database |
| name  | string    | required, unique                                    |

##### Posts

| field   | data type | metadata                                            |
| ------- |-----------| --------------------------------------------------- |
| id      | Guid      | primary key, auto-increments, generated by database |
| text    | text      | required                                            |
| user_id | Guid      | required, must be the `id` of an existing user      |

#### Stretch (Optional)

These tasks are less hand held than up above. Tasks vary in level of difficulty.
If you would like to implement these requirements, you are going to have
to bridge the gap by researching about .NET topics such as `Model Binding`, `Model Validation`, `ProblemDetailsObject`
`out parameters` and other items.

[Routes and Data Access]

You may have noticed this API is not feature complete. Implement missing routes so that full CRUD can be performed on both User and Post
resources including getting ALL Posts / Users or finding a User / Post by id. 
Implement any other functionality needed in the `repositories` to help support the newly created routes and their functionality.

[Filter] `Validate UserId Filter`

instead of the filter responding with `{ message: "user not found }` construct a `ProblemDetails` object
Setting the `Title` to `Validation Failed`, `Status` to `400`, `Instance` to the requested URL (this should come from the HTTP Context)
Add the key `userId` and error of `user with the provided id does not exist`

[IUserRepository Method] `ExistsAsync`

Create a `overload` of ExistsAsync, that accepts two values a `userId` and a `out parameter` for the `user`
your out parameter should be of type `User?` (nullable user). If a user exists return `true` and assign the 
`out paramter` to the found user. Else return false, and assign the `user out parameter` to be `null`

[Mapping Layer] 

In the `Api` project create a folder called `Mapping` with `static` classes of `UserMapping` and `PostMapping`
in these classes you will create `extension methods` that converts types declared in `Contracts` (created in step 1.b) to 
types that are defined in `Application.Models` respectively. You will want a way to map from `Contracts` -> `Models` and back
from `Models` -> `Contracts`. Your controllers are responsible for taking in a `contract` and converting it to a `model` before
calling on database actions **and** converting models back into contracts **when sending out a response**.

[Validation using Fluent Validation]

In the `Api` project, we have our `Contracts`. Our contracts were to be validated using Data Annotations. Pick one of the
contracts either `UpsertUserRequest` or `CreatePostRequest` and remove the data annotation(s).

Install the NuGet Package `FluentValidation.DependencyInjectionExtensions` to the `Api` project

In `Api` project create a new folder called `Validators` and create a class called `UpsertUserValidator` and write the needed
logic for validating the required field on the incoming request.

Create a new interface under `Api` project called `IApiAssemblyMarker` and use it to register **all validators** in the Api
assembly.

From there, create a new filter called `UpsertUserActionFilter` and preform the needed validation on the incoming request.

- If validation is in a failed state respond with a `ProblemDetailsObject`. Setting the `Title` to `Validation Failed`, `Status` to `400`, 
`Instance` to the requested URL (this should come from the HTTP Context). The key and value used here is going to be dynamic
and come from the `validation result` generated using your `UpsertUserValidator`.

- If validation is in a success state, then call on the next method in the pipeline. 
