namespace WebApi.Features.Authors.Commands.UpdateAuthor;

public class UpdateAuthorRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}