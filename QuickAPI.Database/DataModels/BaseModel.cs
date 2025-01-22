using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using QuickAPI.Database.Attributes;

namespace QuickAPI.Database.DataModels;

public abstract class BaseModel
{
    public const int MAX_STR_LEN = 1073741823;
    
    [SqlDefaultValue("newid()")]
    public Guid Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = string.Empty; 

    [StringLength(255)]
    public string? Description { get; set; }

    [SqlDefaultValue("getdate()")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;
    
    public DateTimeOffset? ModifiedAt { get; set; }
    
    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    [SqlDefaultValue("(0)")]
    public bool IsDeleted { get; set; }
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}