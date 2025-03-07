using MY.QuickAPI.Database.Attributes;
using MY.QuickAPI.Database.Core;
using MY.QuickAPI.Database.DataModels;

namespace MY.QuickAPI.Example.DataModels;

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
    
    public ICollection<Product>? Products { get; set; }
}