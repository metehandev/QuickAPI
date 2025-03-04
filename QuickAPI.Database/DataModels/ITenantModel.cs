namespace QuickAPI.Database.DataModels;

/// <summary>
/// Set ITenantModel to models for Tenant support on models
/// </summary>
public interface ITenantModel
{
    /// <summary>
    /// Guid TenantId for foreign key. Create a Tenant data model to reference to use this.
    /// </summary>
    public Guid TenantId { get; set; }
}