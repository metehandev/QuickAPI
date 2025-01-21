using QuickAPI.Database.DataModels;

namespace QuickAPI.Example.DataModels;

public class User : BaseModel, ITenantModel
{
    public Guid TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole UserRole { get; set; }
}