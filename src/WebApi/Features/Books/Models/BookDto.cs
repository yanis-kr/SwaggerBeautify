using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Features.Books.Models;

/// <summary>
/// Represents a book in the system
/// </summary>
public class BookDto
{
    /// <summary>
    /// The unique identifier for the book
    /// </summary>
    /// <example>101</example>
    [SwaggerSchema(Description = "The unique identifier for the book")]
    public int Id { get; set; }
    
    /// <summary>
    /// The title of the book
    /// </summary>
    /// <example>The Great Adventure</example>
    [SwaggerSchema(Description = "The title of the book")]
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// A detailed description of the book's content
    /// </summary>
    /// <example>An epic tale of adventure and discovery that will captivate readers of all ages.</example>
    [SwaggerSchema(Description = "A detailed description of the book's content")]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// The identifier of the author who wrote this book
    /// </summary>
    /// <example>42</example>
    [SwaggerSchema(Description = "The identifier of the author who wrote this book")]
    public int AuthorId { get; set; }
    
    /// <summary>
    /// The date and time when the book was created
    /// </summary>
    /// <example>2024-02-01T09:15:00Z</example>
    [SwaggerSchema(Description = "The date and time when the book was created")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// The date and time when the book was last updated
    /// </summary>
    /// <example>2024-02-10T16:20:00Z</example>
    [SwaggerSchema(Description = "The date and time when the book was last updated")]
    public DateTime? UpdatedAt { get; set; }
}
