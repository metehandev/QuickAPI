using MY.QuickAPI.Database.Core;

namespace MY.QuickAPI.Core;

/// <summary>
/// Basic definition interface with Endpoint Definition properties
/// for Authorization and Crud Operations
/// </summary>
public interface IEndpointDefinition : IDefinition
{
    /// <summary>
    /// Set RequireAuthorization value on constructor for marking this model for Authorization requirements
    /// </summary>
    public bool RequireAuthorization { get; set; }

    /// <summary>
    /// Set CommonRole value on constructor for marking this model's Authorized role for all Crud Operations
    /// </summary>
    public string CommonRole { get; set; }

    /// <summary>
    /// Set CrudOperation value on constructor for auto generation of the marked Crud Operations for model
    /// </summary>
    public CrudOperation CrudOperation { get; set; }

    /// <summary>
    /// Add values to this dictionary to change different HTTP Method's Authorized roles
    /// </summary>
    public Dictionary<HttpMethod, string> MethodRoles { get; set; }

    /// <summary>
    /// Add values to this dictionary to mark different HTTP Methods as AllowAnonymous
    /// </summary>
    public Dictionary<HttpMethod, bool> MethodAllowAnonymouses { get; set; }
    
    /// <summary>
    /// Add default Navigation paths for DbQuery includes 
    /// </summary>
    public string[] IncludeFields { get; set; }
}