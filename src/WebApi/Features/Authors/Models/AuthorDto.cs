using System.ComponentModel.DataAnnotations;

namespace WebApi.Features.Authors.Models;

/// <summary>
/// Represents an author in the system
/// </summary>
public class AuthorDto
{
    /// <summary>
    /// The unique identifier for the author
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }
    
    /// <summary>
    /// The full name of the author
    /// </summary>
    /// <example>John Doe</example>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The email address of the author
    /// </summary>
    /// <example>john.doe@example.com</example>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// The date and time when the author was created
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// The date and time when the author was last updated
    /// </summary>
    /// <example>2024-01-20T14:45:00Z</example>
    public DateTime? UpdatedAt { get; set; }
}
