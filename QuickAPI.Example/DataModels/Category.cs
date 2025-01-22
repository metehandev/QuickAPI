using QuickAPI.Database.Attributes;
using QuickAPI.Database.Core;
using QuickAPI.Database.DataModels;

namespace QuickAPI.Example.DataModels;

[EndpointDefinition(AutomaticEndpointCreation = true,
    CrudOperation = CrudOperation.All, RequireAuthorization = true,
    CommonRole = nameof(UserRole.Admin))]
public class Category : BaseModel, ITenantModel
{
    public Guid TenantId { get; set; }
}