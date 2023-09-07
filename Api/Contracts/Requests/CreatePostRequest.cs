using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Requests;

public class CreatePostRequest
{
    [Required(ErrorMessage = "missing required text field")]
    public string Text { get; init; } = string.Empty;
}