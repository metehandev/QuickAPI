using QuickAPI.Database.Attributes;
using QuickAPI.Database.Core;
using QuickAPI.Database.DataModels;

namespace QuickAPI.Example.DataModels;

// Example 2: We'll use a custom endpoint definition class instead of attribute-based configuration
// Set AutomaticEndpointCreation to false to disable the default endpoint registration
[EndpointDefinition(
    AutomaticEndpointCreation = false,
    CrudOperation = CrudOperation.All, 
    RequireAuthorization = true,
    CommonRole = nameof(UserRole.Admin)
)]
public class Category : BaseModel, ITenantModel
{
    public Guid TenantId { get; set; }
}