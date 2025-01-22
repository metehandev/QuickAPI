using QuickAPI.Database.Attributes;
using QuickAPI.Database.Core;
using QuickAPI.Database.DataModels;

namespace QuickAPI.Example.DataModels;

[EndpointDefinition(AutomaticEndpointCreation = true,
    CrudOperation = CrudOperation.All, RequireAuthorization = true,
    PostRole = nameof(UserRole.Admin),
    PutRole = nameof(UserRole.Admin),
    DeleteRole = nameof(UserRole.Admin),
    GetRole = nameof(UserRole.User))]
public class Product : BaseModel, ITenantModel
{
    public Guid TenantId { get; set; }

    public Guid CategoryId { get; set; }

    public virtual Category? Category { get; set; }
}