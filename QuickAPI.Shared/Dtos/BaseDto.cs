using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuickAPI.Shared.Dtos;

public abstract record BaseDto
{
    [JsonPropertyName("id")]
    [JsonPropertyOrder(10)]
    public Guid Id { get; set; }

    [JsonPropertyName("code")]
    [JsonPropertyOrder(20)]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("info")]
    [JsonPropertyOrder(30)]
    public string Info { get; set; } = string.Empty;

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
