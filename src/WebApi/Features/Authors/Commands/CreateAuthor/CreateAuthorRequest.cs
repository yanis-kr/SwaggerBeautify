namespace WebApi.Features.Authors.Commands.CreateAuthor;

public class CreateAuthorRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}