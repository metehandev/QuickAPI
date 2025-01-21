using QuickAPI.Database.DataModels;

namespace QuickAPI.Example.DataModels;

public class Category : BaseModel, ITenantModel
{
    public Guid TenantId { get; set; }
}