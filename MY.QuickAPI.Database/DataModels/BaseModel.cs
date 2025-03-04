using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using MY.QuickAPI.Database.Attributes;

namespace MY.QuickAPI.Database.DataModels;

/// <summary>
/// BaseModel to use in MY.QuickAPI models
/// </summary>
public abstract class BaseModel
{
    /// <summary>
    /// MAX STR LEN
    /// </summary>
    public const int MAX_STR_LEN = 1073741823;
    
    /// <summary>
    /// Primary Key for models as newid() Guid ID value
    /// </summary>
    [SqlDefaultValue("newid()")]
    public Guid Id { get; set; }

    /// <summary>
    /// Name for models
    /// </summary>
    [StringLength(100)]
    public string Name { get; set; } = string.Empty; 

    /// <summary>
    /// Description for models, nullable
    /// </summary>
    [StringLength(255)]
    public string? Description { get; set; }

    /// <summary>
    /// CreatedAt DateTimeOffset value. Default is getdate()
    /// </summary>
    [SqlDefaultValue("getdate()")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// CreatedBy value as string for username
    /// </summary>
    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;
    
    /// <summary>
    /// ModifiedAt DateTimeOffset value. Null at start, setted and updated on record changes.
    /// </summary>
    public DateTimeOffset? ModifiedAt { get; set; }
    
    /// <summary>
    /// ModifiedBy string value for username. Null at start, setted and updated on record changes.
    /// </summary>
    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    /// <summary>
    /// SoftDelete
    /// </summary>
    [SqlDefaultValue("(0)")]
    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// Json as default ToString()
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}