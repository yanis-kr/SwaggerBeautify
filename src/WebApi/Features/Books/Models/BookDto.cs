using System.ComponentModel.DataAnnotations;

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
    [Range(1, int.MaxValue, ErrorMessage = "Book ID must be a positive integer")]
    public int Id { get; set; }
    
    /// <summary>
    /// The title of the book
    /// </summary>
    /// <example>The Great Adventure</example>
    [MaxLength(200, ErrorMessage = "Book title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// A detailed description of the book's content
    /// </summary>
    /// <example>An epic tale of adventure and discovery that will captivate readers of all ages.</example>
    [MaxLength(2000, ErrorMessage = "Book description cannot exceed 2000 characters")]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// The identifier of the author who wrote this book
    /// </summary>
    /// <example>42</example>
    [Range(1, int.MaxValue, ErrorMessage = "Author ID must be a positive integer")]
    public int AuthorId { get; set; }
    
    /// <summary>
    /// The date and time when the book was created
    /// </summary>
    /// <example>2024-02-01T09:15:00Z</example>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// The date and time when the book was last updated
    /// </summary>
    /// <example>2024-02-10T16:20:00Z</example>
    public DateTime? UpdatedAt { get; set; }
}
