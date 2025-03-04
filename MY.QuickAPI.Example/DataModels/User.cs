using System.ComponentModel.DataAnnotations;
using MY.QuickAPI.Database.Attributes;
using MY.QuickAPI.Database.Core;
using MY.QuickAPI.Database.DataModels;

namespace MY.QuickAPI.Example.DataModels;

[EndpointDefinition(AutomaticEndpointCreation = true,
    CrudOperation = CrudOperation.All, RequireAuthorization = true,
    CommonRole = nameof(UserRole.SuperAdmin))]
public class User : BaseModel, ITenantModel
{
    public Guid TenantId { get; set; }
    
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string Password { get; set; } = string.Empty;
    public UserRole UserRole { get; set; }
}