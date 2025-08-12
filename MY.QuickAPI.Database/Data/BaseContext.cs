using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MY.QuickAPI.Database.DataModels;
using MY.QuickAPI.Database.Extensions;
using MY.QuickAPI.Database.Services.Core;

namespace MY.QuickAPI.Database.Data;

/// <summary>
/// Base DbContext model to use in MY.QuickAPI layers.
/// </summary>
public class BaseContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;

    /// <summary>
    /// Exposed to EF Core query filters as a parameterized value per DbContext instance
    /// </summary>
    public Guid TenantFilterValue => _tenantProvider.GetCurrentTenantId() ?? Guid.Empty;

    /// <summary>
    /// True if tenant filter should be enabled for this context instance.
    /// </summary>
    public bool IsTenantFilterEnabled => _tenantProvider.GetCurrentTenantId().HasValue;

    /// <summary>
    /// Injected TenantProvider
    /// </summary>
    /// <param name="options"></param>
    /// <param name="tenantProvider"></param>
    public BaseContext(DbContextOptions options,
        ITenantProvider tenantProvider) : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    /// <summary>
    /// Apply Custom data annotation attributes
    /// Apply TenantId query filters
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyCustomDataAnnotations();
        if (IsTenantFilterEnabled)
        {
            ApplyTenantQueryFilters(modelBuilder);
        }
    }

    private void ApplyTenantQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var tenantModel in modelBuilder.Model.GetEntityTypes().Select(m => m.ClrType)
                     .Where(m => m.IsAssignableTo(typeof(ITenantModel))))
        {
            var expression = CreateTenantIdFilterExpression(tenantModel);
            modelBuilder.Entity(tenantModel, builder => { builder.HasQueryFilter(expression); });
        }
    }

    private LambdaExpression CreateTenantIdFilterExpression(Type tenantModelType)
    {
        // Always: m => m.TenantId == TenantFilterValue (this method will only be used when filter is enabled)
        var parameter = Expression.Parameter(tenantModelType, "m");
        var tenantIdProperty = Expression.Property(parameter, nameof(ITenantModel.TenantId));
        var contextInstance = Expression.Constant(this);
        var tenantFilterValue = Expression.Property(contextInstance, nameof(TenantFilterValue));
        var equalToTenant = Expression.Equal(tenantIdProperty, tenantFilterValue);
        return Expression.Lambda(equalToTenant, parameter);
    }

    
    /// <summary>
    /// SaveChangesAsync override to set created and modified fields
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        SetCreatedAndModifiedValues();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// SaveChanges override to set created and modified fields
    /// </summary>
    /// <returns></returns>
    public override int SaveChanges()
    {
        SetCreatedAndModifiedValues();
        return base.SaveChanges();
    }

    private void SetCreatedAndModifiedValues()
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
            return;
        var tenantEntries = addedEntries.Where(e => e.Entity is ITenantModel);
        foreach (var entry in tenantEntries)
        {
            ((ITenantModel)entry.Entity).TenantId = tenantId.Value;
        }
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

    /// <summary>
    /// Replace the model cache key factory so EF builds separate models for tenant filter on/off.
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.ReplaceService<IModelCacheKeyFactory, TenantFilterModelCacheKeyFactory>();
    }

    /// <summary>
    /// Model cache key factory that accounts for tenant filter enabled/disabled.
    /// </summary>
    private sealed class TenantFilterModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context, bool designTime)
        {
            // Keyed by context type, designTime flag, and whether tenant filter is enabled
            var filterEnabled = (context as BaseContext)?.IsTenantFilterEnabled ?? false;
            return (context.GetType(), designTime, filterEnabled);
        }
    }
}