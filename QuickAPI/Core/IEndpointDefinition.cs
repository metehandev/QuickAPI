using QuickAPI.Database.Core;

namespace QuickAPI.Core;

public interface IEndpointDefinition : IDefinition
{
    public bool RequireAuthorization { get; set; }
    public string CommonRole { get; set; }
    public CrudOperation CrudOperation { get; set; }
    public Dictionary<HttpMethod, string> MethodRoles { get; set; } 
    public Dictionary<HttpMethod, bool> MethodAllowAnonymouses { get; set; } 
    // public int Order { get; set; }
}