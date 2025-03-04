using MY.QuickAPI.Database.Attributes;
using MY.QuickAPI.Database.Core;
using MY.QuickAPI.Database.DataModels;

namespace MY.QuickAPI.Example.DataModels;

[EndpointDefinition(AutomaticEndpointCreation = true, CommonRole = nameof(UserRole.SuperAdmin),
    CrudOperation = CrudOperation.Post | CrudOperation.Get, RequireAuthorization = true)]
public class Tenant : BaseModel
{
}