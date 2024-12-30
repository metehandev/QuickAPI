using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using QuickAPI.Database.Attributes;

namespace QuickAPI.Database.Models;

public class BaseModel
{
    public const int MAX_STR_LEN = 1073741823;
    
    [SqlDefaultValue("newid()")]
    public Guid Id { get; set; }

    [StringLength(100)]
    public string Code { get; set; } = string.Empty; 

    [StringLength(255)]
    public string? Info { get; set; }

    [SqlDefaultValue("getdate()")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public string CreatedBy { get; set; } = string.Empty;

    [SqlDefaultValue("(0)")]
    public bool IsDeleted { get; set; }
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}