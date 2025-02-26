using QuickAPI.Shared.Dtos;

namespace QuickAPI.Example.Dtos;

/// <summary>
/// DTO for Category entity, hiding the implementation details and tenant information
/// </summary>
public record CategoryDto : BaseDto
{
    /// <summary>
    /// Count of products in this category (optional, only populated when needed)
    /// </summary>
    public int? ProductCount { get; set; }
}