using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using QuickAPI.Database.Attributes;
using QuickAPI.Database.DataModels;
using QuickAPI.Database.Extensions;
using QuickAPI.Database.Services.Core;

namespace QuickAPI.Database.Data;

public class BaseContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;

    public BaseContext(DbContextOptions options,
        ITenantProvider tenantProvider) : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyCustomDataAnnotations();
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var addedEntries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added)
            .ToList();
        
        var modifiedEntries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified)
            .ToList();

        var currentUser = _tenantProvider.GetCurrentUser();

        foreach (var entry in addedEntries)
        {
            // Set default values for new entities
            SetDefaultValues(entry);

            // Set CreatedAt for BaseModel entities
            SetCreatedValues(entry, currentUser);
        }

        foreach (var entry in modifiedEntries)
        {
            // Set ModifiedAt for BaseModel entities
            SetModifiedValues(entry, currentUser);
        }
        
        // Set TenantId for ITenantEntity entities
        var tenantId = _tenantProvider.GetCurrentTenantId();
        if (!tenantId.HasValue)
            return await base.SaveChangesAsync(cancellationToken);

        var tenantEntries = addedEntries.Where(e => e.Entity is ITenantModel);
        foreach (var entry in tenantEntries)
        {
            ((ITenantModel)entry.Entity).TenantId = tenantId.Value;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private static void SetModifiedValues(EntityEntry entry, string? currentUser)
    {
        if (entry.Entity is not BaseModel baseModel)
            return;
        baseModel.ModifiedAt = DateTimeOffset.UtcNow;
        baseModel.ModifiedBy = currentUser ?? string.Empty;
    }

    private static void SetCreatedValues(EntityEntry entry, string? currentUser)
    {
        if (entry.Entity is not BaseModel baseModel)
            return;
        baseModel.CreatedAt = DateTimeOffset.UtcNow;
        baseModel.CreatedBy = currentUser ?? string.Empty;
    }

    private static void SetDefaultValues(EntityEntry entry)
    {
        foreach (var property in entry.Entity.GetType().GetProperties())
        {
            var defaultValueAttr = property.GetCustomAttribute<SqlDefaultValueAttribute>();
            if (defaultValueAttr != null && property.GetValue(entry.Entity) == null)
            {
                property.SetValue(entry.Entity, defaultValueAttr.Value);
            }
        }
    }
}