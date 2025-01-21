using Microsoft.EntityFrameworkCore;
using QuickAPI.Database.Data;
using QuickAPI.Database.Services.Core;
using QuickAPI.Example.DataModels;

namespace QuickAPI.Example.Data;

public class ExampleContext : BaseContext
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    
    public ExampleContext(DbContextOptions options, ITenantProvider tenantProvider) : base(options, tenantProvider)
    {
    }
}