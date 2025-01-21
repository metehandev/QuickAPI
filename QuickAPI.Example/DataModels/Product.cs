using QuickAPI.Database.DataModels;

namespace QuickAPI.Example.DataModels;

public class Product : BaseModel, ITenantModel
{
    public Guid TenantId { get; set; }
    
    public Guid CategoryId { get; set; }
    
    public virtual Category? Category { get; set; }
}