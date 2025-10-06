using System.ComponentModel.DataAnnotations;
using WebApi.Attributes;

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
    [SwaggerProps(Example = 1)]
    [Range(1, int.MaxValue, ErrorMessage = "Author ID must be a positive integer")]
    public int Id { get; set; }
    
    /// <summary>
    /// The full name of the author
    /// </summary>
    /// <example>John Doe</example>
    [MaxLength(100, ErrorMessage = "Author name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The email address of the author
    /// </summary>
    /// <example>john.doe@example.com</example>
    [SwaggerProps(Example = "john.doe@example.com", Format = "email")]
    [MaxLength(255, ErrorMessage = "Email address cannot exceed 255 characters")]
    [EmailAddress(ErrorMessage = "Please provide a valid email address")]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// The date and time when the author was created
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    [SwaggerProps(Example = "2024-01-15T10:30:00Z", Format = "date-time", ReadOnly = true)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// The date and time when the author was last updated
    /// </summary>
    /// <example>2024-01-20T14:45:00Z</example>
    [SwaggerProps(Example = "2024-01-20T14:45:00Z", Format = "date-time")]
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Internal tracking field - hidden from API documentation
    /// </summary>
    [SwaggerProps(Hide = true)]
    public string InternalTrackingId { get; set; } = Guid.NewGuid().ToString();
}
