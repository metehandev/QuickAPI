using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuickAPI.Shared.Dtos;

/// <summary>
/// Base record for Dto objects for QuickAPI's own Mapping and CRUD generation system. 
/// </summary>
public abstract record BaseDto
{
    /// <summary>
    /// Id property
    /// </summary>
    [JsonPropertyName("id")]
    [JsonPropertyOrder(10)]
    public Guid Id { get; init; }

    /// <summary>
    /// Name property
    /// </summary>
    [JsonPropertyName("name")]
    [JsonPropertyOrder(20)]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Description property
    /// </summary>
    [JsonPropertyName("description")]
    [JsonPropertyOrder(30)]
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Default ToString of the Dto is the Json representation of the Dto
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
