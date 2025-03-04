using Microsoft.EntityFrameworkCore;
using MY.QuickAPI.Database.Data;
using MY.QuickAPI.Database.Services.Core;
using MY.QuickAPI.Example.DataModels;

namespace MY.QuickAPI.Example.Data;

public class ExampleContext : BaseContext
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    
    public ExampleContext(DbContextOptions options, ITenantProvider tenantProvider) : base(options, tenantProvider)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Seed Tenant data
        var testTenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        modelBuilder.Entity<Tenant>().HasData(
            new Tenant
            {
                Id = testTenantId,
                Name = "Test Tenant",
                Description = "Default test tenant for development",
                CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                CreatedBy = "System"
            }
        );
        
        // Seed User data
        var adminUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var regularUserId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminUserId,
                TenantId = testTenantId,
                Name = "Admin User",
                Description = "System administrator",
                Email = "admin@example.com",
                Password = "admin123", // In a real app, hash this password
                UserRole = UserRole.Admin,
                CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                CreatedBy = "System"
            },
            new User
            {
                Id = regularUserId,
                TenantId = testTenantId,
                Name = "Regular User",
                Description = "Standard user account",
                Email = "user@example.com",
                Password = "user123", // In a real app, hash this password
                UserRole = UserRole.User,
                CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                CreatedBy = "System"
            }
        );
        
        // Seed Category data
        var electronicsCategoryId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var booksCategoryId = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var clothingCategoryId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = electronicsCategoryId,
                TenantId = testTenantId,
                Name = "Electronics",
                Description = "Electronic devices and gadgets",
                CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                CreatedBy = "System"
            },
            new Category
            {
                Id = booksCategoryId,
                TenantId = testTenantId,
                Name = "Books",
                Description = "Books and publications",
                CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                CreatedBy = "System"
            },
            new Category
            {
                Id = clothingCategoryId,
                TenantId = testTenantId,
                Name = "Clothing",
                Description = "Apparel and accessories",
                CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                CreatedBy = "System"
            }
        );
        
        // Seed Product data
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                TenantId = testTenantId,
                CategoryId = electronicsCategoryId,
                Name = "Smartphone",
                Description = "Latest model smartphone with advanced features",
                CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                CreatedBy = "System"
            },
            new Product
            {
                Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                TenantId = testTenantId,
                CategoryId = electronicsCategoryId,
                Name = "Laptop",
                Description = "High-performance laptop for professionals",
                CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                CreatedBy = "System"
            },
            new Product
            {
                Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                TenantId = testTenantId,
                CategoryId = booksCategoryId,
                Name = "Programming Guide",
                Description = "Comprehensive guide to modern programming",
                CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                CreatedBy = "System"
            },
            new Product
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                TenantId = testTenantId,
                CategoryId = clothingCategoryId,
                Name = "T-Shirt",
                Description = "Comfortable cotton t-shirt",
                CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                CreatedBy = "System"
            },
            new Product
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                TenantId = testTenantId,
                CategoryId = clothingCategoryId,
                Name = "Jeans",
                Description = "Classic denim jeans",
                CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                CreatedBy = "System"
            }
        );
    }
}