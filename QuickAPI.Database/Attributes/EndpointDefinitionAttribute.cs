using QuickAPI.Database.Core;

namespace QuickAPI.Database.Attributes;

/// <summary>
/// Mark any model with EndpointDefinition attribute to create default CRUD endpoints for the model.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class EndpointDefinitionAttribute : Attribute
{
    /// <summary>
    /// Default is true. Set to false if you want to override and set extended endpoints for this model. 
    /// </summary>
    public bool AutomaticEndpointCreation { get; set; } = true;

    /// <summary>
    /// Set authorized role names if RequireAuthorization is true.
    /// </summary>
    public string CommonRole { get; set; } = string.Empty;
    
    public string GetRole { get; set; } = string.Empty;
    public string PostRole { get; set; } = string.Empty;
    public string PutRole { get; set; } = string.Empty;
    public string DeleteRole { get; set; } = string.Empty;
    
    public bool AllowAnonymousGet { get; set; } = false;
    public bool AllowAnonymousPost { get; set; } = false;
    public bool AllowAnonymousPut { get; set; } = false;
    public bool AllowAnonymousDelete { get; set; } = false;
    
    /// <summary>
    /// Default is true. Set to false if you want Anonymous access to this model's endpoints.
    /// </summary>
    public bool RequireAuthorization { get; set; } = true;

    /// <summary>
    /// Select which Crud Operations should be created for this Endpoint. Default is All.  
    /// </summary>
    public CrudOperation CrudOperation { get; set; } = CrudOperation.All;
}