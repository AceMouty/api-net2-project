using Api.Contracts.Requests;
using FluentValidation;

namespace Api.Validators;

// ReSharper disable once ClassNeverInstantiated.Global
//
// Reason:
// class is apart of DI and is then pulled out in ActionFilter
public class UpsertUserValidator : AbstractValidator<UpsertUserRequest>
{
    public UpsertUserValidator()
    {
        RuleFor(u => u.Name)
            .NotEmpty()
            .WithMessage("required name field missing");
    }
}