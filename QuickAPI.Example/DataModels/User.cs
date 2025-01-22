using System.ComponentModel.DataAnnotations;
using QuickAPI.Database.Attributes;
using QuickAPI.Database.Core;
using QuickAPI.Database.DataModels;

namespace QuickAPI.Example.DataModels;

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