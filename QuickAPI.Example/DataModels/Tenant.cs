using QuickAPI.Database.Attributes;
using QuickAPI.Database.Core;
using QuickAPI.Database.DataModels;

namespace QuickAPI.Example.DataModels;

[EndpointDefinition(AutomaticEndpointCreation = true, CommonRole = nameof(UserRole.SuperAdmin),
    CrudOperation = CrudOperation.Post | CrudOperation.Get, RequireAuthorization = true)]
public class Tenant : BaseModel
{
}