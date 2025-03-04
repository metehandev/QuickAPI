using MY.QuickAPI.Shared.Dtos;

namespace MY.QuickAPI.Example.Dtos;

/// <summary>
/// DTO for Product entity, hiding the implementation details and tenant information
/// </summary>
public record ProductDto : BaseDto
{
    /// <summary>
    /// The ID of the category this product belongs to
    /// </summary>
    public Guid CategoryId { get; set; }
    
    /// <summary>
    /// Optional category name, populated when expanded
    /// </summary>
    public string? CategoryName { get; set; }
}