using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Models;

/// <summary>
/// Common header parameters used across all API operations
/// </summary>
public class CommonParameters
{
    /// <summary>
    /// Correlation ID for request tracking across services. If not provided, a new GUID will be automatically generated.
    /// </summary>
    [FromHeader(Name = "Correlation-Id")]
    [SwaggerProps(
        Format = "uuid",
        Example = "12345678-1234-1234-1234-123456789abc",
        Description = "Optional correlation ID for request tracking. If not provided, a new GUID will be automatically generated and returned in the response headers."
    )]
    public Guid? CorrelationId { get; set; }

    /// <summary>
    /// User context information for authentication and authorization purposes
    /// </summary>
    [FromHeader(Name = "User-Context")]
    [SwaggerProps(
        Example = "user@example.com|Admin",
        Description = "User context containing user identification and role information in the format: username|role"
    )]
    public string? UserContext { get; set; }
}

