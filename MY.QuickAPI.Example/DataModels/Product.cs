using MY.QuickAPI.Database.Attributes;
using MY.QuickAPI.Database.Core;
using MY.QuickAPI.Database.DataModels;
using MY.QuickAPI.Example.Dtos;

namespace MY.QuickAPI.Example.DataModels;

// Example 1: Using EndpointDefinition attribute with DtoType property
[EndpointDefinition(
    AutomaticEndpointCreation = true,
    CrudOperation = CrudOperation.All, 
    RequireAuthorization = true,
    PostRole = nameof(UserRole.Admin),
    PutRole = nameof(UserRole.Admin),
    DeleteRole = nameof(UserRole.Admin),
    GetRole = nameof(UserRole.User),
    DtoType = typeof(ProductDto) // Specify the DTO type to use for endpoints
)]
public class Product : BaseModel, ITenantModel
{
    public Guid TenantId { get; set; }

    public Guid CategoryId { get; set; }

    public virtual Category? Category { get; set; }
}