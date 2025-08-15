using MY.QuickAPI.Database.Core;

namespace MY.QuickAPI.Database.Attributes;

/// <summary>
/// Marks an EF Core entity model so MY.QuickAPI can auto-generate CRUD endpoints
/// (Minimal APIs) for it. Optionally supports using a DTO type for input/output,
/// role-based authorization per-HTTP method, and default include paths.
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
    
    /// <summary>
    /// Overrides <see cref="CommonRole"/> for GET endpoints when non-empty.
    /// Provide a comma-separated list of role names if multiple roles are allowed.
    /// </summary>
    public string GetRole { get; set; } = string.Empty;
    /// <summary>
    /// Overrides <see cref="CommonRole"/> for POST endpoints when non-empty.
    /// Provide a comma-separated list of role names if multiple roles are allowed.
    /// </summary>
    public string PostRole { get; set; } = string.Empty;
    /// <summary>
    /// Overrides <see cref="CommonRole"/> for PUT endpoints when non-empty.
    /// Provide a comma-separated list of role names if multiple roles are allowed.
    /// </summary>
    public string PutRole { get; set; } = string.Empty;
    /// <summary>
    /// Overrides <see cref="CommonRole"/> for DELETE endpoints when non-empty.
    /// Provide a comma-separated list of role names if multiple roles are allowed.
    /// </summary>
    public string DeleteRole { get; set; } = string.Empty;
    
    /// <summary>
    /// If true, the GET endpoints are marked AllowAnonymous and do not require authentication.
    /// </summary>
    public bool AllowAnonymousGet { get; set; } = false;
    /// <summary>
    /// If true, the POST endpoints are marked AllowAnonymous and do not require authentication.
    /// </summary>
    public bool AllowAnonymousPost { get; set; } = false;
    /// <summary>
    /// If true, the PUT endpoints are marked AllowAnonymous and do not require authentication.
    /// </summary>
    public bool AllowAnonymousPut { get; set; } = false;
    /// <summary>
    /// If true, the DELETE endpoints are marked AllowAnonymous and do not require authentication.
    /// </summary>
    public bool AllowAnonymousDelete { get; set; } = false;
    
    /// <summary>
    /// Default is true. Set to false if you want Anonymous access to this model's endpoints.
    /// </summary>
    public bool RequireAuthorization { get; set; } = true;

    /// <summary>
    /// Select which CRUD operations should be created for this endpoint. Default is <see cref="CrudOperation.All"/>.
    /// </summary>
    public CrudOperation CrudOperation { get; set; } = CrudOperation.All;
    
    /// <summary>
    /// Optional DTO type to use for input/output models instead of the entity type.
    /// If set, the system will use BaseDtoEndpointDefinition&lt;T, TDto&gt; for CRUD operations.
    /// The DTO type must inherit from BaseDto and have compatible property names.
    /// </summary>
    public Type? DtoType { get; set; } = null;

    /// <summary>
    /// Default Include paths used when querying this model (e.g., navigation properties).
    /// Use when you want common EF Core <c>Include</c> expressions applied automatically.
    /// </summary>
    public string[] IncludeFields { get; set; } = [];
}
